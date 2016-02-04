using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using ExcelApplication = Microsoft.Office.Interop.Excel.DataTable;
namespace SAS.ClassSet.FunctionTools
{
    class ExcelHelper
    {
        public static void UWriteListViewToExcel(ListView LView, string way, string strTitle)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                object m_objOpt = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Workbooks ExcelBooks = (Microsoft.Office.Interop.Excel.Workbooks)ExcelApp.Workbooks;
                Microsoft.Office.Interop.Excel._Workbook ExcelBook = (Microsoft.Office.Interop.Excel._Workbook)(ExcelBooks.Add(m_objOpt));
                Microsoft.Office.Interop.Excel._Worksheet ExcelSheet = (Microsoft.Office.Interop.Excel._Worksheet)ExcelBook.ActiveSheet;
                ExcelSheet.Columns.EntireColumn.AutoFit();//列宽自适应
                //设置标题
                ExcelApp.Caption = strTitle;
                ExcelSheet.Cells[1, 1] = strTitle;
                
                Range rg3 = ExcelSheet.get_Range("A1", "E1");
                rg3.MergeCells = true;
                rg3.set_Value(Type.Missing, strTitle);
                rg3.Font.Bold = true;
                rg3.Font.Size = 18;
                rg3.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                //写入列名
                for (int i = 1; i <= LView.Columns.Count; i++)
                {
                    ExcelSheet.Cells[2, i] = LView.Columns[i - 1].Text;
                  
                }
              
                //写入内容
                for (int i = 3; i < LView.Items.Count + 3; i++)
                {
                    ExcelSheet.Cells[i, 1] = LView.Items[i - 3].Text;
                    for (int j = 2; j <= LView.Columns.Count; j++)
                    {
                        ExcelSheet.Cells[i, j] = LView.Items[i - 3].SubItems[j - 1].Text;
                        
                    }
                }
                for (int i=1;i<LView.Items.Count+3;i++)
                {
                 string startcell=string.Format("A{0}",i.ToString());
                 string endcell=string.Format("E{0}",i.ToString());
                 Range r = ExcelSheet.get_Range(startcell, endcell);
                 r.Font.Bold = true;
                 r.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                 r.HorizontalAlignment = XlVAlign.xlVAlignCenter;
                }
               
                Range range = ExcelSheet.Columns;
                range.Columns.AutoFit();
                ExcelBook.SaveAs(way, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                //显示Excel
                //ExcelApp.Visible = true;
                ExcelApp.Quit();
                GC.Collect();
                MessageBox.Show("导出数据成功!", "系统信息");
            }
            catch (SystemException e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
