using EntityMapper.Attributes;
using EntityMapper.Helpes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityMapper
{
    public class Mapper : IEntityMapper
    {
        private static readonly bool ignoreNulls = true;
        public Entity ToEntity(CrmEntityBase model)
        {
            var entity = new Entity(model.LogicalName);
            var helpers = PropertyHelper.GetProperties(model.GetType());
            var attributes = GetAttributes(model, helpers);
            if (attributes.Contains($"{model.LogicalName}id"))
            {
                var id = (Guid)attributes[$"{model.LogicalName}id"];
                if (id == default)
                    attributes.Remove($"{model.LogicalName}id");
                else
                    entity.Id = id;
            }
            entity.Attributes.AddRange(attributes);
            return entity;
        }
        public T ToModel<T>(Entity entity, string entityAlias = null) where T : CrmEntityBase, new()
        {
            var model = new T();
            if (entity.LogicalName != model.LogicalName && entityAlias == null)
                throw new Exception($"logical names must be the same. entity:{entity.LogicalName} model:{model.LogicalName} ");

            var properties = PropertyHelper.GetProperties(typeof(T));
            foreach (var key in entity.Attributes.Keys.Where(k => entityAlias == null || k.StartsWith(entityAlias)))
            {
                var fn = key.Substring(entityAlias?.Length + 1 ?? 0);
                var keyProperties = properties.Where(p => p.CRMFieldBaseAttribute?.AttributeName == fn);
                if (!keyProperties.Any())
                    continue;

                foreach (var helper in keyProperties)
                {
                    var type = helper.CRMFieldBaseAttribute.Type;
                    object modelValue;

                    if (type == CRMFieldType.FormattedValue)
                    {
                        entity.FormattedValues.TryGetValue(key, out var formattedValue);
                        helper.SetValue(model, formattedValue);
                    }
                    else
                    {
                        var entityValue = entity[key];
                        if (entityAlias != null)
                            entityValue = entity.GetAttributeValue<object>(key);

                        modelValue = GetModelValue(helper, entityValue);
                        helper.SetValue(model, modelValue);
                    }
                }
            }
            model.Id = entity.Id;
            return model;
        }
        private AttributeCollection GetAttributes(CrmEntityBase crmEntity, PropertyHelper[] helpers)
        {
            var attributes = new AttributeCollection();
            var relevantHelpers = helpers.Where(ph => ph.CRMFieldBaseAttribute != null);
            foreach (var helper in relevantHelpers)
            {
                var key = helper.CRMFieldBaseAttribute.AttributeName;
                var modelValue = helper.GetValue(crmEntity);
                var value = modelValue;

                if (helper.CRMFieldBaseAttribute.Type != CRMFieldType.Basic)
                    value = GetCrmValue(helper.CRMFieldBaseAttribute, modelValue);

                if (!ignoreNulls)
                    attributes.Add(key, value);
                else if (value != null)
                    attributes.Add(key, value);
            }
            return attributes;
        }
        private object GetCrmValue(CRMFieldBaseAttribute crmFieldAttribute, object value)
        {
            if (value == null)
                return null;
            var type = crmFieldAttribute.Type;
            switch (type)
            {
                case CRMFieldType.Basic:
                    return value;
                case CRMFieldType.Lookup:
                    return new EntityReference(crmFieldAttribute.ReferenceTo, (Guid)value);
                case CRMFieldType.Enum:
                case CRMFieldType.OptionSet:
                    return new OptionSetValue((int)value);
                case CRMFieldType.OptionSetCollection:
                    if (!(value is IEnumerable<int> values))
                        throw new Exception("[OptionSetCollection] value is not IEnumerable<int>");
                    return new OptionSetValueCollection(values.Select(v => new OptionSetValue(v)).ToList());
                case CRMFieldType.Money:
                    return new Money((decimal)value);
                case CRMFieldType.None:
                case CRMFieldType.LookupName:
                case CRMFieldType.FormattedValue:
                default:
                    throw new Exception($"Invalid Crm Type: \"{type}\"");
            }
        }
        private object GetModelValue(PropertyHelper property, object entityValue)
        {
            if (entityValue == null)
                return null;

            if (entityValue is AliasedValue alias)
            {
                entityValue = alias.Value;
            }
            var entityreference = entityValue as EntityReference;
            var optionSet = entityValue as OptionSetValue;
            var money = entityValue as Money;
            var optionSetValueCollection = entityValue as OptionSetValueCollection;

            switch (property.CRMFieldBaseAttribute.Type)
            {
                case CRMFieldType.Basic:
                    return entityValue;
                case CRMFieldType.Lookup:
                    return entityreference.Id;
                case CRMFieldType.LookupName:
                    return entityreference.Name;
                case CRMFieldType.OptionSetCollection:
                    return optionSetValueCollection.Select(o => o.Value).ToArray();
                case CRMFieldType.OptionSet:
                    return optionSet.Value;
                case CRMFieldType.Enum:
                    var enumType = property.Property.PropertyType;
                    // Unwrap nullable types
                    enumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
                    return Enum.ToObject(enumType, optionSet.Value);
                case CRMFieldType.Money:
                    return money.Value;
                case CRMFieldType.None:
                case CRMFieldType.FormattedValue:
                default:
                    throw new Exception($"Invalid Type: \"{property.CRMFieldBaseAttribute.Type}\"");
            }
        }
        public ColumnSet GetColumnSet<T>() where T : CrmEntityBase, new()
        {
            var propHelpers = PropertyHelper.GetProperties(typeof(T));
            var crmAttributes = propHelpers
                .Where(p => p.CRMFieldBaseAttribute != null)
                .Select(p => p.CRMFieldBaseAttribute.AttributeName)
                .Where(an => !an.Contains("."))
                .Distinct();

            var columnSet = new ColumnSet(crmAttributes.ToArray());
            var logicalName = new T().LogicalName;
            columnSet.AddColumn($"{logicalName}id");

            return columnSet;
        }
    }
}
