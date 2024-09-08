using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ehrlich.Pizza.API.Models;

public partial class Order
{
    public long OrderId { get; set; }

    public DateOnly? Date { get; set; }

    public TimeOnly? Time { get; set; }

    [JsonIgnore]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
