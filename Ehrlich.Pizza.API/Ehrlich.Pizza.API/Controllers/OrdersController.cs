using Azure;
using Ehrlich.Pizza.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ehrlich.Pizza.API.Requests;
using Ehrlich.Pizza.API.Providers;
using System;

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

        [HttpGet("OrderAmount")]
        public async Task<IActionResult> GetOrderAmount([FromQuery] GetOrderAmount.Query query)
        {
            var result = await _provider.GetOrderAmountAsync(query);
            return Ok(result);
        }

        [HttpGet("Profit")]
        public async Task<IActionResult> GetProfit([FromQuery] GetProfit.Query query)
        {
            var result = await _provider.GetProfitAsync(query);
            return Ok(result);
        }

        [HttpPost("Order")]
        public async Task<IActionResult> PostOrder([FromQuery] DateTime dateTime)
        {
            var result = await _provider.AddOrderAsync(dateTime);
            return result.Result;
        }

        [HttpPut("Order")]
        public async Task<IActionResult> PutOrder([FromQuery] UpdateOrder.Request request)
        {
            var result = await _provider.UpdateOrderAsync(request);
            return result.Result;
        }

        [HttpPost("OrderDetail")]
        public async Task<IActionResult> PostOrderDetail([FromQuery] AddOrderDetail.Request request)
        {
            var result = await _provider.AddOrderDetailAsync(request);
            return result.Result;
        }

        [HttpPut("OrderDetail")]
        public async Task<IActionResult> PutOrderDetail([FromQuery] UpdateOrderDetail.Request request)
        {
            var result = await _provider.UpdateOrderDetailAsync(request);
            return result.Result;
        }

        [HttpDelete("OrderDetail")]
        public async Task<IActionResult> DeleteOrderDetail([FromQuery] long orderDetailId)
        {
            var result = await _provider.DeleteOrderDetailAsync(orderDetailId);
            return result.Result;
        }
    }
}
