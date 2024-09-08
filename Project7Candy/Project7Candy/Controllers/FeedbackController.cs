using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project7Candy.DTO;
using Project7Candy.Models;

namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly MyDbContext _db;
        public FeedbackController(MyDbContext db)
        {
            _db = db;
        }


        [HttpPost("AddFeedback")]
        public IActionResult AddFeedback([FromForm] FeedbackRequestDTO feedback)
        {
            var data = new Feedback
            {
                Name = feedback.Name,
                Email = feedback.Email,
                Subject = feedback.Subject,
                Message = feedback.Message,
                SentDate = feedback.SentDate,
            };
            _db.Feedbacks.Add(data);
            _db.SaveChanges();
            return Ok(data);
        }

        [HttpGet("GetAllFeedbacks")]
        public IActionResult GetAllFeedbacks()
        {
            var feedbacks = _db.Feedbacks.ToList();
            return Ok(feedbacks);
        }
    }
}
