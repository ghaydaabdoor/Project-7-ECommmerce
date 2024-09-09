using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project7Candy.DTO;
using Project7Candy.Models;

namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly MyDbContext _db;
        public CartItemController(MyDbContext db)
        {
            _db = db;
        }
        [HttpGet("cartitem")]
        public IActionResult getCartItems()
        {
            var cartItem = _db.CartItems.Select(x => new cartItemResponseDTO
            {
                CartId = x.CartId,
                CartItemId = x.CartItemId,
                Quantity = x.Quantity,
                Product = new ProductMainRequest
                {
                    ProductId = x.Product.ProductId,
                    ProductName = x.Product.ProductName,
                    Price = x.Product.Price,
                }
            });
            return Ok(cartItem);
        }

        [HttpPost("addtocart")]
        public IActionResult AddCartItem([FromBody] CartItemRequest cart)
        {
            // Step 1: Retrieve the available stock of the product from the database
            var product = _db.Products.SingleOrDefault(p => p.ProductId == cart.ProductId);
            if (product == null)
            {
                return NotFound("The product does not exist.");
            }

            // Step 2: Check if the requested quantity is available
            if (product.Stock < cart.Quantity)
            {
                return BadRequest("The requested quantity is not available.");
            }

            // Step 3: Create a new cart item
            var cartItem = new CartItem
            {
                CartId = cart.CartId,
                Quantity = cart.Quantity,
                ProductId = cart.ProductId
            };

            // Add the cart item to the database
            _db.CartItems.Add(cartItem);

            // Update the product's stock in the database
            product.Stock -= cart.Quantity;

            // Save the changes to the database
            _db.SaveChanges();

            return Ok();
        }



        [HttpPut("update/updateCart/{id}")]
        public IActionResult updateCart(int id, [FromBody] CartItemRequest cart)
        {
            var cartItem = _db.CartItems.FirstOrDefault(ci => ci.CartItemId == id);

            if (cartItem == null)
            {
                return BadRequest();
            }
            cartItem.Quantity = cart.Quantity;
            var updatecItems = _db.CartItems.Update(cartItem);
            _db.SaveChanges();
            if (updatecItems == null)
            {
                return BadRequest();
            }
            return Ok(updatecItems);
        }

        [HttpDelete("delete/deleteItem/{id}")]
        public IActionResult deleteItem(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var cartItem = _db.CartItems.FirstOrDefault(ci => ci.CartItemId == id);

            if (cartItem == null)
            {
                return NotFound();
            }
            _db.CartItems.Remove(cartItem);
            _db.SaveChanges();
            return NoContent();
        }

        //[HttpGet("cart/GetCartItemsForUser/{userID}")]
        //public IActionResult GetCartItemsForUser(int userID)
        //{
        //    var user = _db.Carts.FirstOrDefault(x => x.UserId == userID);
        //    var cartItem = _db.CartItems.Where(ci => ci.CartId == user.CartId).Select(
        //        x => new cartItemResponseDTO
        //        {
        //            CartItemId = x.CartItemId,
        //            CartId = x.CartId,
        //            Quantity = x.Quantity,
        //            Product = new ProductMainRequest
        //            {
        //                ProductId = x.Product.ProductId,
        //                ProductName = x.Product.ProductName,
        //                Price = x.Product.Price,
        //                ProductImage = x.Product.ProductImage,
        //            },
        //            Category = new CategoryMainRequest
        //            {
        //                CategoryName=x.Category.CategoryName,
        //            }
        //        });
        //    return Ok(cartItem);
        //}

        //[HttpGet("cart/GetCartItemsForProduct/{Productid}")]
        //public IActionResult GetCartItemsForProduct(int Productid)
        //{
        //    var user = _db.CartItems.FirstOrDefault(x => x.UserId == userID);
        //    var cartItem = _db.CartItems.Where(ci => ci.CartId == user.CartId).Select(
        //    return Ok(); 
        //}

        [HttpGet("cartitem/{id}")]
        public IActionResult getCartItems(int id)
        {
            var cartItem = _db.CartItems.Where(c => c.ProductId == id).ToList();
            return Ok(cartItem);
        }
        ///////////////////////////////////////////////////////////////////////////////////

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



        [HttpGet("cart/GetCartItemsForUser/{userID}")]
        public IActionResult GetCartItemsForUser(int userID)
        {
            var user = _db.Carts.FirstOrDefault(x => x.UserId == userID);
            var cartItem = _db.CartItems.Where(ci => ci.CartId == user.CartId).Select(
                x => new cartItemResponseDTO
                {
                    CartItemId = x.CartItemId,
                    CartId = x.CartId,
                    Quantity = x.Quantity,
                });
            return Ok(cartItem);
        }

        [HttpGet("getCartbyUserIDDDD/{id}")]
        public IActionResult getUsersandCarts(int id)
        {
            var cartUser = _db.Carts.FirstOrDefault(p => p.UserId == id);
            return Ok(cartUser);
        }

    }
}
