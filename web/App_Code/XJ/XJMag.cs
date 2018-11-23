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
using Aspose.Cells;
using System.IO;

/// <summary>
/// XJMag 的摘要说明
/// </summary>
[CSClass("XJMag")]
public class XJMag
{
	public XJMag()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    [CSMethod("GetList")]
    public object GetList(int pagnum, int pagesize, string yhm)
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
                    where += " and " + dbc.C_Like("b.UserXM", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"  select a.*,b.UserName,b.UserXM from tb_b_plattosale a left join tb_b_user b on a.UserID=b.UserID
                        where a.status=0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.points desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SaveXJ")]
    public bool SaveXJ(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                var PlatToSaleId = jsr["PlatToSaleId"].ToString();
                var points = jsr["points"].ToString();
                var memo = jsr["memo"].ToString();

                string sql = "select points,UserID from tb_b_plattosale where PlatToSaleId = @PlatToSaleId";
                SqlCommand cmd = dbc.CreateCommand(sql);
                cmd.Parameters.Add("@PlatToSaleId", PlatToSaleId);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                sql = "update tb_b_plattosale set points = @points where PlatToSaleId = @PlatToSaleId";
                cmd = dbc.CreateCommand(sql);
                cmd.Parameters.Add("@points", Convert.ToDecimal(Convert.ToDecimal(dt.Rows[0]["points"]) - Convert.ToDecimal(points)));
                cmd.Parameters.Add("@PlatToSaleId", PlatToSaleId);
                dbc.ExecuteNonQuery(cmd);

                sql = "select Points,PlatPointId from tb_b_platpoints where UserID = @UserID and status = 0";
                cmd = dbc.CreateCommand(sql);
                cmd.Parameters.Add("@UserID", dt.Rows[0]["UserID"].ToString());
                DataTable dt_plat = dbc.ExecuteDataTable(cmd);

                sql = "update tb_b_platpoints set points = @points where PlatPointId = @PlatPointId";
                cmd = dbc.CreateCommand(sql);
                cmd.Parameters.Add("@points", Convert.ToDecimal(Convert.ToDecimal(dt_plat.Rows[0]["Points"]) + Convert.ToDecimal(points)));
                cmd.Parameters.Add("@PlatPointId", dt_plat.Rows[0]["PlatPointId"].ToString());
                dbc.ExecuteNonQuery(cmd);

                DataTable dt_xj = dbc.GetEmptyDataTable("tb_b_xj");
                DataRow dr = dt_xj.NewRow();
                dr["XJ_ID"] = Guid.NewGuid();
                dr["UserID"] = dt.Rows[0]["UserID"].ToString();
                dr["Points"] = points;
                dr["status"] = 0;
                dr["adduser"] = SystemUser.CurrentUser.UserID;
                dr["addtime"] = DateTime.Now;
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = DateTime.Now;
                dr["Memo"] = memo;
                dt_xj.Rows.Add(dr);
                dbc.InsertTable(dt_xj);

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