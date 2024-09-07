using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ehrlich.Pizza.API.Models;

public partial class OrderDetail
{
    public long OrderDetailsId { get; set; }

    public long? OrderId { get; set; }

    public string? PizzaId { get; set; }

    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Pizza? Pizza { get; set; }
}
