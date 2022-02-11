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
