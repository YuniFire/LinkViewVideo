<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UploadContacts.ascx.cs" Inherits="LI_UserControls_UploadContacts" %>

<h2>Contact List</h2>

 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Literal ID="litError" Visible="false" runat="server"></asp:Literal>    
<asp:RegularExpressionValidator Display="Dynamic" ID="RegularExpressionValidator2" runat="server" ControlToValidate="tbCellphone"
ErrorMessage="Please Enter Only Numbers" Style="color:Red;" ValidationExpression="^\d+$" ValidationGroup="checkcell"></asp:RegularExpressionValidator>
<asp:RegularExpressionValidator Display="Dynamic" ID="RegEx2" runat="server" ControlToValidate="tbEmail" ErrorMessage="Please Enter Valid Email" Style="color:Red;" ValidationExpression="^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"   ValidationGroup="check2"></asp:RegularExpressionValidator>
                   
<asp:GridView ID="gvUserListContact" runat="server" EmptyDataText="No Records Found" AllowPaging="True" 
        AutoGenerateColumns="False" width="100%" PageSize="5" DataKeyNames="ContactUserID" PagerSettings-Mode="NextPrevious" PagerSettings-NextPageText="Next" PagerSettings-PreviousPageText="Previous"
         BorderColor="Gray" GridLines="Both" ForeColor="Black" AlternatingRowStyle-BorderStyle="Solid"  AlternatingRowStyle-BorderColor="Gray"
          RowStyle-BorderColor="Gray" RowStyle-BorderStyle="Solid" HeaderStyle-BorderColor="Gray" HeaderStyle-BorderStyle="Solid"
           FooterStyle-BorderColor="Gray" FooterStyle-BorderStyle="Solid" HeaderStyle-BackColor="beige" 
            OnRowDataBound="gvUserListContact_RowDataBound"
         onpageindexchanging="gvUserListContact_PageIndexChanging" 
                            onrowcancelingedit="gvUserListContact_RowCancelingEdit" 
                            onrowcommand="gvUserListContact_RowCommand" 
                            onrowdeleting="gvUserListContact_RowDeleting" 
                            onrowediting="gvUserListContact_RowEditing" 
                          
                            onrowupdating="gvUserListContact_RowUpdating">
        <Columns> 
        <asp:TemplateField HeaderText="First Name">
          <itemtemplate>
          <asp:Label ID="lblFirstName" runat="server" Text='<%#Eval("FirstName")%>' 
          ></asp:Label>
          </itemtemplate>
          <edititemtemplate>
          <asp:TextBox ID="txtFirstName" Width="75" runat="server" Text='<%#Eval("FirstName")%>' 
          ></asp:TextBox>
          </edititemtemplate>
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Last Name">
          <itemtemplate>
          <asp:Label ID="lblLastName" runat="server" Text='<%#Eval("LastName")%>' 
          ></asp:Label>
          </itemtemplate>
          <edititemtemplate>
          <asp:TextBox ID="txtLastName" Width="75" runat="server" Text='<%#Eval("LastName")%>' 
          ></asp:TextBox>
          </edititemtemplate>
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Cell Phone">
          <itemtemplate>
          <asp:Label ID="lblCellPhone" runat="server" Text='<%#Eval("CellPhone")%>' 
          ></asp:Label>
          </itemtemplate>
          <edititemtemplate>
          <asp:TextBox ID="txtCellPhone" Width="75" MaxLength="10" runat="server" ValidationGroup="check" Text='<%#Eval("CellPhone")%>' 
          ></asp:TextBox>
          <asp:RegularExpressionValidator Display="Dynamic" ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtCellPhone"
