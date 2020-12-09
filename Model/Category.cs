using System;
using System.Collections.Generic;

namespace NorthwindConsole.Model
{
    public partial class Categories
    {
        public Categories()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}