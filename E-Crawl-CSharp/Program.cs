using E_Crawl_CSharp;
using System.Text.Json.Nodes;

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
        
        int totalLines = File.ReadLines(domainList).Count();
        int currentline = 1;
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

                Console.WriteLine($"Progress: {currentline} of {totalLines} websites scraped so far...");
                currentline += 1;
            }
        };
    }

    private static async Task ScrapeDomain(string DomainName, string[] args)
    {
        byte buffer = 1;
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
        List<string> emailList = await crawler.Execute();
        AppendToFile(@"C:\Users\Finet Laurent\Desktop\urls.json", emailList, DomainName);
        return;
    }

    private static bool FormatDomainName(ref string DomainName)
    {
        try
        {
            DomainName = DomainName.Replace("\\", "/");
            Uri newUri = new Uri(DomainName); string root = newUri.Host;
            if (!root.StartsWith("www.")) { root = "www." + root; }
            DomainName = "https://" + root + "/";
            return true;
        }

        catch {
            Console.WriteLine("An error occurred while parsing the URI: " + DomainName);
            return false; 
        }
    }

    private static bool AppendToFile(string FullFileName, List<string> EmailsList, string DomainName)
    {
        JsonObject? newobject = new JsonObject();

        if (!File.Exists(FullFileName)) { 
            Console.WriteLine("Couldn't append the emails to the provided file: File does not exists.");

            try {File.WriteAllText(FullFileName, "{}"); } 
            catch {Console.WriteLine("Couldn't append the emails to the provided file: Could not create the file."); return false; }
        } 

        else
        {
            JsonNode? node = JsonNode.Parse(File.ReadAllText(FullFileName));
            if (node == null) { throw new Exception("New json node is null."); }
            newobject = node as JsonObject; if (newobject == null) { throw new Exception("New json object is null."); }

            if (newobject.ContainsKey(DomainName)) { Console.WriteLine("Couldn't append the emails to the provided file: Domain is already present in the list."); return false; }
        }

        JsonArray emailArray = new JsonArray();
        foreach (string mail in EmailsList) {emailArray.Add(mail); }

        newobject.Add(DomainName, emailArray);
        File.WriteAllText(FullFileName, newobject.ToString());   
        return true;
    }
}