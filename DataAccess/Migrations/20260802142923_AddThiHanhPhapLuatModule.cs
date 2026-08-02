using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddThiHanhPhapLuatModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThiHanhPhapLuatKeHoachs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaKeHoach = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenKeHoach = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    DanhMucVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SoKyHieuVanBanCanCu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayBanHanhVanBanCanCu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrichYeuVanBanCanCu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CoQuanBanHanhVanBanCanCu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DonViChuTriId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCongBo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThiHanhPhapLuatKeHoachs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatKeHoachs_DanhMucDonVis_DonViChuTriId",
                        column: x => x.DonViChuTriId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatKeHoachs_DanhMucVanBans_DanhMucVanBanId",
                        column: x => x.DanhMucVanBanId,
                        principalTable: "DanhMucVanBans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ThiHanhPhapLuatKeHoachDonVis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NgayNhanKeHoach = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DaXem = table.Column<bool>(type: "bit", nullable: false),
                    NgayXem = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThiHanhPhapLuatKeHoachDonVis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatKeHoachDonVis_DanhMucDonVis_DonViId",
                        column: x => x.DonViId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatKeHoachDonVis_ThiHanhPhapLuatKeHoachs_KeHoachId",
                        column: x => x.KeHoachId,
                        principalTable: "ThiHanhPhapLuatKeHoachs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThiHanhPhapLuatNhiemVus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaNhiemVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenNhiemVu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NoiDungNhiemVu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViChuTriId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiDieuPhoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HanHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MucDoUuTien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    YeuCauBaoCao = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThiHanhPhapLuatNhiemVus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatNhiemVus_DanhMucDonVis_DonViChuTriId",
                        column: x => x.DonViChuTriId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatNhiemVus_ThiHanhPhapLuatKeHoachs_KeHoachId",
                        column: x => x.KeHoachId,
                        principalTable: "ThiHanhPhapLuatKeHoachs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatNhiemVus_Users_NguoiDieuPhoiId",
                        column: x => x.NguoiDieuPhoiId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ThiHanhPhapLuatTongHops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiTongHopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayTongHop = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongSoChiTietNhiemVu = table.Column<int>(type: "int", nullable: false),
                    SoChiTietDaHoanThanh = table.Column<int>(type: "int", nullable: false),
                    SoChiTietChuaHoanThanh = table.Column<int>(type: "int", nullable: false),
                    SoChiTietChamTienDo = table.Column<int>(type: "int", nullable: false),
                    SoChiTietQuaHan = table.Column<int>(type: "int", nullable: false),
                    SoChiTietChuaNhapLieu = table.Column<int>(type: "int", nullable: false),
                    TyLeHoanThanh = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    NhanXetTongHop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KetLuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KienNghi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThiHanhPhapLuatTongHops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatTongHops_ThiHanhPhapLuatKeHoachs_KeHoachId",
                        column: x => x.KeHoachId,
                        principalTable: "ThiHanhPhapLuatKeHoachs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatTongHops_Users_NguoiTongHopId",
                        column: x => x.NguoiTongHopId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ThiHanhPhapLuatChiTietNhiemVus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhiemVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaChiTiet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenChiTiet = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NoiDungChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiChiTiet = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DonViThucHienId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiPhuTrachChinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HanHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TyLeHoanThanh = table.Column<int>(type: "int", nullable: false),
                    KetQuaYeuCau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTriChiTieu = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThiHanhPhapLuatChiTietNhiemVus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatChiTietNhiemVus_DanhMucDonVis_DonViThucHienId",
                        column: x => x.DonViThucHienId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatChiTietNhiemVus_ThiHanhPhapLuatNhiemVus_NhiemVuId",
                        column: x => x.NhiemVuId,
                        principalTable: "ThiHanhPhapLuatNhiemVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatChiTietNhiemVus_Users_NguoiPhuTrachChinhId",
                        column: x => x.NguoiPhuTrachChinhId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ThiHanhPhapLuatChiTietPhoiHops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChiTietNhiemVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiDungId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThiHanhPhapLuatChiTietPhoiHops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatChiTietPhoiHops_ThiHanhPhapLuatChiTietNhiemVus_ChiTietNhiemVuId",
                        column: x => x.ChiTietNhiemVuId,
                        principalTable: "ThiHanhPhapLuatChiTietNhiemVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatChiTietPhoiHops_Users_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ThiHanhPhapLuatDanhGias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhiemVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChiTietNhiemVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonViDuocDanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiDanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KetQuaDanhGia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MucDoCanhBao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NoiDungDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KienNghiXuLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YeuCauBoSung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThiHanhPhapLuatDanhGias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatDanhGias_DanhMucDonVis_DonViDuocDanhGiaId",
                        column: x => x.DonViDuocDanhGiaId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatDanhGias_ThiHanhPhapLuatChiTietNhiemVus_ChiTietNhiemVuId",
                        column: x => x.ChiTietNhiemVuId,
                        principalTable: "ThiHanhPhapLuatChiTietNhiemVus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatDanhGias_ThiHanhPhapLuatKeHoachs_KeHoachId",
                        column: x => x.KeHoachId,
                        principalTable: "ThiHanhPhapLuatKeHoachs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatDanhGias_ThiHanhPhapLuatNhiemVus_NhiemVuId",
                        column: x => x.NhiemVuId,
                        principalTable: "ThiHanhPhapLuatNhiemVus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatDanhGias_Users_NguoiDanhGiaId",
                        column: x => x.NguoiDanhGiaId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ThiHanhPhapLuatTienDos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChiTietNhiemVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViCapNhatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiCapNhatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TyLeHoanThanh = table.Column<int>(type: "int", nullable: false),
                    KetQuaThucHien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungBaoCao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhoKhanVuongMac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeXuatKienNghi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiBaoCao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThiHanhPhapLuatTienDos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatTienDos_DanhMucDonVis_DonViCapNhatId",
                        column: x => x.DonViCapNhatId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatTienDos_ThiHanhPhapLuatChiTietNhiemVus_ChiTietNhiemVuId",
                        column: x => x.ChiTietNhiemVuId,
                        principalTable: "ThiHanhPhapLuatChiTietNhiemVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThiHanhPhapLuatTienDos_Users_NguoiCapNhatId",
                        column: x => x.NguoiCapNhatId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatChiTietNhiemVus_DonViThucHienId",
                table: "ThiHanhPhapLuatChiTietNhiemVus",
                column: "DonViThucHienId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatChiTietNhiemVus_NguoiPhuTrachChinhId",
                table: "ThiHanhPhapLuatChiTietNhiemVus",
                column: "NguoiPhuTrachChinhId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatChiTietNhiemVus_NhiemVuId_MaChiTiet",
                table: "ThiHanhPhapLuatChiTietNhiemVus",
                columns: new[] { "NhiemVuId", "MaChiTiet" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatChiTietPhoiHops_ChiTietNhiemVuId_NguoiDungId",
                table: "ThiHanhPhapLuatChiTietPhoiHops",
                columns: new[] { "ChiTietNhiemVuId", "NguoiDungId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatChiTietPhoiHops_NguoiDungId",
                table: "ThiHanhPhapLuatChiTietPhoiHops",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatDanhGias_ChiTietNhiemVuId",
                table: "ThiHanhPhapLuatDanhGias",
                column: "ChiTietNhiemVuId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatDanhGias_DonViDuocDanhGiaId",
                table: "ThiHanhPhapLuatDanhGias",
                column: "DonViDuocDanhGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatDanhGias_KeHoachId",
                table: "ThiHanhPhapLuatDanhGias",
                column: "KeHoachId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatDanhGias_NguoiDanhGiaId",
                table: "ThiHanhPhapLuatDanhGias",
                column: "NguoiDanhGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatDanhGias_NhiemVuId",
                table: "ThiHanhPhapLuatDanhGias",
                column: "NhiemVuId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatKeHoachDonVis_DonViId",
                table: "ThiHanhPhapLuatKeHoachDonVis",
                column: "DonViId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatKeHoachDonVis_KeHoachId_DonViId_VaiTro",
                table: "ThiHanhPhapLuatKeHoachDonVis",
                columns: new[] { "KeHoachId", "DonViId", "VaiTro" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatKeHoachs_DanhMucVanBanId",
                table: "ThiHanhPhapLuatKeHoachs",
                column: "DanhMucVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatKeHoachs_DonViChuTriId",
                table: "ThiHanhPhapLuatKeHoachs",
                column: "DonViChuTriId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatKeHoachs_MaKeHoach",
                table: "ThiHanhPhapLuatKeHoachs",
                column: "MaKeHoach",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatNhiemVus_DonViChuTriId",
                table: "ThiHanhPhapLuatNhiemVus",
                column: "DonViChuTriId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatNhiemVus_KeHoachId_MaNhiemVu",
                table: "ThiHanhPhapLuatNhiemVus",
                columns: new[] { "KeHoachId", "MaNhiemVu" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatNhiemVus_NguoiDieuPhoiId",
                table: "ThiHanhPhapLuatNhiemVus",
                column: "NguoiDieuPhoiId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatTienDos_ChiTietNhiemVuId",
                table: "ThiHanhPhapLuatTienDos",
                column: "ChiTietNhiemVuId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatTienDos_DonViCapNhatId",
                table: "ThiHanhPhapLuatTienDos",
                column: "DonViCapNhatId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatTienDos_NguoiCapNhatId",
                table: "ThiHanhPhapLuatTienDos",
                column: "NguoiCapNhatId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatTongHops_KeHoachId",
                table: "ThiHanhPhapLuatTongHops",
                column: "KeHoachId");

            migrationBuilder.CreateIndex(
                name: "IX_ThiHanhPhapLuatTongHops_NguoiTongHopId",
                table: "ThiHanhPhapLuatTongHops",
                column: "NguoiTongHopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThiHanhPhapLuatChiTietPhoiHops");

            migrationBuilder.DropTable(
                name: "ThiHanhPhapLuatDanhGias");

            migrationBuilder.DropTable(
                name: "ThiHanhPhapLuatKeHoachDonVis");

            migrationBuilder.DropTable(
                name: "ThiHanhPhapLuatTienDos");

            migrationBuilder.DropTable(
                name: "ThiHanhPhapLuatTongHops");

            migrationBuilder.DropTable(
                name: "ThiHanhPhapLuatChiTietNhiemVus");

            migrationBuilder.DropTable(
                name: "ThiHanhPhapLuatNhiemVus");

            migrationBuilder.DropTable(
                name: "ThiHanhPhapLuatKeHoachs");
        }
    }
}
