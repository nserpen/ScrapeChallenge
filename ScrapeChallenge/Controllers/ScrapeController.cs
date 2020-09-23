using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScrapeChallenge.Models;

namespace ScrapeChallenge.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScrapeController : ControllerBase
    {
        [HttpPost]
        public void Post([FromBody] ScrapeUrl urlData)
        {
            // log request
        }
    }
}
