using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NorthwindConsole
{
    public class Categories
    {
        public int CategoryID {get; set;}
        [Required]
        public string CategoryName {get; set;}
        public string Description {get; set; }

        public List<Products> Products { get; set; }

    }
}