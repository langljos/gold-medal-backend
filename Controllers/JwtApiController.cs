using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using gold_medal_backend.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using gold_medal_backend.Hubs;
using Microsoft.AspNetCore.Authorization;

namespace gold_medal_backend.Controllers
{
    [ApiController, Route("jwt/api/country")]
    public class JwtApiController : ControllerBase
    {
        private readonly ILogger<CountryController> _logger;
        private DataContext _dataContext;

        private readonly IHubContext<MedalsHub> _hubContext;
        public JwtApiController(ILogger<CountryController> logger, DataContext db, IHubContext<MedalsHub> hubContext)
        {
            _dataContext = db;
            _logger = logger;
            _hubContext = hubContext;
        }

        // http get entire collection
        [HttpGet]
        public IEnumerable<Country> Get()
        {
            return _dataContext.Country;
        }

        // http get specific member of collection
        [HttpGet("{id}")]
        public Country Get(int id)
        {
            return _dataContext.Country.Find(id);
        }

        // http post member to collection
        [HttpPost, ProducesResponseType(typeof(Country), 201)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Country>> Post([FromBody] Country country) {
            _dataContext.Add(country);
            await _dataContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveAddMessage", country);
            this.HttpContext.Response.StatusCode = 201;
            return country;
        } 

        // http delete member from collection
        [HttpDelete("{id}"), ProducesResponseType(typeof(Country), 204)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(int id){
            Country country = await _dataContext.Country.FindAsync(id);
            if (country == null){
                return NotFound();
            }
            _dataContext.Remove(country);
            await _dataContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveDeleteMessage", id);
            return NoContent();
        } 

        // http patch member of collection
        [HttpPatch("{id}"), ProducesResponseType(typeof(Country), 204)]
        [Authorize(Roles = "admin")]
        // update country (specific fields)
        public async Task<ActionResult> Patch(int id, [FromBody]JsonPatchDocument<Country> patch){
            Country country = await _dataContext.Country.FindAsync(id);
            if (country == null){
                return NotFound();
            }
            patch.ApplyTo(country);
            await  _dataContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceivePatchMessage", country);
            return NoContent();
        }
    }
}