using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        //public readonly IMongoDatabase _database;
        //public readonly IMongoCollection<Product> _collection;



        public CatalogContext(IConfiguration _config)
        {
            var _client=new MongoClient(_config.GetValue<string>("DatabaseSettings:ConnectionString"));
            var _database = _client.GetDatabase(_config.GetValue<string>
                ("DatabaseSettings:DatabaseName"));

            Products = _database.GetCollection<Product>(_config.GetValue<string>
                ("DatabaseSettings:CollectionName"));

            //_database = client.GetDatabase("Productdb");

            //_collection = _database.GetCollection<Product>("Products");

            CatalogContextSeed.SeedData(Products);

        }

        public IMongoCollection<Product> Products { get; }
    }
}
