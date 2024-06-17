using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WSTanHoa.Providers;

namespace WSTanHoa
{
    public partial class info : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.Count > 0 && Request.QueryString.AllKeys.Contains("id") && Request.QueryString["id"].ToString() != "")
            {
                CConnection _cDAL_TTKH = new CConnection(CGlobalVariable.TrungTamKhachHang);
                DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable("select * from NhanVien where MaNV='" + Request.QueryString["id"] + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    lbID.Text = dt.Rows[0]["MaNV"].ToString();
                    lbHoTen.Text = dt.Rows[0]["HoTen"].ToString();
                    lbChucVu.Text = dt.Rows[0]["ChucDanh"].ToString();
                    lbDienThoai.Text = dt.Rows[0]["DienThoai"].ToString();
                    lbEmail.Text = dt.Rows[0]["Email"].ToString();
                    Image1.ImageUrl = "Images/NhanVien/" + dt.Rows[0]["MaNV"].ToString() + ".jpg";
                }
                else
                {
                    lbID.Text = "MaNV";
                    lbHoTen.Text = "Họ Tên";
                    lbChucVu.Text = "Chức Vụ";
                    lbDienThoai.Text = "Điện Thoại";
                    lbEmail.Text = "Email";
                    Image1.ImageUrl = "Images/logoctycp.png";
                }
            }
            else
            {
                lbID.Text = "MaNV";
                lbHoTen.Text = "Họ Tên";
                lbChucVu.Text = "Chức Vụ";
                lbDienThoai.Text = "Điện Thoại";
                lbEmail.Text = "Email";
                Image1.ImageUrl = "Images/logoctycp.png";
            }
        }
    }
}