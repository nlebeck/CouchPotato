using CouchPotato.ButtonMappings;

namespace CouchPotato.AppMenuItems
{
    public class WebsiteItem : AppMenuItem
    {
        public string Url { get; set; }
        public ButtonMapping ButtonMapping { get; set; }

        public WebsiteItem(string name, string url, ButtonMapping buttonMapping)
            : base(name)
        {
            Url = url;
            ButtonMapping = buttonMapping;
        }
    }
}
