using System.Text.RegularExpressions;

namespace E_Crawl_CSharp
{
    public class ECrawler
    {
        public ECrawler(string DomainName, int Depth = -1, int TargetMailCount = -1)
        {
            this.DomainName = DomainName;
            this.Depth = Depth;
            this.Completed = false;
            this.Result = new List<WebsiteEmail>();
            this.VisitedURLs = new List<string>();
            this.TargetMailCount = TargetMailCount;

            Console.WriteLine("Domain name: " + this.DomainName);
            Console.WriteLine("Depth      : " + this.Depth);
            Console.WriteLine("Max Emails : " + this.TargetMailCount);
            Console.WriteLine();
        }

        public List<WebsiteEmail> Result { get; }
        public bool Completed  { get; }
        public string DomainName { get; }
        public int Depth { get; }
        public List<string> VisitedURLs { get; }
        public int TargetMailCount { get; }

        public async Task<List<string>> Execute()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Accept.ParseAdd("image/webp,*/*;q=0.8"); ;
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
            client.DefaultRequestHeaders.Referrer = new Uri("https://www.maisonsmoches.be/");

            await recursivequeue(client, this.Depth, this.DomainName);

            if (this.Result.Count > 0) {
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine();
                foreach (WebsiteEmail mail in this.Result) { Console.WriteLine(mail.Email + " -> " + mail.URL); }
                Console.ResetColor();
            }

            Console.WriteLine();

            List<string> mailsList = new List<string>();
            foreach (WebsiteEmail mail in this.Result) { mailsList.Add(mail.Email); }
            return mailsList;
        }

        private Queue<WebsiteURL> _queue = new Queue<WebsiteURL>();
        
        private async Task recursivequeue(HttpClient Client, int MaxDepth, string targetDomain)
        {
            _queue.Enqueue(new WebsiteURL(targetDomain, 1));
            this.VisitedURLs.Add(targetDomain);

            while (_queue.Count > 0)
            {
                WebsiteURL targetURL = _queue.Dequeue();                                      
                string queryresponse = string.Empty;
                try { 
                    queryresponse = await Client.GetStringAsync(targetURL.URL);
                    Console.WriteLine("[OK200] Depth: " + targetURL.Depth + "; " + targetURL.URL);
                }
                catch (Exception ex) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] Depth: " + targetURL.Depth + "; " + targetURL.URL + " -> " + ex.Message);
                    Console.ResetColor();
                    continue; 
                }
             
                if (targetURL.Depth + 1 <= MaxDepth) { getURLsFromPage(queryresponse, targetURL.Depth); }

                string[] mails = getMailsFromPage(queryresponse);
                if (mails.Length == 0) { continue; }

                foreach (string mail in mails)
                {
                    if (string.IsNullOrWhiteSpace(mail)) { continue; }
                    string cleanmail = mail;
                    if (cleanmail.StartsWith("mailto:")) { cleanmail = cleanmail.Substring(7); }
                    string[] forbiddenExtentions = [".png", ".webp", ".jepg", ".jpg"];
                    string mailExtension = cleanmail.Substring(cleanmail.LastIndexOf('.'));
                    if (forbiddenExtentions.Contains(mailExtension)) { continue; }
                    if (!this.Result.Any(e => e.Email == cleanmail)) { this.Result.Add(new WebsiteEmail(cleanmail, targetURL.URL)); }
                    if (this.TargetMailCount == Result.Count) { return; }
                }
            }
        }

        private void getURLsFromPage(string Text, int Depth) {
            Regex hrefregex = new Regex("href=[\"\\'](.*?)[\"\\']");
            MatchCollection hrefs = hrefregex.Matches(Text);

            foreach (Match match in hrefs)
            {
                string href = match.Value;
                if (href.StartsWith("//")) { continue; }
                if (href.StartsWith("href=")) { href = match.Value.ToString().Substring("href=".Length + 1, match.Value.ToString().Length - 1 - ("href=".Length + 1)); }
                if (href.StartsWith("/")) { href = this.DomainName.Substring(0, this.DomainName.Length - 1) + href; }         
                if (!href.StartsWith(this.DomainName)) { continue; }
                if (!this.VisitedURLs.Contains(href)) {
                    VisitedURLs.Add(href);
                    _queue.Enqueue(new WebsiteURL(href, Depth + 1));             
                }
            }
        }

        private string[] getMailsFromPage(string text)
        {
            Regex emailregex = new Regex(@"(?:mailto:)?([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})");
            MatchCollection mails = emailregex.Matches(text);

            List<string> emails = new List<string>();
            foreach (Match mail in mails) {emails.Add(mail.Value); }
            return emails.ToArray();
        }
    } 

    public class WebsiteURL
    {
        public WebsiteURL(string URL, int Depth)
        {
            this.URL = URL;
            this.Depth = Depth;
        }

        public string URL { get; set; }
        public int Depth { get; set; }
    }

    public class WebsiteEmail
    {
        public WebsiteEmail(string Email, string URL)
        {
            this.Email = Email;
            this.URL = URL;
        }
        
        public string Email { get; }
        public string URL { get; }
    
    }
}
