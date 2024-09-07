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
        Task<AddPizzaType.Response> AddPizzaTypeAsync(AddPizzaType.Request request);
        Task<UpdatePizzaType.Response> UpdatePizzaTypeAsync(UpdatePizzaType.Request request);

    }
    public class PizzasProvider : IPizzasProvider
    {
        private readonly PizzaPlaceDbContext _context;
        public PizzasProvider(PizzaPlaceDbContext context)
        {
            _context = context;
        }
        public async Task<AddPizzaType.Response> AddPizzaTypeAsync(AddPizzaType.Request request)
        {
            var pizzaType = new PizzaType
            {
                PizzaTypeId = request.PizzaTypeId,
                Name = request.Name,
                Category = request.Category,
                Ingredients = request.Ingredients,
            };
            _context.PizzaTypes.Add(pizzaType);
            int result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new AddPizzaType.Response()
                {
                    Success = true,
                };
            }
            return new AddPizzaType.Response()
            {
                Success = false,
            };
        }
        public async Task<UpdatePizzaType.Response> UpdatePizzaTypeAsync(UpdatePizzaType.Request request)
        {
            var pizza = await _context.PizzaTypes.FindAsync(request.PizzaTypeId);
            if (pizza == null)
            {
                return new UpdatePizzaType.Response
                {
                    Success = false,
                    Message = "PizzaTypeId does not exist in Database"
                };
            }

            pizza.Name = request.Name;
            pizza.Category = request.Category;
            pizza.Ingredients = request.Ingredients;

            int result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new UpdatePizzaType.Response()
                {
                    Success = true,
                    Message = "PizzaTypeId updated successfully",
                };
            }
            return new UpdatePizzaType.Response()
            {
                Success = false,
                Message = "PizzaTypeId update failed",
            };
        }
        public async Task<GetPizzaInfo.Response> GetPizzaInfoAsync(string id)
        {
            List<Models.Pizza> pizzas = new List<Models.Pizza> { };
            var pizzaQuery = _context.Pizzas.Include(o => o.PizzaType).AsQueryable();
            if (id == null)
            {
                pizzas = await pizzaQuery.ToListAsync();
                return new GetPizzaInfo.Response(){
                    Pizzas = pizzas,
                };
            }

            pizzas = await pizzaQuery.Where(o => o.PizzaId == id).ToListAsync();
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
