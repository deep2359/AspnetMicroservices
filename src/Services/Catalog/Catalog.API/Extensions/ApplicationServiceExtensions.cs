using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Catalog.API.Extensions
{
    public static class ApplicationServiceExtensions
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IMongoClient, MongoClient>(s =>
            {
                return new MongoClient(config.GetValue<string>("DatabaseSettings:ConnectionString"));
            });
            //services.AddSingleton<IMongoClient, MongoClient>(s =>
            //{
            //    return new MongoClient(config.GetValue<string>("DatabaseSettings:DatabaseName"));
            //});
            //services.AddSingleton<IMongoClient, MongoClient>(s =>
            //{
            //    return new MongoClient(config.GetValue<string>("DatabaseSettings:CollectionName"));
            //});

            return services;
        }

    }
}
