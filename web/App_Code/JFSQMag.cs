using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartFramework4v2.Web.WebExecutor;
using System.Data;
using SmartFramework4v2.Data.SqlServer;
using System.Text;
using System.Data.SqlClient;
using SmartFramework4v2.Web.Common.JSON;
using SmartFramework4v2.Data;

/// <summary>
///JFSQMag 的摘要说明
/// </summary>
[CSClass("JFSQMag")]
public class JFSQMag
{
    public JFSQMag()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }


    [CSMethod("GetList")]
    public object GetList(int pagnum, int pagesize,string yhm)
    {

        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"select a.*,b.UserName from tb_b_jfsq a left join tb_b_user b on a.userId=b.userId where 1=1  ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by  a.issq,a.sqrq desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("JFSQ")]
    public object JFSQ(string sqId, int issq)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var dt = dbc.GetEmptyDataTable("tb_b_jfsq");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var sr = dt.NewRow();
                sr["sqId"] = new Guid(sqId);
                sr["issq"] = issq;
                sr["shtime"] = DateTime.Now;
                sr["shuserId"] = SystemUser.CurrentUser.UserID;
                dt.Rows.Add(sr);
                dbc.UpdateTable(dt, dtt);

                string str = "select * from tb_b_jfsq where sqId=" + dbc.ToSqlValue(sqId);
                DataTable sdt = dbc.ExecuteDataTable(str);

                var userId = "";
                if (sdt.Rows.Count > 0)
                {
                    userId = sdt.Rows[0]["UserID"].ToString();
                    decimal points = 0;
                    points = points + Convert.ToDecimal(sdt.Rows[0]["sqjf"].ToString());

                    var udt = dbc.GetEmptyDataTable("tb_b_user");
                    var udtt = new SmartFramework4v2.Data.DataTableTracker(udt);
                    var usr = udt.NewRow();
                    usr["UserID"] = new Guid(userId);
                    usr["Points"] = points;
                    udt.Rows.Add(usr);
                    dbc.UpdateTable(udt, udtt);
                }
               
                dbc.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }
}
