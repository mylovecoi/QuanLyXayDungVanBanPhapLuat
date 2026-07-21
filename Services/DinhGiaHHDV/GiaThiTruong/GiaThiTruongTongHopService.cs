using DataAccess;
using DataAccess.Entities.DinhGiaHHDV;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System.Reflection;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services.Systems;

namespace Services.DinhGiaHHDV.GiaThiTruong
{
    public class GiaThiTruongTongHopService(ApplicationDbContext dbContext, IAuthService authService) : IGiaThiTruongTongHopService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IAuthService _authService = authService;

        public async Task<CommonResponse> GetListByFilterAsync(int year, string thang, Guid donViId, string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.GiaThiTruongTongHops.AsQueryable();
            if (donViId != Guid.Empty)
            {
                query = query.Where(x => x.DonViQuanLyId == donViId);
            }

            if (year > 0)
            {
                var yearStr = year.ToString();
                query = query.Where(x => x.Nam == yearStr);
            }

            if (!string.IsNullOrEmpty(thang) && thang != "all")
            {
                query = query.Where(x => x.Thang == thang);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    (x.SoBc != null && x.SoBc.ToLower().Contains(search)) ||
                    (x.GhiChu != null && x.GhiChu.ToLower().Contains(search))
                );
            }

            query = query.OrderByDescending(x => x.NgayBc);

            var totalRecord = await query.CountAsync();
            var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

