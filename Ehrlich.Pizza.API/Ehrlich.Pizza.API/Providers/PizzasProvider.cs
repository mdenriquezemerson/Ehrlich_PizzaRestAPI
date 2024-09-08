using Ehrlich.Pizza.API.Models;
using Ehrlich.Pizza.API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using static Ehrlich.Pizza.API.Requests.GetPizzaInfo;

namespace Ehrlich.Pizza.API.Providers
{
    public interface IPizzasProvider
    {
        Task<GetPizzaInfo.Response> GetPizzaInfoAsync(string? id);
        Task<GetPizzaPrice.Response> GetPizzaPriceAsync(GetPizzaPrice.Query query);
        Task<AddPizzaType.Response> AddPizzaTypeAsync(AddPizzaType.Request request);
        Task<UpdatePizzaType.Response> UpdatePizzaTypeAsync(UpdatePizzaType.Request request);
        Task<AddPizzaItem.Response> AddPizzaItemAsync(AddPizzaItem.Request request);
        Task<UpdatePizzaItemPrice.Response> UpdatePizzaItemPriceAsync(UpdatePizzaItemPrice.Request request);

    }
    public class PizzasProvider : IPizzasProvider
    {
        private readonly PizzaPlaceDbContext _context;

        public PizzasProvider(PizzaPlaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        ///     Retrieves pizza information based on a given PizzaId. If no id is provided, it returns all pizzas.
        /// </summary>
        /// <param name="id">The PizzaId to search for</param>
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

        /// <summary>
        ///     Retrieves the price of a pizza based on its PizzaTypeId and size.
        /// </summary>
        /// <param name="query">Query parameters including PizzaTypeId and size</param>
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

        /// <summary>
        ///     Adds a new pizza type with specific attributes like name, category, and ingredients.
        /// </summary>
        /// <param name="request">Request containing pizza type details</param>
        public async Task<AddPizzaItem.Response> AddPizzaItemAsync(AddPizzaItem.Request request)
        {
            request.Size = request.Size.ToUpper();
            request.PizzaTypeId = request.PizzaTypeId.ToLower();

            if (!Enum.TryParse(typeof(PizzaSize), request.Size, true, out _))
            {
                return new AddPizzaItem.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Invalid pizza size. Valid sizes are S, M, L, XL, XXL."),
                };
            }

            if (request.Price <= 0)
            {
                return new AddPizzaItem.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Price must be greater than 0."),
                };
            }

            var pizza = await _context.PizzaTypes.FindAsync(request.PizzaTypeId);
            if (pizza == null)
            {
                return new AddPizzaItem.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Pizza Type ID must exist. Create one first."),
                };
            }
            var pizzaItem = new Models.Pizza
            {
                PizzaId = request.PizzaTypeId + "_" + request.Size.ToLower(),
                PizzaTypeId = request.PizzaTypeId,
                Size = request.Size,
                Price = request.Price,
            };
            _context.Pizzas.Add(pizzaItem);
            int result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new AddPizzaItem.Response()
                {
                    Success = true,
                };
            }
            return new AddPizzaItem.Response()
            {
                Success = false,
            };
        }

        /// <summary>
        ///     Updates an existing pizza type, allowing modifications to name, category, and ingredients.
        /// </summary>
        /// <param name="request">Request containing updated pizza type details</param>
        public async Task<UpdatePizzaItemPrice.Response> UpdatePizzaItemPriceAsync(UpdatePizzaItemPrice.Request request)
        {
            if (request.Price <= 0)
            {
                return new UpdatePizzaItemPrice.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Price must be greater than 0."),
                };
            }

            var pizzaItem = await _context.Pizzas.FindAsync(request.PizzaId);
            if (pizzaItem == null)
            {
                return new UpdatePizzaItemPrice.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Pizza ID does not exist."),
                };
            }
            pizzaItem.Price = request.Price;

            _context.Pizzas.Update(pizzaItem);
            int result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new UpdatePizzaItemPrice.Response()
                {
                    Success = true,
                };
            }
            return new UpdatePizzaItemPrice.Response()
            {
                Success = false,
            };
        }

        /// <summary>
        ///     Adds a new pizza item with a specific size and price, linked to an existing pizza type.
        /// </summary>
        /// <param name="request">Request containing pizza item details</param>
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

        /// <summary>
        ///     Updates the price of an existing pizza item.
        /// </summary>
        /// <param name="request">Request containing the pizza item ID and new price</param>
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
