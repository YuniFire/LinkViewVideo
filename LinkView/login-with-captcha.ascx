<%@ Control Language="C#" AutoEventWireup="true" CodeFile="login-with-captcha.ascx.cs" Inherits="controls_login_with_captcha" %>
<%@ Register TagPrefix="cc1" Namespace="WebControlCaptcha" Assembly="WebControlCaptcha" %>
<%-- LOGIN USER CONTROL WITH CAPTCHA --%>

    <a name="login" id="login" style="display: block; height: 0px; width: 0px; border: 0px;"></a>

        LOGIN - username: admintest&nbsp;&nbsp;pwd: Linkview11!
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ValidationGroup="Login1" EnableClientScript="False" />
<%-- success label --%>
    
        <asp:HyperLink ID="lblFailureText" runat="server" Width="240px" Visible="false" EnableViewState="false"></asp:HyperLink>
    
    <asp:Literal ID="Msg" runat="server" Visible="false"></asp:Literal>
    <asp:LoginView ID="loginBox" runat="server">
        <LoggedInTemplate>
        </LoggedInTemplate>
        <AnonymousTemplate>
           
            <asp:Login ID="Login1" runat="server" OnAuthenticate="Login1_Authenticate" OnLoginError="Login1_LoginError" VisibleWhenLoggedIn="False" OnLoggingIn="Login1_LoggingIn">
                <LayoutTemplate>
                    <asp:Panel ID="pnlLogin" runat="server" DefaultButton="LoginButton">
                        <div>
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:*</asp:Label>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="Login1" Display="Dynamic" EnableClientScript="False">*</asp:RequiredFieldValidator>
                        <asp:TextBox ID="UserName" runat="server" TabIndex="1" ToolTip="enter your user name" MaxLength="50" Width="20%" Height="30px"></asp:TextBox>
                        </div>
                        <br />
                        <div>
                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:*&nbsp;&nbsp; </asp:Label>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="Login1" Display="Dynamic" EnableClientScript="False">*</asp:RequiredFieldValidator>
                        <asp:TextBox ID="Password" runat="server" TabIndex="2" TextMode="Password" ToolTip="enter your password" MaxLength="50" Width="20%" Height="30px"></asp:TextBox>
                        <br /> 
                        <br />
                            <asp:ImageButton ID="LoginButton" ValidationGroup="Login1" CommandName="Login" TabIndex="4" runat="server" ImageUrl="~/images/login.png" />
                        </div>
                        <br />
                       
                       <%-- <div style="text-align:left; margin-left:0;">
                       
                           <b>SECURITY CODE</b> 
                  
                       
                     <cc1:CaptchaControl ID="CAPTCHA" runat="server" LayoutStyle="Vertical" ShowSubmitButton="False" TabIndex="3" CaptchaFontWarping="Medium" CssClass="captcha" ToolTip="enter the code shown above" />
               
                       
                  </div>   
                        <asp:CheckBox ID="RememberMe" runat="server" TabIndex="5" Text="Remember me next time." />
                        <br />
                        <asp:HyperLink ID="lnkRegister" runat="server" NavigateUrl="~/useragreement.aspx">Create an account</asp:HyperLink>
                        <br />
                        <asp:HyperLink ID="lnkLostPassword" runat="server" NavigateUrl="~/recover-password.aspx">Forgot your password?</asp:HyperLink>--%>
                        <asp:HyperLink ID="RegisterHyperLink" runat ="server" BackColor="White" ForeColor="Black" OnClick ="RegisterHyperLink_Click" Text="&nbsp;&nbsp;Register Now" Width="135px" BorderStyle="None" Font-Size="Medium" NavigateUrl="~/register.aspx" />
                    </asp:Panel>
                </LayoutTemplate>
            </asp:Login>
        </AnonymousTemplate>
    </asp:LoginView>
  



  

