using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
       
        public CatalogContext(IConfiguration _config)
        {
            var _client=new MongoClient(_config.GetValue<string>("DatabaseSettings:ConnectionString"));
            var _database = _client.GetDatabase(_config.GetValue<string>
                ("DatabaseSettings:DatabaseName"));

            Products = _database.GetCollection<Product>(_config.GetValue<string>
                ("DatabaseSettings:CollectionName"));           

            CatalogContextSeed.SeedData(Products);

        }

        public IMongoCollection<Product> Products { get; }
    }
}
