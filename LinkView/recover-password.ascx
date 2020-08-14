<%@ Control Language="C#" AutoEventWireup="true" CodeFile="recover-password.ascx.cs" Inherits="controls_recover_password" %>
<%@ Register TagPrefix="cc2" Namespace="WebControlCaptcha" Assembly="WebControlCaptcha" %>
<%-- PASSWORD RECOVERY CONTROL --%>
<div class="prWrap">
    <div class="prTitle">
        PASSWORD RECOVERY</div>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="PasswordRecovery1" EnableClientScript="False" />
    <asp:ValidationSummary ID="ValidationSummary2" runat="server" ValidationGroup="ctl00$PasswordRecovery1" EnableClientScript="False" />
    <div class="prMessage">
        <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
    </div>
    <asp:PasswordRecovery ID="PasswordRecovery1" runat="server" OnSendingMail="PasswordRecovery1_SendingMail" OnVerifyingUser="PasswordRecovery1_VerifyingUser" SuccessText="Thank you! Your password has been sent to you." OnUserLookupError="PasswordRecovery1_UserLookupError">
        <MailDefinition BodyFileName="~/email_templates/password-recovery.txt" Subject="Your password...">
        </MailDefinition>
        <UserNameTemplate>
            <div class="pwRecoveryIcon">
            </div>
            <div class="clearBoth2">
            </div>
            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">Enter User Name:*</asp:Label>
            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="PasswordRecovery1" EnableClientScript="False" Display="Dynamic">*</asp:RequiredFieldValidator>
            <br />
            <asp:TextBox ID="UserName" runat="server" MaxLength="50" ValidationGroup="PasswordRecovery1" ToolTip="enter your user name."></asp:TextBox>
            <div class="clearBoth2">
            </div>
            <div class="hr">
                <b>SECURITY CODE</b>
            </div>
            <div title="enter the code shown on the image.">
                <cc2:CaptchaControl ID="CAPTCHA" runat="server" CaptchaFontWarping="Medium" CssClass="captcha" LayoutStyle="Vertical" ShowSubmitButton="False" />
            </div>
            <div class="clearBoth2">
            </div>
            <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" Text="Submit" ValidationGroup="PasswordRecovery1" />
        </UserNameTemplate>
        <QuestionTemplate>
            <div class="pwRecoveryIcon">
            </div>
            <div class="clearBoth2">
            </div>
            <div class="hr">
                <b>IDENTITY CONFIRMATION</b>
            </div>
            Please answer the following question to receive your password:
            <div class="clearBoth2">
            </div>
            User Name:
            <asp:Literal ID="UserName" runat="server"></asp:Literal>
            <div class="clearBoth2">
            </div>
            Security Question:
            <asp:Literal ID="Question" runat="server"></asp:Literal>
            <br />
            <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">Security Answer:*</asp:Label>
            <br />
            <asp:TextBox ID="Answer" runat="server" ValidationGroup="ctl00$PasswordRecovery1"></asp:TextBox>
            <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" ControlToValidate="Answer" ErrorMessage="Answer is required." ToolTip="Answer is required." ValidationGroup="ctl00$PasswordRecovery1" EnableClientScript="False" Display="Dynamic">*</asp:RequiredFieldValidator>
            <div class="prMessage">
                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
            </div>
            <div class="clearBoth2">
            </div>
            <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" Text="Submit" ValidationGroup="ctl00$PasswordRecovery1" />
        </QuestionTemplate>
    </asp:PasswordRecovery>
</div>
