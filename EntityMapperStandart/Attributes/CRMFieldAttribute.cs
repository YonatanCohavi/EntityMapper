using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityMapperStandart.Attributes
{
    public class CRMFieldAttribute : CRMFieldBaseAttribute
    {
        private CRMFieldType _type;
        public override CRMFieldType Type => _type;
        public CRMFieldAttribute(string attributeName, CRMFieldType type = CRMFieldType.Basic)
        {
            _type = type;
            AttributeName = attributeName;
        }
    }
}
