using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Configuration;
namespace EncoderLibraries
{
    public class EncoderLog
    {
        // Fields
        private string _FilePath;

        // Methods
        public EncoderLog()
        {
            this._FilePath = string.Empty;
        }


        public EncoderLog(string filepath)
        {
            this._FilePath = string.Empty;
            this._FilePath = filepath;
        }

        public EncoderLog(string filepath, string logdata)
        {
            this._FilePath = string.Empty;
            this._FilePath = filepath;
            if (!string.IsNullOrEmpty(logdata))
            {
                this.WriteLog(logdata);
            }
        }

        private string GetFilePath()
        {
            string str = string.Empty;
            if (string.IsNullOrEmpty(Path.GetExtension(this._FilePath)))
            {
                str = "log.txt";
                try
                {
                    this._FilePath = Path.Combine(this._FilePath, str);
                }
                catch (Exception)
                {
                    this._FilePath = str;
                }
            }
            return this._FilePath;
        }

        public void WriteLog(string logdata)
        {
            try
            {
                FileStream stream = new FileStream(this.GetFilePath(), FileMode.Append, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("[Date]= " + DateTime.Now.ToLongDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString());
                builder.AppendLine(logdata);
                builder.AppendLine();
                writer.Write(builder);
                writer.Close();
                stream.Close();
            }
            catch (Exception)
            {
            }
        }
        public string FilePath
        {
            get
            {
                return this._FilePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this._FilePath = value;
                    string directoryName = Path.GetDirectoryName(value);
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                }
            }
        }

    }

    public class FfmpegConverter
    {
        // Fields
        private StringBuilder sb;
        private string _allowedext;
        private string _ErrorInfo;
        private string _ffExe;
        private bool _LogOutput;
        private string _LogPath;
        private string _OutputPath;
        private string _imagePath;

        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }


        private OutputOptions _Settings;
        private int _ConvertedFileLimitInKB;
        private string _VideoHeight;
        private string _VideoWidth;


        // Methods
        public FfmpegConverter()
        {
            this._ffExe = "ffmpeg.exe";
            this._OutputPath = string.Empty;
            this._LogOutput = true;
            this._LogPath = string.Empty;
            this._Settings = new OutputOptions();
            this._ErrorInfo = string.Empty;
            this._allowedext = ".avi,.mov,.mpg,.mpeg,.mp4,.wmv,.flv";
            this._ConvertedFileLimitInKB = 1000000;
            this._ffExe = "ffmpeg.exe";
        }

        public FfmpegConverter(string ffmpegExePath)
        {
            this._ffExe = "ffmpeg.exe";
            this._OutputPath = string.Empty;
            this._LogOutput = true;
            this._LogPath = string.Empty;
            this._Settings = new OutputOptions();
            this._ErrorInfo = string.Empty;
            this._allowedext = ".avi,.mov,.mpg,.mpeg,.mp4,.wmv,.flv";
            this._ConvertedFileLimitInKB = 1000000;
            this._ffExe = ffmpegExePath;
        }

        public Output ConvertTo3GP(FileStream inputStream)
        {
            //Change from using a temp path
            string path = ConfigurationManager.AppSettings["TempPath"];
            Random rd = new Random();


            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
            }
            path = Path.Combine(path, Path.GetFileName(inputStream.Name));
            if (string.IsNullOrEmpty(this._Settings.VideoCodec))
            {
                this._Settings.VideoCodec = "mpeg4";
            }
            bool flag = true;
            string inputFilePath = "";
            try
            {
                //FileStream inputStreamOne = new FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite);
                inputFilePath = this.CreateInputFile(inputStream, path);
            }
            catch (Exception ex)
            {
                sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
                flag = false;
            }
            Output output = this.ConvertTo3GP(inputFilePath);
            if (!flag)
            {
                output.Message = "An error occured while converting file from FileStream";
                output.Success = false;
                output.VideoStream = null;
            }
            try
            {
                RunProcess("-i " + inputFilePath + " -vcodec png -vframes 1 -an -f rawvideo -s 106x106 " + _imagePath);
                File.Delete(path);
            }
            catch (Exception ex)
            {
                sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
            }
            if (sb != null)
                File.WriteAllText(@"C:\tmp\" + rd.Next() + "-error.txt", sb.ToString());

            return output;
        }

