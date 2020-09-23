using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using ScrapeChallenge.Models;

namespace ScrapeChallenge.Repositories
{
    public class DishRepository
    {
        private readonly IMongoCollection<Dish> _dishCollection;

        public DishRepository(IMongoClient mongoClient)
        {
            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

            _dishCollection = mongoClient.GetDatabase("scrapeChallenge").GetCollection<Dish>("dishes");
        }

        public async Task<bool> AddDishAsync(Dish dish, CancellationToken cancellationToken = default)
        {
            try
            {
                await _dishCollection.WithWriteConcern(WriteConcern.WMajority).InsertOneAsync(dish, new InsertOneOptions { BypassDocumentValidation = false }, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                // Log error
                return false;
            }
        }

    }
}
