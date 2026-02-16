using HelpdeskDAL;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace HelpdeskWebsite.Reports
{
    public class CallReport
    {
        public async Task GenerateReportAsync(string rootpath, HelpdeskContext context)
        {
            // Use standard A4 page size for consistency
            PageSize pg = PageSize.A4.Rotate();
            var helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);

            // Create PDF writer and document
            PdfWriter writer = new(rootpath + "/pdfs/CallReport.pdf",
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new(writer);
            Document document = new(pdf, pg);

            // Adjusted logo position for landscape layout
            document.Add(new Image(ImageDataFactory.Create(rootpath + "/img/logo.jpeg"))
                .ScaleAbsolute(200, 100) // Adjusted size for visibility
                .SetFixedPosition((pg.GetWidth() - 200) / 2, pg.GetHeight() - 120)); 


            // Add spacing below the logo
            document.Add(new Paragraph("\n")); // Matches the EmployeeReport spacing
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));

            // Add title
            document.Add(new Paragraph("Current Calls")
                .SetFont(helvetica)
                .SetFontSize(24)
                .SetTextAlignment(TextAlignment.CENTER));

            // Add spacing below the title
            document.Add(new Paragraph("\n"));

            // Create a table with 6 columns (No Borders)
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 15, 15, 15, 20, 15, 20 }))
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);

            // Add headers (No Borders)
            table.AddCell(new Cell().Add(new Paragraph("Opened")
                .SetFontSize(16)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Last Name")
                .SetFontSize(16)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Tech")
                .SetFontSize(16)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Problem")
                .SetFontSize(16)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Status")
                .SetFontSize(16)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Closed")
                .SetFontSize(16)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));

            // Get all calls from the database
            var callViewModel = new CallViewModel();
            List<CallViewModel> calls = await callViewModel.GetAll(context);

            foreach (var call in calls)
            {
                table.AddCell(new Cell().Add(new Paragraph(call.DateOpened.ToShortDateString())
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.EmployeeName ?? "N/A")
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.TechName ?? "N/A")
     .SetFontSize(14)
     .SetTextAlignment(TextAlignment.CENTER))
     .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.ProblemDescription ?? "N/A")
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.OpenStatus ? "Open" : "Closed")
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.DateClosed?.ToShortDateString() ?? "-")
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(Border.NO_BORDER));
            }

            // Add table to the document
            document.Add(table);

            // Add footer
            float bottomY = 20;
            document.Add(new Paragraph("Call report written on - " + DateTime.Now)
                .SetFontSize(8)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition((pg.GetWidth() - 200) / 2, bottomY, 200));

            // Close the document
            document.Close();
        }


    }
}