        public Output ConvertToMP4(string inputFilePath)
        {

            bool flag = true;
            //string inputFilePath = "";

            Output output = new Output();
            Random rd = new Random();
            string outFileName = Path.Combine(OutputPath, Path.GetFileName(inputFilePath));
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                //client.Headers[System.Net.HttpRequestHeader.ContentType] = "application/json";
                //string newPathName = Path.GetFileName(inputFilePath);
                //Random rd2 = new Random();
                //newPathName = rd2.Next() + "-" + newPathName;

                string response = client.DownloadString(ConfigurationManager.AppSettings["url"] + ConfigurationManager.AppSettings["FFMpeg"] + Path.GetFileName(inputFilePath));


                output.FileName = Path.GetFileName(inputFilePath);

                //File.Delete(path);
            }
            catch (Exception ex)
            {
                sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
                output.Success = false;
            }
            if (sb != null)
                File.WriteAllText(@"C:\tmp\" + rd.Next() + "-error.txt", sb.ToString());

            return output;
        }
        public Output ConvertTo3GP(string inputFilePath)
        {
            if ((this._Settings.VideoCodec == "mpeg4") && !(string.IsNullOrEmpty(this._VideoWidth) || string.IsNullOrEmpty(this._VideoHeight)))
            {
                this._Settings.VideoWidth = this._VideoWidth;
                this._Settings.VideoHeight = this._VideoHeight;
            }
            this._Settings.OutputVideoType = "3gp";
            //this._Settings.OutputVideoType = "flv";
            return this.PrepareAndGoConvert(inputFilePath);
        }

        public Output ConvertTo3GP(FileStream inputStream, string videoCodec)
        {
            if (string.IsNullOrEmpty(videoCodec))
            {
                this._Settings.VideoCodec = "mpeg4";
            }
            else
            {
                this._Settings.VideoCodec = videoCodec.ToLower();
            }
            return this.ConvertTo3GP(inputStream);
        }

