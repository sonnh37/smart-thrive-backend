﻿using EXE201.SmartThrive.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.SmartThrive.Domain.Models.Requests.Queries.StudentXPackage
{
    public class StudentXPackageGetAllQuery: GetAllQuery
    {
        public Guid? StudentId { get; set; }

        public Guid? PackageId { get; set; }
    }
}