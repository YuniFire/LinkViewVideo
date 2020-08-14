using System.Web.UI.WebControls;
using System.Configuration;
using System;

public partial class controls_recover_password : System.Web.UI.UserControl
{
    #region page_load - set focus to user name textbox

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            // find user name textbox in UserName Template Container
            TextBox UserName = PasswordRecovery1.UserNameTemplateContainer.FindControl("UserName") as TextBox;
            UserName.Focus();
        }
    }

    #endregion
    
    #region validate user name

    protected void PasswordRecovery1_UserLookupError(object sender, System.EventArgs e)
    {
        // Show the error message                
        FailureText.Text = "User Name is INCORRECT! Please try again.<div class='clearBoth2'></div>";
        FailureText.Visible = true;
    }

    #endregion

    #region validate captcha input

    protected void PasswordRecovery1_VerifyingUser(object sender, LoginCancelEventArgs e)
    {
        // find captcha control
        WebControlCaptcha.CaptchaControl registerCAPTCHA = (WebControlCaptcha.CaptchaControl)PasswordRecovery1.UserNameTemplateContainer.FindControl("CAPTCHA");

        // if captcha is missing or incorrect
        if (!registerCAPTCHA.UserValidated)
        {
            // Show the error message                
            FailureText.Text = "Security Code MISSING or INCORRECT!<div class='clearBoth2'></div>";
            FailureText.Visible = true;

            // Cancel the transaction                
            e.Cancel = true;
        }
    }

    #endregion
    
    #region send carbon copy to admin

    // this code send a carbon copy CC email to the webmaster
    protected void PasswordRecovery1_SendingMail(object sender, MailMessageEventArgs e)
    {
        // get default email address from web.config appsettings section
       // string AdminEmail = ConfigurationManager.AppSettings["AdminEmail"];
       // string emailToWebConfig = AdminEmail.ToString();

        //e.Message.CC.Add(AdminEmail);
    }

    #endregion
}
