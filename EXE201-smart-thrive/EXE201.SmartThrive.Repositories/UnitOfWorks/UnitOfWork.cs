﻿using EXE201.SmartThrive.Data.Context;
using EXE201.SmartThrive.Domain.Contracts.Repositories;
using EXE201.SmartThrive.Domain.Contracts.UnitOfWorks;

namespace EXE201.SmartThrive.Repositories.UnitOfWorks;

public class UnitOfWork : BaseUnitOfWork<STDbContext>, IUnitOfWork
{
    public UnitOfWork(STDbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
    }

    public ISubjectRepository SubjectRepository => GetRepository<ISubjectRepository>();

    public IStudentRepository StudentRepository => GetRepository<IStudentRepository>();

    public IBlogRepository BlogRepository => GetRepository<IBlogRepository>();

    public IFeedbackRepository FeedbackRepository => GetRepository<IFeedbackRepository>();

    public IVoucherRepository VoucherRepository => GetRepository<IVoucherRepository>();

    public IModuleRepository ModuleRepository => GetRepository<IModuleRepository>();

    public ICategoryRepository CategoryRepository => GetRepository<ICategoryRepository>();

    public ICourseRepository CourseRepository => GetRepository<ICourseRepository>();
}