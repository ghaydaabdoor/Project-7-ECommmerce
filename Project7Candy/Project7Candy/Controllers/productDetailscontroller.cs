using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project7Candy.DTO;
using Project7Candy.Models;

namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class productDetailscontroller : ControllerBase
    {
        private readonly MyDbContext _db;
        public productDetailscontroller(MyDbContext db)
        {
            _db = db;
        }
        [HttpGet("GetAllProduct")]
        public IActionResult GetAllProduct()
        {
            var product = _db.Products.ToList();
            return Ok(product);
        }
        [HttpGet("GetAllProductsForOneCategory/{id}")]
        public IActionResult GetProduct(int id)
        {
            var Pro = _db.Products.Where(a => a.ProductId == id).ToList();
            return Ok(Pro);
        }
        [HttpGet("GetOneProductByID/{id}")]
        public IActionResult One(int id)
        {
            var onee = _db.Products.Find(id);
            return Ok(onee);
        }
        //[HttpPost]
        //public IActionResult PostProduct([FromForm] ProductMainRequest main)
        //{
        //    var data = new Product 
        //    { 
        //        ProductName = main.ProductName,
        //        ProductDescription = main.ProductDescription,
        //        Price = main.Price,
        //        Stock = main.Stock,
        //        Discount = main.Discount,
        //        ProductImage= main.ProductImage.FileName
        //    };
        //    var ImageFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uplode");
        //    if (!Directory.Exists(ImageFolder))
        //    {
        //        Directory.CreateDirectory(ImageFolder);
        //    }
        //    var ImageFile = Path.Combine("Uplode", main.ProductImage.FileName);
        //    using (var stream = new FileStream(ImageFile, FileMode.Create))
        //    {
        //        main.ProductImage.CopyToAsync(stream);
        //    }
        //    _db.Products.Add(data);
        //    _db.SaveChanges();
        //    return Ok();
        //}
        //[HttpPut("UpdateTheProductByID/{id}")]
        //public IActionResult PutProduct(int id, [FromForm] QuantityRequest quantity)
        //{
        //    var update = _db.CartItems.FirstOrDefault(a => a.CartItemId== id);
        //    update.Quantity = quantity.Quantity;
        //    _db.CartItems.Update(update);
        //    _db.SaveChanges();
        //    return Ok();
        //}
        [HttpPut("UpdateTheProductRateByID/{id}")]
        public IActionResult Rate(int id, [FromForm] ProductMainRequest Prod)
        {
            var uprate = _db.Products.FirstOrDefault(a => a.ProductId == id);
            uprate.Rate = Prod.Rate;
            _db.Products.Update(uprate);
            _db.SaveChanges();
            return Ok();
        }

        [HttpPost("AddRateByID")]
        public IActionResult AddRate(int id, [FromForm] ProductMainRequest pro)
        {
            var data = new Product
            {
                Rate = pro.Rate,
            };
            _db.Products.Add(data);
            _db.SaveChanges();
            return Ok();
        }

        [HttpPost("AddQuantityToProductByID/{id}")]
        public IActionResult AddQuan(int id, [FromBody] int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Invalid quantity.");
            }
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Unauthorized("User is not logged in.");
            }

            if (int.TryParse(User.Identity.Name, out int userId))
            {
                var user=_db.Carts.FirstOrDefault(a => a.UserId == userId);
                var cartItem = _db.CartItems.Where(ci => ci.CartId == user.CartId).Select(
                    x=> new cartItemResponseDTO
                    {
                        CartId = x.CartId,
                        CartItemId = x.CartItemId,
                        Quantity = x.Quantity,
                        Product = new ProductMainRequest
                        {

                            ProductId = x.Product.ProductId,
                            ProductName = x.Product.ProductName,
                            Price = x.Product.Price
                        }
                    }


                    );





                if (cartItem == null)
                {
                    return NotFound("Cart item not found.");
                }

               

              
                _db.SaveChanges();

                return Ok(cartItem);
            }
            else
            {
                return BadRequest("User ID is not valid.");
            }
        }



        [HttpPost]
        [Route("addRate")]
        public IActionResult AddRating([FromBody] RateRequest rating)
        {
            if (rating == null)
            {
                return BadRequest("Invalid rating data.");
            }
            var r = _db.Ratings.FirstOrDefault(x => x.UserId == rating.UserId);
            if (rating.UserId == r.UserId && rating.ProductId == r.ProductId)
            {
                return BadRequest("You have already rate this product ");
            }
            // Validate rating value (e.g., 1-5)
            if (rating.RatingValue < 1 || rating.RatingValue > 5)
            {
                return BadRequest("Rating value must be between 1 and 5.");
            }
            var rate = new Rating()
            {

                UserId = rating.UserId,
                ProductId = rating.ProductId,
                RatingValue = rating.RatingValue
            };

            rate.CreatedAt = DateTime.UtcNow;
            _db.Ratings.Add(rate);
            _db.SaveChanges();

            return Ok(new { message = "Rating added successfully." });
        }

        [HttpGet("checkrating")]
        public IActionResult checkrating()
        {
            var count = _db.Ratings.Count();
            var x = _db.Ratings.Sum(x => x.RatingValue) / count;
            return Ok(x);
        }
        [HttpGet("GetCountComments/{id}")]
        public IActionResult GetComments(int id)
        {
            // Retrieve comments based on the specified id
            var comments = _db.Comments.Where(c => c.ProductId == id && c.Status == "Approved").ToList();

            // Check if there are no comments
            if (!comments.Any())
            {
                // If no comments are found
                return NotFound("No comments found.");
            }

            // Count the number of comments
            var commentCount = comments.Count;

            // Concatenate all comment texts into a single string
            var allCommentTexts = string.Join(", ", comments.Select(c => c.CommentText));

            // Return the result including the count of comments and concatenated texts
            return Ok(new { CommentCount = commentCount, AllCommentTexts = allCommentTexts });
        }
        //[HttpPost("AddNewCommentToProductByID/{id}")]
        //public IActionResult AddComment(int id, [FromBody] CommentRequest corq)
        //{
        //    if (corq == null)
        //    {
        //        return BadRequest("Invalid comment data.");
        //    }

        //    // Check if there is already a comment from this user on this product
        //    var existingComment = _db.Comments
        //                             .FirstOrDefault(x => x.UserId == corq.UserId && x.ProductId == corq.ProductId);

        //    // If an existing comment is found, return a BadRequest response
        //    if (existingComment != null)
        //    {
        //        return BadRequest("You have already added a comment to this product.");
        //    }

        //    // Create a new comment (do not set CommentId if it's an identity column)
        //    var comment = new Comment()
        //    {
        //        UserId = corq.UserId,
        //        ProductId = corq.ProductId,
        //        CommentText = corq.CommentText
        //        // Do not set CommentId here if it's an identity column
        //    };

        //    try
        //    {
        //        _db.Comments.Add(comment);
        //        _db.SaveChanges();
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        var innerException = ex.InnerException?.Message ?? ex.Message;
        //        return StatusCode(500, $"An error occurred while saving the comment: {innerException}");
        //    }

        //    return Ok(new { message = "Comment added successfully." });
        //}

        //[HttpPost("AddCommentToProductById/{id}")]
        //public IActionResult AddCo(int id, [FromBody] CommentRequest commentreq)
        //{
        //    var data = new Comment
        //    {
        //        UserId = commentreq.UserId,
        //        ProductId = commentreq.ProductId,
        //        CommentText = commentreq.CommentText,
        //        User = new User
        //        {
        //            FirstName = commentreq.UserrRequest.FirstName,
        //            LastName = commentreq.UserrRequest.LastName,
        //            Email = commentreq.UserrRequest.Email,
        //        }
        //    };
        //    _db.Comments.Add(data);
        //    _db.SaveChanges();
        //    return Ok();
        //}
        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpPost("CommentOnProduct")]
        public IActionResult CommentOnProduct([FromForm] CommentPRequestDTO commentPRequestDTO)
        {


            var newComment = new Comment()
            {
                ProductId = commentPRequestDTO.ProductId.Value,
                UserId = commentPRequestDTO.UserId.Value,
                CommentText = commentPRequestDTO.CommentText,
                CreatedAt = commentPRequestDTO.CreatedAt ?? DateTime.Now
            };

            _db.Comments.Add(newComment);
            _db.SaveChanges();

            return Ok("Comment added successfully.");
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet("GetAllComment/{id}")]
        public IActionResult GetAllCoToPro(int id)
        {
            var all = _db.Comments.Where(c => c.ProductId == id && c.Status == "Approved").ToList();
            if (all == null)
            {
                return BadRequest("No Comments Found");
            }
            return Ok(all);
        }






    }
}
