using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project7Candy.DTO;
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
        [HttpPost]
        public IActionResult AddCategory([FromBody] CategoriesDTO category)
        {
            var data = new Category
            {
                CategoryName =category.CategoryName,
                CategoryDescription= category.CategoryDescription,
                CategoryImage=category.CategoryImage,
                CategoryIcon = category.CategoryIcon,
            };
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Categories.Add(data);
            _db.SaveChanges();

            return Ok(data);
        }

        // Update existing category
        [HttpPut("{id}")]
        public IActionResult UpdateCategory([FromBody] CategoriesDTO category)
        {
                var data = new Category
                {
                    CategoryName = category.CategoryName,
                    CategoryDescription = category.CategoryDescription,
                    CategoryImage = category.CategoryImage,
                    CategoryIcon = category.CategoryIcon,
                };
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _db.Categories.Add(data);
                _db.SaveChanges();

                return Ok(data);
            }

            // Delete category by ID
            [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            if (id < 1)
            {
                return BadRequest("ID must be greater than 0");
            }

            var category = _db.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(category);
            _db.SaveChanges();
            return NoContent();
        }
    }
}
