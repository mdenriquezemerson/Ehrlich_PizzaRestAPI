using System;
using System.Collections.Generic;

namespace Ehrlich.Pizza.API.Models;

public partial class PizzaType
{
    public string PizzaTypeId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Category { get; set; }

    public string? Ingredients { get; set; }

    public virtual ICollection<Pizza> Pizzas { get; set; } = new List<Pizza>();
}
