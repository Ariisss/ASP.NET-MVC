using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LayoutModel
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        // Add any other properties you want to include in the layout model
    }
}