using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CliWrap;
using CliWrap.EventStream;
using System.IO;
using System.Reactive;
using Microsoft.Extensions.Configuration;

namespace LinkViewAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FFMpegController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        public FFMpegController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // GET: api/FFMpeg/5
        [HttpGet("{inputfileName}", Name = "InFile")]
        //[HttpGet()]
        public string InFile(string inputfileName)
        {
            string inputPath = Path.Combine(Configuration["FFMpeg:InputPath"], inputfileName);
            string outPath = Path.Combine(Configuration["FFMpeg:OutPath"], inputfileName);
            CommandTask<int> t = Cli.Wrap(Configuration["FFMpeg:BinPath"])
            .WithArguments(" -i " + inputPath + " -vf scale=320:240 " + outPath)
            .ExecuteAsync()
            .Select(r => r.ExitCode);

            CommandTask<int> t2 = Cli.Wrap(Configuration["FFMpeg:BinPath"])
            .WithArguments(" -ss 00:00:03 -i " + inputPath + " -vframes 1 -q:v 2 -vf scale=107:107 " + Path.Combine(Configuration["FFMpeg:OutPath"], Path.GetFileNameWithoutExtension(inputPath) + ".png"))
            .ExecuteAsync()
            .Select(r => r.ExitCode);

            try
            {
                t.Task.Wait();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

            try
            {
                t2.Task.Wait();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }



            return "Completed";
        }
        

        // POST: api/FFMpeg
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/FFMpeg/5
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
