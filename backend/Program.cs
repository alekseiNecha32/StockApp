using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        string? apiKey = Environment.GetEnvironmentVariable("FINNHUB_API_KEY");

                        if (string.IsNullOrEmpty(apiKey))
                        {
                            throw new InvalidOperationException("API key is missing. Please set FINNHUB_API_KEY as an environment variable.");
                        }

                        // Add controller and CORS services
                        services.AddControllers();

                        services.AddCors(options =>
                        {
                            options.AddPolicy("AllowAll", builder =>
                            {
                                builder.AllowAnyOrigin()
                                       .AllowAnyMethod()
                                       .AllowAnyHeader();
                            });
                        });

                        services.AddScoped<ModelTrainingService>();
                        services.AddScoped<StockDataService>(_ => new StockDataService(apiKey));
                    });

                    webBuilder.Configure(app =>
                    {
                        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error");
                            app.UseHsts();
                            app.UseHttpsRedirection();
                        }

                        // ✅ Use CORS here
                        app.UseCors("AllowAll");

                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });
    }
}
