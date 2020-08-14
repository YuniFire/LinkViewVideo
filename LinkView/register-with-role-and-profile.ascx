<%@ Control Language="C#" AutoEventWireup="true" CodeFile="register-with-role-and-profile.ascx.cs" Inherits="controls_register_with_role_and_profile" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="cc2" Namespace="WebControlCaptcha" Assembly="WebControlCaptcha" %>

<%-- create user wizard with role and profile control --%>
<div class="cuwWrap">
    <div class="cuwTitle">
        ACCOUNT REQUEST
    </div>
    <div class="cuwMessage">
        <asp:Literal ID="InvalidUserNameOrPasswordMessage" runat="server" Visible="false" EnableViewState="false"></asp:Literal>
    </div>
    <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" ContinueDestinationPageUrl="~/login.aspx" DuplicateUserNameErrorMessage="The user name that you entered is already in use. Please enter a different user name." OnCreatingUser="CreateUserWizard1_CreatingUser" OnSendingMail="CreateUserWizard1_SendingMail" DisableCreatedUser="True" LoginCreatedUser="False" CompleteSuccessText="Your account has been successfully created and a confirmation email has been sent to you. Please click on the link in this email to activate your account." OnActiveStepChanged="CreateUserWizard1_ActiveStepChanged" CreateUserButtonType="Image" CreateUserButtonImageUrl="~/MP/images/submit.png" OnCreatedUser="CreateUserWizard1_CreatedUser">
        
        <WizardSteps>
            <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                <ContentTemplate>
&nbsp;<%-- validation summary --%>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="CreateUserWizard1" />
                    <%-- error message --%>
                    <div class="cuwMessage">
                        <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                    </div>
                    <%-- icon display --%>
                    <div class="createUserIcon">
                    </div>
                    <div class="clearBoth2">
                    </div>
                    <%-- NEW ACCOUNT INFO --%>
                    <div class="hr">
                        <b>NEW ACCOUNT INFO</b>
                    </div>
                    <%-- user name --%>
                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:*</asp:Label>
                    <br />
                    <asp:TextBox ID="UserName" runat="server" ToolTip="enter a desired user name" MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                    <div class="clearBoth2">
                    </div>
                    <%-- email --%>
                    <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:*</asp:Label><br />
                    <asp:TextBox ID="Email" runat="server" ToolTip="enter your contact email" MaxLength="100"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email" ErrorMessage="E-mail is required." ToolTip="E-mail is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="rgxEmailRequired" runat="server" ControlToValidate="Email" ErrorMessage="E-mail is required." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" SetFocusOnError="True" ToolTip="E-mail is required." ValidationGroup="CreateUserWizard1">*</asp:RegularExpressionValidator>
                    <br />
                    <%-- password --%>
                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:*</asp:Label>
                    <br />
                    <asp:TextBox ID="Password" va runat="server" TextMode="Password" ToolTip="enter a desired password. must be at least 7 (seven) characters, include 1 (one) numeric character, and 1 (one) symbol (non-alphanumeric) character" MaxLength="50"></asp:TextBox>
                    <cc1:PasswordStrength ID="Password_PasswordStrength" runat="server" TargetControlID="Password" MinimumNumericCharacters="1" PreferredPasswordLength="7" TextCssClass="passwordStrengthIndicator" TextStrengthDescriptions="Very Poor;Weak;Average;Strong;Excellent" MinimumSymbolCharacters="1">
                    </cc1:PasswordStrength>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                    <div class="clearBoth2">
                    </div>
                    <%-- confirm password --%>
                    <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">Confirm Password:*</asp:Label>
                    <br />
                    <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword" ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                    <br />
                    <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match." ValidationGroup="CreateUserWizard1"></asp:CompareValidator>
                    <div class="clearBoth2">
                    </div>
                    <div class="hr">
                        <b>SECURITY CODE</b>
                    </div>
                    <%-- CAPTCHA --%>
                    <div title="enter the code shown on the image.">
                        <cc2:CaptchaControl ID="CAPTCHA" runat="server" CaptchaFontWarping="Medium" LayoutStyle="Vertical" ShowSubmitButton="False" CssClass="captcha" />
                    </div>
                    <!--
                    <%-- SECURITY QUESTION --%><br />
&nbsp;<div class="hr">
                        <b>ACCOUNT SECURITY</b>
                    </div>
                    <%-- question --%>
                    <asp:Label ID="QuestionLabel" runat="server" AssociatedControlID="Question">Security Question:*</asp:Label>
                    <br />
                    <asp:TextBox ID="Question" Text="123"  runat="server" ToolTip="if you lose your account login you can have the system email it to the address you've entered above. enter your security question and answer. when retrieving lost account details, you will be asked to provide this information. enter your security question here." MaxLength="100"></asp:TextBox>
                    
                    <div class="clearBoth2">
                    </div>
                    <%-- answer --%>
                    <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">Security Answer:*</asp:Label>
                    <br />
                    <asp:TextBox ID="Answer" Text="234" runat="server" ToolTip="if you loose your account login you can have the system email it to the address you've entered above. enter your security question and answer. when retrieving lost account details, you will be asked to provide this information. enter your security answer here." MaxLength="100"></asp:TextBox>
                    <div class="clearBoth2">
                    </div>
                    <%-- PROVIDER DETAILS --%>
                    <div class="hr">
                        <b>COMPANY DETAILS</b>
                    </div>
                    <%-- office name --%>
                    First Name*:
                    <br />
                    <asp:TextBox ID="txbFirstName" runat="server" ToolTip="enter your first name" MaxLength="150"></asp:TextBox>
                    <div class="clearBoth2">
                    </div>
                    <%-- address --%>
                    Last Name*:
                    <br />
                    <asp:TextBox ID="txbLastName" runat="server" ToolTip="enter your last name" MaxLength="100"></asp:TextBox>
                    <div class="clearBoth2">
                    </div>
                    <%-- cell phone --%>
                    Cell Phone:
                    <br />
                    <asp:TextBox ID="txbPhone" runat="server" ToolTip="enter your cell phone number" MaxLength="100"></asp:TextBox>
                    <div class="clearBoth2">
                    </div>
                    <%-- city --%>
                    City*:
                    <br />
                    <asp:TextBox ID="txbOfficeCity" runat="server" ToolTip="enter the city name you are located in" MaxLength="100"></asp:TextBox>
                    <div class="clearBoth2">
                    </div>
                    <%-- state --%>
                    State*:
                    <br />
                    <asp:DropDownList ID="ddlStates" runat="server" AppendDataBoundItems="True" ToolTip="select the state your office is located in">
                        <asp:ListItem Selected="True" Text="Arizona" />
                    </asp:DropDownList>
                    <div class="clearBoth2">
                    </div>
                    <%-- zip --%>
                    Zip*:
                    <br />
                    <asp:TextBox ID="txbOfficeZip" runat="server" ToolTip="enter your zipcode" MaxLength="10"></asp:TextBox>
                    <div class="clearBoth2">
                    </div>
                    <div class="hr">
                        <b>SECURITY CODE</b>
                    </div>
                   -->
                </ContentTemplate>
            </asp:CreateUserWizardStep>
            <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server" />
        </WizardSteps>
    </asp:CreateUserWizard>
</div>
