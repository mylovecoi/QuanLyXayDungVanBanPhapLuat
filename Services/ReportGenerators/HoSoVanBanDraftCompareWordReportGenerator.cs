using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Services.Manages;

namespace Services.ReportGenerators
{
    public static class HoSoVanBanDraftCompareWordReportGenerator
    {
        public static byte[] GenerateReport(HoSoVanBanDraftCompareModel model)
        {
            using var stream = new MemoryStream();
            using (var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
                var body = mainPart.Document.Body!;

                body.Append(
                    CreateCenteredParagraph("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM", true, "26"),
                    CreateCenteredParagraph("Độc lập - Tự do - Hạnh phúc", true, "24"),
                    CreateCenteredParagraph("BIÊN BẢN SO SÁNH DỰ THẢO VĂN BẢN", true, "30"),
                    CreateParagraph(string.Empty, false));

                body.Append(
                    CreateParagraph($"1. Hồ sơ: {model.TenHoSo}", false),
                    CreateParagraph($"2. Loại văn bản: {model.TenLoaiVanBan ?? "-"}", false),
                    CreateParagraph($"3. Quy trình: {model.TenQuyTrinh ?? "-"}", false),
                    CreateParagraph($"4. File gốc: {model.SourceFile?.NhanHienThi ?? "-"}", false),
                    CreateParagraph($"5. File so sánh: {model.TargetFile?.NhanHienThi ?? "-"}", false),
                    CreateParagraph(string.Empty, false));

                body.Append(CreateParagraph("I. Tổng hợp kết quả so sánh", true, "24"));
                body.Append(CreateSummaryTable(model));
                body.Append(CreateParagraph(string.Empty, false));
                body.Append(CreateParagraph("II. Bảng đối chiếu chi tiết", true, "24"));

                var table = CreateTable();
                table.Append(CreateHeaderRow());

                foreach (var row in model.DiffRows)
                {
                    table.Append(new TableRow(
                        CreateCell(row.Index.ToString(), null),
                        CreateCell(row.LeftText, ResolveCellShading(row.Status, true)),
                        CreateCell(row.RightText, ResolveCellShading(row.Status, false))));
                }

                body.Append(table);
                mainPart.Document.Save();
            }

            return stream.ToArray();
        }

        private static Paragraph CreateParagraph(string text, bool bold, string? fontSize = "24")
        {
            return new Paragraph(
                new Run(
                    new RunProperties(
                        new Bold { Val = bold },
                        new FontSize { Val = fontSize }),
                    new Text(text) { Space = SpaceProcessingModeValues.Preserve }));
        }

        private static Paragraph CreateCenteredParagraph(string text, bool bold, string? fontSize = "24")
        {
            return new Paragraph(
                new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                new Run(
                    new RunProperties(
                        new Bold { Val = bold },
                        new FontSize { Val = fontSize }),
                    new Text(text) { Space = SpaceProcessingModeValues.Preserve }));
        }

        private static Table CreateTable()
        {
            return new Table(
                new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 4 },
                        new BottomBorder { Val = BorderValues.Single, Size = 4 },
                        new LeftBorder { Val = BorderValues.Single, Size = 4 },
                        new RightBorder { Val = BorderValues.Single, Size = 4 },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 })),
                new TableGrid(
                    new GridColumn { Width = "900" },
                    new GridColumn { Width = "4200" },
                    new GridColumn { Width = "4200" }));
        }

        private static TableRow CreateHeaderRow()
        {
            return new TableRow(
                CreateCell("Dòng", "D9E2F3", true),
                CreateCell("Văn bản gốc", "D9E2F3", true),
                CreateCell("Văn bản so sánh", "D9E2F3", true));
        }

        private static Table CreateSummaryTable(HoSoVanBanDraftCompareModel model)
        {
            var table = new Table(
                new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 4 },
                        new BottomBorder { Val = BorderValues.Single, Size = 4 },
                        new LeftBorder { Val = BorderValues.Single, Size = 4 },
                        new RightBorder { Val = BorderValues.Single, Size = 4 },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 })),
                new TableGrid(
                    new GridColumn { Width = "3000" },
                    new GridColumn { Width = "1800" },
                    new GridColumn { Width = "3000" },
                    new GridColumn { Width = "1800" }));

            table.Append(new TableRow(
                CreateCell("Tổng số dòng", "E5E7EB", true),
                CreateCell(model.TongSoDong.ToString(), null, true),
                CreateCell("Số dòng giống nhau", "E5E7EB", true),
                CreateCell(model.SoDongGiongNhau.ToString(), null, true)));

            table.Append(new TableRow(
                CreateCell("Số dòng thêm", "DCFCE7", true),
                CreateCell(model.SoDongThem.ToString(), "F0FDF4", true),
                CreateCell("Số dòng xóa", "FEE2E2", true),
                CreateCell(model.SoDongXoa.ToString(), "FFF1F2", true)));

            table.Append(new TableRow(
                CreateCell("Số dòng sửa", "FEF3C7", true),
                CreateCell(model.SoDongSua.ToString(), "FFFBEB", true),
                CreateCell("Ngày xuất báo cáo", "E5E7EB", true),
                CreateCell(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), null, true)));

            return table;
        }

        private static TableCell CreateCell(string text, string? fillColor, bool bold = false)
        {
            var cellProperties = new TableCellProperties();
            if (!string.IsNullOrWhiteSpace(fillColor))
            {
                cellProperties.Append(new Shading
                {
                    Val = ShadingPatternValues.Clear,
                    Color = "auto",
                    Fill = fillColor
                });
            }

            var paragraph = new Paragraph(
                new Run(
                    new RunProperties(new Bold { Val = bold }),
                    new Text(text ?? string.Empty) { Space = SpaceProcessingModeValues.Preserve }));

            return new TableCell(cellProperties, paragraph);
        }

        private static string? ResolveCellShading(string status, bool isLeftCell)
        {
            return status switch
            {
                "added" => isLeftCell ? null : "C9F7D7",
                "removed" => isLeftCell ? "FFE2E5" : null,
                "changed" => "FFF0B3",
                _ => null
            };
        }
    }
}
