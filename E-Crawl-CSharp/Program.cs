using E_Crawl_CSharp;

class Program
{
    static async Task Main(string[] args)
    {
        ECrawler crawler = new ECrawler("http://www.tb-immo.com/", 5);
        await crawler.Execute();
    }
}



