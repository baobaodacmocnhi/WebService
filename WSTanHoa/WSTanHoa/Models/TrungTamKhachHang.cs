using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSTanHoa.Models
{
    public class ThongTinKhachHang
    {
        /// <summary>
        /// Danh bộ
        /// </summary>
        public string DanhBo { get; set; }

        /// <summary>
        /// Họ tên
        /// </summary>
        public string HoTen { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string DiaChi { get; set; }

        /// <summary>
        /// Hợp đồng
        /// </summary>
        public string HopDong { get; set; }

        /// <summary>
        /// Điện thoại
        /// </summary>
        public string DienThoai { get; set; }

        /// <summary>
        /// Mã lộ trình
        /// </summary>
        public string MLT { get; set; }

        /// <summary>
        /// Định mức
        /// </summary>
        public string DinhMuc { get; set; }

        /// <summary>
        /// Giá Biểu
        /// </summary>
        public string GiaBieu { get; set; }

        /// <summary>
        /// Hiệu ĐHN
        /// </summary>
        public string HieuDH { get; set; }

        /// <summary>
        /// Cỡ ĐHN
        /// </summary>
        public string CoDH { get; set; }

        /// <summary>
        /// Cấp
        /// </summary>
        public string Cap { get; set; }

        /// <summary>
        /// Số thân ĐHN
        /// </summary>
        public string SoThanDH { get; set; }

        /// <summary>
        /// Vị trí ĐHN
        /// </summary>
        public string ViTriDHN { get; set; }

        /// <summary>
        /// Ngày thay ĐHN
        /// </summary>
        public DateTime NgayThay { get; set; }

        /// <summary>
        /// Ngày kiểm định ĐHN
        /// </summary>
        public DateTime NgayKiemDinh { get; set; }

        /// <summary>
        /// Hiệu lực
        /// </summary>
        public string HieuLuc { get; set; }
    }

    public class GhiChiSo
    {
        /// <summary>
        /// Kỳ
        /// </summary>
        public string Ky { get; set; }

        /// <summary>
        /// Ngày đọc
        /// </summary>
        public DateTime NgayDoc { get; set; }

        /// <summary>
        /// Code mới
        /// </summary>
        public string CodeMoi { get; set; }

        /// <summary>
        /// Chỉ số cũ
        /// </summary>
        public string ChiSoCu { get; set; }

        /// <summary>
        /// Chỉ số mới
        /// </summary>
        public string ChiSoMoi { get; set; }

        /// <summary>
        /// Tiêu thụ
        /// </summary>
        public string TieuThu { get; set; }
    }
    public class GhiChu
    {
        /// <summary>
        /// Nội dung
        /// </summary>
        public string NoiDung { get; set; }
        
        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime CreateDate { get; set; }
    }

    public class HoaDonThuTien
    {
        /// <summary>
        /// Giá biểu
        /// </summary>
        public string GiaBieu { get; set; }

        /// <summary>
        /// Định mức
        /// </summary>
        public string DinhMuc { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string SoHoaDon { get; set; }

        /// <summary>
        /// Kỳ
        /// </summary>
        public string Ky { get; set; }

        /// <summary>
        /// Tiêu Thụ
        /// </summary>
        public string TieuThu { get; set; }

        /// <summary>
        /// Giá bán
        /// </summary>
        public string GiaBan { get; set; }

        /// <summary>
        /// Thuế GTGT
        /// </summary>
        public string ThueGTGT { get; set; }

        /// <summary>
        /// Phí BVMT
        /// </summary>
        public string PhiBVMT { get; set; }

        /// <summary>
        /// Tổng cộng
        /// </summary>
        public string TongCong { get; set; }

        /// <summary>
        /// Ngày giải trách
        /// </summary>
        public DateTime NgayGiaiTrach { get; set; }

        /// <summary>
        /// Đăng ngân
        /// </summary>
        public string DangNgan { get; set; }

        /// <summary>
        /// Hành thu
        /// </summary>
        public string HanhThu { get; set; }

        /// <summary>
        /// Lệnh đóng nước
        /// </summary>
        public string MaDN { get; set; }

        /// <summary>
        /// Ngày đóng nước
        /// </summary>
        public DateTime NgayDN { get; set; }

        /// <summary>
        /// Ngày mở nước
        /// </summary>
        public DateTime NgayMN { get; set; }
    }
}