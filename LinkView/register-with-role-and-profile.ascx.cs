using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web;
using System.Configuration;
using System.Net.Mail;

public partial class controls_register_with_role_and_profile : System.Web.UI.UserControl
{
    //-------------------------------------------------------------------------------------------------------
    protected string wsatDefaultRole = "Member"; // default Role for membership
    protected string wsatDefaultRoleMissing = "missing_default_role@Linkview"; // from email to notify admin
    protected string wsatNewRegistration = "new_account_registration@Linkview"; // from email to notify admin
    //-------------------------------------------------------------------------------------------------------
    
    #region PageLoad - Check if user is logged in and disable form if true

    protected void Page_Load(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("log data here");
        if (!Page.IsPostBack)
        {
            // get state names from app_code folder and bind them to the dropdown list
            DropDownList ddlStates = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("ddlStates") as DropDownList;
            ddlStates.DataSource = UnitedStates.StateNames.GetStates();
            ddlStates.DataBind();
        }
        // if current user is logged in
        if (!HttpContext.Current.User.Identity.IsAuthenticated)
            return;

        // disable form
        CreateUserWizard1.Enabled = false;

        // display message
        InvalidUserNameOrPasswordMessage.Text = "To create a new account, please Log out first...";
        InvalidUserNameOrPasswordMessage.Visible = true;
        
    }

    #endregion

    #region Validate CAPTCHA, check for leading/trailing spaces in user name

    // this code checks for leading and trailing spaces in username
    // and makes sure the username does not appear in the password
    protected void CreateUserWizard1_CreatingUser(object sender, LoginCancelEventArgs e)
    {   
        // find captcha control
        WebControlCaptcha.CaptchaControl registerCAPTCHA = (WebControlCaptcha.CaptchaControl)CreateUserWizardStep1.ContentTemplateContainer.FindControl("CAPTCHA");
        
        if (!registerCAPTCHA.UserValidated)
        {
            // Show the error message                
            InvalidUserNameOrPasswordMessage.Text = "Security Code MISSING or INCORRECT!";
            InvalidUserNameOrPasswordMessage.Visible = true;

            // Cancel the create user workflow                
            e.Cancel = true;
        }
        
        // declare variable and assign it to the user name textbox
        string trimmedUserName = CreateUserWizard1.UserName.Trim();

        // Check for empty spaces infront and behind the string
        if (CreateUserWizard1.UserName.Length != trimmedUserName.Length)
        {
            // Show the error message           
            InvalidUserNameOrPasswordMessage.Text = "The username cannot contain leading or trailing spaces.";
            InvalidUserNameOrPasswordMessage.Visible = true;
            // Cancel the create user workflow           
            e.Cancel = true;
        }
        else
        {
            // Username is valid, make sure that the password does not contain the username           
            if (CreateUserWizard1.Password.IndexOf(CreateUserWizard1.UserName, StringComparison.OrdinalIgnoreCase) < 0)
                return;

            // Show the error message                
            InvalidUserNameOrPasswordMessage.Text = "The username may not appear anywhere in the password.";
            InvalidUserNameOrPasswordMessage.Visible = true;

            // Cancel the create user workflow                
            e.Cancel = true;
        }
        SendEmail();
    }

    #endregion

    #region get state list from app code folder and bind them to the dropdown list

    protected void CreateUserWizard1_Load(object sender, EventArgs e)
    {
       
    }

    #endregion

    #region add newly registered user to default role (Member) and create profiles

