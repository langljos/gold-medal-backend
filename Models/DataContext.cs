using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace gold_medal_backend.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Country> Country { get; set; }

        public async Task<Country> AddCountry(Country country)
        {
            await this.AddAsync(country);
            await this.SaveChangesAsync();
            return country;
        }

        public async Task DeleteCountry(int id)
        {
            var country = await this.Country.FirstOrDefaultAsync(c => c.Id == id);
            if (country is null)
            {
                return;
            }
            this.Remove(country);

            await this.SaveChangesAsync();
        }

        public async Task<Country?> PatchCountry(int id, JsonPatchDocument<Country> patch)
        {
            Country? country = await this.Country.FirstOrDefaultAsync(c => c.Id == id);
            if (country is not null)
            {
                patch.ApplyTo(country);
                await this.SaveChangesAsync();
            }

            return country;
        }
    }
}