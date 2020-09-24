using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ScrapeChallenge.Processor
{
    public static class ScraperExtensions
    {
        public static void RegisterPuppeteerScraper(this IServiceCollection servicesBuilder)
        {
            servicesBuilder.AddSingleton<IPureCoUkScraper, ScrapePureCoUkMenu>(s =>
            {
                var logger = s.GetRequiredService<ILogger<ScrapePureCoUkMenu>>();
                return new ScrapePureCoUkMenu(logger);
            });
        }
    }
}
