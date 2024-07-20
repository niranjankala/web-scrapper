namespace CrawlyScraper.Framework
{
    public class ProgressReporter : IProgressReporter
    {
        private readonly IProgress<int> _progress;

        public ProgressReporter(IProgress<int> progress)
        {
            _progress = progress;
        }

        public void ReportProgress(int percentComplete)
        {
            _progress.Report(percentComplete);
        }
    }
}