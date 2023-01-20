using EntityMapper;
using EntityMapper.Attributes;
using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    public enum Hobby
    {
        Eating = 1,
        Soccer = 2,
        Movies = 3,
    }
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
        [CRMField("father.name")]
        public string FathersName { get; set; }
        [CRMField("fullname")]
        public string FullName { get; set; }
        [CRMLookup("parentaccountid", "account")]
        public Guid? ParentAccountId { get; set; }
        [CRMField("parentaccountid", CRMFieldType.FormattedValue)]
        public string ParentAccountIdName { get; set; }
        [CRMField("parentaccountid", CRMFieldType.LookupName)]
        public string ParentAccountIdLookupName { get; set; }
        [CRMEnum("new_mood", typeof(ContactMood?))]
        public ContactMood? Mood { get; set; }
        [CRMField("new_hobbies", CRMFieldType.OptionSetCollection)]
        public Hobby[] Hobbies { get; set; }

    }
}
