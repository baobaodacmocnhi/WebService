<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestServices._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<link href="Stylesheet1.css" rel="stylesheet" type="text/css" />
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>....: Tra Cuu Hoa Don Tien Nuoc :....</title>
    <style type="text/css">
        .style1
        {
            height: 41px;
            
        }
        .style4
        {
            height: 41px;
            width: 83px;
        }
        .style5
        {
        }
        .style6
        {
            height: 41px;
            width: 281px;
        }
        .style7
        {
            width: 281px;
        }
        .style8
        {
            height: 41px;
            width: 50px;
        }
        .style9
        {
            width: 50px;
        }
        .style11
        {
            width: 125px;
        }
        </style>
</head>
<body>
<br />
<div class="block_content">
    <form id="form1" runat="server">    
        <div class="title_page" style="hight:20px;text-align:center;">
            <asp:Label ID="title" runat="server" 
        Text="TRA CỨU THÔNG TIN HÓA ĐƠN NỢ TIỀN NƯỚC  " Font-Size="Large"></asp:Label>
    </div>
    
    
    
        <table style="width:100%; margin-top:10px;  font-size: 13px">
            <tr>
                <td class="style6">
                    </td>
                <td class="style4">
                    Danh Bộ
                </td>
                <td class="style1">
                    <asp:TextBox ID="TextBox1" runat="server" Width="216px" BackColor=Yellow></asp:TextBox>
                </td>
                <td class="style8">
                    </td>
                <td class="style1">
                    </td>
                
            </tr>
            <tr>
                <td class="style7">
                    &nbsp;</td>
                <td class="style5">
                    &nbsp;</td>
                <td align="center" style="text-align: left" valign="middle">
                    <asp:Button ID="btXemBangKe" runat="server" Text="XEM THÔNG TIN" 
                        CssClass="button"  Height="30px" onclick="btXemBangKe_Click"  />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </td>
                <td class="style9">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style7">
                    &nbsp;</td>
                <td class="style5" colspan="2">
                    <br />
                    <asp:Panel ID="Panel1" runat="server">
                    <fieldset>
                       <legend><b>THÔNG TIN HÓA ĐƠN KHÁCH HÀNG </b></legend>                               
                                               
                        <asp:DetailsView ID="DetailsView1" runat="server" 
                            style=" font-family:Times New Roman; font-size:15px; margin-top:7px; margin-left:10px;" 
                            AutoGenerateRows="False" BackColor="White" BorderColor="#336666" 
                            BorderStyle="Double" BorderWidth="3px" CellPadding="4" GridLines="Horizontal" 
                            Width="407px">
                            <FooterStyle BackColor="White" ForeColor="#333333" />
                            <RowStyle BackColor="White" ForeColor="#333333" />
                            <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
                            <Fields>
                                <asp:TemplateField HeaderText="Danh Bạ">
                                    <ItemTemplate>
                                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("DANHBA") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("DANHBA") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("DANHBA") %>'></asp:TextBox>
                                    </InsertItemTemplate>
                                    <ItemStyle Font-Bold="True" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Tên Khách Hàng">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" style="font-weight: 700" 
                                            Text='<%# Bind("TENKH") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TENKH") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TENKH") %>'></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Số Nhà">
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" style="font-weight: 700" 
                                            Text='<%# Bind("SONHA") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("SONHA") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("SONHA") %>'></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Đường">
                                    <ItemTemplate>
                                        <asp:Label ID="Label3" runat="server" style="font-weight: 700" 
                                            Text='<%# Bind("TENDUONG") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("TENDUONG") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("TENDUONG") %>'></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                            </Fields>
                            <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
                            <EditRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
                        </asp:DetailsView>
                       
                       
                         <asp:Panel ID="Panel2" runat="server">
                             <table style="width:100%;">
                                 <tr>
                                     <td class="style11">
                                         &nbsp;</td>
                                     <td>
                                         <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                                             BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" 
                                             CellPadding="4" onrowdatabound="GridView1_RowDataBound" ShowFooter="True" 
                                             Width="629px">
                                             <RowStyle BackColor="White" ForeColor="#330099" />
                                             <Columns>
                                                 <asp:TemplateField HeaderText="IDKey">
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label7" runat="server" Text='<%# Bind("IDkey") %>'></asp:Label>
                                                     </ItemTemplate>
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("IDkey") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemStyle HorizontalAlign="Center" />
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="Kỳ HĐ">
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label1" runat="server" Text='<%# Bind("KyHD") %>'></asp:Label>
                                                     </ItemTemplate>
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("KyHD") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemStyle HorizontalAlign="Center" />
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="Năm HĐ">
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label2" runat="server" Text='<%# Bind("NamHD") %>'></asp:Label>
                                                     </ItemTemplate>
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("NamHD") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemStyle HorizontalAlign="Center" />
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="Tiền Nước">
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label3" runat="server" Text='<%# Bind("TNuoc","{0:0,0}") %>'></asp:Label>
                                                     </ItemTemplate>
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("TNuoc") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemStyle HorizontalAlign="Right" />
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="Phí BVMT">
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label4" runat="server" Text='<%# Bind("PBVMT","{0:0,0}") %>'></asp:Label>
                                                     </ItemTemplate>
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("PBVMT") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemStyle HorizontalAlign="Right" />
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="Thuế GTGT">
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label5" runat="server" Text='<%# Bind("TGTGT","{0:0,0}") %>'></asp:Label>
                                                     </ItemTemplate>
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("TGTGT") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <FooterTemplate>
                                                         <asp:Label ID="Label10" runat="server" Text="Tổng Cộng :" 
                                                             style="font-weight: 700"></asp:Label>
                                                     </FooterTemplate>
                                                     <FooterStyle HorizontalAlign="Right" />
                                                     <ItemStyle HorizontalAlign="Right" />
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="Tổng Cộng">
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label6" runat="server" Text='<%# Bind("TONGCONG","{0:0,0}") %>'></asp:Label>
                                                     </ItemTemplate>
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("TONGCONG") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <FooterTemplate>
                                                         <asp:Label ID="Label9" runat="server" style="font-weight: 700" Text="Label"></asp:Label>                                                         
                                                     </FooterTemplate>
                                                     <FooterStyle HorizontalAlign="Right" />
                                                     <ItemStyle HorizontalAlign="Right" />
                                                 </asp:TemplateField>
                                                
                                                 <asp:TemplateField HeaderText="DANHBA" Visible="False">
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox10" runat="server" Text='<%# Bind("DANHBA") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label10" runat="server" Text='<%# Bind("DANHBA") %>'></asp:Label>
                                                     </ItemTemplate>
                                                 </asp:TemplateField>
                                                 
                                                 <asp:TemplateField HeaderText="KHACHHANG" Visible="False">
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox12" runat="server" Text='<%# Bind("TENKH") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label12" runat="server" Text='<%# Bind("TENKH") %>'></asp:Label>
                                                     </ItemTemplate>
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="SONHA" Visible="False">
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox13" runat="server" Text='<%# Bind("SONHA") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label13" runat="server" Text='<%# Bind("SONHA") %>'></asp:Label>
                                                     </ItemTemplate>
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="TENDUONG" Visible="False">
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox14" runat="server" Text='<%# Bind("TENDUONG") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label14" runat="server" Text='<%# Bind("TENDUONG") %>'></asp:Label>
                                                     </ItemTemplate>
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="GB" Visible="False">
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox9" runat="server" Text='<%# Bind("GB") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label9" runat="server" Text='<%# Bind("GB") %>'></asp:Label>
                                                     </ItemTemplate>
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="DM" Visible="False">
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("DM") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label8" runat="server" Text='<%# Bind("DM") %>'></asp:Label>
                                                     </ItemTemplate>
                                                 </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="DOT" Visible="False">
                                                     <EditItemTemplate>
                                                         <asp:TextBox ID="TextBox11" runat="server" Text='<%# Bind("DOT") %>'></asp:TextBox>
                                                     </EditItemTemplate>
                                                     <ItemTemplate>
                                                         <asp:Label ID="Label11" runat="server" Text='<%# Bind("DOT") %>'></asp:Label>
                                                     </ItemTemplate>
                                                 </asp:TemplateField>
                                             </Columns>
                                             <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                                             <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                                             <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                                             <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                                         </asp:GridView>
                                         <br />
                                         &nbsp;<asp:Button ID="Button1" runat="server" Text="THANH TOÁN" 
                        CssClass="button"  Height="30px" onclick="Button1_Click"  Visible="false"  />
&nbsp;
<asp:Label ID="Label11" runat="server" Font-Bold="True" ForeColor="Blue" 
                                             Text="Không Tìm Thấy Hóa Đơn Nợ !!"></asp:Label></td>
                                 </tr>
                             </table>
                        </asp:Panel>
                        
                         <br />    
                            </fieldset>
                    </asp:Panel>
                    
                </td>
                <td class="style9">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
        </table>
        <br />  
    
    </form>
    </div>
</body>
</html>
