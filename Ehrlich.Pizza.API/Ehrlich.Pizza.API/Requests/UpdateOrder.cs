using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ehrlich.Pizza.API.Requests
{
    public class UpdateOrder
    {
        public class Request
        {
            [Required]
            public long OrderId { get; set; }
            [Required]
            public DateTime DateTime { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}
