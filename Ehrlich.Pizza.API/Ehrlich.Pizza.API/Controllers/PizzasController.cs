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
            return Ok(result);
        }

        [HttpGet("Price")]
        public async Task<IActionResult> GetPizzaPrice([FromQuery] GetPizzaPrice.Query query)
        {
            var result = await _provider.GetPizzaPriceAsync(query);
            return Ok(result);
        }
        /* 
         * Endpoints:
         * Price of Pizza (Pizza type id and Size)
         * Name, Category, Ingredients of Pizza DONE
         * Pizzas per category with 
          
        */

    }
}
