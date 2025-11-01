using BlockedCountriesManagement.BackgroundServices;
using BlockedCountriesManagement.Service;
using BlockedCountriesManagement.Service.BlockedCountries;

namespace BlockedCountriesManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IBlockedCountriesService, BlockedCountriesService>();
            builder.Services.AddSingleton<IAuditLogService, AuditLogService>();

            builder.Services.AddHttpClient<IGeolocationService, GeolocationService>((serviceProvider, client) =>
            {
                string baseUrl = builder.Configuration["GeolocationApi:BaseUrl"]
                                 ?? throw new InvalidOperationException("GeolocationApi:BaseUrl not configured.");

                client.BaseAddress = new Uri(baseUrl);
            });
            builder.Services.AddHostedService<TemporalBlockCleanupService>();

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

            app.Run();
        }
    }
}
