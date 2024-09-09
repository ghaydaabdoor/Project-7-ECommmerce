using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // GET: api/Products
        [HttpGet]
        public IActionResult GetProducts()
        {
            var product = _db.Products.ToList();
            return Ok(product);
        }


        [Route("category/{id}")]
        [HttpGet]
        public IActionResult GetProductById(int id)
        {


            var products = _db.Products.Where(c => c.CategoryId == id).ToList();

            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _db.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }



        // GET: api/Products/priceRange?minPrice=0&maxPrice=6
        [HttpGet("priceRange")]
        public IActionResult GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var products = _db.Products
                                  .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                                  .ToList();

            if (products.Count == 0)
            {
                return NotFound(new { message = "No products found within the specified price range." });
            }

            return Ok(products);
        }

        // GET: api/Products/search?query={query}
        [Route("search")]
        [HttpGet]
        public IActionResult SearchProducts(string query)
        {
            var products = _db.Products
                                   .Where(p => p.ProductName.Contains(query))
                                   .ToList();

            if (products.Count == 0)
            {
                return NotFound(new { message = "No products found matching the search criteria." });
            }

            return Ok(products);
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
        [HttpPost]
        public IActionResult AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _db.Products.Add(product);
            _db.SaveChanges();
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _db.Products.Find(id);
            if (product == null) return NotFound();

            product.ProductName = updatedProduct.ProductName;
            product.Price = updatedProduct.Price;
            product.ProductImage = updatedProduct.ProductImage;
            product.ProductDescription = updatedProduct.ProductDescription;

            _db.SaveChanges();
            return Ok(product);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            if (id < 1)
            {
                return BadRequest("ID must be greater than 0");
            }

            var product = _db.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            _db.Products.Remove(product);
            _db.SaveChanges();
            return Ok();
        }

    }
}
