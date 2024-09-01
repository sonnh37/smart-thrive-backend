﻿using EXE201.SmartThrive.Domain.Contracts.Bases;
using EXE201.SmartThrive.Domain.Contracts.Services;
using EXE201.SmartThrive.Services;
using EXE201.SmartThrive.Services.Base;

namespace EXE201.SmartThrive.API.Registrations;

public static class ServiceRegistration
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseService), typeof(BaseService));
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProviderService, ProviderService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVoucherService, VoucherService>();
    }
}