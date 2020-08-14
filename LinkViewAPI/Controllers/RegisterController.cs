using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using SendGrid;
using SendGrid.Helpers.Mail;
using ETUDataControls;
using System.Data;
using System.Globalization;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LinkViewAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        public RegisterController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet()]
        public ActionResult<string> SendMail(string to, string link)
        {

            var apiKey = Configuration["API:RegisterAPI"];
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage();

            msg.SetFrom(new EmailAddress(Configuration["Email:FromAddress"], "Linkview Registration"));
            List<EmailAddress> eAddress = new List<EmailAddress>();
            if (!string.IsNullOrEmpty(to))
            {
                eAddress.Add(new EmailAddress(to));
            }


            msg.AddTos(eAddress);
            msg.SetSubject("LinkView Registration");
            msg.AddContent(MimeType.Html, link);
            Task t = client.SendEmailAsync(msg);
            t.Wait();
            return "Success";
        }
        // GET: api/<RegisterController>


        // GET api/<RegisterController>/



        // POST api/<RegisterController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<RegisterController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RegisterController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
