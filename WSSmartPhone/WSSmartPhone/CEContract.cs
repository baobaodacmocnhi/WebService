﻿using System;
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
        private string urlApi = "https://api-econtract.cskhtanhoa.com.vn:1443/";
        CConnection _cDAL_TTKH = new CConnection(CGlobalVariable.TTKHWFH);

        public string getAccess_token(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "auth-service/oauth/token");
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
                        bool flag = _cDAL_TTKH.ExecuteNonQuery("update Access_token set access_token='" + obj["access_token"] + "',CreateDate=getdate() where ID='econtract'");
                        strResponse = flag.ToString();
                    }
                    else
                    {
                        strResponse = "Error: " + respuesta.StatusCode;
                    }
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return strResponse;
        }

        public string getAccess_token()
        {
            return _cDAL_TTKH.ExecuteQuery_ReturnOneValue("select access_token from Access_token where ID='econtract'").ToString();
        }

        public string renderEContract(string HopDong, string DanhBo, DateTime CreateDate, string HoTen, string CCCD, string NgayCap, string DCThuongTru, string DCHienNay
            , string DienThoai, string Fax, string Email, string TaiKhoan, string Bank, string MST, string CoDHN, string DCLapDat, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "template-service/api/templates/64d3625c22ab89b8111134ad/render");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    //request.ContentLength = 0;
                    request.Headers["Authorization"] = "Bearer " + getAccess_token();
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
                    data.Add("${coDongHoNuoc}", CoDHN);
                    data.Add("${dongHoNuocDatTai}", DCLapDat);
                    //data.Add("${ngayThangNamThiHanh}", NgayHieuLuc);
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
                        Stream read = respuesta.GetResponseStream();
                        MemoryStream memoryStream = new MemoryStream();
                        read.CopyTo(memoryStream);
                        byte[] bytes = memoryStream.ToArray();
                        File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\Images\EContract.pdf", memoryStream.ToArray());
                        //System.Diagnostics.Process.Start(@"D:\econtract.pdf");
                        return Convert.ToBase64String(bytes);
                    }
                    else
                    {
                        strResponse = "Error: " + respuesta.StatusCode;
                    }
                }
                else
                    strResponse = "Error: Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = "Error: " + ex.Message;
            }
            return strResponse;
        }

        public string createEContract(string HopDong, string DanhBo, DateTime CreateDate, string HoTen, string CCCD, string NgayCap, string DCThuongTru, string DCHienNay
            , string DienThoai, string Fax, string Email, string TaiKhoan, string Bank, string MST, string CoDHN, string DCLapDat, bool GanMoi, bool CaNhan, string MaDon, string SHS, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    renderEContract(HopDong, DanhBo, CreateDate, HoTen, CCCD, NgayCap, DCThuongTru, DCHienNay, DienThoai, Fax, Email, TaiKhoan, Bank, MST, CoDHN, DCLapDat, checksum);

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://api-econtract.cskhtanhoa.com.vn:1443/esolution-service/contracts/create-draft-from-file-and-identification");
                    request.Headers.Add("Authorization", "Bearer " + "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOlsidGVtcGxhdGUiLCJpYW0iLCJkc2lnbiIsInJlc3RzZXJ2aWNlIl0sInNjb3BlIjpbInJlYWQiLCJ3cml0ZSJdLCJyb2xlcyI6WyJUQU5IT0EiXSwiZXhwIjoxNjkyNzYxMTI1LCJwYXJ0eUlkIjoiOWZjZjRmNDEtNWE2OS00MmVkLWE4NjAtMzlkNjI5OTMzNzJmIiwiYXV0aG9yaXRpZXMiOlsiVEFOSE9BIl0sImp0aSI6ImVkNzg3ZWU4LTBjNTQtNDZiYy04ODdlLTFlMzk5MjFmOGM4YSIsImNsaWVudF9pZCI6InRhbmhvYS5jbGllbnRAZWNvbnRyYWN0LnZucHQudm4iLCJ1c2VybmFtZSI6ImFkbWludGFuaG9hIn0.flojyp3sMFmWKPt_c6IADg2vuepbYT9U6xBQ7_gQ1DPlLbeBHiimgkCV--hc4GLzuw7Ev-yNvRXqdJK_jO8mfGYwjQ-pSCKAk2ujffPzzWnchCTXeIpwSIpbI6DWD9OUMNQTbKaIvbGnbQsVpR93cEG6DhrFNH6YEdj0eZ-iydHKxrpEcX2MS3PEjfitkuTh4Xu8nBSKP7kbJg_dhA6L2Xz4TH6fOcO7ulfWJVFrxuAMsj7oLtiontwfqS3qeEvVrmuE8oxNUW_QXTwXA26ptWHq0i6TdQm0j_LXc6FDoaNEDb3kdgQWRgrWP-mfuq8R7FdpF7JR4dVtjq5SCX3B4Q");
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent("{}"), "fields");
                    if (CaNhan)
                        content.Add(new StringContent("{\"username\":\"" + CCCD + "\",\"cmnd\":\"\",\"email\":\"\",\"mst\":\"\",\"sdt\":\"" + DienThoai + "\",\"signFrame\":[{\"x\":650.95,\"y\":253.71536000000003,\"w\":120,\"h\":60,\"page\":2}],\"ten\":\"" + HoTen + "\",\"tenToChuc\":\"\",\"userType\":\"CONSUMER\"}"), "customer");
                    else
                        content.Add(new StringContent("{\"username\":\"" + MST + "\",\"cmnd\":\"\",\"email\":\"" + Email + "\",\"mst\":\"" + MST + "\",\"sdt\":\"" + DienThoai + "\",\"signFrame\":[{\"x\":650.95,\"y\":253.71536000000003,\"w\":120,\"h\":60,\"page\":2}],\"ten\":\"" + HoTen + "\",\"tenToChuc\":\"\",\"userType\":\"BUSINESS\"}"), "customer");
                    if (GanMoi)
                        content.Add(new StringContent("{\"autoRenew\":\"true\",\"contractValue\":\"0\",\"creationNote\":\"Ghi chú hợp đồng\",\"endDate\":\"\",\"flowTemplateId\":\"0fff387d-5646-4993-a5dd-6d96e2036537\",\"templateId\":\"64d3625c22ab89b8111134ad\",\"signFlowType\":\"REQUIRE_FLOW\",\"sequence\":1,\"signFlow\":[{\"signType\":\"DRAFT\",\"departmentId\":\"f420f37a-97c7-421d-af1c-4a0a7dfa88d5\",\"userId\":\"c6ee1894-498e-40fb-848f-00e02ad41f06\",\"sequence\":1,\"limitDate\":1,\"signForm\":[\"NO_AUTHEN\",\"OTP\",\"OTP_EMAIL\",\"SMART_CA\"],\"signFrame\":[{\"x\":1069.54,\"y\":292.11536,\"w\":1095.8591999999999,\"h\":303.15536,\"page\":1}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":1,\"signForm\":[\"SMART_CA\",\"USB_TOKEN\",\"NO_AUTHEN\",\"OTP\"],\"signFrame\":[{\"x\":957.82,\"y\":246.99536,\"w\":984.1192000000001,\"h\":258.03535999999997,\"page\":1},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":1,\"signForm\":[\"USB_TOKEN\",\"SMART_CA\",\"OTP\",\"OTP_EMAIL\"],\"signFrame\":[{\"x\":899.14,\"y\":273.63536,\"w\":925.4392,\"h\":284.67535999999996,\"page\":1}]}]}],\"signForm\":[\"OTP\",\"OTP_EMAIL\",\"NO_AUTHEN\"],\"title\":\"Hợp đồng gắn mới " + HoTen + "\",\"validDate\":\"\",\"verificationType\":\"NONE\",\"fixedPosition\":true,\"callbackUrl\":\"\"}"), "contract");
                    else
                        content.Add(new StringContent("{\"autoRenew\":\"true\",\"contractValue\":\"0\",\"creationNote\":\"Ghi chú hợp đồng\",\"endDate\":\"\",\"flowTemplateId\":\"a7e6494a-f7bc-4306-a3b6-d0842e9c69f3\",\"templateId\":\"64d3625c22ab89b8111134ad\",\"signFlowType\":\"REQUIRE_FLOW\",\"sequence\":1,\"signFlow\":[{\"signType\":\"DRAFT\",\"departmentId\":\"88bedc40-ccb5-4221-9e7f-c9c5aad49d67\",\"userId\":\"3440ba96-3683-49c3-9324-9e2d670d3481\",\"sequence\":1,\"limitDate\":1,\"signForm\":[\"NO_AUTHEN\",\"OTP\",\"OTP_EMAIL\",\"SMART_CA\"],\"signFrame\":[{\"x\":1069.54,\"y\":292.11536,\"w\":1095.8591999999999,\"h\":303.15536,\"page\":1}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":1,\"signForm\":[\"SMART_CA\",\"USB_TOKEN\",\"NO_AUTHEN\",\"OTP\"],\"signFrame\":[{\"x\":957.82,\"y\":246.99536,\"w\":984.1192000000001,\"h\":258.03535999999997,\"page\":1},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":1,\"signForm\":[\"USB_TOKEN\",\"SMART_CA\",\"OTP\",\"OTP_EMAIL\"],\"signFrame\":[{\"x\":899.14,\"y\":273.63536,\"w\":925.4392,\"h\":284.67535999999996,\"page\":1}]}]}],\"signForm\":[\"OTP\",\"OTP_EMAIL\",\"NO_AUTHEN\"],\"title\":\"Hợp đồng sang tên " + HoTen + "\",\"validDate\":\"\",\"verificationType\":\"NONE\",\"fixedPosition\":true,\"callbackUrl\":\"\"}"), "contract");
                    content.Add(new StreamContent(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"\Images\EContract.pdf")), "attachFile", AppDomain.CurrentDomain.BaseDirectory + @"\Images\EContract.pdf");
                    content.Add(new StreamContent(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"\Images\face.png")), "EKYC_CHANDUNG", AppDomain.CurrentDomain.BaseDirectory + @"\Images\face.png");
                    content.Add(new StreamContent(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"\Images\cccd1.png")), "EKYC_MATTRUOC", AppDomain.CurrentDomain.BaseDirectory + @"\Images\cccd1.png");
                    content.Add(new StreamContent(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"\Images\cccd2.png")), "EKYC_MATSAU", AppDomain.CurrentDomain.BaseDirectory + @"\Images\cccd2.png");
                    request.Content = content;
                    var response = client.SendAsync(request);
                    response.Result.EnsureSuccessStatusCode();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var result = response.Result.Content.ReadAsStringAsync();
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result.Result.ToString());
                        _cDAL_TTKH.ExecuteNonQuery("insert into Zalo_EContract_ChiTiet(DienThoai,IDContract,DanhBo,MaDon,SHS)values('" + DienThoai + "','" + obj["message"]["contractId"] + "','" + DanhBo + "'," + MaDon + ",'" + SHS + "')");
                    }
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return strResponse;
        }

        public string sendEContract(string MaDon, string SHS, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    DataTable dt;
                    if (MaDon != "")
                        dt = _cDAL_TTKH.ExecuteQuery_DataTable("select IDEContract from Zalo_EContract_ChiTiet where MaDon=" + MaDon);
                    else
                        dt = _cDAL_TTKH.ExecuteQuery_DataTable("select IDEContract from Zalo_EContract_ChiTiet where SHS='" + SHS + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "esolution-service/contracts/" + dt.Rows[0]["IDEContract"].ToString() + "/submit-contract");
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        request.ContentLength = 0;
                        request.Headers["Authorization"] = "Bearer " + getAccess_token();
                        HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                        if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                        {
                            StreamReader read = new StreamReader(respuesta.GetResponseStream());
                            string result = read.ReadToEnd();
                            read.Close();
                            respuesta.Close();
                        }
                        else
                        {
                            strResponse = "Error: " + respuesta.StatusCode;
                        }
                    }
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return strResponse;
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