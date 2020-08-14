<%@ WebHandler Language="C#" Class="Handler" %>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Configuration;
using ETUDataControls;
using System.Data;
using EncoderLibraries;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Collections;
using System.Web.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.Remoting.Messaging;
public class Handler : IHttpHandler {
    public delegate string ConversionAsync(string cfilePath, string cuserName, string ctempName);
    private AsyncResult ar;
    public void ProcessRequest (HttpContext context) {
        //Convert File to mp4, save to database, send mms message, send email message
        Conversions.ConversionService cs = new Conversions.ConversionService();
        //string fileName = string.Empty;
        string convertedFileName = string.Empty;
        string fileExt = string.Empty;
        string tempName = string.Empty;

        //Fetch the Uploaded File.
        HttpPostedFile postedFile = context.Request.Files[0];
        string mobiText = context.Request["txtMobileText"] as string;//context.Request["ctl00$ContentPlaceHolder1$txtMobileText"] as string;

        //Set the Folder Path.
        //string folderPath = context.Server.MapPath("~/Uploads/");

        //Set the File Name.
        string fileName = Path.GetFileName(postedFile.FileName);

        if (!string.IsNullOrEmpty(fileName))
        {
            Random rd = new Random();
            int lastPos = fileName.LastIndexOf(".");
            fileExt = fileName.Substring(lastPos, (fileName.Length) - lastPos);
            if (fileExt == ".avi" ||
                fileExt == ".wmv" ||
                fileExt == ".mov" ||
                fileExt == ".mpg" ||
                fileExt == ".mp4" ||
                fileExt == ".mpeg" ||
                fileExt == ".flv")
            {
                //fileName = @"C:\support\" + rd.Next() + fileExt;
                //FileStream fsLoad = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                //fsLoad.Write(FileUpload1.FileBytes, 0, FileUpload1.FileBytes.Count());

                //FileUpload1.FileContent.Flush();
                //fsLoad.Flush();
                //fsLoad.Close();
                //FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                //string pathFile = cs.ConvertFile(fs);

                tempName = "LinkviewFile" + rd.Next() + "-";

                fileName = tempName + fileName;

                string filePath = ConfigurationManager.AppSettings["VidFileUpload"].ToString() + fileName;

                //Save the File in Folder.
                postedFile.SaveAs(filePath);

                //Send File details in a JSON Response.
                string json = new JavaScriptSerializer().Serialize(
                    new
                    {
                        name = fileName
                    });
                string userName = context.Request["hidUserName"] as string; //context.Request["ctl00$ContentPlaceHolder1$hidUserName"] as string;
                cs.VideoText = mobiText;
                //Change this code to send to Azure Blob Storage. After FFMPEG Conversion ***TODO - Venkatesh
                
                ConversionAsync pea;

                pea = new ConversionAsync(cs.ConvertFile);
                IAsyncResult iar = pea.BeginInvoke(filePath, userName, tempName, null, null);
                pea.EndInvoke(iar);
                //context.Response.StatusCode = (int)HttpStatusCode.OK;
                //context.Response.Redirect("~/UserUpT2.aspx?m=nt");
                //context.Response.End();
                //convertedFileName = cs.ConvertFile(filePath, userName, tempName);
                //int messageID = cs.MessageID;
                //have to get a handle to converted file
            }
        }
        
    }
    private void ConversionAsyncResult(IAsyncResult iar)
    {
        ar = (AsyncResult)iar;
        ConversionAsync ca = (ConversionAsync)ar.AsyncDelegate;
        ca.EndInvoke(iar);
        string s = string.Empty;
    }
    public bool IsReusable {
        get {
            return false;
        }
    }


}