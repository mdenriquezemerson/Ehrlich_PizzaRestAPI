using Microsoft.AspNetCore.Mvc;
using Ehrlich.Pizza.API.Models;

namespace Ehrlich.Pizza.API.Requests
{
    public class GetOrders
    {
        public class Query
        {
            public long? OrderId { get; set; }
            //If no value provided, set the start date to minimum possible value
            public DateTime StartDate { get; set; } = DateTime.MinValue;
            //If no value provided, set the end date to today.
            public DateTime EndDate { get; set; } = DateTime.Today;
            //if no value provided, set the start time to midnight
            public string StartTime { get; set; } = "00:00:00";
            //If no value provided, set the end time to time now
            public string EndTime { get; set; } = "23:59:59";
            //If no value provided, set Page Size to 1000
            public int PS { get; set; } = 1000;
            //If no value provided, set Page Number to 1
            public int PN { get; set; } = 1;
        }

        public class Response
        {
            public List<Order>? Orders { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}
