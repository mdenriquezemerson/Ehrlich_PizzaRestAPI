using System;
using System.Collections.Generic;

namespace Ehrlich.Pizza.API.Models;

public partial class Pizza
{
    public string PizzaId { get; set; } = null!;

    public string? PizzaTypeId { get; set; }

    public string? Size { get; set; }

    public float? Price { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual PizzaType? PizzaType { get; set; }
}
