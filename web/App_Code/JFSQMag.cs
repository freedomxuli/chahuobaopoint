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
    public object GetList(int pagnum, int pagesize,string yhm,string xm)
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
                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"select a.*,b.UserName,b.UserXM from tb_b_jfsq a left join tb_b_user b on a.userId=b.userId where 1=1  ";
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

                if (issq == 1)
                {
                    string str = "select * from tb_b_jfsq where sqId=" + dbc.ToSqlValue(sqId);
                    DataTable sdt = dbc.ExecuteDataTable(str);

                    var userId = "";
                    if (sdt.Rows.Count > 0)
                    {
                        userId = sdt.Rows[0]["UserID"].ToString();
                        decimal points = 0;
                        str = "select * from tb_b_user where UserID=" + dbc.ToSqlValue(userId);
                        DataTable pdt = dbc.ExecuteDataTable(str);
                        if (pdt.Rows.Count > 0)
                        {
                            points += Convert.ToDecimal(pdt.Rows[0]["Points"].ToString());
                        }

                        points = points + Convert.ToDecimal(sdt.Rows[0]["sqjf"].ToString());

                        var udt = dbc.GetEmptyDataTable("tb_b_user");
                        var udtt = new SmartFramework4v2.Data.DataTableTracker(udt);
                        var usr = udt.NewRow();
                        usr["UserID"] = new Guid(userId);
                        usr["Points"] = points;
                        udt.Rows.Add(usr);
                        dbc.UpdateTable(udt, udtt);
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

    [CSMethod("GetJFSQToFile", 2)]
    public byte[] GetJFSQToFile(string yhm, string xm)
    {
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
                cells[0, 1].PutValue("用户名");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("申请日期");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("申请运费券");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("是否授权");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);

                string where = "";
                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"select a.*,b.UserName,b.UserXM from tb_b_jfsq a left join tb_b_user b on a.userId=b.userId where 1=1  ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by  a.issq,a.sqrq desc");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 1].SetStyle(style4);
                    string sqrq = "";
                    if (dt.Rows[i]["sqrq"] != null && dt.Rows[i]["sqrq"].ToString() != "")
                    {
                        sqrq = Convert.ToDateTime(dt.Rows[i]["sqrq"]).ToString("yyyy-MM-dd");
                    }
                    cells[i + 1, 2].PutValue(sqrq);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["sqjf"]);
                    cells[i + 1, 3].SetStyle(style4);
                    var issq = "";
                    if (dt.Rows[i]["issq"] != null && dt.Rows[i]["issq"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["issq"]) == 0)
                        {
                            issq = "否";
                        }
                        else
                        {
                            issq = "是";
                        }
                    }
                    cells[i + 1, 4].PutValue(issq);
                    cells[i + 1, 4].SetStyle(style4);
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
