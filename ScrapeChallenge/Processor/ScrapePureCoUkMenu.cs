using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;
using ScrapeChallenge.Models;

namespace ScrapeChallenge.Processor
{
    public class ScrapePureCoUkMenu : IPureCoUkScraper
    {
        [DebuggerDisplay("Content: {Title} Url: {Url}")]
        public class MenuAnchor
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public override string ToString() => $"Title: {Title} \nURL: {Url}";
        }

        [DebuggerDisplay("Content: {Title} Id: {Id}")]
        public class MenuSection
        {
            public string Title { get; set; }
            public string Id { get; set; }
            public override string ToString() => $"Title: {Title} \nId: {Id}";
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

            var options = new LaunchOptions { Headless = true };
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            using (var browser = await Puppeteer.LaunchAsync(options))
            {
                using (Page page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url);

                    var jsSelectAllMenuAnchors = @"Array.from(document.querySelectorAll('.submenu a')).map( t=> {return { title: t.innerText, url: t.href}});";
                    var jsSelectMenuDescription = @"document.querySelector('header.menu-header p').innerText;";
                    var jsSelectMenuSectionTitles = @"Array.from(document.querySelectorAll('h4.menu-title a'))
.map(item => {
    id = item.getAttribute('aria-controls');
    title = item.querySelector('span').innerText;
    return { id: id, title: title };
});";
                    var jsSelectDishName = @"document.querySelector('header h2').innerText;";
                    var jsSelectDishDescription = @"document.querySelector('article div:first-of-type p').innerText;";

                    var results = await page.EvaluateExpressionAsync<MenuAnchor[]>(jsSelectAllMenuAnchors);
                    var menuUrls = results.Where(m => !m.Url.Contains("wellbeing -boxes")).ToList();

                    foreach (var menuUrl in menuUrls)
                    {
                        var menuTitle = menuUrl.Title;
                        await page.GoToAsync(menuUrl.Url);
                        var menuDescription = await page.EvaluateExpressionAsync<string>(jsSelectMenuDescription); // .Replace("\n", " ");

                        var menuSectionTitles = await page.EvaluateExpressionAsync<MenuSection[]>(jsSelectMenuSectionTitles);
                        foreach (var item in menuSectionTitles)
                        {
                            var menuSectionTitle = item.Title;
                            var menuSectionAnchors = await page.EvaluateExpressionAsync<string[]>($"Array.from(document.querySelectorAll('#{item.Id} a')).map(t=>t.href);");

                            foreach (var dishUrl in menuSectionAnchors)
                            {
                                await page.GoToAsync(dishUrl);
                                var dishName = await page.EvaluateExpressionAsync<string>(jsSelectDishName);
                                var dishDescription = await page.EvaluateExpressionAsync<string>(jsSelectDishDescription);
                                dishList.Add(new Dish
                                {
                                    MenuTitle = menuTitle,
                                    MenuDescription = menuDescription,
                                    MenuSectionTitle = menuSectionTitle,
                                    DishName = dishName,
                                    DishDescription = dishDescription
                                });
                            }
                        }

                    }
                }
            }

            return dishList;

        }
    }
}
