using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSTanHoa.Models
{
    #region QLĐHN

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
        public DateTime? NgayThay { get; set; }

        /// <summary>
        /// Ngày kiểm định ĐHN
        /// </summary>
        public DateTime? NgayKiemDinh { get; set; }

        /// <summary>
        /// Hiệu lực
        /// </summary>
        public string HieuLuc { get; set; }

        public ThongTinKhachHang()
        {
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            HopDong = "";
            DienThoai = "";
            MLT = "";
            DinhMuc = "";
            GiaBieu = "";
            HieuDH = "";
            CoDH = "";
            Cap = "";
            SoThanDH = "";
            ViTriDHN = "";
            NgayThay = null;
            NgayKiemDinh = null;
            HieuLuc = "";
        }
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
        public DateTime? NgayDoc { get; set; }

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

        public GhiChiSo()
        {
            Ky = "";
            NgayDoc = null;
            CodeMoi = "";
            ChiSoCu = "";
            ChiSoMoi = "";
            TieuThu = "";
        }
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
        public DateTime? CreateDate { get; set; }

        public GhiChu()
        {
            NoiDung = "";
            CreateDate = null;
        }
    }

    #endregion

    #region Thu Tiền

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
        public DateTime? NgayGiaiTrach { get; set; }

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
        public DateTime? NgayDN { get; set; }

        /// <summary>
        /// Ngày mở nước
        /// </summary>
        public DateTime? NgayMN { get; set; }

        public HoaDonThuTien()
        {
            GiaBieu = "";
            DinhMuc = "";
            SoHoaDon = "";
            Ky = "";
            TieuThu = "";
            GiaBan = "";
            ThueGTGT = "";
            PhiBVMT = "";
            TongCong = "";
            NgayGiaiTrach = null;
            DangNgan = "";
            HanhThu = "";
            MaDN = "";
            NgayDN = null;

        }
    }

    #endregion

    #region Phòng Kinh Doanh

    public class DonKinhDoanh
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Loại đơn
        /// </summary>
        public string TenLD { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

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
        /// Giá biểu
        /// </summary>
        public string GiaBieu { get; set; }

        /// <summary>
        /// Định mức
        /// </summary>
        public string DinhMuc { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string NoiDung { get; set; }

        /// <summary>
        /// Kiểm tra xác minh
        /// </summary>
        public List<KTXM> ktxm { get; set; }

        /// <summary>
        /// Bấm chì
        /// </summary>
        public List<BamChi> bamchi { get; set; }

        /// <summary>
        /// Đóng nước
        /// </summary>
        public List<DongNuoc> dongnuoc { get; set; }

        /// <summary>
        /// Điều chỉnh biến động
        /// </summary>
        public List<DCBD> dcbd { get; set; }

        /// <summary>
        /// Điều chỉnh hóa đơn
        /// </summary>
        public List<DCHD> dchd { get; set; }

        /// <summary>
        /// Cắt hủy danh bộ
        /// </summary>
        public List<CHDB> chdb { get; set; }

        /// <summary>
        /// Phiếu cắt hủy danh bộ
        /// </summary>
        public List<PhieuCHDB> phieuchdb { get; set; }

        /// <summary>
        /// Thư trả lời
        /// </summary>
        public List<TTL> ttl { get; set; }

        /// <summary>
        /// Gian lận
        /// </summary>
        public List<GianLan> gianlan { get; set; }

        /// <summary>
        /// Truy thu
        /// </summary>
        public List<TruyThu> truythu { get; set; }

        /// <summary>
        /// Tờ trình
        /// </summary>
        public List<ToTrinh> totrinh { get; set; }

        /// <summary>
        /// Thư mời
        /// </summary>
        public List<ThuMoi> thumoi { get; set; }

        /// <summary>
        /// Tiến trình đơn
        /// </summary>
        public List<TienTrinh> tientrinh { get; set; }

        public DonKinhDoanh()
        {
            MaDon = "";
            TenLD = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            GiaBieu = "";
            DinhMuc = "";
            NoiDung = "";
            ktxm = new List<KTXM>();
            bamchi = new List<BamChi>();
            dongnuoc = new List<DongNuoc>();
            dcbd = new List<DCBD>();
            dchd = new List<DCHD>();
            chdb = new List<CHDB>();
            phieuchdb = new List<PhieuCHDB>();
            ttl = new List<TTL>();
            gianlan = new List<GianLan>();
            truythu = new List<TruyThu>();
            totrinh = new List<ToTrinh>();
            thumoi = new List<ThuMoi>();
            tientrinh = new List<TienTrinh>();
        }

    }

    public class KTXM
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày kiểm tra
        /// </summary>
        public DateTime? NgayKTXM { get; set; }

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
        /// Nội dung kiểm tra
        /// </summary>
        public string NoiDungKiemTra { get; set; }

        /// <summary>
        /// Nội dung đóng tiền
        /// </summary>
        public string NoiDungDongTien { get; set; }

        /// <summary>
        /// Ngày đóng tiền
        /// </summary>
        public DateTime? NgayDongTien { get; set; }

        /// <summary>
        /// Số tiền đóng tiền
        /// </summary>
        public string SoTienDongTien { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public KTXM()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            NgayKTXM = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            NoiDungKiemTra = "";
            NoiDungDongTien = "";
            NgayDongTien = null;
            SoTienDongTien = "";
            CreateBy = "";
        }
    }

    public class BamChi
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày bấm chì
        /// </summary>
        public DateTime? NgayBC { get; set; }

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
        /// Trạng thái bấm chì
        /// </summary>
        public string TrangThaiBC { get; set; }

        /// <summary>
        /// Theo yêu cầu
        /// </summary>
        public string TheoYeuCau { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public BamChi()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            NgayBC = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            TrangThaiBC = "";
            TheoYeuCau = "";
            CreateBy = "";
        }
    }

    public class DongNuoc
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày đóng nước
        /// </summary>
        public DateTime? NgayDN { get; set; }

        /// <summary>
        /// Ngày mở nước
        /// </summary>
        public DateTime? NgayMN { get; set; }

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
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public DongNuoc()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            NgayDN = null;
            NgayMN = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            CreateBy = "";
        }
    }

    public class DCBD
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

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
        /// Giá biểu cũ
        /// </summary>
        public string GiaBieu { get; set; }

        /// <summary>
        /// Giá biểu mới
        /// </summary>
        public string GiaBieu_BD { get; set; }

        /// <summary>
        /// Định mức cũ
        /// </summary>
        public string DinhMuc { get; set; }

        /// <summary>
        /// Định mức mới
        /// </summary>
        public string DinhMuc_BD { get; set; }

        /// <summary>
        /// Họ tên mới
        /// </summary>
        public string HoTen_BD { get; set; }

        /// <summary>
        /// Địa chỉ mới
        /// </summary>
        public string DiaChi_BD { get; set; }

        /// <summary>
        /// Thông tin
        /// </summary>
        public string ThongTin { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public DCBD()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            GiaBieu = "";
            GiaBieu_BD = "";
            DinhMuc = "";
            DinhMuc_BD = "";
            HoTen_BD = "";
            DiaChi_BD = "";
            ThongTin = "";
            CreateBy = "";
        }
    }

    public class DCHD
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

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
        /// Giá biểu cũ
        /// </summary>
        public string GiaBieu { get; set; }

        /// <summary>
        /// Giá biểu mới
        /// </summary>
        public string GiaBieu_BD { get; set; }

        /// <summary>
        /// Định mức cũ
        /// </summary>
        public string DinhMuc { get; set; }

        /// <summary>
        /// Định mức mới
        /// </summary>
        public string DinhMuc_BD { get; set; }

        /// <summary>
        /// Tiêu thụ cũ
        /// </summary>
        public string TieuThu { get; set; }

        /// <summary>
        /// Tiêu thụ mới
        /// </summary>
        public string TieuThu_BD { get; set; }

        /// <summary>
        /// Tổng cộng trước
        /// </summary>
        public string TongCong_Start { get; set; }

        /// <summary>
        /// Tổng cộng sau
        /// </summary>
        public string TongCong_End { get; set; }

        /// <summary>
        /// Tổng cộng biến động
        /// </summary>
        public string TongCong_BD { get; set; }

        /// <summary>
        /// Tăng giảm
        /// </summary>
        public string TangGiam { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public DCHD()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            GiaBieu = "";
            GiaBieu_BD = "";
            DinhMuc = "";
            DinhMuc_BD = "";
            TieuThu = "";
            TieuThu_BD = "";
            TongCong_Start = "";
            TongCong_End = "";
            TongCong_BD = "";
            TangGiam = "";
            CreateBy = "";
        }
    }

    public class CHDB
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

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
        /// Loại cắt
        /// </summary>
        public string LoaiCat { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        public string LyDo { get; set; }

        /// <summary>
        /// Ghi chú lý do
        /// </summary>
        public string GhiChuLyDo { get; set; }

        /// <summary>
        /// Đã lập phiếu
        /// </summary>
        public bool DaLapPhieu { get; set; }

        /// <summary>
        /// Số phiếu
        /// </summary>
        public string SoPhieu { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime? NgayLapPhieu { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public CHDB()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            LoaiCat = "";
            LyDo = "";
            GhiChuLyDo = "";
            DaLapPhieu = false;
            SoPhieu = "";
            NgayLapPhieu = null;
            CreateBy = "";
        }
    }

    public class PhieuCHDB
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

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
        /// Lý do
        /// </summary>
        public string LyDo { get; set; }

        /// <summary>
        /// Ghi chú lý do
        /// </summary>
        public string GhiChuLyDo { get; set; }

        /// <summary>
        /// Hiệu lực kỳ
        /// </summary>
        public string HieuLucKy { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public PhieuCHDB()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            LyDo = "";
            GhiChuLyDo = "";
            HieuLucKy = "";
            CreateBy = "";
        }
    }

    public class TTL
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

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
        /// Về việc
        /// </summary>
        public string VeViec { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string NoiDung { get; set; }

        /// <summary>
        /// Nơi nhận
        /// </summary>
        public string NoiNhan { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public TTL()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            VeViec = "";
            NoiDung = "";
            NoiNhan = "";
            CreateBy = "";
        }
    }

    public class GianLan
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

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
        /// Nội dung vi phạm
        /// </summary>
        public string NoiDungViPham { get; set; }

        /// <summary>
        /// Tình trạng
        /// </summary>
        public string TinhTrang { get; set; }

        /// <summary>
        /// Thanh toán 1
        /// </summary>
        public bool ThanhToan1 { get; set; }

        /// <summary>
        /// Thanh toán 2
        /// </summary>
        public bool ThanhToan2 { get; set; }

        /// <summary>
        /// Thanh toán 3
        /// </summary>
        public bool ThanhToan3 { get; set; }

        /// <summary>
        /// Xếp đơn
        /// </summary>
        public bool XepDon { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public GianLan()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            NoiDungViPham = "";
            TinhTrang = "";
            ThanhToan1 = false;
            ThanhToan2 = false;
            ThanhToan3 = false;
            XepDon = false;
            CreateBy = "";
        }
    }

    public class TruyThu
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

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
        /// Nội dung
        /// </summary>
        public string NoiDung { get; set; }

        /// <summary>
        /// Tổng tiền
        /// </summary>
        public string TongTien { get; set; }

        /// <summary>
        /// Tổng m3
        /// </summary>
        public string Tongm3BinhQuan { get; set; }

        /// <summary>
        /// Tình trạng
        /// </summary>
        public string TinhTrang { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public TruyThu()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            NoiDung = "";
            TongTien = "";
            Tongm3BinhQuan = "";
            TinhTrang = "";
            CreateBy = "";
        }
    }

    public class ToTrinh
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

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
        /// Về việc
        /// </summary>
        public string VeViec { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string NoiDung { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public ToTrinh()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            VeViec = "";
            NoiDung = "";
            CreateBy = "";
        }
    }

    public class ThuMoi
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

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
        /// Lần gửi thứ
        /// </summary>
        public string Lan { get; set; }

        /// <summary>
        /// Về việc
        /// </summary>
        public string VeViec { get; set; }

        /// <summary>
        /// Người lập
        /// </summary>
        public string CreateBy { get; set; }

        public ThuMoi()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            Lan = "";
            VeViec = "";
            CreateBy = "";
        }
    }

    public class TienTrinh
    {
        /// <summary>
        /// Mã đơn
        /// </summary>
        public string MaDon { get; set; }

        /// <summary>
        /// Số thứ tự Tab
        /// </summary>
        public string TabSTT { get; set; }

        /// <summary>
        /// Tên Tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Ngày chuyển
        /// </summary>
        public DateTime? NgayChuyen { get; set; }

        /// <summary>
        /// Nơi chuyển
        /// </summary>
        public string NoiChuyen { get; set; }

        /// <summary>
        /// Nơi nhận
        /// </summary>
        public string NoiNhan { get; set; }

        /// <summary>
        /// Nhân viên kiểm tra
        /// </summary>
        public string KTXM { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string NoiDung { get; set; }

        public TienTrinh()
        {
            MaDon = "";
            TabSTT = "";
            TabName = "";
            NgayChuyen = null;
            NoiChuyen = "";
            NoiNhan = "";
            KTXM = "";
            NoiDung = "";
        }

        #endregion

    }

}