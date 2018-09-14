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
///KFGMMag 的摘要说明
/// </summary>
[CSClass("KFGMMag")]
public class KFGMMag
{
    public KFGMMag()
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
                    where += " and " + dbc.C_Like("b.UserXM", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"  select a.*,b.UserName,b.UserXM from tb_b_platpoints a left join tb_b_user b on a.UserID=b.UserID
                        where a.status=0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.Points ", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SaveKFGM")]
    public bool SaveUser(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var PlatPointId = jsr["PlatPointId"].ToString();
                string str = "select * from tb_b_platpoints where PlatPointId=" + dbc.ToSqlValue(PlatPointId);
                DataTable pdt = dbc.ExecuteDataTable(str);

                if (pdt.Rows.Count>0)
                {
                    var userid = pdt.Rows[0]["UserID"].ToString();

                    var saledt = dbc.GetEmptyDataTable("tb_b_plattosale");
                    var saledr = saledt.NewRow();
                    saledr["PlatToSaleId"] = Guid.NewGuid().ToString();
                    saledr["UserID"] = userid;
                    saledr["points"] = Convert.ToDecimal(jsr["points"].ToString());
                    saledr["addtime"] = DateTime.Now;
                    saledr["status"] = 0;
                    saledr["discount"] = Convert.ToDecimal(jsr["discount"].ToString());
                    saledr["discountmemo"] = jsr["discountmemo"].ToString();
                    //saledr["memo"] = ;
                    saledt.Rows.Add(saledr);
                    dbc.InsertTable(saledt);

                    var dt = dbc.GetEmptyDataTable("tb_b_platpoints");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["PlatPointId"] = PlatPointId;
                    var point = Convert.ToDecimal(pdt.Rows[0]["Points"].ToString()) - Convert.ToDecimal(jsr["points"].ToString());
                    dr["Points"] = point;
                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);

                    str = "select * from tb_b_user where UserID='" + userid + "'";
                    DataTable udt = dbc.ExecuteDataTable(str);

                    var wlmc = "";
                    if (udt.Rows.Count > 0)
                    {
                        wlmc = udt.Rows[0]["UserXM"].ToString();
                    }

                    var logdt = dbc.GetEmptyDataTable("tb_b_record");
                    var logdr = logdt.NewRow();
                    logdr["CaoZuoJiLuID"]=Guid.NewGuid().ToString();
                    logdr["UserID"]=SystemUser.CurrentUser.UserID;
                    logdr["CaoZuoLeiXing"]="开放购买积分";
                    logdr["CaoZuoNeiRong"] = SystemUser.CurrentUser.UserName + DateTime.Now + "开放购买" + wlmc + jsr["points"]+"积分";
                    logdr["CaoZuoTime"]=DateTime.Now;
                    //logdr["CaoZuoRemark"]
                    logdt.Rows.Add(logdr);
                    dbc.InsertTable(logdt);
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
