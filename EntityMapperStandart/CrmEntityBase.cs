using System;

namespace EntityMapperStandart
{
    public abstract class CrmEntityBase
    {
        public virtual string PrimaryId => $"{LogicalName}id";
        public abstract string LogicalName { get; }
        public Guid Id { get; set; }
    }
}
