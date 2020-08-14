using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ETUDataControls;
using System.Data;
using System.Text.RegularExpressions;

public partial class LI_UserControls_UploadContacts : System.Web.UI.UserControl
{
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
    private void BindGridView()
    {
        DataBridgeConnector dbc = new DataBridgeConnector(System.Configuration.ConfigurationManager.ConnectionStrings["Linkview"].ToString());
        object[] objArray = new object[1];
        objArray.SetValue(GetProfile().UserName, 0);

        DataSet ds = dbc.GetSqlDataSet("sp_UP_GetListOfUsers", objArray);
        gvUserListContact.DataSource = ds.Tables[0].DefaultView;
        gvUserListContact.DataBind();
    }
    protected void gvUserListContact_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvUserListContact.EditIndex = e.NewEditIndex;
        BindGridView();
    }
    protected void gvUserListContact_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvUserListContact.EditIndex = -1;
        BindGridView();
    }
    protected void gvUserListContact_RowDataBound(object sender, GridViewRowEventArgs e)
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
    protected void gvUserListContact_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void gvUserListContact_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvUserListContact.EditIndex = -1;
        gvUserListContact.PageIndex = e.NewPageIndex;
        BindGridView();

    }
    protected void gvUserListContact_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        bool bIsValid = true;
        try
        {
            int contactUserID = Convert.ToInt32(gvUserListContact.DataKeys[e.RowIndex].Value.ToString());
            string firstName = ((TextBox)(gvUserListContact.Rows[e.RowIndex].FindControl("txtFirstName"))).Text;
            string lastName = ((TextBox)(gvUserListContact.Rows[e.RowIndex].FindControl("txtLastName"))).Text;
            string cellPhone = ((TextBox)(gvUserListContact.Rows[e.RowIndex].FindControl("txtCellPhone"))).Text;
            string wirelessCarrier = ((DropDownList)(gvUserListContact.Rows[e.RowIndex].FindControl("ddlwireless"))).SelectedValue;
            string email = ((TextBox)(gvUserListContact.Rows[e.RowIndex].FindControl("txtEmail"))).Text;
            if (!Regex.IsMatch(cellPhone, @"^\d+$"))
            {
                cellPhone = gvUserListContact.Rows[e.RowIndex].Cells[3].Text;
                litError.Text = "<font color='red'>Please enter valid cell phone number</font>";
                litError.Visible = true;
                bIsValid = false;
            }
            
            if (!Regex.IsMatch(email, @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
            {
                email = gvUserListContact.Rows[e.RowIndex].Cells[5].Text;
                litError.Text = "<font color='red'>Please enter valid email address</font>";
                litError.Visible = true;
                bIsValid = false;
            }
            if (bIsValid)
            {
                DataBridgeConnector dbc = new DataBridgeConnector(System.Configuration.ConfigurationManager.ConnectionStrings["Linkview"].ToString());
                object[] objDB = new object[6];
                objDB.SetValue(contactUserID, 0);
                objDB.SetValue(firstName, 1);
                objDB.SetValue(lastName, 2);
                objDB.SetValue(cellPhone, 3);
                objDB.SetValue(wirelessCarrier, 4);
                objDB.SetValue(email, 5);

                dbc.ExecuteSqlNonQuery("sp_UP_UpdateUserContact", false, objDB);
                
            }
            gvUserListContact.EditIndex = -1;
            BindGridView();
        }
        catch (Exception ex)
        { throw new Exception(ex.Message, ex.InnerException); }
    }
    protected void btnTopAddUsers_Click(object sender, ImageClickEventArgs e)
    {
        AddUser(tbFirstName.Text,tbLastName.Text,tbCellPhone.Text,ddlWireless2.SelectedValue,tbEmail.Text);
    }
    protected void gvUserListContact_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            int contactUserID = Convert.ToInt32(gvUserListContact.DataKeys[e.RowIndex].Value.ToString());
            DataBridgeConnector dbc = new DataBridgeConnector(System.Configuration.ConfigurationManager.ConnectionStrings["Linkview"].ToString());
            object[] objDB = new object[1];
            objDB.SetValue(contactUserID, 0);
            
            dbc.ExecuteSqlNonQuery("sp_UP_DeleteUserByID", false, objDB);
            gvUserListContact.EditIndex = -1;
            BindGridView();
        }

        catch (Exception ex)
        { }


    }
    private void AddUser(string userFirstName, string userLastName, string userCellPhone, string userCarrier, string userEmail)
    {
        bool bIsValid = true;
        if (!Regex.IsMatch(userCellPhone, @"^\d+$"))
        {
            userCellPhone = string.Empty;
            litError.Text = "<font color='red'>Please enter valid cell phone number</font>";
            litError.Visible = true;
            RegularExpressionValidator2.Visible = true;
            bIsValid = false;
        }

        if (!Regex.IsMatch(userEmail, @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
        {
            userEmail = string.Empty;
            litError.Text = "<font color='red'>Please enter valid email address</font>";
            litError.Visible = true;
            bIsValid = false;
        }
        if (bIsValid)
        {
            DataBridgeConnector dbc = new DataBridgeConnector(System.Configuration.ConfigurationManager.ConnectionStrings["Linkview"].ToString());
            object[] objDB = new object[6];
            objDB.SetValue(GetProfile().UserName, 0);
            objDB.SetValue(userFirstName, 1);
            objDB.SetValue(userLastName, 2);
            objDB.SetValue(userCellPhone, 3);
            objDB.SetValue(userCarrier, 4);
            objDB.SetValue(userEmail, 5);

            dbc.ExecuteSqlNonQuery("sp_UP_InsertUserContact", false, objDB);
        }
        BindGridView();
        Response.Redirect("~/UserUpT2.aspx");
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {

    }
}