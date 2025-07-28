using E_Crawl_CSharp;

class Program
{
    static async Task Main(string[] args)
    {
        ECrawler crawler = new ECrawler("https://mediamodifier.com/", 2);
        await crawler.Execute();
    }
}



