<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="info.aspx.cs" Inherits="WSTanHoa.info" %>

<!DOCTYPE html>

<head runat="server">
    <title></title>
</head>
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<link href="Content/bootstrap.css" rel="stylesheet" />
<script src="Scripts/bootstrap.js"></script>

<body>
    <form id="form1" runat="server">
        <div class="container bg-white mt-3 rounded-3 shadow pb-2">
            <div class="text-center">
                <img src="Images/logofull.jpg" />
            </div>
            <div class="p-2 rounded-3 shadow-sm" style="background-color: rgba(84, 189, 218, 0.90)">
                <div class="row row-cols-1 row-cols-md-auto justify-content-center">
                    <div class="col text-center mt-2">
                        <div class="">
                            <div class="text-warning fw-bold fs-5">THW-<asp:Label ID="lbID" runat="server" Text="ID"></asp:Label></div>
                            <asp:Image ID="Image1" runat="server" class="img-fluid rounded-3" ImageUrl="Images/logoctycp.png" Width="200px" />
                        </div>
                    </div>
                    <div class="col">
                        <div class="mt-3">
                            <p class="text-white fw-bold fs-1 mt-1 text-center">
                                <asp:Label ID="lbHoTen" runat="server" Text="Họ Tên"></asp:Label>
                            </p>
                            <p class="text-white pt-3 fw-bold fs-5">
                                <img src="Images/info.announcement.png" /> <asp:Label ID="lbChucVu" runat="server" Text="Chức Vụ"></asp:Label>
                            </p>
                            <p class="text-white fw-bold fs-5">
                                <img src="Images/info.mobile.png" /> <asp:Label ID="lbDienThoai" runat="server" Text="Điện Thoại"></asp:Label>
                            </p>
                            <p class="text-white fw-bold fs-5">
                                <img src="Images/info.email.png" /> <asp:Label ID="lbEmail" runat="server" Text="Email"></asp:Label>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <%--<div class="card mb-2 ps-1 pb-2 align-items-center" >
                <div class="row align-items-center">
                    <div class="col text-center">
                        <div class="text-warning fw-bold fs-5">THW-<asp:Label ID="lbID" runat="server" Text="ID"></asp:Label></div>
                        <asp:Image ID="Image1" runat="server" class="img-fluid rounded-3" ImageUrl="Images/logoctycp.png" Width="200px"/>
                    </div>
                    <div class="col" style="">
                        <div class="card-body">
                            <p class="card-title text-white fw-bold fs-1"><asp:Label ID="lbHoTen" runat="server" Text="Họ Tên"></asp:Label></p>
                            <p class="card-text text-white pt-3 fw-bold fs-5"><asp:Label ID="lbChucVu" runat="server" Text="Chức Vụ"></asp:Label></p>
                            <p class="card-text text-white fw-bold fs-5"><asp:Label ID="lbDienThoai" runat="server" Text="Điện Thoại"></asp:Label></p>
                            <p class="card-text text-white fw-bold fs-5"><asp:Label ID="lbEmail" runat="server" Text="Email"></asp:Label></p>
                        </div>
                    </div>
                </div>
            </div>
        </div>--%>

    </form>
</body>

