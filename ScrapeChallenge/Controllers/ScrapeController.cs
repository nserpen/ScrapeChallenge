using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScrapeChallenge.Models;
using PuppeteerSharp;
using ScrapeChallenge.Processor;
using ScrapeChallenge.Repositories;

namespace ScrapeChallenge.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScrapeController : ControllerBase
    {
        IPureCoUkScraper _scraper;
        DishRepository _repository;

        public ScrapeController(IPureCoUkScraper scraper, DishRepository repository)
        {
            _scraper = scraper;
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ScrapeUrl urlData)
        {
            try
            {
                // validate & log request
                var dishes = await _scraper.ScrapeMenuAt(urlData.MenuUrl);
                foreach (var dish in dishes)
                {
                    await _repository.AddDishAsync(dish);
                }

                return Ok();        
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult> Test()
        {
            // validate & log request
            var dishes = await _scraper.ScrapeMenuAt("http://www.pure.co.uk/menus/breakfast");
            foreach (var dish in dishes)
            {
                // await _repository.AddDishAsync(dish);
            }

            return Ok();
        }
    }
}
