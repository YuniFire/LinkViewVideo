<%@ WebHandler Language="C#" Class="ImageHandler" %>

using System;
using System.Web;
using System.Data;
using ETUDataControls;
using System.IO;
public class ImageHandler : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        DataBridgeConnector dbc = new DataBridgeConnector("");

        DataTable dt = dbc.GetTable("Select ImagePath From UP_Video uv inner join UP_Messages um on uv.videoid = um.videoid where messageid = " + context.Request["i"]);
        
        context.Response.ContentType = "image/png";
        context.Response.BinaryWrite((byte[])File.ReadAllBytes(dt.Rows[0]["ImagePath"] as string));
        //context.Response.BinaryWrite((byte[])File.ReadAllBytes(@"C:\support\LinkView\ConvertedTemp\LinkviewFile267025049-10148Wildlife.png"));
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}