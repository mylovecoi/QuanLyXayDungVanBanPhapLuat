using DataAccess;
using DataAccess.Entities.DinhGiaHHDV;
using DataAccess.Entities.DinhGiaHHDV.ChiTiet;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using Services.DTOs.DinhGiaHHDV.ThongTinHoSo;
using Services.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Services.Manages;

namespace Services.DinhGiaHHDV.DinhGiaKhac
{
    public class DinhGiaService(
            ApplicationDbContext dbContext,
            IAuthService authService,
            IAttachedFileService attachedFileService
        ) : IDinhGiaService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IAuthService _authService = authService;
        private readonly IAttachedFileService _attachedFileService = attachedFileService;

        public async Task<CommonResponse> GetListByFilterAsync(DinhGiaFilter filter, string MaNghe)
        {
            try
            {
                IQueryable<DinhGia> queryable = _dbContext.DinhGias
                    .Include(x => x.DonViQuanLy)
                    .AsNoTracking()
                    .Where(x => x.DonViQuanLyId == filter.DonViId && x.MaNghe == MaNghe);

                if (filter.TrangThai != "CXD") queryable = queryable.Where(x => x.TrangThai != "CXD");

                if (filter.TargetYear > 0) queryable = queryable.Where(x => x.ThoiDiem.Year == filter.TargetYear);

                if (!string.IsNullOrEmpty(filter.Search))
                    queryable = queryable.Where(x =>
                        (EF.Functions.Like(x.MaHoSo, $"%{filter.Search}%") ||
                        EF.Functions.Like(x.ThoiDiem.Year.ToString(), $"%{filter.Search}%")));

                int totalRecord = await queryable.CountAsync();
                filter.AdjustPageIfInvalid(totalRecord);

                queryable = queryable.OrderByDescending(x => x.ThoiDiem).ThenBy(x => x.MaHoSo);

                var dataView = queryable.Skip((filter.PageCurrent - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                return new("success", "Lấy thông tin danh mục thành công", dataView, totalRecord);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetSingleByIdAsync(Guid hoSoId)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.Id == hoSoId);
                if (model == null) return new("error", "Hồ sơ không tồn tại");
                return new("success", "Thành công", model);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi xảy ra khi lấy thông tin hồ sơ");
            }
        }

        private Type? GetDetailType(string maNghe)
        {
            var type = Type.GetType($"DataAccess.Entities.DinhGiaHHDV.ChiTiet.ChiTiet{maNghe}, DataAccess")
                       ?? Type.GetType($"DataAccess.Entities.DinhGiaHHDV.ChiTiet{maNghe}, DataAccess");
            var catType = Type.GetType($"DataAccess.Entities.Settings.DanhMucGia.DanhMuc{maNghe}, DataAccess");
            if (type == null || catType == null)
            {
                return typeof(DataAccess.Entities.DinhGiaHHDV.ChiTiet.ChiTietGiaChung);
            }
            return type;
        }

        private Type? GetCategoryType(string maNghe)
        {
            var type = Type.GetType($"DataAccess.Entities.Settings.DanhMucGia.DanhMuc{maNghe}, DataAccess");
            var detType = Type.GetType($"DataAccess.Entities.DinhGiaHHDV.ChiTiet.ChiTiet{maNghe}, DataAccess")
                       ?? Type.GetType($"DataAccess.Entities.DinhGiaHHDV.ChiTiet{maNghe}, DataAccess");
            if (type == null || detType == null)
            {
                return typeof(DataAccess.Entities.Settings.DanhMucGia.DanhMucGiaChung);
            }
            return type;
        }

        public (string danhMucTable, string chiTietTable) GetTableNames(string maNghe)
        {
            var catType = GetCategoryType(maNghe);
            var detType = GetDetailType(maNghe);
            return (catType?.Name ?? "DanhMucGiaChung", detType?.Name ?? "ChiTietGiaChung");
        }

        private IQueryable GetDbSet(Type entityType)
        {
            var method = typeof(DbContext).GetMethods()
                .First(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0)
                .MakeGenericMethod(entityType);
            return (IQueryable)method.Invoke(_dbContext, null)!;
        }

        private async Task DeleteDetailsByMaHoSoAndMaNgheAsync(List<string> maHoSos, string maNghe, bool onlyCxd = false)
        {
            var detailType = GetDetailType(maNghe);
            if (detailType != null)
            {
                var dbSet = GetDbSet(detailType);
                var parameter = Expression.Parameter(detailType, "x");
                var property = Expression.Property(parameter, "MaHoSo");
                var containsMethod = typeof(List<string>).GetMethod("Contains", new[] { typeof(string) });
                var maHoSosConstant = Expression.Constant(maHoSos);
                var call = Expression.Call(maHoSosConstant, containsMethod!, property);

                Expression lambdaExpression = call;
                var trangThaiProp = detailType.GetProperty("TrangThai");
                if (onlyCxd && trangThaiProp != null)
                {
                    var propertyTrangThai = Expression.Property(parameter, "TrangThai");
                    var equalTrangThai = Expression.Equal(propertyTrangThai, Expression.Constant("CXD"));
                    lambdaExpression = Expression.AndAlso(call, equalTrangThai);
                }

                var lambda = Expression.Lambda(lambdaExpression, parameter);

                var whereMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(detailType);

                var query = whereMethod.Invoke(null, new object[] { dbSet, lambda }) as IQueryable;
                if (query != null)
                {
                    var list = new List<object>();
                    foreach (var item in query)
                    {
                        list.Add(item);
                    }
                    _dbContext.RemoveRange(list);
                }
            }
        }

        public async Task<CommonResponse> CreateAsync(Guid donViId, string MaNghe, Guid? danhMucId)
        {
            try
            {
                // Xóa hồ sơ CXD cũ để dọn dẹp
                var oldCxd = _dbContext.DinhGias.Where(t => t.TrangThai == "CXD" && t.DonViQuanLyId == donViId && t.MaNghe == MaNghe);
                if (oldCxd.Any())
                {
                    var oldHsCodes = oldCxd.Where(x => x.MaHoSo != null).Select(x => x.MaHoSo!).ToList();
                    await DeleteDetailsByMaHoSoAndMaNgheAsync(oldHsCodes, MaNghe, onlyCxd: true);
                    foreach (var oldItem in oldCxd)
                    {
                        await _attachedFileService.RemoveRangeByGroupId(oldItem.Id);
                    }
                    _dbContext.DinhGias.RemoveRange(oldCxd);
                    await _dbContext.SaveChangesAsync();
                }

                var donVi = await _dbContext.DanhMucDonVis.FindAsync(donViId);
                var prefix = donVi != null ? donVi.MaQHNS ?? "DG" : "DG";
                var maHoSo = prefix + "_" + DateTime.Now.ToString("yyMMddssmmHH");

                var model = new DinhGia
                {
                    Id = Guid.NewGuid(),
                    MaNghe = MaNghe,
                    MaHoSo = maHoSo,
                    DonViQuanLyId = donViId,
                    ThoiDiem = DateTime.Now,
                    TrangThai = "CXD",
                    CongBo = "CHUACONGBO"
                };

                _dbContext.DinhGias.Add(model);

                // Khởi tạo các dòng chi tiết mẫu từ danh mục tương ứng
                var categoryType = GetCategoryType(MaNghe);
                var detailType = GetDetailType(MaNghe);
                if (categoryType == null || detailType == null)
                {
                    return new("error", $"Không tìm thấy cấu trúc bảng DanhMuc/ChiTiet cho mã nghề {MaNghe}");
                }

                var listDetails = new List<object>();
                var detailProps = detailType.GetProperties().ToDictionary(p => p.Name.ToLower(), p => p);

                if (danhMucId != null && danhMucId != Guid.Empty)
                {
                    // Lấy bảng chi tiết danh mục tương ứng: DanhMuc+MaNghe+Ct hoặc DanhMucGiaChungCt
                    var catCtType = Type.GetType(categoryType.FullName + "Ct, DataAccess");
                    if (catCtType == null)
                    {
                        catCtType = typeof(DataAccess.Entities.Settings.DanhMucGia.DanhMucGiaChungCt);
                    }

                    var catCtDbSet = GetDbSet(catCtType);
                    var fkPropName = categoryType.Name + "Id";
                    if (catCtType.GetProperty(fkPropName) == null)
                    {
                        fkPropName = "DanhMucGiaChungId";
                    }

                    var parameter = Expression.Parameter(catCtType, "x");
                    var property = Expression.Property(parameter, fkPropName);
                    var targetId = Expression.Constant(danhMucId.Value);
                    var equals = Expression.Equal(property, targetId);
                    var lambda = Expression.Lambda(equals, parameter);

                    var whereMethod = typeof(Queryable).GetMethods()
                        .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(catCtType);

                    var query = whereMethod.Invoke(null, new object[] { catCtDbSet, lambda }) as IQueryable;

                    // Sắp xếp
                    if (catCtType.GetProperty("STTSapXep") != null)
                    {
                        var sttPropType = catCtType.GetProperty("STTSapXep")!.PropertyType;
                        var sttParam = Expression.Parameter(catCtType, "x");
                        var sttProp = Expression.Property(sttParam, "STTSapXep");
                        var sttLambda = Expression.Lambda(sttProp, sttParam);

                        var orderByMethod = typeof(Queryable).GetMethods()
                            .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                            .MakeGenericMethod(catCtType, sttPropType);

                        query = orderByMethod.Invoke(null, new object[] { query!, sttLambda }) as IQueryable;
                    }

                    var catDetails = new List<object>();
                    if (query != null)
                    {
                        foreach (var cd in query)
                        {
                            catDetails.Add(cd);
                        }
                    }

                    foreach (var catCt in catDetails)
                    {
                        var detail = Activator.CreateInstance(detailType);
                        if (detail != null)
                        {
                            detailType.GetProperty("Id")?.SetValue(detail, Guid.NewGuid());
                            detailType.GetProperty("DonViQuanLyId")?.SetValue(detail, donViId);
                            detailType.GetProperty("MaHoSo")?.SetValue(detail, model.MaHoSo);
                            detailType.GetProperty("TrangThai")?.SetValue(detail, "CXD");

                            var maNgheProp = detailType.GetProperty("MaNghe");
                            if (maNgheProp != null && maNgheProp.CanWrite)
                            {
                                maNgheProp.SetValue(detail, model.MaNghe);
                            }

                            foreach (var catCtProp in catCtType.GetProperties())
                            {
                                string propNameLower = catCtProp.Name.ToLower();
                                var baseProps = new HashSet<string> { "id", "createdby", "createddate", "updatedby", "updateddate", fkPropName.ToLower(), categoryType.Name.ToLower() };
                                if (baseProps.Contains(propNameLower))
                                {
                                    continue;
                                }

                                if (detailProps.TryGetValue(propNameLower, out var detailProp) && detailProp.CanWrite)
                                {
                                    var val = catCtProp.GetValue(catCt);
                                    detailProp.SetValue(detail, val);
                                }
                            }

                            listDetails.Add(detail);
                        }
                    }
                }
                else
                {
                    var catDbSet = GetDbSet(categoryType);
                    var parameter = Expression.Parameter(categoryType, "t");
                    var property = Expression.Property(parameter, "STTSapXep");
                    var lambda = Expression.Lambda(property, parameter);

                    var orderByMethod = typeof(Queryable).GetMethods()
                        .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(categoryType, typeof(int));

                    var orderedQuery = orderByMethod.Invoke(null, new object[] { catDbSet, lambda }) as IQueryable;
                    var categories = new List<object>();
                    if (orderedQuery != null)
                    {
                        foreach (var cat in orderedQuery)
                        {
                            categories.Add(cat);
                        }
                    }

                    foreach (var cat in categories)
                    {
                        var detail = Activator.CreateInstance(detailType);
                        if (detail != null)
                        {
                            detailType.GetProperty("Id")?.SetValue(detail, Guid.NewGuid());
                            detailType.GetProperty("DonViQuanLyId")?.SetValue(detail, donViId);
                            detailType.GetProperty("MaHoSo")?.SetValue(detail, model.MaHoSo);
                            detailType.GetProperty("TrangThai")?.SetValue(detail, "CXD");

                            var maNgheProp = detailType.GetProperty("MaNghe");
                            if (maNgheProp != null && maNgheProp.CanWrite)
                            {
                                maNgheProp.SetValue(detail, model.MaNghe);
                            }

                            foreach (var catProp in categoryType.GetProperties())
                            {
                                string propNameLower = catProp.Name.ToLower();
                                var baseProps = new HashSet<string> { "id", "createdby", "createddate", "updatedby", "updateddate" };
                                if (baseProps.Contains(propNameLower))
                                {
                                    continue;
                                }

                                if (detailProps.TryGetValue(propNameLower, out var detailProp) && detailProp.CanWrite)
                                {
                                    var val = catProp.GetValue(cat);
                                    detailProp.SetValue(detail, val);
                                }
                            }

                            listDetails.Add(detail);
                        }
                    }
                }

                if (listDetails.Any())
                {
                    _dbContext.AddRange(listDetails);
                }

                await _dbContext.SaveChangesAsync();

                return new("success", "Khởi tạo thành công", model);
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi khởi tạo hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> StoreAsync(DinhGia request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.SoQd) || string.IsNullOrWhiteSpace(request.MoTa))
                {
                    return new("error", "Số quyết định và Mô tả không được để trống!");
                }

                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.MaHoSo == request.MaHoSo);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");

                DinhGiaMapper.CopyTo(request, model, isNew: true);
                model.TrangThai = "CC";

                if (!string.IsNullOrEmpty(model.MaHoSo))
                {
                    await SaveSpreadsheetToCt(model.MaHoSo, model.DonViQuanLyId, model.ChiTietExcel ?? "", model.MaNghe ?? "");
                }

                _dbContext.DinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                await _attachedFileService.UpdateRangeStatus(model.Id, "DinhGia");

                return new("success", "Lưu hồ sơ thành công", model);
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi lưu hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> EditAsync(Guid hoSoId)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.Id == hoSoId);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");
                model.AttachedFiles = await _attachedFileService.GetAllAttachedFilesAsync(model.Id, "DinhGia");
                return new("success", "Thành công", model);
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi tải hồ sơ chỉnh sửa: " + ex.Message);
            }
        }

        public async Task<CommonResponse> UpdateAsync(DinhGia request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.SoQd) || string.IsNullOrWhiteSpace(request.MoTa))
                {
                    return new("error", "Số quyết định và Mô tả không được để trống!");
                }

                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.MaHoSo == request.MaHoSo);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");

                DinhGiaMapper.CopyTo(request, model, isNew: false);

                if (!string.IsNullOrEmpty(model.MaHoSo))
                {
                    await SaveSpreadsheetToCt(model.MaHoSo, model.DonViQuanLyId, model.ChiTietExcel ?? "", model.MaNghe ?? "");
                }

                _dbContext.DinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                await _attachedFileService.UpdateRangeStatus(model.Id, "DinhGia");

                return new("success", "Cập nhật hồ sơ thành công", model);
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi cập nhật hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid hoSoId)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.Id == hoSoId);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");

                await DeleteDetailsByMaHoSoAndMaNgheAsync(new List<string> { model.MaHoSo ?? "" }, model.MaNghe ?? "", onlyCxd: false);
                await _attachedFileService.RemoveRangeByGroupId(model.Id);
                _dbContext.DinhGias.Remove(model);
                await _dbContext.SaveChangesAsync();

                return new("success", "Xóa hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi xóa hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> ChuyenAsync(Guid hoSoId, string trangThai)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.Id == hoSoId);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");

                model.TrangThai = trangThai; // sau khi chuyển thì hồ sơ sẽ có trạng thái được chọn
                model.UpdatedDate = DateTime.Now;
                _dbContext.DinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new("success", "Chuyển hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi chuyển hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> GetCodeExcelAsync(string Mahs)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.MaHoSo == Mahs);
                if (model != null && !string.IsNullOrEmpty(model.ChiTietExcel))
                    return new("success", "Thành công", model.ChiTietExcel);

                var detailType = GetDetailType(model?.MaNghe ?? "");
                if (detailType == null)
                {
                    return new("error", $"Không hỗ trợ mã nghề: {model?.MaNghe}");
                }

                var dbSet = GetDbSet(detailType);
                var parameter = Expression.Parameter(detailType, "t");
                var property = Expression.Property(parameter, "MaHoSo");
                var lambda = Expression.Lambda(Expression.Equal(property, Expression.Constant(Mahs)), parameter);

                var whereMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(detailType);

                var query = whereMethod.Invoke(null, new object[] { dbSet, lambda }) as IQueryable;
                var listCt = new List<object>();
                if (query != null)
                {
                    foreach (var d in query)
                    {
                        listCt.Add(d);
                    }
                }

                var sttSapXepProp = detailType.GetProperty("STTSapXep");
                if (sttSapXepProp != null)
                {
                    listCt = listCt.OrderBy(x => (int)(sttSapXepProp.GetValue(x) ?? 0)).ToList();
                }

                var excludedNames = new HashSet<string> { "Id", "DonViQuanLyId", "DonViQuanLy", "MaHoSo", "MaNghe", "MaDoiTuong", "Style" };
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

                var styleProp = detailType.GetProperty("Style");
                var sttHienThiProp = detailType.GetProperty("STTHienThi");

                for (int i = 0; i < listCt.Count; i++)
                {
                    var item = listCt[i];
                    string styleVal = styleProp?.GetValue(item) as string ?? "";
                    bool isBold = !string.IsNullOrEmpty(styleVal) && styleVal.Contains("Chữ in đậm");
                    string styleKey = isBold ? "style_bold" : "style_normal";

                    cellData.Append($",\"{i + 1}\":{{");

                    string sttVal = "";
                    if (sttHienThiProp != null)
                    {
                        sttVal = sttHienThiProp.GetValue(item) as string ?? "";
                    }
                    if (string.IsNullOrEmpty(sttVal))
                    {
                        sttVal = (i + 1).ToString();
                    }
                    sttVal = sttVal.Replace("\"", "\\\"");
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
                    columnDataSb.Append($",\"{colIdx + 1}\":{{\"w\":150}}");
                }
                columnDataSb.Append("}");

                string defaultWorkbook = $@"{{
                                              ""id"": ""{Mahs}"",
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

        public async Task<CommonResponse> SaveCodeExcelAsync(string Mahs, string jsonString)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.MaHoSo == Mahs);
                if (model == null) return new("error", "Hồ sơ không tồn tại");

                model.ChiTietExcel = jsonString;
                model.UpdatedDate = DateTime.Now;

                _dbContext.DinhGias.Update(model);
                await SaveSpreadsheetToCt(model.MaHoSo, model.DonViQuanLyId, model.ChiTietExcel ?? "", model.MaNghe ?? "");
                await _dbContext.SaveChangesAsync();

                return new("success", "Lưu bảng tính thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi lưu bảng tính: " + ex.Message);
            }
        }

        public async Task<CommonResponse> GetDetailsByMaHoSoAsync(string maHoSo)
        {
            try
            {
                var model = await _dbContext.DinhGias.AsNoTracking().FirstOrDefaultAsync(t => t.MaHoSo == maHoSo);
                if (model == null) return new("error", "Hồ sơ không tồn tại");

                object details;
                var detailType = GetDetailType(model.MaNghe ?? "");
                if (detailType != null)
                {
                    var dbSet = GetDbSet(detailType);
                    var parameter = Expression.Parameter(detailType, "t");
                    var property = Expression.Property(parameter, "MaHoSo");
                    var lambda = Expression.Lambda(Expression.Equal(property, Expression.Constant(maHoSo)), parameter);

                    var whereMethod = typeof(Queryable).GetMethods()
                        .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(detailType);

                    var query = whereMethod.Invoke(null, new object[] { dbSet, lambda }) as IQueryable;
                    var listDetails = new List<object>();
                    if (query != null)
                    {
                        foreach (var d in query)
                        {
                            listDetails.Add(d);
                        }
                    }
                    var sttSapXepProp = detailType.GetProperty("STTSapXep");
                    if (sttSapXepProp != null)
                    {
                        listDetails = listDetails.OrderBy(x => (int)(sttSapXepProp.GetValue(x) ?? 0)).ToList();
                    }
                    details = listDetails;
                }
                else
                {
                    return new("error", $"Không hỗ trợ mã nghề: {model.MaNghe}");
                }
                return new("success", "Thành công", details);
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi tải chi tiết: " + ex.Message);
            }
        }

        private async Task SaveSpreadsheetToCt(string mahs, Guid donViId, string codeExcel, string maNghe)
        {
            if (string.IsNullOrEmpty(codeExcel))
            {
                var detailType = GetDetailType(maNghe);
                if (detailType != null)
                {
                    var dbSet = GetDbSet(detailType);
                    var parameter = Expression.Parameter(detailType, "x");
                    var property = Expression.Property(parameter, "MaHoSo");
                    var equalMaHoSo = Expression.Equal(property, Expression.Constant(mahs));

                    Expression lambdaExpression = equalMaHoSo;
                    var trangThaiProp = detailType.GetProperty("TrangThai");
                    if (trangThaiProp != null)
                    {
                        var propertyTrangThai = Expression.Property(parameter, "TrangThai");
                        var equalTrangThai = Expression.Equal(propertyTrangThai, Expression.Constant("CXD"));
                        lambdaExpression = Expression.AndAlso(equalMaHoSo, equalTrangThai);
                    }

                    var lambda = Expression.Lambda(lambdaExpression, parameter);

                    var whereMethod = typeof(Queryable).GetMethods()
                        .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(detailType);

                    var query = whereMethod.Invoke(null, new object[] { dbSet, lambda }) as IQueryable;
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            trangThaiProp?.SetValue(item, "XD");
                            
                            var hsProp = detailType.GetProperty("MaHoSo");
                            if (hsProp != null && hsProp.CanWrite)
                            {
                                hsProp.SetValue(item, mahs);
                            }

                            var maNgheProp = detailType.GetProperty("MaNghe");
                            if (maNgheProp != null && maNgheProp.CanWrite)
                            {
                                maNgheProp.SetValue(item, maNghe);
                            }

                            _dbContext.Update(item);
                        }
                    }
                }
                return;
            }

            try
            {
                var logPath = @"d:\Vhost\CSDLGia_ASP_ThaiNguyen\temp\save_excel_log.txt";
                var logDir = Path.GetDirectoryName(logPath);
                if (logDir != null && !Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

                File.WriteAllText(logPath, $"[Start Sync] mahs: {mahs}, maNghe: {maNghe}\n");

                var data = Newtonsoft.Json.Linq.JObject.Parse(codeExcel);
                var sheets = data["sheets"];
                if (sheets == null)
                {
                    File.AppendAllText(logPath, "[Error] sheets is null\n");
                    return;
                }

                var firstSheet = sheets.Children<Newtonsoft.Json.Linq.JProperty>().FirstOrDefault()?.Value;
                if (firstSheet == null)
                {
                    File.AppendAllText(logPath, "[Error] firstSheet is null\n");
                    return;
                }

                var cellData = firstSheet["cellData"];
                if (cellData == null)
                {
                    File.AppendAllText(logPath, "[Error] cellData is null\n");
                    return;
                }

                var rowKeys = cellData.Children<Newtonsoft.Json.Linq.JProperty>()
                    .Select(p =>
                    {
                        int r;
                        return int.TryParse(p.Name, out r) ? (int?)r : null;
                    })
                    .Where(r => r.HasValue)
                    .Select(r => r!.Value)
                    .OrderBy(k => k)
                    .ToList();

                File.AppendAllText(logPath, $"[Info] rowKeys count: {rowKeys.Count}\n");

                var detailType = GetDetailType(maNghe);
                if (detailType != null)
                {
                    File.AppendAllText(logPath, $"[Info] detailType: {detailType.FullName}\n");
                    await DeleteDetailsByMaHoSoAndMaNgheAsync(new List<string> { mahs }, maNghe, onlyCxd: false);

                    var listDetails = new List<object>();
                    var properties = detailType.GetProperties().ToDictionary(p => p.Name.ToLower(), p => p);
                    var colToProp = new Dictionary<int, System.Reflection.PropertyInfo>();

                    var headerRow = cellData["0"];
                    if (headerRow != null)
                    {
                        File.AppendAllText(logPath, $"[Info] headerRow is not null\n");
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
                                string normalizedPropName = p.Name.ToLower().Replace(" ", "").Replace("_", "");
                                if (normalizedPropName == normalizedHeader) return true;
                                if (normalizedHeader == "stt" && p.Name == "STTSapXep") return true;
                                return false;
                            });

                            if (matchedProp != null)
                            {
                                colToProp[colIdx] = matchedProp;
                                File.AppendAllText(logPath, $"[Prop Match] Col {colIdx} ({headerText}) -> {matchedProp.Name}\n");
                            }
                            else
                            {
                                File.AppendAllText(logPath, $"[Prop No Match] Col {colIdx} ({headerText})\n");
                            }
                        }
                    }
                    else
                    {
                        File.AppendAllText(logPath, "[Error] headerRow is null\n");
                    }

                    foreach (var rowIndex in rowKeys)
                    {
                        if (rowIndex == 0) continue;

                        var rowData = cellData[rowIndex.ToString()];
                        if (rowData == null) continue;

                        var detailObj = Activator.CreateInstance(detailType);
                        if (detailObj != null)
                        {
                            detailType.GetProperty("Id")?.SetValue(detailObj, Guid.NewGuid());
                            detailType.GetProperty("DonViQuanLyId")?.SetValue(detailObj, donViId);
                            detailType.GetProperty("MaHoSo")?.SetValue(detailObj, mahs);

                            var maNgheProp = detailType.GetProperty("MaNghe");
                            if (maNgheProp != null && maNgheProp.CanWrite)
                            {
                                maNgheProp.SetValue(detailObj, maNghe);
                            }

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
                                detailType.GetProperty("TrangThai")?.SetValue(detailObj, "XD");
                                listDetails.Add(detailObj);
                            }
                        }
                    }

                    File.AppendAllText(logPath, $"[Info] parsed listDetails count: {listDetails.Count}\n");

                    if (listDetails.Any())
                    {
                        foreach (var d in listDetails)
                        {
                            _dbContext.Add(d);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var logPath = @"d:\Vhost\CSDLGia_ASP_ThaiNguyen\temp\save_excel_log.txt";
                File.AppendAllText(logPath, $"[Exception] {ex.Message}\n{ex.StackTrace}\n");
                Console.WriteLine("Error syncing spreadsheet: " + ex.Message);
            }
        }

        //public async Task<List<DynamicCategoryOption>> GetCategoryOptionsByMaNgheAsync(string maNghe)
        //{
        //    var list = new List<DynamicCategoryOption>();
        //    var categoryType = GetCategoryType(maNghe);
        //    if (categoryType != null)
        //    {
        //        list.Add(new DynamicCategoryOption
        //        {
        //            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), // Generic GUID representing the category
        //            Code = maNghe,
        //            Name = $"Danh mục {maNghe}"
        //        });
        //    }
        //    return list;
        //}

        public async Task<CommonResponse> GetSoLuongDinhGiaTheoThangAsync()
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                int currentYear = DateTime.Now.Year;

                // 1. DINH GIA THEO MA NGHE
                var dmKinhDoanhDG = await _dbContext.DanhMucKinhDoanhs
                    .AsNoTracking()
                    .Where(t => t.LoaiGia == "DG" && (t.Level > 0 || t.PhanLoai == "Detail"))
                    .ToListAsync();

                var queryDG = _dbContext.DinhGias.AsNoTracking().Where(x => x.TrangThai != "CXD");

                if (userInfo != null && userInfo.DanhMucDonViId != Guid.Empty && !userInfo.SSA)
                {
                    var userDonVi = await _dbContext.DanhMucDonVis.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userInfo.DanhMucDonViId);
                    if (userDonVi != null && userDonVi.Level > 0)
                    {
                        queryDG = queryDG.Where(x => x.DonViQuanLyId == userInfo.DanhMucDonViId);
                    }
                }

                var listDG = await queryDG.ToListAsync();

                var dgByNghe = listDG
                    .GroupBy(x => x.MaNghe ?? "")
                    .Select(g => {
                        var dm = dmKinhDoanhDG.FirstOrDefault(k => k.MaNghe == g.Key);
                        var tenNghe = dm?.TenNghe ?? (!string.IsNullOrEmpty(g.Key) ? g.Key : "Khác");
                        return new {
                            MaNghe = g.Key,
                            TenNghe = tenNghe,
                            TotalCount = g.Count(),
                            ApprovedCount = g.Count(x => x.TrangThai == "DD" || x.TrangThai == "CB")
                        };
                    })
                    .OrderByDescending(x => x.TotalCount)
                    .ToList();

                var result = new
                {
                    Year = currentYear,
                    DinhGiaByMaNghe = dgByNghe
                };

                return new CommonResponse("success", "Lấy dữ liệu thành công", result);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Có lỗi trong quá trình lấy dữ liệu: " + ex.Message);
            }
        }
    }
}
