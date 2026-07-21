using DataAccess;
using DataAccess.Entities.ThamDinhGia;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.ThamDinhGia
{
    public class ThamDinhGiaService(ApplicationDbContext dbContext, Services.Systems.IAuthService authService) : IThamDinhGiaService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly Services.Systems.IAuthService _authService = authService;

        public async Task<CommonResponse> GetListByFilterAsync(int year, Guid donViId, string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.ThamDinhGias.AsQueryable();

            if (donViId != Guid.Empty)
            {
                query = query.Where(x => x.DonViQuanLyId == donViId);
            }

            if (year > 0)
            {
                query = query.Where(x => x.Thoidiem.Year == year);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    (x.SoTbKl != null && x.SoTbKl.ToLower().Contains(search)) ||
                    (x.GhiChu != null && x.GhiChu.ToLower().Contains(search))
                );
            }

            query = query.OrderByDescending(x => x.Thoidiem);

            var totalRecord = await query.CountAsync();
            var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

            return new CommonResponse
            {
                Status = "success",
                Data = dataView,
                TotalRecord = totalRecord
            };
        }

        public async Task<CommonResponse> CreateAsync(Guid hangHoaId, Guid donViId, string phanLoai)
        {
            try
            {
                var maHoSo = Guid.NewGuid();

                // Copy entries from ThamDinhGiaDanhMucHangHoaCt to ThamDinhGiaCt
                var dmCts = await _dbContext.ThamDinhGiaDanhMucHangHoaCts
                    .Where(x => x.HangHoaId == hangHoaId && x.TrangThai == "Kích hoạt")
                    .ToListAsync();

                var ctsToInsert = new List<ThamDinhGiaCt>();
                foreach (var dmCt in dmCts)
                {
                    ctsToInsert.Add(new ThamDinhGiaCt
                    {
                        Id = Guid.NewGuid(),
                        MaHoSo = maHoSo,
                        HangHoaId = hangHoaId,
                        MaHangHoa = dmCt.MaHangHoa,
                        TenHangHoa = dmCt.TenHangHoa,
                        QuyCachChatLuong = dmCt.QuyCachChatLuong,
                        ThongSoKt = dmCt.ThongSoKt,
                        XuatXu = dmCt.XuatXu,
                        DonViTinh = dmCt.DonViTinh,
                        SoLuong = 0,
                        DonGiaThamDinh = 0,
                        GiaTriTsThamDinh = 0,
                        GhiChu = "",
                        TrangThai = "CXD"
                    });
                }

                if (ctsToInsert.Count > 0)
                {
                    _dbContext.ThamDinhGiaCts.AddRange(ctsToInsert);
                    await _dbContext.SaveChangesAsync();
                }

                var thoidiem = DateTime.Now;
                var donVi = await _dbContext.DanhMucDonVis.FindAsync(donViId);
                var diaBanId = donVi?.DonViChuQuanId ?? Guid.Empty;

                // Return model
                var model = new DataAccess.Entities.ThamDinhGia.ThamDinhGia
                {
                    Id = maHoSo,
                    DiaBanId = diaBanId,
                    DonViQuanLyId = donViId,
                    DonViChuQuanId = donVi?.DonViChuQuanId ?? Guid.Empty,
                    PhanLoai = phanLoai,
                    Thoidiem = thoidiem,
                    ThoiHan = thoidiem.AddMonths(1),
                    NgayQdPheDuyet = thoidiem,
                    TrangThai = "CXD",
                    HangHoaId = hangHoaId
                };

                return new CommonResponse("success", "Khởi tạo thành công", model);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi khởi tạo hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> StoreAsync(DataAccess.Entities.ThamDinhGia.ThamDinhGia request)
        {
            try
            {
                var maHoSoGuid = request.Id;
                var existing = await _dbContext.ThamDinhGias.AnyAsync(x => x.Id == request.Id);
                if (existing)
                {
                    return new CommonResponse("error", "Mã hồ sơ đã tồn tại!");
                }

                request.TrangThai = "CC"; // Chờ chuyển
                
                _dbContext.ThamDinhGias.Add(request);
                await _dbContext.SaveChangesAsync();

                // Update status of related ThamDinhGiaCt to "XD"
                var cts = await _dbContext.ThamDinhGiaCts.Where(x => x.MaHoSo == maHoSoGuid).ToListAsync();
                foreach (var ct in cts)
                {
                    ct.TrangThai = "XD";
                }
                _dbContext.ThamDinhGiaCts.UpdateRange(cts);
                await _dbContext.SaveChangesAsync();

                // If Excel details exist, sync them to ThamDinhGiaCt
                if (!string.IsNullOrEmpty(request.ChiTietExcel))
                {
                    await SyncSpreadsheetToCtAsync(maHoSoGuid, request.ChiTietExcel);
                }

                return new CommonResponse("success", "Thêm mới hồ sơ thẩm định giá thành công!");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lưu hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.ThamDinhGias.FindAsync(id);
                if (data == null) return new CommonResponse("error", "Không tìm thấy hồ sơ!");
                return new CommonResponse("success", "Thành công", data);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", ex.Message);
            }
        }

        public async Task<CommonResponse> UpdateAsync(DataAccess.Entities.ThamDinhGia.ThamDinhGia request)
        {
            try
            {
                var data = await _dbContext.ThamDinhGias.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (data == null) return new CommonResponse("error", "Không tìm thấy hồ sơ cần cập nhật!");

                ThamDinhGiaMapper.CopyTo(request, data);

                if (!string.IsNullOrEmpty(request.ChiTietExcel))
                {
                    var maHoSoGuid = data.Id;
                    await SyncSpreadsheetToCtAsync(maHoSoGuid, request.ChiTietExcel);
                }

                _dbContext.ThamDinhGias.Update(data);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", "Cập nhật hồ sơ thành công!");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi cập nhật hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.ThamDinhGias.FindAsync(id);
                if (data == null) return new CommonResponse("error", "Không tìm thấy hồ sơ!");

                var maHoSoGuid = data.Id;
                // Remove attached details
                var cts = await _dbContext.ThamDinhGiaCts.Where(x => x.MaHoSo == maHoSoGuid).ToListAsync();
                if (cts.Count > 0)
                {
                    _dbContext.ThamDinhGiaCts.RemoveRange(cts);
                }

                _dbContext.ThamDinhGias.Remove(data);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", "Xóa hồ sơ thành công!");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi xóa hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> GetCodeExcelAsync(string MaHoSo)
        {
            try
            {
                var maHoSoGuid = Guid.Parse(MaHoSo);
                var model = await _dbContext.ThamDinhGias.FirstOrDefaultAsync(t => t.Id == maHoSoGuid);
                if (model != null && !string.IsNullOrEmpty(model.ChiTietExcel))
                    return new("success", "Thành công", model.ChiTietExcel);

                var listCt = await _dbContext.ThamDinhGiaCts
                    .Where(x => x.MaHoSo == maHoSoGuid)
                    .ToListAsync();

                var excludedNames = new HashSet<string> { "Id", "MaHoSo", "HangHoaId", "TrangThai", "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" };
                var columns = typeof(ThamDinhGiaCt).GetProperties()
                    .Where(p => !excludedNames.Contains(p.Name) && p.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>() != null)
                    .ToList();

                var cellData = new StringBuilder();
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
                    cellData.Append($"\"0\":{{\"v\":\"{i + 1}\",\"s\":\"{styleKey}\"}}");

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

                var columnDataSb = new StringBuilder();
                columnDataSb.Append("{");
                columnDataSb.Append("\"0\":{\"w\":60}");
                for (int colIdx = 0; colIdx < columns.Count; colIdx++)
                {
                    columnDataSb.Append($",\"{colIdx + 1}\":{{\"w\":150}}");
                }
                columnDataSb.Append("}");

                string defaultWorkbook = $@"{{" +
                                          $"\"id\": \"{MaHoSo}\"," +
                                          $"\"name\": \"Bảng giá chi tiết\"," +
                                          $"\"sheetOrder\": [\"sheet1\"]," +
                                          $"\"styles\": {{" +
                                            $"\"style_header\": {{" +
                                              $"\"bl\": 1," +
                                              $"\"ht\": 2," +
                                              $"\"vt\": 2," +
                                              $"\"tb\": 1" +
                                            $"}}," +
                                            $"\"style_normal\": {{" +
                                              $"\"vt\": 2," +
                                              $"\"tb\": 1" +
                                            $"}}" +
                                          $"}}," +
                                          $"\"sheets\": {{" +
                                            $"\"sheet1\": {{" +
                                              $"\"id\": \"sheet1\"," +
                                              $"\"name\": \"Sheet1\"," +
                                              $"\"rowCount\": {Math.Max(totalRows + 20, 50)}," +
                                              $"\"columnCount\": {totalColumns}," +
                                              $"\"cellData\": {cellData}," +
                                              $"\"rowData\": {{" +
                                                $"\"0\": {{\"h\": 80}}" +
                                              $"}}," +
                                              $"\"columnData\": {columnDataSb}" +
                                            $"}}" +
                                          $"}}," +
                                          $"\"locale\": \"vi-VN\"" +
                                        $"}}";

                return new("success", "Thành công", defaultWorkbook);
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi tải bảng tính: " + ex.Message);
            }
        }

        public async Task<CommonResponse> SaveCodeExcelAsync(string MaHoSo, string jsonString)
        {
            try
            {
                var maHoSoGuid = Guid.Parse(MaHoSo);
                var model = await _dbContext.ThamDinhGias.FirstOrDefaultAsync(t => t.Id == maHoSoGuid);
                if (model == null) return new("error", "Hồ sơ không tồn tại");

                model.ChiTietExcel = jsonString;
                model.UpdatedDate = DateTime.Now;

                _dbContext.ThamDinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                await SyncSpreadsheetToCtAsync(maHoSoGuid, jsonString);

                return new("success", "Lưu bảng tính thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi lưu bảng tính: " + ex.Message);
            }
        }

        public async Task<CommonResponse> ChuyenAsync(Guid hoSoId, string trangThai)
        {
            try
            {
                var model = await _dbContext.ThamDinhGias.FirstOrDefaultAsync(t => t.Id == hoSoId);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");

                // Retrieve managing unit information
                var unit = await _dbContext.DanhMucDonVis.FirstOrDefaultAsync(x => x.Id == model.DonViQuanLyId);
                if (unit == null)
                {
                    return new("error", "Không tìm thấy đơn vị quản lý của hồ sơ!");
                }

                // Verify parent unit (Cơ quan chủ quản)
                if (unit.DonViChuQuanId == Guid.Empty)
                {
                    model.DonViChuQuanId = unit.Id; // Tự chuyển cho chính mình nếu chưa thiết lập cơ quan chủ quản
                }
                else
                {
                    var parentUnitExists = await _dbContext.DanhMucDonVis.AnyAsync(x => x.Id == unit.DonViChuQuanId);
                    if (!parentUnitExists)
                    {
                        return new("error", "Cơ quan chủ quản được thiết lập không tồn tại trong hệ thống. Vui lòng liên hệ quản trị hệ thống để thiết lập lại!");
                    }
                    model.DonViChuQuanId = unit.DonViChuQuanId;
                }
                model.TrangThai = trangThai;
                model.UpdatedDate = DateTime.Now;

                _dbContext.ThamDinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new("success", "Chuyển hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi chuyển hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> GetDetailsByMaHoSoAsync(string maHoSo)
        {
            try
            {
                var maHoSoGuid = Guid.Parse(maHoSo);
                var details = await _dbContext.ThamDinhGiaCts
                    .Where(x => x.MaHoSo == maHoSoGuid)
                    .ToListAsync();
                return new CommonResponse("success", "Thành công", details);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", ex.Message);
            }
        }

        private async Task SyncSpreadsheetToCtAsync(Guid maHoSo, string jsonString)
        {
            try
            {
                var workbook = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
                var sheetId = workbook["sheetOrder"]?[0]?.ToString();
                if (string.IsNullOrEmpty(sheetId)) return;

                var cellData = workbook["sheets"]?[sheetId]?["cellData"] as Newtonsoft.Json.Linq.JObject;
                if (cellData == null) return;

                var listCt = new List<ThamDinhGiaCt>();
                var excludedNames = new HashSet<string> { "Id", "MaHoSo", "HangHoaId", "TrangThai", "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" };
                var columns = typeof(ThamDinhGiaCt).GetProperties()
                    .Where(p => !excludedNames.Contains(p.Name) && p.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>() != null)
                    .ToList();

                // Get original HangHoaId to preserve
                var existingCts = await _dbContext.ThamDinhGiaCts.Where(x => x.MaHoSo == maHoSo).ToListAsync();
                Guid originalHangHoaId = existingCts.FirstOrDefault()?.HangHoaId ?? Guid.Empty;

                foreach (var rowProperty in cellData.Properties())
                {
                    if (rowProperty.Name == "0") continue; // Bỏ qua dòng tiêu đề

                    var rowObj = rowProperty.Value as Newtonsoft.Json.Linq.JObject;
                    if (rowObj == null) continue;

                    bool hasData = false;
                    for (int c = 1; c <= columns.Count; c++)
                    {
                        var cellVal = rowObj[c.ToString()]?["v"]?.ToString();
                        if (!string.IsNullOrEmpty(cellVal))
                        {
                            hasData = true;
                            break;
                        }
                    }

                    if (!hasData) continue;

                    var ctItem = new ThamDinhGiaCt
                    {
                        Id = Guid.NewGuid(),
                        MaHoSo = maHoSo,
                        HangHoaId = originalHangHoaId,
                        TrangThai = "XD"
                    };

                    for (int colIdx = 0; colIdx < columns.Count; colIdx++)
                    {
                        var prop = columns[colIdx];
                        var cellVal = rowObj[(colIdx + 1).ToString()]?["v"]?.ToString();
                        if (cellVal != null)
                        {
                            if (prop.PropertyType == typeof(double))
                            {
                                double.TryParse(cellVal, out double doubleVal);
                                prop.SetValue(ctItem, doubleVal);
                            }
                            else if (prop.PropertyType == typeof(int))
                            {
                                int.TryParse(cellVal, out int intVal);
                                prop.SetValue(ctItem, intVal);
                            }
                            else if (prop.PropertyType == typeof(decimal))
                            {
                                decimal.TryParse(cellVal, out decimal decimalVal);
                                prop.SetValue(ctItem, decimalVal);
                            }
                            else
                            {
                                prop.SetValue(ctItem, cellVal);
                            }
                        }
                    }

                    // Auto calculate GiaTriTsThamDinh if not set
                    if (ctItem.GiaTriTsThamDinh == 0 && ctItem.SoLuong > 0 && ctItem.DonGiaThamDinh > 0)
                    {
                        ctItem.GiaTriTsThamDinh = ctItem.SoLuong * ctItem.DonGiaThamDinh;
                    }

                    listCt.Add(ctItem);
                }

                if (existingCts.Count > 0)
                {
                    _dbContext.ThamDinhGiaCts.RemoveRange(existingCts);
                }

                if (listCt.Count > 0)
                {
                    _dbContext.ThamDinhGiaCts.AddRange(listCt);
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Silently ignore or log sync errors
            }
        }

        public async Task<CommonResponse> GetThamDinhGiaStatsAsync()
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                Guid? donViId = userInfo?.DanhMucDonViId;
                bool isSSA = userInfo?.SSA ?? false;

                var query = _dbContext.ThamDinhGias.AsNoTracking().Where(x => x.TrangThai != "CXD");
                if (donViId != null && donViId != Guid.Empty && !isSSA)
                {
                    var userDonVi = await _dbContext.DanhMucDonVis.AsNoTracking().FirstOrDefaultAsync(x => x.Id == donViId);
                    if (userDonVi != null && userDonVi.Level > 0)
                    {
                        query = query.Where(x => x.DonViQuanLyId == donViId.Value);
                    }
                }

                var allDocs = await query.ToListAsync();
                int currentYear = DateTime.Now.Year;
                if (allDocs.Any())
                {
                    currentYear = allDocs.Max(x => x.Thoidiem.Year);
                }

                var listDocs = allDocs.Where(x => x.Thoidiem.Year == currentYear).ToList();
                var monthlyCounts = new List<int>();
                var monthlyApprovedCounts = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    monthlyCounts.Add(listDocs.Count(x => x.Thoidiem.Month == i));
                    monthlyApprovedCounts.Add(listDocs.Count(x => x.Thoidiem.Month == i && (x.TrangThai == "DD" || x.TrangThai == "CB")));
                }

                var statusCounts = new Dictionary<string, int>
                {
                    { "CC", listDocs.Count(x => x.TrangThai == "CC" || string.IsNullOrEmpty(x.TrangThai)) },
                    { "CD", listDocs.Count(x => x.TrangThai == "CD") },
                    { "DD", listDocs.Count(x => x.TrangThai == "DD") },
                    { "CB", listDocs.Count(x => x.TrangThai == "CB") },
                    { "BTL", listDocs.Count(x => x.TrangThai == "BTL") }
                };

                var result = new
                {
                    Year = currentYear,
                    MonthlyCounts = monthlyCounts,
                    MonthlyApprovedCounts = monthlyApprovedCounts,
                    StatusCounts = statusCounts
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
