﻿using EXE201.SmartThrive.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.SmartThrive.Domain.Models.Requests.Commands.Module
{
    public class ModuleCreateCommand : CreateCommand
    {
        public Guid? CourseId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
