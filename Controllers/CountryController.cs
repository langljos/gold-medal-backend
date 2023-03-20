using System.Collections.Generic;
using System.Threading.Tasks;
using gold_medal_backend.Models;
using gold_medal_backend.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace gold_medal_backend.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ILogger<CountryController> _logger;
        private readonly DataContext _dataContext;

        private readonly ICountryRepository _countryRepository;

        public CountryController(ILogger<CountryController> logger, DataContext db, ICountryRepository countryRepository)
        {
            _dataContext = db;
            _logger = logger;
            
            _countryRepository = countryRepository;
        }

        [HttpGet("{id}")]
        public Task<Country?> GetSpecificCountry(int id)
        {
            return _countryRepository.GetSpecificCountry(id);
        }

        [HttpGet]
        public Task<IEnumerable<Country>> GetAllCountries()
        {
            return _countryRepository.GetAllCountries();
        }

        [HttpPost]
        public IActionResult CreateNewCountry([FromBody] CountryDto country)
        {
            
            var result = _countryRepository.CreateNewCountry(country);
             _logger.LogInformation("<<<<<<<<<<<<<<<<<<<Result: {0}", result);
            return Created($"/api/country/{result}", country);
        }

        [HttpDelete("{id}")]
        public Task<NoContentResult> DeleteCountry(int id)
        {
            _countryRepository.DeleteCountry(id);
            return Task.FromResult(NoContent());
        }

        [HttpPatch("{id}"), ProducesResponseType(typeof(Country), 204)]
        // update country (specific fields)
        public IActionResult PatchCountry(int id, [FromBody] JsonPatchDocument<Country> patch)
        {
            var isTrue = _countryRepository.PatchCountry(id, patch).Result;
            if (isTrue)
            {
                return NoContent();
            }
            return NotFound();
        }

    }
}