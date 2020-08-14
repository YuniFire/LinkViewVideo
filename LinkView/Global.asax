<%@ Application Language="C#" %>
<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        Application["OnlineUsers"] = 0;

    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs
        Exception ex = Server.GetLastError().GetBaseException();
        //Context.Items["msg"] = ex.Message;
        //Context.Items["source"] = ex.Source;
        throw new Exception(ex.Message, ex.InnerException);
        //Server.ClearError();
        //Server.Transfer("~/LI/Error.aspx");

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started
        Application.Lock();
        Application["OnlineUsers"] = (int)Application["OnlineUsers"] + 1;
        Application.UnLock();
    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode is set to InProc in the Web.config file. If session mode is set to StateServer or SQLServer, the event is not raised.
        Application.Lock();
        Application["OnlineUsers"] = (int)Application["OnlineUsers"] - 1;
        Application.UnLock();
    }

</script>
