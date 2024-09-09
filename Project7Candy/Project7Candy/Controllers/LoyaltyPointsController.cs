using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project7Candy.Models;

namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyPointsController : ControllerBase
    {
        private readonly MyDbContext _db;

        public LoyaltyPointsController(MyDbContext db)
        {
            _db = db;
        }
        //////////////////////////////////////////////////////////////////////
        [HttpPost("award")]
        public IActionResult AwardPoints([FromBody] AwardPointsRequest request)
        {
            if (request == null || request.UserId <= 0 || string.IsNullOrWhiteSpace(request.Action) || request.Points <= 0)
                return BadRequest("Invalid request");

            int pointsAwarded = request.Points;

            var userPoints = _db.UserPoints
                .Where(up => up.UserId == request.UserId)
                .OrderByDescending(up => up.Id)
                .FirstOrDefault();

            if (userPoints == null)
            {
                userPoints = new UserPoint
                {
                    UserId = request.UserId,
                    Points = pointsAwarded,
                    Action = request.Action,
                    TotalPoints = pointsAwarded
                };
                _db.UserPoints.Add(userPoints);
            }
            else
            {
                userPoints.Points += pointsAwarded;
                userPoints.TotalPoints += pointsAwarded;
                userPoints.Action = request.Action;
                _db.Entry(userPoints).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }

            _db.SaveChanges();

            return Ok(new { success = true, totalPoints = userPoints.TotalPoints });
        }

        //////////////////////////////////////////////////////////////////////

        [HttpGet("points")]
        public IActionResult GetUserPoints([FromQuery] int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID");

            var userPoints = _db.UserPoints
                .Where(up => up.UserId == userId)
                .OrderByDescending(up => up.Id)
                .Select(up => new
                {
                    up.Action,
                    up.Points,
                    up.TotalPoints,

                })
                .ToList();

            if (!userPoints.Any())
                return NotFound("No points found for this user");

            return Ok(userPoints);
        }
        //////////////////////////////////////////////////////////////////////

        public class AwardPointsRequest
        {
            public int UserId { get; set; }
            public string Action { get; set; }
            public int Points { get; set; }
        }

        //////////////////////////////////////////////////////////////////////

        [HttpPost("ConvertToVoucher")]
        public IActionResult ConvertToVoucher(int id)
        {
            var userPoints = _db.UserPoints.FirstOrDefault(x => x.UserId == id);
            if (userPoints == null || userPoints.Points < 14)
                return BadRequest("User does not have enough points to convert to vouchers.");
            int? numberOfVouchers = userPoints.TotalPoints / 15;
            for (int i = 0; i < numberOfVouchers; i++)
            {
                string voucherCode = "SHARE" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
                var newVoucher = new Voucher
                {
                    VoucherCode = voucherCode,
                    DiscountValue = 10,
                    ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                    ValidTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    MinimumCartValue = 5,
                    MaxTotalUsage = 1,
                    MaxUsagePerUser = 1,
                    IsActive = 1

                };
                _db.Vouchers.Add(newVoucher);
                _db.SaveChanges();

                var userVoucher = new UserVoucherUsage
                {
                    UserId = id,
                    VoucherId = newVoucher.Id,
                    UsageCount = 0
                };
                _db.UserVoucherUsages.Add(userVoucher);

                userPoints.Points -= 20;
                userPoints.TotalPoints = userPoints.Points;
                _db.SaveChanges();
            }

            return Ok("Vouchers created successfully.");
        }

        //////////////////////////////////////////////////////////////////////

        [HttpPost("ConvertOrdersToVoucher")]
        public IActionResult ConvertOrdersToVoucher(int id)
        {
            var userOrders = _db.Orders.Where(x => x.UserId == id).OrderByDescending(x => x.OrderDate).FirstOrDefault();
            if (userOrders == null)
                return BadRequest("User does not have any orders!");


            string voucherCode = "CASHBACK" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            var newVoucher = new Voucher
            {
                VoucherCode = voucherCode,
                DiscountValue = 5,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                MinimumCartValue = 20,
                MaxTotalUsage = 1,
                MaxUsagePerUser = 1,
                IsActive = 1

            };
            _db.Vouchers.Add(newVoucher);
            _db.SaveChanges();

            var userVoucher = new UserVoucherUsage
            {
                UserId = id,
                VoucherId = newVoucher.Id,
                UsageCount = 0
            };
            _db.UserVoucherUsages.Add(userVoucher);


            _db.SaveChanges();


            return Ok("Vouchers created successfully.");
        }

        //////////////////////////////////////////////////////////////////////

        //[HttpPost("ConvertCommentsToVoucher")]
        //public IActionResult ConvertCommentsToVoucher(int id)
        //{
        //    var userComments = _db.Comments.Where(x => x.UserId == id).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        //    if (userComments == null)
        //        return BadRequest("User does not have any comments!");


        //    string voucherCode = "COMMENT" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        //    var newVoucher = new Voucher
        //    {
        //        VoucherCode = voucherCode,
        //        DiscountValue = 15,
        //        ValidFrom = DateOnly.FromDateTime(DateTime.Now),
        //        ValidTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        //        MinimumCartValue = 20,
        //        MaxTotalUsage = 1,
        //        MaxUsagePerUser = 1,
        //        IsActive = 1

        //    };
        //    _db.Vouchers.Add(newVoucher);
        //    _db.SaveChanges();

        //    var userVoucher = new UserVoucherUsage
        //    {
        //        UserId = id,
        //        VoucherId = newVoucher.Id,
        //        UsageCount = 0
        //    };
        //    _db.UserVoucherUsages.Add(userVoucher);


        //    _db.SaveChanges();


        //    return Ok("Vouchers created successfully.");
        //}

        //////////////////////////////////////////////////////////////////////
        [HttpPost("daily-checkin")]
        public IActionResult DailyCheckIn(int userId)
        {
            var userCheckIn = _db.UserCheckIns.FirstOrDefault(u => u.UserId == userId);

            if (userCheckIn == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var today = DateTime.UtcNow.Date;
            var lastCheckInDate = userCheckIn.LastCheckInDate.HasValue ? userCheckIn.LastCheckInDate.Value.Date : DateTime.MinValue;


            if (lastCheckInDate < today.AddDays(-1))
            {
                userCheckIn.Points = 0;
            }
            if (userCheckIn.LastCheckInDate.HasValue && userCheckIn.LastCheckInDate.Value.Date == today)
            {
                return BadRequest(new { message = "You can only check in once per day." });
            }

            userCheckIn.Points = (userCheckIn.Points ?? 0) + 1;
            userCheckIn.LastCheckInDate = DateTime.UtcNow;

            if (userCheckIn.Points >= 7)
            {
                userCheckIn.Points = 0;

                var voucher = new Voucher
                {
                    VoucherCode = GenerateVoucherCode(),
                    DiscountValue = 50,
                    ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow),
                    ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                    MinimumCartValue = 100,
                    MaxUsagePerUser = 1,
                    MaxTotalUsage = 100,
                    IsActive = 1
                };

                _db.Vouchers.Add(voucher);

                var userVoucherUsage = new UserVoucherUsage
                {
                    UserId = userId,
                    VoucherId = voucher.Id,
                    UsageCount = 0
                };

                _db.UserVoucherUsages.Add(userVoucherUsage);
            }

            _db.SaveChanges();

            return Ok(new
            {
                Points = userCheckIn.Points,
                Message = userCheckIn.Points >= 7
        ? "Congratulations! You've earned a voucher."
        : $"You have checked in for {userCheckIn.Points} days. Keep going until you reach 7 days to earn a voucher with 50% discount!"
            });

        }

        private string GenerateVoucherCode()
        {
            var random = new Random();
            const string chars = "0123456789";
            var randomNumbers = new string(Enumerable.Repeat(chars, 5)
                                                      .Select(s => s[random.Next(s.Length)])
                                                      .ToArray());
            return $"DAILYCHECKIN{randomNumbers}";
        }


    }
}
