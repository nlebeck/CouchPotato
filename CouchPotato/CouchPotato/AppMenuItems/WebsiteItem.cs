namespace CouchPotato.AppMenuItems
{
    public class WebsiteItem : AppMenuItem
    {
        public string Url { get; set; }

        public WebsiteItem(string name, string url)
            : base(name)
        {
            Url = url;
        }
    }
}
