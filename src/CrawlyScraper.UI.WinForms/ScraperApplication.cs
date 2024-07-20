using CrawlyScraper.Core.Interfaces;
using CrawlyScraper.Core.Services;
using CrawlyScraper.Framework;
using CrawlyScraper.Infrastructure;
using CrawlyScraper.Infrastructure.Repositories;
using CrawlyScraper.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CrawlyScraper.UI.WinForms
{
    public class ScraperApplication : ApplicationBase
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddHttpClient();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IImageDownloader, ImageDownloader>();
            services.AddTransient<IExcelGenerator, ExcelGenerator>();
            services.AddSingleton<IQueueManager<Func<Task>>, QueueManager<Func<Task>>>();
            services.AddTransient<IScraperService, ScraperService>();
            services.AddTransient<ScrapDataForm>();
        }

        public override void Run()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = ServiceProvider.GetRequiredService<ScrapDataForm>();
            Application.Run(mainForm);
        }
    }    
}