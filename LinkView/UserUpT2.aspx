<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserUpT2.aspx.cs" Inherits="UserUpT2" %>
<%@ Register Src="~/UploadContacts.ascx" TagName="UploadContacts" TagPrefix="uc1" %>
<%@ Register Src="~/ViewMessages.ascx" TagName="ViewMessages" TagPrefix="ucMessg" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
     <script type="text/javascript">
        function ChangeHeaderAsNeeded() {
            // Whenever a checkbox in the GridView is toggled, we need to
            // check the Header checkbox if ALL of the GridView checkboxes are
            // checked, and uncheck it otherwise
            if (CheckBoxIDs != null) {
                // check to see if all other checkboxes are checked
                for (var i = 1; i < CheckBoxIDs.length; i++) {
                    var cb = document.getElementById(CheckBoxIDs[i]);
                    if (!cb.checked) {
                        // Whoops, there is an unchecked checkbox, make sure
                        // that the header checkbox is unchecked
                        ChangeCheckBoxState(CheckBoxIDs[0], false);
                        return;
                    }
                }

                // If we reach here, ALL GridView checkboxes are checked
                ChangeCheckBoxState(CheckBoxIDs[0], true);
            }
        }
        function ChangeCheckBoxState(id, checkState) {
            var cb = document.getElementById(id);
            if (cb != null)
                cb.checked = checkState;
        }

        function ChangeAllCheckBoxStates(checkState) {
            // Toggles through all of the checkboxes defined in the CheckBoxIDs array
            // and updates their value to the checkState input parameter
            if (CheckBoxIDs != null) {
                for (var i = 0; i < CheckBoxIDs.length; i++)
                    ChangeCheckBoxState(CheckBoxIDs[i], checkState);
            }
        }
        function UploadHold() {
            
            $.ajax({
               
                    url: 'Handler.ashx',
                    type: 'POST',
                    data: new FormData($('form')[0]),
                    cache: false,
                    contentType: false,
                    processData: false,
                success: function (file) {
                   // alert("Upload Completed");
                        $("#fileProgress").hide();
                    //    $("#lblMessage").html("<b>" + file.name + "</b> has been uploaded.");
                       
                     },
                  
    error: function (jqXHR, exception) {
        var msg = '';
        if (jqXHR.status === 0) {
            msg = 'Not connect.\n Verify Network.';
        } else if (jqXHR.status == 404) {
            msg = 'Requested page not found. [404]';
        } else if (jqXHR.status == 500) {
            msg = 'Internal Server Error [500].';
        } else if (exception === 'parsererror') {
            msg = 'Requested JSON parse failed.';
        } else if (exception === 'timeout') {
            msg = 'Time out error.';
        } else if (exception === 'abort') {
            msg = 'Ajax request aborted.';
        } else {
            msg = 'Uncaught Error.\n' + jqXHR.responseText;
        }
        alert(msg);
    },
                    xhr: function () {  
                        var fileXhr = $.ajaxSettings.xhr();
                        if (fileXhr.upload) {
                            $("progress").show();
                            fileXhr.upload.addEventListener("progress", function (e) {
                                if (e.lengthComputable) {
                                    $("#fileProgress").attr({
                                        value: e.loaded,
                                        max: e.total
                                    });
                                    if (e.total == e.loaded) {
                                        $("#lblMessage").html("UPLOAD COMPLETED!  ONCE THE VIDEO CONVERSION IS COMPLETED, THE VIDEO WILL APPEAR UNDER VIDEO MESSAGES.")
                                    }

                                }
                            }, false);
                        }
                        return fileXhr;
                    }
                 }
                 );
         }
         function modalContact()
         {
             var modal2 = document.getElementById("contactModal");
             modal2.style.display = "block";
         }
    function spanClose()
    {
        var modal2 = document.getElementById("contactModal");
             modal2.style.display = "none";
         }

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    var modal2 = document.getElementById("contactModal");
  if (event.target == modal2) {
    modal2.style.display = "none";
  }
}
    </script>
