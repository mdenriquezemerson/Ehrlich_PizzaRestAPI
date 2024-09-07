using Ehrlich.Pizza.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ehrlich.Pizza.API.Requests
{
    public class GetPizzaPrice
    {
        public class Query
        {
            [Required]
            public string PizzaTypeId { get; set; }
            [Required]
            public string Size { get; set; }
        }

        public class Response
        {
            public float Price { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}
