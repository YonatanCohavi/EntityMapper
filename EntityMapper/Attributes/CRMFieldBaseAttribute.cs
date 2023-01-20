using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityMapper.Attributes
{
    public enum CRMFieldType
    {
        None= 0,
        Basic = 1,
        OptionSet = 2,
        Lookup = 3,
        Money = 4,
        FormattedValue = 5,
        Enum = 6,
        LookupName = 7,
        OptionSetCollection = 8,
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class CRMFieldBaseAttribute : Attribute
    {
        public abstract CRMFieldType Type { get; }
        public string AttributeName { get; set; }
        public string ReferenceTo { get; set; }
        public Type EnumType { get; set; }
    }
}
