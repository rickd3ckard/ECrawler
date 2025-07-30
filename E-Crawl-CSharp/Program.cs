using E_Crawl_CSharp;
using System.Runtime.InteropServices;

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

        string firstArgument = args[0];
        switch (firstArgument)
        {
            case "help":
                //DisplayHelp();
                return; 
            case "domain":
                await ScrapeMails(args); return;
            default:
                Console.WriteLine("Unknown command. Type 'ecrawler help' for available commands."); return;
        }     
    }

    private static async Task ScrapeMails(string[] args)
    {
        if (args.Length < 3) { Console.WriteLine("You must provide a domain name. Type 'ecrawler help' for clarification."); }
        string domainName = args[2] ?? string.Empty;

        if (FormatDomainName(ref domainName) == false)
        {
            Console.WriteLine("The provided URL is not properly formatted. Type 'ecrawler help' for clarification.");
            return;
        }

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

        ECrawler crawler = new ECrawler(domainName, depth, maxemails);
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