ErrorMessage="Please Enter Only Numbers" Style="color:Red;" ValidationExpression="^\d+$" ValidationGroup="check"></asp:RegularExpressionValidator>
          </edititemtemplate>
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
          <edititemtemplate>
          <asp:DropDownList ID="ddlWireless" runat="server">
          <asp:ListItem Value="Unknown">N/A</asp:ListItem>
          <asp:ListItem Value="@mms.att.net">AT&T Wireless</asp:ListItem>
          <asp:ListItem Value="@vzwpix.com">Verizon</asp:ListItem>
          <asp:ListItem Value="@tmomail.net">T-Mobile</asp:ListItem>
          <asp:ListItem Value="@pm.sprint.com">Sprint</asp:ListItem>
          <asp:ListItem Value="@mymetropcs.com">Metro PCS</asp:ListItem>
          <asp:ListItem Value="@mms.mycricket.com">Cricket</asp:ListItem>
          <asp:ListItem Value="@mmst5.tracfone.com">Tracfone</asp:ListItem>
          </asp:DropDownList>
          </edititemtemplate>
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Email">
          <itemtemplate>
          <asp:Label ID="lblEmail" runat="server" Text='<%#Eval("Email")%>' 
          ></asp:Label>
          </itemtemplate>
          <edititemtemplate>
          <asp:TextBox ID="txtEmail" Width="200" runat="server" Text='<%#Eval("Email")%>' 
          ></asp:TextBox>
          <asp:RegularExpressionValidator ID="RegEx2" Display="Dynamic" runat="server" ControlToValidate="txtEmail" ErrorMessage="Please Enter Valid Email" Style="color:Red;" ValidationExpression="^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"   ValidationGroup="check2"></asp:RegularExpressionValidator>
          </edititemtemplate>
           </asp:TemplateField>
          <asp:templatefield HeaderText="Edit">
                                    <itemtemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="true" CommandName="Edit" Text="Edit" />
                                    </itemtemplate>
                                    <edititemtemplate>
                                   <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" 
                                        CausesValidation="true"   Text="Update" />
                                        <asp:LinkButton ID="Button1" runat="server" CommandName="Cancel" 
                                         CausesValidation="true"   Text="Cancel" />
                                    </edititemtemplate>
                                </asp:templatefield>
           <asp:templatefield HeaderText="Delete">
                                    <itemtemplate>
                                        <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="true" CommandName="Delete" Text="Delete" />
                                    </itemtemplate>
                                    
                                </asp:templatefield>
         
        </Columns>

    </asp:GridView>
    <br />
     
     <a href="#" id="btnPopContact" onclick="modalContact();"><img src="/images/addcontact.png" id="imgPopContact" /></a>&nbsp;&nbsp;<a href="UserContacts.aspx">Bulk Import Contacts</a>
    <div class="modalContacts" id="contactModal">
        <div class="modal-content">
            <span class="close" onclick="spanClose();">&times;</span>
            <br /><b><center>Add A Contact To Send Video Messages</center></b><br /><br />
    First Name:<br />
    <asp:TextBox ID="tbFirstName" runat="server" Width="150px"></asp:TextBox><br />
    Last Name:<br />
    <asp:TextBox ID="tbLastName" runat="server"></asp:TextBox><br />
    Cell Phone:<br />
   <asp:TextBox ID="tbCellPhone" MaxLength="10" runat="server" ValidationGroup="checkcell"></asp:TextBox><br />
    Carrier:<br />
    <asp:DropDownList ID="ddlWireless2" runat="server" Enabled="false">
          <asp:ListItem Value="Unknown">N/A</asp:ListItem>
          <asp:ListItem Value="@mms.att.net">AT&T Wireless</asp:ListItem>
          <asp:ListItem Value="@vzwpix.com">Verizon</asp:ListItem>
          <asp:ListItem Value="@tmomail.net">T-Mobile</asp:ListItem>
          <asp:ListItem Value="@pm.sprint.com">Sprint</asp:ListItem>
          <asp:ListItem Value="@mymetropcs.com">Metro PCS</asp:ListItem>
          <asp:ListItem Value="@mms.mycricket.com">Cricket</asp:ListItem>
          <asp:ListItem Value="@mmst5.tracfone.com">Tracfone</asp:ListItem>
          </asp:DropDownList><br />
    Email:<br /> 
    <asp:TextBox ID="tbEmail"
        runat="server"></asp:TextBox>
   <br /><br />
   <asp:ImageButton ID="btnTopAddUsers" runat="server" ImageUrl="~/images/savecontact.png" onclick="btnTopAddUsers_Click" OnClientClick="spanClose()" />
 
            </div>
    </div>
       
     
     </ContentTemplate>
    </asp:UpdatePanel>
 
     