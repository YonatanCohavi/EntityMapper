using EntityMapper.Attributes;
using EntityMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTester.Models
{
    public enum ContactMood
    {
        Sad = 0,
        Tired = 1,
        Happy = 2,
        Coding = 14,
    }
    public class ContactModel : CrmEntityBase
    {
        public override string LogicalName => "contact";

        [CRMField("fullname")]
        public string FullName { get; set; }

        [CRMLookup("parentaccountid", "account")]
        public Guid? ParentAccountId { get; set; }

        [CRMField("parentaccountid", CRMFieldType.LookupName)]
        public string ParentAccountIdName { get; set; }
    }
}
