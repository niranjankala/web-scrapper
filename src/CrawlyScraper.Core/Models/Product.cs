using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlyScraper.Core.Models
{
    public class Product
    {
        public string ProductName { get; set; }
        public string ProductLink { get; set; }
        public List<string> ProductImages { get; set; } = new List<string>();
        public List<string> DownloadImages { get; set; } = new List<string>();
        public string ProductPrice { get; set; }
        public string PriceIncludingGST { get; set; }
        public string PriceExcludingGST { get; set; }
        public string Availability { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> ProductDetails { get; set; } = new Dictionary<string, string>();
    }
}
