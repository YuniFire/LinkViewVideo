using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ETUDataControls;
using System.Data;
using EncoderLibraries;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;
using System.Collections;
using System.Web.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.Remoting.Messaging;

/// <summary>
/// Summary description for SendMMEmail
/// </summary>
public class SendMMEmail
{
    public SendMMEmail()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    //public string ProcessEmails(string convertedFileName, string formalName, string fileName,
    //    GridView gvUserList,string mobileText, string email,int messageID)
    //{
    //    MemoryStream ms = new MemoryStream();
    //    string msg = string.Empty;
    //    try
    //    {
    //        int i = 0;
    //        MMSMessaging.Messaging messaging = new MMSMessaging.Messaging();
    //        if (gvUserList.Rows.Count > 0)
    //        {
    //            //string fileFormalName = Path.GetFileName(formalName);
    //            //messaging.FormalName = fileFormalName;
    //            while (i < gvUserList.Rows.Count)
    //            {
    //                CheckBox cbb = (CheckBox)gvUserList.Rows[i].FindControl("RowLevelCheckBox");
    //                if (cbb.Checked)
    //                {
    //                    Label lbCell = (Label)gvUserList.Rows[i].FindControl("lblCellPhone");
    //                    Label lbEmail = (Label)gvUserList.Rows[i].FindControl("lblEmail");
    //                    string recipient = lbCell.Text;
    //                    DropDownList lbCarrier = (DropDownList)gvUserList.Rows[i].FindControl("ddlWireless");
    //                    messaging.Attachment = convertedFileName;
    //                    if (recipient != string.Empty && lbCarrier.Text != string.Empty)
    //                    {

    //                        if (lbCarrier.SelectedValue != "Unknown")
    //                            recipient = recipient + lbCarrier.Text;
    //                    }

    //                    DataBridgeConnector dbc = new DataBridgeConnector("");
    //                    object[] objArray2 = new object[1];
    //                    objArray2.SetValue(messageID, 0);
    //                    DataSet ds2 = dbc.GetSqlDataSet("sp_UP_GetVideoByID", objArray2);
    //                    ms = new MemoryStream((byte[])ds2.Tables[0].Rows[0]["Video"]);
    //                    string videoName = ds2.Tables[0].Rows[0]["VideoName"].ToString();
    //                    string fileFormalName = Path.GetFileName(videoName);
    //                    messaging.FormalName = fileFormalName;

    //                    messaging.SendEmailFromDBGo("You have received a file from Linkview", mobileText,
    //                        email,
    //                        recipient, true, "", "", lbEmail.Text,ms);
    //                }
    //                //send message asynchronously
    //                i++;
    //            }
    //        }

    //        return msg;
    //    }
    //    catch (Exception ex)
    //    {
    //        return "ERROR- PLEASE CONTACT Linkview";
    //    }
    //    finally
    //    {
    //        ms.Dispose();
    //        ms.Close();
    //        FileInfo fi = new FileInfo(fileName);
    //        fi.Delete();
    //        fi = new FileInfo(convertedFileName);
    //        fi.Delete();
    //    }

    //}
}

public class ResendMMEmail
{
    public ResendMMEmail()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //public string ProcessEmails(string username, GridView dgvUpdateRecords2, string emailFrom)
    //{
    //    DataBridgeConnector dbc = new DataBridgeConnector(string.Empty);
    //    object[] objArray = new object[1];
    //    objArray.SetValue(username, 0);

    //    DataSet ds = dbc.GetSqlDataSet("sp_UP_GetListOfUsers", objArray);

    //    MMSMessaging.Messaging messaging = new MMSMessaging.Messaging();

    //    int gg = 0;
    //    bool goodSend = false;
    //    if (ds.Tables[0].Rows.Count > 0)
    //    {
    //        while (gg < dgvUpdateRecords2.Rows.Count)
    //        {
    //            CheckBox cbb = (CheckBox)dgvUpdateRecords2.Rows[gg].FindControl("chkMessage");
    //            if (cbb.Checked)
    //            {

    //                int i = 0;
    //                string videoName = string.Empty;
    //                string messageText = string.Empty;
    //                StringBuilder sbRecipient = new StringBuilder();
    //                StringBuilder sbEmail = new StringBuilder();
    //                int messageID = Convert.ToInt32(dgvUpdateRecords2.DataKeys[gg].Value.ToString());

