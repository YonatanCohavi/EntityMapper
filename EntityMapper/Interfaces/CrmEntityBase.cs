using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityMapper.Interfaces
{
    public abstract class CrmEntityBase
    {
        public abstract string LogicalName { get; }
        public Guid Id { get; set; }
    }
}
