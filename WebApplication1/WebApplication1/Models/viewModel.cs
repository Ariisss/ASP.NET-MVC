using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class viewModel
    {
        public List<User> Users { get; set; }
        public List<Order> Orders { get; set; }
        public LayoutModel LayoutModel { get; set; }
    }
}