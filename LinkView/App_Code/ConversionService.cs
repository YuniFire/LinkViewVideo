using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using EncoderLibraries;
using ETUDataControls;
using System.Configuration;
using System.Text;

using System.Threading;
using System.Runtime;
namespace Conversions
{
    public class SMTPHelp
    {

        public string FileName
        { get; set; }
        public string MessageText
        { get; set; }
        public string EmailFrom
        { get; set; }
        public string Recipient
        { get; set; }
        public string Email
        { get; set; }
        public string VideoPath
        { get; set; }

    }
    public class ConversionService
    {
        private HttpContext context;

        public HttpContext Context
        {
            get { return context; }
            set { context = value; }
        }
        private int messageID;

        public int MessageID
        {
            get { return messageID; }
            set { messageID = value; }
        }
        private string videoText;

        public string VideoText
        {
            get { return videoText; }
            set { videoText = value; }
        }
        private string videoPath;
        public string VideoPath
        {
            get { return videoPath; }
            set { videoPath = value; }
        }
        public ConversionService()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }
        public string GetVideoFromDataLake()
        {
            System.Net.WebClient client = new System.Net.WebClient();
            client.Headers[System.Net.HttpRequestHeader.ContentType] = "video/mp4";

            client.DownloadFile(ConfigurationManager.AppSettings["Blob"], "");
            return "";
        }
        public string PutVideoToDataLake(string fileName)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            client.Headers[System.Net.HttpRequestHeader.ContentType] = "application/json";

            string response = client.DownloadString(ConfigurationManager.AppSettings["url"] + ConfigurationManager.AppSettings["LVAPI"] + fileName);
            return response;
            //ConfigurationManager.AppSettings["Blob"] + fileName;
        }
        public string ConvertFile(FileStream fs)
        {

            FfmpegConverter converter = new FfmpegConverter();
            string outputpath = ConfigurationManager.AppSettings["OutputPath"].ToString();
            converter.LogPath = Path.Combine(outputpath, "log.txt");
            converter.FfmpegPath = ConfigurationManager.AppSettings["FFMpegPath"].ToString();
            //Output output = converter.ConvertTo3GP(fs,"h263");
            Output output = converter.ConvertTo3GP(fs);
            if (output.Success)
            {
                FileStream outStream = File.OpenWrite(Path.Combine(outputpath, output.FileName));
                output.VideoStream.WriteTo(outStream);
                outStream.Flush();
                outStream.Close();
                fs.Close();
                return Path.Combine(outputpath, output.FileName);
            }
            else
                fs.Close();
            return null;

        }

        public string ConvertFile(string inputPath, string userName, string tempName)
        {
            string outputOf = string.Empty;
            try
            {
                FfmpegConverter converter = new FfmpegConverter();
                string outputpath = ConfigurationManager.AppSettings["OutputPath"].ToString();
                if (ConfigurationManager.AppSettings["TempPath"].ToString() == "tmp")
                {

                }
                else
                {
                    converter.OutputPath = ConfigurationManager.AppSettings["OutputPath"].ToString();
                }
                converter.LogPath = Path.Combine(outputpath, "log.txt");
                converter.FfmpegPath = ConfigurationManager.AppSettings["FFMpegPath"].ToString();
                //FileStream fs = new FileStream(inputPath, FileMode.Open, FileAccess.ReadWrite);
                converter.ImagePath = Path.Combine(ConfigurationManager.AppSettings["OutputPath"].ToString(), Path.GetFileNameWithoutExtension(inputPath) + ".png");
                Output output = converter.ConvertToMP4(inputPath);
                output.Success = true;

                //StringBuilder sb = new StringBuilder();
                /*sb.AppendLine("Outputpath " + outputpath);
                sb.AppendLine("Logpath " + converter.LogPath);
                sb.AppendLine("FfmpegPath " + converter.FfmpegPath);
                sb.AppendLine("ImagePath " + converter.ImagePath);
                sb.AppendLine("Output Error " + output.Message);
                */

                if (output.Success)
                {
                    string renamedMP4 = Path.Combine(outputpath, output.FileName);
                    //renamedMP4 = Path.ChangeExtension(renamedMP4, "mp4");

                    //FileStream outStream = new FileStream(renamedMP4, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    //output.VideoStream.WriteTo(outStream);
                    //output.VideoStream.Close();

                    //byte[] fileBytes = new byte[outStream.Length];
                    byte[] imageBytes = File.ReadAllBytes(converter.ImagePath);
                    //outStream.Flush();
                    //outStream.Close();

                    //fileBytes = File.ReadAllBytes(renamedMP4);
                    //Copy to data lake here then pass to videoPath
                    //set videoPath here
                    string videoPath = PutVideoToDataLake(Path.GetFileName(renamedMP4));
                    ETUDataControls.DataBridgeConnector dbc = new DataBridgeConnector("");
                    object[] objArray = new object[7];
                    objArray.SetValue(userName, 0);
                    objArray.SetValue(Path.GetFileName(renamedMP4).Replace(tempName, ""), 1);
                    objArray.SetValue(null, 2);
                    objArray.SetValue(videoText, 3);
                    objArray.SetValue(videoPath, 4);
                    objArray.SetValue(0, 5);
                    objArray.SetValue(converter.ImagePath, 6);

                    object[] outputQuery = dbc.ExecuteSQLNonQueryWithOutput("sp_UP_InsertVideo", objArray);

                    messageID = Convert.ToInt32(outputQuery[0]);
                    outputOf = renamedMP4;
                    //After the file is created on disk, perform the same task on Blob Storage and delete
                    //the local file **TODO Venkatesh
                    //context.Session["updategrid"];
                    return outputOf;
                }
                else

                    return outputOf;
            }
            catch (Exception ex)
            {
                return outputOf;
            }

        }

        //public string ConvertFile(FileStream fs, string userName)
        //{

        //    FfmpegConverter converter = new FfmpegConverter();
        //    string outputpath = @"C:\support\Linkview\converted\";
        //    converter.LogPath = Path.Combine(outputpath, "log.txt");
        //    converter.FfmpegPath = @"C:\support\Linkview\Bin\ffmpeg.exe";
        //    //Output output = converter.ConvertTo3GP(fs,"h263");
        //    Output output = converter.ConvertTo3GP(fs);
        //    if (output.Success)
        //    {
        //        FileStream outStream = File.OpenWrite(Path.Combine(outputpath, output.FileName));
        //        output.VideoStream.WriteTo(outStream);
        //        outStream.Flush();
        //        outStream.Close();

        //        /*ETUDataControls.DataBridgeConnector dbc = new DataBridgeConnector(string.Empty);
        //        object[] objArray = new object[3];
        //        objArray.SetValue(userName, 0);
        //        objArray.SetValue(fs, 1);
        //        objArray.SetValue(output.FileName, 2);

        //        object[] outVal = dbc.ExecuteSQLNonQueryWithOutput("sp_UP_InsertVideo", false, objArray);
        //        */
        //        fs.Close();


        //        return "Success";
        //    }
        //    else
        //        fs.Close();
        //    return null;

        //}

    }
}

