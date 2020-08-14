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
using Microsoft.Extensions.Configuration;
using System.Net;
using Newtonsoft.Json;
namespace LinkViewAPI.Controllers
{
    class BrandedURL
    {
        public string shortUrl { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class LVAPIController : ControllerBase
    {
        private static string storageConn;
        private static CloudStorageAccount storageacc;
        private static CloudBlobContainer container;

        private readonly IConfiguration Configuration;
        public LVAPIController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // GET api/LVAPI/123.mp4
        [HttpGet("{filename}")]
        public ActionResult<string> Get(string filename)
        {
            filename = Path.Combine(Configuration["FFMpeg:OutPath"], filename);
            storageacc = CloudStorageAccount.Parse(Configuration["LVAPI:StorageInfo"]);

            //Create Reference to Azure Blob
            container = storageacc.CreateCloudBlobClient().GetContainerReference("linkview");
            var s = container.GetDirectoryReference("raw");
            CloudBlockBlob blockBlob = s.GetBlockBlobReference(Path.GetFileName(filename));
            blockBlob.Properties.ContentType = "video/mp4";
           
            Task upload = blockBlob.UploadFromFileAsync(filename);
            upload.Wait();
            

            string apiKey = Configuration["Rebrandly:RebrandlyApiKey"];
            string workspaceID = Configuration["Rebrandly:RebrandlyWorkspace"];
            string domain = Configuration["Rebrandly:RebrandlyDomain"];
            string dstURL = Configuration["LVAPI:ReturnPath"] + Path.GetFileName(filename);
            string url = $"https://api.rebrandly.com/v1/links/new?apikey={apiKey}&domain[fullName]={domain}&workspace={workspaceID}&destination={System.Web.HttpUtility.UrlEncode(dstURL)}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            string html = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            var link = JsonConvert.DeserializeObject<BrandedURL>(html);

            string shortURL = link.shortUrl;
            if (!shortURL.Contains("https://"))
            {
                shortURL = "https://" + link.shortUrl;
            }
            return shortURL;
            
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }


}
