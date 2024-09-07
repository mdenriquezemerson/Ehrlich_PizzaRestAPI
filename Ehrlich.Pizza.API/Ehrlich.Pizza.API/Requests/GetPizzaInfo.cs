﻿using Ehrlich.Pizza.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ehrlich.Pizza.API.Requests
{
    public class GetPizzaInfo
    {
        public class Response
        {
            public List<PizzaType>? Pizzas { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}
