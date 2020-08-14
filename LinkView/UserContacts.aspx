<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="UserContacts.aspx.cs" Inherits="LI_UserContacts" %>
<%@ Register Src="~/UploadContacts.ascx" TagName="UploadContacts" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet">
<link href="style.css" rel="stylesheet" />
<!-- <link href="responsive.css" rel="stylesheet" /> -->
<link rel="shortcut icon" href="images/favicon.ico"  />
<script type="text/javascript" src="js/jquery.min.js"></script>
<!-- MNEU CSS AND JS -->
<link rel="stylesheet" href="css/menu.css" />
<script src="js/animated-menu.js" type="text/javascript"></script>
</head>
<body>
    <form runat="server" id="form1"> 
     <asp:ScriptManager ID="ScriptManager1" runat="server">
       </asp:ScriptManager>
<!-- header start -->
<div class="row s_header">
  <div class="container">
    <div class="logo"><a href="UserUpT2.aspx"><img src="images/logo.png" alt="" /></a><%--<img src="images/tallylogo.png" alt="" class="tallylogo" />--%></div>
    <div class="nav">
      <ul>
        <%--<li><a href="#">Tools</a></li>
        <li><a href="#">Pricing</a></li>
        <li><a href="#">Resources</a></li>--%>
        <li id="last"><a href="LogOff.aspx">Sign Out</a></li>
        <div class="clear"></div>
      </ul>
    </div>
    <div class="clear"></div>
  </div>
  <div class="clear"></div>
</div>
        <div>
             <div style="height:40px;"></div>
             <div class="clear"></div>
        </div>
<%--<div class="row s_slider"> <img src="images/slider.jpg" alt=" " />
  <div class="clear"></div>
</div>--%>

  <div class="row s_user">
    <div class="container">
         <div class="s_itms">
          <h2>BULK UPLOAD CONTACTS</h2>
   <asp:Literal ID="litMsg" runat="server"></asp:Literal>
    UPLOAD CONTACT SPREADSHEET - instructions to export contacts from Outlook <asp:HyperLink ID="HyperLink1" NavigateUrl="https://support.microsoft.com/en-us/office/export-contacts-from-outlook-10f09abd-643c-4495-bb80-543714eca73f" runat="server">here</asp:HyperLink>
   <br /><br />
    <asp:FileUpload ID="FileUpload2" runat="server"  /><br />
    <br /><br />
        <asp:ImageButton ID="ImageButton1" runat="server" 
            onclick="ImageButton1_Click" ImageUrl="~/images/savecontact.png" />
             
    <br /><br />
  
        </div>
    </div>
    <div class="clear"></div>
  </div>
</form>
<div class="row footer_bg">
  <div class="container">
    <div class="footer_left"> TERMS & CONDITIONS, ALL RIGHTS RESERVED 2020
      <div class="clear"></div>
    </div>
    <div class="footer_right">
      <ul>
        <li> <a href="#"><img src="images/icon1.png" /></a>
        <li> <a href="#"><img src="images/icon2.png" /></a>
        <li> <a href="#"><img src="images/icon3.png" /></a>
        <li> <a href="#"><img src="images/icon4.png" /></a> </li>
      </ul>
      <div class="clear"></div>
    </div>
    <div class="clear"></div>
  </div>
  <div class="clear"></div>
</div>
</body>
</html>
   


