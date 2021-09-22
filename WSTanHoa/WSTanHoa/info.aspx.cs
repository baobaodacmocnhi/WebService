using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WSTanHoa
{
    public partial class info : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (Request.QueryString["id"])
            {
                case "204":
                    lbID.Text = Request.QueryString["id"];
                    lbHoTen.Text = "Lê Tấn Đạt";
                    lbChucVu.Text = "Phó Trưởng Phòng KTCN";
                    lbDienThoai.Text = "0909933397";
                    lbEmail.Text = "letandat@capnuoctanhoa.com.vn";
                    Image1.ImageUrl = "Images/ava240.jpg";
                    break;
                case "312":
                    lbID.Text = Request.QueryString["id"];
                    lbHoTen.Text = "Nguyễn Ngọc Quốc Bảo";
                    lbChucVu.Text = "Nhân Viên Phòng KTCN";
                    lbDienThoai.Text = "0938040301";
                    lbEmail.Text = "quocbao@capnuoctanhoa.com.vn";
                    Image1.ImageUrl = "Images/ava312.jpg";
                    break;
                default:
                    lbID.Text = "ID";
                    lbHoTen.Text = "Họ Tên";
                    lbChucVu.Text = "Chức Vụ";
                    lbDienThoai.Text = "Điện Thoại";
                    lbEmail.Text = "Email";
                    Image1.ImageUrl = "Images/logoctycp.png";
                    break;
            }
            
        }
    }
}