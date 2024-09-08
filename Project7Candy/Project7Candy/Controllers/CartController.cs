using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project7Candy.Models;

namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly MyDbContext _db;
        public CartController(MyDbContext db)
        {
            _db = db;
        }

        [HttpGet("GetCartItems/{userID}")]
        public IActionResult GetCartItems(int userID)
        {
            var cartItems = _db.CartItems.Select(p => new
            {
                p.Quantity,
                Cart = new
                {
                    p.Cart.UserId,
                },
                Product = new
                {
                    p.Product.ProductImage,
                    p.Product.ProductName,
                    p.Product.Price,
                    p.Product.Category.CategoryName,
                }
            }).Where(p => p.Cart.UserId == userID).ToList();    
            return Ok(cartItems);
        }
    }
}
