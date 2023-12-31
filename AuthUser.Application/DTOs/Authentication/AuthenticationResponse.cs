﻿namespace AuthUser.Application.DTOs.Authentication
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string UserStatus { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string RegSource { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}