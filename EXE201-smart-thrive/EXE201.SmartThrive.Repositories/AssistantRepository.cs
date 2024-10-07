﻿using EXE201.SmartThrive.Data.Context;
using EXE201.SmartThrive.Domain.Contracts.Repositories;
using EXE201.SmartThrive.Domain.Entities;
using EXE201.SmartThrive.Repositories.Base;

namespace EXE201.SmartThrive.Repositories
{
    public class AssistantRepository : BaseRepository<Assistant>, IAssistantRepository
    {
        public AssistantRepository(STDbContext dbContext) : base(dbContext)
        {
        }
    }
}
