using System.Threading.Tasks;
using AuthUser.Application.Interfaces.Repositories;
using AuthUser.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;

namespace AuthUser.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var mapper = new ModelMapper();
            mapper.AddMappings(typeof(UserMap).Assembly.ExportedTypes);

            HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            var connectionString = string.Empty;
            var NHConfiguration = new Configuration();

            if (configuration.GetValue<string>("DBProvider").ToLower().Equals("mssql"))
            {
                connectionString = configuration.GetConnectionString("MSSQLConnection");
                NHConfiguration.DataBaseIntegration(c =>
                {
                    c.Dialect<MsSql2008Dialect>();
                    c.ConnectionString = connectionString;
                    c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                    c.LogFormattedSql = true;
                    c.LogSqlInConsole = true;
                });
            }
            NHConfiguration.AddMapping(domainMapping);

            var sessionFactory = NHConfiguration.BuildSessionFactory();

            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.OpenSession());

            #region Repositories

            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IUserRepositoryAsync, UserRepositoryAsync>();
            services.AddTransient<IUserStatusRepositoryAsync, UserStatusRepositoryAsync>();
            services.AddTransient<ILoginLogRepositoryAsync, LoginLogRepositoryAsync>();
            services.AddTransient<ILoginLogTypeRepositoryAsync, LoginLogTypeRepositoryAsync>();
            services.AddTransient<IUserTokenRepositoryAsync, UserTokenRepositoryAsync>();

            #endregion Repositories
        }
    }
}