        private Output ConvertToVideoType(VideoFile input)
        {
            StringBuilder builder = new StringBuilder();
            Output output = new Output();
            if (!input.Found)
            {
                output.Message = input.Message;
            }
            else if (this._allowedext.IndexOf(Path.GetExtension(input.Path).ToLower()) < 0)
            {
                if (".xml.log.db".IndexOf(Path.GetExtension(input.Path)) < 0)
                {
                    output.Message = "\"" + input.Path + "\" " + Path.GetExtension(input.Path) + " files are not allowed to convert.";
                }
            }
            else
            {
                if (!input.infoGathered)
                {
                }
                string fileName = string.Empty;
                string path = string.Empty;
                string parameters = string.Empty;
                string str4 = string.Empty;
                string outputSettings = this.GetOutputSettings();
                fileName = Path.GetFileName(input.Path);
                path = Path.Combine(this._OutputPath, fileName);
                if (this._Settings.OutputAudioOnly)
                {
                    path = Path.ChangeExtension(path, this._Settings.OutputAudioType);
                }
                else
                {
                    path = Path.ChangeExtension(path, this._Settings.OutputVideoType);
                }
                parameters = string.Format("-i \"{0}\" {1} -pass 1 -passlogfile \"{2}\" NULL.3gp", input.Path, outputSettings, input.Path + "_pass2");
                output.ConversionStart = DateTime.Now;
                str4 = this.RunProcess(parameters);
                parameters = string.Format("-i \"{0}\" {1} -pass 2 -passlogfile \"{2}\" \"{3}\"", new object[] { input.Path, outputSettings, input.Path + "_pass2", path });
                str4 = this.RunProcess(parameters);
                try
                {
                    if (File.Exists("NULL.3gp"))
                    {
                        File.Delete("NULL.3gp");
                    }
                    if (File.Exists(input.Path + "_pass2-0.log"))
                    {
                        File.Delete(input.Path + "_pass2-0.log");
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
                }
                output.ConversionEnd = DateTime.Now;
                output.RawOutput = str4;
                output.Success = false;
                output.Message = "Failed";
                if (str4.Contains("Output #"))
                {
                    Regex regex = new Regex("");
                    Match match = regex.Match("");
                    int num = 0;
                    int num2 = 0;
                    if (this._Settings.OutputAudioOnly)
                    {
                        regex = new Regex(" audio:(?<AudioSize>[0-9]+)[k|K|m|M|g|G]B ", RegexOptions.IgnoreCase);
                        match = regex.Match(str4);
                        try
                        {
                            num = Convert.ToInt32(match.Groups["AudioSize"].Value);
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
                        }
                        if (num > 0)
                        {
                            output.Success = true;
                            output.Message = "File has successfully been converted";
                        }
                    }
                    else
                    {
                        match = new Regex("video:(?<VideoSize>[0-9]+)[k|K|m|M|g|G]B audio:(?<AudioSize>[0-9]+)[k|K|m|M|g|G]B ", RegexOptions.IgnoreCase).Match(str4);
                        try
                        {
                            num = Convert.ToInt32(match.Groups["AudioSize"].Value);
                            num2 = Convert.ToInt32(match.Groups["VideoSize"].Value);
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
                        }
                        if ((num2 > 0) && (num > 0))
                        {
                            output.Success = true;
                            output.Message = "File has successfully been converted";
                        }
                        else if ((num2 > 0) && (num <= 0))
                        {
                            output.Message = output.Message + " (audio not converted)";
                        }
                        else if (num2 <= 0)
                        {
                            output.Message = output.Message + " (video not converted)";
                        }
                    }
                }
                else
                {
                    string[] strArray = str4.Split(new char[] { '\r' });
                    if (strArray.Length > 0)
                    {
                        string str6 = "";
                        if (strArray[strArray.Length - 1] == "\n")
                        {
                            str6 = strArray[strArray.Length - 2].Replace('\n', ' ').Trim();
                        }
                        else
                        {
                            str6 = strArray[strArray.Length - 1].Replace('\n', ' ').Trim();
                        }
                        if (string.IsNullOrEmpty(str6))
                        {
                            str6 = "Ffmpeg didn't return any output";
                        }
                        output.Message = str6;
                    }
                }
                if (output.Success && File.Exists(path))
                {
                    string str7 = Path.ChangeExtension(fileName, this._Settings.OutputVideoType);
                    output.FileName = str7;
                    output.VideoStream = LoadMemoryStreamFromFile(path);
                    int num3 = this.ConvertedFileLimitInKB * 0x400;
                    if (output.VideoStream.Length > num3)
                    {
                        output.VideoStream = null;
                        output.Success = false;
                        output.Message = "Output file stream is larger than " + Utility.BytesToWords((long)num3);
                    }
                }
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
                }
                builder.AppendLine("[Input]= " + input.Path);
                builder.AppendLine("[Output]= " + path);
                builder.AppendLine();
                builder.AppendLine("[Ffmpeg History] ");
                builder.Append(str4);
                if (string.IsNullOrEmpty(str4))
                {
                    builder.Append("Ffmpeg didn't return any output");
                    builder.AppendLine();
                }
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine("[Conversion Status]= " + output.Message);
                builder.AppendLine();
                builder.AppendFormat("[Conversion Start Time]= {0}", output.ConversionStart.ToLongTimeString());
                builder.AppendLine();
                builder.AppendFormat("[Conversion End Time]= {0}", output.ConversionEnd.ToLongTimeString());
                builder.AppendLine();
                builder.AppendFormat("[Total Time]= {0}", output.GetConversionTime());
            }
            if (this._LogOutput)
            {
                if (string.IsNullOrEmpty(builder.ToString()))
                {
                    return output;
                }
                EncoderLog log = new EncoderLog();
                if (string.IsNullOrEmpty(this._LogPath))
                {
                    log.FilePath = Path.GetDirectoryName(input.Path);
                }
                else
                {
                    log.FilePath = this._LogPath;
                }
                log.WriteLog(builder.ToString());
            }
            return output;
        }

