using Ehrlich.Pizza.API.Models;
using Ehrlich.Pizza.API.Requests;
using Microsoft.EntityFrameworkCore;

namespace Ehrlich.Pizza.API.Providers
{
    public interface IOrdersProvider
    {
        Task<GetOrders.Response> GetOrdersAsync(GetOrders.Query query);
        Task<GetOrderAmount.Response> GetOrderAmountAsync(GetOrderAmount.Query query);
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
            List<Order> orders = new List<Order> { };

            //OrderId is unique meaning they're only querying for one item and we should return just that if OrderId exists in the query.
            if (query.OrderId.HasValue)
            {
                var orderItem = await _context.Orders.Where(o => o.OrderId == query.OrderId).ToListAsync();
                return new GetOrders.Response()
                {
                    Orders = orders
                };
            }

            //Rest of code if OrderId is not specified and multiple items are expected.
            query = SanitizeGetOrdersQuery(query);
            var orderQuery = _context.Orders.AsQueryable();
            orderQuery = orderQuery.Where(o => o.Date >= DateOnly.FromDateTime(query.StartDate) && o.Date <= DateOnly.FromDateTime(query.EndDate));
            orderQuery = orderQuery.Where(o => o.Time >= TimeOnly.Parse(query.StartTime) && o.Time <= TimeOnly.Parse(query.EndTime));
            orderQuery = orderQuery.Skip((query.PN - 1) * query.PS).Take(query.PS);
            orders = await orderQuery.ToListAsync();
            return new GetOrders.Response()
            {
                Orders = orders,
            };
        }

        public async Task<GetOrderAmount.Response> GetOrderAmountAsync(GetOrderAmount.Query query)
        {
            query = SanitizeGetOrderAmountQuery(query);
            var orderQuery = _context.Orders.AsQueryable();
            orderQuery = orderQuery.Where(o => o.Date >= DateOnly.FromDateTime(query.StartDate) && o.Date <= DateOnly.FromDateTime(query.EndDate));
            orderQuery = orderQuery.Where(o => o.Time >= TimeOnly.Parse(query.StartTime) && o.Time <= TimeOnly.Parse(query.EndTime));
            return new GetOrderAmount.Response()
            {
                OrderAmount = orderQuery.Count(),
            };
        }

        public GetOrders.Query SanitizeGetOrdersQuery(GetOrders.Query query)
        {
            TimeOnly endTime;
            TimeOnly startTime;
            //TODO: Timezone not clarified. Specify with stakeholders.
            //If no value provided, set Page Size to 1000
            query.PS = query.PS <= 0 ? 1000 : query.PS;
            //If no value provided, set Page Number to 1
            query.PN = query.PN <= 0 ? 1 : query.PN;
            //If invalid value provided, set the end time to time now
            query.EndTime = !TimeOnly.TryParse(query.EndTime, out endTime) ? DateTime.Now.ToString("HH:mm:ss") : query.EndTime;
            //if invalid value provided, set the start time to midnight
            query.StartTime = !TimeOnly.TryParse(query.StartTime, out startTime) ? "00:00:00" : query.StartTime;

            return query;
        }

        public GetOrderAmount.Query SanitizeGetOrderAmountQuery(GetOrderAmount.Query query)
        {
            TimeOnly endTime;
            TimeOnly startTime;

            //If invalid value provided, set the end time to time now
            query.EndTime = !TimeOnly.TryParse(query.EndTime, out endTime) ? DateTime.Now.ToString("HH:mm:ss") : query.EndTime;
            //if invalid value provided, set the start time to midnight
            query.StartTime = !TimeOnly.TryParse(query.StartTime, out startTime) ? "00:00:00" : query.StartTime;

            return query;
        }
    }
}
