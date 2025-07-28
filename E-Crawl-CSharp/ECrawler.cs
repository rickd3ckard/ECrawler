using System.Text.RegularExpressions;

namespace E_Crawl_CSharp
{
    public class ECrawler
    {
        public ECrawler(string DomainName, int Depth)
        {
            this.DomainName = DomainName;
            this.Depth = Depth;
            this.Completed = false;
            this.Result = new List<string>();
        }

        public List<string> Result { get; }
        public bool Completed  { get; }
        public string DomainName { get; }
        public int Depth { get; }

        public async Task Execute()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Accept.ParseAdd("image/webp,*/*;q=0.8"); ;
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
            client.DefaultRequestHeaders.Referrer = new Uri("https://rule34.xxx/");

            string queryresponse = await client.GetStringAsync(this.DomainName);
            WebsiteURL[] urlList = getURLsFromPage(queryresponse);


            foreach (WebsiteURL websiteUrl in urlList)
            {
                Console.WriteLine("[SCANNING] " + websiteUrl.URL);
                try
                {
                    queryresponse = await client.GetStringAsync(websiteUrl.URL);
                    string[] mails = getMailsFromPage(queryresponse);
                    if (mails.Length == 0) { continue; }

                    foreach (string mail in mails)
                    {
                        if (string.IsNullOrWhiteSpace(mail)) { continue; }
                        string cleanmail = mail;

                        if (cleanmail.StartsWith("mailto:")) { cleanmail = cleanmail.Substring(7); }
                        if (!this.Result.Contains(cleanmail)) {this.Result.Add(cleanmail); }
                    }
                }

                catch (Exception ex) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR]: " + ex.Message.ToString());
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (string mail in this.Result)
            {
                Console.WriteLine(mail);
            }
            Console.ResetColor();
        }

        private WebsiteURL[] getURLsFromPage(string text) {
            Regex hrefregex = new Regex("href=[\"\\'](.*?)[\"\\']");
            MatchCollection hrefs = hrefregex.Matches(text);

            List<WebsiteURL> hrefslist = new List<WebsiteURL>();
            foreach (Match match in hrefs)
            {
                string href = match.Value;
                if (href.StartsWith("href=")) { href = match.Value.ToString().Substring(6, match.Value.ToString().Length - 1 - 6); }
                if (href.StartsWith("/")) { href = this.DomainName.Substring(0, this.DomainName.Length - 1) + href; }
                if (!href.StartsWith(this.DomainName)) { continue; }
                if (!hrefslist.Any(w => w.URL == href)) { hrefslist.Add(new WebsiteURL(href)); }
            }
            return hrefslist.ToArray();
        }

        private string[] getMailsFromPage(string text)
        {
            Regex emailregex = new Regex(@"(?:mailto:)?([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})");
            MatchCollection mails = emailregex.Matches(text);

            List<string> emails = new List<string>();
            foreach (Match mail in mails)
            {
                emails.Add(mail.Value);
            }
            return emails.ToArray();
        }
    } 

    public class WebsiteURL
    {
        public WebsiteURL(string URL, bool Visited = false)
        {
            this.URL = URL;
            this.Visited = Visited;
        }

        public string URL { get; set; }
        public bool Visited { get; set; }
    }
}
