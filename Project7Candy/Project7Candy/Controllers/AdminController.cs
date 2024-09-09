using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project7Candy.Models;
using Project7Candy.DTO;
namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly MyDbContext _db;
        public AdminController(MyDbContext db)
        {
            _db = db;
        }

        ///////////////////////////////////////////////////////////////

        [HttpGet("GetALLVouchers")]
        public IActionResult GetALLVouchers()
        {
            var Vouchers = _db.Vouchers.ToList();
            return Ok(Vouchers);
        }
        ///////////////////////////////////////////////////////////////


        [HttpGet("GetVoucherByID")]
        public IActionResult GetVoucherByID(int id)
        {
            if (id <= 0) { return BadRequest(); }
            var Voucher = _db.Vouchers.Where(x => x.Id == id).FirstOrDefault();
            return Ok(Voucher);
        }

        ///////////////////////////////////////////////////////////////

        [HttpGet("GetUserVouchers")]
        public IActionResult GetUserVouchers(int id)
        {
            if (id <= 0) { return BadRequest(); }
            var userVouchers = _db.UserVoucherUsages.Where(x => x.UserId == id).ToList();
            return Ok(userVouchers);
        }

        ///////////////////////////////////////////////////////////////

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var userVouchers = _db.Users.ToList();
            return Ok(userVouchers);
        }

        ///////////////////////////////////////////////////////////////

        [HttpPost]
        public IActionResult AddVoucher([FromForm] VoucherRequestDTO voucherRequestDTO)
        {
            if (voucherRequestDTO.ValidFrom == null || voucherRequestDTO.ValidTo == null)
            {
                return BadRequest("ValidFrom and ValidTo must be provided.");
            }

            // Create new voucher
            var newVoucher = new Voucher()
            {
                VoucherCode = voucherRequestDTO.VoucherCode,
                DiscountValue = voucherRequestDTO.DiscountValue,
                ValidFrom = voucherRequestDTO.ValidFrom.HasValue
                    ? DateOnly.FromDateTime(voucherRequestDTO.ValidFrom.Value)
                    : null,
                ValidTo = voucherRequestDTO.ValidTo.HasValue
                    ? DateOnly.FromDateTime(voucherRequestDTO.ValidTo.Value)
                    : null,
                MinimumCartValue = voucherRequestDTO.MinimumCartValue,
                ProductId = voucherRequestDTO.ProductId,
                MaxUsagePerUser = voucherRequestDTO.MaxUsagePerUser,
                MaxTotalUsage = voucherRequestDTO.MaxTotalUsage,
                IsActive = voucherRequestDTO.IsActive
            };

            _db.Vouchers.Add(newVoucher);
            _db.SaveChanges(); // Save voucher to get its Id

            // Add voucher to all users
            var allUsers = _db.Users.ToList();
            foreach (var user in allUsers)
            {
                var userVoucherUsage = new UserVoucherUsage
                {
                    UserId = user.UserId,
                    VoucherId = newVoucher.Id,
                    UsageCount = 0 // or any default value
                };
                _db.UserVoucherUsages.Add(userVoucherUsage);
            }

            _db.SaveChanges(); // Save all user voucher usages

            return Ok();
        }




        ///////////////////////////////////////////////////////////////
        [HttpPut("editVoucher/{id:int}")]
        public IActionResult EditVoucher(int id, [FromBody] VoucherRequestDTO voucherRequestDTO)
        {
            if (id <= 0) return BadRequest("Invalid voucher ID");

            // Fetch the existing voucher
            var voucher = _db.Vouchers.FirstOrDefault(x => x.Id == id);
            if (voucher == null) return NotFound("Voucher not found");

            // Update voucher properties with values from the request
            if (!string.IsNullOrEmpty(voucherRequestDTO.VoucherCode))
                voucher.VoucherCode = voucherRequestDTO.VoucherCode;

            // Use ?? to check for null or use default values from request
            voucher.DiscountValue = voucherRequestDTO.DiscountValue ?? voucher.DiscountValue;
            voucher.ValidFrom = voucherRequestDTO.ValidFrom.HasValue
                ? DateOnly.FromDateTime(voucherRequestDTO.ValidFrom.Value)
                : voucher.ValidFrom; // Use existing value if not provided

            voucher.ValidTo = voucherRequestDTO.ValidTo.HasValue
                ? DateOnly.FromDateTime(voucherRequestDTO.ValidTo.Value)
                : voucher.ValidTo; // Use existing value if not provided

            voucher.MinimumCartValue = voucherRequestDTO.MinimumCartValue ?? voucher.MinimumCartValue;
            voucher.MaxUsagePerUser = voucherRequestDTO.MaxUsagePerUser ?? voucher.MaxUsagePerUser;
            voucher.MaxTotalUsage = voucherRequestDTO.MaxTotalUsage ?? voucher.MaxTotalUsage;
            voucher.IsActive = voucherRequestDTO.IsActive ?? voucher.IsActive;

            // Mark the voucher as modified
            _db.Vouchers.Update(voucher);

            try
            {
                // Save changes to the database
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                Console.Error.WriteLine($"Error updating voucher: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

            return Ok("Voucher updated successfully");
        }



        ///////////////////////////////////////////////////////////////

        [HttpDelete("DeleteVoucher/{id:int}")]
        public IActionResult DeleteVoucher(int id)
        {
            if (id <= 0) { return BadRequest(); }
            var voucher = _db.Vouchers.FirstOrDefault(y => y.Id == id);
            if (voucher == null) { return NotFound(); }
            _db.Vouchers.Remove(voucher);
            _db.SaveChanges();

            return Ok();
        }
        ///////////////////////////////////////////////////////////////

    }
}