    protected void CreateUserWizard1_ActiveStepChanged(object sender, EventArgs e)
    {
        // Have we JUST reached the Complete step?
        if (CreateUserWizard1.ActiveStep.Title != "Complete")
            return;

        // add newly created user to default Role specified above
        if (Roles.RoleExists(wsatDefaultRole))
        {
            // Add the newly created user to the default Role.
            Roles.AddUserToRole(CreateUserWizard1.UserName, wsatDefaultRole);

            // Create an empty Profile for the newly created user
            ProfileCommon p = (ProfileCommon)ProfileCommon.Create(CreateUserWizard1.UserName, true);

            // Populate some Profile properties. Values are located in web.config file
            //p.Company.Company = ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txbOfficeName")).Text;
            //p.Company.Address = ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txbOfficeAddress")).Text;
            //p.Company.City = ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txbOfficeCity")).Text;
            //p.Company.State = ((DropDownList)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("ddlStates")).SelectedValue;
            //p.Company.PostalCode = ((DropDownList)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txbOfficeZip")).SelectedValue;
            //p.Company.PostalCode = ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txbOfficeZip")).Text;
            //p.Company.Phone = ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txbContactPhone")).Text;
            //p.Company.Fax = ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txbContactFax")).Text;
            //p.Preferences.Newsletter = ((DropDownList)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("ddlNewsletter")).SelectedValue;
            //p.Personal.FirstName = ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txbFirstName")).Text;
            //p.Personal.LastName = ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txbLastName")).Text;
            //p.Contacts.CellPhone = ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txbPhone")).Text;
            // Save profile - must be done since we explicitly created it
            p.Save();


            SendEmail();

        }
        else
        {
            // Show the error message                
            InvalidUserNameOrPasswordMessage.Text = "Oops! The default Role does not exist for this site.<br/>The site administrator has been contacted and made aware of this error...";
            InvalidUserNameOrPasswordMessage.Visible = true;

            // notify administrator via email
            try
            {
                // get default email address from web.config
                string AdminEmail = ConfigurationManager.AppSettings["AdminEmail"];
                string emailToWebConfig = AdminEmail.ToString();

                // create timestamp
                string timeStamp = DateTime.Now.ToString("F");

                // send mail
                System.Net.Mail.MailMessage MyMailer = new System.Net.Mail.MailMessage();
                MyMailer.To.Add(emailToWebConfig);
                MyMailer.From = new MailAddress(wsatDefaultRoleMissing, "Missing Default Role Notification");
                MyMailer.Subject = "Missing Default Role Notification";
                MyMailer.Body = "Dear Site Administrator, <br/><br/> You have recieved a Missing Default Role Notification. <br/> Please remember to remedy the problem.</b><br/>";
                MyMailer.Body += "time of occurance: " + timeStamp;
                MyMailer.Body += "<br/><br/><b>Have a wonderful day!";
                MyMailer.IsBodyHtml = true;
                MyMailer.Priority = MailPriority.High;
                

                SmtpClient client = new SmtpClient();
                client.Send(MyMailer);
            }
            catch
            {
                // do nothing
            }
        }
        
    }

    #endregion

    #region send verification email to new registrant

    // this code uses the CreateUserWizard.txt file in the EmailTemplates folder -
    // it creates and sends a verification URL to the user before allowing login -
    // user must click on link in email to verify email address -
    // the verification takes place in Verification.aspx file
    protected void CreateUserWizard1_SendingMail(object sender, MailMessageEventArgs e)
    {
       /* // Get the UserId of the newly added user
        MembershipUser newUser = Membership.GetUser(CreateUserWizard1.UserName);
        Guid newUserId = (Guid)newUser.ProviderUserKey;

        // Determine the full verification URL (i.e., http://yoursite.com/verification.aspx?ID=...)
        string urlBase = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
        string verifyUrl = "/verification.aspx?ID=" + newUserId.ToString();
        string fullUrl = urlBase + verifyUrl;
        fullUrl = System.Web.HttpContext.Current.Server.UrlEncode(fullUrl);


        // Replace <%VerificationUrl%> with the appropriate URL and querystring
        e.Message.Body = e.Message.Body.Replace("<%VerificationUrl%>", fullUrl);
        System.Net.WebClient client = new System.Net.WebClient();
        client.Headers[System.Net.HttpRequestHeader.ContentType] = "application/json";

        string response = client.DownloadString("http://localhost:19146/api/Register?to=" + e.Message.To + "&link=" + e.Message.Body); */

    } 
    private void SendEmail()
    {
        // Get the UserId of the newly added user
        MembershipUser newUser = Membership.GetUser(CreateUserWizard1.UserName);
        Guid newUserId = (Guid)newUser.ProviderUserKey;

        // Determine the full verification URL (i.e., http://yoursite.com/verification.aspx?ID=...)
        string urlBase = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
        string verifyUrl = "/verification.aspx?ID=" + newUserId.ToString();
        string fullUrl = urlBase + verifyUrl;
        fullUrl = System.Web.HttpContext.Current.Server.UrlEncode(fullUrl);


        // Replace <%VerificationUrl%> with the appropriate URL and querystring
        //e.Message.Body = e.Message.Body.Replace("<%VerificationUrl%>", fullUrl);
        System.Net.WebClient client = new System.Net.WebClient();
        client.Headers[System.Net.HttpRequestHeader.ContentType] = "application/json";
        string emailTo = Request.Form["RegisterWithRole1$CreateUserWizard1$CreateUserStepContainer$Email"] as string;
        string body = "Welcome to LinkView, please click the link <a href='" + fullUrl + "'>here</a> to complete your registration.";
        
        string response = client.DownloadString(ConfigurationManager.AppSettings["url"] + ConfigurationManager.AppSettings["Register"] + "?to=" + emailTo + "&link=" + body);

    }

    #endregion

    #region send alert email to admin to notify of new registration

    protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
    {
        
    }

    #endregion
}