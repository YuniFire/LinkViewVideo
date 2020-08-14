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
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;

namespace LinkViewAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMTPController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        public SMTPController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // GET api/values
        [HttpGet()]
        public ActionResult<string> SendMail(int id, string mobiRecipient, string email)
        {
            DataBridgeConnector dbc = new DataBridgeConnector(Configuration.GetConnectionString("SMTPdb"));
            object[] objArray2 = new object[1];
            objArray2.SetValue(id, 0);

            DataSet ds2 = dbc.GetSqlDataSet("sp_UP_GetVideoByID", objArray2);
            string videoPath = ds2.Tables[0].Rows[0]["VideoPath"].ToString();
            string messageText = ds2.Tables[0].Rows[0]["MessageText"].ToString();
            string videoName = ds2.Tables[0].Rows[0]["VideoName"].ToString();
            string origText = messageText;
            var apiKey = Configuration["API:SMTPAPI"];
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage();

            msg.SetFrom(new EmailAddress(Configuration["Email:FromAddress"], "Linkview Video"));
            List<EmailAddress> eAddress = new List<EmailAddress>();
            if (!string.IsNullOrEmpty(email))
            {
                eAddress.Add(new EmailAddress(email));
            }
            msg.AddTos(eAddress);

            msg.SetSubject("Linkview Video " + videoName);
            messageText += "<br /><br /><a href='" + videoPath + "'>View Video</a>";
            msg.AddContent(MimeType.Html, messageText);
            Task t = client.SendEmailAsync(msg);
            t.Wait();

            //string messageSMSText = "<br /><br /><a href='" + videoPath + "'>View " + videoName + " video.</a>";
            string messageSMSText = origText + " - View Video: " + videoPath;  
            var accountSid = Configuration["Twilio:AccountSid"];
            var authToken = Configuration["Twilio:AuthToken"];
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber(mobiRecipient));
            messageOptions.From = new PhoneNumber(Configuration["Twilio:PhoneNumber"]);
            messageOptions.Body = messageSMSText;

            var message = MessageResource.Create(messageOptions);

            
            return "Success";
        }

        // POST: api/SMTP
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/SMTP/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
