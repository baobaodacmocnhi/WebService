using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
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

        public string DataTableToJSON(DataTable table)
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

        private bool checkExists(string DanhBo)
        {
            try
            {
                object result = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select * from sDHN where DanhBo='" + DanhBo + "'");
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

        [Route("updateDS_DHN")]
        [HttpGet]
        public bool updateDS_DHN(string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    updateDS_DHN_HoaSen();
                    updateDS_DHN_Rynan();
                    updateDS_DHN_Deviwas();
                    updateDS_DHN_PhamLam();
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

        private bool updateDS_DHN_HoaSen()
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
                    _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=1");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists(item["MaDanhbo"]) == false)
                            if (string.IsNullOrEmpty(item["SeriModule"]))
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item["MaDanhbo"] + "',1,1,0)");
                            else
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,IDLogger,Valid,CreateBy)values('" + item["MaDanhbo"] + "',1,'" + item["SeriModule"] + "',1,0)");
                        else
                            _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=1 where DanhBo='" + item["MaDanhbo"] + "' and IDNCC=1");
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool updateDS_DHN_Rynan()
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
                    _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=2");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists(item) == false)
                            _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item + "',2,1,0)");
                        else
                            _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=1 where DanhBo='" + item + "' and IDNCC=2");
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool updateDS_DHN_Deviwas()
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
                    _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=3");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists(item["MaDanhbo"]) == false)
                            if (string.IsNullOrEmpty(item["SeriModule"]))
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item["MaDanhbo"] + "',3,1,0)");
                            else
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,IDLogger,Valid,CreateBy)values('" + item["MaDanhbo"] + "',3,'" + item["SeriModule"] + "',1,0)");
                        else
                            _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=1,IDLogger='" + item["SeriModule"] + "' where DanhBo='" + item["MaDanhbo"] + "' and IDNCC=3");
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool updateDS_DHN_PhamLam()
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
                    _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=4");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists(item["wmid"]) == false)
                            if (string.IsNullOrEmpty(item["idlogger"]))
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item["wmid"] + "',4,1,0)");
                            else
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,IDLogger,Valid,CreateBy)values('" + item["wmid"] + "',4,'" + item["idlogger"] + "',1,0)");
                        else
                            _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=1,IDLogger='" + item["idlogger"] + "' where DanhBo='" + item["wmid"] + "' and IDNCC=4");
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [Route("get_All")]
        [HttpGet]
        public bool get_All(string Time, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    DataTable dt = _cDAL_DocSo.ExecuteQuery_DataTable("select a.DanhBo,IDNCC from sDHN a,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG b where Valid=1 and a.DanhBo=b.DanhBo and IDNCC=4 order by a.DanhBo");
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

        public bool get_All_HoaSen(string DanhBo, string Time)
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
                                string sql = "insert into sDHN_LichSu(ID,DanhBo,ChiSo,ThoiGianCapNhat,Loai)"
                                           + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                                           + ",'" + DanhBo + "'"
                                           + "," + item["Volume"]
                                           + ",'" + item["Time"] + "',N'All')";
                                _cDAL_DocSo.ExecuteNonQuery(sql);
                            }
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
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool get_All_Rynan(string DanhBo, string Time)
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
                                string sql = "insert into sDHN_LichSu(ID,DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                    + ",Longitude,Latitude,Altitude,ChuKyGui"
                                    + ",ThoiGianCapNhat,Loai)"
                                               + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                                               + ",'" + DanhBo + "'"
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
                                _cDAL_DocSo.ExecuteNonQuery(sql);
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
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool get_All_Deviwas(string DanhBo, string Time)
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
                            string sql = "insert into sDHN_LichSu(ID,DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                + ",Longitude,Latitude,Altitude,ChuKyGui"
                                + ",ThoiGianCapNhat,Loai)"
                                           + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                                           + ",'" + DanhBo + "'"
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
                            _cDAL_DocSo.ExecuteNonQuery(sql);
                        }
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

        public bool get_All_PhamLam(string DanhBo, string Time)
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
                            //if (item["flow"] != null)
                            //    LuuLuong = ((int)item["flow"]).ToString();
                            string sql = "insert into sDHN_LichSu(ID,DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                                + ",Longitude,Latitude,Altitude,ChuKyGui"
                                + ",ThoiGianCapNhat,Loai)"
                                           + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                                           + ",'" + DanhBo + "'"
                                           + "," + item["flow"]
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
                            _cDAL_DocSo.ExecuteNonQuery(sql);
                        }
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

        public bool get_ChiSoNuoc_PhamLam(string DanhBo, string Time)
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
                                _cDAL_DocSo.ExecuteNonQuery(sql);
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
            catch (Exception ex)
            {
                return false;
            }
        }

        public IList<ThongTinKhachHang> getDS_ThaysDHN(string TuNgay, string DenNgay, string checksum)
        {
            try
            {
                if (checksum == "DHC@2022")
                {
                    List<ThongTinKhachHang> lst = new List<ThongTinKhachHang>();
                    if (TuNgay != "" && DenNgay != "")
                    {
                        DateTime dateTu = DateTime.Parse(TuNgay), dateDen = DateTime.Parse(DenNgay);
                        DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("select DANHBO,HOTEN,DiaChi=SONHA+' '+TENDUONG,SOTHANDH,NGAYTHAY from TB_DULIEUKHACHHANG where CAST(NGAYTHAY as date)>='" + dateTu.ToString("yyyyMMdd") + "' and CAST(NGAYTHAY as date)<='" + dateDen.ToString("yyyyMMdd") + "'");
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

        [Route("getVersion")]
        [HttpGet]
        public MResult getVersion()
        {
            MResult result = new MResult();
            try
            {
                result.message = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select Version from DeviceConfig").ToString();
                result.success = true;
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
                result.success = false;
            }
            return result;
        }

        [Route("updateUID")]
        [HttpGet]
        public MResult updateUID(string MaNV, string UID)
        {
            MResult result = new MResult();
            try
            {
                result.success = _cDAL_DocSo.ExecuteNonQuery("update NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
                result.success = false;
            }
            return result;
        }

        [Route("DangNhap")]
        [HttpGet]
        public MResult DangNhap(string Username, string Password, string IDMobile, string UID)
        {
            MResult result = new MResult();
            try
            {
                object MaNV = null;
                MaNV = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select MaND from NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");
                if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
                    MaNV = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select MaND from NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and IDMobile='" + IDMobile + "' and An=0");

                if (MaNV == null || MaNV.ToString() == "")
                {
                    result.message = "Sai mật khẩu hoặc IDMobile";
                    result.success = false;
                }
                else
                {
                    //xóa máy đăng nhập MaNV khác
                    object MaNV_UID_Old = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");
                    if (MaNV_UID_Old != null && (int)MaNV_UID_Old > 0)
                        _cDAL_DocSo.ExecuteNonQuery("delete DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");

                    //if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
                    //{
                    //    DataTable dt = _cDAL.ExecuteQuery_DataTable("select UID from TT_DeviceSigned where MaNV=" + MaNV);
                    //    foreach (DataRow item in dt.Rows)
                    //    {
                    //        SendNotificationToClient("Thông Báo Đăng Xuất", "Hệ thống server gửi đăng xuất đến thiết bị", item["UID"].ToString(), "DangXuat", "DangXuat", "false", "");
                    //        _cDAL.ExecuteNonQuery("delete TT_DeviceSigned where UID='" + item["UID"].ToString() + "'");
                    //    }
                    //}

                    object MaNV_UID = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from DeviceSigned where MaNV='" + MaNV + "' and UID='" + UID + "'");
                    if (MaNV_UID != null)
                        if ((int)MaNV_UID == 0)
                            _cDAL_DocSo.ExecuteNonQuery("insert DeviceSigned(MaNV,UID,CreateDate)values(" + MaNV + ",'" + UID + "',getDate())");
                        else
                            _cDAL_DocSo.ExecuteNonQuery("update DeviceSigned set ModifyDate=getdate() where MaNV=" + MaNV + " and UID='" + UID + "'");

                    _cDAL_DocSo.ExecuteNonQuery("update NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);

                    result.message = DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable("select TaiKhoan,MatKhau,MaND,HoTen,May,Admin,Doi,ToTruong,MaTo,DienThoai from NguoiDung where MaND=" + MaNV));
                    result.success = true;
                }
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
                result.success = false;
            }
            return result;
        }

    }
}