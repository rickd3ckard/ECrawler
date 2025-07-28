using E_Crawl_CSharp;

class Program
{
    static async Task Main(string[] args)
    {
        ECrawler crawler = new ECrawler("https://www.immoweb.be/", 2);
        await crawler.Execute();
    }
}



