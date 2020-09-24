using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using ScrapeChallenge.Models;

namespace ScrapeChallenge.Processor
{
    public class ScrapePureCoUkMenu : IPureCoUkScraper
    {
        private readonly ILogger<ScrapePureCoUkMenu> _logger;

        public ScrapePureCoUkMenu(ILogger<ScrapePureCoUkMenu> logger)
        {
            _logger = logger;
        }

        [DebuggerDisplay("MenuTitle: {Title} Url: {Url}")]
        public class MenuAnchor
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }

        public class MenuHier
        {
            public string Url { get; set; }
            public string MenuTitle { get; set; }
            public string MenuDescription { get; set; }

            public List<MenuSection> MenuSections { get; set; }
        }

        [DebuggerDisplay("Title: {Title} Id: {Id}")]
        public class MenuSection
        {
            public string Title { get; set; }
            public string Id { get; set; }

            public List<string> DishUrls { get; set; }
        }

        public async Task<List<Dish>> ScrapeMenuAt(string url)
        {
            //var options = new LaunchOptions { Headless = true };
            //await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            //
            //var connectOptions = new ConnectOptions
            //{
            //    BrowserWSEndpoint = "wss://chrome.browserless.io"
            //};
            //using (var browser = await Puppeteer.ConnectAsync(connectOptions))

            var dishList = new List<Dish>();
            var menuHierList = new List<MenuHier>();

            var options = new LaunchOptions { Headless = true };
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            using (var browser = await Puppeteer.LaunchAsync(options))
            {
                using (Page page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url);

                    var jsSelectAllMenuAnchors = @"Array.from(document.querySelectorAll('.submenu a')).map(t=> { return { 'Title':t.innerText, 'Url':t.href };  });";
                    var jsSelectMenuDescription = @"document.querySelector('header.menu-header p').innerText;";
                    var jsSelectMenuSectionTitles = @"Array.from(document.querySelectorAll('h4.menu-title a'))
.map(item => {
    id = item.getAttribute('aria-controls');
    title = item.querySelector('span').innerText;
    return { 'Id': id, 'Title': title };
});";


                    var results = await page.EvaluateExpressionAsync<MenuAnchor[]>(jsSelectAllMenuAnchors);
                    var menuUrls = results.Where(m => !m.Url.Contains("wellbeing-boxes")).ToList();

                    foreach (var menuAnchor in menuUrls)
                    {
                        await page.GoToAsync(menuAnchor.Url);
                        var menuDescription = await page.EvaluateExpressionAsync<string>(jsSelectMenuDescription); // .Replace("\n", " ");

                        var menuHier = new MenuHier
                        {
                            Url = menuAnchor.Url,
                            MenuTitle = menuAnchor.Title,
                            MenuDescription = menuDescription,
                            MenuSections = new List<MenuSection>()
                        };

                        menuHierList.Add(menuHier);

                        var menuSections = await page.EvaluateExpressionAsync<MenuSection[]>(jsSelectMenuSectionTitles);

                        foreach (var item in menuSections)
                        {
                            var menuSection = new MenuSection
                            {
                                Title = item.Title,
                                Id = item.Id,
                                DishUrls = new List<string>()
                            };

                            menuHier.MenuSections.Add(menuSection);

                            var menuSectionDishUrls = await page.EvaluateExpressionAsync<string[]>($"Array.from(document.querySelectorAll('#{item.Id} a')).map(t=>t.href);");
                            foreach (var dishUrl in menuSectionDishUrls)
                            {
                                menuSection.DishUrls.Add(dishUrl);
                            }
                        }
                    }

                    await page.CloseAsync();

                }

                Debug.WriteLine("STARTING DISH PAGES!");

                using (Page page = await browser.NewPageAsync())
                {
                    var jsSelectDishName = @"document.querySelector('header h2').innerText;";
                    var jsSelectDishDescription = @"document.querySelector('article > div > p').innerText;";

                    // Visit each dish page

                    foreach (var menuHier in menuHierList)
                    {
                        var menuSections = menuHier.MenuSections;
                        foreach (var menuSection in menuSections)
                        {
                            var dishUrls = menuSection.DishUrls;
                            foreach (var dishUrl in dishUrls)
                            {
                                await page.GoToAsync(dishUrl);
                                string dishName = null;
                                string dishDescription = null;
                                bool hasError = false;

                                try
                                {
                                    dishName = await page.EvaluateExpressionAsync<string>(jsSelectDishName);
                                    dishDescription = await page.EvaluateExpressionAsync<string>(jsSelectDishDescription);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, ex.Message, null);
                                    hasError = true;
                                }

                                var dish = new Dish
                                {
                                    MenuTitle = menuHier.MenuTitle,
                                    MenuDescription = menuHier.MenuDescription,
                                    MenuSectionTitle = menuSection.Title,
                                    DishName = dishName,
                                    DishDescription = dishDescription,
                                    HasError = hasError
                                };

                                dishList.Add(dish);
                            }
                        }
                    }

                    await page.CloseAsync();
                }
            }

            _logger.LogInformation($"Completed {dishList.Count} dishes.");
            return dishList;

        }

    }
}
