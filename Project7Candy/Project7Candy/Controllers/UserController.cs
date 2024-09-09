using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project7Candy.DTO;
using Project7Candy.Models;
using Project7Candy.Services;
using Microsoft.EntityFrameworkCore;


namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _db;
        private readonly TokenGenerator _tokenGenerator;


        public UserController(MyDbContext db, TokenGenerator tokenGenerator)
        {
            _db = db;
            _tokenGenerator = tokenGenerator;

        }
        [HttpPost("register")]
        public ActionResult Register([FromForm] UserDTO model)
        {
            if (model.Password != model.RepeatedPassword)
            {
                return BadRequest();
            }
            var existingUser = _db.Users.FirstOrDefault(x => x.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest("this email is already exist");
            }
            byte[] passwordHash, passwordSalt;
            PasswordHasher.CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);

            User user = new User
            {

                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserType = "client"
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(user);
        }
        [HttpPost("login")]
        public IActionResult Login([FromForm] loginUserDTO model)
        {
            var user = _db.Users.FirstOrDefault(x => x.Email == model.Email);


            if (user == null || !PasswordHasher.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Invalid username or password.");
            }

            var roles = _db.UserRoles.Where(r => r.UserId == user.UserId).Select(r => r.Role).ToList();
            var token = _tokenGenerator.GenerateToken(user.FirstName + " " + user.LastName, roles);
            // Generate a token or return a success response
            return Ok(new { Token = token, UserId = user.UserId, UserType=user.UserType });
        }


        [HttpPost("registerbyGoogle")]
        public IActionResult RegisterUser([FromBody] GoogleUserDto userDto)

        {
            // Check if the user already exists
            if (_db.Users.Any(u => u.Email == userDto.email))
            {
                return BadRequest("User already exists.");
            }

            // Create a new user object
            var user = new User
            {

                Uid = userDto.uid,
                FirstName = userDto.displayName,

                Email = userDto.email,
                UserType = "client"



            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(user);
        }
        [HttpGet("Orders")]
        public IActionResult GetAllUsers()
        {
            var users = _db.Users.ToList();
            return Ok(users);
        }


        [HttpGet("getOuserById")]
        public IActionResult GetOrderById([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than 0.");
            }

            var user = _db.Users.Where(c => c.UserId == id).FirstOrDefault();
            if (user == null)
            {
                return NotFound("user not found.");
            }

            return Ok(user);
        }




        [HttpGet("GetUsersWithOrders")]
        public IActionResult GetUsersWithOrders()
        {
            var users = _db.Users
                .Select(user => new UserWithOrdersDTO
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Orders = user.Orders.Select(order => new OrderDTO
                    {
                        TotalAmount = order.TotalAmount,
                        PaymentMethod = order.PaymentMethod,
                        OrderStatus = order.OrderStatus
                    }).ToList()
                })
                .ToList();

            return Ok(users);
        }
    }
    public class GoogleUserDto
    {
        public string uid { get; set; }
        public string displayName { get; set; }
        public string email { get; set; }

    }





}

