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
        public string ImageDownloadUrl { get; set; } // This will not be exported but will be read
        public string DateAdded { get; set; }
        public string DateModified { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string StoreIds { get; set; }
        public string Layout { get; set; }
        public string Status { get; set; }
    }
}
