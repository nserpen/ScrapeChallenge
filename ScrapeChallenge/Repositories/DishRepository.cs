using System.Collections.Generic;
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

        public async Task InsertDishAsync(Dish dish, CancellationToken cancellationToken = default)
        {
            await _dishCollection.WithWriteConcern(WriteConcern.WMajority).InsertOneAsync(dish, new InsertOneOptions { BypassDocumentValidation = false }, cancellationToken);
        }

        public async Task BulkInsertDishesAsync(IEnumerable<Dish> dishes, CancellationToken cancellationToken = default)
        {
            await _dishCollection.InsertManyAsync(dishes, new InsertManyOptions { IsOrdered = true, BypassDocumentValidation = false }, cancellationToken);
        }

    }
}
