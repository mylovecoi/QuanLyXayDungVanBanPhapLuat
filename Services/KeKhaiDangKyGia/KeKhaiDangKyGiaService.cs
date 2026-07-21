using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using Services.Model;
using Services.DTOs.KeKhaiDangKyGia;

using System.Reflection;
using Services.Systems;
using Services.Manages;

namespace Services.KeKhaiDangKyGia
{
    public class KeKhaiDangKyGiaService : IKeKhaiDangKyGiaService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAttachedFileService _attachedFileService;
        private readonly IAuthService _authService;

        public KeKhaiDangKyGiaService(ApplicationDbContext dbContext, IAttachedFileService attachedFileService, IAuthService authService)
        {
            _dbContext = dbContext;
            _attachedFileService = attachedFileService;
            _authService = authService;
        }

        public async Task<CommonResponse> GetListByFilterAsync(KeKhaiDangKyGiaFilter filter)
        {
            try
            {
                IQueryable<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia> query = _dbContext.KeKhaiDangKyGias
                    .Include(x => x.DoanhNghiepQuanLy)
                    .Where(x => x.TrangThai != "CXD")
                    .AsNoTracking();

                if (filter.DoanhNghiepQuanLyId != Guid.Empty)
                {
                    query = query.Where(x => x.DoanhNghiepQuanLyId == filter.DoanhNghiepQuanLyId);
                }

                if (filter.TargetYear > 0)
                {
                    query = query.Where(x => x.ThoiDiem.Year == filter.TargetYear);
                }

                if (!string.IsNullOrEmpty(filter.Search))
                {
                    var searchLower = filter.Search.ToLower().Trim();
                    query = query.Where(x =>
                        (x.SoQd != null && x.SoQd.ToLower().Contains(searchLower)) ||
                        (x.GhiChu != null && x.GhiChu.ToLower().Contains(searchLower)) ||
                        (x.DonViTinh != null && x.DonViTinh.ToLower().Contains(searchLower))
                    );
                }

                int totalRecord = await query.CountAsync();
                filter.AdjustPageIfInvalid(totalRecord);

                var dataList = await query
                    .OrderByDescending(x => x.ThoiDiem)
                    .Skip((filter.PageCurrent - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                return new CommonResponse(status: "success", message: "Lấy danh sách thành công", data: dataList, totalRecord: totalRecord);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lấy danh sách hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> CreateAsync(Guid doanhNghiepQuanLyId, string maNghe)
        {
            try
            {
                // Xóa hồ sơ CXD cũ để dọn dẹp
                var oldCxd = _dbContext.KeKhaiDangKyGias.Where(t => t.TrangThai == "CXD" && t.DoanhNghiepQuanLyId == doanhNghiepQuanLyId && t.MaNghe == maNghe);
                if (oldCxd.Any())
                {
                    var oldHsCodes = oldCxd.Where(x => x.MaHoSo != null).Select(x => x.MaHoSo!).ToList();
                    var oldCts = await _dbContext.KeKhaiDangKyGiaCts.Where(x => oldHsCodes.Contains(x.MaHoSo!)).ToListAsync();
                    _dbContext.KeKhaiDangKyGiaCts.RemoveRange(oldCts);
                    foreach (var oldItem in oldCxd)
                    {
                        await _attachedFileService.RemoveRangeByGroupId(oldItem.Id);
                    }
                    _dbContext.KeKhaiDangKyGias.RemoveRange(oldCxd);
                    await _dbContext.SaveChangesAsync();
                }

                var danhMucKinhDoanh = await _dbContext.DanhMucKinhDoanhs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.MaNghe == maNghe);
                string? donViDongChuyenId = danhMucKinhDoanh?.DonViDongChuyenId;

                var model = new DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia
                {
                    Id = Guid.NewGuid(),
                    DoanhNghiepQuanLyId = doanhNghiepQuanLyId,
                    MaNghe = maNghe,
                    NgayQd = DateTime.Now,
                    NgayThucHien = DateTime.Now,
                    ThoiDiem = DateTime.Now,
                    MaHoSo = Guid.NewGuid().ToString(),
                    TrangThai = "CXD",
                    DonViDongChuyenId = donViDongChuyenId
                };

                var modelLk = await _dbContext.KeKhaiDangKyGias
                    .Where(t => t.MaNghe == maNghe && t.DoanhNghiepQuanLyId == doanhNghiepQuanLyId && (t.TrangThai == "DD" || t.TrangThai == "CB"))
                    .OrderByDescending(t => t.NgayThucHien)
                    .FirstOrDefaultAsync();

                if (modelLk != null)
                {
                    model.SoQdLk = modelLk.SoQd;
                    model.NgayQdLk = modelLk.NgayQd;
                }

                _dbContext.KeKhaiDangKyGias.Add(model);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", "Khởi tạo thành công", model);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi khởi tạo: " + ex.Message);
            }
        }

        public async Task<CommonResponse> StoreAsync(DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SoQd))
                {
                    return new CommonResponse("error", "Số quyết định không được để trống");
                }

                var model = await _dbContext.KeKhaiDangKyGias.FirstOrDefaultAsync(t => t.MaHoSo == request.MaHoSo);
                if (model == null)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ");
                }