<title>Link View</title>
<meta name="description" content="Link View">
<meta name="keywords" content="  ">
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
    <div class="logo"><a href="#"><img src="images/logo.png" alt="" /></a><%--<img src="images/tallylogo.png" alt="" class="tallylogo" />--%></div>
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
         <h2 class="s_center"><asp:Literal ID="litMsg" runat="server"></asp:Literal></h2>
         <h2 class="s_center">Video <strong>Messaging</strong> </h2>
  
         
            <asp:HiddenField ID="hidUserName" runat="server" />
   
       
        <h5 class="s_center">STEP 1. UPLOAD CONTACTS TO SEND MESSAGES</h5>
    
     <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:UploadContacts runat="server" ID="UploadContacts" />
    </ContentTemplate>
    </asp:UpdatePanel>
    <div class="clear"></div>
        <br /> <br />

        <h5 class="s_center">STEP 2. UPLOAD AND CONVERT VIDEO ENTER MESSAGE TEXT (500 characters max)</h5>
    <asp:TextBox ID="txtMobileText" runat="server" Width="489px" height="185px" CssClass="textarea1"
            TextMode="MultiLine" MaxLength="500"></asp:TextBox>
    <br />
             <span id="lblMessage" style="color: black;"></span><br />
    <progress id="fileProgress" style="display: none"></progress>
    <br />
    <div >
    <asp:FileUpload ID="FileUpload1" runat="server">
    </asp:FileUpload>
       </div><br />
             <img src="/images/submit.png" onclick="UploadHold();" />
    <!--<asp:ImageButton ID="btnUpload2" runat="server" ImageUrl="~/images/submit.png" OnClientClick="UploadHold();" />-->
    <br /><br />

  
        <div class="s_recipents">
         
   
       <h5 class="s_center margin_top50">STEP 3. SELECT MESSAGE RECIPIENTS AND SEND MESSAGE</h5>

    <h2>Message Recipients</h2>
    <asp:GridView ID="gvUserList" runat="server" EmptyDataText="No Records Found" AllowPaging="True" 
        AutoGenerateColumns="False" width="100%" PageSize="5"  PagerSettings-Mode="NextPrevious" PagerSettings-NextPageText="Next" PagerSettings-PreviousPageText="Previous"
        DataKeyNames="ContactUserID" BorderColor="Gray" GridLines="Both" 
        ForeColor="Black" AlternatingRowStyle-BorderStyle="Solid"  AlternatingRowStyle-BorderColor="Gray"
          RowStyle-BorderColor="Gray" RowStyle-BorderStyle="Solid" 
        HeaderStyle-BorderColor="Gray" HeaderStyle-BorderStyle="Solid" HeaderStyle-BackColor="beige" 
           FooterStyle-BorderColor="Gray" FooterStyle-BorderStyle="Solid"
         OnDataBound="gvUserList_DataBound"  
         OnRowDataBound="gvUserList_RowDataBound" 
        onselectedindexchanged="gvUserList_SelectedIndexChanged">
        <Columns> 
      <asp:TemplateField>
            <HeaderTemplate>
                <center><asp:CheckBox runat="server" ID="HeaderLevelCheckBox" /></center>
            </HeaderTemplate>
            <ItemTemplate>
                <center><asp:CheckBox runat="server" ID="RowLevelCheckBox" /></center>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="First Name">
          <itemtemplate>
          <asp:Label ID="lblFirstName" runat="server" Text='<%#Eval("FirstName")%>' 
          ></asp:Label>
          </itemtemplate>
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Last Name">
          <itemtemplate>
          <asp:Label ID="lblLastName" runat="server" Text='<%#Eval("LastName")%>' 
          ></asp:Label>
          </itemtemplate>
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Cell Phone">
          <itemtemplate>
          <asp:Label ID="lblCellPhone" runat="server" Text='<%#Eval("CellPhone")%>' 
          ></asp:Label>
          </itemtemplate>
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Wireless Carrier">
          <itemtemplate>
          <asp:DropDownList ID="ddlWireless" runat="server" Enabled="false">
          <asp:ListItem Value="Unknown" Selected="True">N/A</asp:ListItem>
          <asp:ListItem Value="@mms.att.net">AT&T Wireless</asp:ListItem>
          <asp:ListItem Value="@vzwpix.com">Verizon</asp:ListItem>
          <asp:ListItem Value="@tmomail.net">T-Mobile</asp:ListItem>
          <asp:ListItem Value="@pm.sprint.com">Sprint</asp:ListItem>
          <asp:ListItem Value="@mymetropcs.com">Metro PCS</asp:ListItem>
          <asp:ListItem Value="@mms.mycricket.com">Cricket</asp:ListItem>
          <asp:ListItem Value="@mmst5.tracfone.com">Tracfone</asp:ListItem>
          </asp:DropDownList>
          
          </itemtemplate>
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Email">
          <itemtemplate>
          <asp:Label ID="lblEmail" runat="server" Text='<%#Eval("Email")%>' 
          ></asp:Label>
          </itemtemplate>
          </asp:TemplateField>
        </Columns>

    </asp:GridView>
    <br /><br />
     <ucMessg:ViewMessages runat="server" ID="ucMessg" />
            </div>
        </div>
    </div>
    <div class="clear"></div>
  </div>

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
        </form>
</body>
</html>
