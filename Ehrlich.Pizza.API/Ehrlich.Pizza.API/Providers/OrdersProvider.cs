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
        Task<AddOrderDetail.Response> AddOrderDetailAsync(AddOrderDetail.Request request);
        Task<UpdateOrderDetail.Response> UpdateOrderDetailAsync(UpdateOrderDetail.Request request);
        Task<DeleteOrderDetail.Response> DeleteOrderDetailAsync(long orderDetailId);
    }

    public class OrdersProvider : IOrdersProvider
    {
        private readonly PizzaPlaceDbContext _context;
        private readonly ILogger<IOrdersProvider> _logger;
        public OrdersProvider(PizzaPlaceDbContext context, ILogger<IOrdersProvider> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        ///     Retrieves a list of orders based on query filters like OrderId, date, and time range.
        ///     Supports pagination with Page Number (PN) and Page Size (PS).
        /// </summary>
        /// <param name="query">Query parameters including filters for orders</param>
        public async Task<GetOrders.Response> GetOrdersAsync(GetOrders.Query query)
        {
            _logger.LogInformation("Retrieving orders with query: {@Query}", query);
            List<Order> orders = new List<Order> { };

            //OrderId is unique meaning they're only querying for one item and we should return just that if OrderId exists in the query.
            if (query.OrderId.HasValue)
            {
                _logger.LogInformation("Querying for OrderId={OrderId}", query.OrderId.Value);
                var orderItem = await _context.Orders.Where(o => o.OrderId == query.OrderId).ToListAsync();
                _logger.LogInformation("Found {Count} orders with OrderId={OrderId}", orderItem.Count, query.OrderId.Value);
                return new GetOrders.Response() { Orders = orderItem };
            }

            //Rest of code if OrderId is not specified and multiple items are expected.
            query = SanitizeGetOrdersQuery(query);
            var orderQuery = _context.Orders.AsQueryable();
            orderQuery = orderQuery.Where(o => o.Date >= DateOnly.FromDateTime(query.StartDate) && o.Date <= DateOnly.FromDateTime(query.EndDate));
            orderQuery = orderQuery.Where(o => o.Time >= TimeOnly.Parse(query.StartTime) && o.Time <= TimeOnly.Parse(query.EndTime));
            orderQuery = orderQuery.Skip((query.PN - 1) * query.PS).Take(query.PS);
            orders = await orderQuery.ToListAsync();

            _logger.LogInformation("Retrieved {Count} orders matching query", orders.Count);

            return new GetOrders.Response() { Orders = orders };
        }

        /// <summary>
        ///     Retrieves the total count of orders within a specified date and time range.
        /// </summary>
        /// <param name="query">Query parameters for filtering orders</param>
        public async Task<GetOrderAmount.Response> GetOrderAmountAsync(GetOrderAmount.Query query)
        {
            _logger.LogInformation("Calculating order amount with query: {@Query}", query);

            query = SanitizeGetOrderAmountQuery(query);
            var orderQuery = _context.Orders.AsQueryable();
            orderQuery = orderQuery.Where(o => o.Date >= DateOnly.FromDateTime(query.StartDate) && o.Date <= DateOnly.FromDateTime(query.EndDate));
            orderQuery = orderQuery.Where(o => o.Time >= TimeOnly.Parse(query.StartTime) && o.Time <= TimeOnly.Parse(query.EndTime));
            return new GetOrderAmount.Response()
            {
                OrderAmount = orderQuery.Count(),
            };
        }

        /// <summary>
        ///     Calculates the total profit from orders within a specified date and time range.
        ///     Allows filtering by specific Pizza IDs.
        /// </summary>
        /// <param name="query">Query parameters for calculating profit</param>
        public async Task<GetProfit.Response> GetProfitAsync(GetProfit.Query query)
        {
            _logger.LogInformation("Calculating profit with query: {@Query}", query);

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

        /// <summary>
        ///     Adds a new order with a specific date and time. Generates a new OrderId.
        /// </summary>
        /// <param name="dateTime">The date and time of the new order</param>
        public async Task<AddOrder.Response> AddOrderAsync(DateTime dateTime)
        {
            _logger.LogInformation("Adding new order with DateTime: {DateTime}", dateTime);

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

        /// <summary>
        ///     Updates an existing order's date and time based on its OrderId.
        ///     Returns success status or an error message if the update fails.
        /// </summary>
        /// <param name="request">Request object containing the order information to update</param>
        public async Task<UpdateOrder.Response> UpdateOrderAsync(UpdateOrder.Request request)
        {
            _logger.LogInformation("Updating order with OrderId: {OrderId} and DateTime: {DateTime}", request.OrderId, request.DateTime);

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

        /// <summary>
        ///     Adds a new order detail for an existing order. Validates the order and pizza IDs, and checks if the quantity is greater than 0.
        /// </summary>
        /// <param name="request">Request object containing the order detail information</param>
        public async Task<AddOrderDetail.Response> AddOrderDetailAsync(AddOrderDetail.Request request)
        {
            _logger.LogInformation("Adding order detail with OrderId: {OrderId}, PizzaId: {PizzaId}, Quantity: {Quantity}", request.OrderId, request.PizzaId, request.Quantity);

            if (request.Quantity <= 0)
            {
                return new AddOrderDetail.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Quantity must be greater than 0."),
                };
            }

            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null)
            {
                return new AddOrderDetail.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Order ID does not exist."),
                };
            }

            var pizza = await _context.Pizzas.FindAsync(request.PizzaId);
            if (pizza == null)
            {
                return new AddOrderDetail.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Pizza ID does not exist."),
                };
            }

            long latestOrderDetailId = await _context.OrderDetails.MaxAsync(o => o.OrderDetailsId);
            var orderDetail = new OrderDetail
            {
                OrderDetailsId = latestOrderDetailId + 1,
                OrderId = request.OrderId,
                PizzaId = request.PizzaId,
                Quantity = request.Quantity,
            };

            _context.OrderDetails.Add(orderDetail);
            int result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new AddOrderDetail.Response
                {
                    Success = true,
                };
            }
            return new AddOrderDetail.Response
            {
                Success = false,
                Result = new BadRequestObjectResult("Failed to add order detail."),
            };
        }

        /// <summary>
        ///     Updates an existing order detail, including the OrderId, PizzaId, and Quantity.
        ///     Returns success or failure with appropriate messages.
        /// </summary>
        /// <param name="request">Request object containing the updated order detail information</param>
        public async Task<UpdateOrderDetail.Response> UpdateOrderDetailAsync(UpdateOrderDetail.Request request)
        {
            _logger.LogInformation("Updating order detail with OrderDetailsId: {OrderDetailsId}, OrderId: {OrderId}, PizzaId: {PizzaId}, Quantity: {Quantity}", request.OrderDetailsId, request.OrderId, request.PizzaId, request.Quantity);

            if (request.Quantity <= 0)
            {
                return new UpdateOrderDetail.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Quantity must be greater than 0."),
                };
            }

            var orderDetail = await _context.OrderDetails.Where(od => od.OrderDetailsId == request.OrderDetailsId).FirstOrDefaultAsync();
            if (orderDetail == null)
            {
                return new UpdateOrderDetail.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Order detail does not exist."),
                };
            }

            orderDetail.OrderId = request.OrderId;
            orderDetail.PizzaId = request.PizzaId;
            orderDetail.Quantity = request.Quantity;

            _context.OrderDetails.Update(orderDetail);
            int result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new UpdateOrderDetail.Response
                {
                    Success = true,
                };
            }
            return new UpdateOrderDetail.Response
            {
                Success = false,
                Result = new BadRequestObjectResult("Failed to update order detail."),
            };
        }

        /// <summary>
        ///     Deletes an order detail based on its OrderDetailId.
        ///     Returns success or failure with appropriate messages.
        /// </summary>
        /// <param name="orderDetailId">The ID of the order detail to be deleted</param>
        public async Task<DeleteOrderDetail.Response> DeleteOrderDetailAsync(long orderDetailId)
        {
            _logger.LogInformation("Deleting order detail with OrderDetailsId: {OrderDetailsId}", orderDetailId);
            var orderDetail = await _context.OrderDetails.Where(od => od.OrderDetailsId == orderDetailId).FirstOrDefaultAsync();
            if (orderDetail == null)
            {
                return new DeleteOrderDetail.Response
                {
                    Success = false,
                    Result = new BadRequestObjectResult("Order detail does not exist."),
                };
            }

            _context.OrderDetails.Remove(orderDetail);
            int result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new DeleteOrderDetail.Response
                {
                    Success = true,
                };
            }
            return new DeleteOrderDetail.Response
            {
                Success = false,
                Result = new BadRequestObjectResult("Failed to delete order detail."),
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
