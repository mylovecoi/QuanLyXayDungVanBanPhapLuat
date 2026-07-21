using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Services.DTOs.BaoCaoKhac;

namespace Services.ReportGenerators
{
    public static class WordReportGenerator
    {
        public static byte[] GenerateReport(string templatePath, BaoCaoSuDungLaoDongResponse response)
        {
            // Kiểm tra file mẫu có tồn tại không
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Không tìm thấy file mẫu: {templatePath}");
            }

            try
            {
                // Tạo file output trong memory
                using var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
                using var outputStream = new MemoryStream();

                // Copy template stream to output stream
                templateStream.CopyTo(outputStream);
                outputStream.Position = 0;

                // Mở file để chỉnh sửa
                using (var doc = WordprocessingDocument.Open(outputStream, true))
                {
                    // Thay thế text placeholders
                    ReplaceTextPlaceholders(doc, response);

                    // Điền dữ liệu vào bảng
                    FillTables(doc, response);

                    doc.Save();
                }

                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xử lý file Word: {ex.Message}. Đảm bảo file mẫu là định dạng .docx");
            }
        }

        private static void ReplaceTextPlaceholders(WordprocessingDocument doc, BaoCaoSuDungLaoDongResponse response)
        {
            var body = doc.MainDocumentPart!.Document.Body!;

            // Lấy dữ liệu đã format từ extension method
            var replacements = response.GetWordExportData();

            // Tìm và thay thế từng placeholder
            foreach (var replacement in replacements)
            {
                ReplaceTextInElement(body, replacement.Key, replacement.Value);
            }
        }

        private static void ReplaceTextInElement(OpenXmlElement element, string oldText, string newText)
        {
            foreach (var text in element.Descendants<Text>())
            {
                if (text.Text.Contains(oldText))
                {
                    text.Text = text.Text.Replace(oldText, newText);
                }
            }
        }

        private static void FillTables(WordprocessingDocument doc, BaoCaoSuDungLaoDongResponse response)
        {
            var tables = doc.MainDocumentPart!.Document.Body!.Elements<Table>().ToList();

            if (tables.Count >= 2)
            {
                // Bảng 1: Công chứng viên
                FillCongChungVienTable(tables[0], response.GetCongChungVienForWordExport());

                // Bảng 2: Nhân viên
                FillNhanVienTable(tables[1], response.GetNhanVienForWordExport());
            }
        }

        private static void FillCongChungVienTable(Table table, List<string[]> congChungVienData)
        {
            var rows = table.Elements<TableRow>().ToList();

            // Lấy dòng thứ 2 (index 1) làm template và xóa nó
            if (rows.Count > 1)
            {
                var templateRow = rows[1];
                templateRow.Remove();

                // Thêm dữ liệu mới
                foreach (var rowData in congChungVienData)
                {
                    var newRow = CloneRowWithData(templateRow, rowData);
                    table.AppendChild(newRow);
                }
            }
        }

        private static void FillNhanVienTable(Table table, List<string[]> nhanVienData)
        {
            var rows = table.Elements<TableRow>().ToList();

            // Lấy dòng thứ 2 (index 1) làm template và xóa nó
            if (rows.Count > 1)
            {
                var templateRow = rows[1];
                templateRow.Remove();

                // Thêm dữ liệu mới
                foreach (var rowData in nhanVienData)
                {
                    var newRow = CloneRowWithData(templateRow, rowData);
                    table.AppendChild(newRow);
                }
            }
        }

        private static TableRow CloneRowWithData(TableRow templateRow, string[] cellTexts)
        {
            // Clone toàn bộ row template (giữ nguyên 100% styling)
            var newRow = (TableRow)templateRow.CloneNode(true);

            // Lấy tất cả text elements trong row và cập nhật
            var textElements = newRow.Descendants<Text>().ToList();

            for (int i = 0; i < cellTexts.Length && i < textElements.Count; i++)
            {
                textElements[i].Text = cellTexts[i] ?? "";
            }

            return newRow;
        }
    }
}