    //                dbc = new DataBridgeConnector(string.Empty);
    //                object[] objArray2 = new object[1];
    //                objArray2.SetValue(messageID, 0);
    //                Label lbMessage = (Label)dgvUpdateRecords2.Rows[gg].FindControl("lblMessage");
    //                DataSet ds2 = dbc.GetSqlDataSet("sp_UP_GetVideoByID", objArray2);
    //                MemoryStream ms = new MemoryStream(); 
    //                videoName = ds2.Tables[0].Rows[0]["VideoName"].ToString();

    //                while (i < ds.Tables[0].Rows.Count)
    //                {
    //                    string recipient = ds.Tables[0].Rows[i]["CellPhone"].ToString();
    //                    string email = ds.Tables[0].Rows[i]["Email"].ToString();

    //                    if (ds.Tables[0].Rows[i]["WirelessCarrier"].ToString() != "Unknown")
    //                    {
    //                        string lbCarrier = ds.Tables[0].Rows[i]["WirelessCarrier"].ToString();
    //                        if (lbCarrier != "Unknown")
    //                            recipient = recipient + lbCarrier;
    //                    }

    //                    messageText = lbMessage.Text;
    //                    string fileFormalName = Path.GetFileName(videoName);
    //                    messaging.FormalName = fileFormalName;
    //                    ms = new MemoryStream((byte[])ds2.Tables[0].Rows[0]["Video"]);
    //                    messaging.SendEmailFromDBGo("You have received a file from Linkview", messageText,
    //                          emailFrom,
    //                          recipient, true, "", "", email, ms);
    //                    //send message asynchronously
    //                    i++;
    //                }

    //                ms.Dispose();
    //                ms.Close();

    //                dbc = new DataBridgeConnector(string.Empty);
    //                object[] objDB = new object[1];
    //                objDB.SetValue(messageID, 0);

    //                dbc.ExecuteSqlNonQuery("sp_UP_UpdateMessageSent", false, objDB);
    //                goodSend = true;
    //            }
    //            gg++;
    //        }
    //    }
    //    string msg = "Fail";
    //    if (goodSend)
    //    {
    //        msg = "Success";
    //    }
    //    return msg;
    //}
    public string SendSMTPEmail(int id, string recipient, string email)
    {

        System.Net.WebClient client = new System.Net.WebClient();
        client.Headers[System.Net.HttpRequestHeader.ContentType] = "application/json";

        string response = client.DownloadString(ConfigurationManager.AppSettings["url"] + ConfigurationManager.AppSettings["SMTP"] + "?ID=" + id + "&email=" + email + "&mobiRecipient=" + recipient);
        return "Success";
    }
    public string ProcessEmails(string username, GridView dgvUpdateRecords2, string emailFrom, GridView gvUserList)
    {
        DataBridgeConnector dbc = new DataBridgeConnector("");
        object[] objArray = new object[1];
        objArray.SetValue(username, 0);

        DataSet ds = dbc.GetSqlDataSet("sp_UP_GetListOfUsers", objArray);

        MMSMessaging.Messaging messaging = new MMSMessaging.Messaging();

        int gg = 0;
        bool goodSend = false;
        if (ds.Tables[0].Rows.Count > 0)
        {
            while (gg < dgvUpdateRecords2.Rows.Count)
            {
                CheckBox cbb = (CheckBox)dgvUpdateRecords2.Rows[gg].FindControl("chkMessage");
                if (cbb.Checked)
                {

                    int i = 0;
                    string videoName = string.Empty;
                    string messageText = string.Empty;
                    StringBuilder sbRecipient = new StringBuilder();
                    StringBuilder sbEmail = new StringBuilder();
                    int messageID = Convert.ToInt32(dgvUpdateRecords2.DataKeys[gg].Value.ToString());

                    dbc = new DataBridgeConnector("");
                    object[] objArray2 = new object[1];
                    objArray2.SetValue(messageID, 0);
                    Label lbMessage = (Label)dgvUpdateRecords2.Rows[gg].FindControl("lblMessage");
                    //DataSet ds2 = dbc.GetSqlDataSet("sp_UP_GetVideoByID", objArray2);
                    //MemoryStream ms = new MemoryStream((byte[])ds2.Tables[0].Rows[0]["Video"]);
                    //videoName = ds2.Tables[0].Rows[0]["VideoName"].ToString();
                    //string videoPath = ds2.Tables[0].Rows[0]["VideoPath"].ToString();
                    while (i < gvUserList.Rows.Count)
                    {
                        CheckBox cbb2 = (CheckBox)gvUserList.Rows[i].FindControl("RowLevelCheckBox");
                        if (cbb2.Checked)
                        {
                            Label lbCell = (Label)gvUserList.Rows[i].FindControl("lblCellPhone");
                            Label lbEmail = (Label)gvUserList.Rows[i].FindControl("lblEmail");

                            string recipient = lbCell.Text;
                            DropDownList lbCarrier = (DropDownList)gvUserList.Rows[i].FindControl("ddlWireless");

                            if (recipient != string.Empty && lbCarrier.SelectedValue != "Unknown")
                            {
                                //if (lbCarrier.SelectedValue != "Unknown")

                            }

                            string email = lbEmail.Text;

                            SendSMTPEmail(messageID, recipient, email);
                            //messageText = lbMessage.Text;
                            //string fileFormalName = Path.GetFileName(videoName);
                            //messaging.FormalName = fileFormalName;
                            //messaging.SendEmailFromDBGo("You have received a file from Linkview", messageText,
                            //      emailFrom,
                            //      recipient, true, "", "", email, null, videoPath);
                            //send message asynchronously

                        }
                        i++;
                        //ms.Dispose();
                        //ms.Close();


                    }
                    //TODO: build up user list then bulk update this call, change table structure
                    //dbc = new DataBridgeConnector("");
                    //object[] objDB = new object[1];
                    //objDB.SetValue(messageID, 0);

                    //dbc.ExecuteSqlNonQuery("sp_UP_UpdateMessageSent", false, objDB);
                    goodSend = true;
                }
                gg++;
            }
        }
        string msg = "Fail";
        if (goodSend)
        {
            msg = "Success";
        }
        return msg;
    }
    //public string ProcessOriginalFileEmails(GridView gvUserList, string mobileText, string emailFrom,int messageID)
    //{
    //    DataBridgeConnector dbc = new DataBridgeConnector("");

