using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScrapeChallenge.Models;
using ScrapeChallenge.Processor;
using ScrapeChallenge.Repositories;
using Microsoft.Extensions.Logging;

namespace ScrapeChallenge.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScrapeController : ControllerBase
    {
        private readonly IPureCoUkScraper _scraper;
        private readonly DishRepository _repository;
        private readonly ILogger<ScrapeController> _logger;

        public ScrapeController(IPureCoUkScraper scraper, DishRepository repository, ILogger<ScrapeController> logger)
        {
            _scraper = scraper;
            _repository = repository;
            _logger = logger;
        }

        // POST /scrape
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ScrapeUrl urlData)
        {
            try
            {
                // validate & log request
                var dishes = await _scraper.ScrapeMenuAt(urlData.MenuUrl);
                await _repository.BulkInsertDishesAsync(dishes);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                throw;
            }
        }
    }
}