            return new CommonResponse
            {
                Status = "success",
                Data = dataView,
                TotalRecord = totalRecord
            };
        }

        public async Task<CommonResponse> CreateAsync(Guid thongTuId, Guid donViId, string thang, string nam, string[] selectedHoSo)
        {
            try
            {
                var maHoSo = Guid.NewGuid().ToString();

                // Get details from selected GiaThiTruong
                var dsHoSoChiTiet = await _dbContext.GiaThiTruongCts
                    .Where(x => selectedHoSo.Contains(x.MaHoSo))
                    .ToListAsync();

                // Get category commodities
                var dmHangHoa = await _dbContext.GiaThiTruongDanhMucCts
                    .Where(x => x.ThongTuId == thongTuId && x.TheoDoi == "TD")
                    .OrderBy(x => x.STTSapXep != null ? x.STTSapXep.Length : 0)
                    .ThenBy(x => x.STTSapXep)
                    .ToListAsync();

                var ctsToInsert = new List<GiaThiTruongTongHopCt>();
                foreach (var item in dmHangHoa)
                {
                    var ct = dsHoSoChiTiet.Where(x => x.MaHhDv == item.MaHhDv).ToList();
                    
                    var ctTruoc = ct.Where(x => x.GiaKyTruoc != 0).ToList();
                    double GiaKyTruoc = ctTruoc.Any() ? ctTruoc.Average(x => x.GiaKyTruoc) : 0;

                    var ctNay = ct.Where(x => x.GiaKyNay != 0).ToList();
                    double GiaKyNay = ctNay.Any() ? ctNay.Average(x => x.GiaKyNay) : 0;

                    ctsToInsert.Add(new GiaThiTruongTongHopCt
                    {
                        Id = Guid.NewGuid(),
                        MaHoSo = maHoSo,
                        ThongTuId = thongTuId,
                        MaHhDv = item.MaHhDv,
                        TenHhDv = item.TenHhDv,
                        DacDiemKt = item.DacDiemKt,
                        DonViTinh = item.DonViTinh,
                        GiaKyTruoc = GiaKyTruoc,
                        GiaKyNay = GiaKyNay,
                        TrangThai = "CXD",
                        STTSapXep = item.STTSapXep
                    });
                }

                if (ctsToInsert.Count > 0)
                {
                    _dbContext.GiaThiTruongTongHopCts.AddRange(ctsToInsert);
                    await _dbContext.SaveChangesAsync();
                }

                var model = new GiaThiTruongTongHop
                {
                    Id = Guid.NewGuid(),
                    MaHoSo = maHoSo,
                    MaHoSoTongHop = string.Join(",", selectedHoSo),
                    DonViQuanLyId = donViId,
                    ThongTuId = thongTuId,
                    Thang = thang,
                    Nam = nam,
                    NgayBc = DateTime.Now,
                    NgayChotBc = DateTime.Now,
                    TrangThai = "CXD",
                    GhiChu = $"Tổng hợp số liệu tháng {thang} năm {nam}"
                };

                return new CommonResponse("success", "Khởi tạo thành công", model);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi khởi tạo hồ sơ tổng hợp: " + ex.Message);
            }
        }

        public async Task<CommonResponse> StoreAsync(GiaThiTruongTongHop request, List<GiaThiTruongTongHopCt> details)
        {
            try
            {
                var existing = await _dbContext.GiaThiTruongTongHops.AnyAsync(x => x.MaHoSo == request.MaHoSo);
                if (existing)
                {
                    return new CommonResponse("error", "Mã hồ sơ đã tồn tại!");
                }

                request.Id = Guid.NewGuid();
                request.TrangThai = "CHT"; // Chưa hoàn thành/Chờ duyệt/Chờ chuyển tùy nghiệp vụ, để mặc định "CHT" hoặc "CC"

                _dbContext.GiaThiTruongTongHops.Add(request);

                // Update detail records
                var dbDetails = await _dbContext.GiaThiTruongTongHopCts
                    .Where(x => x.MaHoSo == request.MaHoSo)
                    .ToListAsync();

                foreach (var dbDt in dbDetails)
                {
                    dbDt.TrangThai = "XD";
                    if (details != null && details.Count > 0)
                    {
                        var reqDt = details.FirstOrDefault(x => x.MaHhDv == dbDt.MaHhDv);
                        if (reqDt != null)
                        {
                            dbDt.GiaKyTruoc = reqDt.GiaKyTruoc;
                            dbDt.GiaKyNay = reqDt.GiaKyNay;
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Lưu hồ sơ thành công", request);
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
                var record = await _dbContext.GiaThiTruongTongHops.FindAsync(id);
                if (record == null)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ!");
                }

                return new CommonResponse("success", "Lấy dữ liệu thành công", record);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lấy dữ liệu: " + ex.Message);
            }
        }

        public async Task<CommonResponse> UpdateAsync(GiaThiTruongTongHop request, List<GiaThiTruongTongHopCt> details)
        {
            try
            {
                var record = await _dbContext.GiaThiTruongTongHops.FirstOrDefaultAsync(x => x.MaHoSo == request.MaHoSo);
                if (record == null)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ!");
                }

                record.SoBc = request.SoBc;
                record.NgayBc = request.NgayBc;
                record.NgayChotBc = request.NgayChotBc;
                record.GhiChu = request.GhiChu;

                // Update detail records
                if (details != null && details.Count > 0)
                {
                    var dbDetails = await _dbContext.GiaThiTruongTongHopCts
                        .Where(x => x.MaHoSo == request.MaHoSo)
                        .ToListAsync();

                    foreach (var dbDt in dbDetails)
                    {
                        var reqDt = details.FirstOrDefault(x => x.MaHhDv == dbDt.MaHhDv);
                        if (reqDt != null)
                        {
                            dbDt.GiaKyTruoc = reqDt.GiaKyTruoc;
                            dbDt.GiaKyNay = reqDt.GiaKyNay;
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Cập nhật hồ sơ thành công", record);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi cập nhật: " + ex.Message);
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var record = await _dbContext.GiaThiTruongTongHops.FindAsync(id);
                if (record == null)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ!");
                }

                var details = await _dbContext.GiaThiTruongTongHopCts
                    .Where(x => x.MaHoSo == record.MaHoSo)
                    .ToListAsync();

                _dbContext.GiaThiTruongTongHopCts.RemoveRange(details);
                _dbContext.GiaThiTruongTongHops.Remove(record);

                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Xóa hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi khi xóa hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> GetCodeExcelAsync(string MaHoSo)
        {
            try
            {
                var model = await _dbContext.GiaThiTruongTongHops.FirstOrDefaultAsync(t => t.MaHoSo == MaHoSo);
                if (model != null && !string.IsNullOrEmpty(model.ChiTietExcel))
                    return new("success", "Thành công", model.ChiTietExcel);

                var listCt = await _dbContext.GiaThiTruongTongHopCts.Where(x => x.MaHoSo == MaHoSo)
                    .OrderBy(x => x.STTSapXep != null ? x.STTSapXep.Length : 0)
                    .ThenBy(x => x.STTSapXep)
                    .ToListAsync();

                var excludedNames = new HashSet<string> { "Id", "MaHoSo", "ThongTuId", "TrangThai", "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" };
                var columns = typeof(GiaThiTruongTongHopCt).GetProperties()
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

                var columnDataSb = new System.Text.StringBuilder();
                columnDataSb.Append("{");
                columnDataSb.Append("\"0\":{\"w\":60}");
                for (int colIdx = 0; colIdx < columns.Count; colIdx++)
                {
                    columnDataSb.Append($",\"{colIdx + 1}\":{{\"w\":150}}");
                }
                columnDataSb.Append("}");

                string defaultWorkbook = $@"{{
                                              ""id"": ""{MaHoSo}"",
                                              ""name"": ""Bảng giá chi tiết"",
                                              ""sheetOrder"": [""sheet1""],
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
                var model = await _dbContext.GiaThiTruongTongHops.FirstOrDefaultAsync(t => t.MaHoSo == MaHoSo);
                if (model == null) return new("error", "Hồ sơ không tồn tại");

                model.ChiTietExcel = jsonString;

                _dbContext.GiaThiTruongTongHops.Update(model);
                await _dbContext.SaveChangesAsync();

                await SyncSpreadsheetToCtAsync(MaHoSo, jsonString, model.ThongTuId);

                return new("success", "Lưu bảng tính thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi lưu bảng tính: " + ex.Message);
            }
        }

        private async Task SyncSpreadsheetToCtAsync(string maHoSo, string jsonString, Guid thongTuId)
        {
            try
            {
                var workbook = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
                var sheetId = workbook["sheetOrder"]?[0]?.ToString();
                if (string.IsNullOrEmpty(sheetId)) return;

                var cellData = workbook["sheets"]?[sheetId]?["cellData"] as Newtonsoft.Json.Linq.JObject;
                if (cellData == null) return;

                var listCt = new List<GiaThiTruongTongHopCt>();
                var excludedNames = new HashSet<string> { "Id", "MaHoSo", "ThongTuId", "TrangThai", "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" };
                var columns = typeof(GiaThiTruongTongHopCt).GetProperties()
                    .Where(p => !excludedNames.Contains(p.Name) && p.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>() != null)
                    .ToList();

                foreach (var rowProperty in cellData.Properties())
                {
                    if (rowProperty.Name == "0") continue; // Bỏ qua dòng tiêu đề

                    var rowObj = rowProperty.Value as Newtonsoft.Json.Linq.JObject;
                    if (rowObj == null) continue;

                    // Kiểm tra xem dòng này có giá trị hay không
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

                    var ctItem = new GiaThiTruongTongHopCt
                    {
                        Id = Guid.NewGuid(),
                        MaHoSo = maHoSo,
                        ThongTuId = thongTuId,
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

                    // Auto-calculate MucTangGiam and TyLeTangGiam
                    double diff = ctItem.GiaKyNay - ctItem.GiaKyTruoc;
                    ctItem.MucTangGiam = diff.ToString();
                    if (ctItem.GiaKyTruoc != 0)
                    {
                        ctItem.TyLeTangGiam = (diff / ctItem.GiaKyTruoc * 100).ToString("0.##") + "%";
                    }
                    else
                    {
                        ctItem.TyLeTangGiam = "0%";
                    }

                    listCt.Add(ctItem);
                }

                // Xóa chi tiết cũ và thêm chi tiết mới
                var oldDetails = await _dbContext.GiaThiTruongTongHopCts.Where(x => x.MaHoSo == maHoSo).ToListAsync();
                if (oldDetails.Count > 0)
                {
                    _dbContext.GiaThiTruongTongHopCts.RemoveRange(oldDetails);
                }

                if (listCt.Count > 0)
                {
                    _dbContext.GiaThiTruongTongHopCts.AddRange(listCt);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error syncing GiaThiTruongTongHop spreadsheet: " + ex.Message);
            }
        }
    }
}
