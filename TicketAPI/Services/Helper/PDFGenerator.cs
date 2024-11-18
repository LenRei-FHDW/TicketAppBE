/*using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace TicketAPI.Services.Helper;

using PdfSharp.Drawing;
using PdfSharp.Pdf;

public class PDFGenerator
{
    public void GeneratePDF()
    {
        var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Arial", 20);
        gfx.DrawString("Hello, PDFsharp!", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
        var filename = "BasicExample.pdf";
        document.Save(filename);
        Console.WriteLine($"PDF created successfully! Check the file at: {filename}");
    }

    public void GenerateQrCode()
    {
        
    }
}*/
