using Ehrlich.Pizza.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ehrlich.Pizza.API.Requests
{
    public class GetProfit
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
            public string EndTime { get; set; } = "23:59:59";
        }

        public class Response
        {
            public float? TotalProfit { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}
