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
using System.Web.Services;
using System.Configuration;
using System.Data.OleDb;
using System.Collections;
using System.Web.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
public partial class UserUpT2 : System.Web.UI.Page
{
    private string msg = string.Empty;
    public delegate string ProcessEmailsAsync(GridView gv, string formlname, string filename, int msgID);
    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (!Page.IsPostBack)
        {
            hidUserName.Value = GetProfile().UserName;
            BindGridView();
            if (Request.QueryString["m"] != null)
            {
                if (Request.QueryString["m"] == "nt")
                {
                    litMsg.Text = "SUCCESSFUL UPLOAD AND FILE SENT!";

                }
                else
                {
                    litMsg.Text = "ERROR- PLEASE CONTACT Linkview";
                }
                litMsg.Visible = true;
                ClearQueryString("m");
            }
            if (Request.QueryString["ms"] != null)
            {
                if (Request.QueryString["ms"] == "nt")
                {
                    litMsg.Text = "VIDEO MESSAGE SENT!";

                }
                else
                {
                    litMsg.Text = "ERROR- PLEASE CONTACT Linkview";
                }
                litMsg.Visible = true;
                ClearQueryString("ms");
            }
        }
    }
    [WebMethod()]
    public static void UpdateNotification()
    {
        
    }
    private void ClearQueryString(string paramName)
    {
        PropertyInfo isreadonly = typeof(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        isreadonly.SetValue(this.Request.QueryString, false, null);
        this.Request.QueryString.Remove(paramName);
    }
    private ProfileCommon GetProfile()
    {
        ProfileCommon p = (ProfileCommon)Session["profile"];
        return p;
    }
    private void BindGridView()
    {
        DataBridgeConnector dbc = new DataBridgeConnector(string.Empty);
        object[] objArray = new object[1];
        objArray.SetValue(GetProfile().UserName, 0);

        DataSet ds = dbc.GetSqlDataSet("sp_UP_GetListOfUsers", objArray);
        gvUserList.DataSource = ds.Tables[0].DefaultView;
        gvUserList.DataBind();
    }
   
    protected void gvUserList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DropDownList ddl = (DropDownList)e.Row.FindControl("ddlWireless");
            if (((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray[5].ToString() == "")
            {
                ddl.Text = "N/A";
            }
            else
            {
                ddl.Text = ddl.Items.FindByValue(((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray[5].ToString()).Value;
            }
        }

    }
    private Guid GetUserID()
    {
        DataBridgeConnector dbc = new DataBridgeConnector("");
        object[] objArray = new object[1];
        objArray.SetValue(GetProfile().UserName, 0);

        DataSet ds = dbc.GetSqlDataSet("sp_UP_GetUserID", objArray);
        return (Guid)ds.Tables[0].Rows[0]["UserID"];
    }
   
    protected void gvUserList_DataBound(object sender, EventArgs e)
    {
        if (gvUserList.HeaderRow != null)
        {
            CheckBox cb = (CheckBox)gvUserList.HeaderRow.FindControl("HeaderLevelCheckBox");
            cb.Attributes["onclick"] = "ChangeAllCheckBoxStates(this.checked);";
            ClientScript.RegisterArrayDeclaration("CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
            foreach (GridViewRow gvr in gvUserList.Rows)
            {
                CheckBox cbb = (CheckBox)gvr.FindControl("RowLevelCheckBox");
                cbb.Attributes["onclick"] = "ChangeHeaderAsNeeded();";

                ClientScript.RegisterArrayDeclaration("CheckBoxIDs", String.Concat("'", cbb.ClientID, "'"));
            }
        }
    }
    
    protected void gvUserList_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
