using AuthUser.Application.DTOs.Authentication;
using AuthUser.Application.Enums;
using AuthUser.Application.Exceptions;
using AuthUser.Application.Interfaces.Repositories;
using AuthUser.Application.Interfaces.Services;
using AuthUser.Application.Wrappers;
using AuthUser.Domain;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthUser.Infrastructure.Shared.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepositoryAsync _UserRepository;
        private readonly ILoginLogRepositoryAsync _LoginLogRepository;
        private readonly ILoginLogTypeRepositoryAsync _LoginLogTypeRepository;
        private readonly IUserTokenRepositoryAsync _UserTokenRepository;
        private readonly IPasswordService _PasswordService;
        private readonly JwtConfig _jwtSettings;

        public AuthenticationService(IUserRepositoryAsync userRepository, IOptions<JwtConfig> jwtSettings, IPasswordService passwordService, ILoginLogRepositoryAsync loginLogRepository, ILoginLogTypeRepositoryAsync loginLogTypeRepository, IUserTokenRepositoryAsync userTokenRepository)
        {
            _UserRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
            _PasswordService = passwordService;
            _LoginLogRepository = loginLogRepository;
            _LoginLogTypeRepository = loginLogTypeRepository;
            _UserTokenRepository = userTokenRepository;
        }

        public virtual async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            List<ValidationFailure> errorMessages = new List<ValidationFailure>();

            var user = (await _UserRepository.FindByCondition(x => x.UserName.ToLower() == request.UserName.ToLower()).ConfigureAwait(false)).AsQueryable().FirstOrDefault();

            LoginLog log = new LoginLog()
            {
                LoginLogId = Guid.NewGuid(),
                LoginDate = DateTime.UtcNow,
                LoginUserIP = ipAddress,
                UserIdentifier = request.UserName,
            };

            if (user == null)
            {
                errorMessages.Add(new ValidationFailure("UserName", $"No Accounts Registered with {request.UserName}."));
                log.LoginSuccess = false;
                log.LoginLogTypes = (await _LoginLogTypeRepository.FindByCondition(x => x.LoginLogTypeName == LoginLogTypeId.IncorrectUserNameOrPassword.ToString()).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
                await this._LoginLogRepository.AddAsync(log).ConfigureAwait(false);
            }

            var isBase64String = this._PasswordService.IsBase64String(request.Password);
            if (!isBase64String)
            {
                errorMessages.Add(new ValidationFailure("Password", $"Password is not base64 encoded."));
            }

            if (user != null)
            {
                var isPasswordValid = this._PasswordService.VerifyPasswordHash(request.Password, Convert.FromBase64String(user.PasswordHash), Convert.FromBase64String(user.PasswordSalt));
                if (!isPasswordValid)
                {
                    errorMessages.Add(new ValidationFailure("Password", $"Password is Incorrect."));
                    log.LoginSuccess = false;
                    log.LoginLogTypes = (await _LoginLogTypeRepository.FindByCondition(x => x.LoginLogTypeName == LoginLogTypeId.IncorrectUserNameOrPassword.ToString()).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
                    await this._LoginLogRepository.AddAsync(log).ConfigureAwait(false);
                }
            }

            if (errorMessages.Count > 0) throw new ValidationException(errorMessages);

            log.LoginSuccess = true;
            log.LoginLogTypes = (await _LoginLogTypeRepository.FindByCondition(x => x.LoginLogTypeName == LoginLogTypeId.Success.ToString()).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
            await this._LoginLogRepository.AddAsync(log).ConfigureAwait(false);

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            var JwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var userToken = GenerateRefreshToken(ipAddress);
            userToken.JwtToken = JwtToken;
            userToken.UserId = user.UserId;

            user.UserTokens.Add(userToken);
            await _UserRepository.UpdateAsync(user).ConfigureAwait(false);

            AuthenticationResponse response = new AuthenticationResponse()
            {
                Id = user.UserId.ToString(),
                Token = JwtToken,
                Email = user.UserEmail,
                UserType = user.UserType,
                RegSource = user.RegSource,
                UserName = user.UserName,
                UserStatus = user.UserStatuses.StatusValue,
                RefreshToken = userToken.Token,
            };

            return new Response<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }

        public virtual async Task<Response<string>> ForgotPassword(ForgotPasswordRequest request)
        {
            List<ValidationFailure> errorMessages = new List<ValidationFailure>();
            try
            {
                var user = (await _UserRepository.FindByCondition(x => x.UserName.ToLower() == request.UserName.ToLower()).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
                if (user == null || user.UserStatuses.StatusValue != "Active")
                {
                    errorMessages.Add(new ValidationFailure("UserName", "Please check your email for password reset instructions"));
                }

                if (errorMessages.Count > 0) throw new ValidationException(errorMessages);

                var generatedPassword = this._PasswordService.GeneratePassword(8);
                var generatedPasswordEnc = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(generatedPassword));
                string passwordhash, passwordSalt;
                _PasswordService.CreatePasswordHash(generatedPasswordEnc, out passwordhash, out passwordSalt);

                user.PasswordHash = passwordhash;
                user.PasswordSalt = passwordSalt;

                await _UserRepository.UpdateAsync(user).ConfigureAwait(false);

                //return new Response<string>($"Please check your email for password reset instructions");
                return new Response<string>(generatedPassword);
            }
            catch(Exception ex)
            {
                return new Response<string>("Error " + ex);
            }
        }

        public virtual async Task<Response<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
        {
            List<ValidationFailure> errorMessages = new List<ValidationFailure>();

            var userToken = (await _UserTokenRepository.FindByCondition(x => x.Token == request.Token).ConfigureAwait(false)).AsQueryable().FirstOrDefault();

            if (userToken == null)
            {
                errorMessages.Add(new ValidationFailure("token", $"refresh token does not exist."));

            }

            if (userToken != null && !userToken.IsActive)
            {
                errorMessages.Add(new ValidationFailure("token", $"refresh token has expired."));
            }

            if (errorMessages.Count > 0) throw new ValidationException(errorMessages);
            var user = (await _UserRepository.FindByCondition(x => x.UserId == userToken.UserId).ConfigureAwait(false)).AsQueryable().FirstOrDefault();

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            var JwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var userTokenNew = GenerateRefreshToken(ipAddress);
            userTokenNew.JwtToken = JwtToken;
            userTokenNew.UserId = user.UserId;
            user.UserTokens.Where(x => x.Token == request.Token).ToList().ForEach(x => x.ReplacedByToken = userTokenNew.Token);

            user.UserTokens.Add(userTokenNew);

            await _UserRepository.UpdateAsync(user).ConfigureAwait(false);

            AuthenticationResponse response = new AuthenticationResponse()
            {
                Id = user.UserId.ToString(),
                Token = JwtToken,
                Email = user.UserEmail,
                UserName = user.UserName,
                UserStatus = user.UserStatuses.StatusValue,
                RefreshToken = userTokenNew.Token,
            };

            return new Response<AuthenticationResponse>(response, $"Refresh Token Successful.");
        }

        public virtual async Task<Response<string>> ResetPassword(ResetPasswordRequest request)
        {
            List<ValidationFailure> errorMessages = new List<ValidationFailure>();
            try
            { 
                var user = (await _UserRepository.FindByCondition(x => x.UserName.ToLower() == request.UserName.ToLower()).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
                var userTok = (await _UserTokenRepository.FindByCondition(x => x.UserId == user.UserId && x.JwtToken == request.Token).ConfigureAwait(false)).AsQueryable();

                if (user == null || user.UserStatuses.StatusValue != "Active")
                {
                    errorMessages.Add(new ValidationFailure("UserName", "Please check your email for password reset instructions"));
                }

                if (userTok.Count() == 0)
                {
                    errorMessages.Add(new ValidationFailure("Token", "Incorrect Token"));
                }

                if (errorMessages.Count > 0) throw new ValidationException(errorMessages);

                var generatedPassword = request.Password;
                string passwordhash, passwordSalt;
                _PasswordService.CreatePasswordHash(generatedPassword, out passwordhash, out passwordSalt);

                user.PasswordHash = passwordhash;
                user.PasswordSalt = passwordSalt;

                await _UserRepository.UpdateAsync(user).ConfigureAwait(false);

                return new Response<string>(generatedPassword);
            }
            catch (Exception ex)
            {
                return new Response<string>("Error " + ex);
            }
        }

        public virtual async Task<Response<string>> RevokeTokenAsync(RevokeTokenRequest request, string ipAddress)
        {
            List<ValidationFailure> errorMessages = new List<ValidationFailure>();

            var userToken = (await _UserTokenRepository.FindByCondition(x => x.Token == request.Token).ConfigureAwait(false)).AsQueryable().FirstOrDefault();

            if (userToken == null)
            {
                errorMessages.Add(new ValidationFailure("token", $"refresh token does not exist."));

            }

            if (userToken != null && !userToken.IsActive)
            {
                errorMessages.Add(new ValidationFailure("token", $"refresh token has expired."));
            }

            if (errorMessages.Count > 0) throw new ValidationException(errorMessages);

            userToken.RevokedDate = DateTime.UtcNow;
            userToken.RevokedByIp = ipAddress;
            await _UserTokenRepository.UpdateAsync(userToken).ConfigureAwait(false);

            return new Response<string>($"Revoke Token Successful.");
        }

        private Task<JwtSecurityToken> GenerateJWToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.UserEmail),
                new Claim("uid", user.UserId.ToString()),
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return Task.FromResult(jwtSecurityToken);
        }

        private UserToken GenerateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new UserToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifetimeInMin),
                    CreatedDate = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}