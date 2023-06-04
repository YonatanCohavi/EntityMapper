﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityMapper
{
    public abstract class CrmEntityBase
    {
        public virtual string PrimaryId => $"{LogicalName}id";
        public abstract string LogicalName { get; }
        public Guid Id { get; set; }
    }
}
