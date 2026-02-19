using ClosedXML.Excel;
using Space.Service;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using OxyPlot.Wpf;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ClosedXML.Excel.Drawings;

public static class ExcelReportService
{
    public static string CreateReport(
     List<Space.Service.RowData> rows,
     double coefAX, double coefBX,
     double coefAY, double coefBY,
     PlotView signalPlot = null,
     PlotView comparisonPlot = null,
     string filePath = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Не указан путь для сохранения отчета.", nameof(filePath));

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Отчет");

        // Заголовки и данные
        sheet.Cell(1, 1).Value = "Время";
        sheet.Cell(1, 2).Value = "До калибровки X";
        sheet.Cell(1, 3).Value = "После калибровки X";
        sheet.Cell(1, 4).Value = "До калибровки Y";
        sheet.Cell(1, 5).Value = "После калибровки Y";
        sheet.Range(1, 1, 1, 5).Style.Font.Bold = true;

        for (int i = 0; i < rows.Count; i++)
        {
            sheet.Cell(i + 2, 1).Value = rows[i].Time;
            sheet.Cell(i + 2, 2).Value = rows[i].Sun1X;
            sheet.Cell(i + 2, 3).Value = rows[i].Sun1X_C;
            sheet.Cell(i + 2, 4).Value = rows[i].Sun1Y;
            sheet.Cell(i + 2, 5).Value = rows[i].Sun1Y_C;
        }

        // Коэффициенты
        sheet.Cell("G2").Value = "Коэффициенты калибровки X";
        sheet.Cell("G2").Style.Font.Bold = true;
        sheet.Cell("G3").Value = "A:"; sheet.Cell("H3").Value = coefAX;
        sheet.Cell("G4").Value = "B:"; sheet.Cell("H4").Value = coefBX;

        sheet.Cell("G6").Value = "Коэффициенты калибровки Y";
        sheet.Cell("G6").Style.Font.Bold = true;
        sheet.Cell("G7").Value = "A:"; sheet.Cell("H7").Value = coefAY;
        sheet.Cell("G8").Value = "B:"; sheet.Cell("H8").Value = coefBY;

        sheet.Columns().AdjustToContents();

        workbook.SaveAs(filePath);
        return filePath;
    }


    // Метод конвертации PlotView в картинку и вставка в Excel
    private static void InsertPlotViewToExcel(IXLWorksheet sheet, PlotView plotView, string cellAddress)
    {
        if (plotView == null) return;

        var bitmap = new RenderTargetBitmap(
            (int)plotView.ActualWidth, (int)plotView.ActualHeight,
            96, 96, PixelFormats.Pbgra32);
        bitmap.Render(plotView);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));

        using var stream = new MemoryStream();
        encoder.Save(stream);
        stream.Seek(0, SeekOrigin.Begin);

        sheet.AddPicture(stream)
             .MoveTo(sheet.Cell(cellAddress))
             .WithPlacement(XLPicturePlacement.Move);
    }
}
