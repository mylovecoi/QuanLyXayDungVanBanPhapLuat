using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddWorkflowSoanThao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMucQuyTrinhSoanThaos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaQuyTrinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenQuyTrinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DanhMucVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CapApDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhienBan = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayHieuLuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayHetHieuLuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucQuyTrinhSoanThaos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucQuyTrinhSoanThaos_DanhMucVanBans_DanhMucVanBanId",
                        column: x => x.DanhMucVanBanId,
                        principalTable: "DanhMucVanBans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DanhMucBuocQuyTrinhs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuyTrinhSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaBuoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenBuoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    LoaiBuoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatBuoc = table.Column<bool>(type: "bit", nullable: false),
                    ChoPhepBoQua = table.Column<bool>(type: "bit", nullable: false),
                    ChoPhepQuayLui = table.Column<bool>(type: "bit", nullable: false),
                    CachHoanThanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoLuongPhanHoiToiThieu = table.Column<int>(type: "int", nullable: true),
                    YeuCauFileDinhKem = table.Column<bool>(type: "bit", nullable: false),
                    SoLanTraLaiToiDa = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucBuocQuyTrinhs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucBuocQuyTrinhs_DanhMucQuyTrinhSoanThaos_QuyTrinhSoanThaoId",
                        column: x => x.QuyTrinhSoanThaoId,
                        principalTable: "DanhMucQuyTrinhSoanThaos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucChuyenBuocQuyTrinhs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuyTrinhSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TuBuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DenBuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DieuKienKetQua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaNhanhMacDinh = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucChuyenBuocQuyTrinhs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucChuyenBuocQuyTrinhs_DanhMucBuocQuyTrinhs_DenBuocId",
                        column: x => x.DenBuocId,
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DanhMucChuyenBuocQuyTrinhs_DanhMucBuocQuyTrinhs_TuBuocId",
                        column: x => x.TuBuocId,
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DanhMucChuyenBuocQuyTrinhs_DanhMucQuyTrinhSoanThaos_QuyTrinhSoanThaoId",
                        column: x => x.QuyTrinhSoanThaoId,
                        principalTable: "DanhMucQuyTrinhSoanThaos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoVanBans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenHoSo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DanhMucVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuyTrinhSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocHienTaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DanhMucTrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonViSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiTaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTaoHoSo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoLanTraLaiHienTai = table.Column<int>(type: "int", nullable: false),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBans_DanhMucBuocQuyTrinhs_BuocHienTaiId",
                        column: x => x.BuocHienTaiId,
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBans_DanhMucDonVis_DonViSoanThaoId",
                        column: x => x.DonViSoanThaoId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBans_DanhMucQuyTrinhSoanThaos_QuyTrinhSoanThaoId",
                        column: x => x.QuyTrinhSoanThaoId,
                        principalTable: "DanhMucQuyTrinhSoanThaos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBans_DanhMucTrangThais_DanhMucTrangThaiId",
                        column: x => x.DanhMucTrangThaiId,
                        principalTable: "DanhMucTrangThais",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBans_DanhMucVanBans_DanhMucVanBanId",
                        column: x => x.DanhMucVanBanId,
                        principalTable: "DanhMucVanBans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBans_Users_NguoiTaoId",
                        column: x => x.NguoiTaoId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HoSoVanBanDanhGias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocQuyTrinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanDanhGia = table.Column<int>(type: "int", nullable: false),
                    DonViDanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiDanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KetQuaDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YeuCauChinhSua = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TraLaiBuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanDanhGias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanDanhGias_DanhMucBuocQuyTrinhs_BuocQuyTrinhId",
                        column: x => x.BuocQuyTrinhId,
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanDanhGias_DanhMucBuocQuyTrinhs_TraLaiBuocId",
                        column: x => x.TraLaiBuocId,
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanDanhGias_DanhMucDonVis_DonViDanhGiaId",
                        column: x => x.DonViDanhGiaId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanDanhGias_HoSoVanBans_HoSoVanBanId",
                        column: x => x.HoSoVanBanId,
                        principalTable: "HoSoVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanDanhGias_Users_NguoiDanhGiaId",
                        column: x => x.NguoiDanhGiaId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HoSoVanBanLayYKiens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocQuyTrinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiDuocLayYKienId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonViDuocLayYKienId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NoiDungYeuCau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungPhanHoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanPhanHoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayPhanHoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiPhanHoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanLayYKiens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanLayYKiens_DanhMucBuocQuyTrinhs_BuocQuyTrinhId",
                        column: x => x.BuocQuyTrinhId,
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanLayYKiens_DanhMucDonVis_DonViDuocLayYKienId",
                        column: x => x.DonViDuocLayYKienId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanLayYKiens_HoSoVanBans_HoSoVanBanId",
                        column: x => x.HoSoVanBanId,
                        principalTable: "HoSoVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanLayYKiens_Users_NguoiDuocLayYKienId",
                        column: x => x.NguoiDuocLayYKienId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HoSoVanBanXuLys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocQuyTrinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanXuLy = table.Column<int>(type: "int", nullable: false),
                    DonViXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KetQuaXuLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungXuLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanhMucTrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanXuLys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanXuLys_DanhMucBuocQuyTrinhs_BuocQuyTrinhId",
                        column: x => x.BuocQuyTrinhId,
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanXuLys_DanhMucDonVis_DonViXuLyId",
                        column: x => x.DonViXuLyId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanXuLys_DanhMucTrangThais_DanhMucTrangThaiId",
                        column: x => x.DanhMucTrangThaiId,
                        principalTable: "DanhMucTrangThais",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanXuLys_HoSoVanBans_HoSoVanBanId",
                        column: x => x.HoSoVanBanId,
                        principalTable: "HoSoVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanXuLys_Users_NguoiXuLyId",
                        column: x => x.NguoiXuLyId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HoSoVanBanPhanHoiDanhGias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanDanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanDanhGia = table.Column<int>(type: "int", nullable: false),
                    DonViSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiPhanHoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayPhanHoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoiDungGiaiTrinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanPhanHoiDanhGias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanPhanHoiDanhGias_DanhMucDonVis_DonViSoanThaoId",
                        column: x => x.DonViSoanThaoId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanPhanHoiDanhGias_HoSoVanBanDanhGias_HoSoVanBanDanhGiaId",
                        column: x => x.HoSoVanBanDanhGiaId,
                        principalTable: "HoSoVanBanDanhGias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanPhanHoiDanhGias_HoSoVanBans_HoSoVanBanId",
                        column: x => x.HoSoVanBanId,
                        principalTable: "HoSoVanBans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanPhanHoiDanhGias_Users_NguoiPhanHoiId",
                        column: x => x.NguoiPhanHoiId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucBuocQuyTrinhs_QuyTrinhSoanThaoId",
                table: "DanhMucBuocQuyTrinhs",
                column: "QuyTrinhSoanThaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucChuyenBuocQuyTrinhs_DenBuocId",
                table: "DanhMucChuyenBuocQuyTrinhs",
                column: "DenBuocId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucChuyenBuocQuyTrinhs_QuyTrinhSoanThaoId",
                table: "DanhMucChuyenBuocQuyTrinhs",
                column: "QuyTrinhSoanThaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucChuyenBuocQuyTrinhs_TuBuocId",
                table: "DanhMucChuyenBuocQuyTrinhs",
                column: "TuBuocId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucQuyTrinhSoanThaos_DanhMucVanBanId",
                table: "DanhMucQuyTrinhSoanThaos",
                column: "DanhMucVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanDanhGias_BuocQuyTrinhId",
                table: "HoSoVanBanDanhGias",
                column: "BuocQuyTrinhId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanDanhGias_DonViDanhGiaId",
                table: "HoSoVanBanDanhGias",
                column: "DonViDanhGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanDanhGias_HoSoVanBanId",
                table: "HoSoVanBanDanhGias",
                column: "HoSoVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanDanhGias_NguoiDanhGiaId",
                table: "HoSoVanBanDanhGias",
                column: "NguoiDanhGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanDanhGias_TraLaiBuocId",
                table: "HoSoVanBanDanhGias",
                column: "TraLaiBuocId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanLayYKiens_BuocQuyTrinhId",
                table: "HoSoVanBanLayYKiens",
                column: "BuocQuyTrinhId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanLayYKiens_DonViDuocLayYKienId",
                table: "HoSoVanBanLayYKiens",
                column: "DonViDuocLayYKienId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanLayYKiens_HoSoVanBanId",
                table: "HoSoVanBanLayYKiens",
                column: "HoSoVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanLayYKiens_NguoiDuocLayYKienId",
                table: "HoSoVanBanLayYKiens",
                column: "NguoiDuocLayYKienId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanPhanHoiDanhGias_DonViSoanThaoId",
                table: "HoSoVanBanPhanHoiDanhGias",
                column: "DonViSoanThaoId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanPhanHoiDanhGias_HoSoVanBanDanhGiaId",
                table: "HoSoVanBanPhanHoiDanhGias",
                column: "HoSoVanBanDanhGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanPhanHoiDanhGias_HoSoVanBanId",
                table: "HoSoVanBanPhanHoiDanhGias",
                column: "HoSoVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanPhanHoiDanhGias_NguoiPhanHoiId",
                table: "HoSoVanBanPhanHoiDanhGias",
                column: "NguoiPhanHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBans_BuocHienTaiId",
                table: "HoSoVanBans",
                column: "BuocHienTaiId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBans_DanhMucTrangThaiId",
                table: "HoSoVanBans",
                column: "DanhMucTrangThaiId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBans_DanhMucVanBanId",
                table: "HoSoVanBans",
                column: "DanhMucVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBans_DonViSoanThaoId",
                table: "HoSoVanBans",
                column: "DonViSoanThaoId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBans_NguoiTaoId",
                table: "HoSoVanBans",
                column: "NguoiTaoId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBans_QuyTrinhSoanThaoId",
                table: "HoSoVanBans",
                column: "QuyTrinhSoanThaoId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanXuLys_BuocQuyTrinhId",
                table: "HoSoVanBanXuLys",
                column: "BuocQuyTrinhId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanXuLys_DanhMucTrangThaiId",
                table: "HoSoVanBanXuLys",
                column: "DanhMucTrangThaiId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanXuLys_DonViXuLyId",
                table: "HoSoVanBanXuLys",
                column: "DonViXuLyId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanXuLys_HoSoVanBanId",
                table: "HoSoVanBanXuLys",
                column: "HoSoVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanXuLys_NguoiXuLyId",
                table: "HoSoVanBanXuLys",
                column: "NguoiXuLyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DanhMucChuyenBuocQuyTrinhs");
            migrationBuilder.DropTable(name: "HoSoVanBanLayYKiens");
            migrationBuilder.DropTable(name: "HoSoVanBanPhanHoiDanhGias");
            migrationBuilder.DropTable(name: "HoSoVanBanXuLys");
            migrationBuilder.DropTable(name: "HoSoVanBanDanhGias");
            migrationBuilder.DropTable(name: "HoSoVanBans");
            migrationBuilder.DropTable(name: "DanhMucBuocQuyTrinhs");
            migrationBuilder.DropTable(name: "DanhMucQuyTrinhSoanThaos");
        }
    }
}
