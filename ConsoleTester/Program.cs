using ConsoleTester.Models;
using EntityMapper;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var accountRef = new EntityReference("account", Guid.Empty)
            {
                Name = "Mama Theresa"
            };

            var contactEntity = new Entity("contact", Guid.Empty)
            {
                ["parentaccountid"] = accountRef,
            };

            var contactModel = Mapper.ToModel<ContactModel>(contactEntity);

            Console.WriteLine(contactModel.ParentAccountIdName);
        }
    }
}
