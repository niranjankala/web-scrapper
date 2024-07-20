using CrawlyScraper.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlyScraper.Core.Services
{

    public interface IScraperService
    {
        Task ScrapDataAsync(string baseUrl, int pages, string targetDirectory, string excelFilePath, IProgressReporter progressReporter);
    }

}
