using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;

namespace WebWeather.Services
{
    public static class ExcelTransformer
    {
        public static IEnumerable<IWorkbook> TransformFilesToExcel(IFormFileCollection files)
        {
            foreach (var file in files)
            {
                using (var stream = file.OpenReadStream())
                {
                    var excelBook = new XSSFWorkbook(stream);
                    yield return excelBook;
                }
            }
        }
    }
}
