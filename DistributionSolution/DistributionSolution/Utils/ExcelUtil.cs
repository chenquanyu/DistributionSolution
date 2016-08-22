using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace DistributionSolution.Utils
{
    public class ExcelUtil
    {
        public static List<List<string>> ReadSheet(string fileName, int sheetIndex, string startCell, string endCell)
        {
            var result = new List<List<string>>();
            Application app = new Application();
            Workbook book1 = app.Workbooks.Open(fileName, ReadOnly: true);
            Worksheet sheet1 = (Worksheet)book1.Sheets[sheetIndex];
            Range range = sheet1.get_Range(startCell, endCell);
            int a = range.Count;

            foreach (Range row in range.Rows)
            {
                var list = new List<string>();
                foreach (Range cell in row.Cells)
                {
                    list.Add(cell.Value2.ToString());
                }
                result.Add(list);
            }

            book1.Close();
            return result;
        }


    }
}
