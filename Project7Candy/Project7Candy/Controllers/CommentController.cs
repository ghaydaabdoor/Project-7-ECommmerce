using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project7Candy.Models;

namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CommentController(MyDbContext context)
        {
            _context = context;
        }

        // Get all comments (for admin view)
        [HttpGet]
        public IActionResult GetAllComments()
        {
            var comments = _context.Comments.Select(p => new
            {
                p.CommentId,
                p.CommentText,
                p.ProductId,
                Product = new
                {
                    p.Product.ProductName,
                    p.Product.ProductImage,
                },
                User = new
                {
                    p.User.FirstName,
                    p.User.LastName,
                }
            });
            return Ok(comments);
        }

        // Get comments by product ID
        [HttpGet("byProduct/{productId}")]
        public IActionResult GetCommentsByProduct(int productId)
        {
            var comments = _context.Comments
                .Where(c => c.ProductId == productId)
                .Include(c => c.Product)
                .Include(c => c.User)
                .ToList();
            return Ok(comments);
        }

        // Add a comment
        [HttpPost]
        public IActionResult AddComment([FromBody] Comment comment)
        {
            if (comment == null || comment.ProductId <= 0 || comment.UserId == null || string.IsNullOrWhiteSpace(comment.CommentText))
            {
                return BadRequest("Invalid comment data.");
            }

            _context.Comments.Add(comment);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetCommentsByProduct), new { productId = comment.ProductId }, comment);
        }

        // Approve a comment
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.Status = "Approved";
            await _context.SaveChangesAsync();
            await ConvertCommentsToVoucherAsync(comment.UserId);
            return NoContent();
        }
        private async Task<IActionResult> ConvertCommentsToVoucherAsync(int userId)
        {
            var userComments = _context.Comments
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (userComments == null)
                return BadRequest("User does not have any comments!");

            // Generate voucher code
            string voucherCode = "COMMENT" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            var newVoucher = new Voucher
            {
                VoucherCode = voucherCode,
                DiscountValue = 15,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                MinimumCartValue = 20,
                MaxTotalUsage = 1,
                MaxUsagePerUser = 1,
                IsActive = 1
            };

            // Add new voucher to the database
            _context.Vouchers.Add(newVoucher);
            await _context.SaveChangesAsync();

            // Associate the voucher with the user
            var userVoucher = new UserVoucherUsage
            {
                UserId = userId,
                VoucherId = newVoucher.Id,
                UsageCount = 0
            };
            _context.UserVoucherUsages.Add(userVoucher);

            // Save changes
            await _context.SaveChangesAsync();

            return Ok("Voucher created successfully.");
        }
        // Ignore (reject) a comment
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
