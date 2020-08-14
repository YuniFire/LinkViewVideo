<%@ Page Title="Account Removed" Language="C#" AutoEventWireup="true" CodeFile="account-removed.aspx.cs" Inherits="account_removed" EnableViewState="false" %>
<%@ Register Src="account-removed.ascx" TagName="account" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Link View</title>
    <meta name="description" content="Link View">
    <meta name="keywords" content="  ">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <link href="style.css" rel="stylesheet" />
    <!-- <link href="responsive.css" rel="stylesheet" /> -->
    <link rel="shortcut icon" href="images/favicon.ico" />
    <script type="text/javascript" src="js/jquery.min.js"></script>
    <!-- MNEU CSS AND JS -->
    <link rel="stylesheet" href="css/menu.css" />
    <script src="js/animated-menu.js" type="text/javascript"></script>
</head>
<body>
    <div class="entire">
        <!-- header start -->
        <div class="row s_header">
            <div class="container">
                <div class="logo">
                    <a href="#">
                        <img src="images/logo.png" alt="" /></a><%--<img src="images/tallylogo.png" alt="" class="tallylogo" />--%>
                </div>
                <div class="nav">
                    <ul>
                        <li><a href="#">Tools</a></li>
                        <li><a href="#">Pricing</a></li>
                        <li><a href="#">Resources</a></li>
                        <li id="last"><a href="#">Sign In</a></li>
                        <div class="clear"></div>
                    </ul>
                </div>
                <div class="clear"></div>
            </div>
            <div class="clear"></div>
        </div>
        <div class="row s_slider">
            <img src="images/slider.jpg" alt=" " />
            <div class="clear"></div>
        </div>
        <form runat="server" id="formhome">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <div class="row s_user">
                <div class="container">
                    <div class="s_itms">
                        <span style="text-align: left">
                              <uc1:account ID="account2" runat="server" />
                        </span>
                    </div>
                </div>
                <div class="clear"></div>
            </div>
        </form>
        <div class="row footer_bg">
            <div class="container">
                <div class="footer_left">
                    TERMS & CONDITIONS, ALL RIGHTS RESERVED 2020
     
                    <div class="clear"></div>
                </div>
                <div class="footer_right">
                    <ul>
                        <li><a href="#">
                            <img src="images/icon1.png" /></a>
                            <li><a href="#">
                                <img src="images/icon2.png" /></a>
                                <li><a href="#">
                                    <img src="images/icon3.png" /></a>
                                    <li><a href="#">
                                        <img src="images/icon4.png" /></a> </li>
                    </ul>
                    <div class="clear"></div>
                </div>
                <div class="clear"></div>
            </div>
            <div class="clear"></div>
        </div>
    </div>
    <!--close of entire-->
</body>
</html>
