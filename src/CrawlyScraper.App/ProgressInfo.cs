using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlyScraper.App
{
    public class ProgressInfo
    {
        public ProgressInfo()
        {
                
        }
        public ProgressInfo(int value, string message)
        {
            Value = value;
            Message = message;
        }
        public string Message { get; set; }
        public int Value { get; set; }
    }
}
