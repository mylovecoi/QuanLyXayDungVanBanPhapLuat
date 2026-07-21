using DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Services;
using Services.BaoCao12;
using Services.BaoCao17;
using Services.Hubs;
using Services.DinhGiaHHDV.DinhGiaKhac;
using Services.DinhGiaHHDV.GiaThiTruong;
using Services.Manages;
using Services.Manages.ThongTinHoSo;
using Services.Settings;
using Services.Settings.DanhMucDungChung;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Settings.DanhMucGia;
using Services.Systems;
using Services.KeKhaiDangKyGia;
using Services.TraCuuBaoCao;
using Services.TraCuuBaoCao.TraCuu;
using Services.ThamDinhGia;
using System.Text;
using System.Threading.RateLimiting;
using UI.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

//Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection"), sqlOptions =>
    {
        sqlOptions.CommandTimeout(180); // 3 minutes
    })
);

// Bổ sung dịch vụ bộ nhớ cho session
builder.Services.AddDistributedMemoryCache(); // Cần thiết cho Session
// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<IMoMoPaymentService, MoMoPaymentService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Đăng ký HttpContextAccessor (dùng trong Service)
builder.Services.AddHttpContextAccessor();

//
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);    // HSTS trong 1 năm
    options.IncludeSubDomains = true;          // Áp dụng cho subdomains
    options.Preload = true;                    // Đưa vào danh sách preload
});
//Cấu hình Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown_ip",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 15, // Số yêu cầu tối đa
                Window = TimeSpan.FromSeconds(30), // Khoảng thời gian kiểm tra
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2 // Số yêu cầu được xếp hàng đợi
            }
        )
    );

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Quá nhiều yêu cầu, hãy thử lại sau.", cancellationToken);
    };
});
//Services DinhGiaHHDV
builder.Services.AddScoped<IDinhGiaService, DinhGiaService>();
builder.Services.AddScoped<IDinhGiaXetDuyetService, DinhGiaXetDuyetService>();
builder.Services.AddScoped<IGiaThiTruongDanhMucService, GiaThiTruongDanhMucService>();
builder.Services.AddScoped<IGiaThiTruongDanhMucCtService, GiaThiTruongDanhMucCtService>();
builder.Services.AddScoped<IGiaThiTruongService, GiaThiTruongService>();
builder.Services.AddScoped<IGiaThiTruongXetDuyetService, GiaThiTruongXetDuyetService>();
builder.Services.AddScoped<IGiaThiTruongTongHopService, GiaThiTruongTongHopService>();
//End Services DinhGiaHHDV

//Services KeKhaiDangKyGia
builder.Services.AddScoped<IDoanhNghiepService, DoanhNghiepService>();
// builder.Services.AddScoped<IKeKhaiDangKyGiaCsKdService, KeKhaiDangKyGiaCsKdService>();
builder.Services.AddScoped<IKeKhaiDangKyGiaService, KeKhaiDangKyGiaService>();
builder.Services.AddScoped<IKeKhaiDangKyGiaXetDuyetService, KeKhaiDangKyGiaXetDuyetService>();
builder.Services.AddScoped<IKeKhaiDangKyGiaDanhMucService, KeKhaiDangKyGiaDanhMucService>();
builder.Services.AddScoped<IKeKhaiDangKyGiaTheoDoiService, KeKhaiDangKyGiaTheoDoiService>();
//End Services KeKhaiDangKyGia

//Services ThamDinhGia
builder.Services.AddScoped<IThamDinhGiaDanhMucDonViService, ThamDinhGiaDanhMucDonViService>();
builder.Services.AddScoped<IThamDinhGiaDanhMucHangHoaService, ThamDinhGiaDanhMucHangHoaService>();
builder.Services.AddScoped<IThamDinhGiaDanhMucHangHoaCtService, ThamDinhGiaDanhMucHangHoaCtService>();
builder.Services.AddScoped<IThamDinhGiaHoiDongService, ThamDinhGiaHoiDongService>();
builder.Services.AddScoped<IThamDinhGiaHoiDongCtService, ThamDinhGiaHoiDongCtService>();
builder.Services.AddScoped<IThamDinhGiaService, ThamDinhGiaService>();
builder.Services.AddScoped<IThamDinhGiaXetDuyetService, ThamDinhGiaXetDuyetService>();
//End Services ThamDinhGia

//Services Systems
builder.Services.AddSingleton<OTPService>();
builder.Services.AddTransient<SmtpMailService>();

builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IRoleActionService, RoleActionService>();
builder.Services.AddScoped<IGroupPermissionService, GroupPermissionService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISystemInfoService, SystemInfoService>();
builder.Services.AddScoped<IOptionDataService, OptionDataService>();
builder.Services.AddScoped<IChatBotService, ChatBotService>();
//End Services Systems

//Services Settings
builder.Services.AddScoped<IDanhMucDonViService, DanhMucDonViService>();
builder.Services.AddScoped<IDanhMucPhongBanService, DanhMucPhongBanService>();
builder.Services.AddScoped<IDanhMucCanBoService, DanhMucCanBoService>();
builder.Services.AddScoped<IDanhMucDiaDanhService, DanhMucDiaDanhService>();
builder.Services.AddScoped<IDmHopDongService, DmHopDongService>();
builder.Services.AddScoped<IDmHopDongChiTietService, DmHopDongChiTietService>();
builder.Services.AddScoped<IDanhMucPhiLePhiService, DanhMucPhiLePhiService>();
builder.Services.AddScoped<IDmKinhDoanhService, DanhMucKinhDoanhService>();
builder.Services.AddScoped<IDanhMucNuocSachService, DanhMucNuocSachService>();
builder.Services.AddScoped<IDanhMucNuocSachCtService, DanhMucNuocSachCtService>();
builder.Services.AddScoped<IDanhMucGiaChungService, DanhMucGiaChungService>();
builder.Services.AddScoped<IDanhMucGiaChungCtService, DanhMucGiaChungCtService>();
builder.Services.AddScoped<IDanhMucGiaThueTaiNguyenService, DanhMucGiaThueTaiNguyenService>();
builder.Services.AddScoped<IDanhMucGiaThueTaiNguyenCtService, DanhMucGiaThueTaiNguyenCtService>();
//End Services Settings


