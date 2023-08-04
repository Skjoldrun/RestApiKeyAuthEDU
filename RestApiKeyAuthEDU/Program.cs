using Microsoft.OpenApi.Models;
using RestApiKeyAuthEDU.Authentication;
using RestApiKeyAuthEDU.Controllers;

namespace RestApiKeyAuthEDU
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Controller without any custom filters
            builder.Services.AddControllers();

            // Controller with added filter for EVERY controller
            //builder.Services.AddControllers(x => x.Filters.Add<ApiKeyAuthFilter>());

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Default Swagger activation
            //builder.Services.AddSwaggerGen();

            // Add API Key Swagger support
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "The API Key to access the API",
                    Type = SecuritySchemeType.ApiKey,
                    Name = AuthConstants.ApiKeyHeaderName,
                    In = ParameterLocation.Header,
                    Scheme = "ApiKeyScheme"
                });

                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header
                };

                var requirement = new OpenApiSecurityRequirement
                {
                    {scheme, new List<string>() }
                };

                c.AddSecurityRequirement(requirement);
            });

            // Register the AuthKeyFilter for single controllers
            builder.Services.AddScoped<ApiKeyAuthFilter>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Register our own API key middleware here to filter all requests
            //app.UseMiddleware<ApiKeyAuthMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            // Example for filtering with minimal API
            app.Map("Weathermini", () =>
            {
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = WeatherForecastController.Summaries[Random.Shared.Next(WeatherForecastController.Summaries.Length)]
                })
                .ToArray();
            })
                .AddEndpointFilter<ApiKeyEndpointFilter>(); // This adds the filtering for this endpoint

            app.Run();
        }
    }
}