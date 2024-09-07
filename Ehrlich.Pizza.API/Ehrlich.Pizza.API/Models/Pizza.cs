using System;
using System.Collections.Generic;

namespace Ehrlich.Pizza.API.Models;

public partial class Pizza
{
    public string? PizzaId { get; set; }

    public string? PizzaTypeId { get; set; }

    public string? Size { get; set; }

    public float? Price { get; set; }
}
