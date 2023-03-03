using System.Text.Json.Serialization;
using GoldMedalBackend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultSQLiteConnection")));
builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "Hubs",
                    builder =>
                    {
                        builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins("http://localhost:3000","https://incredible-churros-bdccc4.netlify.app")
                            .AllowCredentials();
                    });
            });
builder.Services.AddSignalR();
// builder.Services.AddMvc().AddJsonOptions(options => 
// {
//     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
// });

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

app.UseCors("Hubs");

app.Run();
