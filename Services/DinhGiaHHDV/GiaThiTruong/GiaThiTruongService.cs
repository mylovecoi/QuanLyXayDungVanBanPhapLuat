using DataAccess;
using DataAccess.Entities.DinhGiaHHDV;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Services.Systems;

namespace Services.DinhGiaHHDV.GiaThiTruong
{
    public class GiaThiTruongService(ApplicationDbContext dbContext, IAuthService authService) : IGiaThiTruongService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IAuthService _authService = authService;

        public async Task<CommonResponse> GetListByFilterAsync(int year, string thang, Guid donViId, string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.GiaThiTruongs.AsQueryable();
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
                    (x.SoQd != null && x.SoQd.ToLower().Contains(search)) ||
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

        public async Task<CommonResponse> CreateAsync(Guid thongTuId, Guid donViId, string thang, string nam)
        {
            try
            {
                var maHoSo = Guid.NewGuid().ToString();
                
                // Copy entries from GiaThiTruongDanhMucCt to GiaThiTruongCt
                var dmCts = await _dbContext.GiaThiTruongDanhMucCts
                    .Where(x => x.ThongTuId == thongTuId && x.TheoDoi == "TD")
                    .ToListAsync();

                var ctsToInsert = new List<GiaThiTruongCt>();
                foreach (var dmCt in dmCts)
                {
                    ctsToInsert.Add(new GiaThiTruongCt
                    {
                        Id = Guid.NewGuid(),
                        MaHoSo = maHoSo,
                        ThongTuId = thongTuId,
                        MaHhDv = dmCt.MaHhDv,
                        TenHhDv = dmCt.TenHhDv,
                        DacDiemKt = dmCt.DacDiemKt,
                        DonViTinh = dmCt.DonViTinh,
                        LoaiGia = "Giá bán lẻ",
                        NguonThongTin = "",
                        GhiChu = "",
                        TrangThai = "CXD",
                        STTSapXep = dmCt.STTSapXep
                    });
                }

                if (ctsToInsert.Count > 0)
                {
                    _dbContext.GiaThiTruongCts.AddRange(ctsToInsert);
                    await _dbContext.SaveChangesAsync();
                }

                // Return model
                var model = new DataAccess.Entities.DinhGiaHHDV.GiaThiTruong
                {
                    Id = Guid.NewGuid(),
                    MaHoSo = maHoSo,
                    DonViQuanLyId = donViId,
                    ThongTuId = thongTuId,
                    Thang = thang,
                    Nam = nam,
                    Thoidiem = DateTime.Now,
                    ThoiDiemLk = DateTime.Now,
                    TrangThai = "CXD"
                };

                return new CommonResponse("success", "Khởi tạo thành công", model);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi khởi tạo hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> StoreAsync(DataAccess.Entities.DinhGiaHHDV.GiaThiTruong request)
        {
            try
            {
                var existing = await _dbContext.GiaThiTruongs.AnyAsync(x => x.MaHoSo == request.MaHoSo);
                if (existing)
                {
                    return new CommonResponse("error", "Mã hồ sơ đã tồn tại!");
                }

                request.Id = Guid.NewGuid();
                request.TrangThai = "CC"; // Chờ chuyển
                
                _dbContext.GiaThiTruongs.Add(request);
                await _dbContext.SaveChangesAsync();

                // Update status of related GiaThiTruongCt to "XD"
                var cts = await _dbContext.GiaThiTruongCts.Where(x => x.MaHoSo == request.MaHoSo).ToListAsync();
                foreach (var ct in cts)
                {
                    ct.TrangThai = "XD";
                }
                _dbContext.GiaThiTruongCts.UpdateRange(cts);
                await _dbContext.SaveChangesAsync();

                // If Excel details exist, sync them to GiaThiTruongCt
                if (!string.IsNullOrEmpty(request.ChiTietExcel))
                {
                    await SyncSpreadsheetToCtAsync(request.MaHoSo ?? "", request.ChiTietExcel, request.ThongTuId);
                }

                return new CommonResponse("success", "Thêm mới hồ sơ giá thị trường thành công!");
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
                var data = await _dbContext.GiaThiTruongs.FindAsync(id);
                if (data == null) return new CommonResponse("error", "Không tìm thấy hồ sơ!");
                return new CommonResponse("success", "Thành công", data);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", ex.Message);
            }
        }

        public async Task<CommonResponse> UpdateAsync(DataAccess.Entities.DinhGiaHHDV.GiaThiTruong request)
        {
            try
            {
                var data = await _dbContext.GiaThiTruongs.FirstOrDefaultAsync(x => x.MaHoSo == request.MaHoSo);
                if (data == null) return new CommonResponse("error", "Không tìm thấy hồ sơ cần cập nhật!");

                GiaThiTruongMapper.CopyTo(request, data);

                if (!string.IsNullOrEmpty(request.ChiTietExcel))
                {
                    await SyncSpreadsheetToCtAsync(data.MaHoSo ?? "", request.ChiTietExcel, data.ThongTuId);
                }

                _dbContext.GiaThiTruongs.Update(data);
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
                var data = await _dbContext.GiaThiTruongs.FindAsync(id);
                if (data == null) return new CommonResponse("error", "Không tìm thấy hồ sơ!");

                // Remove attached details
                var cts = await _dbContext.GiaThiTruongCts.Where(x => x.MaHoSo == data.MaHoSo).ToListAsync();
                if (cts.Count > 0)
                {
                    _dbContext.GiaThiTruongCts.RemoveRange(cts);
                }

                _dbContext.GiaThiTruongs.Remove(data);
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
                var model = await _dbContext.GiaThiTruongs.FirstOrDefaultAsync(t => t.MaHoSo == MaHoSo);
                if (model != null && !string.IsNullOrEmpty(model.ChiTietExcel))
                    return new("success", "Thành công", model.ChiTietExcel);

                var listCt = await _dbContext.GiaThiTruongCts
                    .Where(x => x.MaHoSo == MaHoSo)
                    .ToListAsync();

                listCt = listCt.OrderBy(x => x.STTSapXep != null ? x.STTSapXep.Length : 0)
                               .ThenBy(x => x.STTSapXep)
                               .ToList();

                var excludedNames = new HashSet<string> { "Id", "MaHoSo", "ThongTuId", "TrangThai", "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" };
                var columns = typeof(GiaThiTruongCt).GetProperties()
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
                var model = await _dbContext.GiaThiTruongs.FirstOrDefaultAsync(t => t.MaHoSo == MaHoSo);
                if (model == null) return new("error", "Hồ sơ không tồn tại");

                model.ChiTietExcel = jsonString;
                model.UpdatedDate = DateTime.Now;

                _dbContext.GiaThiTruongs.Update(model);
                await _dbContext.SaveChangesAsync();

                await SyncSpreadsheetToCtAsync(MaHoSo, jsonString, model.ThongTuId);

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
                var model = await _dbContext.GiaThiTruongs.FirstOrDefaultAsync(t => t.Id == hoSoId);
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

                _dbContext.GiaThiTruongs.Update(model);
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
                var details = await _dbContext.GiaThiTruongCts
                    .Where(x => x.MaHoSo == maHoSo)
                    .ToListAsync();
                return new CommonResponse("success", "Thành công", details);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", ex.Message);
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

                var listCt = new List<GiaThiTruongCt>();
                var excludedNames = new HashSet<string> { "Id", "MaHoSo", "ThongTuId", "TrangThai", "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" };
                var columns = typeof(GiaThiTruongCt).GetProperties()
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

                    var ctItem = new GiaThiTruongCt
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
                var oldDetails = await _dbContext.GiaThiTruongCts.Where(x => x.MaHoSo == maHoSo).ToListAsync();
                if (oldDetails.Count > 0)
                {
                    _dbContext.GiaThiTruongCts.RemoveRange(oldDetails);
                }

                if (listCt.Count > 0)
                {
                    _dbContext.GiaThiTruongCts.AddRange(listCt);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error syncing GiaThiTruong spreadsheet: " + ex.Message);
            }
        }

        public async Task<CommonResponse> GetGiaThiTruongStatsAsync()
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                Guid? donViId = userInfo?.DanhMucDonViId;
                bool isSSA = userInfo?.SSA ?? false;

                var queryGTT = _dbContext.GiaThiTruongs.AsNoTracking().Where(x => x.TrangThai != "CXD");
                if (donViId != null && donViId != Guid.Empty && !isSSA)
                {
                    var userDonVi = await _dbContext.DanhMucDonVis.AsNoTracking().FirstOrDefaultAsync(x => x.Id == donViId);
                    if (userDonVi != null && userDonVi.Level > 0)
                    {
                        queryGTT = queryGTT.Where(x => x.DonViQuanLyId == donViId.Value);
                    }
                }

                var allGTT = await queryGTT.ToListAsync();
                int currentYear = DateTime.Now.Year;
                if (allGTT.Any())
                {
                    currentYear = allGTT.Max(x => x.Thoidiem.Year);
                }

                var listGTT = allGTT.Where(x => x.Thoidiem.Year == currentYear).ToList();
                var gttMonthlyCounts = new List<int>();
                var gttMonthlyApprovedCounts = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    gttMonthlyCounts.Add(listGTT.Count(x => x.Thoidiem.Month == i));
                    gttMonthlyApprovedCounts.Add(listGTT.Count(x => x.Thoidiem.Month == i && (x.TrangThai == "DD" || x.TrangThai == "CB")));
                }

                var gttStatusCounts = new Dictionary<string, int>
                {
                    { "CC", listGTT.Count(x => x.TrangThai == "CC" || string.IsNullOrEmpty(x.TrangThai)) },
                    { "CD", listGTT.Count(x => x.TrangThai == "CD") },
                    { "DD", listGTT.Count(x => x.TrangThai == "DD") },
                    { "CB", listGTT.Count(x => x.TrangThai == "CB") },
                    { "BTL", listGTT.Count(x => x.TrangThai == "BTL") }
                };

                var result = new
                {
                    Year = currentYear,
                    MonthlyCounts = gttMonthlyCounts,
                    MonthlyApprovedCounts = gttMonthlyApprovedCounts,
                    StatusCounts = gttStatusCounts
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
