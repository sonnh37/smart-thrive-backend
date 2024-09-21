using System.Text;
using System.Text.Json.Serialization;
using EXE201.SmartThrive.Data;
using EXE201.SmartThrive.Data.Context;
using EXE201.SmartThrive.Domain.Configs.Mappings;
using EXE201.SmartThrive.Domain.Contracts.Bases;
using EXE201.SmartThrive.Domain.Contracts.Repositories;
using EXE201.SmartThrive.Domain.Contracts.Services;
using EXE201.SmartThrive.Domain.Contracts.UnitOfWorks;
using EXE201.SmartThrive.Domain.Middleware;
using EXE201.SmartThrive.Repositories;
using EXE201.SmartThrive.Repositories.Base;
using EXE201.SmartThrive.Repositories.UnitOfWorks;
using EXE201.SmartThrive.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("starting server.");
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });
    
    builder.Services.AddControllers().AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
    });

    builder.Services.AddDbContext<STDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SmartThrive"));
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });

    builder.Services.AddAutoMapper(typeof(MappingProfile));

    #region Add-Scoped

    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

    builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
    builder.Services.AddScoped<ISubjectService, SubjectService>();

    builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
    builder.Services.AddScoped<IFeedbackService, FeedbackService>();

    builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
    builder.Services.AddScoped<IModuleService, ModuleService>();

    builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
    builder.Services.AddScoped<IVoucherService, VoucherService>();

    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();

    builder.Services.AddScoped<ICourseRepository, CourseRepository>();
    builder.Services.AddScoped<ICourseService, CourseService>();

    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IOrderService, OrderService>();

    builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
    builder.Services.AddScoped<IProviderService, ProviderService>();

// builder.Services.AddScoped<ICourseXPackageRepository, CourseXPackageRepository>();
// builder.Services.AddScoped<IOrderRepository, OrderRepository>();
// builder.Services.AddScoped<IPackageRepository, PackageRepository>();
// builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
// builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<ISessionRepository, SessionRepository>();
    builder.Services.AddScoped<IStudentRepository, StudentRepository>();
    builder.Services.AddScoped<IStudentService, StudentService>();

    builder.Services.AddScoped<IBlogRepository, BlogRepository>();
    builder.Services.AddScoped<IBlogService, BlogService>();

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UserService>();

    builder.Services.AddScoped<ISessionMeetingRepository, SessionMeetingRepository>();


    builder.Services.AddScoped<ISessionOfflineRepository, SessionOfflineRepository>();


    builder.Services.AddScoped<ISessionSelfLearnRepository, SessionSelfLearnRepository>();

    builder.Services.AddScoped<ISessionRepository, SessionRepository>();
    builder.Services.AddScoped<ISessionService, SessionService>();
// builder.Services.AddScoped<IRoleService, RoleService>();
//builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// builder.Services.AddScoped<ICourseXPackageService, CouseXPackageService>();
// builder.Services.AddScoped<ICourseXPackageRepository, CourseXPackageRepository>();

// builder.Services.AddScoped<IPackageRepository, PackageRepository>();
// builder.Services.AddScoped<ISessionRepository, SessionRepository>();

    #endregion

    builder.Services.AddHttpContextAccessor();
//Register session type
    SessionService.RegisterProductType("Meeting", typeof(SessionService.SessionMeetingService));
    SessionService.RegisterProductType("Offline", typeof(SessionService.SessionOfflineService));
    SessionService.RegisterProductType("SelfLearn", typeof(SessionService.SessionSelfLearnService));

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    #region Config_Authentication

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    builder.Configuration.GetValue<string>("JWT:Token") ?? string.Empty)),
                ClockSkew = TimeSpan.Zero
            };

            options.Configuration = new OpenIdConnectConfiguration();
        });

    builder.Services.AddAuthorization();

    #endregion

// .AddGoogle(options =>
// {
//     IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
//
//     options.ClientId = googleAuthNSection["ClientId"];
//     options.ClientSecret = googleAuthNSection["ClientSecret"];
// });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<RequestTokenUserMiddleware>();

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<STDbContext>();
        DummyData.SeedDatabase(context);
    }

    app.UseRouting();

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}