//Sevices Manages
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IVanBanPhapLuatService, VanBanPhapLuatService>();
builder.Services.AddScoped<IAttachedFileService, AttachedFileService>();
builder.Services.AddScoped<IThongTinNganChanService, ThongTinNganChanService>();
builder.Services.AddScoped<IHoSoCCCTService, HoSoCCCTService>();
builder.Services.AddScoped<IHoSoCCCTXetDuyetService, HoSoCCCTXetDuyetService>();
builder.Services.AddScoped<IHoSoCCCTBaoCaoService, HoSoCCCTBaoCaoService>();
builder.Services.AddScoped<IThuTucHanhChinhService, ThuTucHanhChinhService>();
builder.Services.AddScoped<IHoSoCCCTChiTietService, HoSoCCCTChiTietService>();
builder.Services.AddScoped<IHoSoCCCTDynamicService, HoSoCCCTDynamicService>();

// Services BaoCao12 và BaoCao17
builder.Services.AddScoped<IBaoCao12Service, BaoCao12Service>();
builder.Services.AddScoped<IBaoCao17Service, BaoCao17Service>();
// Đăng ký MoMoPaymentService
builder.Services.Configure<MoMoConfig>(builder.Configuration.GetSection("MoMo"));
builder.Services.AddScoped<ITraCuuService, TraCuuService>();
builder.Services.AddScoped<IBaoCaoService, BaoCaoService>();

// Đăng ký BackgroundTaskQueue và QueuedHostedService
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

builder.Services.AddHostedService<QueuedHostedService>();
// Đăng ký MoMoScheduledTaskService để kiểm tra giao dịch định kỳ
builder.Services.AddHostedService<MoMoScheduledTaskService>();
//End Services Manages

// Filter
builder.Services.AddScoped<SetViewDataFilter>();

//dịch vụ SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<AgentConnectionManager>();

// Cấu hình dịch vụ SmartCAService
builder.Services.AddHttpClient<SmartCAService>();


// Cấu hình JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new ArgumentNullException(nameof(secretKey), "JWT SecretKey cannot be null or empty.");
}

if (string.IsNullOrEmpty(issuer))
{
    throw new ArgumentNullException(nameof(issuer), "JWT Issuer cannot be null or empty.");
}

if (string.IsNullOrEmpty(audience))
{
    throw new ArgumentNullException(nameof(audience), "JWT Audience cannot be null or empty.");
}

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Cho API yêu cầu JWT
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "LifeSoftware";
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(12);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero,
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // Token hợp lệ, có thể thực hiện các thao tác khác tại đây
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new
                {
                    message = "Token has expired."
                });
                return context.Response.WriteAsync(result);
            }

            return Task.CompletedTask;
        }
    };
});

// Đăng ký Swagger trong DI container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        var routeTemplate = apiDesc.RelativePath; // Lấy đường dẫn của API
        return routeTemplate != null && routeTemplate.StartsWith("api/");
        //return routeTemplate != null ;
    });
    c.DocumentFilter<SortPathsDocumentFilter>();
});
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Middleware bảo mật (Dùng OnStarting để tránh lỗi "Headers are already sent")
if (!app.Environment.IsDevelopment()) // Chỉ bật CSP nghiêm ngặt khi Production
{
    app.Use(async (context, next) =>
    {
        context.Response.Headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "script-src 'self' https://cdnjs.cloudflare.com https://apis.google.com https://unpkg.com https://cdn.jsdelivr.net https://maps.googleapis.com https://code.jquery.com https://speedcf.cloudflareaccess.com 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' https://fonts.googleapis.com https://cdnjs.cloudflare.com https://unpkg.com 'unsafe-inline'; " +
            "img-src 'self' https://*.tile.openstreetmap.org https://images.example.com https://quickchart.io data:; " +
            "connect-src 'self' https://*.tile.openstreetmap.org https://api.mapbox.com https://events.mapbox.com; " +
            "font-src 'self' https://fonts.gstatic.com data:; ";

        // Bảo mật bổ sung
        context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

        await next();
    });
}

// HSTS, HTTPS
app.UseHttpsRedirection();
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" },
    RequestPath = "/lifesoft_api_endpoint"
});

// Hỗ trợ file tĩnh (Swagger)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/swagger-ui")),
    RequestPath = "/lifesoft_api_endpoint"
});
app.UseStaticFiles();
// Routing phải đặt trước Authentication & Authorization 
app.UseRouting();
// Session phải đặt trước Authentication
app.UseSession();
// Xác thực & phân quyền
app.UseAuthentication(); // Kích hoạt xác thực cookie
app.UseMiddleware<SessionManagementMiddleware>();
app.UseAuthorization();  // Kích hoạt ủy quyền
// Middleware Custom 
app.UseMiddleware<LoggingMiddleware>();

// Map Routes
//app.MapStaticAssets();
app.MapHub<RequestHub>("RequestHub");
app.MapHub<AgentHub>("/agentHub");

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Bật Swagger trên môi trường Production
var enableSwagger = builder.Configuration.GetValue<bool>("Swagger:Enabled");
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = "lifesoft_api_endpoint"; // Truy cập trực tiếp qua /
    });
}
app.Run();