        private string CreateInputFile(FileStream inputStream, string tmp_path)
        {
            int count = 0x10000;
            using (FileStream stream = new FileStream(tmp_path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                int num2 = -1;
                byte[] buffer = new byte[count];
                while ((num2 = inputStream.Read(buffer, 0, count)) > 0)
                {
                    stream.Write(buffer, 0, num2);
                    stream.Flush();
                }
            }
            return tmp_path;
        }

        private string GetOutputSettings()
        {
            return this.GetOutputSettings(0);
        }

        private string GetOutputSettings(int pass_no)
        {
            StringBuilder builder = new StringBuilder();
            if (this._Settings.OverwriteFile)
            {
                builder.Append(" -y");
            }
            if (!(string.IsNullOrEmpty(this._Settings.VideoHeight) || string.IsNullOrEmpty(this._Settings.VideoWidth)))
            {
                builder.AppendFormat(" -s {0}x{1}", this._Settings.VideoWidth, this._Settings.VideoHeight);
                builder.AppendFormat(" -aspect {0}:{1}", 0xb0, 0x90);
            }
            builder.Append(" -vcodec " + this._Settings.VideoCodec);
            //figure out these parameters and make editable
            //builder.Append(" -b 131k -r 25 -acodec libamr_nb -ac 1 -ar 8000 -ab 12.2k");
            builder.Append(" -b 380k -r 25 -acodec libamr_nb -ac 1 -ar 8000 -ab 12.2k");
            //-i pathToStream.flv -vcodec png -vframes 1 -an -f rawvideo -s 106x106 pathToImage.png
            return builder.ToString();
        }

        public void GetVideoInfo(VideoFile input)
        {
            string parameters = string.Format("-i \"{0}\"", input.Path);
            string str2 = this.RunInfoProcess(parameters);
            input.RawInfo = str2;
            Match match = new Regex(@"[D|d]uration:.((\d|:|\.)*)").Match(input.RawInfo);
            if (match.Success)
            {
                string[] strArray = match.Groups[1].Value.Split(new char[] { ':', '.' });
                if (strArray.Length == 4)
                {
                    input.Duration = new TimeSpan(0, Convert.ToInt16(strArray[0]), Convert.ToInt16(strArray[1]), Convert.ToInt16(strArray[2]), Convert.ToInt16(strArray[3]));
                }
            }
            match = new Regex(@"[B|b]itrate:.((\d|:)*)").Match(input.RawInfo);
            double result = 0.0;
            if (match.Success)
            {
                double.TryParse(match.Groups[1].Value, out result);
            }
            input.BitRate = result;
            match = new Regex("[A|a]udio:.*").Match(input.RawInfo);
            if (match.Success)
            {
                input.AudioFormat = match.Value;
            }
            match = new Regex("[V|v]ideo:.*").Match(input.RawInfo);
            if (match.Success)
            {
                input.VideoFormat = match.Value;
            }
            match = new Regex(@"(\d{2,3})x(\d{2,3})").Match(input.RawInfo);
            if (match.Success)
            {
                int num2 = 0;
                int num3 = 0;
                int.TryParse(match.Groups[1].Value, out num2);
                int.TryParse(match.Groups[2].Value, out num3);
                input.Width = num2;
                input.Height = num3;
            }
            input.infoGathered = true;
        }

        public VideoFile GetVideoInfo(string inputPath)
        {
            VideoFile input = new VideoFile(inputPath);
            if (input.Found)
            {
                this.GetVideoInfo(input);
            }
            return input;
        }

        private void Initialize()
        {
            this._ErrorInfo = string.Empty;
            if (string.IsNullOrEmpty(this._ffExe))
            {
                this._ErrorInfo = "Could not find the location of ffmpeg.exe file";
            }
            else if (!File.Exists(this._ffExe))
            {
                this._ErrorInfo = "The file " + this._ffExe + " does not exist.";
            }
        }

        private bool IsConverterReady()
        {
            this.Initialize();
            return string.IsNullOrEmpty(this._ErrorInfo);
        }

        public static FileStream LoadFileStreamFromFile(string fileName)
        {
            FileStream stream = null;
            try
            {
                int count = 0x10000;
                using (FileStream stream2 = new FileStream(fileName, FileMode.Open))
                {
                    int num2 = -1;
                    byte[] buffer = new byte[count];
                    while ((num2 = stream2.Read(buffer, 0, count)) > 0)
                    {
                        stream.Write(buffer, 0, num2);
                        stream.Flush();
                    }
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                //sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
            }
            return stream;
        }

        public static MemoryStream LoadMemoryStreamFromFile(string fileName)
        {
            MemoryStream stream = null;
            try
            {
                using (FileStream stream2 = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[stream2.Length];
                    stream2.Read(buffer, 0, buffer.Length);
                    stream2.Close();
                    stream = new MemoryStream(buffer);
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                //sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
            }
            return stream;
        }

        private Output PrepareAndGoConvert(string inputPath)
        {
            Output output = new Output();
            if (this.IsConverterReady())
            {
                VideoFile input = new VideoFile(inputPath);
                return this.ConvertToVideoType(input);
            }
            output.Message = this._ErrorInfo;
            EncoderLog log = new EncoderLog("error.log", output.Message);
            return output;
        }

        private string RunInfoProcess(string Parameters)
        {
            string str = null;
            if (string.IsNullOrEmpty(this._ErrorInfo))
            {
                ProcessStartInfo info = new ProcessStartInfo(this._ffExe, Parameters);
                info.UseShellExecute = false;
                info.CreateNoWindow = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                StreamReader standardError = null;
                Process process = new Process();
                try
                {
                    try
                    {
                        process.StartInfo = info;
                        process.Start();
                        process.BeginOutputReadLine();
                        standardError = process.StandardError;
                        str = standardError.ReadToEnd();
                        process.WaitForExit();
                        process.Close();
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
                        str = "";
                        process.Close();
                    }
                    return str;
                }
                finally
                {
                    if (standardError != null)
                    {
                        standardError.Close();
                        standardError.Dispose();
                    }
                }
                return str;
            }
            return this._ErrorInfo;
        }

        private string RunProcess(string Parameters)
        {
            string str = null;
            if (string.IsNullOrEmpty(this._ErrorInfo))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(this._ffExe, Parameters);
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                StreamReader standardError = null;
                try
                {
                    try
                    {
                        Process process = Process.Start(startInfo);
                        process.WaitForExit();
                        standardError = process.StandardError;
                        str = standardError.ReadToEnd();
                        process.Close();
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine("Message - " + ex.Message + " Exception - " + ex.InnerException + " Source - " + ex.Source);
                        str = string.Empty;
                    }
                    return str;
                }
                finally
                {
                    if (standardError != null)
                    {
                        standardError.Close();
                        standardError.Dispose();
                    }
                }
                return str;
            }
            return this._ErrorInfo;
        }

        // Properties
        private string AllowedVideoFileExtension
        {
            get
            {
                return this._allowedext;
            }
            set
            {
                this._allowedext = value;
            }
        }

        private int ConvertedFileLimitInKB
        {

            get
            {
                return this._ConvertedFileLimitInKB;
            }

            set
            {
                this._ConvertedFileLimitInKB = value;
            }
        }

        private string ErrorInfo
        {
            get
            {
                return this._ErrorInfo;
            }
            set
            {
                this._ErrorInfo = value;
            }
        }

        public string FfmpegPath
        {
            get
            {
                return this._ffExe;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this._ffExe = value;
                }
            }
        }
        public string OutputPath
        {
            get { return _OutputPath; }
            set { _OutputPath = value; }
        }
        private bool Log
        {
            get
            {
                return this._LogOutput;
            }
            set
            {
                this._LogOutput = value;
            }
        }

        public string LogPath
        {
            get
            {
                return this._LogPath;
            }
            set
            {
                this._LogPath = value;
                if (string.IsNullOrEmpty(this._LogPath))
                {
                    this._LogOutput = false;
                }
                else
                {
                    this._LogOutput = true;
                }
            }
        }

        private OutputOptions Settings
        {
            get
            {
                return this._Settings;
            }
            set
            {
                this._Settings = value;
                this._OutputPath = value.SaveLocation;
            }
        }

        public string VideoHeight
        {
            get
            {
                return this._VideoHeight;
            }

            set
            {
                this._VideoHeight = value;
            }
        }

        public string VideoWidth
        {

            get
            {
                return this._VideoWidth;
            }

            set
            {
                this._VideoWidth = value;
            }
        }
    }

