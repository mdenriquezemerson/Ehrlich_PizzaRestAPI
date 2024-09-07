using Microsoft.AspNetCore.Mvc;
using Ehrlich.Pizza.API.Models;

namespace Ehrlich.Pizza.API.Requests
{
    public class GetOrderAmount
    {
        public class Query
        {
            //If no value provided, set the start date to minimum possible value
            public DateTime StartDate { get; set; } = DateTime.MinValue;
            //If no value provided, set the end date to today.
            public DateTime EndDate { get; set; } = DateTime.Today;
            //if no value provided, set the start time to midnight
            public string StartTime { get; set; } = "00:00:00";
            //If no value provided, set the end time to time now
            public string EndTime { get; set; } = DateTime.Now.ToString("HH:mm:ss");
        }

        public class Response
        {
            public int OrderAmount { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}