    //    MMSMessaging.Messaging messaging = new MMSMessaging.Messaging();

    //    int gg = 0;
    //    bool goodSend = false;

    //        while (gg < gvUserList.Rows.Count)
    //        {
    //            CheckBox cbb = (CheckBox)gvUserList.Rows[gg].FindControl("RowLevelCheckBox");

    //            if (cbb.Checked)
    //            {

    //                int i = 0;
    //                string videoName = string.Empty;
    //                string messageText = string.Empty;
    //                Label lbCell = (Label)gvUserList.Rows[gg].FindControl("lblCellPhone");
    //                Label lbEmail = (Label)gvUserList.Rows[gg].FindControl("lblEmail");
    //                string recipient = lbCell.Text;
    //                DropDownList lbCarrier = (DropDownList)gvUserList.Rows[gg].FindControl("ddlWireless");

    //                if (recipient != string.Empty && lbCarrier.Text != string.Empty)
    //                {

    //                    if (lbCarrier.SelectedValue != "Unknown")
    //                        recipient = recipient + lbCarrier.Text;
    //                }

    //                dbc = new DataBridgeConnector("");
    //                object[] objArray2 = new object[1];
    //                objArray2.SetValue(messageID, 0);
    //                DataSet ds2 = dbc.GetSqlDataSet("sp_UP_GetVideoByID", objArray2);
    //                MemoryStream ms = new MemoryStream();
    //                videoName = ds2.Tables[0].Rows[0]["VideoName"].ToString();

    //                messageText = mobileText;
    //                string fileFormalName = Path.GetFileName(videoName);
    //                messaging.FormalName = fileFormalName;
    //                //TODO get video from data lake
    //                ms = new MemoryStream((byte[])ds2.Tables[0].Rows[0]["Video"]);
    //                messaging.SendEmailFromDBGo("You have received a file from Linkview", messageText,
    //                      emailFrom,
    //                      recipient, true, "", "", lbEmail.Text, ms);
    //                //send message asynchronously


    //                ms.Dispose();
    //                ms.Close();

    //                goodSend = true;
    //            }
    //            gg++;
    //        }

    //    string msg = "Fail";
    //    if (goodSend)
    //    {
    //        msg = "Success";
    //    }
    //    return msg;
    //}
}
