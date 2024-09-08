using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ehrlich.Pizza.API.Requests
{
    public class AddOrder
    {
        public class Response
        {
            public bool Success { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}

