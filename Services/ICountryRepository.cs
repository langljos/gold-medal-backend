using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gold_medal_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using gold_medal_backend.Hubs;
using Microsoft.AspNetCore.JsonPatch;
using gold_medal_backend.Controllers;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;

namespace gold_medal_backend.Services
{
    public interface ICountryRepository
    {
        Task<Country?> GetSpecificCountry(int id);
        Task<IEnumerable<Country>> GetAllCountries();
        Task<int> CreateNewCountry([FromBody] CountryDto country);
        void DeleteCountry(int id);
        Task<bool> PatchCountry(int id, [FromBody] JsonPatchDocument<Country> patch);
    }
    public class CountryRepository: ICountryRepository
    {
         private readonly DataContext _dataContext;
         private readonly IHubContext<MedalsHub> _hubContext;
         private readonly ILogger<CountryRepository> _logger;

         public CountryRepository(DataContext context, IHubContext<MedalsHub> hubContext, ILogger<CountryRepository> logger)
         {
            _dataContext = context;
            _hubContext = hubContext;
            _logger = logger;
         }

         public async Task<Country?> GetSpecificCountry(int id)
         {
            return await _dataContext.Country.FindAsync(id).AsTask();
         }

         public async Task<IEnumerable<Country>> GetAllCountries()
         {
            return await _dataContext.Country.ToArrayAsync();
         }

         public async Task<int> CreateNewCountry([FromBody] CountryDto countryDto)
         {
            var result = await _dataContext.AddCountry(new Country
            {
                Name = countryDto.Name,
                GoldMedalCount = countryDto.GoldMedalCount,
                SilverMedalCount = countryDto.SilverMedalCount,
                BronzeMedalCount = countryDto.BronzeMedalCount
            });
            
            await _hubContext.Clients.All.SendAsync("ReceiveAddMessage", result);
            return result.Id;
         }

         public async void DeleteCountry(int id)
        {
            await _dataContext.DeleteCountry(id);
            await _hubContext.Clients.All.SendAsync("ReceiveDeleteMessage", id);
        }

        public async Task<bool> PatchCountry(int id, [FromBody] JsonPatchDocument<Country> patch)
        {
            var country = await _dataContext.PatchCountry(id, patch);
            var isTrue = false;
            if (country is not null)
            {
                await _hubContext.Clients.All.SendAsync("ReceivePatchMessage", country);
                return isTrue = true;
            }
            return isTrue;
        }
    }
}