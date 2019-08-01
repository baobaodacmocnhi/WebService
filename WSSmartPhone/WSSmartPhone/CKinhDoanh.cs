using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace WSSmartPhone
{
    public class CKinhDoanh
    {
        CConnection _DAL = new CConnection("Data Source=serverg8-01;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=db11@tanhoa");

        public DataTable GetDSGiaNuoc()
        {
            return _DAL.ExecuteQuery_DataTable("select * from GiaNuoc");
        }

        public string getDanhBo_CatTam(string ID)
        {
            object result=_DAL.ExecuteQuery_ReturnOneValue("select DanhBo from CHDB_ChiTietCatTam where MaCTCTDB=" + ID);
            if (result == null)
                return "";
            else
                return result.ToString();
        }

        public string getDanhBo_CatHuy(string ID)
        {
            object result = _DAL.ExecuteQuery_ReturnOneValue("select DanhBo from CHDB_ChiTietCatHuy where MaCTCHDB=" + ID);
            if (result == null)
                return "";
            else
                return result.ToString();
        }
    }
}