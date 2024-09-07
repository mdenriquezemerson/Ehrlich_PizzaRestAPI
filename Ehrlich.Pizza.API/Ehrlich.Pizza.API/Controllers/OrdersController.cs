using Azure;
using Ehrlich.Pizza.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ehrlich.Pizza.API.Requests;
using Ehrlich.Pizza.API.Providers;

namespace Ehrlich.Pizza.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly IOrdersProvider _provider;
        public OrdersController(IOrdersProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetOrders.Query query)
        {
            var result = await _provider.GetOrdersAsync(query);
            return Ok(result);
        }
    }
}
