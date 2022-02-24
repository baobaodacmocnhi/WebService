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
        public string getDS_DocSo(string Nam, string Ky, string Dot, string May)
        {
            return _cThuTien.getDS_DocSo_DHN(Nam, Ky, Dot, May);
        }

        [WebMethod]
        public string getDS_Code()
        {
            return _cThuTien.getDS_Code_DHN();
        }

        [WebMethod]
        public string ghiChiSo(string ID, string Code, string ChiSo, string HinhDHN, string MaNV)
        {
            return _cThuTien.ghiChiSo_DHN(ID, Code, ChiSo, HinhDHN, MaNV);
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


    }
}
