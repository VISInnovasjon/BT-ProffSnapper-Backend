using MiniExcelLibs;

namespace Util.Excel;

public void ReadExcel()
{
    var rows = MiniExcel.Query("../TestFiles/TEST_SHEET.xlsx");
    Console.WriteLine(rows);
}
