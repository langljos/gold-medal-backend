using GoldMedalBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoldMedalBackend.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ILogger<CountryController> _logger;
        private DataContext _dataContext;

        public CountryController(ILogger<CountryController> logger, DataContext db)
        {
            _dataContext = db;
            _logger = logger;
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

            return Created($"/api/country/{result.Id}", result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _dataContext.DeleteCountry(id);
            return NoContent();
        }

    }
}