    public class Output
    {
        // Fields
        private DateTime _ConversionEnd;

        private DateTime _ConversionStart;

        private string _FileName;

        private FileStream _FileStream;

        private string _Message;

        private string _RawOutput;

        private bool _Success;

        private MemoryStream _VideoStream;

        // Methods
        public Output()
        {
            this._VideoStream = null;
            this._RawOutput = string.Empty;
            this._Success = false;
        }

        public string GetConversionTime()
        {
            TimeSpan span = (TimeSpan)(this._ConversionEnd - this._ConversionStart);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", new object[] { span.Hours, span.Minutes, span.Seconds, span.Milliseconds });
        }

        // Properties
        public DateTime ConversionEnd
        {
            get
            {
                return this._ConversionEnd;
            }

            set
            {
                this._ConversionEnd = value;
            }
        }

        public DateTime ConversionStart
        {

            get
            {
                return this._ConversionStart;
            }

            set
            {
                this._ConversionStart = value;
            }
        }

        public string FileName
        {

            get
            {
                return this._FileName;
            }

            set
            {
                this._FileName = value;
            }
        }

        private FileStream FileStream
        {

            get
            {
                return this._FileStream;
            }

            set
            {
                this._FileStream = value;
            }
        }

        public string Message
        {

            get
            {
                return this._Message;
            }

            set
            {
                this._Message = value;
            }
        }

