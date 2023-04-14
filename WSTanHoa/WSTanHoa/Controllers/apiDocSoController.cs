using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/DocSo")]
    public class apiDocSoController : ApiController
    {
        private CConnection _cDAL_DHN = new CConnection(CGlobalVariable.DHN);
        private CConnection _cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);
        private CConnection _cDAL_sDHN = new CConnection(CGlobalVariable.sDHN);
        private CConnection _cDAL_ThuongVu = new CConnection(CGlobalVariable.ThuongVu);
        private CConnection _cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTien);
        private QLDHNController _QLDHNController = new QLDHNController();
        private string url = "https://dhntm.sawaco.com.vn/";
        private string urlApi = "https://dhntmapi.sawaco.com.vn/";
        private string urlTest = "http://testdhntm.sawaco.com.vn/";
        //private string urlApiTest = "http://testdhntmapi.sawaco.com.vn/";


        [Route("updateDS_sDHN")]
        [HttpGet]
        public bool updateDS_sDHN(string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    updateDS_sDHN_HoaSen();
                    updateDS_sDHN_Rynan();
                    updateDS_sDHN_Deviwas();
                    updateDS_sDHN_PhamLam();
                    updateDS_sDHN_DucHung();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool updateDS_sDHN_HoaSen()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8033/api/all/?req=list_swm_Id");
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=1");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists_DHN(item["MaDanhbo"]) == true)
                            if (checkExists_sDHN(item["MaDanhbo"]) == false)
                                if (string.IsNullOrEmpty(item["SeriModule"]))
                                    _cDAL_sDHN.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item["MaDanhbo"] + "',1,1,0)");
                                else
                                    _cDAL_sDHN.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,IDLogger,Valid,CreateBy)values('" + item["MaDanhbo"] + "',1,'" + item["SeriModule"] + "',1,0)");
                            else
                                if (string.IsNullOrEmpty(item["SeriModule"]))
                                _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=1 where DanhBo='" + item["MaDanhbo"] + "' and IDNCC=1");
                            else
                                _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=1,IDLogger='" + item["SeriModule"] + "' where DanhBo='" + item["MaDanhbo"] + "' and IDNCC=1");
                    }
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool updateDS_sDHN_Rynan()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:7032/api/list_swm_Id");
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=2");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists_DHN(item) == true)
                            if (checkExists_sDHN(item) == false)
                                _cDAL_sDHN.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item + "',2,1,0)");
                            else
                                _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=1 where DanhBo='" + item + "' and IDNCC=2");
                    }
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool updateDS_sDHN_Deviwas()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8039/api/all/?req=list_swm_Id");
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=3");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists_DHN(item["MaDanhbo"]) == true)
                            if (checkExists_sDHN(item["MaDanhbo"]) == false)
                                if (string.IsNullOrEmpty(item["SeriModule"]))
                                    _cDAL_sDHN.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item["MaDanhbo"] + "',3,1,0)");
                                else
                                    _cDAL_sDHN.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,IDLogger,Valid,CreateBy)values('" + item["MaDanhbo"] + "',3,'" + item["SeriModule"] + "',1,0)");
                            else
                                if (string.IsNullOrEmpty(item["SeriModule"]))
                                _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=1 where DanhBo='" + item["MaDanhbo"] + "' and IDNCC=3");
                            else
                                _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=1,IDLogger='" + item["SeriModule"] + "' where DanhBo='" + item["MaDanhbo"] + "' and IDNCC=3");
                    }
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool updateDS_sDHN_PhamLam()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8032/apipl/List_Swm_Id");
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=4");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists_DHN(item["wmid"]) == true)
                            if (checkExists_sDHN(item["wmid"]) == false)
                                if (string.IsNullOrEmpty(item["idlogger"]))
                                    _cDAL_sDHN.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item["wmid"] + "',4,1,0)");
                                else
                                    _cDAL_sDHN.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,IDLogger,Valid,CreateBy)values('" + item["wmid"] + "',4,'" + item["idlogger"] + "',1,0)");
                            else
                                 if (string.IsNullOrEmpty(item["idlogger"]))
                                _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=1 where DanhBo='" + item["wmid"] + "' and IDNCC=4");
                            else
                                _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=1,IDLogger='" + item["idlogger"] + "' where DanhBo='" + item["wmid"] + "' and IDNCC=4");
                    }
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool updateDS_sDHN_DucHung()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8030/tanhoa/api/list_swm_Id");
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=5");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists_DHN(item["MaDanhbo"]) == true)
                            if (checkExists_sDHN(item["MaDanhbo"]) == false)
                                if (string.IsNullOrEmpty(item["SeriModule"]))
                                    _cDAL_sDHN.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item["MaDanhbo"] + "',5,1,0)");
                                else
                                    _cDAL_sDHN.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,IDLogger,Valid,CreateBy)values('" + item["MaDanhbo"] + "',5,'" + item["SeriModule"] + "',1,0)");
                            else
                                if (string.IsNullOrEmpty(item["SeriModule"]))
                                _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=1 where DanhBo='" + item["MaDanhbo"] + "' and IDNCC=5");
                            else
                                _cDAL_sDHN.ExecuteNonQuery("update sDHN set Valid=1,IDLogger='" + item["SeriModule"] + "' where DanhBo='" + item["MaDanhbo"] + "' and IDNCC=5");
                    }
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        [Route("getChiSo_sDHN_Day_Back")]
        [HttpGet]
        public bool getChiSo_sDHN_Day_Back(string Time, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    string[] datestr = Time.Split('-');
                    DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("select * from"
                                     + " (select DanhBo, IDNCC, SoLuong = (select COUNT(*) from sDHN_LichSu where CAST(ThoiGianCapNhat as date) = '" + datestr[2] + datestr[1] + datestr[0] + "' and DanhBo = sDHN.DanhBo) from sDHN"
                                     + " where Valid = 1)t1"
                                     + " where t1.SoLuong < 24");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        switch (int.Parse(dt.Rows[i]["IDNCC"].ToString()))
                        {
                            case 1:
                                get_All_HoaSen(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 2:
                                get_All_Rynan(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 3:
                                get_All_Deviwas(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 4:
                                get_All_PhamLam(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 5:
                                get_All_DucHung(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            default:
                                break;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("getChiSo_sDHN_Day")]
        [HttpGet]
        public bool getChiSo_sDHN_Day(string Time, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("select DanhBo,IDNCC from sDHN where Valid=1 order by DanhBo");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        switch (int.Parse(dt.Rows[i]["IDNCC"].ToString()))
                        {
                            case 1:
                                get_All_HoaSen(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 2:
                                get_All_Rynan(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 3:
                                get_All_Deviwas(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 4:
                                get_All_PhamLam(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 5:
                                get_All_DucHung(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            default:
                                break;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("getChiSo_sDHN_Hour")]
        [HttpGet]
        private bool getChiSo_sDHN_Hour(string Time, string Hour, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("select DanhBo,IDNCC from sDHN where Valid=1 order by DanhBo");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        switch (int.Parse(dt.Rows[i]["IDNCC"].ToString()))
                        {
                            case 1:
                                get_All_HoaSen(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 2:
                                get_All_Rynan(dt.Rows[i]["DanhBo"].ToString(), Time, Hour);
                                break;
                            case 3:
                                get_All_Deviwas(dt.Rows[i]["DanhBo"].ToString(), Time, Hour);
                                break;
                            case 4:
                                get_All_PhamLam(dt.Rows[i]["DanhBo"].ToString(), Time, Hour);
                                break;
                            default:
                                break;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("updateDiffRUN")]
        [HttpGet]
        private bool updateDiffRUN(string Time, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("select DanhBo,IDNCC from sDHN where Valid=1 order by DanhBo");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        updateDiff(dt.Rows[i]["DanhBo"].ToString(), Time);
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void updateDiff(string DanhBo, string Time)
        {
            string[] datestr = Time.Split('-');
            DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("select ChiSo,DanhBo,ThoiGianCapNhat=convert(varchar(30),ThoiGianCapNhat,120) from sDHN_LichSu where DanhBo='" + DanhBo + "' and cast(ThoiGianCapNhat as date)='" + datestr[2] + datestr[1] + datestr[0] + "' order by ThoiGianCapNhat asc");
            DataTable dtTruoc = _cDAL_sDHN.ExecuteQuery_DataTable("select top 1 ChiSo,DanhBo,ThoiGianCapNhat=convert(varchar(30),ThoiGianCapNhat,120) from sDHN_LichSu where DanhBo='" + DanhBo + "' and cast(ThoiGianCapNhat as date)=DATEADD(DAY, -1, '" + datestr[2] + datestr[1] + datestr[0] + "') order by ThoiGianCapNhat desc");
            if (dtTruoc != null && dtTruoc.Rows.Count > 0)
                _cDAL_sDHN.ExecuteNonQuery("update sDHN_LichSu set Diff=" + (double.Parse(dt.Rows[0]["ChiSo"].ToString()) - double.Parse(dtTruoc.Rows[0]["ChiSo"].ToString())).ToString("0.000") + " where DanhBo='" + dt.Rows[0]["DanhBo"].ToString() + "' and ThoiGianCapNhat=convert(datetime,'" + dt.Rows[0]["ThoiGianCapNhat"].ToString() + "')");
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                _cDAL_sDHN.ExecuteNonQuery("update sDHN_LichSu set Diff=" + (double.Parse(dt.Rows[i]["ChiSo"].ToString()) - double.Parse(dt.Rows[i - 1]["ChiSo"].ToString())).ToString("0.000") + " where DanhBo='" + dt.Rows[i]["DanhBo"].ToString() + "' and ThoiGianCapNhat=convert(datetime,'" + dt.Rows[i]["ThoiGianCapNhat"].ToString() + "')");
            }
        }

        private bool get_All_HoaSen(string DanhBo, string Time)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8033/api/Survey/?id=" + DanhBo + "&date=" + Time);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                //if (request.HaveResponse == true)
                {
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        if (result != "")
                        {
                            var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                            foreach (var item in obj)
                            {
                                //int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                                //if (obj["IsLowBatt"] == true)
                                //    flagCBPinYeu = 1;
                                //if (obj["IsLeakage"] == true)
                                //    flagCBRoRi = 1;
                                //if (obj["IsOverLoad"] == true)
                                //    flagCBQuaDong = 1;
                                //if (obj["IsReverse"] == true)
                                //    flagCBChayNguoc = 1;
                                //if (obj["IsTampering"] == true)
                                //    flagCBNamCham = 1;
                                //if (obj["IsDry"] == true)
                                //    flagCBKhoOng = 1;
                                //if (obj["IsOpenBox"] == true)
                                //    flagCBMoHop = 1;
                                //string LuuLuong = "NULL";
                                //if (obj["Flow"] != null)
                                //    LuuLuong = obj["Flow"];
                                //string sql = "insert into sDHN_LichSu(ID,DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                //    + ",Longitude,Latitude,Altitude,ChuKyGui"
                                //    + ",ThoiGianCapNhat,Loai)"
                                //               + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                                //               + ",'" + DanhBo + "'"
                                //               + "," + obj["Volume"]
                                //               + "," + obj["Battery"]
                                //               + ",'" + obj["RemainBatt"] + "'"
                                //               + "," + LuuLuong
                                //               + ",'" + obj["Rssi"] + "'"
                                //               + "," + flagCBPinYeu
                                //               + "," + flagCBRoRi
                                //               + "," + flagCBQuaDong
                                //               + "," + flagCBChayNguoc
                                //               + "," + flagCBNamCham
                                //               + "," + flagCBKhoOng
                                //               + "," + flagCBMoHop
                                //               + "," + obj["Longitude"]
                                //               + "," + obj["Latitude"]
                                //               + "," + obj["Altitude"]
                                //               + "," + obj["Interval"]
                                //               + ",'" + obj["Time"] + "',N'All')";
                                string sql = "if not exists(select * from sDHN_LichSu where DanhBo='" + DanhBo + "' and ThoiGianCapNhat=convert(datetime,'" + item["Time"] + "'))"
                                            + " insert into sDHN_LichSu(DanhBo,ChiSo,ThoiGianCapNhat,Loai)"
                                           + "values("
                                           + "'" + DanhBo + "'"
                                           + "," + item["Volume"]
                                           + ",'" + item["Time"] + "',N'All')";
                                _cDAL_sDHN.ExecuteNonQuery(sql);
                            }
                            updateDiff(DanhBo, Time);
                            return true;
                        }
                        return false;
                    }
                    else
                        return false;
                }
                //else
                //    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool get_All_Rynan(string DanhBo, string Time)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:7032/api/swm_hour?Id=" + DanhBo + "&Date=" + Time);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                //if (request.HaveResponse == true)
                {
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        if (result != "Not value return.")
                        {
                            var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                            foreach (var item in obj)
                            {
                                int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                                if (item["IsLowBatt"] == true)
                                    flagCBPinYeu = 1;
                                if (item["IsLeakage"] == true)
                                    flagCBRoRi = 1;
                                if (item["IsOverLoad"] == true)
                                    flagCBQuaDong = 1;
                                if (item["IsReverse"] == true)
                                    flagCBChayNguoc = 1;
                                if (item["IsTampering"] == true)
                                    flagCBNamCham = 1;
                                if (item["IsDry"] == true)
                                    flagCBKhoOng = 1;
                                if (item["IsOpenBox"] == true)
                                    flagCBMoHop = 1;
                                string LuuLuong = "NULL";
                                if (item["Flow"] != null)
                                    LuuLuong = ((int)item["Flow"]).ToString();
                                string sql = "if not exists(select * from sDHN_LichSu where DanhBo='" + DanhBo + "' and ThoiGianCapNhat=convert(datetime,'" + item["Time"] + "'))"
                                    + " insert into sDHN_LichSu(DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                    + ",Longitude,Latitude,Altitude,ChuKyGui"
                                    + ",ThoiGianCapNhat,Loai)"
                                               + "values("
                                               + "'" + DanhBo + "'"
                                               + "," + item["Volume"]
                                               + "," + item["Battery"]
                                               + ",'" + item["RemainBatt"] + "'"
                                               + "," + LuuLuong
                                               + ",'" + item["Rssi"] + "'"
                                               + "," + flagCBPinYeu
                                               + "," + flagCBRoRi
                                               + "," + flagCBQuaDong
                                               + "," + flagCBChayNguoc
                                               + "," + flagCBNamCham
                                               + "," + flagCBKhoOng
                                               + "," + flagCBMoHop
                                               + ",NULL" //+ item["Longitude"]
                                               + ",NULL" //+ item["Latitude"]
                                               + ",NULL"
                                               + ",NULL" //+ item["Interval"]
                                               + ",'" + item["Time"] + "',N'All')";
                                _cDAL_sDHN.ExecuteNonQuery(sql);
                            }
                            updateDiff(DanhBo, Time);
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                //else
                //    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool get_All_Rynan(string DanhBo, string Time, string Hour)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:7032/api/swm_hour?Id=" + DanhBo + "&Date=" + Time + "&Hour=" + Hour);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                //if (request.HaveResponse == true)
                {
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        if (result != "Not value return.")
                        {
                            var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                            int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                            if (obj["IsLowBatt"] == true)
                                flagCBPinYeu = 1;
                            if (obj["IsLeakage"] == true)
                                flagCBRoRi = 1;
                            if (obj["IsOverLoad"] == true)
                                flagCBQuaDong = 1;
                            if (obj["IsReverse"] == true)
                                flagCBChayNguoc = 1;
                            if (obj["IsTampering"] == true)
                                flagCBNamCham = 1;
                            if (obj["IsDry"] == true)
                                flagCBKhoOng = 1;
                            if (obj["IsOpenBox"] == true)
                                flagCBMoHop = 1;
                            string LuuLuong = "NULL";
                            if (obj["Flow"] != null)
                                LuuLuong = ((int)obj["Flow"]).ToString();
                            string sql = "if not exists(select * from sDHN_LichSu where DanhBo='" + DanhBo + "' and ThoiGianCapNhat=convert(datetime,'" + obj["Time"] + "'))"
                                + " insert into sDHN_LichSu(DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                + ",Longitude,Latitude,Altitude,ChuKyGui"
                                + ",ThoiGianCapNhat,Loai)"
                                           + "values("
                                           + "'" + DanhBo + "'"
                                           + "," + obj["Volume"]
                                           + "," + obj["Battery"]
                                           + ",'" + obj["RemainBatt"] + "'"
                                           + "," + LuuLuong
                                           + ",'" + obj["Rssi"] + "'"
                                           + "," + flagCBPinYeu
                                           + "," + flagCBRoRi
                                           + "," + flagCBQuaDong
                                           + "," + flagCBChayNguoc
                                           + "," + flagCBNamCham
                                           + "," + flagCBKhoOng
                                           + "," + flagCBMoHop
                                           + ",NULL" //+ obj["Longitude"]
                                           + ",NULL" //+ obj["Latitude"]
                                           + ",NULL"
                                           + ",NULL" //+ obj["Interval"]
                                           + ",'" + obj["Time"] + "',N'All')";
                            _cDAL_sDHN.ExecuteNonQuery(sql);
                            updateDiff(DanhBo, Time);
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                //else
                //    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool get_All_Deviwas(string DanhBo, string Time)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8039/api/all?id=" + DanhBo + "&date=" + Time);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                //if (request.HaveResponse == true)
                {
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();

                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        foreach (var item in obj)
                        {
                            int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                            string[] canhbaos;
                            if (item["Warning"] != null)
                            {
                                canhbaos = ((string)item["Warning"]).Split('|');
                                foreach (var itemW in canhbaos)
                                {
                                    switch (itemW)
                                    {
                                        case "leakage current":
                                        case "leakage historic":
                                            flagCBRoRi = 1;
                                            break;
                                        case "no usage":
                                            flagCBKhoOng = 1;
                                            break;
                                        case "back flow":
                                            flagCBChayNguoc = 1;
                                            break;
                                        case "over flow":
                                            flagCBQuaDong = 1;
                                            break;
                                        case "low battery":
                                            flagCBPinYeu = 1;
                                            break;
                                        case "mechanical fraud":
                                            flagCBMoHop = 1;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            string LuuLuong = "NULL";
                            //if (item["Flow"] != null)
                            //    LuuLuong = item["Flow"];
                            string sql = "if not exists(select * from sDHN_LichSu where DanhBo='" + DanhBo + "' and ThoiGianCapNhat=convert(datetime,'" + item["TimeUpdate"] + "'))"
                                + " insert into sDHN_LichSu(DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                + ",Longitude,Latitude,Altitude,ChuKyGui"
                                + ",ThoiGianCapNhat,Loai)"
                                           + "values("
                                           + "'" + DanhBo + "'"
                                           + "," + item["Vol"]
                                           + ",NULL" //+ item["Battery"]
                                           + ",'" + item["bat_duration"] + "'"
                                           + "," + LuuLuong
                                           + ",NULL" //+ item["Rssi"] + "'"
                                           + "," + flagCBPinYeu
                                           + "," + flagCBRoRi
                                           + "," + flagCBQuaDong
                                           + "," + flagCBChayNguoc
                                           + "," + flagCBNamCham
                                           + "," + flagCBKhoOng
                                           + "," + flagCBMoHop
                                           + ",NULL" //+ item["Longitude"]
                                           + ",NULL" //+ item["Latitude"]
                                           + ",NULL" //+ item["Altitude"]
                                           + ",NULL" //+ item["Interval"]
                                           + ",'" + item["TimeUpdate"] + "',N'All')";
                            _cDAL_sDHN.ExecuteNonQuery(sql);
                        }
                        updateDiff(DanhBo, Time);
                        return true;
                    }
                    else
                        return false;
                }
                //else
                //    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool get_All_Deviwas(string DanhBo, string Time, string Hour)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8039/api/all?id=" + DanhBo + "&date=" + Time + "&hour=" + Hour);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                //if (request.HaveResponse == true)
                {
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();

                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        foreach (var item in obj)
                        {
                            int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                            string[] canhbaos;
                            if (item["Warning"] != "")
                            {
                                canhbaos = ((string)item["Warning"]).Split('|');
                                foreach (var itemW in canhbaos)
                                {
                                    switch (itemW)
                                    {
                                        case "leakage current":
                                        case "leakage historic":
                                            flagCBRoRi = 1;
                                            break;
                                        case "no usage":
                                            flagCBKhoOng = 1;
                                            break;
                                        case "back flow":
                                            flagCBChayNguoc = 1;
                                            break;
                                        case "over flow":
                                            flagCBQuaDong = 1;
                                            break;
                                        case "low battery":
                                            flagCBPinYeu = 1;
                                            break;
                                        case "mechanical fraud":
                                            flagCBMoHop = 1;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            string LuuLuong = "NULL";
                            //if (item["Flow"] != null)
                            //    LuuLuong = item["Flow"];
                            string sql = "if not exists(select * from sDHN_LichSu where DanhBo='" + DanhBo + "' and ThoiGianCapNhat=convert(datetime,'" + item["TimeUpdate"] + "'))"
                                + " insert into sDHN_LichSu(DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                + ",Longitude,Latitude,Altitude,ChuKyGui"
                                + ",ThoiGianCapNhat,Loai)"
                                           + "values("
                                           + "'" + DanhBo + "'"
                                           + "," + item["Vol"]
                                           + ",NULL" //+ item["Battery"]
                                           + ",'" + item["bat_duration"] + "'"
                                           + "," + LuuLuong
                                           + ",NULL" //+ item["Rssi"] + "'"
                                           + "," + flagCBPinYeu
                                           + "," + flagCBRoRi
                                           + "," + flagCBQuaDong
                                           + "," + flagCBChayNguoc
                                           + "," + flagCBNamCham
                                           + "," + flagCBKhoOng
                                           + "," + flagCBMoHop
                                           + ",NULL" //+ item["Longitude"]
                                           + ",NULL" //+ item["Latitude"]
                                           + ",NULL" //+ item["Altitude"]
                                           + ",NULL" //+ item["Interval"]
                                           + ",'" + item["TimeUpdate"] + "',N'All')";
                            _cDAL_sDHN.ExecuteNonQuery(sql);
                        }
                        updateDiff(DanhBo, Time);
                        return true;
                    }
                    else
                        return false;
                }
                //else
                //    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool get_All_PhamLam(string DanhBo, string Time)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8032/apipl/swm_hour/" + DanhBo + "/" + Time);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                //if (request.HaveResponse == true)
                {
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();

                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        foreach (var item in obj)
                        {
                            int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                            if (item["isLowBatt"] == true)
                                flagCBPinYeu = 1;
                            if (item["isLeakage"] == true)
                                flagCBRoRi = 1;
                            if (item["isOverLoad"] == true)
                                flagCBQuaDong = 1;
                            if (item["isReverse"] == true)
                                flagCBChayNguoc = 1;
                            if (item["isTampering"] == true)
                                flagCBNamCham = 1;
                            if (item["isDry"] == true)
                                flagCBKhoOng = 1;
                            if (item["isOpenBox"] == true)
                                flagCBMoHop = 1;
                            string LuuLuong = "NULL";
                            if (item["flow"] != null)
                                LuuLuong = ((int)item["flow"]).ToString();
                            string sql = "if not exists(select * from sDHN_LichSu where DanhBo='" + DanhBo + "' and ThoiGianCapNhat=convert(datetime,'" + item["time"] + "'))"
                                + " insert into sDHN_LichSu(DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                + ",Longitude,Latitude,Altitude,ChuKyGui"
                                + ",ThoiGianCapNhat,Loai)"
                                           + "values("
                                           + "'" + DanhBo + "'"
                                           + "," + item["vol"]
                                           + ",NULL" //+ item["Battery"]
                                           + ",NULL" //+ item["RemainBatt"] + "'"
                                           + "," + LuuLuong
                                           + ",'" + item["rsrp"] + "'"
                                           + "," + flagCBPinYeu
                                           + "," + flagCBRoRi
                                           + "," + flagCBQuaDong
                                           + "," + flagCBChayNguoc
                                           + "," + flagCBNamCham
                                           + "," + flagCBKhoOng
                                           + "," + flagCBMoHop
                                           + ",NULL" //+ item["longitude"]
                                           + ",NULL" //+ item["latitude"]
                                           + ",NULL" //+ item["altitude"]
                                           + ",NULL" //+ item["interval"]
                                           + ",'" + item["time"] + "',N'All')";
                            //string sql = "update sDHN_LichSu set LuuLuong=ChiSo,ChiSo=" + item["vol"] + " where danhbo='" + DanhBo + "' and ThoiGianCapNhat=convert(datetime,'" + item["time"] + "')";
                            _cDAL_sDHN.ExecuteNonQuery(sql);
                        }
                        updateDiff(DanhBo, Time);
                        return true;
                    }
                    else
                        return false;
                }
                //else
                //    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool get_All_PhamLam(string DanhBo, string Time, string Hour)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8032/apipl/swm_hour/" + DanhBo + "/" + Time + "/" + Hour);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                //if (request.HaveResponse == true)
                {
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        if (result != "NoneData")
                        {
                            var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                            int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                            if (obj["isLowBatt"] == true)
                                flagCBPinYeu = 1;
                            if (obj["isLeakage"] == true)
                                flagCBRoRi = 1;
                            if (obj["isOverLoad"] == true)
                                flagCBQuaDong = 1;
                            if (obj["isReverse"] == true)
                                flagCBChayNguoc = 1;
                            if (obj["isTampering"] == true)
                                flagCBNamCham = 1;
                            if (obj["isDry"] == true)
                                flagCBKhoOng = 1;
                            if (obj["isOpenBox"] == true)
                                flagCBMoHop = 1;
                            string LuuLuong = "NULL";
                            if (obj["flow"] != null)
                                LuuLuong = ((int)obj["flow"]).ToString();
                            string sql = "if not exists(select * from sDHN_LichSu where DanhBo='" + DanhBo + "' and ThoiGianCapNhat=convert(datetime,'" + obj["time"] + "'))"
                                + " insert into sDHN_LichSu(DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                + ",Longitude,Latitude,Altitude,ChuKyGui"
                                + ",ThoiGianCapNhat,Loai)"
                                           + "values("
                                           + "'" + DanhBo + "'"
                                           + "," + obj["vol"]
                                           + ",NULL" //+ obj["Battery"]
                                           + ",NULL" //+ obj["RemainBatt"] + "'"
                                           + "," + LuuLuong
                                           + ",'" + obj["rsrp"] + "'"
                                           + "," + flagCBPinYeu
                                           + "," + flagCBRoRi
                                           + "," + flagCBQuaDong
                                           + "," + flagCBChayNguoc
                                           + "," + flagCBNamCham
                                           + "," + flagCBKhoOng
                                           + "," + flagCBMoHop
                                           + ",NULL" //+ obj["longitude"]
                                           + ",NULL" //+ obj["latitude"]
                                           + ",NULL" //+ obj["altitude"]
                                           + ",NULL" //+ obj["interval"]
                                           + ",'" + obj["time"] + "',N'All')";
                            _cDAL_sDHN.ExecuteNonQuery(sql);
                        }
                        updateDiff(DanhBo, Time);
                        return true;
                    }
                    else
                        return false;
                }
                //else
                //    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool get_All_DucHung(string DanhBo, string Time)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8030/tanhoa/api/swm_hour?Id=" + DanhBo + "&Date=" + Time);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                //if (request.HaveResponse == true)
                {
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        if (result != "Not value return.")
                        {
                            var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                            foreach (var item in obj)
                            {
                                int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                                if (item["IsLowBatt"] == true)
                                    flagCBPinYeu = 1;
                                if (item["IsLeakage"] == true)
                                    flagCBRoRi = 1;
                                if (item["IsOverLoad"] == true)
                                    flagCBQuaDong = 1;
                                if (item["IsReverse"] == true)
                                    flagCBChayNguoc = 1;
                                if (item["IsTampering"] == true)
                                    flagCBNamCham = 1;
                                if (item["IsDry"] == true)
                                    flagCBKhoOng = 1;
                                if (item["IsOpenBox"] == true)
                                    flagCBMoHop = 1;
                                string LuuLuong = "NULL";
                                if (item["Flow"] != null)
                                    LuuLuong = ((int)item["Flow"]).ToString();
                                string sql = "if not exists(select * from sDHN_LichSu where DanhBo='" + DanhBo + "' and ThoiGianCapNhat=convert(datetime,'" + item["Time"] + "'))"
                                    + " insert into sDHN_LichSu(DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                    + ",Longitude,Latitude,Altitude,ChuKyGui"
                                    + ",ThoiGianCapNhat,Loai)"
                                               + "values("
                                               + "'" + DanhBo + "'"
                                               + "," + item["Volume"]
                                               + ",NULL" //+ item["Battery"]
                                               + ",'" + item["RemainBatt"] + "'"
                                               + "," + LuuLuong
                                               + ",'" + item["Rssi"] + "'"
                                               + "," + flagCBPinYeu
                                               + "," + flagCBRoRi
                                               + "," + flagCBQuaDong
                                               + "," + flagCBChayNguoc
                                               + "," + flagCBNamCham
                                               + "," + flagCBKhoOng
                                               + "," + flagCBMoHop
                                               + ",NULL" //+ item["Longitude"]
                                               + ",NULL" //+ item["Latitude"]
                                               + ",NULL"
                                               + ",NULL" //+ item["Interval"]
                                               + ",'" + item["Time"] + "',N'All')";
                                _cDAL_sDHN.ExecuteNonQuery(sql);
                            }
                            updateDiff(DanhBo, Time);
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                //else
                //    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool get_ChiSoNuoc_PhamLam(string DanhBo, string Time)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8032/apipl/Volume/" + DanhBo + "/" + Time);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                //if (request.HaveResponse == true)
                {
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        if (result != "NoneData")
                        {
                            var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                            foreach (var item in obj)
                            {
                                string sql = "insert into sDHN_LichSu(ID,DanhBo,ChiSo,ThoiGianCapNhat,Loai)"
                                               + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                                               + ",'" + DanhBo + "'"
                                               + "," + item["vol"]
                                               + ",'" + item["timeUpdate"] + "',N'All')";
                                _cDAL_sDHN.ExecuteNonQuery(sql);
                            }
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                //else
                //    return false;
            }
            catch
            {
                return false;
            }
        }

        [Route("getDS_ThaysDHN")]
        [HttpGet]
        public IList<ThongTinKhachHang> getDS_ThaysDHN(string TuNgay, string DenNgay, string checksum)
        {
            try
            {
                if (checksum == "DHC@2022")
                {
                    List<ThongTinKhachHang> lst = new List<ThongTinKhachHang>();
                    if (TuNgay != "" && DenNgay != "")
                    {
                        string[] dateTustr = TuNgay.Split('/');
                        DateTime dateTu = new DateTime(int.Parse(dateTustr[2]), int.Parse(dateTustr[1]), int.Parse(dateTustr[0]));
                        string[] dateDenstr = DenNgay.Split('/');
                        DateTime dateDen = new DateTime(int.Parse(dateDenstr[2]), int.Parse(dateDenstr[1]), int.Parse(dateDenstr[0]));
                        DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("SELECT ROW_NUMBER() OVER (ORDER BY HCT_NGAYGAN  ASC) [STT],  DanhBo=DHN_DANHBO,DIACHI,HoTen=TENKH, REPLACE(DHN_TODS,'DHTM-','TH-') AS MADMA,HCT_HIEUDHNGAN,SoThanDH=HCT_SOTHANGAN,HCT_CODHNGAN,NgayThay=HCT_NGAYGAN"
                                                        + " FROM TB_THAYDHN WHERE DHN_LOAIBANGKE = 'DHTM' AND CAST(HCT_NGAYGAN as date) >= '" + dateTu.ToString("yyyyMMdd") + "' and CAST(HCT_NGAYGAN as date) <= '" + dateDen.ToString("yyyyMMdd") + "' and HCT_HIEUDHNGAN like 'B-METERS'"
                                                        + " ORDER BY HCT_NGAYGAN ASC");
                        foreach (DataRow item in dt.Rows)
                        {
                            ThongTinKhachHang en = new ThongTinKhachHang();
                            en.DanhBo = item["DanhBo"].ToString();
                            en.HoTen = item["HoTen"].ToString();
                            en.DiaChi = item["DiaChi"].ToString();
                            en.SoThanDH = item["SoThanDH"].ToString();
                            en.NgayThay = DateTime.Parse(item["NgayThay"].ToString());
                            lst.Add(en);
                        }
                    }
                    return lst;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        private string DataTableToJSON(DataTable table)
        {
            CGlobalVariable.jsSerializer.MaxJsonLength = Int32.MaxValue;
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return CGlobalVariable.jsSerializer.Serialize(parentRow);
        }

        private bool checkExists_sDHN(string DanhBo)
        {
            try
            {
                object result = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select * from sDHN where DanhBo='" + DanhBo + "'");
                if (result != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool checkExists_DHN(string DanhBo)
        {
            try
            {
                object result = _cDAL_DHN.ExecuteQuery_ReturnOneValue("select * from TB_DULIEUKHACHHANG where DanhBo='" + DanhBo + "'");
                if (result != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //api TCT

        private string getAccess_token()
        {
            return _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select access_token from Configure").ToString();
        }

        [Route("getAccess_tokenFromTCT")]
        [HttpGet]
        public string getAccess_tokenFromTCT(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.cheksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                           | SecurityProtocolType.Ssl3;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "api/Authenticate/login");
                    request.Method = "POST";
                    request.ContentType = "application/json";

                    var data = new
                    {
                        Username = "quocbao241@gmail.com",
                        Password = "Harry_240189"
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
                        bool flag = _cDAL_sDHN.ExecuteNonQuery("update Configure set access_token='" + obj["token"] + "',expiration_date='" + obj["expiration"] + "',CreateDate=getdate()");
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

        [Route("checkExists_TCT")]
        [HttpGet]
        public string checkExists_TCT(string serialnumber, string DanhBo, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.cheksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                           | SecurityProtocolType.Ssl3;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "api/DongHoNuoc/TraCuuDongHoNuoc?so_seri=" + serialnumber + "&danhBo=" + DanhBo);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select access_token from Configure").ToString();

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        if (obj["ketQua"] == 1)
                            return "1";
                        else
                            return "-1";
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

        [Route("them_TCT")]
        [HttpGet]
        public string themDHN_TCT(string DanhBo, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.cheksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                           | SecurityProtocolType.Ssl3;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "api/DongHoNuoc/ThemDongHoNuoc");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select access_token from Configure").ToString();
                    DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("select serialnumber= CASE WHEN ttkh.HIEUDH='EMS' THEN ISNULL((select Serial_number from [sDHN].[dbo].[DHN_PHAMLAM] where DANHBO=dhn.DanhBo),0) ELSE ttkh.SOTHANDH END"
                                + " ,IDModule = ISNULL((select IDLogger from[sDHN].[dbo].[sDHN] where DanhBo = dhn.DanhBo),0)"
                                + " ,IDLogger = ISNULL((select IDLogger from[sDHN].[dbo].[sDHN] where DanhBo = dhn.DanhBo),0)"
                                + " ,sim = ''"
                                + " ,dhn.DanhBo,dot_ds = SUBSTRING(ttkh.LOTRINH, 1, 2),so_ds = SUBSTRING(ttkh.LOTRINH, 3, 2),MLT = ttkh.LOTRINH"
                                + " ,ttkh.HOTEN,ttkh.SONHA,ttkh.TENDUONG,phuong = (select tenphuong from CAPNUOCTANHOA.dbo.PHUONG where MAPHUONG = ttkh.PHUONG and MAQUAN = ttkh.QUAN)"
                                + " ,quan = (select tenquan from CAPNUOCTANHOA.dbo.QUAN where MAQUAN = ttkh.QUAN)"
                                + " ,DIENTHOAI = (select top 1 DienThoai from CAPNUOCTANHOA.dbo.SDT_DHN where DanhBo = ttkh.DANHBO order by CreateDate desc)"
                                + " ,ttkh.NGAYTHAY,ttkh.HIEUDH,ttkh.CODH,ttkh.SOTHANDH,vitri = ttkh.VITRIDHN,chisobao = '',loaibaothay = '',goclapdat =case when ttkh.ViTriDHN_Hop = 1 then N'Hộp' else '' end"
                                + " ,tt.tgbh_pin,tt.loai_pin,tt.so_phe_duyet,tt.cty_phe_duyet,tt.chong_nuoc,tt.cong_nghe,tt.ket_noi,hieuluc = 1"
                                + " ,tt.chu_ky_phat_song,tt.so_lan_gui_lai,ttkh.MADMA,tt.id_cty,tt.id_donvi"
                                + " ,gps_latitude = (select top 1 gps_latitude from CAPNUOCTANHOA.dbo.DanhBoGPS where DanhBo = ttkh.DANHBO),gps_lontitude = (select top 1 gps_lontitude from CAPNUOCTANHOA.dbo.DanhBoGPS where DanhBo = ttkh.DANHBO),gps_lontitude = '',hl = 1"
                                + " from(SELECT DanhBo = DHN_DANHBO, DIACHI, REPLACE(DHN_TODS, 'DHTM-', 'TH-') AS MADMA, HCT_HIEUDHNGAN AS HG, HCT_SOTHANGAN AS STGAN, HCT_CHISOGAN AS CSGa, HCT_CODHNGAN AS COGAN,"
                                + " CAST(HCT_NGAYGAN as date) AS NGAYGAN, CAST(HCT_NGAYKIEMDINH as date) AS KD,"
                                + " [DHN_SOTHAN] AS SOTHAN, HCT_SOTHANGO AS THANGO, [DHN_CHISO] AS CSB, HCT_CHISOGO AS CSG, HCT_CHISOGO - DHN_CHISO AS SS,[HCT_CREATEBY], [HCT_MODIFYBY]"
                                + " FROM CAPNUOCTANHOA.dbo.TB_THAYDHN WHERE DHN_LOAIBANGKE = 'DHTM' AND HCT_NGAYGAN IS NOT NULL"
                                + " ) dhn,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh,[sDHN].[dbo].[DHTM_THONGTIN] tt"
                                + " where dhn.DanhBo=ttkh.DANHBO and dhn.HG=tt.HIEU_DHTM and dhn.DanhBo='" + DanhBo + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var data = new
                        {
                            serial_number = dt.Rows[0]["serialnumber"].ToString(),
                            id_module = dt.Rows[0]["IDModule"].ToString(),
                            id_logger = dt.Rows[0]["IDLogger"].ToString(),
                            so_sim = dt.Rows[0]["sim"].ToString(),
                            danh_bo = dt.Rows[0]["DanhBo"].ToString(),
                            dot = dt.Rows[0]["dot_ds"].ToString(),
                            so_ds = dt.Rows[0]["so_ds"].ToString(),
                            mlt = dt.Rows[0]["MLT"].ToString(),
                            ho_ten_kh = dt.Rows[0]["HOTEN"].ToString(),
                            so_nha = dt.Rows[0]["SONHA"].ToString(),
                            duong = dt.Rows[0]["TENDUONG"].ToString(),
                            phuong = dt.Rows[0]["phuong"].ToString(),
                            quan = dt.Rows[0]["quan"].ToString(),
                            dien_thoai = dt.Rows[0]["DIENTHOAI"].ToString(),
                            ngay_gan = dt.Rows[0]["NGAYTHAY"].ToString(),
                            hieu = dt.Rows[0]["HIEUDH"].ToString(),
                            co = dt.Rows[0]["CODH"].ToString(),
                            so_than = dt.Rows[0]["SOTHANDH"].ToString(),
                            vi_tri = dt.Rows[0]["vitri"].ToString(),
                            chi_so_bao = dt.Rows[0]["chisobao"].ToString(),
                            loai_bao_thay = dt.Rows[0]["loaibaothay"].ToString(),
                            goc_lap_dat = dt.Rows[0]["goclapdat"].ToString(),
                            tgbh_pin = dt.Rows[0]["tgbh_pin"].ToString(),
                            loai_pin = dt.Rows[0]["loai_pin"].ToString(),
                            so_phe_duyet = dt.Rows[0]["so_phe_duyet"].ToString(),
                            cty_phe_duyet = dt.Rows[0]["cty_phe_duyet"].ToString(),
                            chong_nuoc = dt.Rows[0]["chong_nuoc"].ToString(),
                            cong_nghe = dt.Rows[0]["cong_nghe"].ToString(),
                            ket_noi = dt.Rows[0]["ket_noi"].ToString(),
                            chu_ky_phat_song = dt.Rows[0]["chu_ky_phat_song"].ToString(),
                            so_lan_gui_lai = dt.Rows[0]["so_lan_gui_lai"].ToString(),
                            //tan_suat_gui_lai = dt.Rows[0]["serialnumber"].ToString(),
                            dma = dt.Rows[0]["MADMA"].ToString(),
                            id_cty = dt.Rows[0]["id_cty"].ToString(),
                            id_donvi = dt.Rows[0]["id_donvi"].ToString(),
                            //ghi_chu = dt.Rows[0]["ghi_chu"].ToString(),
                            gps_latitude = dt.Rows[0]["gps_latitude"].ToString(),
                            gps_lontitude = dt.Rows[0]["gps_lontitude"].ToString(),
                            gps_altitude = dt.Rows[0]["gps_lontitude"].ToString(),
                            hieu_luc = dt.Rows[0]["hl"].ToString(),
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
                            if (obj["ketQua"] == 1)
                                return "1";
                            else
                                return "-1";
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

        [Route("suaDHN_TCT")]
        [HttpGet]
        public string suaDHN_TCT(string DanhBo, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.cheksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                           | SecurityProtocolType.Ssl3;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "api/DongHoNuoc/CapNhatDongHoNuoc");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select access_token from Configure").ToString();
                    DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("select serialnumber= CASE WHEN ttkh.HIEUDH='EMS' THEN ISNULL((select Serial_number from [sDHN].[dbo].[DHN_PHAMLAM] where DANHBO=dhn.DanhBo),0) ELSE ttkh.SOTHANDH END"
                                + " ,IDModule = ISNULL((select IDLogger from[sDHN].[dbo].[sDHN] where DanhBo = dhn.DanhBo),0)"
                                + " ,IDLogger = ISNULL((select IDLogger from[sDHN].[dbo].[sDHN] where DanhBo = dhn.DanhBo),0)"
                                + " ,sim = ''"
                                + " ,dhn.DanhBo,dot_ds = SUBSTRING(ttkh.LOTRINH, 1, 2),so_ds = SUBSTRING(ttkh.LOTRINH, 3, 2),MLT = ttkh.LOTRINH"
                                + " ,ttkh.HOTEN,ttkh.SONHA,ttkh.TENDUONG,phuong = (select tenphuong from CAPNUOCTANHOA.dbo.PHUONG where MAPHUONG = ttkh.PHUONG and MAQUAN = ttkh.QUAN)"
                                + " ,quan = (select tenquan from CAPNUOCTANHOA.dbo.QUAN where MAQUAN = ttkh.QUAN)"
                                + " ,DIENTHOAI = (select top 1 DienThoai from CAPNUOCTANHOA.dbo.SDT_DHN where DanhBo = ttkh.DANHBO order by CreateDate desc)"
                                + " ,ttkh.NGAYTHAY,ttkh.HIEUDH,ttkh.CODH,ttkh.SOTHANDH,vitri = ttkh.VITRIDHN,chisobao = '',loaibaothay = '',goclapdat =case when ttkh.ViTriDHN_Hop = 1 then N'Hộp' else '' end"
                                + " ,tt.tgbh_pin,tt.loai_pin,tt.so_phe_duyet,tt.cty_phe_duyet,tt.chong_nuoc,tt.cong_nghe,tt.ket_noi,hieuluc = 1"
                                + " ,tt.chu_ky_phat_song,tt.so_lan_gui_lai,ttkh.MADMA,tt.id_cty,tt.id_donvi"
                                + " ,gps_latitude = (select top 1 gps_latitude from CAPNUOCTANHOA.dbo.DanhBoGPS where DanhBo = ttkh.DANHBO),gps_lontitude = (select top 1 gps_lontitude from CAPNUOCTANHOA.dbo.DanhBoGPS where DanhBo = ttkh.DANHBO),gps_lontitude = '',hl = 1"
                                + " from(SELECT DanhBo = DHN_DANHBO, DIACHI, REPLACE(DHN_TODS, 'DHTM-', 'TH-') AS MADMA, HCT_HIEUDHNGAN AS HG, HCT_SOTHANGAN AS STGAN, HCT_CHISOGAN AS CSGa, HCT_CODHNGAN AS COGAN,"
                                + " CAST(HCT_NGAYGAN as date) AS NGAYGAN, CAST(HCT_NGAYKIEMDINH as date) AS KD,"
                                + " [DHN_SOTHAN] AS SOTHAN, HCT_SOTHANGO AS THANGO, [DHN_CHISO] AS CSB, HCT_CHISOGO AS CSG, HCT_CHISOGO - DHN_CHISO AS SS,[HCT_CREATEBY], [HCT_MODIFYBY]"
                                + " FROM CAPNUOCTANHOA.dbo.TB_THAYDHN WHERE DHN_LOAIBANGKE = 'DHTM' AND HCT_NGAYGAN IS NOT NULL"
                                + " ) dhn,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh,[sDHN].[dbo].[DHTM_THONGTIN] tt"
                                + " where dhn.DanhBo=ttkh.DANHBO and dhn.HG=tt.HIEU_DHTM and dhn.DanhBo='" + DanhBo + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var data = new
                        {
                            serial_number = dt.Rows[0]["serialnumber"].ToString(),
                            id_module = dt.Rows[0]["IDModule"].ToString(),
                            id_logger = dt.Rows[0]["IDLogger"].ToString(),
                            so_sim = dt.Rows[0]["sim"].ToString(),
                            danh_bo = dt.Rows[0]["DanhBo"].ToString(),
                            dot = dt.Rows[0]["dot_ds"].ToString(),
                            so_ds = dt.Rows[0]["so_ds"].ToString(),
                            mlt = dt.Rows[0]["MLT"].ToString(),
                            ho_ten_kh = dt.Rows[0]["HOTEN"].ToString(),
                            so_nha = dt.Rows[0]["SONHA"].ToString(),
                            duong = dt.Rows[0]["TENDUONG"].ToString(),
                            phuong = dt.Rows[0]["phuong"].ToString(),
                            quan = dt.Rows[0]["quan"].ToString(),
                            dien_thoai = dt.Rows[0]["DIENTHOAI"].ToString(),
                            ngay_gan = dt.Rows[0]["NGAYTHAY"].ToString(),
                            hieu = dt.Rows[0]["HIEUDH"].ToString(),
                            co = dt.Rows[0]["CODH"].ToString(),
                            so_than = dt.Rows[0]["SOTHANDH"].ToString(),
                            vi_tri = dt.Rows[0]["vitri"].ToString(),
                            chi_so_bao = dt.Rows[0]["chisobao"].ToString(),
                            loai_bao_thay = dt.Rows[0]["loaibaothay"].ToString(),
                            goc_lap_dat = dt.Rows[0]["goclapdat"].ToString(),
                            tgbh_pin = dt.Rows[0]["tgbh_pin"].ToString(),
                            loai_pin = dt.Rows[0]["loai_pin"].ToString(),
                            so_phe_duyet = dt.Rows[0]["so_phe_duyet"].ToString(),
                            cty_phe_duyet = dt.Rows[0]["cty_phe_duyet"].ToString(),
                            chong_nuoc = dt.Rows[0]["chong_nuoc"].ToString(),
                            cong_nghe = dt.Rows[0]["cong_nghe"].ToString(),
                            ket_noi = dt.Rows[0]["ket_noi"].ToString(),
                            chu_ky_phat_song = dt.Rows[0]["chu_ky_phat_song"].ToString(),
                            so_lan_gui_lai = dt.Rows[0]["so_lan_gui_lai"].ToString(),
                            //tan_suat_gui_lai = dt.Rows[0]["serialnumber"].ToString(),
                            dma = dt.Rows[0]["MADMA"].ToString(),
                            id_cty = dt.Rows[0]["id_cty"].ToString(),
                            id_donvi = dt.Rows[0]["id_donvi"].ToString(),
                            //ghi_chu = dt.Rows[0]["ghi_chu"].ToString(),
                            gps_latitude = dt.Rows[0]["gps_latitude"].ToString(),
                            gps_lontitude = dt.Rows[0]["gps_lontitude"].ToString(),
                            gps_altitude = dt.Rows[0]["gps_lontitude"].ToString(),
                            hieu_luc = dt.Rows[0]["hl"].ToString(),
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
                            if (obj["ketQua"] == 1)
                                return "1";
                            else
                                return "-1";
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

        [Route("getChiSo_TCT")]
        [HttpGet]
        public string getChiSo_TCT(string Time, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.cheksum)
                {
                    DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("SELECT DanhBo=DHN_DANHBO FROM CAPNUOCTANHOA.dbo.TB_THAYDHN WHERE DHN_LOAIBANGKE='DHTM' AND HCT_NGAYGAN IS NOT NULL and HCT_HIEUDHNGAN in (select a1.HIEU_DHTM from sDHN.dbo.DHTM_THONGTIN a1)");
                    foreach (DataRow item in dt.Rows)
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                               | SecurityProtocolType.Ssl3;
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "api/ChiSoNuoc/TraCuuChiSoNuoc?id_donVi=TH&danhBo=" + item["DanhBo"].ToString() + "&ngay=" + Time);
                        request.Method = "GET";
                        request.ContentType = "application/json";
                        request.Headers["Authorization"] = "Bearer " + _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select access_token from Configure").ToString();

                        HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                        if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                        {
                            StreamReader read = new StreamReader(respuesta.GetResponseStream());
                            string result = read.ReadToEnd();
                            read.Close();
                            respuesta.Close();
                            CGlobalVariable.jsSerializer.MaxJsonLength = Int32.MaxValue;
                            var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                            if (obj["ketQua"] == 1)
                            {
                                var lst1 = obj["duLieu"];
                                foreach (var itemC1 in lst1)
                                {
                                    var lst2 = CGlobalVariable.jsSerializer.Deserialize<dynamic>(itemC1["livedata"]);
                                    foreach (var itemC2 in lst2)
                                    {
                                        int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                                        if (itemC2["pin_yeu"] == true)
                                            flagCBPinYeu = 1;
                                        if (itemC2["ro_ri"] == true)
                                            flagCBRoRi = 1;
                                        if (itemC2["qua_dong"] == true)
                                            flagCBQuaDong = 1;
                                        if (itemC2["chay_nguoc"] == true)
                                            flagCBChayNguoc = 1;
                                        if (itemC2["co_nam_cham"] == true)
                                            flagCBNamCham = 1;
                                        if (itemC2["kho_ong"] == true)
                                            flagCBKhoOng = 1;
                                        if (itemC2["mo_hop"] == true)
                                            flagCBMoHop = 1;
                                        string LuuLuong = "NULL";
                                        //if (itemC2["Flow"] != null)
                                        //    LuuLuong = ((int)itemC2["Flow"]).ToString();
                                        string sql = "if not exists(select * from sDHN_LichSu_TCT where DanhBo='" + itemC2["danh_bo"] + "' and ThoiGianCapNhat=convert(datetime,'" + itemC2["thoi_gian_nhan"] + "'))"
                                            + " insert into sDHN_LichSu_TCT(DanhBo,ChiSo,Diff,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                            + ",Longitude,Latitude,Altitude,ChuKyGui"
                                            + ",ThoiGianCapNhat,Loai)"
                                                       + "values("
                                                       + "'" + itemC2["danh_bo"] + "'"
                                                       + "," + itemC2["chi_so_nuoc"]
                                                       + "," + itemC2["tieu_thu"]
                                                       + ",NULL" //+ item["Battery"]
                                                       + ",N'" + itemC2["thoi_gian_con_lai"] + "'"
                                                       + "," + LuuLuong
                                                       + ",N'" + itemC2["chat_luong_song"] + "'"
                                                       + "," + flagCBPinYeu
                                                       + "," + flagCBRoRi
                                                       + "," + flagCBQuaDong
                                                       + "," + flagCBChayNguoc
                                                       + "," + flagCBNamCham
                                                       + "," + flagCBKhoOng
                                                       + "," + flagCBMoHop
                                                       + ",NULL" //+ item["Longitude"]
                                                       + ",NULL" //+ item["Latitude"]
                                                       + ",NULL"
                                                       + ",NULL" //+ item["Interval"]
                                                       + ",'" + itemC2["thoi_gian_nhan"] + "',N'All')";
                                        _cDAL_sDHN.ExecuteNonQuery(sql);
                                    }
                                }
                            }
                        }
                        else
                        {
                            strResponse = "Error: " + respuesta.StatusCode;
                        }
                    }
                    strResponse = "Đã xử lý";
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

        [Route("getChiSo_Back_TCT")]
        [HttpGet]
        public string getChiSo_Back_TCT(string Time, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.cheksum)
                {
                    string[] datestr = Time.Split('-');
                    DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("select * from"
                                     + " (select DanhBo, IDNCC, SoLuong = (select COUNT(*) from sDHN_LichSu_TCT where CAST(ThoiGianCapNhat as date) = '" + datestr[0] + datestr[1] + datestr[2] + "' and DanhBo = sDHN_TCT.DanhBo) from sDHN_TCT"
                                     + " where Valid = 1)t1"
                                     + " where t1.SoLuong < 24");
                    foreach (DataRow item in dt.Rows)
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                               | SecurityProtocolType.Ssl3;
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "api/ChiSoNuoc/TraCuuChiSoNuoc?id_donVi=TH&danhBo=" + item["DanhBo"].ToString() + "&ngay=" + Time);
                        request.Method = "GET";
                        request.ContentType = "application/json";
                        request.Headers["Authorization"] = "Bearer " + _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select access_token from Configure").ToString();

                        HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                        if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                        {
                            StreamReader read = new StreamReader(respuesta.GetResponseStream());
                            string result = read.ReadToEnd();
                            read.Close();
                            respuesta.Close();
                            CGlobalVariable.jsSerializer.MaxJsonLength = Int32.MaxValue;
                            var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                            if (obj["ketQua"] == 1)
                            {
                                var lst1 = obj["duLieu"];
                                foreach (var itemC1 in lst1)
                                {
                                    var lst2 = CGlobalVariable.jsSerializer.Deserialize<dynamic>(itemC1["livedata"]);
                                    foreach (var itemC2 in lst2)
                                    {
                                        int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                                        if (itemC2["pin_yeu"] == true)
                                            flagCBPinYeu = 1;
                                        if (itemC2["ro_ri"] == true)
                                            flagCBRoRi = 1;
                                        if (itemC2["qua_dong"] == true)
                                            flagCBQuaDong = 1;
                                        if (itemC2["chay_nguoc"] == true)
                                            flagCBChayNguoc = 1;
                                        if (itemC2["co_nam_cham"] == true)
                                            flagCBNamCham = 1;
                                        if (itemC2["kho_ong"] == true)
                                            flagCBKhoOng = 1;
                                        if (itemC2["mo_hop"] == true)
                                            flagCBMoHop = 1;
                                        string LuuLuong = "NULL";
                                        //if (itemC2["Flow"] != null)
                                        //    LuuLuong = ((int)itemC2["Flow"]).ToString();
                                        string sql = "if not exists(select * from sDHN_LichSu_TCT where DanhBo='" + itemC2["danh_bo"] + "' and ThoiGianCapNhat=convert(datetime,'" + itemC2["thoi_gian_nhan"] + "'))"
                                            + " insert into sDHN_LichSu_TCT(DanhBo,ChiSo,Diff,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                            + ",Longitude,Latitude,Altitude,ChuKyGui"
                                            + ",ThoiGianCapNhat,Loai)"
                                                       + "values("
                                                       + "'" + itemC2["danh_bo"] + "'"
                                                       + "," + itemC2["chi_so_nuoc"]
                                                       + "," + itemC2["tieu_thu"]
                                                       + ",NULL" //+ item["Battery"]
                                                       + ",N'" + itemC2["thoi_gian_con_lai"] + "'"
                                                       + "," + LuuLuong
                                                       + ",N'" + itemC2["chat_luong_song"] + "'"
                                                       + "," + flagCBPinYeu
                                                       + "," + flagCBRoRi
                                                       + "," + flagCBQuaDong
                                                       + "," + flagCBChayNguoc
                                                       + "," + flagCBNamCham
                                                       + "," + flagCBKhoOng
                                                       + "," + flagCBMoHop
                                                       + ",NULL" //+ item["Longitude"]
                                                       + ",NULL" //+ item["Latitude"]
                                                       + ",NULL"
                                                       + ",NULL" //+ item["Interval"]
                                                       + ",'" + itemC2["thoi_gian_nhan"] + "',N'All')";
                                        _cDAL_sDHN.ExecuteNonQuery(sql);
                                    }
                                }
                            }
                        }
                        else
                        {
                            strResponse = "Error: " + respuesta.StatusCode;
                        }
                    }
                    strResponse = "Đã xử lý";
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

        [Route("getChiSo_All_TCT")]
        [HttpGet]
        public string getChiSo_All_TCT(string Time, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.cheksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                           | SecurityProtocolType.Ssl3;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "api/ChiSoNuoc/LayChiSoNuocTheoNgay?id_donVi=TH&tuNgay=" + Time + "&denNgay=" + Time);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select access_token from Configure").ToString();

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        CGlobalVariable.jsSerializer.MaxJsonLength = Int32.MaxValue;
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        if (obj["ketQua"] == 1)
                        {
                            var lst1 = obj["duLieu"];
                            foreach (var itemC1 in lst1)
                            {
                                var lst2 = CGlobalVariable.jsSerializer.Deserialize<dynamic>(itemC1["livedata"]);
                                foreach (var itemC2 in lst2)
                                {
                                    int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                                    if (itemC2["pin_yeu"] == true)
                                        flagCBPinYeu = 1;
                                    if (itemC2["ro_ri"] == true)
                                        flagCBRoRi = 1;
                                    if (itemC2["qua_dong"] == true)
                                        flagCBQuaDong = 1;
                                    if (itemC2["chay_nguoc"] == true)
                                        flagCBChayNguoc = 1;
                                    if (itemC2["co_nam_cham"] == true)
                                        flagCBNamCham = 1;
                                    if (itemC2["kho_ong"] == true)
                                        flagCBKhoOng = 1;
                                    if (itemC2["mo_hop"] == true)
                                        flagCBMoHop = 1;
                                    string LuuLuong = "NULL";
                                    //if (itemC2["Flow"] != null)
                                    //    LuuLuong = ((int)itemC2["Flow"]).ToString();
                                    string sql = "if not exists(select * from sDHN_LichSu_TCTA where DanhBo='" + itemC2["danh_bo"] + "' and ThoiGianCapNhat=convert(datetime,'" + itemC2["thoi_gian_nhan"] + "'))"
                                        + " insert into sDHN_LichSu_TCTA(DanhBo,ChiSo,Diff,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                        + ",Longitude,Latitude,Altitude,ChuKyGui"
                                        + ",ThoiGianCapNhat,Loai)"
                                                   + "values("
                                                   + "'" + itemC2["danh_bo"] + "'"
                                                   + "," + itemC2["chi_so_nuoc"]
                                                   + "," + itemC2["tieu_thu"]
                                                   + ",NULL" //+ item["Battery"]
                                                   + ",N'" + itemC2["thoi_gian_con_lai"] + "'"
                                                   + "," + LuuLuong
                                                   + ",N'" + itemC2["chat_luong_song"] + "'"
                                                   + "," + flagCBPinYeu
                                                   + "," + flagCBRoRi
                                                   + "," + flagCBQuaDong
                                                   + "," + flagCBChayNguoc
                                                   + "," + flagCBNamCham
                                                   + "," + flagCBKhoOng
                                                   + "," + flagCBMoHop
                                                   + ",NULL" //+ item["Longitude"]
                                                   + ",NULL" //+ item["Latitude"]
                                                   + ",NULL"
                                                   + ",NULL" //+ item["Interval"]
                                                   + ",'" + itemC2["thoi_gian_nhan"] + "',N'All')";
                                    _cDAL_sDHN.ExecuteNonQuery(sql);
                                }
                            }
                            strResponse = "Đã xử lý";
                        }
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

        //[Route("getVersion")]
        //[HttpGet]
        //public MResult getVersion()
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select Version from DeviceConfig").ToString();
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("updateUID")]
        //[HttpGet]
        //public MResult updateUID(string MaNV, string UID)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.success = _cDAL_sDHN.ExecuteNonQuery("update NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("dangNhap")]
        //[HttpGet]
        //public MResult dangNhap(string Username, string Password, string IDMobile, string UID)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        object MaNV = null;
        //        MaNV = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select MaND from NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");
        //        if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
        //            MaNV = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select MaND from NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and IDMobile='" + IDMobile + "' and An=0");

        //        if (MaNV == null || MaNV.ToString() == "")
        //        {
        //            result.message = "Sai mật khẩu hoặc IDMobile";
        //            result.success = false;
        //        }
        //        else
        //        {
        //            //xóa máy đăng nhập MaNV khác
        //            object MaNV_UID_Old = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");
        //            if (MaNV_UID_Old != null && (int)MaNV_UID_Old > 0)
        //                _cDAL_sDHN.ExecuteNonQuery("delete DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");

        //            //if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
        //            //{
        //            //    DataTable dt = _cDAL.ExecuteQuery_DataTable("select UID from TT_DeviceSigned where MaNV=" + MaNV);
        //            //    foreach (DataRow item in dt.Rows)
        //            //    {
        //            //        SendNotificationToClient("Thông Báo Đăng Xuất", "Hệ thống server gửi đăng xuất đến thiết bị", item["UID"].ToString(), "DangXuat", "DangXuat", "false", "");
        //            //        _cDAL.ExecuteNonQuery("delete TT_DeviceSigned where UID='" + item["UID"].ToString() + "'");
        //            //    }
        //            //}

        //            object MaNV_UID = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from DeviceSigned where MaNV='" + MaNV + "' and UID='" + UID + "'");
        //            if (MaNV_UID != null)
        //                if ((int)MaNV_UID == 0)
        //                    _cDAL_sDHN.ExecuteNonQuery("insert DeviceSigned(MaNV,UID,CreateDate)values(" + MaNV + ",'" + UID + "',getDate())");
        //                else
        //                    _cDAL_sDHN.ExecuteNonQuery("update DeviceSigned set ModifyDate=getdate() where MaNV=" + MaNV + " and UID='" + UID + "'");

        //            _cDAL_sDHN.ExecuteNonQuery("update NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);

        //            result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select TaiKhoan,MatKhau,MaND,HoTen,May,Admin,Doi,ToTruong,MaTo,DienThoai from NguoiDung where MaND=" + MaNV));
        //            result.success = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("dangXuat")]
        //[HttpGet]
        //public MResult DangXuat(string Username, string UID)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        //string MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and An=0").ToString();

        //        //_cDAL.ExecuteNonQuery("delete TT_DeviceSigned where MaNV=" + MaNV + " and UID='" + UID + "'");

        //        result.success = _cDAL_sDHN.ExecuteNonQuery("update NguoiDung set UID='' where TaiKhoan='" + Username + "'");
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("dangXuat_Person")]
        //[HttpGet]
        //public MResult dangXuat_Person(string Username, string UID)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        object MaNV = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select MaND from NguoiDung where TaiKhoan='" + Username + "' and An=0").ToString();

        //        if (MaNV != null)
        //            _cDAL_sDHN.ExecuteNonQuery("delete DeviceSigned where MaNV=" + MaNV + " and UID='" + UID + "'");

        //        result.success = _cDAL_sDHN.ExecuteNonQuery("update NguoiDung set UID='' where TaiKhoan='" + Username + "'");
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_To")]
        //[HttpGet]
        //public MResult getDS_To()
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select MaTo,TenTo,HanhThu from [To] where HanhThu=1"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_NhanVien_HanhThu")]
        //[HttpGet]
        //public MResult getDS_NhanVien_HanhThu()
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select MaND,HoTen,May,HanhThu,DongNuoc,MaTo,DienThoai,Zalo from NguoiDung where MaND!=0 and May is not null and An=0 order by STT asc"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_NhanVien")]
        //[HttpGet]
        //public MResult getDS_NhanVien()
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select MaND,HoTen,May,MaTo,DienThoai from NguoiDung where MaND!=0 and An=0 order by STT asc"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_NhanVien")]
        //[HttpGet]
        //public MResult getDS_NhanVien(string MaTo)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select MaND,HoTen,May,MaTo,DienThoai from NguoiDung where MaND!=0 and MaTo=" + MaTo + " and An=0 order by STT asc"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_Nam")]
        //[HttpGet]
        //public MResult getDS_Nam()
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select Nam=CAST(SUBSTRING(BillID,0,5)as int)"
        //                  + " from BillState"
        //                  + " group by SUBSTRING(BillID,0,5)"
        //                  + " order by Nam desc"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_ViTriDHN")]
        //[HttpGet]
        //public MResult getDS_ViTriDHN()
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_DHN.ExecuteQuery_DataTable("select KyHieu from ViTriDHN"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_Code")]
        //[HttpGet]
        //public MResult getDS_Code()
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select Code,MoTa from TTDHN order by stt asc"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_GiaNuoc")]
        //[HttpGet]
        //public MResult getDS_GiaNuoc()
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_ThuongVu.ExecuteQuery_DataTable("SELECT ID,Name,SHN,SHTM,SHVM1,SHVM2,SX,HCSN,KDDV,NgayTangGia=CONVERT(char(10),NgayTangGia,103),PhiBVMT,VAT,VAT2_Ky,VAT2 FROM GiaNuoc2"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_KhongTinhPBVMT")]
        //[HttpGet]
        //public MResult getDS_KhongTinhPBVMT()
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select DanhBo from DanhBoKPBVMT"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //private bool checkChot_BillState(string Nam, string Ky, string Dot)
        //{
        //    return bool.Parse(_cDAL_sDHN.ExecuteQuery_ReturnOneValue("select case when exists(select BillID from BillState where BillID='" + Nam + Ky + Dot + "' and izDS=1) then 'true' else 'false' end").ToString());
        //}

        //private bool checkXuLy(string ID)
        //{
        //    return bool.Parse(_cDAL_sDHN.ExecuteQuery_ReturnOneValue("select case when exists(select DocSoID from DocSo where DocSoID='" + ID + "' and StaCapNhat='1') then 'true' else 'false' end").ToString());
        //}

        //private bool checkChuBao(string ID)
        //{
        //    return bool.Parse(_cDAL_sDHN.ExecuteQuery_ReturnOneValue("select case when exists(select DocSoID from DocSo where DocSoID='" + ID + "' and ChuBao=1) then 'true' else 'false' end").ToString());
        //}

        //[Route("checkNgayDoc")]
        //[HttpGet]
        //public bool checkNgayDoc(string Nam, string Ky, string Dot, string May)
        //{
        //    if (bool.Parse(_cDAL_sDHN.ExecuteQuery_ReturnOneValue("select case when exists(select Nam from DocSoTruoc where Nam=" + Nam + " and Ky='" + Ky + "' and Dot='" + Dot + "' and May='" + May + "') then 'true' else 'false' end").ToString()) == true)
        //        return true;
        //    else
        //        return bool.Parse(_cDAL_sDHN.ExecuteQuery_ReturnOneValue("select case when exists(select NgayDoc from Lich_DocSo ds,Lich_DocSo_ChiTiet dsct where ds.Nam=" + Nam + " and ds.Ky=" + Ky + " and dsct.IDDot=" + Dot + " and ((dsct.NgayDoc=CAST(DATEADD(day,1,GETDATE()) as date) and CONVERT(varchar(10),GETDATE(),108)>='17:00:00') or dsct.NgayDoc<=CAST(GETDATE() as date)) and ds.ID=dsct.IDDocSo) then 'true' else 'false' end").ToString());
        //}

        ////đọc số
        //[Route("getDS_DocSo")]
        //[HttpGet]
        //public MResult getDS_DocSo(string Nam, string Ky, string Dot, string May)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        string sql = "DECLARE @LastNamKy INT;"
        //                   + " declare @Nam int"
        //                   + " declare @Ky char(2)"
        //                   + " declare @Dot char(2)"
        //                   + " declare @May char(2)"
        //                   + " set @Nam=" + Nam
        //                   + " set @Ky='" + Ky + "'"
        //                   + " set @Dot='" + Dot + "'"
        //                   + " set @May='" + May + "'"
        //                   + " SET @LastNamKy = @Nam * 12  + @Ky;"
        //                   + " IF (OBJECT_ID('tempdb.dbo.#ChiSo', 'U') IS NOT NULL) DROP TABLE #ChiSo;"
        //                   + " SELECT DanhBa, MAX([ChiSo0]) AS [ChiSo0], MAX([ChiSo1]) AS [ChiSo1], MAX([ChiSo2]) AS [ChiSo2], MAX([Code0]) AS [Code0],"
        //                   + "     MAX([Code1]) AS [Code1], MAX([Code2]) AS [Code2], MAX([TieuThu0]) AS [TieuThu0], MAX([TieuThu1]) AS [TieuThu1],"
        //                   + "     MAX([TieuThu2]) AS [TieuThu2]"
        //                   + "     INTO #ChiSo"
        //                   + "     FROM ("
        //                   + "         SELECT DanhBa, 'ChiSo'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS ChiSoKy, 'Code'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS CodeKy,"
        //                   + "             'TieuThu'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS TieuThuKy, [CSCu], [CodeCu], [TieuThuCu]"
        //                   + "             FROM [DocSoTH].[dbo].[DocSo]"
        //                   + "             WHERE @LastNamKy-Nam*12-Ky between 0 and 2 and (PhanMay=@May or May=@May)) src"
        //                   + "     PIVOT (MAX([CSCu]) FOR ChiSoKy IN ([ChiSo0],[ChiSo1],[ChiSo2])) piv_cs"
        //                   + "     PIVOT (MAX([CodeCu]) FOR CodeKy IN ([Code0],[Code1],[Code2])) piv_code"
        //                   + "     PIVOT (MAX([TieuThuCu]) FOR TieuThuKy IN ([TieuThu0],[TieuThu1],[TieuThu2])) piv_tt"
        //                   + "     GROUP BY DanhBa;"
        //                   + "     with sdt as("
        //                   + "        select g1.DanhBo"
        //                   + "        , stuff(("
        //                   + "            select ' | ' + g.DienThoai+' '+g.HoTen"
        //                   + "            from CAPNUOCTANHOA.dbo.SDT_DHN g"
        //                   + "            where g.DanhBo = g1.DanhBo and SoChinh=1"
        //                   + "            order by CreateDate desc"
        //                   + "            for xml path('')"
        //                   + "        ),1,2,'') as DienThoai"
        //                   + "        from CAPNUOCTANHOA.dbo.SDT_DHN g1"
        //                   + "        group by g1.DanhBo)"
        //                   + " select ds.DocSoID,MLT=kh.LOTRINH,DanhBo=ds.DanhBa,HoTen=kh.HOTEN,SoNha=kh.SONHA,TenDuong=kh.TENDUONG,ds.Nam,ds.Ky,ds.Dot,ds.PhanMay"
        //                   + "                          ,Hieu=kh.HIEUDH,Co=kh.CODH,SoThan=kh.SOTHANDH,ViTri=VITRIDHN,ViTriNgoai=ViTriDHN_Ngoai,ViTriHop=ViTriDHN_Hop,bd.SH,bd.SX,bd.DV,HCSN=bd.HC,ds.TienNuoc,ThueGTGT=ds.Thue,PhiBVMT=ds.BVMT,PhiBVMT_Thue=ds.BVMT_Thue,TongCong=ds.TongTien"
        //                   + "                          ,DiaChi=(select top 1 DiaChi=case when SO is null then DUONG else case when DUONG is null then SO else SO+' '+DUONG end end from server9.HOADON_TA.dbo.HOADON where DanhBa=ds.DanhBa order by ID_HOADON desc)"
        //                   + "                          ,GiaBieu=bd.GB,DinhMuc=bd.DM,DinhMucHN=bd.DMHN,CSMoi,CodeMoi,TieuThuMoi,ds.TBTT,TuNgay=CONVERT(varchar(10),TuNgay,103),DenNgay=CONVERT(varchar(10),DenNgay,103),cs.*"
        //                   + "                          ,kh.Gieng,kh.KhoaTu,kh.AmSau,kh.XayDung,kh.DutChi_Goc,kh.DutChi_Than,kh.NgapNuoc,kh.KetTuong,kh.LapKhoaGoc,kh.BeHBV,kh.BeNapMatNapHBV,kh.MauSacChiGoc,ds.ChuBao,DienThoai=sdt.DienThoai,kh.GhiChu"
        //                   + "                          ,NgayThuTien=(select CONVERT(varchar(10),NgayThuTien,103) from Lich_DocSo ds,Lich_DocSo_ChiTiet dsct where ds.ID=dsct.IDDocSo and ds.Nam=@Nam and ds.Ky=@Ky and dsct.IDDot=@Dot)"
        //                   + "                          ,TinhTrang=(select"
        //                   + "                             case when exists (select top 1 MaKQDN from server9.HOADON_TA.dbo.TT_KQDongNuoc where MoNuoc=0 and TroNgaiMN=0 and DanhBo=ds.DanhBa order by NgayDN desc)"
        //                   + "                             then (select top 1 N'Thu Tiền đóng nước: '+CONVERT(varchar(10),NgayDN,103)+' '+CONVERT(varchar(10),NgayDN,108) from server9.HOADON_TA.dbo.TT_KQDongNuoc where MoNuoc=0 and TroNgaiMN=0 and DanhBo=ds.DanhBa order by NgayDN desc)"
        //                   + "                             else ''"
        //                   + "                             end)"
        //                   + "                          ,CuaHangThuHo=(select CuaHangThuHo1+CHAR(10)+case when CuaHangThuHo2 is null or CuaHangThuHo2=CuaHangThuHo1 then '' else CuaHangThuHo2 end from server9.HOADON_TA.dbo.TT_DichVuThu_DanhBo_CuaHang where DanhBo=ds.DanhBa)"
        //                   + "                          from DocSo ds left join server8.CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG kh on ds.DanhBa=kh.DANHBO"
        //                   + "                          left join BienDong bd on ds.DocSoID=bd.BienDongID"
        //                   + "                          left join #ChiSo cs on ds.DanhBa=cs.DanhBa"
        //                   + "                          left join sdt on sdt.DanhBo=ds.DanhBa"
        //                   + "                          where ds.Nam=@Nam and ds.Ky=@Ky and ds.Dot=@Dot and ds.PhanMay=@May order by ds.MLT1 asc";
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable(sql));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;

        //}

        //[Route("getDS_DocSo_Ton")]
        //[HttpGet]
        //public MResult getDS_DocSo_Ton(string Nam, string Ky, string Dot, string May)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select DocSoID,CodeMoi,CSMoi from DocSo where Nam=" + Nam + " and Ky='" + Ky + "' and Dot='" + Dot + "' and PhanMay='" + May + "' order by MLT1 asc"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_Hinh_Ton")]
        //[HttpGet]
        //public MResult getDS_Hinh_Ton(string Nam, string Ky, string Dot, string May)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        string sql = "select DocSoID,GhiHinh=CAST(0 as bit) from DocSo where Nam=" + Nam + " and Ky='" + Ky + "' and Dot='" + Dot + "' and PhanMay='" + May + "' order by MLT1 asc";
        //        DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable(sql);
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //            if (checkExists_HinhDHN_Func(dt.Rows[0]["DocSoID"].ToString()) == true)
        //            {
        //                dt.Rows[0]["GhiHinh"] = 1;
        //            }
        //        result.message = DataTableToJSON(dt);
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;
        //}

        //[Route("getDS_HoaDonTon")]
        //[HttpGet]
        //public MResult getDS_HoaDonTon(string Nam, string Ky, string Dot, string May)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        string sql = "select MaHD=hd.ID_HOADON,DanhBo=hd.DANHBA,KyHD=(RIGHT('0' + CAST(hd.Ky AS VARCHAR(2)), 2)+'/'+convert(varchar(4),hd.NAM))"
        //            + " 	,hd.GiaBan,ThueGTGT=hd.THUE,PhiBVMT=hd.PHI,PhiBVMT_Thue=case when hd.ThueGTGT_TDVTN is null then 0 else hd.ThueGTGT_TDVTN end,hd.TongCong"
        //            + " 	from HOADON hd,server8.DocSoTH.dbo.DocSo ds"
        //            + " 	where ds.Nam=" + Nam + " and ds.Ky='" + Ky + "' and ds.Dot='" + Dot + "' and ds.PhanMay='" + May + "'"
        //            + " 	and hd.DANHBA=ds.DanhBa and NGAYGIAITRACH is null"
        //            + " 	and ID_HOADON not in (select MaHD from TT_DichVuThu)"
        //            + " 	and ID_HOADON not in (select MaHD from TT_TraGop)"
        //            + " 	and ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where CodeF2=1 and NGAYGIAITRACH is null and ID_HOADON=FK_HOADON)"
        //            + " 	and not exists(select * from TT_ChanThuHo where Nam=hd.NAM and Ky=hd.KY and Dot=hd.DOT)--chặn thu hộ"
        //         //+ " 	and ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and UpdatedHDDT=0)"
        //         + "     and ID_HOADON not in (select distinct FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and SoHoaDonMoi is null and SoPhieu is null)"
        //            //+ "     and ID_HOADON not in (select distinct FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and SoHoaDonMoi is null and (SoPhieu is null or CAST(Ngay_DC as date)<'20220701' or (NAM<2022 or (NAM=2022 and KY<5))))"
        //            + " 	order by ds.MLT1,ds.DanhBa,hd.ID_HOADON";
        //        //string sql = "EXEC [dbo].[spGetDSHoaDonTon_DocSo]	@Nam = " + Nam + ",@Ky = N'" + Ky + "',@Dot = N'" + Dot + "',@May = N'" + May + "'";
        //        result.message = DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //        result.success = false;
        //    }
        //    return result;

        //}

        //[Route("getDS_LichSu_DocSo")]
        //[HttpGet]
        //public MResult getDS_LichSu_DocSo(string DanhBo)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        string sql = "select top 12 Ky=ds.Ky+'/'+CONVERT(char(4),ds.Nam),CodeMoi,CSMoi,TieuThuMoi,GiaBieu=bd.GB,DinhMuc=bd.DM,DinhMucHN=bd.DMHN"
        //            + ",TuNgay=CONVERT(varchar(10),TuNgay,103),DenNgay=CONVERT(varchar(10),DenNgay,103),ds.TienNuoc,ThueGTGT=ds.Thue,PhiBVMT=ds.BVMT"
        //            + ",PhiBVMT_Thue=case when ds.BVMT_Thue is null then 0 else ds.BVMT_Thue end,TongCong=ds.TongTien"
        //            + " from DocSo ds,BienDong bd where ds.DanhBa='" + DanhBo.Replace(" ", "") + "' and ds.DocSoID=bd.BienDongID order by ds.DocSoID desc";
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable(sql));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        ////đọc số
        //private bool tinhCodeTieuThu(string DocSoID, string Code, int CSM, out int TieuThu, out int TienNuoc, out int ThueGTGT, out int TDVTN, out int ThueTDVTN)
        //{
        //    try
        //    {
        //        TienNuoc = ThueGTGT = TDVTN = ThueTDVTN = 0;
        //        string sql = "EXEC [dbo].[spTinhTieuThu]"
        //           + " @DANHBO = N'" + DocSoID.Substring(6, 11) + "',"
        //           + " @KY = " + DocSoID.Substring(4, 2) + ","
        //           + " @NAM = " + DocSoID.Substring(0, 4) + ","
        //           + " @CODE = N'" + Code + "',"
        //           + " @CSMOI = " + CSM;
        //        object result = _cDAL_sDHN.ExecuteQuery_ReturnOneValue(sql);
        //        if (result != null)
        //            TieuThu = (int)result;
        //        else
        //            TieuThu = -1;
        //        if (TieuThu < 0)
        //            return true;
        //        DataTable dtDocSo = _cDAL_sDHN.ExecuteQuery_DataTable("select * from DocSo where DocSoID='" + DocSoID + "'");
        //        DataTable dtBienDong = _cDAL_sDHN.ExecuteQuery_DataTable("select * from BienDong where BienDongID='" + DocSoID + "'");
        //        if (dtDocSo != null && dtDocSo.Rows.Count > 0 && dtBienDong != null && dtBienDong.Rows.Count > 0)
        //        {
        //            int TienNuocNamCu = 0, TienNuocNamMoi = 0, PhiBVMTNamCu = 0, PhiBVMTNamMoi = 0, TieuThu_DieuChinhGia = 0;
        //            string ChiTietA = "", ChiTietB = "", ChiTietPhiBVMTA = "", ChiTietPhiBVMTB = "";
        //            TinhTienNuoc(false, false, false, 0, dtBienDong.Rows[0]["DanhBa"].ToString(), int.Parse(dtBienDong.Rows[0]["Ky"].ToString()), int.Parse(dtBienDong.Rows[0]["Nam"].ToString()), DateTime.Parse(dtDocSo.Rows[0]["TuNgay"].ToString()), DateTime.Parse(dtDocSo.Rows[0]["DenNgay"].ToString())
        //                 , int.Parse(dtBienDong.Rows[0]["GB"].ToString()), int.Parse(dtBienDong.Rows[0]["SH"].ToString()), int.Parse(dtBienDong.Rows[0]["SX"].ToString()), int.Parse(dtBienDong.Rows[0]["DV"].ToString()), int.Parse(dtBienDong.Rows[0]["HC"].ToString())
        //                 , int.Parse(dtBienDong.Rows[0]["DM"].ToString()), int.Parse(dtBienDong.Rows[0]["DMHN"].ToString()), TieuThu, ref TienNuocNamCu, ref ChiTietA, ref TienNuocNamMoi, ref ChiTietB, ref TieuThu_DieuChinhGia, ref PhiBVMTNamCu, ref ChiTietPhiBVMTA, ref PhiBVMTNamMoi, ref ChiTietPhiBVMTB, ref TienNuoc, ref ThueGTGT, ref TDVTN, ref ThueTDVTN);
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void updateChiTiet(ref string main_value, string update_value)
        //{
        //    if (main_value == "")
        //        main_value = update_value;
        //    else
        //        main_value += "\r\n" + update_value;
        //}

        //private bool HasValue(double value)
        //{
        //    return !Double.IsNaN(value) && !Double.IsInfinity(value);
        //}

        //private const int _GiamTienNuoc = 10;

        //private int TinhTienNuoc(bool DieuChinhGia, int GiaDieuChinh, List<int> lstGiaNuoc, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, out string ChiTiet, out int TieuThu_DieuChinhGia, out int PhiBVMT, out string ChiTietPhiBVMT)
        //{
        //    try
        //    {
        //        string _chiTiet = "", _chiTietPhiBVMT = "";
        //        int DinhMuc = TongDinhMuc - DinhMucHN, _SH = 0, _SX = 0, _DV = 0, _HCSN = 0;
        //        TieuThu_DieuChinhGia = 0;
        //        ///Table GiaNuoc được thiết lập theo bảng giá nước
        //        ///1. Đến 4m3/người/tháng
        //        ///2. Trên 4m3 đến 6m3/người/tháng
        //        ///3. Trên 6m3/người/tháng
        //        ///4. Đơn vị sản xuất
        //        ///5. Cơ quan, đoàn thể HCSN
        //        ///6. Đơn vị kinh doanh, dịch vụ
        //        ///7. Hộ nghèo, cận nghèo
        //        ///List bắt đầu từ phần tử thứ 0
        //        int TongTien = 0, TongTienPhiBVMT = 0;
        //        switch (GiaBieu)
        //        {
        //            ///TƯ GIA
        //            case 10:
        //                DinhMucHN = TongDinhMuc;
        //                if (TieuThu <= DinhMucHN)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[6];
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                    if (TieuThu - DinhMucHN <= Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero))
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + ((TieuThu - DinhMucHN) * lstGiaNuoc[1]);
        //                        TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)
        //                                    + ((TieuThu - DinhMucHN) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if ((TieuThu - DinhMucHN) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + ((int)Math.Round((double)DinhMuc / 2) * lstGiaNuoc[1])
        //                                    + ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((int)Math.Round((double)DinhMuc / 2) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if ((int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                else
        //                {
        //                    TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                + ((TieuThu - DinhMucHN) * GiaDieuChinh);
        //                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                               + ((TieuThu - DinhMucHN) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (DinhMucHN > 0)
        //                    {
        //                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if ((TieuThu - DinhMucHN) > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
        //                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if (lstGiaNuoc[6] == GiaDieuChinh)
        //                        TieuThu_DieuChinhGia = TieuThu;
        //                    else
        //                        TieuThu_DieuChinhGia = TieuThu - DinhMucHN;
        //                }
        //                break;
        //            case 11:
        //            case 21:///SH thuần túy
        //                if (TieuThu <= DinhMucHN + DinhMuc)
        //                {
        //                    double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                    if (HasValue(TyLe) == false)
        //                        TyLe = 0;
        //                    int TieuThuHN = 0, TieuThuDC = 0;
        //                    TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
        //                    TieuThuDC = TieuThu - TieuThuHN;
        //                    TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                + (TieuThuDC * lstGiaNuoc[0]);
        //                    TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (TieuThuHN > 0)
        //                    {
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (TieuThuDC > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                    if (TieuThu - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                    + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                else
        //                {
        //                    TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                + (DinhMuc * lstGiaNuoc[0])
        //                                + ((TieuThu - DinhMuc) * GiaDieuChinh);
        //                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                + ((TieuThu - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (DinhMucHN > 0)
        //                    {
        //                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (DinhMuc > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
        //                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if (lstGiaNuoc[0] == GiaDieuChinh)
        //                        TieuThu_DieuChinhGia = TieuThu;
        //                    else
        //                        TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
        //                }
        //                break;
        //            case 12:
        //            case 22:
        //            case 32:
        //            case 42:///SX thuần túy
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[3];
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * GiaDieuChinh;
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 13:
        //            case 23:
        //            case 33:
        //            case 43:///DV thuần túy
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[5];
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * GiaDieuChinh;
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 14:
        //            case 24:///SH + SX
        //                    ///Nếu không có tỉ lệ
        //                if (TyLeSH == 0 && TyLeSX == 0)
        //                {
        //                    if (TieuThu <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        if (HasValue(TyLe) == false)
        //                            TyLe = 0;
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = TieuThu - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (TieuThuHN > 0)
        //                        {
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (TieuThuDC > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[3]);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((TieuThu - DinhMuc) * GiaDieuChinh);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                   + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                   + ((TieuThu - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if (lstGiaNuoc[0] == GiaDieuChinh)
        //                            TieuThu_DieuChinhGia = TieuThu;
        //                        else
        //                            TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
        //                    }
        //                }
        //                else
        //                ///Nếu có tỉ lệ SH + SX
        //                {
        //                    //int _SH = 0, _SX = 0;
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    _SX = TieuThu - _SH;
        //                    if (_SH <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        if (HasValue(TyLe) == false)
        //                            TyLe = 0;
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = _SH - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                  + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (TieuThuHN > 0)
        //                        {
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (TieuThuDC > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                        if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + (DinhMuc * lstGiaNuoc[0])
        //                                        + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
        //                            TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            if (DinhMucHN > 0)
        //                            {
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            }
        //                            if (DinhMuc > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + (DinhMuc * lstGiaNuoc[0])
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                        + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                            TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            if (DinhMucHN > 0)
        //                            {
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            }
        //                            if (DinhMuc > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                        }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if (lstGiaNuoc[0] == GiaDieuChinh)
        //                            TieuThu_DieuChinhGia = _SH;
        //                        else
        //                            TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                    }
        //                    TongTien += _SX * lstGiaNuoc[3];
        //                    TongTienPhiBVMT += _SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (_SX > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                break;
        //            case 15:
        //            case 25:///SH + DV
        //                    ///Nếu không có tỉ lệ
        //                if (TyLeSH == 0 && TyLeDV == 0)
        //                {
        //                    if (TieuThu <= DinhMucHN + DinhMuc)
        //                    {
        //                        //double TyLe = Math.Round((double)DinhMucHN / (DinhMucHN + DinhMuc), 2);
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        if (HasValue(TyLe) == false)
        //                            TyLe = 0;
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = TieuThu - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (TieuThuHN > 0)
        //                        {
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (TieuThuDC > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[5]);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((TieuThu - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }

        //                        if (lstGiaNuoc[0] == GiaDieuChinh)
        //                            TieuThu_DieuChinhGia = TieuThu;
        //                        else
        //                            TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
        //                    }
        //                }
        //                else
        //                ///Nếu có tỉ lệ SH + DV
        //                {
        //                    //int _SH = 0, _DV = 0;
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _SH;
        //                    if (_SH <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        if (HasValue(TyLe) == false)
        //                            TyLe = 0;
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = _SH - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (TieuThuHN > 0)
        //                        {
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (TieuThuDC > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                        if (_SH - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + (DinhMuc * lstGiaNuoc[0])
        //                                        + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
        //                            TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            if (DinhMucHN > 0)
        //                            {
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            }
        //                            if (DinhMuc > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + (DinhMuc * lstGiaNuoc[0])
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                        + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                            TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            if (DinhMucHN > 0)
        //                            {
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            }
        //                            if (DinhMuc > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                        }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }

        //                        if (lstGiaNuoc[0] == GiaDieuChinh)
        //                            TieuThu_DieuChinhGia = _SH;
        //                        else
        //                            TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                    }
        //                    TongTien += _DV * lstGiaNuoc[5];
        //                    TongTienPhiBVMT += _DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (_DV > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                break;
        //            case 16:
        //            case 26:///SH + SX + DV
        //                    ///Nếu chỉ có tỉ lệ SX + DV mà không có tỉ lệ SH, không xét Định Mức
        //                if (TyLeSX != 0 && TyLeDV != 0 && TyLeSH == 0)
        //                {
        //                    if (TyLeSX != 0)
        //                        _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _SX;
        //                    TongTien = (_SX * lstGiaNuoc[3])
        //                                + (_DV * lstGiaNuoc[5]);
        //                    TongTienPhiBVMT = (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (_SX > 0)
        //                    {
        //                        _chiTiet = _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
        //                        _chiTietPhiBVMT = _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (_DV > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                else
        //                ///Nếu có đủ 3 tỉ lệ SH + SX + DV
        //                {
        //                    //int _SH = 0, _SX = 0, _DV = 0;
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TyLeSX != 0)
        //                        _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _SH - _SX;
        //                    if (_SH <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        if (HasValue(TyLe) == false)
        //                            TyLe = 0;
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = _SH - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (TieuThuHN > 0)
        //                        {
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (TieuThuDC > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                        if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + (DinhMuc * lstGiaNuoc[0])
        //                                        + ((_SH - DinhMuc) * lstGiaNuoc[1]);
        //                            TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((_SH - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            if (DinhMucHN > 0)
        //                            {
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            }
        //                            if (DinhMuc > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + (DinhMuc * lstGiaNuoc[0])
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                        + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                            TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            if (DinhMucHN > 0)
        //                            {
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            }
        //                            if (DinhMuc > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                        }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }

        //                        if (lstGiaNuoc[0] == GiaDieuChinh)
        //                            TieuThu_DieuChinhGia = _SH;
        //                        else
        //                            TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                    }
        //                    TongTien += (_SX * lstGiaNuoc[3])
        //                                + (_DV * lstGiaNuoc[5]);
        //                    TongTienPhiBVMT += (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (_SX > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if (_DV > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                break;
        //            case 17:
        //            case 27:///SH ĐB
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[0];
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * GiaDieuChinh;
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 18:
        //            case 28:
        //            case 38:///SH + HCSN
        //                    ///Nếu không có tỉ lệ
        //                if (TyLeSH == 0 && TyLeHCSN == 0)
        //                {
        //                    if (TieuThu <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        if (HasValue(TyLe) == false)
        //                            TyLe = 0;
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = TieuThu - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                   + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (TieuThuHN > 0)
        //                        {
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (TieuThuDC > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((TieuThu - DinhMuc) * lstGiaNuoc[4]);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((TieuThu - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((TieuThu - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }

        //                        if (lstGiaNuoc[0] == GiaDieuChinh)
        //                            TieuThu_DieuChinhGia = TieuThu;
        //                        else
        //                            TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
        //                    }
        //                }
        //                else
        //                ///Nếu có tỉ lệ SH + HCSN
        //                {
        //                    //int _SH = 0, _HCSN = 0;
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    _HCSN = TieuThu - _SH;
        //                    if (_SH <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        if (HasValue(TyLe) == false)
        //                            TyLe = 0;
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = _SH - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (TieuThuHN > 0)
        //                        {
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (TieuThuDC > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                        if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + (DinhMuc * lstGiaNuoc[0])
        //                                        + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
        //                            TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            if (DinhMucHN > 0)
        //                            {
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            }
        //                            if (DinhMuc > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + (DinhMuc * lstGiaNuoc[0])
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                        + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                            TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            if (DinhMucHN > 0)
        //                            {
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                            }
        //                            if (DinhMuc > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                            if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                            {
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                                updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                            }
        //                        }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if (lstGiaNuoc[0] == GiaDieuChinh)
        //                            TieuThu_DieuChinhGia = _SH;
        //                        else
        //                            TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                    }
        //                    TongTien += _HCSN * lstGiaNuoc[4];
        //                    TongTienPhiBVMT += _HCSN * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (_HCSN > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                break;
        //            case 19:
        //            case 29:
        //            case 39:///SH + HCSN + SX + DV
        //                //int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
        //                if (TyLeSH != 0)
        //                    _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeHCSN != 0)
        //                    _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeSX != 0)
        //                    _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                _DV = TieuThu - _SH - _HCSN - _SX;
        //                if (_SH <= DinhMucHN + DinhMuc)
        //                {
        //                    double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                    if (HasValue(TyLe) == false)
        //                        TyLe = 0;
        //                    int TieuThuHN = 0, TieuThuDC = 0;
        //                    TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                    TieuThuDC = _SH - TieuThuHN;
        //                    TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                + (TieuThuDC * lstGiaNuoc[0]);
        //                    TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (TieuThuHN > 0)
        //                    {
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (TieuThuDC > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                           + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                           + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                else
        //                {
        //                    TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                + (DinhMuc * lstGiaNuoc[0])
        //                                + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (DinhMucHN > 0)
        //                    {
        //                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (DinhMuc > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
        //                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if (lstGiaNuoc[0] == GiaDieuChinh)
        //                        TieuThu_DieuChinhGia = _SH;
        //                    else
        //                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                }
        //                TongTien += (_HCSN * lstGiaNuoc[4])
        //                            + (_SX * lstGiaNuoc[3])
        //                            + (_DV * lstGiaNuoc[5]);
        //                TongTienPhiBVMT += (_HCSN * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                if (_HCSN > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                if (_SX > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                if (_DV > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                break;
        //            ///CƠ QUAN
        //            case 31:///SHVM thuần túy
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[4];
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * GiaDieuChinh;
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 34:///HCSN + SX
        //                    ///Nếu không có tỉ lệ
        //                if (TyLeHCSN == 0 && TyLeSX == 0)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[3];
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                ///Nếu có tỉ lệ
        //                {
        //                    //int _HCSN = 0, _SX = 0;
        //                    if (TyLeHCSN != 0)
        //                        _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                    _SX = TieuThu - _HCSN;

        //                    TongTien = (_HCSN * lstGiaNuoc[4])
        //                                + (_SX * lstGiaNuoc[3]);
        //                    TongTienPhiBVMT = (_HCSN * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (_HCSN > 0)
        //                    {
        //                        _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
        //                        _chiTietPhiBVMT = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (_SX > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                break;
        //            case 35:///HCSN + DV
        //                    ///Nếu không có tỉ lệ
        //                if (TyLeHCSN == 0 && TyLeDV == 0)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[5];
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                ///Nếu có tỉ lệ
        //                {
        //                    //int _HCSN = 0, _DV = 0;
        //                    if (TyLeHCSN != 0)
        //                        _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _HCSN;
        //                    TongTien = (_HCSN * lstGiaNuoc[4])
        //                                + (_DV * lstGiaNuoc[5]);
        //                    TongTienPhiBVMT = (_HCSN * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (_HCSN > 0)
        //                    {
        //                        _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
        //                        _chiTietPhiBVMT = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (_DV > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                break;
        //            case 36:///HCSN + SX + DV
        //                {
        //                    if (TyLeHCSN != 0)
        //                        _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TyLeSX != 0)
        //                        _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _HCSN - _SX;
        //                    TongTien = (_HCSN * lstGiaNuoc[4])
        //                                + (_SX * lstGiaNuoc[3])
        //                                + (_DV * lstGiaNuoc[5]);
        //                    TongTienPhiBVMT = (_HCSN * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (_HCSN > 0)
        //                    {
        //                        _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
        //                        _chiTietPhiBVMT = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (_SX > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if (_DV > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                break;
        //            ///NƯỚC NGOÀI
        //            case 41:///SHVM thuần túy
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[2];
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * GiaDieuChinh;
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 44:///SH + SX
        //                {
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    _SX = TieuThu - _SH;
        //                    TongTien = (_SH * lstGiaNuoc[2])
        //                                + (_SX * lstGiaNuoc[3]);
        //                    TongTienPhiBVMT = (_SH * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (_SH > 0)
        //                    {
        //                        _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
        //                        _chiTietPhiBVMT = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (_SX > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                break;
        //            case 45:///SH + DV
        //                if (TyLeSH != 0)
        //                    _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                _DV = TieuThu - _SH;
        //                TongTien = (_SH * lstGiaNuoc[2])
        //                            + (_DV * lstGiaNuoc[5]);
        //                TongTienPhiBVMT = (_SH * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                if (_SH > 0)
        //                {
        //                    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
        //                    _chiTietPhiBVMT = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                }
        //                if (_DV > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                break;
        //            case 46:///SH + SX + DV
        //                {
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TyLeSX != 0)
        //                        _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _SH - _SX;
        //                    TongTien = (_SH * lstGiaNuoc[2])
        //                                + (_SX * lstGiaNuoc[3])
        //                                + (_DV * lstGiaNuoc[5]);
        //                    TongTienPhiBVMT = (_SH * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (_SH > 0)
        //                    {
        //                        _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
        //                        _chiTietPhiBVMT = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (_SX > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if (_DV > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                        updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                break;
        //            ///BÁN SỈ
        //            case 51:///sỉ khu dân cư - Giảm % tiền nước cho ban quản lý chung cư
        //                if (TieuThu <= DinhMucHN + DinhMuc)
        //                {
        //                    double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                    if (HasValue(TyLe) == false)
        //                        TyLe = 0;
        //                    int TieuThuHN = 0, TieuThuDC = 0;
        //                    TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
        //                    TieuThuDC = TieuThu - TieuThuHN;
        //                    TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                    TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (TieuThuDC * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (TieuThuHN > 0)
        //                    {
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (TieuThuDC > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                        updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                    if (TieuThu - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                    {
        //                        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                    + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                    + ((TieuThu - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                              + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                              + ((TieuThu - DinhMuc) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                    + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
        //                                    + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                           + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                           + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                           + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                else
        //                {
        //                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                + ((TieuThu - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                      + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                      + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (DinhMucHN > 0)
        //                    {
        //                        _chiTiet = +DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = +DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (DinhMuc > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));
        //                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
        //                        TieuThu_DieuChinhGia = TieuThu;
        //                    else
        //                        TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
        //                }
        //                break;
        //            case 52:///sỉ khu công nghiệp
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100);
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 53:///sỉ KD - TM
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100);
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 54:///sỉ HCSN
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100);
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
        //                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TieuThu > 0)
        //                    {
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 58:
        //                if (TyLeSH != 0)
        //                    _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeHCSN != 0)
        //                    _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeSX != 0)
        //                    _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                _DV = TieuThu - _SH - _HCSN - _SX;
        //                TongTien = (_SH * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                            + (_HCSN * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100))
        //                            + (_SX * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100))
        //                            + (_DV * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
        //                TongTienPhiBVMT = (_SH * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (_HCSN * (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (_SX * (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (_DV * (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                if (_SH > 0)
        //                {
        //                    _chiTiet += _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                    _chiTietPhiBVMT += _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                }
        //                if (_HCSN > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                if (_SX > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                if (_DV > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100)));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                break;
        //            case 59:///sỉ phức tạp
        //                if (TyLeSH != 0)
        //                    _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeHCSN != 0)
        //                    _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeSX != 0)
        //                    _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                _DV = TieuThu - _SH - _HCSN - _SX;
        //                if (_SH <= DinhMucHN + DinhMuc)
        //                {
        //                    double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                    if (HasValue(TyLe) == false)
        //                        TyLe = 0;
        //                    int TieuThuHN = 0, TieuThuDC = 0;
        //                    TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                    TieuThuDC = _SH - TieuThuHN;
        //                    TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                    TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (TieuThuDC * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (TieuThuHN > 0)
        //                    {
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (TieuThuDC > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                    {
        //                        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                    + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                    + ((_SH - DinhMucHN - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                    + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
        //                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                           + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                           + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                           + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                else
        //                {
        //                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                + ((_SH - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (DinhMucHN > 0)
        //                    {
        //                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (DinhMuc > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));
        //                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
        //                        TieuThu_DieuChinhGia = _SH;
        //                    else
        //                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                }
        //                TongTien += (_HCSN * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100))
        //                                + (_SX * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100))
        //                                + (_DV * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
        //                TongTienPhiBVMT += (_HCSN * (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                + (_SX * (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                + (_DV * (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                if (_HCSN > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                if (_SX > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                if (_DV > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100)));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                break;
        //            case 68:///SH giá sỉ - KD giá lẻ
        //                if (TyLeSH != 0)
        //                    _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                _DV = TieuThu - _SH;
        //                if (_SH <= DinhMucHN + DinhMuc)
        //                {
        //                    double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                    if (HasValue(TyLe) == false)
        //                        TyLe = 0;
        //                    int TieuThuHN = 0, TieuThuDC = 0;
        //                    TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                    TieuThuDC = _SH - TieuThuHN;
        //                    TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                    TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (TieuThuDC * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (TieuThuHN > 0)
        //                    {
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (TieuThuDC > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                        updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                    {
        //                        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                    + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                    + ((_SH - DinhMucHN - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                            + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                    + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
        //                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
        //                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        if (DinhMucHN > 0)
        //                        {
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                        }
        //                        if (DinhMuc > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                        {
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
        //                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                        }
        //                    }
        //                else
        //                {
        //                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                + ((_SH - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
        //                                        + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    if (DinhMucHN > 0)
        //                    {
        //                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
        //                    }
        //                    if (DinhMuc > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                    {
        //                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));
        //                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                    }
        //                    if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
        //                        TieuThu_DieuChinhGia = _SH;
        //                    else
        //                        TieuThu_DieuChinhGia = _SH - DinhMuc;
        //                }
        //                TongTien += _DV * lstGiaNuoc[5];
        //                TongTienPhiBVMT += _DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
        //                if (_DV > 0)
        //                {
        //                    updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                    updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
        //                }
        //                break;
        //            default:
        //                _chiTiet = "";
        //                TongTien = 0;
        //                break;
        //        }
        //        ChiTiet = _chiTiet;
        //        PhiBVMT = TongTienPhiBVMT;
        //        ChiTietPhiBVMT = _chiTietPhiBVMT;
        //        return TongTien;
        //    }
        //    catch (Exception ex)
        //    {
        //        ChiTiet = "";
        //        TieuThu_DieuChinhGia = 0;
        //        throw ex;
        //    }
        //}

        //private DataTable getGiaNuocGiam(int Nam, int Ky, int GiaBieu)
        //{
        //    return _cDAL_ThuongVu.ExecuteQuery_DataTable("select * from GiaNuoc_Giam where Nam like '%" + Nam + "%' and Ky like '%" + Ky.ToString("00") + "%' and GiaBieu like '%" + GiaBieu + "%'");
        //}

        //private bool checkExists_GiamGiaNuoc(int Nam, int Ky, int GiaBieu, ref DataTable dt)
        //{
        //    DataTable dtGiaNuocGiam = getGiaNuocGiam(Nam, Ky, GiaBieu);

        //    if (dtGiaNuocGiam != null && dtGiaNuocGiam.Rows.Count > 0)
        //    {
        //        double TyLeGiam = double.Parse(dtGiaNuocGiam.Rows[0]["TyLeGiam"].ToString());
        //        foreach (DataRow item in dt.Rows)
        //        {
        //            item["SHN"] = int.Parse(item["SHN"].ToString()) - (int)(int.Parse(item["SHN"].ToString()) * TyLeGiam / 100);
        //            item["SHTM"] = int.Parse(item["SHTM"].ToString()) - (int)(int.Parse(item["SHTM"].ToString()) * TyLeGiam / 100);
        //            item["SHVM1"] = int.Parse(item["SHVM1"].ToString()) - (int)(int.Parse(item["SHVM1"].ToString()) * TyLeGiam / 100);
        //            item["SHVM2"] = int.Parse(item["SHVM2"].ToString()) - (int)(int.Parse(item["SHVM2"].ToString()) * TyLeGiam / 100);
        //            item["SX"] = int.Parse(item["SX"].ToString()) - (int)(int.Parse(item["SX"].ToString()) * TyLeGiam / 100);
        //            item["HCSN"] = int.Parse(item["HCSN"].ToString()) - (int)(int.Parse(item["HCSN"].ToString()) * TyLeGiam / 100);
        //            item["KDDV"] = int.Parse(item["KDDV"].ToString()) - (int)(int.Parse(item["KDDV"].ToString()) * TyLeGiam / 100);
        //        }
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        //private bool checkKhongTinhPBVMT(string DanhBo)
        //{
        //    if (_cDAL_sDHN.ExecuteQuery_ReturnOneValue("select DanhBo from DanhBoKPBVMT where DanhBo='" + DanhBo + "'") != null)
        //        return true;
        //    else
        //        return false;
        //}

        //private DataTable getDS_GiaNuoc_Func()
        //{
        //    return _cDAL_ThuongVu.ExecuteQuery_DataTable("select * from GiaNuoc2");
        //}

        //private void TinhTienNuoc(bool KhongApGiaGiam, bool ApGiaNuocCu, bool DieuChinhGia, int GiaDieuChinh, string DanhBo, int Ky, int Nam, DateTime TuNgay, DateTime DenNgay, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, ref int TienNuocNamCu, ref string ChiTietNamCu, ref int TienNuocNamMoi, ref string ChiTietNamMoi, ref int TieuThu_DieuChinhGia, ref int PhiBVMTNamCu, ref string ChiTietPhiBVMTNamCu, ref int PhiBVMTNamMoi, ref string ChiTietPhiBVMTNamMoi, ref int TienNuoc, ref int ThueGTGT, ref int TDVTN, ref int ThueTDVTN)
        //{
        //    DataTable dtGiaNuoc = getDS_GiaNuoc_Func();
        //    //check giảm giá
        //    if (KhongApGiaGiam == false)
        //        checkExists_GiamGiaNuoc(Nam, Ky, GiaBieu, ref dtGiaNuoc);

        //    int index = -1;
        //    TienNuocNamCu = TienNuocNamMoi = PhiBVMTNamCu = PhiBVMTNamMoi = TienNuoc = ThueGTGT = TDVTN = ThueTDVTN = 0;
        //    ChiTietNamCu = ChiTietNamMoi = ChiTietPhiBVMTNamCu = ChiTietPhiBVMTNamMoi = "";
        //    TieuThu_DieuChinhGia = 0;
        //    for (int i = 0; i < dtGiaNuoc.Rows.Count; i++)
        //        if (TuNgay.Date < DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date && DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date < DenNgay.Date)
        //        {
        //            index = i;
        //        }
        //        else
        //            if (TuNgay.Date >= DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date)
        //        {
        //            index = i;
        //        }
        //    if (index != -1)
        //    {
        //        if (DenNgay.Date < new DateTime(2019, 11, 15))
        //        {
        //            List<int> lstGiaNuoc = new List<int> { int.Parse(dtGiaNuoc.Rows[index]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["PhiBVMT"].ToString()) };
        //            TienNuocNamCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuoc, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, 0, TieuThu, out ChiTietNamCu, out TieuThu_DieuChinhGia, out PhiBVMTNamCu, out ChiTietPhiBVMTNamCu);
        //        }
        //        else
        //            if (TuNgay.Date < DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date && DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date < DenNgay.Date)
        //        {
        //            if (ApGiaNuocCu == false)
        //            {
        //                //int TieuThu_DieuChinhGia;
        //                int TongSoNgay = (int)((DenNgay.Date - TuNgay.Date).TotalDays);

        //                int SoNgayCu = (int)((DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date - TuNgay.Date).TotalDays);
        //                int TieuThuCu = (int)Math.Round((double)TieuThu * SoNgayCu / TongSoNgay, 0, MidpointRounding.AwayFromZero);
        //                int TieuThuMoi = TieuThu - TieuThuCu;
        //                int TongDinhMucCu = (int)Math.Round((double)TongDinhMuc * SoNgayCu / TongSoNgay, 0, MidpointRounding.AwayFromZero);
        //                int TongDinhMucMoi = TongDinhMuc - TongDinhMucCu;
        //                int DinhMucHN_Cu = 0, DinhMucHN_Moi = 0;
        //                if (TuNgay.Date > new DateTime(2019, 11, 15))
        //                    if (TongDinhMucCu != 0 && DinhMucHN != 0 && TongDinhMuc != 0)
        //                        DinhMucHN_Cu = (int)Math.Round((double)TongDinhMucCu * DinhMucHN / TongDinhMuc, 0, MidpointRounding.AwayFromZero);
        //                if (TongDinhMucMoi != 0 && DinhMucHN != 0 && TongDinhMuc != 0)
        //                    DinhMucHN_Moi = (int)Math.Round((double)TongDinhMucMoi * DinhMucHN / TongDinhMuc, 0, MidpointRounding.AwayFromZero);
        //                List<int> lstGiaNuocCu = new List<int> { int.Parse(dtGiaNuoc.Rows[index - 1]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHN"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["PhiBVMT"].ToString()) };
        //                List<int> lstGiaNuocMoi = new List<int> { int.Parse(dtGiaNuoc.Rows[index]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["PhiBVMT"].ToString()) };
        //                //lần đầu áp dụng giá biểu 10, tổng áp giá mới luôn
        //                if (TuNgay.Date < new DateTime(2019, 11, 15) && new DateTime(2019, 11, 15) < DenNgay.Date && GiaBieu == 10)
        //                    TienNuocNamCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocMoi, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucCu, DinhMucHN_Cu, TieuThuCu, out ChiTietNamCu, out TieuThu_DieuChinhGia, out PhiBVMTNamCu, out ChiTietPhiBVMTNamCu);
        //                else
        //                    TienNuocNamCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocCu, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucCu, DinhMucHN_Cu, TieuThuCu, out ChiTietNamCu, out TieuThu_DieuChinhGia, out PhiBVMTNamCu, out ChiTietPhiBVMTNamCu);
        //                TienNuocNamMoi = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocMoi, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucMoi, DinhMucHN_Moi, TieuThuMoi, out ChiTietNamMoi, out TieuThu_DieuChinhGia, out PhiBVMTNamMoi, out ChiTietPhiBVMTNamMoi);
        //            }
        //            else
        //            {
        //                List<int> lstGiaNuocCu = new List<int> { int.Parse(dtGiaNuoc.Rows[index - 1]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHN"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["PhiBVMT"].ToString()) };
        //                TienNuocNamCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocCu, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, out ChiTietNamCu, out TieuThu_DieuChinhGia, out PhiBVMTNamCu, out ChiTietPhiBVMTNamCu);
        //            }
        //        }
        //        else
        //        {
        //            List<int> lstGiaNuoc = new List<int> { int.Parse(dtGiaNuoc.Rows[index]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["PhiBVMT"].ToString()) };
        //            TienNuocNamCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuoc, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, out ChiTietNamCu, out TieuThu_DieuChinhGia, out PhiBVMTNamCu, out ChiTietPhiBVMTNamCu);
        //        }
        //        if (checkKhongTinhPBVMT(DanhBo) == true)
        //        {
        //            PhiBVMTNamCu = PhiBVMTNamMoi = 0;
        //            ChiTietPhiBVMTNamCu = ChiTietPhiBVMTNamMoi = "";
        //        }
        //        TienNuoc = TienNuocNamCu + TienNuocNamMoi;
        //        ThueGTGT = (int)Math.Round((double)(TienNuocNamCu + TienNuocNamMoi) * 5 / 100, 0, MidpointRounding.AwayFromZero);
        //        TDVTN = PhiBVMTNamCu + PhiBVMTNamMoi;
        //        //Từ 2022 Phí BVMT -> Tiền Dịch Vụ Thoát Nước
        //        int ThueTDVTN_VAT = 0;
        //        if (dtGiaNuoc.Rows[index]["VAT2_Ky"].ToString().Contains(Ky.ToString("00") + "/" + Nam))
        //            ThueTDVTN_VAT = int.Parse(dtGiaNuoc.Rows[index]["VAT2"].ToString());
        //        else
        //            ThueTDVTN_VAT = int.Parse(dtGiaNuoc.Rows[index]["VAT"].ToString());
        //        if ((TuNgay.Year < 2021) || (TuNgay.Year == 2021 && DenNgay.Year == 2021))
        //        {
        //            ThueTDVTN = 0;
        //        }
        //        else
        //            if (TuNgay.Year == 2021 && DenNgay.Year == 2022)
        //        {
        //            ThueTDVTN = (int)Math.Round((double)(PhiBVMTNamMoi) * ThueTDVTN_VAT / 100, 0, MidpointRounding.AwayFromZero);
        //        }
        //        else
        //                if (TuNgay.Year >= 2022)
        //        {
        //            ThueTDVTN = (int)Math.Round((double)(PhiBVMTNamCu + PhiBVMTNamMoi) * ThueTDVTN_VAT / 100, 0, MidpointRounding.AwayFromZero);
        //        }
        //    }
        //}

        //[Route("ghi_ChiSo_TrucTiep")]
        //[HttpGet]
        //public MResult ghi_ChiSo(string ID, string Code, string ChiSo, string HinhDHN, string Dot, string MaNV, string TBTT)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        if (ChiSo == "")
        //            ChiSo = "0";
        //        if (checkChot_BillState(ID.Substring(0, 4), ID.Substring(4, 2), Dot) == true)
        //        {
        //            result.success = false;
        //            result.error = "Đã chốt dữ liệu";
        //        }
        //        else
        //            if (checkChuBao(ID) == true)
        //        {
        //            result.success = false;
        //            result.error = "Chủ Báo";
        //        }
        //        else
        //                if (checkXuLy(ID) == true)
        //        {
        //            result.success = false;
        //            result.error = "Tổ đã xử lý";
        //        }
        //        else
        //        {
        //            MHoaDon hd = new MHoaDon();
        //            int TieuThu = -1;
        //            int GiaBan = 0, ThueGTGT = 0, PhiBVMT = 0, PhiBVMT_Thue = 0;
        //            bool success = tinhCodeTieuThu(ID, Code, int.Parse(ChiSo), out TieuThu, out GiaBan, out ThueGTGT, out PhiBVMT, out PhiBVMT_Thue);
        //            if (success == true)
        //            {
        //                hd.TieuThu = TieuThu;
        //                hd.GiaBan = GiaBan;
        //                hd.ThueGTGT = ThueGTGT;
        //                hd.PhiBVMT = PhiBVMT;
        //                hd.PhiBVMT_Thue = PhiBVMT_Thue;
        //                //if (hd.TieuThu < 0)
        //                //{
        //                //    result.success = false;
        //                //    result.error = "Tiêu Thụ âm = " + hd.TieuThu;
        //                //}
        //                //else
        //                {
        //                    DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("select CodeCu,CSCu from DocSo where DocSoID=" + ID);
        //                    if (dt != null && dt.Rows.Count > 0)
        //                    {
        //                        if (Code.Substring(0, 1) == "4" && (dt.Rows[0]["CodeCu"].ToString().Substring(0, 1) == "F" || dt.Rows[0]["CodeCu"].ToString().Substring(0, 1) == "6" || dt.Rows[0]["CodeCu"].ToString().Substring(0, 1) == "K" || dt.Rows[0]["CodeCu"].ToString().Substring(0, 1) == "N"))
        //                        {
        //                            Code = "5" + dt.Rows[0]["CodeCu"].ToString().Substring(0, 1);
        //                        }
        //                        if (Code.Substring(0, 1) == "F" || Code == "61")
        //                            ChiSo = (int.Parse(dt.Rows[0]["CSCu"].ToString()) + int.Parse(TBTT)).ToString();
        //                        else
        //                            if (Code.Substring(0, 1) == "K")
        //                            ChiSo = dt.Rows[0]["CSCu"].ToString();
        //                        else
        //                                if (Code.Substring(0, 1) == "N")
        //                            ChiSo = "0";
        //                        else
        //                                    if (Code == "5N" || Code == "5F" || Code == "5K")
        //                            hd.CSC = (int.Parse(ChiSo) - hd.TieuThu);
        //                    }
        //                    hd.TongCong = hd.GiaBan + hd.ThueGTGT + hd.PhiBVMT + hd.PhiBVMT_Thue;
        //                    string sql = "update DocSo set CodeMoi=N'" + Code + "',TTDHNMoi=(select TTDHN from TTDHN where Code='" + Code + "'),CSMoi=" + ChiSo + ",TieuThuMoi=" + hd.TieuThu
        //                        + ",TienNuoc=" + hd.GiaBan + ",Thue=" + hd.ThueGTGT + ",BVMT=" + hd.PhiBVMT + ",BVMT_Thue=" + hd.PhiBVMT_Thue + ",TongTien=" + hd.TongCong
        //                        + ",NVCapNhat=N'" + MaNV + "',NgayCapNhat=getdate(),GioGhi=getdate() where DocSoID='" + ID + "'";
        //                    success = _cDAL_sDHN.ExecuteNonQuery(sql);
        //                    if (HinhDHN != "")
        //                        success = ghi_HinhDHN_Func(ID, HinhDHN);
        //                    result.success = success;
        //                    if (hd.TieuThu < 0)
        //                    {
        //                        result.error = "Tiêu Thụ âm = " + hd.TieuThu;
        //                    }
        //                    else
        //                        if (hd.TieuThu == 0)
        //                    {
        //                        result.alert = "Tiêu Thụ = " + hd.TieuThu;
        //                    }
        //                    else
        //                            if (hd.TieuThu > 0 && (hd.TieuThu < int.Parse(TBTT) - int.Parse(TBTT) * 1.4 || hd.TieuThu >= int.Parse(TBTT) * 1.4))
        //                    {
        //                        result.alert = "Tiêu Thụ bất thường = " + hd.TieuThu;
        //                    }
        //                }
        //                hd.CodeMoi = Code;
        //                hd.ChiSoMoi = int.Parse(ChiSo);
        //                result.message = CGlobalVariable.jsSerializer.Serialize(hd);
        //                //DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select KyHD,TongCong from fnGetHoaDonTon(" + ID.Substring(6, 11) + ")");
        //                //if (dt.Rows.Count > 0)
        //                //    result.hoadonton = DataTableToJSON(dt);
        //            }
        //            else
        //            {
        //                result.success = false;
        //                if (hd.TieuThu < 0)
        //                {
        //                    result.error = "Tiêu Thụ âm = " + hd.TieuThu;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //[Route("ghi_ChiSo_GianTiep")]
        //[HttpGet]
        //public MResult ghi_ChiSo(string ID, string Code, string ChiSo, string TieuThu, string TienNuoc, string ThueGTGT, string PhiBVMT, string PhiBVMT_Thue, string TongCong, string HinhDHN, string Dot, string MaNV, string NgayDS)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        if (ChiSo == "")
        //            ChiSo = "0";
        //        if (checkChot_BillState(ID.Substring(0, 4), ID.Substring(4, 2), Dot) == true)
        //        {
        //            result.success = false;
        //            result.error = "Đã chốt dữ liệu";
        //        }
        //        else
        //            if (checkChuBao(ID) == true)
        //        {
        //            result.success = false;
        //            result.error = "Chủ Báo";
        //        }
        //        else
        //                if (checkXuLy(ID) == true)
        //        {
        //            result.success = false;
        //            result.error = "Tổ đã xử lý";
        //        }
        //        else
        //        {
        //            IFormatProvider culture = new CultureInfo("en-US", true);
        //            DateTime date = DateTime.ParseExact(NgayDS, "dd/MM/yyyy HH:mm:ss", culture);
        //            string sql = "update DocSo set CodeMoi=N'" + Code + "',TTDHNMoi=(select TTDHN from TTDHN where Code='" + Code + "'),CSMoi=" + ChiSo + ",TieuThuMoi=" + TieuThu
        //                + ",TienNuoc=" + TienNuoc + ",Thue=" + ThueGTGT + ",BVMT=" + PhiBVMT + ",BVMT_Thue=" + PhiBVMT_Thue + ",TongTien=" + TongCong
        //                + ",NVCapNhat=N'" + MaNV + "',NgayCapNhat=getdate(),GioGhi='" + date.ToString("yyyyMMdd HH:mm:ss") + "' where DocSoID='" + ID + "'";
        //            result.success = _cDAL_sDHN.ExecuteNonQuery(sql);
        //            if (HinhDHN != "")
        //                result.success = ghi_HinhDHN_Func(ID, HinhDHN);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //[Route("get_ThongTin")]
        //[HttpGet]
        //public MResult get_ThongTin(string DanhBo, string Nam, string Ky)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("select TuNgay=CONVERT(varchar(10),TuNgay,103),DenNgay=CONVERT(varchar(10),DenNgay,103),CodeMoi,CSMoi,TieuThuMoi from DocSo where DanhBa='" + DanhBo + "' and Nam=" + Nam + " and Ky=" + Ky);
        //        MHoaDon hd = new MHoaDon();
        //        hd.TuNgay = DateTime.Parse(dt.Rows[0]["TuNgay"].ToString());
        //        hd.DenNgay = DateTime.Parse(dt.Rows[0]["DenNgay"].ToString());
        //        hd.CodeMoi = dt.Rows[0]["CodeMoi"].ToString();
        //        hd.ChiSoMoi = int.Parse(dt.Rows[0]["CSMoi"].ToString());
        //        hd.TieuThuMoi = int.Parse(dt.Rows[0]["TieuThuMoi"].ToString());
        //        result.message = CGlobalVariable.jsSerializer.Serialize(hd);
        //        byte[] hinh = get_HinhDHN_Func(Nam + Ky + DanhBo);
        //        if (hinh != null)
        //            result.alert = Convert.ToBase64String(hinh);
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //[Route("getTon_FromDienThoai")]
        //[HttpGet]
        //public void getTon_FromDienThoai()
        //{
        //    string sql = "select Nam,Ky=RIGHT('0' + CAST(Ky AS VARCHAR(2)), 2),Dot=RIGHT('0' + CAST(IDDot AS VARCHAR(2)), 2) from Lich_DocSo ds,Lich_DocSo_ChiTiet dsct,Lich_Dot dot"
        //                 + " where ds.ID=dsct.IDDocSo and dot.ID=dsct.IDDot and CAST(dsct.NgayDoc as date)=CAST(GETDATE() as date)";
        //    DataTable dtKy = _cDAL_sDHN.ExecuteQuery_DataTable(sql);
        //    DataTable dtDocSo = _cDAL_sDHN.ExecuteQuery_DataTable("select DocSoID,CodeMoi,Dot from DocSo where Nam=" + dtKy.Rows[0]["Nam"].ToString() + " and Ky='" + dtKy.Rows[0]["Ky"].ToString() + "' and Dot='" + dtKy.Rows[0]["Dot"].ToString() + "'");
        //    foreach (DataRow item in dtDocSo.Rows)
        //        if (item["CodeMoi"] == null || item["CodeMoi"].ToString() == "")
        //        {
        //            _cDAL_sDHN.ExecuteNonQuery("exec [dbo].[spSendNotificationToClient] N'CodeTon',N'" + item["Dot"].ToString() + "',N'" + item["DocSoID"].ToString() + "'");
        //        }
        //        else
        //            if (checkExists_HinhDHN_Func(item["DocSoID"].ToString()) == false)
        //        {
        //            _cDAL_sDHN.ExecuteNonQuery("exec [dbo].[spSendNotificationToClient] N'HinhTon',N'" + item["Dot"].ToString() + "',N'" + item["DocSoID"].ToString() + "'");
        //        }
        //    int count = 5;
        //    int Nam = int.Parse(dtKy.Rows[0]["Nam"].ToString());
        //    int Ky = int.Parse(dtKy.Rows[0]["Ky"].ToString());
        //    int Dot = int.Parse(dtKy.Rows[0]["Dot"].ToString());
        //    while (count > 0)
        //    {
        //        Dot--;
        //        if (Dot == 0)
        //        {
        //            Dot = 20;
        //            Ky--;
        //            if (Ky == 0)
        //            {
        //                Ky = 12;
        //                Nam--;
        //            }
        //        }
        //        DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("select DocSoID,CodeMoi,Dot from DocSo where Nam=" + Nam.ToString() + " and Ky='" + Ky.ToString("00") + "' and Dot='" + Dot.ToString("00") + "'");
        //        foreach (DataRow item in dt.Rows)
        //            if (item["CodeMoi"].ToString() != "" && checkExists_HinhDHN_Func(item["DocSoID"].ToString()) == false)
        //            {
        //                sql = "exec [dbo].[spSendNotificationToClient] N'HinhTon',N'" + item["Dot"].ToString() + "',N'" + item["DocSoID"].ToString() + "'";
        //                _cDAL_sDHN.ExecuteNonQuery(sql);
        //            }
        //        count--;
        //    }
        //}

        ////ghi chú
        //[Route("get_GhiChu")]
        //[HttpGet]
        //public MResult get_GhiChu(string DanhBo)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select SoNha,TenDuong,ViTri=VITRIDHN,ViTriNgoai=ViTriDHN_Ngoai,ViTriHop=ViTriDHN_Hop,Gieng,KhoaTu,AmSau,XayDung,NgapNuoc,KetTuong,LapKhoaGoc,BeHBV,BeNapMatNapHBV from TB_DULIEUKHACHHANG where DanhBo='" + DanhBo.Replace(" ", "") + "'"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //[Route("update_GhiChu")]
        //[HttpGet]
        //public MResult update_GhiChu(string DanhBo, string SoNha, string TenDuong, string ViTri, string ViTriNgoai, string ViTriHop, string Gieng, string KhoaTu, string AmSau, string XayDung, string DutChiGoc, string DutChiThan
        //    , string NgapNuoc, string KetTuong, string LapKhoaGoc, string BeHBV, string BeNapMatNapHBV, string MauSacChiGoc, string GhiChu, string MaNV)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        string flagGieng = bool.Parse(Gieng) == true ? "1" : "0";
        //        string flagKhoaTu = bool.Parse(KhoaTu) == true ? "1" : "0";
        //        string flagAmSau = bool.Parse(AmSau) == true ? "1" : "0";
        //        string flagXayDung = bool.Parse(XayDung) == true ? "1" : "0";
        //        string flagDutChiGoc = bool.Parse(XayDung) == true ? "1" : "0";
        //        string flagDutChiThan = bool.Parse(XayDung) == true ? "1" : "0";
        //        string flagNgapNuoc = bool.Parse(NgapNuoc) == true ? "1" : "0";
        //        string flagKetTuong = bool.Parse(KetTuong) == true ? "1" : "0";
        //        string flagLapKhoaGoc = bool.Parse(LapKhoaGoc) == true ? "1" : "0";
        //        string flagBeHBV = bool.Parse(BeHBV) == true ? "1" : "0";
        //        string flagBeNapMatNapHBV = bool.Parse(BeNapMatNapHBV) == true ? "1" : "0";
        //        string flagViTriNgoai = bool.Parse(ViTriNgoai) == true ? "1" : "0";
        //        string flagViTriHop = bool.Parse(ViTriHop) == true ? "1" : "0";
        //        string sql = "";
        //        sql += "update TB_DULIEUKHACHHANG set AmSau=" + flagAmSau + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and AmSau=1";
        //        sql += " update TB_DULIEUKHACHHANG set XayDung=" + flagXayDung + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and XayDung=1";
        //        sql += " update TB_DULIEUKHACHHANG set DutChi_Goc=" + flagDutChiGoc + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and DutChi_Goc=1";
        //        sql += " update TB_DULIEUKHACHHANG set DutChi_Than=" + flagDutChiThan + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and DutChi_Than=1";
        //        sql += " update TB_DULIEUKHACHHANG set NgapNuoc=" + flagNgapNuoc + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and NgapNuoc=1";
        //        sql += " update TB_DULIEUKHACHHANG set KetTuong=" + flagKetTuong + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and KetTuong=1";
        //        sql += " update TB_DULIEUKHACHHANG set LapKhoaGoc=" + flagLapKhoaGoc + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and LapKhoaGoc=1";
        //        sql += " update TB_DULIEUKHACHHANG set BeHBV=" + flagBeHBV + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and BeHBV=1";
        //        sql += " update TB_DULIEUKHACHHANG set BeNapMatNapHBV=" + flagBeNapMatNapHBV + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and BeNapMatNapHBV=1";
        //        sql += " update TB_DULIEUKHACHHANG set SoNha=N'" + SoNha + "',TenDuong=N'" + TenDuong + "',VITRIDHN=N'" + ViTri + "',ViTriDHN_Ngoai=" + flagViTriNgoai + ",ViTriDHN_Hop=" + flagViTriHop
        //            + ",Gieng=" + flagGieng + ",KhoaTu=" + flagKhoaTu
        //            + ",MauSacChiGoc=N'" + MauSacChiGoc + "',GhiChu=N'" + GhiChu + "',MODIFYBY=" + MaNV + ",MODIFYDATE=getdate() where DanhBo='" + DanhBo.Replace(" ", "") + "'";
        //        result.success = _cDAL_DHN.ExecuteNonQuery(sql);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //[Route("getDS_DienThoai")]
        //[HttpGet]
        //public MResult getDS_DienThoai(string DanhBo)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_DHN.ExecuteQuery_DataTable("select DanhBo,DienThoai,HoTen,SoChinh,GhiChu from SDT_DHN where DanhBo='" + DanhBo.Replace(" ", "") + "' order by CreateDate desc"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //[Route("update_DienThoai")]
        //[HttpGet]
        //public MResult update_DienThoai(string DanhBo, string DienThoai, string HoTen, string SoChinh, string MaNV)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        if (DienThoai.Length != 10 || DienThoai.All(char.IsNumber) == false)
        //        {
        //            result.success = false;
        //            result.error = "Không đủ 10 số";
        //        }
        //        else
        //        {
        //            string flagSoChinh = bool.Parse(SoChinh) == true ? "1" : "0";
        //            string sql = "declare @DanhBo char(11)"
        //                    + " declare @DienThoai varchar(15)"
        //                    + " declare @HoTen nvarchar(50)"
        //                    + " declare @SoChinh bit"
        //                    + " declare @GhiChu nvarchar(50)"
        //                    + " set @DanhBo='" + DanhBo.Replace(" ", "") + "'"
        //                    + " set @DienThoai='" + DienThoai + "'"
        //                    + " set @HoTen=N'" + HoTen + "'"
        //                    + " set @SoChinh=" + flagSoChinh
        //                    + " set @GhiChu=N'Đ. QLĐHN'"
        //                    + " if exists(select DanhBo from SDT_DHN where DanhBo=@DanhBo and DienThoai=@DienThoai)"
        //                    + " update SDT_DHN set HoTen=@HoTen,SoChinh=@SoChinh,GhiChu=@GhiChu,ModifyBy=" + MaNV + ",ModifyDate=GETDATE() where DanhBo=@DanhBo and DienThoai=@DienThoai"
        //                    + " else"
        //                    + " insert into SDT_DHN(DanhBo,DienThoai,HoTen,SoChinh,GhiChu,CreateBy,CreateDate)values(@DanhBo,@DienThoai,@HoTen,@SoChinh,@GhiChu," + MaNV + ",GETDATE())";
        //            result.success = _cDAL_DHN.ExecuteNonQuery(sql);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //[Route("delete_DienThoai")]
        //[HttpGet]
        //public MResult delete_DienThoai(string DanhBo, string DienThoai)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.success = _cDAL_DHN.ExecuteNonQuery("delete SDT_DHN where DanhBo='" + DanhBo.Replace(" ", "") + "' and DienThoai='" + DienThoai + "'");
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        ////phiếu chuyển
        //[Route("getDS_PhieuChuyen")]
        //[HttpGet]
        //public MResult getDS_PhieuChuyen()
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select [Name],KhongLapDon from MaHoa_PhieuChuyen where App=1"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //[Route("ghi_DonTu")]
        //[HttpGet]
        //public MResult ghi_DonTu(string DanhBo, string NoiDung, string GhiChu, string Hinh, string MaNV)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        DataTable dtPC = _cDAL_sDHN.ExecuteQuery_DataTable("select Folder from MaHoa_PhieuChuyen where App=1 and KhongLapDon=1 and Name=N'" + NoiDung + "'");
        //        if (dtPC != null && dtPC.Rows.Count > 0)
        //        {
        //            switch (NoiDung)
        //            {
        //                case "Đứt Chì Góc":
        //                    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set DutChi_Goc=1,DutChi_Goc_Ngay=getdate() where DanhBo='" + DanhBo + "' and DutChi_Goc=0");
        //                    break;
        //                case "Đứt Chì Thân":
        //                    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set DutChi_Than=1,DutChi_Than_Ngay=getdate() where DanhBo='" + DanhBo + "' and DutChi_Than=0");
        //                    break;
        //                case "Đứt Chì Góc + Thân":
        //                    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set DutChi_Goc=1,DutChi_Goc_Ngay=getdate(),DutChi_Than=1,DutChi_Than_Ngay=getdate() where DanhBo='" + DanhBo + "' and DutChi_Than=0");
        //                    break;
        //                case "Ngập Nước":
        //                    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set NgapNuoc=1,NgapNuoc_Ngay=getdate() where DanhBo='" + DanhBo + "' and NgapNuoc=0");
        //                    break;
        //                case "Kẹt Tường":
        //                    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set KetTuong=1,KetTuong_Ngay=getdate() where DanhBo='" + DanhBo + "' and KetTuong=0");
        //                    break;
        //                case "Lấp Khóa Góc":
        //                    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set LapKhoaGoc=1,LapKhoaGoc_Ngay=getdate() where DanhBo='" + DanhBo + "' and LapKhoaGoc=0");
        //                    break;
        //                case "Bể HBV":
        //                    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set BeHBV=1,BeHBV_Ngay=getdate() where DanhBo='" + DanhBo + "' and BeHBV=0");
        //                    break;
        //                case "Bể Nấp, Mất Nấp HBV":
        //                    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set BeNapMatNapHBV=1,BeNapMatNapHBV_Ngay=getdate() where DanhBo='" + DanhBo + "' and BeNapMatNapHBV=0");
        //                    break;
        //                default:
        //                    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set " + dtPC.Rows[0]["Folder"].ToString() + "=1," + dtPC.Rows[0]["Folder"].ToString() + "_Ngay=getdate() where DanhBo='" + DanhBo + "'");
        //                    break;
        //            }
        //            if (Hinh != "")
        //                result.success = ghi_Hinh_241(CGlobalVariable.pathHinhDHNMaHoa, dtPC.Rows[0]["Folder"].ToString(), "", DanhBo + ".jpg", Hinh);
        //        }
        //        else
        //        {
        //            DataTable dt = _cDAL_sDHN.ExecuteQuery_DataTable("select top 1 MLT=MLT1,HoTen=TENKH,DiaChi=SO+' '+DUONG,GiaBieu=GB,DinhMuc=DM,DinhMucHN=DMHN,Dot,Ky,Nam,Phuong,Quan,HopDong from BienDong where DanhBa='" + DanhBo + "' order by BienDongID desc");
        //            if (dt != null && dt.Rows.Count > 0)
        //            {
        //                object checkExists_DanhBoBoQua = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select DanhBo from MaHoa_DanhBo_Except where DanhBo='" + DanhBo + "'");
        //                if (NoiDung == "Giá Biểu" && checkExists_DanhBoBoQua != null)
        //                {
        //                    result.success = false;
        //                    result.error = "Danh Bộ nằm trong danh sách bỏ qua";
        //                }
        //                else
        //                {
        //                    object checkExists = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select top 1 ID from MaHoa_DonTu where DanhBo='" + DanhBo + "' and NoiDung=N'" + NoiDung + "' and cast(getdate() as date)=cast(createdate as date)");
        //                    if (checkExists == null)
        //                    {
        //                        string ID = "";
        //                        checkExists = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select top 1 ID from MaHoa_DonTu where ID like '" + DateTime.Now.ToString("yyMM") + "%'");
        //                        if (checkExists != null)
        //                        {
        //                            object stt = _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select MAX(SUBSTRING(CAST(ID as varchar(8)),5,4))+1 from MaHoa_DonTu where ID like '" + DateTime.Now.ToString("yyMM") + "%'");
        //                            if (stt != null)
        //                                ID = DateTime.Now.ToString("yyMM") + ((int)stt).ToString("0000");
        //                        }
        //                        else
        //                        {
        //                            ID = DateTime.Now.ToString("yyMM") + 1.ToString("0000");
        //                        }
        //                        string DinhMucHN = "NULL";
        //                        if (dt.Rows[0]["DinhMucHN"].ToString() != "")
        //                            DinhMucHN = dt.Rows[0]["DinhMucHN"].ToString();
        //                        string sql = "insert into MaHoa_DonTu(ID,MLT,DanhBo,HoTen,DiaChi,GiaBieu,DinhMuc,DinhMucHN,NoiDung,GhiChu,Dot,Ky,Nam,Phuong,Quan,CreateBy,CreateDate,HopDong)values"
        //                            + "("
        //                            + ID + ",'" + dt.Rows[0]["MLT"] + "','" + DanhBo + "',N'" + dt.Rows[0]["HoTen"] + "',N'" + dt.Rows[0]["DiaChi"] + "'"
        //                            + "," + dt.Rows[0]["GiaBieu"] + "," + dt.Rows[0]["DinhMuc"] + "," + DinhMucHN + ",N'" + NoiDung + "',N'" + GhiChu + "'," + dt.Rows[0]["Dot"]
        //                            + "," + dt.Rows[0]["Ky"] + "," + dt.Rows[0]["Nam"] + "," + dt.Rows[0]["Phuong"] + "," + dt.Rows[0]["Quan"] + "," + MaNV + ",getdate(),N'" + dt.Rows[0]["HopDong"] + "'"
        //                            + ")";
        //                        result.success = _cDAL_sDHN.ExecuteNonQuery(sql);
        //                        if (Hinh != "")
        //                            result.success = ghi_Hinh_DonTu_Func(ID, Hinh, MaNV);
        //                        MHoaDon hd = new MHoaDon();
        //                        hd.TieuThu = int.Parse(ID);
        //                        result.message = CGlobalVariable.jsSerializer.Serialize(hd);
        //                    }
        //                    else
        //                    {
        //                        result.success = false;
        //                        result.error = "Danh Bộ đã lập Đơn cùng nội dung trong ngày";
        //                    }
        //                }
        //            }
        //            else
        //                result.success = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //[Route("getDS_DonTu")]
        //[HttpGet]
        //public MResult getDS_DonTu(string DanhBo)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select CreateDate=CONVERT(char(10),CreateDate,103),NoiDung,TinhTrang from MaHoa_DonTu where DanhBo='" + DanhBo + "' order by CreateDate desc"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //private bool ghi_Hinh_241(string pathroot, string FolderLoai, string FolderIDCT, string FileName, string HinhDHN)
        //{
        //    try
        //    {
        //        if (Directory.Exists(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT) == false)
        //            Directory.CreateDirectory(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT);
        //        if (File.Exists(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName) == true)
        //            File.Delete(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName);
        //        byte[] hinh = System.Convert.FromBase64String(HinhDHN);
        //        File.WriteAllBytes(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName, hinh);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //[Route("ghi_Hinh_DonTu")]
        //[HttpGet]
        //public MResult ghi_Hinh_DonTu(string ID, string Hinh, string MaNV)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        string filename = DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss");
        //        string sql = "insert into MaHoa_DonTu_Hinh(ID,IDParent,Name,Loai,CreateBy,CreateDate)values((select case when exists(select ID from MaHoa_DonTu_Hinh) then (select MAX(ID)+1 from MaHoa_DonTu_Hinh) else 1 end)," + ID + ",N'" + filename + "',N'.jpg'," + MaNV + ",getdate())";
        //        _cDAL_sDHN.ExecuteNonQuery(sql);
        //        result.success = ghi_Hinh_241(CGlobalVariable.pathHinhDHNMaHoa, "DonTu", ID, filename + ".jpg", Hinh);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //private bool ghi_Hinh_DonTu_Func(string ID, string Hinh, string MaNV)
        //{
        //    try
        //    {
        //        string filename = DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss");
        //        string sql = "insert into MaHoa_DonTu_Hinh(ID,IDParent,Name,Loai,CreateBy,CreateDate)values((select case when exists(select ID from MaHoa_DonTu_Hinh) then (select MAX(ID)+1 from MaHoa_DonTu_Hinh) else 1 end)," + ID + ",N'" + filename + "',N'.jpg'," + MaNV + ",getdate())";
        //        _cDAL_sDHN.ExecuteNonQuery(sql);
        //        return ghi_Hinh_241(CGlobalVariable.pathHinhDHNMaHoa, "DonTu", ID, filename + ".jpg", Hinh);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ////hình đhn
        //[Route("checkExists_HinhDHN")]
        //[HttpGet]
        //public MResult checkExists_HinhDHN(string ID)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.success = checkExists_HinhDHN_Func(ID);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //private bool checkExists_HinhDHN_Func(string ID)
        //{
        //    try
        //    {
        //        string folder = CGlobalVariable.pathHinhDHN + @"\" + ID.Substring(0, 6);
        //        string filename = ID.Substring(6, 11) + ".jpg";
        //        return File.Exists(folder + @"\" + filename);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //[Route("get_HinhDHN")]
        //[HttpGet]
        //public MResult get_HinhDHN(string ID)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        byte[] hinh = get_HinhDHN_Func(ID);

        //        if (hinh == null)
        //            result.message = "";
        //        else
        //            result.message = Convert.ToBase64String(hinh);
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //private byte[] get_HinhDHN_Func(string ID)
        //{
        //    try
        //    {
        //        byte[] hinh = null;

        //        string sql = "SELECT top 1 Image " +
        //           "FROM DocSoTH_Hinh.dbo.HinhDHN " +
        //           "WHERE HinhDHNID =" + ID;
        //        if (hinh == null)
        //        {
        //            string folder = CGlobalVariable.pathHinhDHN + @"\" + ID.Substring(0, 6);
        //            string filename = ID.Substring(6, 11) + ".jpg";
        //            bool fileExists = File.Exists(folder + @"\" + filename);
        //            if (fileExists == true)
        //                hinh = File.ReadAllBytes(folder + @"\" + filename);
        //        }
        //        if (hinh.Length == 0)
        //            return null;
        //        else
        //            return hinh;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //[Route("ghi_HinhDHN")]
        //[HttpGet]
        //public MResult ghi_HinhDHN(string ID, string HinhDHN)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.success = ghi_HinhDHN_Func(ID, HinhDHN);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //public bool ghi_HinhDHN_Func(string ID, string HinhDHN)
        //{
        //    try
        //    {
        //        string folder = CGlobalVariable.pathHinhDHN + @"\" + ID.Substring(0, 6);
        //        string filename = ID.Substring(6, 11) + ".jpg";
        //        if (Directory.Exists(folder) == false)
        //            Directory.CreateDirectory(folder);
        //        if (File.Exists(folder + @"\" + filename) == true)
        //            File.Delete(folder + @"\" + filename);
        //        byte[] hinh = System.Convert.FromBase64String(HinhDHN);
        //        File.WriteAllBytes(folder + @"\" + filename, hinh);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //[Route("xoa_HinhDHN")]
        //[HttpGet]
        //public MResult xoa_HinhDHN(string ID)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        string folder = CGlobalVariable.pathHinhDHN + @"\" + ID.Substring(0, 6);
        //        string filename = ID.Substring(6, 11) + ".jpg";
        //        if (File.Exists(folder + @"\" + filename) == true)
        //            File.Delete(folder + @"\" + filename);
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}


        ////quản lý
        //[Route("getDS_TheoDoi")]
        //[HttpGet]
        //public MResult getDS_TheoDoi(string MaTo, string Nam, string Ky, string Dot)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable("select May,Tong=COUNT(DocSoID)"
        //            + " ,DaDoc=COUNT(CASE WHEN CodeMoi not like '' THEN 1 END)"
        //            + " ,ChuaDoc=COUNT(CASE WHEN CodeMoi like '' THEN 1 END)"
        //            + " ,CodeF=COUNT(CASE WHEN CodeMoi like 'F%' THEN 1 END)"
        //            + " from DocSo where Nam=" + Nam + " and Ky=" + Ky + " and Dot=" + Dot + " and (select TuMay from [To] where MaTo=" + MaTo + ")<=May and May<=(select DenMay from [To] where MaTo=" + MaTo + ")"
        //            + " group by May"));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        //[Route("getDS_BatThuong")]
        //[HttpGet]
        //public MResult getDS_BatThuong(string MaTo, string Nam, string Ky, string Dot)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        string sql = "DECLARE @LastNamKy INT;"
        //    + " declare @Nam int"
        //    + " declare @Ky char(2)"
        //    + " declare @Dot char(2)"
        //    + " declare @TuMay char(2)"
        //    + " declare @DenMay char(2)"
        //    + " set @Nam=" + Nam
        //    + " set @Ky='" + Ky + "'"
        //    + " set @Dot='" + Dot + "'"
        //    + " set @TuMay=RIGHT('0' + CAST((select TuMay from [To] where MaTo=" + MaTo + ") AS VARCHAR(2)), 2)"
        //    + " set @DenMay=RIGHT('0' + CAST((select DenMay from [To] where MaTo=" + MaTo + ") AS VARCHAR(2)), 2)"
        //    + " SET @LastNamKy = @Nam * 12  + @Ky;"
        //    + " IF (OBJECT_ID('tempdb.dbo.#ChiSo', 'U') IS NOT NULL) DROP TABLE #ChiSo;"
        //    + " SELECT DanhBa, MAX([ChiSo0]) AS [ChiSo0], MAX([ChiSo1]) AS [ChiSo1], MAX([ChiSo2]) AS [ChiSo2], MAX([Code0]) AS [Code0],"
        //    + "     MAX([Code1]) AS [Code1], MAX([Code2]) AS [Code2], MAX([TieuThu0]) AS [TieuThu0], MAX([TieuThu1]) AS [TieuThu1],"
        //    + "     MAX([TieuThu2]) AS [TieuThu2]"
        //    + "     INTO #ChiSo"
        //    + "     FROM ("
        //    + "         SELECT DanhBa, 'ChiSo'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS ChiSoKy, 'Code'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS CodeKy,"
        //    + "             'TieuThu'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS TieuThuKy, [CSCu], [CodeCu], [TieuThuCu]"
        //    + "             FROM [DocSoTH].[dbo].[DocSo]"
        //    + "             WHERE @LastNamKy-Nam*12-Ky between 0 and 2 and ((PhanMay>=@TuMay and PhanMay<=@DenMay) or (May>=@TuMay and May<=@DenMay))) src"
        //    + "     PIVOT (MAX([CSCu]) FOR ChiSoKy IN ([ChiSo0],[ChiSo1],[ChiSo2])) piv_cs"
        //    + "     PIVOT (MAX([CodeCu]) FOR CodeKy IN ([Code0],[Code1],[Code2])) piv_code"
        //    + "     PIVOT (MAX([TieuThuCu]) FOR TieuThuKy IN ([TieuThu0],[TieuThu1],[TieuThu2])) piv_tt"
        //    + "     GROUP BY DanhBa;"
        //    + "     with sdt as("
        //    + "        select g1.DanhBo"
        //    + "        , stuff(("
        //    + "            select ' | ' + g.DienThoai+' '+g.HoTen"
        //    + "            from CAPNUOCTANHOA.dbo.SDT_DHN g"
        //    + "            where g.DanhBo = g1.DanhBo and SoChinh=1"
        //    + "            order by CreateDate desc"
        //    + "            for xml path('')"
        //    + "        ),1,2,'') as DienThoai"
        //    + "        from CAPNUOCTANHOA.dbo.SDT_DHN g1"
        //    + "        group by g1.DanhBo)"
        //    + " select ds.DocSoID,MLT=kh.LOTRINH,DanhBo=ds.DanhBa,HoTen=kh.HOTEN,SoNha=kh.SONHA,TenDuong=kh.TENDUONG,ds.Nam,ds.Ky,ds.Dot,ds.PhanMay"
        //    + "                          ,Hieu=kh.HIEUDH,Co=kh.CODH,SoThan=kh.SOTHANDH,ViTri=VITRIDHN,ViTriNgoai=ViTriDHN_Ngoai,ViTriHop=ViTriDHN_Hop,bd.SH,bd.SX,bd.DV,HCSN=bd.HC,ds.TienNuoc,ThueGTGT=ds.Thue,PhiBVMT=ds.BVMT,PhiBVMT_Thue=ds.BVMT_Thue,TongCong=ds.TongTien"
        //    + "                          ,DiaChi=(select top 1 DiaChi=case when SO is null then DUONG else case when DUONG is null then SO else SO+' '+DUONG end end from server9.HOADON_TA.dbo.HOADON where DanhBa=ds.DanhBa order by ID_HOADON desc)"
        //    + "                          ,GiaBieu=bd.GB,DinhMuc=bd.DM,DinhMucHN=bd.DMHN,CSMoi,CodeMoi,TieuThuMoi,ds.TBTT,TuNgay=CONVERT(varchar(10),TuNgay,103),DenNgay=CONVERT(varchar(10),DenNgay,103),cs.*"
        //    + "                          ,kh.Gieng,kh.KhoaTu,kh.AmSau,kh.XayDung,kh.DutChi_Goc,kh.DutChi_Than,kh.NgapNuoc,kh.KetTuong,kh.LapKhoaGoc,kh.BeHBV,kh.BeNapMatNapHBV,kh.MauSacChiGoc,ds.ChuBao,DienThoai=sdt.DienThoai,kh.GhiChu"
        //    + "                          ,NgayThuTien=(select CONVERT(varchar(10),NgayThuTien,103) from Lich_DocSo ds,Lich_DocSo_ChiTiet dsct where ds.ID=dsct.IDDocSo and ds.Nam=@Nam and ds.Ky=@Ky and dsct.IDDot=@Dot)"
        //    + "                          ,TinhTrang=(select"
        //    + "                             case when exists (select top 1 MaKQDN from server9.HOADON_TA.dbo.TT_KQDongNuoc where MoNuoc=0 and TroNgaiMN=0 and DanhBo=ds.DanhBa order by NgayDN desc)"
        //    + "                             then (select top 1 N'Thu Tiền đóng nước: '+CONVERT(varchar(10),NgayDN,103)+' '+CONVERT(varchar(10),NgayDN,108) from server9.HOADON_TA.dbo.TT_KQDongNuoc where MoNuoc=0 and TroNgaiMN=0 and DanhBo=ds.DanhBa order by NgayDN desc)"
        //    + "                             else ''"
        //    + "                             end)"
        //    + "                          ,CuaHangThuHo=(select CuaHangThuHo1+CHAR(10)+case when CuaHangThuHo2 is null or CuaHangThuHo2=CuaHangThuHo1 then '' else CuaHangThuHo2 end from server9.HOADON_TA.dbo.TT_DichVuThu_DanhBo_CuaHang where DanhBo=ds.DanhBa)"
        //    + "                          from DocSo ds left join server8.CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG kh on ds.DanhBa=kh.DANHBO"
        //    + "                          left join BienDong bd on ds.DocSoID=bd.BienDongID"
        //    + "                          left join #ChiSo cs on ds.DanhBa=cs.DanhBa"
        //    + "                          left join sdt on sdt.DanhBo=ds.DanhBa"
        //    + "                          where ds.Nam=@Nam and ds.Ky=@Ky and ds.Dot=@Dot and ds.PhanMay>=@TuMay and ds.PhanMay<=@DenMay"
        //    + "                          and (ds.TieuThuMoi=0 or ds.TieuThuMoi>=TBTT*1.4 or ds.TieuThuMoi<=TBTT-TBTT*0.4) order by ds.MLT1 asc";
        //        result.message = DataTableToJSON(_cDAL_sDHN.ExecuteQuery_DataTable(sql));
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;

        //}

        ////send notification
        //[Route("SendNotificationToClient")]
        //[HttpGet]
        //public MResult SendNotificationToClient(string Title, string Content, string UID, string Action, string NameUpdate, string ValueUpdate, string ID)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        string serverKey = "AAAAEFkFujs:APA91bEkg2KLk53WsmZXHxTfU2AgElSDTq1GG5UsUAsCffgrXlex3wGU3rnp0iWX-GAgIm0JW9Qvq22aCQy0X-ns8LyrvPSHzmk1w2iSdK440VxRYHL9nOdNaKAAaAo_iXB7wlZCrdQi";
        //        string senderId = "70213024315";

        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //        request.Method = "POST";
        //        request.Headers.Add("Authorization", "key=" + serverKey);
        //        request.Headers.Add("Sender", "id=" + senderId);
        //        request.ContentType = "application/json";

        //        var data = new
        //        {
        //            to = UID,
        //            data = new
        //            {
        //                Title = Title,
        //                Body = Content,
        //                Action = Action,
        //                NameUpdate = NameUpdate,
        //                ValueUpdate = ValueUpdate,
        //                ID = ID,
        //            }
        //        };

        //        var json = CGlobalVariable.jsSerializer.Serialize(data);
        //        Byte[] byteArray = Encoding.UTF8.GetBytes(json);
        //        request.ContentLength = byteArray.Length;
        //        //gắn data post
        //        Stream dataStream = request.GetRequestStream();
        //        dataStream.Write(byteArray, 0, byteArray.Length);
        //        dataStream.Close();

        //        HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
        //        if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
        //        {
        //            StreamReader read = new StreamReader(respuesta.GetResponseStream());
        //            result.message = read.ReadToEnd();
        //            read.Close();
        //            respuesta.Close();
        //            _cDAL_sDHN.ExecuteNonQuery("insert into Temp(Name,Value,MaHD,Result)values(N'" + Title + "|" + Content + "|" + Action + "|" + NameUpdate + "|" + ValueUpdate + "',N'" + UID + "',N'" + ID + "',N'" + result.message + "')");
        //            result.success = true;
        //        }
        //        else
        //        {
        //            result.success = false;
        //            result.error = respuesta.StatusCode.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.success = false;
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}



    }
}