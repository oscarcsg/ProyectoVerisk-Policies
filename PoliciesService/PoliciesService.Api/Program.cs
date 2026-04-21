
using Microsoft.EntityFrameworkCore;
using PoliciesService.Application.Interfaces;
using PoliciesService.Infrastructure;
using Refit;

namespace PoliciesService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Get connection string from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DeaultConnection");

            // Register the DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            // Get URL of Reference Data API
            var referenceDataUrl = builder.Configuration["Services:ReferenceData"];

            // Register Refit
            builder.Services.AddRefitClient<IReferenceDataClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(referenceDataUrl!));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
