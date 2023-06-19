using System;
using System.Collections.Generic;
using System.Data;
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
        /// Địa chỉ hóa đơn
        /// </summary>
        public string DiaChiHoaDon { get; set; }

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
        /// Giá Biểu
        /// </summary>
        public string GiaBieu { get; set; }

        /// <summary>
        /// Định mức
        /// </summary>
        public string DinhMuc { get; set; }

        /// <summary>
        /// Định mức hộ nghèo
        /// </summary>
        public string DinhMucHN { get; set; }

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

        /// <summary>
        /// Thông tin
        /// </summary>
        public string ThongTin { get; set; }

        /// <summary>
        /// Hồ sơ gốc
        /// </summary>
        public string HoSoGoc { get; set; }

        /// <summary>
        /// Bấm chì
        /// </summary>
        public string BamChi { get; set; }

        public string ThongTinDongNuoc { get; set; }

        public string DMA { get; set; }

        public string NVDocSo { get; set; }

        public string NVThuTien { get; set; }

        public List<HoaDonThuTien> lstHoaDon { get; set; }

        public List<ChartData> ChartData { get; set; }

        public ThongTinKhachHang()
        {
            DanhBo = "";
            HoTen = "";
            DiaChi = DiaChiHoaDon = "";
            HopDong = "";
            DienThoai = "";
            MLT = "";
            GiaBieu = "";
            DinhMuc = "";
            DinhMucHN = "";
            HieuDH = "";
            CoDH = "";
            Cap = "";
            SoThanDH = "";
            ViTriDHN = "";
            NgayThay = null;
            NgayKiemDinh = null;
            HieuLuc = "";
            ThongTin = "";
            HoSoGoc = "";
            BamChi = "";
            ThongTinDongNuoc = "";
            DMA = "";
            NVDocSo = "";
            NVThuTien = "";
            lstHoaDon = new List<HoaDonThuTien>();
            ChartData = new List<ChartData>();
        }
    }

    public class DocSo
    {
        /// <summary>
        /// Nhân viên đọc số
        /// </summary>
        public string NhanVien { get; set; }

        /// <summary>
        /// Lịch ghi chỉ số
        /// </summary>
        public List<GhiChiSo> lstGhiChiSo { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public List<GhiChu> lstGhiChu { get; set; }

        public DocSo()
        {
            NhanVien = "";
            lstGhiChiSo = new List<GhiChiSo>();
            lstGhiChu = new List<GhiChu>();
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

    public class ThuTien
    {
        /// <summary>
        /// Nhân viên thu tiền
        /// </summary>
        public string NhanVien { get; set; }

        /// <summary>
        /// Thông tin
        /// </summary>
        public string ThongTin { get; set; }

        /// <summary>
        /// Danh sách hóa đơn
        /// </summary>
        public List<HoaDonThuTien> lstHoaDon { get; set; }

        public ThuTien()
        {
            NhanVien = "";
            ThongTin = "";
            lstHoaDon = new List<HoaDonThuTien>();
        }
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
        /// Định mức hộ nghèo
        /// </summary>
        public string DinhMucHN { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string SoHoaDon { get; set; }

        /// <summary>
        /// Kỳ
        /// </summary>
        public string Ky { get; set; }

        public string CSC { get; set; }

        public string CSM { get; set; }

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
        /// Tiền dịch vụ thoát nước
        /// </summary>
        public string PhiBVMT { get; set; }

        /// <summary>
        /// Thuế TDVTN
        /// </summary>
        public string PhiBVMT_Thue { get; set; }

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
        /// Lệnh đóng nước, nếu có giá trị thì tô màu vàng
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

        /// <summary>
        /// Đóng nước lần 2, true => tô màu cam
        /// </summary>
        public bool DongNuoc2 { get; set; }

        /// <summary>
        /// Lệnh hủy, true => tô màu đỏ
        /// </summary>
        public bool LenhHuy { get; set; }

        /// <summary>
        /// Tờ trình cắt hủy gửi Phòng Kinh Doanh, true => tô màu xanh lá
        /// </summary>
        public bool ToTrinh { get; set; }


        public HoaDonThuTien()
        {
            GiaBieu = "";
            DinhMuc = "";
            DinhMucHN = "";
            SoHoaDon = "";
            Ky = "";
            CSC = "";
            CSM = "";
            TieuThu = "";
            GiaBan = "";
            ThueGTGT = "";
            PhiBVMT = "";
            PhiBVMT_Thue = "";
            TongCong = "";
            NgayGiaiTrach = null;
            DangNgan = "";
            HanhThu = "";
            MaDN = "";
            NgayDN = null;
            DongNuoc2 = false;
            LenhHuy = false;
            ToTrinh = false;
        }

    }

    public class ChartData
    {
        public string Ky { get; set; }
        public int TieuThu { get; set; }
        public decimal ThanhTien { get; set; }

        public ChartData()
        {
            Ky = "";
            TieuThu = 0;
            ThanhTien = 0;
        }

    }

    #endregion

    #region Phòng Kinh Doanh

    public class DonKinhDoanh
    {
        /// <summary>
        /// Thông tin
        /// </summary>
        public string ThongTin { get; set; }

        /// <summary>
        /// Danh sách hóa đơn
        /// </summary>
        public List<DonTu> lstDonTu { get; set; }
        public DonKinhDoanh()
        {
            ThongTin = "";
            lstDonTu = new List<DonTu>();
        }
    }

    public class DonTu
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
        /// Định mức hộ nghèo
        /// </summary>
        public string DinhMucHN { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string NoiDung { get; set; }

        /// <summary>
        /// Danh sách Kiểm tra xác minh
        /// </summary>
        public List<KTXM> lstKTXM { get; set; }

        /// <summary>
        /// Danh sách Bấm chì
        /// </summary>
        public List<BamChi> lstBamChi { get; set; }

        /// <summary>
        /// Danh sách Đóng nước
        /// </summary>
        public List<DongNuoc> lstDongNuoc { get; set; }

        /// <summary>
        /// Danh sách Điều chỉnh biến động
        /// </summary>
        public List<DCBD> lstDCBD { get; set; }

        /// <summary>
        /// Danh sách Điều chỉnh hóa đơn
        /// </summary>
        public List<DCHD> lstDCHD { get; set; }

        /// <summary>
        /// Danh sách Cắt hủy danh bộ
        /// </summary>
        public List<CHDB> lstCHDB { get; set; }

        /// <summary>
        /// Danh sách Phiếu cắt hủy danh bộ
        /// </summary>
        public List<PhieuCHDB> lstPhieuCHDB { get; set; }

        /// <summary>
        /// Danh sách Thư trả lời
        /// </summary>
        public List<TTL> lstTTL { get; set; }

        /// <summary>
        /// Danh sách Gian lận
        /// </summary>
        public List<GianLan> lstGianLan { get; set; }

        /// <summary>
        /// Danh sách Truy thu
        /// </summary>
        public List<TruyThu> lstTruyThu { get; set; }

        /// <summary>
        /// Danh sách Tờ trình
        /// </summary>
        public List<ToTrinh> lstToTrinh { get; set; }

        /// <summary>
        /// Danh sách Thư mời
        /// </summary>
        public List<ThuMoi> lstThuMoi { get; set; }

        /// <summary>
        /// Danh sách Tiến trình đơn
        /// </summary>
        public List<TienTrinh> lstTienTrinh { get; set; }

        public DonTu()
        {
            MaDon = "";
            TenLD = "";
            CreateDate = null;
            DanhBo = "";
            HoTen = "";
            DiaChi = "";
            GiaBieu = "";
            DinhMuc = "";
            DinhMucHN = "";
            NoiDung = "";
            lstKTXM = new List<KTXM>();
            lstBamChi = new List<BamChi>();
            lstDongNuoc = new List<DongNuoc>();
            lstDCBD = new List<DCBD>();
            lstDCHD = new List<DCHD>();
            lstCHDB = new List<CHDB>();
            lstPhieuCHDB = new List<PhieuCHDB>();
            lstTTL = new List<TTL>();
            lstGianLan = new List<GianLan>();
            lstTruyThu = new List<TruyThu>();
            lstToTrinh = new List<ToTrinh>();
            lstThuMoi = new List<ThuMoi>();
            lstTienTrinh = new List<TienTrinh>();
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
        /// Định mức hộ nghèo cũ
        /// </summary>
        public string DinhMucHN { get; set; }

        /// <summary>
        /// Định mức hộ nghèo mới
        /// </summary>
        public string DinhMucHN_BD { get; set; }

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
            DinhMucHN = "";
            DinhMucHN_BD = "";
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
        /// Định mức hộ nghèo cũ
        /// </summary>
        public string DinhMucHN { get; set; }

        /// <summary>
        /// Định mức hộ nghèo mới
        /// </summary>
        public string DinhMucHN_BD { get; set; }

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
    }

    public class HoSoScan
    {
        /// <summary>
        /// Mã ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Loại văn bản
        /// </summary>
        public string LoaiVanBan { get; set; }

        /// <summary>
        /// Danh sách file
        /// </summary>
        public List<HoSoScan_File> lstFile { get; set; }

        public HoSoScan()
        {
            ID = 0;
            CreateDate = null;
            LoaiVanBan = "";
            lstFile = new List<HoSoScan_File>();
        }
    }

    public class HoSoScan_File
    {
        public byte[] File { get; set; }

        public HoSoScan_File()
        {
            File = null;
        }
    }

    #endregion

    #region Gắn Mới

    public class HoSoGanMoi
    {
        /// <summary>
        /// Số hồ sơ
        /// </summary>
        public string SoHoSo { get; set; }

        /// <summary>
        /// Loại hồ sơ
        /// </summary>
        public string LoaiHoSo { get; set; }

        /// <summary>
        /// Họ tên khách hàng
        /// </summary>
        public string HoTen { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string DiaChi { get; set; }

        /// <summary>
        /// Ngày nhận
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Tiến trình
        /// </summary>
        public List<GhiChu> lstGhiChu { get; set; }

        public HoSoGanMoi()
        {
            SoHoSo = "";
            LoaiHoSo = "";
            HoTen = "";
            DiaChi = "";
            CreateDate = null;
            lstGhiChu = new List<GhiChu>();
        }
    }

    #endregion

    #region Khác

    public class ThongTinExtra
    {
        /// <summary>
        /// Tiêu Đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Tổng số Cột
        /// </summary>
        public int totalColumn { get; set; }

        /// <summary>
        /// Danh sách Tên Cột
        /// </summary>
        public List<string> lstColumn { get; set; }

        /// <summary>
        /// Tổng số Dòng
        /// </summary>
        public int totalRow { get; set; }

        /// <summary>
        /// Danh sách nội dung Dòng
        /// </summary>
        public List<List<string>> lstRow { get; set; }

        public ThongTinExtra()
        {
            Title = "";
            totalColumn = totalRow = 0;
            lstColumn = new List<string>();
            lstRow = new List<List<string>>();
        }
    }

    #endregion

    #region CSKH TCT

    public class CSKH_TCT
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Loại đơn
        /// </summary>
        public string Loai { get; set; }

        /// <summary>
        /// Danh bộ
        /// </summary>
        public string DanhBo { get; set; }

        /// <summary>
        /// Điện thoại
        /// </summary>
        public string DienThoai { get; set; }

        /// <summary>
        /// Họ tên
        /// </summary>
        public string HoTen { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string TieuDe { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string NoiDung { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string GhiChu { get; set; }

        /// <summary>
        /// url hình ảnh đính kèm
        /// </summary>
        public string urlImage { get; set; }

        /// <summary>
        /// url file
        /// </summary>
        public string urlFile { get; set; }

        /// <summary>
        /// Năm
        /// </summary>
        public string Nam { get; set; }

        /// <summary>
        /// Kỳ
        /// </summary>
        public string Ky { get; set; }

        /// <summary>
        /// Chỉ số nước
        /// </summary>
        public int CSN { get; set; }

        public CSKH_TCT()
        {
            ID = -1;
            Loai = "";
            DanhBo = "";
            DienThoai = "";
            HoTen = "";
            CreateDate = null;
            TieuDe = "";
            NoiDung = "";
            GhiChu = "";
            urlImage = "";
            urlFile = "";
            Nam = "";
            Ky = "";
            CSN = -1;
        }
    }

    #endregion

}