                KeKhaiDangKyGiaMapper.CopyTo(request, model);
                model.TrangThai = "CC"; // Hoàn thành chuyển trạng thái sang CC

                _dbContext.KeKhaiDangKyGias.Update(model);
                await _dbContext.SaveChangesAsync();

                await SaveSpreadsheetToCt(model.MaHoSo, model.DoanhNghiepQuanLyId, model.ChiTietExcel ?? "");
                await _dbContext.SaveChangesAsync();

                await _attachedFileService.UpdateRangeStatus(model.Id, "KeKhaiDangKyGia");

                return new CommonResponse("success", "Thêm mới hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lưu dữ liệu: " + ex.Message);
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.KeKhaiDangKyGias.FindAsync(id);
                if (model == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin hồ sơ");
                }
                model.AttachedFiles = await _attachedFileService.GetAllAttachedFilesAsync(model.Id, "KeKhaiDangKyGia");
                return new CommonResponse("success", "Lấy dữ liệu thành công", model);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lấy dữ liệu: " + ex.Message);
            }
        }

        public async Task<CommonResponse> UpdateAsync(DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SoQd))
                {
                    return new CommonResponse("error", "Số quyết định không được để trống");
                }

                var model = await _dbContext.KeKhaiDangKyGias.FindAsync(request.Id);
                if (model == null)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ cần cập nhật");
                }

                KeKhaiDangKyGiaMapper.CopyTo(request, model);

                _dbContext.KeKhaiDangKyGias.Update(model);
                await _dbContext.SaveChangesAsync();

                await SaveSpreadsheetToCt(model.MaHoSo, model.DoanhNghiepQuanLyId, model.ChiTietExcel ?? "");
                await _dbContext.SaveChangesAsync();

                await _attachedFileService.UpdateRangeStatus(model.Id, "KeKhaiDangKyGia");

                return new CommonResponse("success", "Cập nhật hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi cập nhật dữ liệu: " + ex.Message);
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.KeKhaiDangKyGias.FindAsync(id);
                if (model == null)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ cần xóa");
                }

                var listCt = await _dbContext.KeKhaiDangKyGiaCts.Where(x => x.MaHoSo == model.MaHoSo).ToListAsync();
                if (listCt.Any())
                {
                    _dbContext.KeKhaiDangKyGiaCts.RemoveRange(listCt);
                }

                await _attachedFileService.RemoveRangeByGroupId(model.Id);

                _dbContext.KeKhaiDangKyGias.Remove(model);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", "Xóa hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi xóa dữ liệu: " + ex.Message);
            }
        }

        public async Task<CommonResponse> GetCodeExcelAsync(string MaHoSo)
        {
            try
            {
                var model = await _dbContext.KeKhaiDangKyGias.FirstOrDefaultAsync(t => t.MaHoSo == MaHoSo);
                if (model != null && !string.IsNullOrEmpty(model.ChiTietExcel))
                    return new CommonResponse("success", "Thành công", model.ChiTietExcel);

                var listCt = await _dbContext.KeKhaiDangKyGiaCts
                    .Where(x => x.MaHoSo == MaHoSo)
                    .ToListAsync();

                var detailType = typeof(DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGiaCt);
                var excludedNames = new HashSet<string> { "Id", "DoanhNghiepQuanLyId", "DoanhNghiepQuanLy", "MaHoSo", "TrangThai" };
                var columns = detailType.GetProperties()
                    .Where(p => !excludedNames.Contains(p.Name) && p.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>() != null)
                    .ToList();

                var cellData = new System.Text.StringBuilder();
                cellData.Append("{");

                cellData.Append("\"0\":{");
                cellData.Append("\"0\":{\"v\":\"STT\",\"s\":\"style_header\"}");
                for (int colIdx = 0; colIdx < columns.Count; colIdx++)
                {
                    var prop = columns[colIdx];
                    var displayNameAttr = prop.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>();
                    string headerText = displayNameAttr?.DisplayName ?? prop.Name;
                    cellData.Append($",\"{colIdx + 1}\":{{\"v\":\"{headerText}\",\"s\":\"style_header\"}}");
                }
                cellData.Append("}");

                for (int i = 0; i < listCt.Count; i++)
                {
                    var item = listCt[i];
                    string styleKey = "style_normal";

                    cellData.Append($",\"{i + 1}\":{{");
                    string sttVal = (i + 1).ToString();
                    cellData.Append($"\"0\":{{\"v\":\"{sttVal}\",\"s\":\"{styleKey}\"}}");

                    for (int colIdx = 0; colIdx < columns.Count; colIdx++)
                    {
                        var prop = columns[colIdx];
                        var val = prop.GetValue(item);
                        string cellVal = "";
                        if (val != null)
                        {
                            if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(decimal))
                            {
                                cellVal = val.ToString()!;
                            }
                            else
                            {
                                cellVal = val.ToString()!.Replace("\"", "\\\"");
                            }
                        }
                        cellData.Append($",\"{colIdx + 1}\":{{\"v\":\"{cellVal}\",\"s\":\"{styleKey}\"}}");
                    }
                    cellData.Append("}");
                }
                cellData.Append("}");

                int totalRows = listCt.Count + 1;
                int totalColumns = columns.Count + 1;

                var columnDataSb = new System.Text.StringBuilder();
                columnDataSb.Append("{");
                columnDataSb.Append("\"0\":{\"w\":60}");
                for (int colIdx = 0; colIdx < columns.Count; colIdx++)
                {
                    columnDataSb.Append($",\"{colIdx + 1}\":{{\"w\":180}}");
                }
                columnDataSb.Append("}");

                string defaultWorkbook = $@"{{
                                               ""id"": ""workbook1"",
                                               ""styles"": {{
                                                 ""style_header"": {{
                                                   ""bl"": 1,
                                                   ""ht"": 2,
                                                   ""vt"": 2,
                                                   ""tb"": 1
                                                 }},
                                                 ""style_bold"": {{
                                                   ""bl"": 1,
                                                   ""vt"": 2,
                                                   ""tb"": 1
                                                 }},
                                                 ""style_normal"": {{
                                                   ""vt"": 2,
                                                   ""tb"": 1
                                                 }}
                                               }},
                                               ""sheets"": {{
                                                 ""sheet1"": {{
                                                   ""id"": ""sheet1"",
                                                   ""name"": ""Sheet1"",
                                                   ""rowCount"": {Math.Max(totalRows + 20, 50)},
                                                   ""columnCount"": {totalColumns},
                                                   ""cellData"": {cellData},
                                                   ""rowData"": {{
                                                     ""0"": {{""h"": 80}}
                                                   }},
                                                   ""columnData"": {columnDataSb}
                                                 }}
                                               }},
                                               ""locale"": ""vi-VN""
                                             }}";

                return new CommonResponse(status: "success", message: "Thành công", data: defaultWorkbook);
            }
            catch (Exception ex)
            {
                return new CommonResponse(status: "error", message: "Lỗi tải bảng tính: " + ex.Message);
            }
        }

        public async Task<CommonResponse> SaveCodeExcelAsync(string MaHoSo, string jsonString)
        {
            try
            {
                var model = await _dbContext.KeKhaiDangKyGias.FirstOrDefaultAsync(t => t.MaHoSo == MaHoSo);
                if (model == null) return new CommonResponse(status: "error", message: "Hồ sơ không tồn tại");

                model.ChiTietExcel = jsonString;

                _dbContext.KeKhaiDangKyGias.Update(model);
                await _dbContext.SaveChangesAsync();

                await SaveSpreadsheetToCt(model.MaHoSo, model.DoanhNghiepQuanLyId, model.ChiTietExcel ?? "");
                await _dbContext.SaveChangesAsync();

                return new CommonResponse(status: "success", message: "Lưu bảng tính thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse(status: "error", message: "Lỗi lưu bảng tính: " + ex.Message);
            }
        }

        private async Task SaveSpreadsheetToCt(string mahs, Guid doanhNghiepQuanLyId, string codeExcel)
        {
            if (string.IsNullOrEmpty(codeExcel))
            {
                return;
            }

            try
            {
                var data = Newtonsoft.Json.Linq.JObject.Parse(codeExcel);
                var sheets = data["sheets"];
                if (sheets == null) return;

                var firstSheet = sheets.Children<Newtonsoft.Json.Linq.JProperty>().FirstOrDefault()?.Value;
                if (firstSheet == null) return;

                var cellData = firstSheet["cellData"];
                if (cellData == null) return;

                var rowKeys = cellData.Children<Newtonsoft.Json.Linq.JProperty>()
                    .Select(p =>
                    {
                        int r;
                        return int.TryParse(p.Name, out r) ? (int?)r : null;
                    })
                    .Where(r => r.HasValue)
                    .Select(r => r!.Value)
                    .OrderBy(k => k);

                // Xóa chi tiết cũ
                var oldDetails = await _dbContext.KeKhaiDangKyGiaCts.Where(x => x.MaHoSo == mahs).ToListAsync();
                if (oldDetails.Any())
                {
                    _dbContext.KeKhaiDangKyGiaCts.RemoveRange(oldDetails);
                }

                var listDetails = new List<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGiaCt>();
                var detailType = typeof(DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGiaCt);
                var colToProp = new Dictionary<int, System.Reflection.PropertyInfo>();

                var headerRow = cellData["0"];
                if (headerRow != null)
                {
                    foreach (var col in headerRow.Children<Newtonsoft.Json.Linq.JProperty>())
                    {
                        string headerText = col.Value["v"]?.ToString()?.Trim() ?? "";
                        int colIdx = int.Parse(col.Name);
                        string normalizedHeader = headerText.ToLower().Replace(" ", "").Replace("_", "");

                        System.Reflection.PropertyInfo? matchedProp = detailType.GetProperties().FirstOrDefault(p =>
                        {
                            var displayNameAttr = p.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>();
                            if (displayNameAttr != null && displayNameAttr.DisplayName != null)
                            {
                                string normalizedDisplayName = displayNameAttr.DisplayName.ToLower().Replace(" ", "").Replace("_", "");
                                if (normalizedDisplayName == normalizedHeader) return true;
                            }
                            return p.Name.ToLower().Replace(" ", "").Replace("_", "") == normalizedHeader;
                        });

                        if (matchedProp != null)
                        {
                            colToProp[colIdx] = matchedProp;
                        }
                    }
                }

                foreach (var rowIndex in rowKeys)
                {
                    if (rowIndex == 0) continue;

                    var rowData = cellData[rowIndex.ToString()];
                    if (rowData == null) continue;

                    var detailObj = new DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGiaCt();
                    detailObj.Id = Guid.NewGuid();
                    detailObj.DoanhNghiepQuanLyId = doanhNghiepQuanLyId;
                    detailObj.MaHoSo = mahs;
                    detailObj.TrangThai = "XD";

                    bool hasAnyData = false;

                    foreach (var entry in colToProp)
                    {
                        var cellVal = rowData[entry.Key.ToString()]?["v"]?.ToString();
                        if (cellVal != null)
                        {
                            hasAnyData = true;
                            var prop = entry.Value;
                            if (prop.PropertyType == typeof(double))
                            {
                                double.TryParse(cellVal, out double dVal);
                                prop.SetValue(detailObj, dVal);
                            }
                            else if (prop.PropertyType == typeof(int))
                            {
                                int.TryParse(cellVal, out int iVal);
                                prop.SetValue(detailObj, iVal);
                            }
                            else
                            {
                                prop.SetValue(detailObj, cellVal);
                            }
                        }
                    }

                    if (hasAnyData)
                    {
                        listDetails.Add(detailObj);
                    }
                }

                if (listDetails.Any())
                {
                    await _dbContext.KeKhaiDangKyGiaCts.AddRangeAsync(listDetails);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error syncing spreadsheet: " + ex.Message);
            }
        }

        public async Task<CommonResponse> GetSingleByIdAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.KeKhaiDangKyGias
                    .Include(x => x.DoanhNghiepQuanLy)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (model == null)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ");
                }
                return new CommonResponse("success", "Lấy dữ liệu thành công", model);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lấy dữ liệu: " + ex.Message);
            }
        }

        public async Task<CommonResponse> GetDetailsByMaHoSoAsync(string maHoSo)
        {
            try
            {
                var list = await _dbContext.KeKhaiDangKyGiaCts
                    .Where(x => x.MaHoSo == maHoSo)
                    .ToListAsync();
                return new CommonResponse("success", "Lấy dữ liệu thành công", list);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lấy dữ liệu chi tiết: " + ex.Message);
            }
        }

        public async Task<CommonResponse> ChuyenAsync(Guid hoSoId, Guid donViQuanLyId, string? thongTinNguoiChuyen, string? soDtNguoiChuyen)
        {
            try
            {
                var model = await _dbContext.KeKhaiDangKyGias.FirstOrDefaultAsync(t => t.Id == hoSoId);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                if (donViQuanLyId == Guid.Empty) // tài khoản SSA chuyển hồ sơ khi chưa chọn doanh nghiệp
                {
                    var lvkd = await _dbContext.DoanhNghiepLvKds
                        .FirstOrDefaultAsync(x => x.DoanhNghiepQuanLyId == model.DoanhNghiepQuanLyId && x.MaNghe == model.MaNghe);
                    donViQuanLyId = lvkd?.DonViQuanLyId ?? Guid.Empty;

                    if (donViQuanLyId == Guid.Empty)
                    {
                        var dn = await _dbContext.DoanhNghieps.FirstOrDefaultAsync(x => x.Id == model.DoanhNghiepQuanLyId);
                        donViQuanLyId = dn?.DonViQuanLyId ?? Guid.Empty;
                    }
                }

                model.TrangThai = "CD";
                model.NgayChuyen = DateTime.Now;
                model.DonViQuanLyId = donViQuanLyId;
                model.ThongTinNguoiChuyen = thongTinNguoiChuyen;
                model.SoDtNguoiChuyen = soDtNguoiChuyen;

                _dbContext.KeKhaiDangKyGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", "Chuyển hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi chuyển hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> GetKeKhaiDangKyGiaStatsAsync()
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                Guid? donViId = userInfo?.DanhMucDonViId;
                bool isSSA = userInfo?.SSA ?? false;

                var queryKK = _dbContext.KeKhaiDangKyGias.AsNoTracking().Where(x => x.TrangThai != "CXD");
                if (donViId != null && donViId != Guid.Empty && !isSSA)
                {
                    var userDonVi = await _dbContext.DanhMucDonVis.AsNoTracking().FirstOrDefaultAsync(x => x.Id == donViId);
                    if (userDonVi != null && userDonVi.Level > 0)
                    {
                        queryKK = queryKK.Where(x => x.DonViQuanLyId == donViId.Value);
                    }
                }

                var allKK = await queryKK.ToListAsync();
                int currentYear = DateTime.Now.Year;
                if (allKK.Any())
                {
                    currentYear = allKK.Max(x => x.ThoiDiem.Year);
                }

                var listKK = allKK.Where(x => x.ThoiDiem.Year == currentYear).ToList();
                var kkMonthlyCounts = new List<int>();
                var kkMonthlyApprovedCounts = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    kkMonthlyCounts.Add(listKK.Count(x => x.ThoiDiem.Month == i));
                    kkMonthlyApprovedCounts.Add(listKK.Count(x => x.ThoiDiem.Month == i && (x.TrangThai == "DD" || x.TrangThai == "CB")));
                }

                var kkStatusCounts = new Dictionary<string, int>
                {
                    { "CC", listKK.Count(x => x.TrangThai == "CC" || string.IsNullOrEmpty(x.TrangThai)) },
                    { "CD", listKK.Count(x => x.TrangThai == "CD") },
                    { "DD", listKK.Count(x => x.TrangThai == "DD") },
                    { "CB", listKK.Count(x => x.TrangThai == "CB") },
                    { "BTL", listKK.Count(x => x.TrangThai == "BTL") }
                };

                var result = new
                {
                    Year = currentYear,
                    MonthlyCounts = kkMonthlyCounts,
                    MonthlyApprovedCounts = kkMonthlyApprovedCounts,
                    StatusCounts = kkStatusCounts
                };

                return new CommonResponse("success", "Thành công", result);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi: " + ex.Message);
            }
        }
    }
}
