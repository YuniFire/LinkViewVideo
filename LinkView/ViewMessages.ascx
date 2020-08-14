<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewMessages.ascx.cs" Inherits="LI_UserControls_ViewMessages" %>
<%@ Register Src="UploadContacts.ascx" TagName="UploadContacts" TagPrefix="uc1" %>
 
<h2>Video Messages</h2>
<script runat="server" type="text/c#">
  protected void Timer1_Tick(object sender, EventArgs e)
    {
         BindGridView();
        Label1.Text = "Messages Refreshed: " +
DateTime.Now.ToLongTimeString();
    }
</script>
   <asp:UpdatePanel ID="UpdatePanel3" runat="server">
    <ContentTemplate>
<asp:Timer runat="server" id="Timer1" Interval="60000" OnTick="Timer1_Tick"></asp:Timer>
<asp:Label runat="server" Text="" id="Label1" /><br /><br />
<asp:GridView ID="dgvUpdateRecords2" runat="server" Visible="true" EmptyDataText="No Records Found" AllowPaging="True" 
        AutoGenerateColumns="False" width="100%" PageSize="5" BorderColor="Gray" GridLines="Both" ForeColor="Black" AlternatingRowStyle-BorderStyle="Solid"  AlternatingRowStyle-BorderColor="Gray"
          RowStyle-BorderColor="Gray" RowStyle-BorderStyle="Solid" HeaderStyle-BorderColor="Gray" HeaderStyle-BorderStyle="Solid"
           FooterStyle-BorderColor="Gray" FooterStyle-BorderStyle="Solid" HeaderStyle-BackColor="beige" PagerSettings-Mode="NextPrevious" PagerSettings-NextPageText="Next" PagerSettings-PreviousPageText="Previous"
          DataKeyNames="MessageID"
      onpageindexchanging="dgvUpdateRecords2_PageIndexChanging" 
                            onrowcancelingedit="dgvUpdateRecords2_RowCancelingEdit" 
                            onrowcommand="dgvUpdateRecords2_RowCommand" 
                            onrowdeleting="dgvUpdateRecords2_RowDeleting" 
                            onrowediting="dgvUpdateRecords2_RowEditing" 
                          
                            onrowupdating="dgvUpdateRecords2_RowUpdating">
        <Columns> 
        <asp:TemplateField HeaderText=" Select Message">
          <itemtemplate>
              <asp:CheckBox ID="chkMessage" runat="server" />
          </itemtemplate>
          
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Message">
          <itemtemplate>
          <asp:Label ID="lblMessage" runat="server" Text='<%#Eval("MessageText")%>' 
          ></asp:Label>
          </itemtemplate>
          <edititemtemplate>
          <asp:TextBox ID="lblMessage" Width="489px" height="185px" runat="server" MaxLength="500" Text='<%#Eval("MessageText")%>' 
          ></asp:TextBox>
          </edititemtemplate>
        </asp:TemplateField>
         <asp:TemplateField HeaderText="Play Video">
          <itemtemplate>
          <center><a href='<%#Eval("VideoPath")%>' id="avidpath" runat="server"><asp:Image ImageUrl='<%#"~/ImageHandler.ashx?i=" + Eval("MessageID")%>' runat="server"/>
          </a></center>
          </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Video Name">
          <itemtemplate>
          <asp:Label ID="lblVideoName" runat="server" Text='<%#Eval("VideoName")%>' 
          ></asp:Label>
          </itemtemplate>
          <edititemtemplate>
          <asp:TextBox ID="lblVideoName" Width="75" ReadOnly="true" runat="server" Text='<%#Eval("VideoName")%>' 
          ></asp:TextBox>
          </edititemtemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Date Sent">
          <itemtemplate>
          <asp:Label ID="lblDateSent" runat="server" Text='<%#Eval("MessageSent")%>' 
          ></asp:Label>
          </itemtemplate>
          <edititemtemplate>
          <asp:TextBox ID="lblDateSent" Width="75" ReadOnly="true" runat="server" Text='<%#Eval("MessageSent")%>' 
          ></asp:TextBox>
          </edititemtemplate>
        </asp:TemplateField>
         <asp:templatefield HeaderText="Edit">
                                    <itemtemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="false" CommandName="Edit" Text="Edit" />
                                    </itemtemplate>
                                    <edititemtemplate>
                                   <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" 
                                        CausesValidation="false"   Text="Update" />
                                        <asp:LinkButton ID="Button1" runat="server" CommandName="Cancel" 
                                         CausesValidation="false"   Text="Cancel" />
                                    </edititemtemplate>
                                </asp:templatefield>
         <asp:templatefield HeaderText="Delete">
                                    <itemtemplate>
                                        <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="false" CommandName="Delete" Text="Delete" />
                                    </itemtemplate>
                                    
                                </asp:templatefield>

        </Columns>
        </asp:GridView>
    <br />
     </ContentTemplate>
   </asp:UpdatePanel>
  <asp:ImageButton ID="btnTopSend" ImageUrl="~/images/sendmessage.png" onclick="btnTopSend_Click" runat="server" />
  