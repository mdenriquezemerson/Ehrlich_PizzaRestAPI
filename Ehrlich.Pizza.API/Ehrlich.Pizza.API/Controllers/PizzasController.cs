using Ehrlich.Pizza.API.Providers;
using Ehrlich.Pizza.API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Ehrlich.Pizza.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PizzasController : Controller
    {
        private readonly IPizzasProvider _provider;
        public PizzasController(IPizzasProvider provider)
        {
            _provider = provider;
        }

        //returns 
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? id)
        {
            var result = await _provider.GetPizzaInfoAsync(id);
            return result.Result;
        }

        [HttpGet("Price")]
        public async Task<IActionResult> GetPizzaPrice([FromQuery] GetPizzaPrice.Query query)
        {
            var result = await _provider.GetPizzaPriceAsync(query);
            return result.Result;
        }

        [HttpPost("PizzaType")]
        public async Task<IActionResult> AddPizzaType([FromQuery] AddPizzaType.Request request)
        {
            var result = await _provider.AddPizzaTypeAsync(request);
            return result.Result;
        }

        [HttpPut("PizzaType")]
        public async Task<IActionResult> UpdatePizzaType([FromQuery] UpdatePizzaType.Request request)
        {
            var result = await _provider.UpdatePizzaTypeAsync(request);
            return result.Result;
        }

        [HttpPost("PizzaItem")]
        public async Task<IActionResult> AddPizzaItem([FromQuery] AddPizzaItem.Request request)
        {
            var result = await _provider.AddPizzaItemAsync(request);
            return result.Result;
        }

        [HttpPut("PizzaItemPrice")]
        public async Task<IActionResult> UpdatePizzaItemPrice([FromQuery] UpdatePizzaItemPrice.Request request)
        {
            var result = await _provider.UpdatePizzaItemPriceAsync(request);
            return result.Result;
        }
    }
}
