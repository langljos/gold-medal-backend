using System.Collections.Generic;
using System.Threading.Tasks;
using gold_medal_backend.Hubs;
using gold_medal_backend.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace gold_medal_backend.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ILogger<CountryController> _logger;
        private DataContext _dataContext;
        private readonly IHubContext<MedalsHub> _hubContext;

        public CountryController(ILogger<CountryController> logger, DataContext db, IHubContext<MedalsHub> hubContext)
        {
            _dataContext = db;
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpGet("{id}")]
        public Task<Country?> Get(int id)
        {
            return _dataContext.Country.FindAsync(id).AsTask();
        }

        [HttpGet]
        public async Task<IEnumerable<Country>> Get()
        {
            return await _dataContext.Country.ToArrayAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CountryDto country)
        {
            var result = await _dataContext.AddCountry(new Country
            {
                Name = country.Name,
                GoldMedalCount = country.GoldMedalCount,
                SilverMedalCount = country.SilverMedalCount,
                BronzeMedalCount = country.BronzeMedalCount
            });

            await _hubContext.Clients.All.SendAsync("ReceiveAddMessage", country);

            return Created($"/api/country/{result.Id}", result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _dataContext.DeleteCountry(id);
            await _hubContext.Clients.All.SendAsync("ReceiveDeleteMessage", id);
            return NoContent();
        }

        [HttpPatch("{id}"), ProducesResponseType(typeof(Country), 204)]
        // update country (specific fields)
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<Country> patch)
        {
            var country = await _dataContext.PatchCountry(id, patch);
            if (country is not null)
            {
                await _hubContext.Clients.All.SendAsync("ReceivePatchMessage", country);
                return NoContent();
            }
            return NotFound();
        }

    }
}