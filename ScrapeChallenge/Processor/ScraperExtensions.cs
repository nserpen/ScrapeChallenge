using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ScrapeChallenge.Processor
{
    public static class ScraperExtensions
    {
        public static void RegisterPuppeteerScraper(this IServiceCollection servicesBuilder)
        {
            servicesBuilder.AddSingleton<IPureCoUkScraper, ScrapePureCoUkMenu>(s =>
            {
                return new ScrapePureCoUkMenu();
            });
        }
    }
}
