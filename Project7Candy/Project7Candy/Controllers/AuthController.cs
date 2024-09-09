using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project7Candy.Models;
using Project7Candy.DTO;
using System.Net.Mail;


namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyDbContext _context;

        public AuthController(MyDbContext context)
        {
            _context = context;
        }

        [HttpPost("send-reset-code")]
        public IActionResult SendResetCode([FromBody] string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // إنشاء رمز عشوائي
            var resetToken = Guid.NewGuid().ToString().Substring(0, 6);
            user.ResetToken = resetToken;
            user.TokenExpiration = DateTime.Now.AddMinutes(30); // صلاحية الرمز لمدة 30 دقيقة
            _context.SaveChanges();

            // إرسال الرمز إلى البريد الإلكتروني
            SendEmail(email, resetToken);

            return Ok("Reset code sent to email.");
        }

        private void SendEmail(string email, string resetToken)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("ramaoudat151@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Password Reset Code";
            mail.Body = $"Your reset code is: {resetToken}";

            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential("techlearnhub.contact@gmail.com", "lyrlogeztsxclank");
            smtpServer.EnableSsl = true;

            smtpServer.Send(mail);
        }
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetToken == model.Token);
            if (user == null || user.TokenExpiration < DateTime.Now)
            {
                return BadRequest("Invalid token or token expired.");
            }
            byte[] passwordHash, passwordSalt;
            PasswordHasher.CreatePasswordHash(model.NewPassword, out passwordHash, out passwordSalt);
            user.Password = model.NewPassword;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.ResetToken = null;
            user.TokenExpiration = null;
            _context.SaveChanges();

            return Ok("Password reset successful.");
        }

        public class ResetPasswordModel
        {
          
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
