using E_Crawl_CSharp;

// implement output file as json
// implement domains for a list (txt file)
// write help display function

class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Unknown command. Type 'ecrawler help' for available commands.");
            return;
        }

        Console.WriteLine(args);

        string firstArgument = args[0];
        switch (firstArgument)
        {
            case "help":
                //DisplayHelp();
                return; 
            case "domain":
                await ScrapeSingleDomain(args); return;
            case "domains":
                await ScrapeMultipleDomains(args); return;
            default:
                Console.WriteLine("Unknown command. Type 'ecrawler help' for available commands."); return;
        }     
    }

    private static async Task ScrapeSingleDomain(string[] args)
    {
        if (args.Length < 3) { Console.WriteLine("You must provide a domain name. Type 'ecrawler help' for clarification."); }
        string domainName = args[1] ?? string.Empty;

        if (FormatDomainName(ref domainName) == false)
        {
            Console.WriteLine($"The provided URL {domainName} is not properly formatted. Type 'ecrawler help' for clarification.");
            return;
        }

        try { await ScrapeDomain(domainName, args); }
        catch { Console.WriteLine("An error occured while attempting to scrape the domain: " + domainName); }
    }

    private static async Task ScrapeMultipleDomains(string[] args)
    {
        if (args.Length < 3) { Console.WriteLine("You must provide a domain list. Type 'ecrawler help' for clarification."); }
        string domainList = args[1] ?? string.Empty;
        if (!File.Exists(domainList)) { Console.WriteLine("The provided domain list file does not exist."); return; }
        if (!domainList.EndsWith(".txt")) { Console.WriteLine("The provided domain list file must be a text file (.txt)"); return; }
        Console.WriteLine(domainList);
        using (StreamReader reader = new StreamReader(domainList))
        {
            while (reader.EndOfStream == false)
            {
                string domainName = reader.ReadLine() ?? string.Empty;
                if (FormatDomainName(ref domainName) == false)
                {
                    Console.WriteLine($"The provided URL {domainName} is not properly formatted. Type 'ecrawler help' for clarification.");
                    return;
                }

                try { await ScrapeDomain(domainName, args); }
                catch { Console.WriteLine("An error occured while attempting to scrape the domain: " + domainName); }
            }
        };
    }

    private static async Task ScrapeDomain(string DomainName, string[] args)
    {
        byte buffer = 2;
        int depth = -1;
        int maxemails = -1;

        while (true)
        {
            buffer += 2; if (buffer > args.Length - 1) { break; } //6                
            string modifier = args[buffer - 1];

            switch (modifier)
            {
                case "-depth":
                case "-d":
                    depth = int.Parse(args[buffer]); break;
                case "-maxemails":
                case "-m":
                    maxemails = int.Parse(args[buffer]); break;
                default:
                    Console.WriteLine("Invalid arguements []. Type 'ecrawler help' for available arguements"); return;
            }
        }

        ECrawler crawler = new ECrawler(DomainName, depth, maxemails);
        await crawler.Execute();
        return;
    }

    private static bool FormatDomainName(ref string DomainName)
    {
        if (!DomainName.StartsWith("http://www.") && !DomainName.StartsWith("https://www.")) {return false; }
        if (DomainName.StartsWith("http://www.")) { DomainName.Replace("http://www.", "https://www."); }
        if (!DomainName.EndsWith('/')) { DomainName = DomainName + "/"; }
        return true;
    }
}