using System;
using System.Collections;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace DailyReportFixApp
{
    class Excel
    {
        public string path = "";
        _Application excel;
        Workbook wb;
        Worksheet ws;

        public Excel()
        {
            CheckExcellProcesses();
            excel = new _Excel.Application();
        }

        public Excel(string path, int sheet)
        {
            CheckExcellProcesses();
            excel = new _Excel.Application();
            this.path = path;
            wb = excel.Workbooks.Open(path);
            ws = wb.Worksheets[sheet];
            ws.Columns.AutoFit();
        }

        public string ReadCell(int i, int j)
        {
            i++;
            j++;
            if(ws.Cells[i, j].Value2 != null)
            {
                string temp = Convert.ToString(ws.Cells[i, j].Value2);
                return temp;
            }
            else
            {
                return "";
            }
        }

        public void WriteFile(int col, int row, string str)
        {
            col++;
            row++;
            ws.Cells[col, row].Value2 = str;
            ws.Columns.AutoFit();
        }

        public void Save()
        {
            wb.Save();
        }

        public void SaveAs(string path)
        {
            wb.SaveAs(path);
        }

        Hashtable myHashtable;

        public void CheckExcellProcesses()
        {
            Process[] AllProcesses = Process.GetProcessesByName("EXCEL");
            myHashtable = new Hashtable();
            int iCount = 0;

            foreach (Process ExcelProcess in AllProcesses)
            {
                myHashtable.Add(ExcelProcess.Id, iCount);
                iCount++;
            }
        }

        private void KillExcel()
        {
            Process[] AllProcesses = Process.GetProcessesByName("EXCEL");

            foreach (Process ExcelProcess in AllProcesses)
            {
                if (myHashtable.ContainsKey(ExcelProcess.Id) == false)
                    ExcelProcess.Kill();
            }
        }

        public void Close()
        {
            KillExcel();
        }

        public void LoadFile(string path)
        {
            this.path = path;
        }

        public void CreateFile(int nrLines)
        {
            wb = excel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            ws = wb.Worksheets[1];

            Range rg = (Range)ws.Range[ws.Cells[1, 1], ws.Cells[nrLines + 1, 7]];
            Range rg1 = (Range)ws.Cells[nrLines + 1, 2];
            Range rg2 = (Range)ws.Range[ws.Cells[nrLines + 1, 5], ws.Cells[nrLines + 1, 8]];
            rg.Cells.HorizontalAlignment = XlHAlign.xlHAlignLeft;

            rg1.Borders.Value = true;
            rg2.Borders.Value = true;
            rg1.Borders.Weight = 4;
            rg2.Borders.Weight = 4;
        }
    }
}
