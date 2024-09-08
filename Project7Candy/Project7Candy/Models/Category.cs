using System;
using System.Collections.Generic;

namespace Project7Candy.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategoryDescription { get; set; }

    public string? CategoryImage { get; set; }

    public string? CategoryIcon { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
