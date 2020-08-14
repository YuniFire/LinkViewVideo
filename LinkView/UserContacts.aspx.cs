using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ETUDataControls;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;
using System.Collections;
using System.Web.Security;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;
public partial class LI_UserContacts : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       
    }
    private Guid GetUserID()
    {
        DataBridgeConnector dbc = new DataBridgeConnector("");
        object[] objArray = new object[1];
        objArray.SetValue(GetProfile().UserName, 0);

        DataSet ds = dbc.GetSqlDataSet("sp_UP_GetUserID", objArray);
        return (Guid)ds.Tables[0].Rows[0]["UserID"];
    }
    private void PerformBulkCopy(string tableName, DataTable dt, ArrayList columnMapping)
    {
       
        using (SqlConnection destinationConnection =
                   new SqlConnection(ConfigurationManager.ConnectionStrings["Linkview"].ToString()))
        {
            destinationConnection.Open();

            // Set up the bulk copy object. 
            // Note that the column positions in the source
            // data reader match the column positions in 
            // the destination table so there is no need to
            // map columns.
            using (SqlBulkCopy bulkCopy =
                       new SqlBulkCopy(destinationConnection))
            {
                bulkCopy.DestinationTableName =
                   tableName;

                string columnOriginal = string.Empty;
                string columnNew = string.Empty;
                int i = 0;

                while (i < columnMapping.Count)
                {
                    string[] cols = columnMapping[i].ToString().Split('|');
                    columnOriginal = cols[0];
                    columnNew = cols[1];
                    bulkCopy.ColumnMappings.Add(columnOriginal, columnNew);
                    i++;
                }

                try
                {
                    // Write from the source to the destination.
                    bulkCopy.WriteToServer(dt);
                    bulkCopy.ColumnMappings.Clear();
                    //BindGridView();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
                finally
                {
                    // Close the SqlDataReader. The SqlBulkCopy
                    // object is automatically closed at the end
                    // of the using block.


                }
            }
        }

    }
    protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
    {
        if (FileUpload2.HasFile && FileUpload2.FileBytes.Length <= 250000000)
        {
            string FileName = "Upload" + Path.GetFileName(FileUpload2.PostedFile.FileName);
            string Extension = Path.GetExtension(FileUpload2.PostedFile.FileName);
            string FolderPath = ConfigurationManager.AppSettings["FolderPath"];

            string FilePath = Server.MapPath("~/admin/");
            FileUpload2.SaveAs(FilePath + FileName);
            Import_To_Grid(FilePath, Extension, FileName);
            Response.Redirect("~/UserUpT2.aspx");
        }
    }
    private void Import_To_Grid(string FilePath, string Extension, string FileName)
    {
        string conStr = "";
        switch (Extension)
        {
            case ".xls": //Excel 97-03
                conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                break;
            case ".xlsx": //Excel 07
                conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                break;
            case ".csv": //Excel 07
                conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                break;
        }
        conStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + FilePath + ";" + "Extended Properties='text;HDR=YES;'";
        OleDbConnection connExcel = new OleDbConnection(conStr);
        OleDbCommand cmdExcel = new OleDbCommand();
        OleDbDataAdapter oda = new OleDbDataAdapter();
        DataTable dt = new DataTable();
        cmdExcel.Connection = connExcel;

        //Get the name of First Sheet
        connExcel.Open();
        DataTable dtExcelSchema;
        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
        connExcel.Close();

        //Read Data from First Sheet
        connExcel.Open();
        cmdExcel.CommandText = "SELECT * From [" + FileName + "]";
        oda.SelectCommand = cmdExcel;
        
        DataColumn dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.Guid");
        dc.DefaultValue = GetUserID();
        dc.ColumnName = "UserID";

        dt.Columns.Add(dc);
        dt.AcceptChanges();
        //oda.Fill(dt);

        dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.String");
        dc.DefaultValue = string.Empty;
        dc.ColumnName = "WirelessCarrier";

        dt.Columns.Add(dc);
        dt.AcceptChanges();

        oda.Fill(dt);
        foreach (DataRow dr in dt.Rows)
        {
            string cellphone = dr["Mobile Phone"].ToString();
            if (!Regex.IsMatch(cellphone, @"^\d+$"))
            {
                cellphone = string.Empty;
            }
           
            string email = dr["E-mail Address"].ToString();
            if (!Regex.IsMatch(email, @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
            {
                email = string.Empty;
            }
        }
        connExcel.Close();
        ArrayList listOfColumns = new ArrayList();

        listOfColumns.Add("UserID|UserID");
        listOfColumns.Add("First Name|FirstName");
        listOfColumns.Add("Last Name|LastName");
        listOfColumns.Add("Mobile Phone|CellPhone");
        listOfColumns.Add("WirelessCarrier|WirelessCarrier");
        listOfColumns.Add("E-mail Address|Email");

        try
        {
            int i = 0;

            while (i < dt.Columns.Count)
            {
                if (dt.Columns[i].Caption == "ï»¿First Name")
                {
                    dt.Columns["ï»¿First Name"].ColumnName = "First Name";
                    dt.AcceptChanges();
                }
                i++;
            }
                
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message,ex.InnerException);
        }

        PerformBulkCopy("dbo.UP_Contacts", dt, listOfColumns);
        
        litMsg.Text = "Bulk Upload Completed!  Click to return to <a href='userupt.aspx'>Video Messaging</a>";
    }


    private ProfileCommon GetProfile()
    {
        ProfileCommon p = (ProfileCommon)Session["profile"];
        return p;
    }
    

}