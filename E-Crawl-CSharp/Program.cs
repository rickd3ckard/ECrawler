using E_Crawl_CSharp;

class Program
{
    static async Task Main(string[] args)
    {
        ECrawler crawler = new ECrawler("https://www.immoweb.be/", -1, 10);
        await crawler.Execute();
    }
}



