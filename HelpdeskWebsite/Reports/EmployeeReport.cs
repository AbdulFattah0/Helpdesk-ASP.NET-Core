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
    public class EmployeeReport
    {
        public async Task GenerateReportAsync(string rootpath, HelpdeskContext context)
        {
            PageSize pg = PageSize.A4;
            var helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);
            PdfWriter writer = new(rootpath + "/pdfs/EmployeeReport.pdf",
                 new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new(writer);
            Document document = new(pdf);  // PageSize(595, 842) 
            document.Add(new Image(ImageDataFactory.Create(rootpath + "/img/logo.jpeg"))

                 .ScaleAbsolute(200, 100)
                .SetFixedPosition(((pg.GetWidth() - 200) / 2), 710));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Current Employee")
                .SetFont(helvetica)
                .SetFontSize(24)
              
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph(""));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 35, 35 })) 
                .SetWidth(UnitValue.CreatePercentValue(100)) 
                .SetTextAlignment(TextAlignment.CENTER)
                .SetRelativePosition(0, 0, 0, 0)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);
            table.AddCell(new Cell().Add(new Paragraph("Title")
                .SetFontSize(16)
                .SetBold()
                .SetMargin(0) 
                .SetPadding(0) 
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("First Name")
                .SetFontSize(16)
                .SetBold()
                .SetMargin(0)
                .SetPadding(0)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Last Name")
                .SetFontSize(16)
                .SetBold()
                .SetMargin(0)
                .SetPadding(0)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));


            var employeeViewModel = new EmployeeViewModel();
            List<EmployeeViewModel> employees = await employeeViewModel.GetAll(context);

            
            foreach (var employee in employees)
            {
                table.AddCell(new Cell().Add(new Paragraph(employee.Title)
                    .SetFontSize(14)
                    .SetMargin(0)
                    .SetPadding(0)
                    .SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(employee.Firstname)
                    .SetFontSize(14)
                    .SetMargin(0)
                    .SetPadding(0)
                    .SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(employee.Lastname)
                    .SetFontSize(14)
                    .SetMargin(0)
                    .SetPadding(0)
                    .SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(Border.NO_BORDER));
            }

            float bottomY = 20; 
            document.Add(new Paragraph("Employee report written on - " + DateTime.Now)
                .SetFontSize(8)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition((pg.GetWidth() - 200) / 2, bottomY, 200));

            document.Add(table);
            document.Close();


        }
    }
}
