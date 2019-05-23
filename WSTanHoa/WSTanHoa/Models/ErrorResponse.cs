using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSTanHoa.Models
{
    public class ErrorResponse
    {
        public string message { get; set; }
        public int code { get; set; }
        public ErrorResponse(string message, int code)
        {
            this.message = message;
            this.code = code;
        }

        public override string ToString()
        {
                return code.ToString() + " : " + message;
        }

        const int errorCodeSQL = -1;
        const int errorCodeKhongDung = 1000; const string errorKhongDung = "Danh Bộ không đúng";
        const int errorCodeHetNo = 1001; const string errorHetNo = "Khách Hàng hết nợ";
        const int errorCodeIDGiaoDichKhongTonTai = 1002; const string errorIDGiaoDichKhongTonTai = "IDGiaoDich không tồn tại";
        const int errorCodeIDGiaoDichTonTai = 1003; const string errorIDGiaoDichTonTai = "IDGiaoDich này đã tồn tại";
        const int errorCodeGiaiTrach = 1004; const string errorGiaiTrach = "Hóa Đơn đã Giải Trách";
        const int errorCodePhiMoNuoc = 1005; const string errorPhiMoNuoc = "Hóa Đơn có Phí Mở Nước";
        const int errorCodeMaHD = 1006; const string errorMaHD = "Mã Hóa Đơn thiếu";
        const int errorCodeHoaDon = 1007; const string errorHoaDon = "Phải thanh toán hết Hóa Đơn Tồn";
        const int errorCodeSoTien = 1008; const string errorSoTien = "Số Tiền không đúng";
        const int errorCodeTBDongNuoc = 1009; const string errorTBDongNuoc = "Hóa Đơn có Thông Báo Đóng Nước";
        const int errorCodePassword = 1010; const string errorPassword = "Sai Mã kiểm tra";

        const int successCode = 1; const string success = "Thành Công";
        const int failCode = 0; const string fail = "Thất Bại";

        public static int ErrorCodeSQL
        {
            get
            {
                return errorCodeSQL;
            }
        }

        public static int ErrorCodeKhongDung
        {
            get
            {
                return errorCodeKhongDung;
            }
        }

        public static string ErrorKhongDung
        {
            get
            {
                return errorKhongDung;
            }
        }

        public static int ErrorCodeHetNo
        {
            get
            {
                return errorCodeHetNo;
            }
        }

        public static string ErrorHetNo
        {
            get
            {
                return errorHetNo;
            }
        }

        public static int ErrorCodeIDGiaoDichKhongTonTai
        {
            get
            {
                return errorCodeIDGiaoDichKhongTonTai;
            }
        }

        public static string ErrorIDGiaoDichKhongTonTai
        {
            get
            {
                return errorIDGiaoDichKhongTonTai;
            }
        }

        public static int ErrorCodeIDGiaoDichTonTai
        {
            get
            {
                return errorCodeIDGiaoDichTonTai;
            }
        }

        public static string ErrorIDGiaoDichTonTai
        {
            get
            {
                return errorIDGiaoDichTonTai;
            }
        }

        public static int ErrorCodeGiaiTrach
        {
            get
            {
                return errorCodeGiaiTrach;
            }
        }

        public static string ErrorGiaiTrach
        {
            get
            {
                return errorGiaiTrach;
            }
        }

        public static int ErrorCodePhiMoNuoc
        {
            get
            {
                return errorCodePhiMoNuoc;
            }
        }

        public static string ErrorPhiMoNuoc
        {
            get
            {
                return errorPhiMoNuoc;
            }
        }

        public static int ErrorCodeMaHD
        {
            get
            {
                return errorCodeMaHD;
            }
        }

        public static string ErrorMaHD
        {
            get
            {
                return errorMaHD;
            }
        }

        public static int ErrorCodeHoaDon
        {
            get
            {
                return errorCodeHoaDon;
            }
        }

        public static string ErrorHoaDon
        {
            get
            {
                return errorHoaDon;
            }
        }

        public static int ErrorCodeSoTien
        {
            get
            {
                return errorCodeSoTien;
            }
        }

        public static string ErrorSoTien
        {
            get
            {
                return errorSoTien;
            }
        }

        public static int ErrorCodePassword
        {
            get
            {
                return errorCodePassword;
            }
        }

        public static string ErrorPassword
        {
            get
            {
                return errorPassword;
            }
        }
    }

    public class ErrorResponseDetail
    {
        public string message { get; set; }
        public int code { get; set; }
        public string DanhBo { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public ErrorResponseDetail(string message, int code, string DanhBo, string HoTen, string DiaChi)
        {
            this.message = message;
            this.code = code;
            this.DanhBo = DanhBo;
            this.HoTen = HoTen;
            this.DiaChi = DiaChi;
        }
        public override string ToString()
        {
            return code.ToString() + " : " + message + " : " + DanhBo + " : " + HoTen + " : " + DiaChi;
        }
    }
}