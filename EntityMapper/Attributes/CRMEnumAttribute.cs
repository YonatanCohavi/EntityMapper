using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityMapper.Attributes
{
    public class CRMEnumAttribute : CRMFieldBaseAttribute
    {
        public override CRMFieldType Type => CRMFieldType.Enum;

        public CRMEnumAttribute(string attributeName, Type enumType)
        {
            AttributeName = attributeName;
            EnumType = enumType;
        }
    }
}