        public string RawOutput
        {

            get
            {
                return this._RawOutput;
            }

            set
            {
                this._RawOutput = value;
            }
        }

        public bool Success
        {
            get
            {
                return this._Success;
            }

            set
            {
                this._Success = value;
            }
        }

        public MemoryStream VideoStream
        {

            get
            {
                return this._VideoStream;
            }

            set
            {
                this._VideoStream = value;
            }
        }
    }

    public class OutputOptions
    {
        // Fields
        private string _AdditionalArgument = string.Empty;
        private string _Aspect = string.Empty;
        private string _AudioBitRate = string.Empty;
        private string _AudioChannel = "2";
        private string _AudioSampleRate = string.Empty;
        private int _CropBottom = 0;
        private int _CropLeft = 0;
        private int _CropRight = 0;
        private int _CropTop = 0;
        private bool _Deinterlace = false;
        private bool _FTPAfterEncoded = false;
        private string _FTPHost = string.Empty;
        private string _FTPLocation = string.Empty;
        private string _FTPPassword = string.Empty;
        private string _FTPUsername = string.Empty;
        private string _HostTemplate = string.Empty;
        private string _OriginalSourceSaveLocation = string.Empty;
        private bool _OutputAudioOnly = false;
        private string _OutputAudioType = string.Empty;
        private string _OutputVideoType = string.Empty;
        private bool _OverwriteFile = true;
        private bool _Pass2 = false;
        private bool _SaveAfterEncoded = true;
        private string _SaveLocation = string.Empty;
        private bool _SaveOriginalSource = true;
        private int _ThumbHeight = 90;
        private bool _Thumbnail = false;
        private int _ThumbWidth = 120;
        private string _VideoBitRate = string.Empty;
        private string _VideoCodec = "mpeg4";
        private string _VideoFrameRate = string.Empty;
        private string _VideoHeight = "144";
        private string _VideoWidth = "176";

        // Properties
        public string AdditionalArgument
        {
            get
            {
                return this._AdditionalArgument;
            }
            set
            {
                this._AdditionalArgument = value;
            }
        }

        public string AspectRatio
        {
            get
            {
                return this._Aspect;
            }
            set
            {
                this._Aspect = value;
            }
        }

