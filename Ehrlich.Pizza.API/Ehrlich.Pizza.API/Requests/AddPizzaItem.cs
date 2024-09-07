using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ehrlich.Pizza.API.Requests
{
    public class AddPizzaItem
    {
        public class Request
        {
            [Required]
            public string PizzaTypeId { get; set; } = null!;
            [Required]
            public string Size { get; set; } = null!;
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

