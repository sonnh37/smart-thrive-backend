using System.Text;
using System.Text.Json.Serialization;
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

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
// builder.Services.AddScoped<ICourseRepository, CourseRepository>();
// builder.Services.AddScoped<ICourseXPackageRepository, CourseXPackageRepository>();
// builder.Services.AddScoped<IOrderRepository, OrderRepository>();
// builder.Services.AddScoped<IPackageRepository, PackageRepository>();
// builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
// builder.Services.AddScoped<IRoleRepository, RoleRepository>();
// builder.Services.AddScoped<ISessionRepository, SessionRepository>();
// builder.Services.AddScoped<IStudentRepository, StudentRepository>();
// builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
// builder.Services.AddScoped<IUserRepository, UserRepository>();
//
builder.Services.AddScoped<ISubjectService, SubjectService>();
// builder.Services.AddScoped<ISessionService, SessionService>();
// builder.Services.AddScoped<IOrderService, OrderService>();
// builder.Services.AddScoped<IPackageService, PackageService>();
// builder.Services.AddScoped<ICourseService, CourseService>();
// builder.Services.AddScoped<IProviderService, ProviderService>();
// builder.Services.AddScoped<IStudentService, StudentService>();
// builder.Services.AddScoped<IRoleService, RoleService>();
// builder.Services.AddScoped<ISubjectService, SubjectService>();
// builder.Services.AddScoped<ICategoryService, CategoryService>();
// builder.Services.AddScoped<ICourseXPackageService, CouseXPackageService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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
                builder.Configuration.GetValue<string>("JWT:Token"))),
            ClockSkew = TimeSpan.Zero
        };

        options.Configuration = new OpenIdConnectConfiguration();
    });
// .AddGoogle(options =>
// {
//     IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
//
//     options.ClientId = googleAuthNSection["ClientId"];
//     options.ClientSecret = googleAuthNSection["ClientSecret"];
// });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<TokenUserMiddleware>();
app.UseRouting();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();