using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Test_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        public UploadFileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest("กรุณาเลือก file upload.");
            }

            var uploadFolder = Path.Combine(_environment.ContentRootPath, "FileUpdate");
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            SendEmail(filePath);

            return Ok("อัปโหลดไฟล์สำเร็จและส่งอีเมลแล้ว");
        }

        public void SendEmail(string filePath)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("thirakanj58@gmail.com", "xplqobqhpdianbys"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("thirakanj58@gmail.com"),
                Subject = "การแจ้งเตือนการอัปโหลดไฟล์",
                Body = "ไฟล์ถูกอัปโหลดสำเร็จแล้ว",
            };

            mailMessage.To.Add("thirakan58@gmail.com");

            var attachment = new Attachment(filePath);
            mailMessage.Attachments.Add(attachment);

            smtpClient.Send(mailMessage);
        }
    }
}
