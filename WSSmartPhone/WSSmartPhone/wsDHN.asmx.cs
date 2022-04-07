using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net;
using System.Xml;
using System.IO;

namespace WSSmartPhone
{
    /// <summary>
    /// Summary description for wsDHN
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsDHN : System.Web.Services.WebService
    {
        CThuTien _cThuTien = new CThuTien();

        [WebMethod]
        public string GetVersion()
        {
            return _cThuTien.GetVersion_DHN();
        }

        [WebMethod]
        public bool UpdateUID(string MaNV, string UID)
        {
            return _cThuTien.UpdateUID_DHN(MaNV, UID);
        }

        [WebMethod]
        public string DangNhaps(string Username, string Password, string IDMobile, string UID)
        {
            return _cThuTien.DangNhaps_DHN(Username, Password, IDMobile, UID);
        }

        [WebMethod]
        public string DangXuats(string Username, string UID)
        {
            return _cThuTien.DangXuats_DHN(Username, UID);
        }

        [WebMethod]
        public string DangXuats_Person(string Username, string UID)
        {
            return _cThuTien.DangXuats_Person_DHN(Username, UID);
        }

        [WebMethod]
        public string getDS_Nam()
        {
            return _cThuTien.getDS_Nam_DHN();
        }

        [WebMethod]
        public string GetDSTo()
        {
            return _cThuTien.getDS_To_DHN();
        }

        [WebMethod]
        public string getDS_NhanVien_HanhThu()
        {
            return _cThuTien.getDS_NhanVien_HanhThu_DHN();
        }

        [WebMethod]
        public string GetDSNhanVienTo(string MaTo)
        {
            return _cThuTien.getDS_NhanVien_DHN(MaTo);
        }

        [WebMethod]
        public string getDS_NhanVien()
        {
            return _cThuTien.getDS_NhanVien_DHN();
        }

        [WebMethod]
        public string getDS_Code()
        {
            return _cThuTien.getDS_Code_DHN();
        }

        [WebMethod]
        public string getDS_ViTriDHN()
        {
            return _cThuTien.getDS_ViTriDHN();
        }

        [WebMethod]
        public string getDS_GiaNuoc()
        {
            return _cThuTien.getDS_GiaNuoc_DHN();
        }

        [WebMethod]
        public string getDS_KhongTinhPBVMT()
        {
            return _cThuTien.getDS_KhongTinhPBVMT_DHN();
        }

        [WebMethod]
        public bool checkNgayDoc(string Nam, string Ky, string Dot, string May)
        {
            return _cThuTien.checkNgayDoc_DHN(Nam, Ky, Dot, May);
        }

        [WebMethod]
        public string getDS_DocSo(string Nam, string Ky, string Dot, string May)
        {
            return _cThuTien.getDS_DocSo_DHN(Nam, Ky, Dot, May);
        }

        [WebMethod]
        public string getDS_DocSo_Ton(string Nam, string Ky, string Dot, string May)
        {
            return _cThuTien.getDS_DocSo_Ton_DHN(Nam, Ky, Dot, May);
        }

        [WebMethod]
        public string getDS_Hinh_Ton(string Nam, string Ky, string Dot, string May)
        {
            return _cThuTien.getDS_Hinh_Ton_DHN(Nam, Ky, Dot, May);
        }

        [WebMethod]
        public string getDS_HoaDonTon(string Nam, string Ky, string Dot, string May)
        {
            return _cThuTien.getDS_HoaDonTon_DHN(Nam, Ky, Dot, May);
        }


        //ghi chú
        [WebMethod]
        public string get_GhiChu(string DanhBo)
        {
            return _cThuTien.get_GhiChu_DHN(DanhBo);
        }

        [WebMethod]
        public string update_GhiChu(string DanhBo, string SoNha, string TenDuong, string ViTri1, string ViTri2, string Gieng, string GhiChu, string MaNV)
        {
            return _cThuTien.update_GhiChu_DHN(DanhBo, SoNha, TenDuong, ViTri1, ViTri2, Gieng, GhiChu, MaNV);
        }

        [WebMethod]
        public string getDS_DienThoai(string DanhBo)
        {
            return _cThuTien.getDS_DienThoai_DHN(DanhBo);
        }

        [WebMethod]
        public string update_DienThoai(string DanhBo, string DienThoai, string HoTen, string SoChinh, string MaNV)
        {
            return _cThuTien.update_DienThoai_DHN(DanhBo, DienThoai, HoTen, SoChinh, MaNV);
        }

        [WebMethod]
        public string delete_DienThoai(string DanhBo, string DienThoai)
        {
            return _cThuTien.delete_DienThoai_DHN(DanhBo, DienThoai);
        }

        [WebMethod]
        public byte[] get_Hinh(string ID)
        {
            return _cThuTien.get_Hinh_DHN(ID);
        }

        [WebMethod]
        public bool ghi_Hinh(string ID, string HinhDHN)
        {
            return _cThuTien.ghi_Hinh_DHN(ID, HinhDHN);
        }

        [WebMethod]
        public bool xoa_Hinh(string ID)
        {
            return _cThuTien.xoa_Hinh_DHN(ID);
        }

        [WebMethod]
        public bool checkExists_Hinh(string ID)
        {
            return _cThuTien.checkExists_Hinh_DHN(ID);
        }

        //đọc số
        [WebMethod]
        public string ghiChiSo(string ID, string Code, string ChiSo, string HinhDHN, string Dot, string MaNV, string TBTT)
        {
            return _cThuTien.ghi_ChiSo_DHN(ID, Code, ChiSo, HinhDHN, Dot, MaNV, TBTT);
        }

        [WebMethod]
        public string ghiChiSo_GianTiep(string ID, string Code, string ChiSo, string TieuThu, string TienNuoc, string ThueGTGT, string PhiBVMT, string PhiBVMT_Thue, string TongCong, string HinhDHN, string Dot, string MaNV, string NgayDS)
        {
            return _cThuTien.ghi_ChiSo_DHN(ID, Code, ChiSo, TieuThu, TienNuoc, ThueGTGT, PhiBVMT, PhiBVMT_Thue, TongCong, HinhDHN, Dot, MaNV, NgayDS);
        }

        [WebMethod]
        public bool insertBilling(string DocSoID, string checksum, out string message)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"http://192.168.90.6:82/wsbilling.asmx?op=insertBilling");
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <soap:Body>
                   <insertBilling xmlns=""http://tempuri.org/"">"
                + "   <DocSoID>" + DocSoID + "</DocSoID>"
                + "      <checksum>" + checksum + "</checksum>"
                + "    </insertBilling>"
                + "  </soap:Body>"
                + " </soap:Envelope>");

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    bool result;
                    Boolean.TryParse(soapResult, out result);
                    message = soapResult;
                    if (result == true)
                        return true;
                    else
                        return false;
                }
            }
        }

        [WebMethod]
        public bool tinhCodeTieuThu_TieuThu(string DocSoID, string Code, int TieuThu, out int GiaBan, out int ThueGTGT, out int PhiBVMT, out int TongCong)
        {
            return _cThuTien.tinhCodeTieuThu(DocSoID, Code, TieuThu, out GiaBan, out ThueGTGT, out PhiBVMT, out TongCong);
        }

        [WebMethod]
        public bool tinhCodeTieuThu_CSM(string DocSoID, string Code, int CSM, out int TieuThu, out int GiaBan, out int ThueGTGT, out int PhiBVMT, out int TongCong)
        {
            return _cThuTien.tinhCodeTieuThu(DocSoID, Code, CSM, out TieuThu, out GiaBan, out ThueGTGT, out PhiBVMT, out TongCong);
        }

        //send notification
        [WebMethod]
        public string SendNotificationToClient(string Title, string Content, string UID, string Action, string NameUpdate, string ValueUpdate, string ID)
        {
            return _cThuTien.SendNotificationToClient_DHN(Title, Content, UID, Action, NameUpdate, ValueUpdate, ID);
        }

        [WebMethod]
        public byte[] get_Hinh_MaHoa(string FolderLoai, string FolderIDCT, string FileName)
        {
            return _cThuTien.get_Hinh_241(CGlobalVariable.pathHinhDHNMaHoa, FolderLoai, FolderIDCT, FileName);
        }

        [WebMethod]
        public bool ghi_Hinh_MaHoa(string FolderLoai, string FolderIDCT, string FileName, byte[] HinhDHN)
        {
            return _cThuTien.ghi_Hinh_241(CGlobalVariable.pathHinhDHNMaHoa, FolderLoai, FolderIDCT, FileName, HinhDHN);
        }

        [WebMethod]
        public bool xoa_Hinh_MaHoa(string FolderLoai, string FolderIDCT, string FileName)
        {
            return _cThuTien.xoa_Hinh_241(CGlobalVariable.pathHinhDHNMaHoa, FolderLoai, FolderIDCT, FileName);
        }

        [WebMethod]
        public bool xoa_Folder_Hinh_MaHoa(string FolderLoai, string FolderIDCT)
        {
            return _cThuTien.xoa_Folder_241(CGlobalVariable.pathHinhDHNMaHoa, FolderLoai, FolderIDCT);
        }

        //phiếu chuyển
        [WebMethod]
        public string getDS_PhieuChuyen()
        {
            return _cThuTien.getDS_PhieuChuyen_DHN();
        }

        [WebMethod]
        public string ghi_DonTu(string DanhBo, string NoiDung, string GhiChu,string Hinh, string MaNV)
        {
            return _cThuTien.ghi_DonTu_DHN(DanhBo, NoiDung, GhiChu,Hinh, MaNV);
        }

        [WebMethod]
        public string ghi_Hinh_DonTu(string ID, string Hinh, string MaNV)
        {
            return _cThuTien.ghi_Hinh_DonTu_DHN(ID, Hinh, MaNV).ToString();
        }

        [WebMethod]
        public string getDS_DonTu(string DanhBo)
        {
            return _cThuTien.getDS_DonTu_DHN(DanhBo);
        }

    }
}
