using Ehrlich.Pizza.API.Models;
using Ehrlich.Pizza.API.Requests;
using Microsoft.EntityFrameworkCore;
using static Ehrlich.Pizza.API.Requests.GetPizzaInfo;

namespace Ehrlich.Pizza.API.Providers
{
    public interface IPizzasProvider
    {
        Task<GetPizzaInfo.Response> GetPizzaInfoAsync(string? id);
        Task<GetPizzaPrice.Response> GetPizzaPriceAsync(GetPizzaPrice.Query query);

    }
    public class PizzasProvider : IPizzasProvider
    {
        private readonly PizzaPlaceDbContext _context;
        public PizzasProvider(PizzaPlaceDbContext context)
        {
            _context = context;
        }
        public async Task<GetPizzaInfo.Response> GetPizzaInfoAsync(string id)
        {
            List<PizzaType> pizzas = new List<PizzaType> { };
            var pizzaQuery = _context.PizzaTypes.AsQueryable();
            if (id == null)
            {
                pizzas = await pizzaQuery.ToListAsync();
                return new GetPizzaInfo.Response(){
                    Pizzas = pizzas,
                };
            }

            pizzas = await pizzaQuery.Where(o => o.PizzaTypeId == id).ToListAsync();
            return new GetPizzaInfo.Response()
            {
                Pizzas = pizzas,
            };
        }

        public async Task<GetPizzaPrice.Response> GetPizzaPriceAsync(GetPizzaPrice.Query query)
        {
            var pizzaPrice = 0f;
            var pizzaItem = await _context.Pizzas.Where(o => o.PizzaTypeId == query.PizzaTypeId && o.Size == query.Size).FirstOrDefaultAsync();
            if (pizzaItem != null)
            {
                pizzaPrice = pizzaItem.Price ?? 0f;
            }
            return new GetPizzaPrice.Response()
            {
                Price = pizzaPrice,
            };
        }
    }
}
