using E_Crawl_CSharp;

//implement output file
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

        // check for domain name formatting.

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
}