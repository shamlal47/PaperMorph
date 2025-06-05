using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;




[ApiController]
[Route("api/[controller]")]
public class ImageToPdfController : ControllerBase
{
    [HttpPost("convert")]
    public IActionResult ConvertImageToPdf(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return BadRequest("Image file is missing.");

        using var stream = new MemoryStream();
        imageFile.CopyTo(stream);
        stream.Position = 0;

        using var image = Image.FromStream(stream);
        var pdf = new PdfDocument();
        var page = pdf.AddPage();
        page.Width = image.Width;
        page.Height = image.Height;

        using var gfx = XGraphics.FromPdfPage(page);
        using var img = XImage.FromStream(() => new MemoryStream(stream.ToArray()));
        gfx.DrawImage(img, 0, 0);

        using var output = new MemoryStream();
        pdf.Save(output);
        output.Position = 0;

        return File(output.ToArray(), "application/pdf", "converted.pdf");
    }
}
