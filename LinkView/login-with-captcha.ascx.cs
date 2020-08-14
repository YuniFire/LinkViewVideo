using System;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

public partial class controls_login_with_captcha : System.Web.UI.UserControl
{
    //-------------------------------------------------------------------------------------------------
    protected int passwordResetDuration = 90; // pw expiration duration in DAYS
    protected Boolean passwordResetEnabled = false; // true = ENABLED
    protected int passwordAttemptLockoutDuration = 10; // lockout duration in MINUTES
    protected string FromEmailAddress = "passwordreset@Linkview"; // from email used for new credentials
    //--------------------------------------------------------------------------------------------------

    #region methods

    // find login control and user name textbox in loginview control
    private Login GetLogin1()
    {
        Login Login1 = loginBox.FindControl("Login1") as Login;
        return Login1;
    }

    // update user info
    private static void UpdateUserInfo(MembershipUser usrInfo)
    {
        Membership.UpdateUser(usrInfo);
    }

    #endregion

    #region page_load - set focus to user name textbox

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!HttpContext.Current.User.Identity.IsAuthenticated && !Page.IsPostBack)
        {
            // find login control and user name textbox in loginview control
            Login Login1 = GetLogin1();
            TextBox UserName = Login1.FindControl("UserName") as TextBox;
            UserName.Focus();
        }
    }

    #endregion

    #region login authentication with captcha

    protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
    {
        // find login control in loginview control
        Login Login1 = GetLogin1();

        // We need to determine if the user is authenticated and set e.Authenticated accordingly
        // Get the values entered by the user
        string loginUsername = Login1.UserName;
        string loginPassword = Login1.Password;

        // find captcha control in login control
        //WebControlCaptcha.CaptchaControl loginCAPTCHA = (WebControlCaptcha.CaptchaControl)Login1.FindControl("CAPTCHA");

        // First, check if CAPTCHA matches up
        
            // Next, determine if the user's username/password are valid
            if (Membership.ValidateUser(loginUsername, loginPassword))
            {
                e.Authenticated = true;
                ProfileCommon p = new ProfileCommon().GetProfile(loginUsername);
                Session["profile"] = p;
                Response.Redirect("~/UserUpT2.aspx");
                //FormsAuthentication.RedirectFromLoginPage(loginUsername, true);
            }
            else
            {
                e.Authenticated = false;
                lblFailureText.Text = "User Name and/or Password did not match.";
                lblFailureText.Visible = true;
            }
        
    }

    #endregion

    #region check for lockout and approval

    protected void Login1_LoginError(object sender, EventArgs e)
    {
        // find login control in loginvoew control
        Login Login1 = GetLogin1();

        // get user info
        MembershipUser usrInfo = Membership.GetUser(Login1.UserName);

        // if username typed into loginbox exists
        if (usrInfo == null)
            return;

        // if user is locked out
        if (usrInfo.IsLockedOut)
        {
            // let user know to try again in specified minutes.
            lblFailureText.Text = "Your account has been locked out because of too many invalid login attempts. Please wait " + passwordAttemptLockoutDuration + " minutes and try again.";
            lblFailureText.Visible = true;
        }

        // if password is not expired but the account is not approved
        if (usrInfo.IsApproved || usrInfo.LastPasswordChangedDate.ToUniversalTime().AddDays(passwordResetDuration) <= DateTime.UtcNow)
            return;

        // let user know that the account is not approved yet
        lblFailureText.Text = "Your account has not yet been approved.";
        lblFailureText.Visible = true;
    }

    #endregion

    #region auto unlock and password expiration procedure

    protected void Login1_LoggingIn(object sender, LoginCancelEventArgs e)
    {
        #region automagically unlock user after set amount of time

        Login Login1 = GetLogin1();

        // get the current user's info
        MembershipUser usrInfo = Membership.GetUser(Login1.UserName, false);

        // if lockout datetime + lockout duration is less than datetime now
        if (usrInfo != null && usrInfo.IsLockedOut && usrInfo.LastLockoutDate.ToUniversalTime().AddMinutes(passwordAttemptLockoutDuration) < DateTime.UtcNow)
        {
            // then automatically Unlock user
            usrInfo.UnlockUser();
        }

        #endregion

        #region check for password expiration, auto reset password, and email new one to user

        // if auto password expiration is enabled on top of page
        if (passwordResetEnabled != true)
            return;

        // if last password changed date + reset duration is less than the date now
        if (usrInfo == null || usrInfo.LastPasswordChangedDate.ToUniversalTime().AddDays(passwordResetDuration) >= DateTime.UtcNow)
            return;

        // set user's approval to false so user can't login
        usrInfo.IsApproved = false;
        UpdateUserInfo(usrInfo);

        // Create random GUID for new password - (its nasty long but works if many thousands of users are present)
        Guid randomPasswordGuid = System.Guid.NewGuid();

        // declare variables for user name and password
        string username = Convert.ToString(usrInfo);
        string password = randomPasswordGuid.ToString();

        // get user name (use second membership provider from web.config - does not require question and answer)
        MembershipUser mu = Membership.Providers["dbSqlMemberShipProviderAdmin"].GetUser(username, false);

        // change user password to newly generated guid
        mu.ChangePassword(mu.ResetPassword(), password);

        // set user approval to true so user can now login with new guid password
        usrInfo.IsApproved = true;
        UpdateUserInfo(usrInfo);

        // let the user know that the password has expired.
        lblFailureText.Text = usrInfo + ", As per our user policy, your account password has expired! An e-mail is being sent with your new credentials.";
        lblFailureText.Visible = true;

        // send new credentials to user via EMAIL
        try
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

            // determine the site's URL so we can use it in the email
            string urlBase = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;

            // get user's new password and  account email so we can email it - mu is already declared above
            string ePassword = mu.GetPassword();

            string eEmail = mu.Email;
            string eFrom = FromEmailAddress;
            string eFromDisplayName = "Site Administrator";
            string eSubject = "Password Reset! - Your new credentials";

            message.To.Add(eEmail);
            message.From = new MailAddress(eFrom, eFromDisplayName);
            message.Subject = eSubject;
            message.Body = "<p>Hello " + usrInfo + ",</p>" + " <p>Your password for " + urlBase + " have been reset:<br/>User Name: " + usrInfo + " <br />Your New Password: " + ePassword + " <br /> Please use your new password to login to your account and change it to something you can remember.<br/><br/> Thank You!<br/>Webmaster.</p>";
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;

            // cool! we only have declare the smtp. System.Net.Mail automatically finds it in web.config
            SmtpClient client = new SmtpClient();
            client.Send(message);
        }
        catch (Exception ex)
        {
            lblFailureText.Text += " ERROR Sending Mail: " + ex.Message;
            lblFailureText.Visible = true;
        }
        finally
        {
            // do nothing
        }

        #endregion
    }

    #endregion

    protected void RegisterHyperlink_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/register.aspx");
    }

  
}