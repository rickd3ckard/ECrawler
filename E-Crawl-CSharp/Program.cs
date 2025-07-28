using E_Crawl_CSharp;

class Program
{
    static async Task Main(string[] args)
    {
        ECrawler crawler = new ECrawler("https://www.maisonsmoches.be/", -1);
        await crawler.Execute();
    }
}



