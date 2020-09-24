using System.Collections.Generic;
using System.Threading.Tasks;
using ScrapeChallenge.Models;

namespace ScrapeChallenge.Processor
{
    public interface IPureCoUkScraper
    {
        Task<List<Dish>> ScrapeMenuAt(string url);
    }
}