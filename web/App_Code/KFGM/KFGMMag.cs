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

                    string sql = @"select distinct b.UserID,b.OpenID from tb_b_user_gz a left join tb_b_user b on a.UserId=b.UserID
                        where a.GZUserID=" + dbc.ToSqlValue(userid) + " and b.OpenID is not null";
                    DataTable gzdt = dbc.ExecuteDataTable(sql);

                    if (gzdt.Rows.Count > 0)
                    {
                        for (int i = 0; i < gzdt.Rows.Count; i++)
                        {
                            try
                            {
                                new Handler().SendWeText(gzdt.Rows[i]["OpenID"].ToString(), wlmc + "已开放“" + jsr["discountmemo"].ToString() + "”的运费券，请速度登录进行抢购，手快有，手慢无！");
                            }
                            catch (Exception ex)
                            { 
                                
                            }
                        }
                    }

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

    [CSMethod("GetKFGMToFile", 2)]
    public byte[] GetKFGMToFile(string yhm){
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                Workbook workbook = new Workbook(); //工作簿
                Worksheet sheet = workbook.Worksheets[0]; //工作表
                Cells cells = sheet.Cells;//单元格

                //为标题设置样式  
                Style styleTitle = workbook.Styles[workbook.Styles.Add()];
                styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                styleTitle.Font.Name = "宋体";//文字字体
                styleTitle.Font.Size = 18;//文字大小
                styleTitle.Font.IsBold = true;//粗体

                //样式1
                Style style1 = workbook.Styles[workbook.Styles.Add()];
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.Font.IsBold = true;//粗体

                //样式2
                Style style2 = workbook.Styles[workbook.Styles.Add()];
                style2.HorizontalAlignment = TextAlignmentType.Left;//文字居中
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 14;//文字大小
                style2.Font.IsBold = true;//粗体
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //应用边界线 左边界线
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //应用边界线 右边界线
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin; //应用边界线 上边界线
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //应用边界线 下边界线
                style2.IsLocked = true;

                //样式3
                Style style4 = workbook.Styles[workbook.Styles.Add()];
                style4.HorizontalAlignment = TextAlignmentType.Left;//文字居中
                style4.Font.Name = "宋体";//文字字体
                style4.Font.Size = 11;//文字大小
                style4.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;


                cells.SetRowHeight(0, 20);
                cells[0, 0].PutValue("物流名称");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);

                string where="";
                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserXM", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"  select a.*,b.UserName,b.UserXM from tb_b_platpoints a left join tb_b_user b on a.UserID=b.UserID
                        where a.status=0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by a.Points ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["Points"]);
                    cells[i + 1, 1].SetStyle(style4);
                }

                MemoryStream ms = workbook.SaveToStream();
                byte[] bt = ms.ToArray();
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

}
