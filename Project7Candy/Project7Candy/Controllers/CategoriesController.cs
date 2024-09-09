using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project7Candy.Models;

namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly MyDbContext _db;
        public CategoriesController(MyDbContext db)
        {
            _db = db;
        }

        [HttpGet("GetAllCategories")]
        public IActionResult GetAllCategories()
        {
            var categories = _db.Categories.ToList();
            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            var category = _db.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }
    }
}
