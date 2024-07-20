using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CrawlyScraper.Framework
{
    public interface IApplication
    {
        IServiceProvider ServiceProvider { get; }
        IConfiguration Configuration { get; }
        ILogger Logger { get; }
        void Run();
    }
}