using Microsoft.AspNetCore.Mvc;
using Ehrlich.Pizza.API.Models;

namespace Ehrlich.Pizza.API.Requests
{
    public class GetOrders
    {
        public class Query : Response
        {
            public long? OrderId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? StartTime { get; set; }
            public string? EndTime { get; set; }
            public int? PS { get; set; } = 1000;
            public int? PN { get; set; } = 1;
        }

        public class Response
        {
            public List<Order>? Orders { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}
