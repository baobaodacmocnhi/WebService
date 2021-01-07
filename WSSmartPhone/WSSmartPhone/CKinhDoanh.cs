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
            object result = _DAL.ExecuteQuery_ReturnOneValue("select DanhBo from CHDB_ChiTietCatTam where MaCTCTDB=" + ID);
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

        public bool ThemHinhDHN(string DanhBo, string CreateBy, string imageStr, string Latitude, string Longitude)
        {
            //int ID = 0;
            //if (int.Parse(_DAL.ExecuteQuery_DataTable("select COUNT(ID) from HinhDHN").Rows[0][0].ToString()) == 0)
            //    ID = 1;
            //else
            //    ID = int.Parse(_DAL.ExecuteQuery_DataTable("select MAX(ID)+1 from HinhDHN").Rows[0][0].ToString());
            //string sql = "insert into HinhDHN(ID,DanhBo,Hinh,CreateBy,CreateDate)values(" + ID + ",'" + DanhBo + "'," + image + "," + CreateBy + ",GETDATE())";
            //return _DAL.ExecuteNonQuery(sql);

            try
            {
                //dbDocSoDataContext db = new dbDocSoDataContext();
                //Byte[] image = System.Convert.FromBase64String(imageStr);

                //HinhDHN entity = new HinhDHN();
                //if (db.HinhDHNs.Count() == 0)
                //    entity.ID = 1;
                //else
                //    entity.ID = db.HinhDHNs.Max(item => item.ID) + 1;

                //entity.DanhBo = DanhBo;
                //entity.Image = image;
                //entity.Latitude = Latitude;
                //entity.Longitude = Longitude;
                //entity.CreateBy = int.Parse(CreateBy);
                //entity.CreateDate = DateTime.Now;
                //db.HinhDHNs.InsertOnSubmit(entity);
                //db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}