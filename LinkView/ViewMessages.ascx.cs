using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ETUDataControls;
using System.Data;
using System.Web.Security;
using System.Text;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Threading;

public partial class LI_UserControls_ViewMessages : System.Web.UI.UserControl
{
    public delegate string ProcessEmailsAsync(string username, GridView dgvUpdateRecords2, string emailFrom, GridView gvUserList);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            BindGridView();
        }
    }
    private ProfileCommon GetProfile()
    {
        ProfileCommon p = (ProfileCommon)Session["profile"];
        return p;
    }
    
    protected void BindGridView()
    {
        DataBridgeConnector dbc = new DataBridgeConnector(System.Configuration.ConfigurationManager.ConnectionStrings["Linkview"].ToString());
        object[] objArray = new object[1];
        objArray.SetValue(GetProfile().UserName, 0);

        DataSet ds = dbc.GetSqlDataSet("sp_UP_GetMessages", objArray);
        dgvUpdateRecords2.DataSource = ds.Tables[0].DefaultView;
        dgvUpdateRecords2.DataBind();

        if (ds.Tables[0].Rows.Count == 0)
            btnTopSend.Visible = false;
    }

    protected void dgvUpdateRecords2_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvUpdateRecords2.EditIndex = e.NewEditIndex;
        BindGridView();
    }
    protected void dgvUpdateRecords2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgvUpdateRecords2.EditIndex = -1;
        BindGridView();
    }
    protected void dgvUpdateRecords2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void dgvUpdateRecords2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvUpdateRecords2.EditIndex = -1;
        dgvUpdateRecords2.PageIndex = e.NewPageIndex;
        BindGridView();

    }
    protected void dgvUpdateRecords2_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            int messageID = Convert.ToInt32(dgvUpdateRecords2.DataKeys[e.RowIndex].Value.ToString());
            DataBridgeConnector dbc = new DataBridgeConnector(System.Configuration.ConfigurationManager.ConnectionStrings["Linkview"].ToString());
            object[] objDB = new object[1];
            objDB.SetValue(messageID, 0);

            dbc.ExecuteSqlNonQuery("sp_UP_DeleteMessageByID", false, objDB);
            dgvUpdateRecords2.EditIndex = -1;
            BindGridView();
        }

        catch (Exception ex)
        { }


    }
    protected void dgvUpdateRecords2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int messageID = Convert.ToInt32(dgvUpdateRecords2.DataKeys[e.RowIndex].Value.ToString());
            string messageText = ((TextBox)(dgvUpdateRecords2.Rows[e.RowIndex].FindControl("lblMessage"))).Text;
            
            DataBridgeConnector dbc = new DataBridgeConnector(System.Configuration.ConfigurationManager.ConnectionStrings["Linkview"].ToString());
            object[] objDB = new object[2];
            objDB.SetValue(messageID, 0);
            objDB.SetValue(messageText, 1);
           
            dbc.ExecuteSqlNonQuery("sp_UP_UpdateMessageText", false,  objDB);
            dgvUpdateRecords2.EditIndex = -1;
            BindGridView();
        }
        catch (Exception ex)
        { throw new Exception(ex.Message, ex.InnerException); }
    }
   
    protected void btnTopSend_Click(object sender, ImageClickEventArgs e)
    {
       ProcessEmailsAsync pea;
       ResendMMEmail rsmm = new ResendMMEmail();
        GridView gvUList = this.Parent.Page.FindControl("gvUserList") as GridView;
        pea = new ProcessEmailsAsync(rsmm.ProcessEmails);
       pea.BeginInvoke(GetProfile().UserName, dgvUpdateRecords2, Membership.GetUser(GetProfile().UserName).Email, gvUList, null, null);
        
       Response.Redirect("~/UserUpT2.aspx?ms=nt");
       
    }
    
}