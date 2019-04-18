using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace TestServices
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Button1.Visible = false;
            Label11.Visible = false;
            MaintainScrollPositionOnPostBack = true;
            if (IsPostBack)
                return;
        }
        double Tongcong = 0;
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label Label6 = (Label)e.Row.FindControl("Label6");
                Tongcong += double.Parse(Label6.Text);
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                Label kn_DHN = (Label)e.Row.FindControl("Label9");
                kn_DHN.Text = String.Format("{0:0,0}", Tongcong); ;
            }
        }

        protected void btXemBangKe_Click(object sender, EventArgs e)
        {
            WSTH.THService se = new TestServices.WSTH.THService();

            DetailsView1.DataSource = se.getCustomerInfo(this.TextBox1.Text, "AGRIBANK", DateTime.Now.Date.Day.ToString());
            DetailsView1.DataBind();
            DataSet ds = se.W_Bill(this.TextBox1.Text, "AGRIBANK", DateTime.Now.Date.Day.ToString());
            if (ds.Tables[0].Rows.Count == 0)
            {
                Label11.Visible = true;
                GridView1.Visible = false;
            }
            else
            {
                Label11.Visible = false;
                GridView1.Visible = true;
            //    Button1.Visible = false;
                GridView1.DataSource = ds;
                GridView1.DataBind();
            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            ////foreach (GridViewRow row in GridView1.Rows)
            ////{
            ////    if (row.RowType == DataControlRowType.DataRow)
            ////    {
            ////        TextBox textBox = row.C .FindControl("TextBox10") as TextBox;
            ////        Label11.Visible = true;
            ////        Label11.Text = textBox.Text;

            ////    }
            ////}
            //for (int i = 0; i < GridView1.Rows.Count; i++)
            //{
            //    Label11.Visible = true;
            //    Label11.Text = ((TextBox)GridView1.Rows[i].FindControl("TextBox10")).Text;

            //}
            ////TextBox1.Text = GridView1.SelectedRow.Cells[0].Text;
            ////TextBox2.Text = ((Label)GridView1.SelectedRow.FindControl("Label1")).Text;


           string idhd = "";
            foreach (GridViewRow item in GridView1.Rows)
            {
                Label ID_HOADON = item.FindControl("Label7") as Label;
                idhd += ID_HOADON.Text+"#";
               
            }

            WSTH.THService se = new TestServices.WSTH.THService();
            bool res = se.payW_Bill(idhd.Substring(0, idhd.Length - 1), "AGRIBANK", DateTime.Now.Date.Day.ToString());
            if (res)
            {
                Label11.Visible = true;
                Label11.Text = "Thanh Toán Thành Công !";
            }
            else
            {
                Label11.Visible = true;
                Label11.Text = "Thanh Toán Không Thành Công !";
            }
        }
    }
}