using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSSmartPhone
{
    public class CDHN
    {
        CConnection _DAL = new CConnection("Data Source=hp_g7\\kd;Initial Catalog=CAPNUOCTANHOA;Persist Security Info=True;User ID=sa;Password=db8@tanhoa");

        public string getPhuongQuan(string DanhBo)
        {
            string sql = "select ' P.'+p.TENPHUONG+' Q.'+q.TENQUAN from TB_DULIEUKHACHHANG dlkh,PHUONG p,QUAN q"
                    + " where DANHBO='" + DanhBo + "' and dlkh.QUAN=q.MAQUAN and dlkh.PHUONG=p.MAPHUONG"
                    + " and p.MAQUAN=q.MAQUAN";
            object result = _DAL.ExecuteQuery_ReturnOneValue(sql);
            if (result != null)
                return result.ToString();
            else
                return "";
        }
    }
}