        public string AudioBitRate
        {
            get
            {
                return this._AudioBitRate;
            }
            set
            {
                this._AudioBitRate = value;
            }
        }

        public string AudioChannel
        {
            get
            {
                return this._AudioChannel;
            }
            set
            {
                this._AudioChannel = value;
            }
        }

        public string AudioSampleRate
        {
            get
            {
                return this._AudioSampleRate;
            }
            set
            {
                this._AudioSampleRate = value;
            }
        }

        public int CropBottom
        {
            get
            {
                return this._CropBottom;
            }
            set
            {
                this._CropBottom = value;
            }
        }

        public int CropLeft
        {
            get
            {
                return this._CropLeft;
            }
            set
            {
                this._CropLeft = value;
            }
        }

        public int CropRight
        {
            get
            {
                return this._CropRight;
            }
            set
            {
                this._CropRight = value;
            }
        }

        public int CropTop
        {
            get
            {
                return this._CropTop;
            }
            set
            {
                this._CropTop = value;
            }
        }

        public bool Deinterlace
        {
            get
            {
                return this._Deinterlace;
            }
            set
            {
                this._Deinterlace = value;
            }
        }

        public bool FTPAfterEncoded
        {
            get
            {
                return this._FTPAfterEncoded;
            }
            set
            {
                this._FTPAfterEncoded = value;
            }
        }

        public string FTPFolderLocation
        {
            get
            {
                return this._FTPLocation;
            }
            set
            {
                this._FTPLocation = value;
            }
        }

        public string FTPHost
        {
            get
            {
                return this._FTPHost;
            }
            set
            {
                this._FTPHost = value;
            }
        }

        public string FTPPassword
        {
            get
            {
                return this._FTPPassword;
            }
            set
            {
                this._FTPPassword = value;
            }
        }

        public string FTPUsername
        {
            get
            {
                return this._FTPUsername;
            }
            set
            {
                this._FTPUsername = value;
            }
        }

        public string HostTemplate
        {
            get
            {
                return this._HostTemplate;
            }
            set
            {
                this._HostTemplate = value;
            }
        }

        public string OriginalSourceSaveLocation
        {
            get
            {
                return this._OriginalSourceSaveLocation;
            }
            set
            {
                this._OriginalSourceSaveLocation = value;
            }
        }

        public bool OutputAudioOnly
        {
            get
            {
                return this._OutputAudioOnly;
            }
            set
            {
                this._OutputAudioOnly = value;
            }
        }

        public string OutputAudioType
        {
            get
            {
                return this._OutputAudioType;
            }
            set
            {
                this._OutputAudioType = value;
            }
        }

        public string OutputVideoType
        {
            get
            {
                return this._OutputVideoType;
            }
            set
            {
                this._OutputVideoType = value;
            }
        }

        public bool OverwriteFile
        {
            get
            {
                return this._OverwriteFile;
            }
            set
            {
                this._OverwriteFile = value;
            }
        }

        public bool Pass2Encoding
        {
            get
            {
                return this._Pass2;
            }
            set
            {
                this._Pass2 = value;
            }
        }

        public bool SaveAfterEncoded
        {
            get
            {
                return this._SaveAfterEncoded;
            }
            set
            {
                this._SaveAfterEncoded = value;
            }
        }

        public string SaveLocation
        {
            get
            {
                return this._SaveLocation;
            }
            set
            {
                this._SaveLocation = value;
            }
        }

        public bool SaveOriginalSource
        {
            get
            {
                return this._SaveOriginalSource;
            }
            set
            {
                this._SaveOriginalSource = value;
            }
        }

        public int ThumbHeight
        {
            get
            {
                return this._ThumbHeight;
            }
            set
            {
                this._ThumbHeight = value;
            }
        }

        public int ThumbWidth
        {
            get
            {
                return this._ThumbWidth;
            }
            set
            {
                this._ThumbWidth = value;
            }
        }

        public bool Thunbnail
        {
            get
            {
                return this._Thumbnail;
            }
            set
            {
                this._Thumbnail = value;
            }
        }

