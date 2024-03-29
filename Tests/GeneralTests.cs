﻿using EntityMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Tests.Models;

namespace Tests
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void MapFromEntity()
        {
            var guid = Guid.NewGuid();
            var accountRef = new EntityReference("account", guid)
            {
                Name = "ParentAccountIdLookupName"
            };
            var mood = ContactMood.Coding;
            var contactEntity = new Entity("contact")
            {
                ["father.name"] = new AliasedValue(null, null, "John's Father"),
                ["fullname"] = "John Doe",
                ["parentaccountid"] = accountRef,
                ["new_mood"] = new OptionSetValue((int)mood),
                ["new_hobbies"] = new OptionSetValueCollection(new[] { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) })
            };

            // faking formatted value
            contactEntity.FormattedValues["parentaccountid"] = "Mama Theresa";
            var mapper = new Mapper();
            var contactModel = mapper.ToModel<ContactModel>(contactEntity);
            Assert.IsTrue(contactModel.Hobbies.Any(h => h == Hobby.Soccer));
            Assert.AreEqual("John Doe", contactModel.FullName);
            Assert.AreEqual("John's Father", contactModel.FathersName);
            Assert.AreEqual(guid, contactModel.ParentAccountId);
            Assert.AreEqual("Mama Theresa", contactModel.ParentAccountIdName);
            Assert.AreEqual("ParentAccountIdLookupName", contactModel.ParentAccountIdLookupName);
            Assert.AreEqual(contactModel.Mood, mood);
            Assert.AreEqual(contactModel.Mood.GetType().FullName, typeof(ContactMood).FullName);
        }

        [TestMethod]
        public void MapToEntity()
        {
            var mood = ContactMood.Coding;
            var guid = Guid.NewGuid();
            var contactModel = new ContactModel
            {
                FullName = "Bar Refaeli",
                ParentAccountId = guid,
                Mood = mood,
                Hobbies = new[] { Hobby.Soccer, Hobby.Eating, Hobby.Movies }
            };
            var mapper = new Mapper();

            var contactEntity = mapper.ToEntity(contactModel);

            var fullname = contactEntity.GetAttributeValue<string>("fullname");
            var parentAccountRef = contactEntity.GetAttributeValue<EntityReference>("parentaccountid");
            var moodValue = contactEntity.GetAttributeValue<OptionSetValue>("new_mood").Value;

            Assert.AreEqual(contactModel.FullName, fullname);
            Assert.AreEqual("account", parentAccountRef.LogicalName);
            Assert.AreEqual(contactModel.ParentAccountId, parentAccountRef.Id);
            Assert.AreEqual(contactModel.Mood, (ContactMood)moodValue);
            Assert.AreEqual(contactModel.Mood.GetType().FullName, typeof(ContactMood).FullName);
        }

        [TestMethod]
        public void GetColumnSet()
        {
            var mapper = new Mapper();
            var contactColumnSet = mapper.GetColumnSet<ContactModel>();
            var expectedColumns = new[] { "fullname", "parentaccountid", "new_mood", "new_hobbies","contactid" };
            Assert.IsTrue(Enumerable.SequenceEqual(expectedColumns, contactColumnSet.Columns));
        }
    }
}
