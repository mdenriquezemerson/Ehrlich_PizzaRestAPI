using Ehrlich.Pizza.API.Models;
using Ehrlich.Pizza.API.Requests;
using Microsoft.EntityFrameworkCore;

namespace Ehrlich.Pizza.API.Providers
{
    public interface IOrdersProvider
    {
        Task<GetOrders.Response> GetOrdersAsync(GetOrders.Query query);
    }

    public class OrdersProvider : IOrdersProvider
    {
        private readonly PizzaPlaceDbContext _context;
        public OrdersProvider(PizzaPlaceDbContext context)
        {
            _context = context;
        }

        public async Task<GetOrders.Response> GetOrdersAsync(GetOrders.Query query)
        {
            query = SanitizeQuery(query);
            int PN = query.PN ?? default(int);
            int PS = query.PS ?? default(int);
            var orders = await _context.Orders.Skip((PN - 1) * PS).Take(PS).ToListAsync();
            return new GetOrders.Response()
            {
                Orders = orders,
            };
        }

        public GetOrders.Query SanitizeQuery(GetOrders.Query query)
        {
            return query;
        }

    }
}
