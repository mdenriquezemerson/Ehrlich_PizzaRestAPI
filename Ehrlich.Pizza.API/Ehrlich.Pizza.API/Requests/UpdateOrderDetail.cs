using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ehrlich.Pizza.API.Requests
{
    public class UpdateOrderDetail
    {
        public class Request
        {
            [Required]
            public long OrderDetailsId { get; set; }
            [Required]
            public long OrderId { get; set; }
            [Required]
            public string PizzaId { get; set; }
            [Required]
            public int Quantity { get; set; }
        }
        public class Response
        {
            public bool Success { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}
