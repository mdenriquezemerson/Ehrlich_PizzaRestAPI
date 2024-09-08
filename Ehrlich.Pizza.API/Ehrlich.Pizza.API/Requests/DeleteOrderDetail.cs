using Microsoft.AspNetCore.Mvc;

namespace Ehrlich.Pizza.API.Requests
{
    public class DeleteOrderDetail
    {
        public class Response
        {
            public bool Success { get; set; }
            public ActionResult? Result { get; set; } = new OkResult();
        }
    }
}
