# Entity Mapper
[![CI](https://github.com/YonatanCohavi/EntityMapper/actions/workflows/build.yml/badge.svg)](https://github.com/YonatanCohavi/EntityMapper/actions/workflows/build.yml)
![Nuget](https://img.shields.io/nuget/v/EntityMapper.Dynamics.CRM)

Entity mapper is an attribute based mapper for **Microsoft Dynamics CRM SDK**.

## Existing methods

There are two programming models that you can choose from **Early bound** and **Late bound**.

### Late bound method

> Use the code generation tool (CrmSvcUtil) to create early-bound entity classes, derived from the [Entity](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.entity) class, which you can use to access business data in Dynamics 365 Customer Engagement. These classes include one class for each entity in your installation, including custom entities. More information: [Use the early bound entity classes in code](https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/org-service/use-early-bound-entity-classes-code)

### Early bound method

> The [Entity](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.entity) class contains the logical name of an entity and a property-bag array of the entity's attributes. This lets you use late binding so that you can work with types such as custom entities and custom attributes that were not present when your application was compiled. More information: [Use the late bound entity class in code](https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/org-service/use-late-bound-entity-class-code)

### Entity Mapper method

Lets you enjoy the **stability** of a strong typed model and the **agility** of a property-bag array.

## Usage

### The "CrmEntityBase" Class

This basic class holds only two attributes

**LogicalName** (the schema name for the sorce entity)
**Id** (as you know every entity in dynamics crm has one)

_Every model you build has to inherit from this class._

```c#
public abstract class CrmEntityBase
{
    public abstract string LogicalName { get; }
    public Guid Id { get; set; }
}
```

### A Basic Model

```c#
public class ContactModel : CrmEntityBase
{
    public override string LogicalName => "contact";

    [CRMField("fullname")]
    public string FullName { get; set; }

    [CRMLookup("parentaccountid", "account")]
    public Guid? ParentAccountId { get; set; }
}
```

### Mapping From Entity

```c#
var contactEntity = new Entity("contact")
{
    ["fullname"] = "John Doe",
    ["parentaccountid"] = new EntityReference("account", Guid.Empty)
};

var contactModel = Mapper.ToModel<ContactModel>(contactEntity);

Console.WriteLine(contactModel.FullName);
Console.WriteLine(contactModel.ParentAccountId.ToString());

// The example displays the following output:
//  John Doe
//  00000000-0000-0000-0000-000000000000
```

### Mapping To Entity

```c#
var contactModel = new ContactModel
{
    FullName = "Bar Refaeli",
    ParentAccountId = Guid.Empty,
};

var contactEntity = Mapper.ToEntity(contactModel);
var fullname = contactEntity.GetAttributeValue<string>("fullname");
var parentAccountRef = contactEntity.GetAttributeValue<EntityReference>("parentaccountid");

Console.WriteLine(fullname);
Console.WriteLine(parentAccountRef.LogicalName);
Console.WriteLine(parentAccountRef.Id);

// The example displays the following output:
//  Bar Refaeli
//  account
//  00000000-0000-0000-0000-000000000000
```

### Getting The Required ColumnSet

```c#
var contactColumnSet = Mapper.GetColumnSet<ContactModel>();

Console.WriteLine(string.Join(", ",contactColumnSet.Columns));

// The example displays the following output:
//  fullname, parentaccountid
```

## CRMFieldType

This is the type of the field that we are declaring.
It has the following values:

1. **Basic** - A general map `string` > `string`, `int` > `int`, etc..
2. **OptionSet** - `int` get / set the value of OptionSet
3. [**Lookup**](#Lookup) - `Guid` get / set the id of the referenced target
4. **Money** - `decimal` get / set the value of the Money object
5. [**FormattedValue**](#FormattedValue) - `string` get (only) the formatted value
6. [**Enum**](#enum) - `Enum` get / set the value of OptionSet (using its numeric value)
7. [**LookupName**](#lookupname) - `string` get the reference name.

## Attributes Examples

### Lookup

```c#
public class ContactModel : CrmEntityBase
{
    ...
    [CRMLookup("parentaccountid", "account")]
    public Guid? ParentAccountId { get; set; }
    ...
}
```

```c#
var contactModel = new ContactModel
{
    ParentAccountId = Guid.Empty,
};

var contactEntity = Mapper.ToEntity(contactModel);
var parentAccountRef = contactEntity.GetAttributeValue<EntityReference>("parentaccountid");
var contactModelAgain = Mapper.ToModel<ContactModel>(contactEntity);

Console.WriteLine(parentAccountRef.LogicalName);
Console.WriteLine(parentAccountRef.Id);
Console.WriteLine(contactModelAgain.ParentAccountId);

// The example displays the following output:
//  account
//  00000000-0000-0000-0000-000000000000
//  00000000-0000-0000-0000-000000000000
```

### FormattedValue

```c#
public class ContactModel : CrmEntityBase
{
    ...
    [CRMField("parentaccountid", CRMFieldType.FormattedValue)]
    public string ParentAccountIdName { get; set; }
    ...
}
```

```c#
var contactEntity = new Entity("contact", Guid.NewGuid())
{
    ["parentaccountid"] = new EntityReference("account", Guid.Empty)
};

// faking formatted value
contactEntity.FormattedValues["parentaccountid"] = "Mama Theresa";
var contactModel = Mapper.ToModel<ContactModel>(contactEntity);

Console.WriteLine(contactModel.ParentAccountIdName);

// The example displays the following output:
//  Mama Theresa
```

### Enum

```c#
public enum ContactMood
{
    Sad = 0,
    Tired = 1,
    Happy = 2,
    Coding = 14,
}

public class ContactModel : CrmEntityBase
{
    ...
    [CRMEnum("new_mood", typeof(ContactMood?))]
    public ContactMood? Mood { get; set; }
    ...
}
```

```c#
var contactModel = new ContactModel
{
    FullName = "Bar Refaeli",
    ParentAccountId = Guid.Empty,
    Mood = ContactMood.Coding
};

var contactEntity = Mapper.ToEntity(contactModel);
var contactState = contactEntity.GetAttributeValue<OptionSetValue>("new_mood");
var contactModelAgain = Mapper.ToModel<ContactModel>(contactEntity);

Console.WriteLine(contactState.Value);
Console.WriteLine(contactModelAgain.Mood);
Console.WriteLine(contactModelAgain.Mood.GetType().Name);

// The example displays the following output:
//  14
//  Coding
//  ContactMood
```

### LookupName

```c#
public class ContactModel : CrmEntityBase
{
    ...
    [CRMField("parentaccountid", CRMFieldType.LookupName)]
    public string ParentAccountIdName { get; set; }
    ...
}
```

```c#
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

// The example displays the following output:
//  Mama Theresa
```

### Working With Aliased Values

In development
