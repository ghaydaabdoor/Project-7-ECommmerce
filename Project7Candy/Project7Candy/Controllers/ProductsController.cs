using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project7Candy.Models;

namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly MyDbContext _db;
        public ProductsController(MyDbContext db)
        {
            _db = db;
        }

        [HttpGet("GetHighestRateProducts")]  
        public IActionResult GetHighestRateProducts()
        {
            var HighestRateProducts = _db.Products.OrderByDescending(p => p.Rate).Take(6).Select(p => new
            {
                p.ProductName,
                p.ProductDescription,
                p.Price,
                p.Stock,
                p.Discount,
                p.ProductImage,
                p.Rate,
                Category = new
                {
                    p.CategoryId,
                    p.Category.CategoryName,
                },
            });
            return Ok(HighestRateProducts);
        }

        [HttpGet("GetDiscountProducts")]
        public IActionResult GetDiscountProducts()
        {
            var DiscountProducts = _db.Products.OrderByDescending(p => p.Discount).Take(6).Select(p => new
            {
                p.ProductName,
                p.ProductDescription,
                p.Price,
                p.Stock,
                p.Discount,
                p.ProductImage,
                p.Rate,
                Category = new
                {
                    p.CategoryId,
                    p.Category.CategoryName,
                },
            });
            return Ok(DiscountProducts);
        }
    }
}
