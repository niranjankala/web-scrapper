namespace CrawlyScraper.App
{
    public class Category
    {
        public string CategoryId { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }        
        public string Top { get; set; }
        public string Columns { get; set; }
        public string SortOrder { get; set; }
        public string ImageDownloadUrl { get; set; }
    }

}
