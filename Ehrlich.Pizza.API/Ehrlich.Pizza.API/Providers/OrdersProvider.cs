using Ehrlich.Pizza.API.Models;
using Ehrlich.Pizza.API.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ehrlich.Pizza.API.Providers
{
    public interface IOrdersProvider
    {
        Task<GetOrders.Response> GetOrdersAsync(GetOrders.Query query);
        Task<GetOrderAmount.Response> GetOrderAmountAsync(GetOrderAmount.Query query);
        Task<GetProfit.Response> GetProfitAsync(GetProfit.Query query);
        Task<AddOrder.Response> AddOrderAsync(DateTime dateTime);
        Task<UpdateOrder.Response> UpdateOrderAsync(UpdateOrder.Request request);
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

        public async Task<GetProfit.Response> GetProfitAsync(GetProfit.Query query)
        {
            float? totalProfit = 0f;
            query = SanitizeGetProfitQuery(query);
            var profitQuery = _context.OrderDetails.Include(o => o.Order).Include(o => o.Pizza).AsQueryable();
            profitQuery = profitQuery.Where(o => o.Order.Date >= DateOnly.FromDateTime(query.StartDate) && o.Order.Date <= DateOnly.FromDateTime(query.EndDate));
            profitQuery = profitQuery.Where(o => o.Order.Time >= TimeOnly.Parse(query.StartTime) && o.Order.Time <= TimeOnly.Parse(query.EndTime));
            if (query.PizzaIds != null && query.PizzaIds.Any())
            {
                profitQuery = profitQuery.Where(o => query.PizzaIds.Contains(o.PizzaId));
            }
            var orderDetails = await profitQuery.ToListAsync();
            foreach (var orderDetail in orderDetails)
            {
                totalProfit += orderDetail.Pizza.Price * orderDetail.Quantity;
            }
            return new GetProfit.Response()
            {
                TotalProfit = totalProfit,
            };
        }

        public async Task<AddOrder.Response> AddOrderAsync(DateTime dateTime)
        {
            DateOnly date = DateOnly.FromDateTime(dateTime);
            TimeOnly time = TimeOnly.FromDateTime(dateTime);

            long latestOrderId = await _context.Orders.MaxAsync(o => o.OrderId);
            long newOrderId = latestOrderId + 1;
            Order newOrder = new Order
            {
                OrderId = newOrderId,
                Date = date,
                Time = time
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return new AddOrder.Response()
            {
                Success = true,
            };
        }

        public async Task<UpdateOrder.Response> UpdateOrderAsync(UpdateOrder.Request request)
        {
            Order order = await _context.Orders.FindAsync(request.OrderId);
            order.Date = DateOnly.FromDateTime(request.DateTime);
            order.Time = TimeOnly.FromDateTime(request.DateTime);
            int result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new UpdateOrder.Response()
                {
                    Success = true,
                };
            }
            return new UpdateOrder.Response()
            {
                Success = false,
                Result = new BadRequestObjectResult("Update operation failed."),
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
            query.EndTime = !TimeOnly.TryParse(query.EndTime, out endTime) ? "23:59:59" : query.EndTime;
            //if invalid value provided, set the start time to midnight
            query.StartTime = !TimeOnly.TryParse(query.StartTime, out startTime) ? "00:00:00" : query.StartTime;

            return query;
        }

        public GetOrderAmount.Query SanitizeGetOrderAmountQuery(GetOrderAmount.Query query)
        {
            TimeOnly endTime;
            TimeOnly startTime;

            //If invalid value provided, set the end time to time now
            query.EndTime = !TimeOnly.TryParse(query.EndTime, out endTime) ? "23:59:59" : query.EndTime;
            //if invalid value provided, set the start time to midnight
            query.StartTime = !TimeOnly.TryParse(query.StartTime, out startTime) ? "00:00:00" : query.StartTime;

            return query;
        }

        //TODO: The Sanitize Methods have redundancy. They can be combined.
        public GetProfit.Query SanitizeGetProfitQuery(GetProfit.Query query)
        {
            TimeOnly endTime;
            TimeOnly startTime;

            //If invalid value provided, set the end time to time now
            query.EndTime = !TimeOnly.TryParse(query.EndTime, out endTime) ? "23:59:59" : query.EndTime;
            //if invalid value provided, set the start time to midnight
            query.StartTime = !TimeOnly.TryParse(query.StartTime, out startTime) ? "00:00:00" : query.StartTime;
            //convert all pizzaids to lowercase
            query.PizzaIds = query.PizzaIds.Select(item => item.ToLower()).ToList();

            return query;
        }
    }
}
