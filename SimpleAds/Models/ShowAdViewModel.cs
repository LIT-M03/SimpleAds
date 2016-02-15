using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleAds.Data;

namespace SimpleAds.Models
{
    public class ShowAdViewModel
    {
        public Ad Ad { get; set; }
        public bool ShowDeleteButton { get; set; }
    }
}