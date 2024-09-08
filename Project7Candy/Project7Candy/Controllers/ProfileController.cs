using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using Project7Candy.Models;
using Microsoft.AspNetCore.Identity;
using Project7Candy.DTO;

namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly MyDbContext _db;
        private readonly IConverter _converter;

        public ProfileController(MyDbContext db, IConverter converter)
        {
            _db = db;
            _converter = converter;

        }


        ///////////////////////////////////////////////////////////////

        [HttpGet("{id}")]
        public IActionResult GetUserByID(int id)
        {
            if (id <= 0) { return BadRequest(); }
            var user = _db.Users.FirstOrDefault(x => x.UserId == id);
            if (user == null) { return NotFound(); }
            return Ok(user);
        }

        ///////////////////////////////////////////////////////////////

        [HttpPut("{id}")]
        public IActionResult EditUserProfile(int id, [FromForm] UserRequestDTO userRequestDTO)
        {
            if (id <= 0) { return BadRequest(); }
            var user = _db.Users.Find(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            if (userRequestDTO.ProfileImage != null)
            {
                var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }
                var filePath = Path.Combine(uploadsFolderPath, userRequestDTO.ProfileImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    userRequestDTO.ProfileImage.CopyToAsync(stream);
                }
            }
            user.FirstName = userRequestDTO.FirstName ?? user.FirstName;
            user.LastName = userRequestDTO.LastName ?? user.LastName;
            user.Phone = userRequestDTO.Phone ?? user.Phone;

            _db.Users.Update(user);
            _db.SaveChanges();
            return NoContent();
        }

        ///////////////////////////////////////////////////////////////

        [HttpGet("Address")]
        public IActionResult GetUserAddress(int id)
        {
            if (id <= 0) { return BadRequest(); }
            var adresses = _db.UserAddresses.Where(x => x.UserId == id).ToList();
            if (adresses == null) { return NotFound(new { Message = "adresses not found" }); }
            return Ok(adresses);
        }

        ///////////////////////////////////////////////////////////////

        [HttpPut("adresses/{id:int}")]
        public IActionResult editAddress(int id, [FromForm] AddressRequestDTO addressRequestDTO)
        {
            if (id <= 0) { return BadRequest(); }
            var address = _db.UserAddresses.FirstOrDefault(x => x.AddressId == id);
            if (address == null) { return NotFound(new { Message = "address not found" }); }

            address.Street = addressRequestDTO.Street ?? address.Street;
            address.City = addressRequestDTO.City ?? address.City;
            address.HomeNumberCode = addressRequestDTO.HomeNumberCode ?? address.HomeNumberCode;

            _db.UserAddresses.Update(address);
            _db.SaveChanges();
            return NoContent();
        }

        ///////////////////////////////////////////////////////////////

        [HttpPost("Address/{id}")]
        public IActionResult AddNewAddress(int id, [FromForm] AddressRequestDTO addressRequestDTO)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var user = _db.Users.FirstOrDefault(x => x.UserId == id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var newAddress = new UserAddress
            {
                UserId = id,
                Street = addressRequestDTO.Street,
                City = addressRequestDTO.City,
                HomeNumberCode = addressRequestDTO.HomeNumberCode,
            };

            _db.UserAddresses.Add(newAddress);
            _db.SaveChanges();

            return Ok();
        }

        ///////////////////////////////////////////////////////////////

        [HttpDelete("Address/{id:int}")]
        public IActionResult DeleteAddress(int id)
        {
            if (id <= 0) { return BadRequest(); }
            var address = _db.UserAddresses.FirstOrDefault(y => y.AddressId == id);
            if (address == null) { return NotFound(); }
            _db.UserAddresses.Remove(address);
            _db.SaveChanges();

            return Ok();
        }

        ///////////////////////////////////////////////////////////////

        [HttpGet("Points")]
        public IActionResult GetPoints(int id)
        {
            if (id <= 0) { return BadRequest(); }
            var user = _db.Users.Where(x => x.UserId == id).FirstOrDefault();
            if (user == null) { return NotFound(); }
            var points = user.Points;

            return Ok(points);
        }

        ///////////////////////////////////////////////////////////////

        [HttpGet("Vouchers")]
        public IActionResult GetVouchers(int id)
        {
            if (id <= 0) { return BadRequest("Invalid user ID."); }

            var userVouchers = _db.UserVoucherUsages
                .Where(x => x.UserId == id)
                .Select(userVoucher => new VoucherDetailDTO
                {
                    UserId = userVoucher.UserId ?? 0,
                    VoucherId = userVoucher.VoucherId ?? 0,
                    UsageCount = userVoucher.UsageCount ?? 0,
                    VoucherCode = userVoucher.Voucher.VoucherCode,
                    DiscountValue = userVoucher.Voucher.DiscountValue,
                    ValidFrom = userVoucher.Voucher.ValidFrom,
                    ValidTo = userVoucher.Voucher.ValidTo,
                    MinimumCartValue = userVoucher.Voucher.MinimumCartValue,
                    ProductId = userVoucher.Voucher.ProductId,
                    MaxUsagePerUser = userVoucher.Voucher.MaxUsagePerUser,
                    MaxTotalUsage = userVoucher.Voucher.MaxTotalUsage,
                    IsActive = userVoucher.Voucher.IsActive
                })
                .ToList();

            if (userVouchers == null || !userVouchers.Any())
            {
                return NotFound("No vouchers found for this user.");
            }

            return Ok(userVouchers);
        }


        ///////////////////////////////////////////////////////////////

        [HttpGet("Orders")]
        public IActionResult GetOrders(int id)
        {
            if (id <= 0) { return BadRequest(); }
            var userOrders = _db.Orders.Where(x => x.UserId == id).ToList();
            if (userOrders == null) { return NotFound(); }

            return Ok(userOrders);
        }


        ///////////////////////////////////////////////////////////////

        [HttpGet("InvoiceByOrderID")]
        public IActionResult InvoiceByOrderID(int id)
        {
            if (id <= 0) { return BadRequest(); }
            var OrderInvoice = _db.OrderItems.Where(x => x.OrderId == id).ToList();
            if (OrderInvoice == null) { return NotFound(); }

            return Ok(OrderInvoice);
        }

        ///////////////////////////////////////////////////////////////


        [HttpGet("ProductByID")]
        public IActionResult ProductByID(int id)
        {
            if (id <= 0) { return BadRequest(); }
            var ProductByID = _db.Products.Where(x => x.ProductId == id).ToList();
            if (ProductByID == null) { return NotFound(); }

            return Ok(ProductByID);
        }

        ///////////////////////////////////////////////////////////////

        [HttpGet("GenerateInvoice")]
        public IActionResult GenerateInvoice(int orderId)
        {
            var orderItems = _db.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                .ToList();

            // Generate PDF
            var pdfDocument = new HtmlToPdfDocument
            {
                GlobalSettings = {
                    DocumentTitle = $"Invoice for Order {orderId}",
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings
                    {
                        HtmlContent = GenerateInvoiceHtml(orderItems),
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            var pdf = _converter.Convert(pdfDocument);

            return File(pdf, "application/pdf", $"invoice_{orderId}.pdf");
        }

        private string GenerateInvoiceHtml(List<OrderItem> orderItems)
        {
            var html = @"
        <html>
        <head>
            <link href='https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap' rel='stylesheet'>
            <style>
                body {
                    font-family: 'Roboto', sans-serif;
                    color: #333;
                    background-color: #f9f9f9;
                    margin: 0;
                    padding: 0;
                }
                .container {
                    width: 80%;
                    margin: auto;
                    background-color: #fff;
                    padding: 20px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    border-radius: 8px;
                }
                .header {
                    text-align: center;
                    margin-bottom: 20px;
                    color: #e91e63;
                }
                .header h1 {
                    margin: 0;
                    font-size: 2.5em;
                    font-weight: 700;
                }
                .header h2 {
                    margin: 0;
                    font-size: 1.5em;
                    font-weight: 400;
                }
                table {
                    width: 100%;
                    border-collapse: collapse;
                    margin: 20px 0;
                }
                th, td {
                    border: 1px solid #ddd;
                    padding: 12px;
                    text-align: center;
                }
                th {
                    background-color: #f8bbd0;
                    color: #e91e63;
                    font-weight: 700;
                }
                tr:nth-child(even) {
                    background-color: #fce4ec;
                }
                .footer {
                    margin-top: 20px;
                    text-align: center;
                    font-size: 1.2em;
                    color: #e91e63;
                }
                .total {
                    font-weight: 700;
                }
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>SugarLand</h1>
                    <h2>Invoice</h2>
                </div>
                <table>
                    <thead>
                        <tr>
                            <th>Product Name</th>
                            <th>Quantity</th>
                            <th>Price</th>
                            <th>Total</th>
                        </tr>
                    </thead>
                    <tbody>";

            foreach (var item in orderItems)
            {
                var productName = item.Product?.ProductName ?? "Unknown";
                var price = item.Product?.Price ?? 0;
                var quantity = item.Quantity ?? 0;
                var total = price * quantity;

                html += $@"
                        <tr>
                            <td>{productName}</td>
                            <td>{quantity}</td>
                            <td>${price:F2}</td>
                            <td>${total:F2}</td>
                        </tr>";
            }

            var totalAmount = orderItems.Sum(oi => oi.Product.Price * oi.Quantity) ?? 0;

            html += $@"
                    </tbody>
                </table>
                <div class='footer'>
                    <p class='total'>Total Amount: ${totalAmount:F2}</p>
                </div>
            </div>
        </body>
        </html>";

            return html;
        }

        ///////////////////////////////////////////////////////////////
        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequestDTO model)
        {
            if (model == null ||
                string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.OldPassword) ||
                string.IsNullOrEmpty(model.NewPassword) ||
                string.IsNullOrEmpty(model.ConfirmPassword))
            {
                return BadRequest("All fields are required.");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("New password and confirmation do not match.");
            }

            var user = _db.Users.FirstOrDefault(x => x.Email == model.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!PasswordHasher.VerifyPasswordHash(model.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Old password is incorrect.");
            }

            byte[] newPasswordHash, newPasswordSalt;
            PasswordHasher.CreatePasswordHash(model.NewPassword, out newPasswordHash, out newPasswordSalt);
            user.Password = model.NewPassword;
            user.PasswordHash = newPasswordHash;
            user.PasswordSalt = newPasswordSalt;

            // Save changes to the database
            _db.Users.Update(user);
            _db.SaveChanges();

            return Ok("Password changed successfully.");
        }

        ///////////////////////////////////////////////////////////////

        //[HttpPost("register")]
        //public IActionResult Register([FromForm] RegisterRequestDTO model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Check if the user already exists
        //        var existingUser = _db.Users.FirstOrDefault(u => u.Email == model.Email);
        //        if (existingUser != null)
        //        {
        //            return BadRequest("User already exists with this email.");
        //        }

        //        byte[] passwordHash, passwordSalt;
        //        PasswordHasher.CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);

        //        var user = new User
        //        {
        //            Email = model.Email,
        //            PasswordHash = passwordHash, // Store hashed password
        //            PasswordSalt = passwordSalt, // Store salt used for hashing
        //            FirstName = model.FirstName,
        //            LastName = model.LastName,
        //            Phone = model.Phone
        //        };

        //        _db.Users.Add(user);
        //        _db.SaveChanges();

        //        return Ok(new { Message = "User registered successfully." });
        //    }
        //    else
        //    {
        //        return BadRequest(ModelState);
        //    }
        //}
    }
}
