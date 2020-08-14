using System;
using System.Web.Security;

public partial class controls_verification : System.Web.UI.UserControl
{
    #region verify user

    // this code verifies the submitted URL and unlocks the user's account
    protected void Page_Load(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(Request.QueryString["ID"]))
            StatusMessage.Text = "The UserId was not included in the querystring...";
        else
        {
            // declare variable to hold guid
            Guid userId;

            try
            {
                // assign guid id sent from email to userid
                userId = new Guid(Request.QueryString["ID"]);
            }
            catch
            {
                // if user id received is not in the proper format - display error message
                StatusMessage.Text = "The UserId passed into the querystring is not in the proper format...";
                return;
            }

            // if id received passes the mustard, lookup user in user database
            MembershipUser usr = Membership.GetUser(userId);

            // if user does not exist display error msg
            if (usr == null)
                StatusMessage.Text = "User account could not be found...";
            else
            {
                // otherwise approve the user account
                usr.IsApproved = true;
                Membership.UpdateUser(usr);

                StatusMessage.Text = "Your account has been created and is now active. <br/>You may now go to the <a href='login.aspx'>Login Page</a> and use your account credentials to login.<br/><br/>Thank you!";
            }
        }
    }

    #endregion
}