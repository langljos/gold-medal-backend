using gold_medal_backend.Hubs;
using gold_medal_backend.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultSQLiteConnection")));
builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "Hubs",
                    cors =>
                    {
                        cors
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(builder.Configuration.GetSection("allowedOrigins").Get<string[]>())
                            .AllowCredentials();
                    });
            });
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.MapHub<MedalsHub>("/medalsHub");

app.UseCors("Hubs");

app.Run();
