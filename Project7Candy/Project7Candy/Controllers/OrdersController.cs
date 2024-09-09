using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project7Candy.DTOs;
using Project7Candy.Models;


namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly MyDbContext _db;

        public OrdersController(MyDbContext context)
        {
            _db = context;
        }

        //[HttpPost]
        //public IActionResult CreateOrder([FromBody] CreateOrderDTO newOrder)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var order = new Order
        //    {
        //        UserId = newOrder.UserId,
        //        TotalAmount = newOrder.TotalAmount,
        //        PaymentMethod = newOrder.PaymentMethod,
        //        OrderStatus = newOrder.OrderStatus,
        //        Comment = newOrder.Comment,
        //        OrderDate = newOrder.OrderDate
        //    };

        //    _db.Orders.Add(order);
        //    _db.SaveChanges();

        //    return Ok(order);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO newOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a new order
            var order = new Order
            {
                UserId = newOrder.UserId,
                TotalAmount = newOrder.TotalAmount,
                PaymentMethod = newOrder.PaymentMethod,
                OrderStatus = newOrder.OrderStatus,
                Comment = newOrder.Comment,
                OrderDate = newOrder.OrderDate
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // After creating the order, trigger voucher creation automatically
            var result = await ConvertOrdersToVoucherAsync(order.UserId);
            if (result is BadRequestObjectResult)
            {
                return BadRequest("Unable to create a voucher.");
            }

            return Ok(order);
        }

        private async Task<IActionResult> ConvertOrdersToVoucherAsync(int userId)
        {
            var userOrders = _db.Orders
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.OrderDate)
                .FirstOrDefault();

            if (userOrders == null)
                return BadRequest("User does not have any orders!");

            // Generate voucher code
            string voucherCode = "CASHBACK" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            var newVoucher = new Voucher
            {
                VoucherCode = voucherCode,
                DiscountValue = 5,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                MinimumCartValue = 20,
                MaxTotalUsage = 1,
                MaxUsagePerUser = 1,
                IsActive = 1
            };

            // Add new voucher to the database
            _db.Vouchers.Add(newVoucher);
            await _db.SaveChangesAsync();

            // Associate the voucher with the user
            var userVoucher = new UserVoucherUsage
            {
                UserId = userId,
                VoucherId = newVoucher.Id,
                UsageCount = 0
            };

            _db.UserVoucherUsages.Add(userVoucher);
            await _db.SaveChangesAsync();

            return Ok("Voucher created successfully.");
        }

        [HttpGet("byUser/{userId}")]
        public IActionResult GetOrdersByUser(int userId)
        {
            var orders = _db.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();

            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            return Ok(orders);
        }
    }
}