        public string VideoBitRate
        {
            get
            {
                return this._VideoBitRate;
            }
            set
            {
                this._VideoBitRate = value;
            }
        }

        public string VideoCodec
        {
            get
            {
                return this._VideoCodec;
            }
            set
            {
                this._VideoCodec = value;
            }
        }

        public string VideoFrameRate
        {
            get
            {
                return this._VideoFrameRate;
            }
            set
            {
                this._VideoFrameRate = value;
            }
        }

        public string VideoHeight
        {
            get
            {
                return this._VideoHeight;
            }
            set
            {
                this._VideoHeight = value;
            }
        }

        public string VideoWidth
        {
            get
            {
                return this._VideoWidth;
            }
            set
            {
                this._VideoWidth = value;
            }
        }
    }



    public static class Utility
    {
        // Methods
        public static string BytesToWords(long bytes)
        {
            int num = 5;
            int index = 0;
            string[] strArray = new string[num];
            strArray[0] = "bytes";
            strArray[1] = "KB";
            strArray[2] = "MB";
            strArray[3] = "GB";
            strArray[4] = "TB";
            double a = bytes;
            while ((a > 1024.0) && (index < (num - 1)))
            {
                a /= 1024.0;
                index++;
            }
            return (Convert.ToInt32(Math.Ceiling(a)).ToString() + strArray[index]);
        }

        public static string GetRandomString()
        {
            return Path.GetRandomFileName().Replace(".", "");
        }
    }

    public sealed class VideoCodecs
    {
        // Fields
        public const string H263 = "h263";
        public const string MPEG4 = "mpeg4";

        // Methods
        private VideoCodecs()
        {
        }
    }

    public class VideoFile
    {
        // Fields
        private string _Path;

        private string _AudioFormat;

        private double _BitRate;

        private TimeSpan _Duration;

        private bool _Found;

        private int _Height;

        private bool _infoGathered;

        private string _Message;

        private string _RawInfo;

        private string _VideoFormat;

        private int _Width;

        // Methods
        public VideoFile(string path)
        {
            this._Path = path;
            this.Initialize();
        }

        private void Initialize()
        {
            this._infoGathered = false;
            this._Found = false;
            if (string.IsNullOrEmpty(this._Path))
            {
                this._Message = "Could not find the location of the file to convert";
            }
            else if (!File.Exists(this._Path))
            {
                this._Message = "\"" + this._Path + "\" does not exist.";
            }
            else
            {
                this._Found = true;
            }
        }

        // Properties
        public string AudioFormat
        {

            get
            {
                return this._AudioFormat;
            }

            set
            {
                this._AudioFormat = value;
            }
        }

        public double BitRate
        {

            get
            {
                return this._BitRate;
            }
            [CompilerGenerated]
            set
            {
                this._BitRate = value;
            }
        }

        public TimeSpan Duration
        {

            get
            {
                return this._Duration;
            }

            set
            {
                this._Duration = value;
            }
        }

        public bool Found
        {

            get
            {
                return this._Found;
            }

            set
            {
                this._Found = value;
            }
        }

        public int Height
        {

            get
            {
                return this._Height;
            }

            set
            {
                this._Height = value;
            }
        }

        public bool infoGathered
        {

            get
            {
                return this._infoGathered;
            }

            set
            {
                this._infoGathered = value;
            }
        }

        public string Message
        {

            get
            {
                return this._Message;
            }
            [CompilerGenerated]
            set
            {
                this._Message = value;
            }
        }

        public string Path
        {
            get
            {
                return this._Path;
            }
            set
            {
                this._Path = value;
            }
        }

        public string RawInfo
        {

            get
            {
                return this._RawInfo;
            }
            [CompilerGenerated]
            set
            {
                this._RawInfo = value;
            }
        }

        public string VideoFormat
        {

            get
            {
                return this._VideoFormat;
            }

            set
            {
                this._VideoFormat = value;
            }
        }

        public int Width
        {

            get
            {
                return this._Width;
            }

            set
            {
                this._Width = value;
            }
        }
    }











}
