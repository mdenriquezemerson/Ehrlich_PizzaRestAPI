using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ehrlich.Pizza.API.Requests
{
    public class UpdatePizzaItemPrice
    {
        public class Request
        {
            [Required]
            public string PizzaId { get; set; } = null!;
            [Required]
            public float Price { get; set; } = 0f;
        }

        public class Response
        {
            public bool Success { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}