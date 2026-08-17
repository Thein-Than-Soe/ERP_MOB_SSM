using System;
using Microsoft.Maui.Controls;

namespace CS.ERP_MOB.Models.Frame
{
    public class ModelRoute
    {
        public string Title { get; set; }
        public ContentView Page { get; set; }
        public string Icon { get; set; }
        public ModelRoute(ContentView argView, string argTitle, string argIcon)
        {
            this.Page = argView;
            this.Title =argTitle;
            this.Icon = argIcon;
        }
    }
}
