using System;
using System.Collections.Generic;

namespace Ehrlich.Pizza.API.Models;

public partial class PizzaType
{
    public string? PizzaTypeId { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }

    public string? Ingredients { get; set; }
}
