using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Drawing;
using System.Net.Http;
using System.Data;

namespace WSSmartPhone
{
    public class CEContract
    {
        private string urlApi = "https://api-econtract.cskhtanhoa.com.vn:1443";
        CConnection _cDAL_TTKH = new CConnection(CGlobalVariable.TTKH);

        private string getAccess_tokenClient()
        {
            return _cDAL_TTKH.ExecuteQuery_ReturnOneValue("select access_token from Access_token where ID='econtractclient'").ToString();
        }

        private string getAccess_tokenUser()
        {
            return _cDAL_TTKH.ExecuteQuery_ReturnOneValue("select access_token from Access_token where ID='econtractclient'").ToString();
        }

        public bool getAccess_token(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    //client
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "/auth-service/oauth/token");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    var data = new
                    {
                        username = "admintanhoa",
                        password = "Abc@12345",
                        grant_type = "client_credentials",
                        client_id = "tanhoa.client@econtract.vnpt.vn",
                        client_secret = "C29XWd2bDhsz9jB9h8lq5WOPmw3kS2O0"
                    };
                    var json = CGlobalVariable.jsSerializer.Serialize(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    request.ContentLength = byteArray.Length;
                    //gắn data post
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        _cDAL_TTKH.ExecuteNonQuery("update Access_token set access_token='" + obj["access_token"] + "',expires_in='" + obj["expires_in"] + " seconds',CreateDate=getdate() where ID='econtractclient'");
                    }
                    else
                    {
                        strResponse = "Error: " + respuesta.StatusCode;
                    }
                    //user
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(urlApi + "/auth-service/oauth/token");
                    request2.Method = "POST";
                    request2.ContentType = "application/json";
                    var data2 = new
                    {
                        domain = "econtract.cskhtanhoa.com.vn",
                        username = "admintanhoa",
                        password = "Abc@12345",
                        grant_type = "password",
                        client_id = "tanhoa.client@econtract.vnpt.vn",
                        client_secret = "C29XWd2bDhsz9jB9h8lq5WOPmw3kS2O0"
                    };
                    var json2 = CGlobalVariable.jsSerializer.Serialize(data2);
                    Byte[] byteArray2 = Encoding.UTF8.GetBytes(json2);
                    request2.ContentLength = byteArray2.Length;
                    //gắn data post
                    Stream dataStream2 = request2.GetRequestStream();
                    dataStream2.Write(byteArray2, 0, byteArray2.Length);
                    dataStream2.Close();
                    HttpWebResponse respuesta2 = (HttpWebResponse)request2.GetResponse();
                    if (respuesta2.StatusCode == HttpStatusCode.Accepted || respuesta2.StatusCode == HttpStatusCode.OK || respuesta2.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta2.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        _cDAL_TTKH.ExecuteNonQuery("update Access_token set access_token='" + obj["access_token"] + "',expires_in='" + obj["expires_in"] + " seconds',CreateDate=getdate() where ID='econtractuser'");
                    }
                    else
                    {
                        strResponse = "Error: " + respuesta.StatusCode;
                    }
                }
                else
                    strResponse = "Sai checksum";
                return true;
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return false;
        }

        public byte[] renderEContract(string HopDong, string DanhBo, DateTime CreateDate, string HoTen, string CCCD, string NgayCap, string DCThuongTru, string DCHienNay
        , string DienThoai, string Fax, string Email, string TaiKhoan, string Bank, string MST, string CoDHN, string DCLapDat, string NgayHieuLuc, string checksum, out string strResponse)
        {
            strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "/template-service/api/templates/64d3625c22ab89b8111134ad/render");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    //request.ContentLength = 0;
                    request.Headers["Authorization"] = "Bearer " + getAccess_tokenUser();
                    var data = new Dictionary<string, string>();
                    data.Add("${soHopDong}", HopDong);
                    data.Add("${soDanhBo}", DanhBo);
                    data.Add("${ngay}", CreateDate.Day.ToString());
                    data.Add("${thang}", CreateDate.Month.ToString());
                    data.Add("${nam}", CreateDate.Year.ToString());
                    data.Add("${tenBenB}", HoTen);
                    data.Add("${soCCCD}", CCCD);
                    data.Add("${capNgay}", NgayCap);
                    data.Add("${noiThuongTru}", DCThuongTru);
                    data.Add("${choOHienNay}", DCHienNay);
                    data.Add("${soDienThoai}", DienThoai);
                    data.Add("${fax}", Fax);
                    data.Add("${email}", Email);
                    data.Add("${taiKhoanSo}", TaiKhoan);
                    data.Add("${taiNganHang}", Bank);
                    data.Add("${maSoThue}", MST);
                    data.Add("${coDongHoNuoc}", "     " + CoDHN);
                    data.Add("${dongHoNuocDatTai}", DCLapDat);
                    data.Add("${ngayThangNamThiHanh}", NgayHieuLuc);
                    var json = CGlobalVariable.jsSerializer.Serialize(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    request.ContentLength = byteArray.Length;
                    //gắn data post
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();

                    if (respuesta.StatusCode == HttpStatusCode.OK)
                    {
                        Stream read = respuesta.GetResponseStream();
                        MemoryStream memoryStream = new MemoryStream();
                        read.CopyTo(memoryStream);
                        byte[] bytes = memoryStream.ToArray();
                        File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\Images\EContract.pdf", memoryStream.ToArray());
                        //System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Images\EContract.pdf");
                        return bytes;
                    }
                    else
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        strResponse = "Lỗi api - " + obj["message"] + " - " + obj["error"][0];
                    }
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return null;
        }

        public bool createEContract(string HopDong, string DanhBo, DateTime CreateDate, string HoTen, string CCCD, string NgayCap, string DCThuongTru, string DCHienNay
        , string DienThoai, string Fax, string Email, string TaiKhoan, string Bank, string MST, string CoDHN, string DCLapDat, string NgayHieuLuc, bool GanMoi, bool CaNhan, string MaDon, string SHS, string checksum, out string strResponse)
        {
            strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    renderEContract(HopDong, DanhBo, CreateDate, HoTen, CCCD, NgayCap, DCThuongTru, DCHienNay, DienThoai, Fax, Email, TaiKhoan, Bank, MST, CoDHN, DCLapDat, NgayHieuLuc, checksum, out strResponse);

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, urlApi + "/esolution-service/contracts/create-draft-from-file-and-identification");
                    request.Headers.Add("Authorization", "Bearer " + getAccess_tokenClient());
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent("{}"), "fields");
                    string a = "";
                    if (CaNhan)
                    {
                        a = "{\"username\":\"" + CCCD + "\",\"cmnd\":\"" + CCCD + "\",\"email\":\"" + Email + "\",\"mst\":\"\",\"sdt\":\"" + DienThoai + "\",\"signFrame\":[{\"x\":160,\"y\":180,\"w\":80,\"h\":70,\"page\":4}],\"ten\":\"" + HoTen + "\",\"tenToChuc\":\"\",\"userType\":\"CONSUMER\"}";
                        content.Add(new StringContent("{\"username\":\"" + CCCD + "\",\"cmnd\":\"" + CCCD + "\",\"email\":\"" + Email + "\",\"mst\":\"\",\"sdt\":\"" + DienThoai + "\",\"signFrame\":[{\"x\":160,\"y\":180,\"w\":80,\"h\":70,\"page\":4}],\"ten\":\"" + HoTen + "\",\"tenToChuc\":\"\",\"userType\":\"CONSUMER\"}"), "customer");
                    }
                    else
                    {
                        a = "{\"username\":\"" + MST + "\",\"cmnd\":\"\",\"email\":\"" + Email + "\",\"mst\":\"" + MST + "\",\"sdt\":\"" + DienThoai + "\",\"signFrame\":[{\"x\":160,\"y\":180,\"w\":80,\"h\":70,\"page\":4}],\"ten\":\"\",\"tenToChuc\":\"" + HoTen + "\",\"userType\":\"BUSINESS\"}";
                        content.Add(new StringContent("{\"username\":\"" + MST + "\",\"cmnd\":\"\",\"email\":\"" + Email + "\",\"mst\":\"" + MST + "\",\"sdt\":\"" + DienThoai + "\",\"signFrame\":[{\"x\":160,\"y\":180,\"w\":80,\"h\":70,\"page\":4}],\"ten\":\"\",\"tenToChuc\":\"" + HoTen + "\",\"userType\":\"BUSINESS\"}"), "customer");
                    }
                    if (GanMoi)
                    {
                        if (CaNhan)
                        {
                            a = "{\"autoRenew\":\"true\",\"contractValue\":\"0\",\"creationNote\":\"Ghi chú hợp đồng\",\"endDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"flowTemplateId\":\"0fff387d-5646-4993-a5dd-6d96e2036537\",\"templateId\":\"64d3625c22ab89b8111134ad\",\"signFlowType\":\"REQUIRE_FLOW_STAMP\",\"sequence\":1,\"signFlow\":[{\"signType\":\"DRAFT\",\"departmentId\":\"f420f37a-97c7-421d-af1c-4a0a7dfa88d5\",\"userId\":\"c6ee1894-498e-40fb-848f-00e02ad41f06\",\"sequence\":1,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":470,\"y\":220,\"w\":70,\"h\":50,\"page\":4}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":360,\"y\":180,\"w\":145,\"h\":80,\"page\":4}]},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":330,\"y\":210,\"w\":120,\"h\":120,\"page\":4}]}],\"signForm\":[\"OTP\",\"OTP_EMAIL\"],\"title\":\"Hợp đồng gắn mới " + SHS + " " + DateTime.Now.ToString("dd/MM/yyyy") + "\",\"validDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"verificationType\":\"NONE\",\"fixedPosition\":true,\"callbackUrl\":\"\"}";
                            content.Add(new StringContent("{\"autoRenew\":\"true\",\"contractValue\":\"0\",\"creationNote\":\"Ghi chú hợp đồng\",\"endDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"flowTemplateId\":\"0fff387d-5646-4993-a5dd-6d96e2036537\",\"templateId\":\"64d3625c22ab89b8111134ad\",\"signFlowType\":\"REQUIRE_FLOW_STAMP\",\"sequence\":1,\"signFlow\":[{\"signType\":\"DRAFT\",\"departmentId\":\"f420f37a-97c7-421d-af1c-4a0a7dfa88d5\",\"userId\":\"c6ee1894-498e-40fb-848f-00e02ad41f06\",\"sequence\":1,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":470,\"y\":220,\"w\":70,\"h\":50,\"page\":4}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":360,\"y\":180,\"w\":145,\"h\":80,\"page\":4}]},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":330,\"y\":210,\"w\":120,\"h\":120,\"page\":4}]}],\"signForm\":[\"OTP\",\"OTP_EMAIL\"],\"title\":\"Hợp đồng gắn mới " + SHS + " " + DateTime.Now.ToString("dd/MM/yyyy") + "\",\"validDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"verificationType\":\"NONE\",\"fixedPosition\":true,\"callbackUrl\":\"\"}"), "contract");
                        }
                        else
                        {
                            a = "{\"autoRenew\":\"true\",\"contractValue\":\"0\",\"creationNote\":\"Ghi chú hợp đồng\",\"endDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"flowTemplateId\":\"0fff387d-5646-4993-a5dd-6d96e2036537\",\"templateId\":\"64d3625c22ab89b8111134ad\",\"signFlowType\":\"REQUIRE_FLOW_STAMP\",\"sequence\":1,\"signFlow\":[{\"signType\":\"DRAFT\",\"departmentId\":\"f420f37a-97c7-421d-af1c-4a0a7dfa88d5\",\"userId\":\"c6ee1894-498e-40fb-848f-00e02ad41f06\",\"sequence\":1,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":470,\"y\":220,\"w\":70,\"h\":50,\"page\":4}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":360,\"y\":180,\"w\":145,\"h\":80,\"page\":4}]},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":330,\"y\":210,\"w\":120,\"h\":120,\"page\":4}]}],\"signForm\":[\"OTP\",\"OTP_EMAIL\",\"USB_TOKEN\"],\"title\":\"Hợp đồng gắn mới " + SHS + " " + DateTime.Now.ToString("dd/MM/yyyy") + "\",\"validDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"verificationType\":\"NONE\",\"fixedPosition\":true,\"callbackUrl\":\"\"}";
                            content.Add(new StringContent("{\"autoRenew\":\"true\",\"contractValue\":\"0\",\"creationNote\":\"Ghi chú hợp đồng\",\"endDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"flowTemplateId\":\"0fff387d-5646-4993-a5dd-6d96e2036537\",\"templateId\":\"64d3625c22ab89b8111134ad\",\"signFlowType\":\"REQUIRE_FLOW_STAMP\",\"sequence\":1,\"signFlow\":[{\"signType\":\"DRAFT\",\"departmentId\":\"f420f37a-97c7-421d-af1c-4a0a7dfa88d5\",\"userId\":\"c6ee1894-498e-40fb-848f-00e02ad41f06\",\"sequence\":1,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":470,\"y\":220,\"w\":70,\"h\":50,\"page\":4}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":360,\"y\":180,\"w\":145,\"h\":80,\"page\":4}]},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":330,\"y\":210,\"w\":120,\"h\":120,\"page\":4}]}],\"signForm\":[\"OTP\",\"OTP_EMAIL\",\"USB_TOKEN\"],\"title\":\"Hợp đồng gắn mới " + SHS + " " + DateTime.Now.ToString("dd/MM/yyyy") + "\",\"validDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"verificationType\":\"NONE\",\"fixedPosition\":true,\"callbackUrl\":\"\"}"), "contract");
                        }
                    }
                    else
                    {
                        if (CaNhan)
                        {
                            a = "{\"autoRenew\":\"true\",\"contractValue\":\"0\",\"creationNote\":\"Ghi chú hợp đồng\",\"endDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"flowTemplateId\":\"a7e6494a-f7bc-4306-a3b6-d0842e9c69f3\",\"templateId\":\"64d3625c22ab89b8111134ad\",\"signFlowType\":\"REQUIRE_FLOW_STAMP\",\"sequence\":1,\"signFlow\":[{\"signType\":\"DRAFT\",\"departmentId\":\"88bedc40-ccb5-4221-9e7f-c9c5aad49d67\",\"userId\":\"3440ba96-3683-49c3-9324-9e2d670d3481\",\"sequence\":1,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":470,\"y\":220,\"w\":70,\"h\":50,\"page\":4}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":360,\"y\":180,\"w\":145,\"h\":80,\"page\":4}]},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":330,\"y\":210,\"w\":120,\"h\":120,\"page\":4}]}],\"signForm\":[\"OTP\",\"OTP_EMAIL\"],\"title\":\"Hợp đồng sang tên " + DanhBo + " " + DateTime.Now.ToString("dd/MM/yyyy") + "\",\"validDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"verificationType\":\"NONE\",\"fixedPosition\":true,\"callbackUrl\":\"\"}";
                            content.Add(new StringContent("{\"autoRenew\":\"true\",\"contractValue\":\"0\",\"creationNote\":\"Ghi chú hợp đồng\",\"endDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"flowTemplateId\":\"a7e6494a-f7bc-4306-a3b6-d0842e9c69f3\",\"templateId\":\"64d3625c22ab89b8111134ad\",\"signFlowType\":\"REQUIRE_FLOW_STAMP\",\"sequence\":1,\"signFlow\":[{\"signType\":\"DRAFT\",\"departmentId\":\"88bedc40-ccb5-4221-9e7f-c9c5aad49d67\",\"userId\":\"3440ba96-3683-49c3-9324-9e2d670d3481\",\"sequence\":1,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":470,\"y\":220,\"w\":70,\"h\":50,\"page\":4}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":360,\"y\":180,\"w\":145,\"h\":80,\"page\":4}]},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":330,\"y\":210,\"w\":120,\"h\":120,\"page\":4}]}],\"signForm\":[\"OTP\",\"OTP_EMAIL\"],\"title\":\"Hợp đồng sang tên " + DanhBo + " " + DateTime.Now.ToString("dd/MM/yyyy") + "\",\"validDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"verificationType\":\"NONE\",\"fixedPosition\":true,\"callbackUrl\":\"\"}"), "contract");
                        }
                        else
                        {
                            a = "{\"autoRenew\":\"true\",\"contractValue\":\"0\",\"creationNote\":\"Ghi chú hợp đồng\",\"endDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"flowTemplateId\":\"a7e6494a-f7bc-4306-a3b6-d0842e9c69f3\",\"templateId\":\"64d3625c22ab89b8111134ad\",\"signFlowType\":\"REQUIRE_FLOW_STAMP\",\"sequence\":1,\"signFlow\":[{\"signType\":\"DRAFT\",\"departmentId\":\"88bedc40-ccb5-4221-9e7f-c9c5aad49d67\",\"userId\":\"3440ba96-3683-49c3-9324-9e2d670d3481\",\"sequence\":1,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":470,\"y\":220,\"w\":70,\"h\":50,\"page\":4}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":360,\"y\":180,\"w\":145,\"h\":80,\"page\":4}]},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":330,\"y\":210,\"w\":120,\"h\":120,\"page\":4}]}],\"signForm\":[\"OTP\",\"OTP_EMAIL\",\"USB_TOKEN\"],\"title\":\"Hợp đồng sang tên " + DanhBo + " " + DateTime.Now.ToString("dd/MM/yyyy") + "\",\"validDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"verificationType\":\"NONE\",\"fixedPosition\":true,\"callbackUrl\":\"\"}";
                            content.Add(new StringContent("{\"autoRenew\":\"true\",\"contractValue\":\"0\",\"creationNote\":\"Ghi chú hợp đồng\",\"endDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"flowTemplateId\":\"a7e6494a-f7bc-4306-a3b6-d0842e9c69f3\",\"templateId\":\"64d3625c22ab89b8111134ad\",\"signFlowType\":\"REQUIRE_FLOW_STAMP\",\"sequence\":1,\"signFlow\":[{\"signType\":\"DRAFT\",\"departmentId\":\"88bedc40-ccb5-4221-9e7f-c9c5aad49d67\",\"userId\":\"3440ba96-3683-49c3-9324-9e2d670d3481\",\"sequence\":1,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":470,\"y\":220,\"w\":70,\"h\":50,\"page\":4}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":360,\"y\":180,\"w\":145,\"h\":80,\"page\":4}]},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":99,\"signForm\":[\"SMART_CA\"],\"signFrame\":[{\"x\":330,\"y\":210,\"w\":120,\"h\":120,\"page\":4}]}],\"signForm\":[\"OTP\",\"OTP_EMAIL\",\"USB_TOKEN\"],\"title\":\"Hợp đồng sang tên " + DanhBo + " " + DateTime.Now.ToString("dd/MM/yyyy") + "\",\"validDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\",\"verificationType\":\"NONE\",\"fixedPosition\":true,\"callbackUrl\":\"\"}"), "contract");
                        }
                    }
                    content.Add(new StreamContent(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"\Images\EContract.pdf")), "attachFile", AppDomain.CurrentDomain.BaseDirectory + @"\Images\EContract.pdf");
                    content.Add(new StreamContent(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"\Images\face.png")), "EKYC_CHANDUNG", AppDomain.CurrentDomain.BaseDirectory + @"\Images\face.png");
                    content.Add(new StreamContent(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"\Images\cccd1.png")), "EKYC_MATTRUOC", AppDomain.CurrentDomain.BaseDirectory + @"\Images\cccd1.png");
                    content.Add(new StreamContent(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"\Images\cccd2.png")), "EKYC_MATSAU", AppDomain.CurrentDomain.BaseDirectory + @"\Images\cccd2.png");
                    request.Content = content;
                    var response = client.SendAsync(request);
                    var result = response.Result.Content.ReadAsStringAsync();
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result.Result.ToString());
                    if (response.Result.IsSuccessStatusCode)
                    {
                        string username = "";
                        if (CaNhan)
                            username = CCCD;
                        else
                            username = MST;
                        _cDAL_TTKH.ExecuteNonQuery("insert into Zalo_EContract_ChiTiet(DienThoai,IDEContract,DanhBo,MaDon,SHS)values('" + DienThoai + "','" + obj["object"]["contractId"] + "','" + DanhBo + "','" + MaDon + "','" + SHS + "')"
                            + " if not exists (select * from Zalo_EContract_User where Username='" + username + "' and UserID='" + obj["object"]["partnerId"] + "') insert into Zalo_EContract_User(Username,UserID)values('" + username + "','" + obj["object"]["partnerId"] + "')");

                        return true;
                    }
                    else
                        strResponse = "Lỗi api - " + obj["message"] + " - " + obj["error"][0];
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return false;
        }

        public bool sendEContract(string MaDon, string SHS, string checksum, out string strResponse)
        {
            strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    DataTable dt;
                    if (MaDon != "")
                        dt = _cDAL_TTKH.ExecuteQuery_DataTable("select top 1 IDEContract from Zalo_EContract_ChiTiet where MaDon='" + MaDon + "' and Huy=0 order by CreateDate desc");
                    else
                        dt = _cDAL_TTKH.ExecuteQuery_DataTable("select top 1 IDEContract from Zalo_EContract_ChiTiet where SHS='" + SHS + "' and Huy=0 order by CreateDate desc");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, urlApi + "/esolution-service/contracts/" + dt.Rows[0]["IDEContract"].ToString() + "/submit-contract");
                        request.Headers.Add("Authorization", "Bearer " + getAccess_tokenUser());
                        var response = client.SendAsync(request);
                        var result = response.Result.Content.ReadAsStringAsync();
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result.Result.ToString());
                        if (response.Result.IsSuccessStatusCode)
                        {
                            return true;
                        }
                        else
                            strResponse = "Lỗi api - " + obj["message"] + " - " + obj["error"][0];
                    }
                    else
                        strResponse = "Không có dữ liệu từ mã đơn";
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return false;
        }

        public bool editEContract(string MaDon, string SHS, string checksum, out string strResponse)
        {
            strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    DataTable dt;
                    string DanhBo = "", HopDong = "", Co = "", NgayHieuLuc = "";
                    if (MaDon != "")
                    {
                        dt = _cDAL_TTKH.ExecuteQuery_DataTable("select top 1 IDEContract from Zalo_EContract_ChiTiet where MaDon='" + MaDon + "' and Huy=0 and HieuLuc=0 order by CreateDate desc");
                        //DataTable dtThongTin = _cDAL_TTKH.ExecuteQuery_DataTable("select CreateDate='08/09/2023'");
                        DataTable dtThongTin = _cDAL_TTKH.ExecuteQuery_DataTable("select CreateDate=CONVERT(varchar(10),b.CreateDate,103) from KTKS_DonKH.dbo.DCBD a,KTKS_DonKH.dbo.DCBD_ChiTietBienDong b"
                        + " where a.MaDCBD=b.MaDCBD and a.MaDonMoi=" + MaDon + " and b.ThongTin like N'%tên%'");
                        if (dtThongTin == null || dtThongTin.Rows.Count == 0)
                        {
                            strResponse = "Mã đơn chưa có điều chỉnh";
                            return false;
                        }
                        NgayHieuLuc = dtThongTin.Rows[0]["CreateDate"].ToString();
                    }
                    else
                    {
                        dt = _cDAL_TTKH.ExecuteQuery_DataTable("select top 1 IDEContract from Zalo_EContract_ChiTiet where SHS='" + SHS + "' and Huy=0 and HieuLuc=0 order by CreateDate desc");
                        //DataTable dtThongTin = _cDAL_TTKH.ExecuteQuery_DataTable("SELECT COTLK='15',DHN_SODANHBO='13130000000',DHN_SOHOPDONG='TP123456',DHN_NGAYCHOSODB='08/09/2023'");
                        DataTable dtThongTin = _cDAL_TTKH.ExecuteQuery_DataTable("SELECT COTLK,DHN_SODANHBO,DHN_SOHOPDONG,DHN_NGAYCHOSODB=CONVERT(varchar(10),DHN_NGAYCHOSODB,103) FROM TANHOA_WATER.dbo.KH_HOSOKHACHHANG where shs='" + SHS + "' and DHN_SODANHBO is not null");
                        if (dtThongTin == null || dtThongTin.Rows.Count == 0)
                        {
                            strResponse = "Mã đơn chưa có cho danh bộ";
                            return false;
                        }
                        DanhBo = dtThongTin.Rows[0]["DHN_SODANHBO"].ToString();
                        HopDong = dtThongTin.Rows[0]["DHN_SOHOPDONG"].ToString();
                        Co = dtThongTin.Rows[0]["COTLK"].ToString();
                        NgayHieuLuc = dtThongTin.Rows[0]["DHN_NGAYCHOSODB"].ToString();
                    }
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (checkHieuLucEContract(dt.Rows[0]["IDEContract"].ToString()))
                        {
                            strResponse = "EContract đã có hiệu lực";
                            return false;
                        }
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        var client = new HttpClient();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        var request = new HttpRequestMessage(HttpMethod.Post, urlApi + "/esignature-service/dsign/attach-tawaco-information");
                        request.Headers.Add("Authorization", "Bearer " + getAccess_tokenUser());
                        var content = new StringContent("{\"contractId\":\"" + dt.Rows[0]["IDEContract"].ToString() + "\",\"soDanhBa\":{\"value\":\"" + DanhBo + "\",\"signFrame\":[{\"pageSign\":0,\"bboxSign\":[290,815,320,230]}]},\"soHoSo\":{\"value\":\"" + HopDong + "\",\"signFrame\":[{\"pageSign\":0,\"bboxSign\":[290,840,320,230]}]},\"ngayKy\":{\"value\":\"" + NgayHieuLuc + "\",\"signFrame\":[{\"pageSign\":3,\"bboxSign\":[312,400,300,250]}]},\"coDongHo\":{\"value\":\"" + Co + "\",\"signFrame\":[{\"pageSign\":1,\"bboxSign\":[190,890,300,250]}]},\"QRcode\":{\"value\":\"\",\"signFrame\":[{\"pageSign\":0,\"bboxSign\":[0,0,0,0]}]}}", null, "application/json");
                        request.Content = content;
                        var response = client.SendAsync(request);
                        var result = response.Result.Content.ReadAsStringAsync();
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result.Result.ToString());
                        if (response.Result.IsSuccessStatusCode)
                        {
                            _cDAL_TTKH.ExecuteNonQuery("update Zalo_EContract_ChiTiet set HieuLuc=1 where IDEContract='" + dt.Rows[0]["IDEContract"].ToString() + "'");
                            return true;
                        }
                        else
                            strResponse = "Lỗi api - " + obj["message"] + " - " + obj["error"][0];
                    }
                    else
                        strResponse = "Không có dữ liệu từ mã đơn";
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return false;
        }

        public bool cancelEContract(string MaDon, string SHS, string checksum, out string strResponse)
        {
            strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    DataTable dt;
                    if (MaDon != "")
                    {
                        dt = _cDAL_TTKH.ExecuteQuery_DataTable("select top 1 IDEContract from Zalo_EContract_ChiTiet where MaDon='" + MaDon + "' and Huy=0 order by CreateDate desc");
                    }
                    else
                    {
                        dt = _cDAL_TTKH.ExecuteQuery_DataTable("select top 1 IDEContract from Zalo_EContract_ChiTiet where SHS='" + SHS + "' and Huy=0 order by CreateDate desc");
                    }
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (checkHieuLucEContract(dt.Rows[0]["IDEContract"].ToString()))
                        {
                            strResponse = "EContract đã có hiệu lực";
                            return false;
                        }
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, urlApi + "/esolution-service/contracts/" + dt.Rows[0]["IDEContract"].ToString() + "/cancel-draft");
                        request.Headers.Add("Authorization", "Bearer " + getAccess_tokenUser());
                        var content = new StringContent("{\"cancelReason\":\"api hủy\"}", null, "application/json");
                        request.Content = content;
                        var response = client.SendAsync(request);
                        var result = response.Result.Content.ReadAsStringAsync();
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result.Result.ToString());
                        if (response.Result.IsSuccessStatusCode)
                        {
                            _cDAL_TTKH.ExecuteNonQuery("update Zalo_EContract_ChiTiet set Huy=1 where IDEContract='" + dt.Rows[0]["IDEContract"].ToString() + "'");
                            return true;
                        }
                        else
                            strResponse = "Lỗi api - " + obj["message"] + " - " + obj["error"][0];
                    }
                    else
                        strResponse = "Không có dữ liệu từ mã đơn";
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return false;
        }

        public bool deleteEContract(string MaDon, string SHS, string checksum, out string strResponse)
        {
            strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    DataTable dt;
                    if (MaDon != "")
                    {
                        dt = _cDAL_TTKH.ExecuteQuery_DataTable("select top 1 IDEContract from Zalo_EContract_ChiTiet where MaDon='" + MaDon + "' and Huy=1 order by CreateDate desc");
                    }
                    else
                    {
                        dt = _cDAL_TTKH.ExecuteQuery_DataTable("select top 1 IDEContract from Zalo_EContract_ChiTiet where SHS='" + SHS + "' and Huy=1 order by CreateDate desc");
                    }
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (checkHieuLucEContract(dt.Rows[0]["IDEContract"].ToString()))
                        {
                            strResponse = "EContract đã có hiệu lực";
                            return false;
                        }
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, urlApi + "/esolution-service/contracts/" + dt.Rows[0]["IDEContract"].ToString() + "/delete-draft");
                        request.Headers.Add("Authorization", "Bearer " + getAccess_tokenUser());
                        var response = client.SendAsync(request);
                        var result = response.Result.Content.ReadAsStringAsync();
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result.Result.ToString());
                        if (response.Result.IsSuccessStatusCode)
                        {
                            _cDAL_TTKH.ExecuteNonQuery("delete Zalo_EContract_ChiTiet where IDEContract='" + dt.Rows[0]["IDEContract"].ToString() + "'");
                            return true;
                        }
                        else
                            strResponse = "Lỗi api - " + obj["message"] + " - " + obj["error"][0];
                    }
                    else
                        strResponse = "Không có dữ liệu từ mã đơn";
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return false;
        }

        public bool updateDoiTac(string CCCD, string Email, string DienThoai, string HoTen, string MST, string checksum, out string strResponse)
        {
            strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    var client = new HttpClient();
                    var request = new HttpRequestMessage();
                    request.Headers.Add("Authorization", "Bearer " + getAccess_tokenUser());
                    var content = new MultipartFormDataContent();
                    if (MST == "")
                    {
                        string UserID = _cDAL_TTKH.ExecuteQuery_ReturnOneValue("select UserID from Zalo_EContract_User where Username='" + CCCD + "'").ToString();
                        request = new HttpRequestMessage(HttpMethod.Put, urlApi + "esolution-service/trusted-partners/consumer/" + UserID);
                        content.Add(new StringContent(CCCD), "username");
                        content.Add(new StringContent(DienThoai), "sdt");
                        content.Add(new StringContent(Email), "email");
                        content.Add(new StringContent("{\"ten\":\"" + HoTen + "\",\"cmnd\":\"" + CCCD + "\",\"ngaySinh\":\"\",\"noiSinh\":\"\",\"gioiTinhId\":\"\",\"ngayCap\":\"\",\"noiCap\":\"\",\"dkhktt\":\"\",\"expiryDate\":\"\"}"), "info");
                    }
                    else
                    {
                        string UserID = _cDAL_TTKH.ExecuteQuery_ReturnOneValue("select UserID from Zalo_EContract_User where Username='" + MST + "'").ToString();
                        request = new HttpRequestMessage(HttpMethod.Put, urlApi + "esolution-service/trusted-partners/business/" + UserID);
                        content.Add(new StringContent("{\"username\":\"" + MST + "\",\"ten\":\"\",\"sdt\":\"" + DienThoai + "\",\"ngaySinh\":null,\"gioiTinhId\":null}"), "daiDien");
                        content.Add(new StringContent("{\"maSoThue\":\"" + MST + "\",\"tenToChuc\":\"" + HoTen + "\",\"tenRutGon\":\"\",\"email\":\"" + Email + "\",\"tinhId\":null,\"huyenId\":null,\"xaId\":null,\"duong\":\"\",\"soNha\":\"\"}"), "toChuc");
                    }
                    request.Content = content;
                    var response = client.SendAsync(request);
                    var result = response.Result.Content.ReadAsStringAsync();
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result.Result.ToString());
                    if (response.Result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                        strResponse = "Lỗi api - " + obj["message"] + " - " + obj["error"][0];
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return false;
        }

        private bool checkHieuLucEContract(string IDEContract)
        {
            if (_cDAL_TTKH.ExecuteQuery_ReturnOneValue("select HieuLuc from Zalo_EContract_ChiTiet where IDEContract='" + IDEContract + "'").ToString() == "1")
                return true;
            else
                return false;
        }

        public byte[] ImageToByte(Bitmap image)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }

        public byte[] scanImage(string path)
        {
            Image image = Image.FromFile(path);
            return ImageToByte((Bitmap)image);
        }

        public byte[] scanFile(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}