using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ehrlich.Pizza.API.Requests
{
    public class UpdatePizzaType
    {
        public class Request
        {
            [Required]
            public string PizzaTypeId { get; set; } = null!;
            public string? Name { get; set; }
            public string? Category { get; set; }
            public string? Ingredients { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; } = null!;
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}