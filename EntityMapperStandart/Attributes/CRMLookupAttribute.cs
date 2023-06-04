using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityMapperStandart.Attributes
{
    public class CRMLookupAttribute : CRMFieldBaseAttribute
    {
        public override CRMFieldType Type => CRMFieldType.Lookup;

        public CRMLookupAttribute(string attributeName, string referenceTo)
        {
            AttributeName = attributeName;
            ReferenceTo = referenceTo;
        }
    }
}
