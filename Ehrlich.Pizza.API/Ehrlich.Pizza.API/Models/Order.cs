using System;
using System.Collections.Generic;

namespace Ehrlich.Pizza.API.Models;

public partial class Order
{
    public long OrderId { get; set; }

    public DateOnly? Date { get; set; }

    public TimeOnly? Time { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
