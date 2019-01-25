# FFLib
FFLib is a library of utilities I use often and like to have available in my projects.

The most common thing I use is my ORM tool. 

The central classes of the ORM are `DBConnection` and `DBTable<>`. Creating a connection requires a `IDBProvider` which is a database specific implementation of the basic CRUD functions. A `SqlServerProvider` is included in the library in the 'Providers' namespace. A `DBTable<>` instance requires a POCO generic type, this will be the type that database fields will be mapped to/from. The name of the table can be passed as a string in the DBTable contructor or as an attribute of the POCO class. Here is an example of a POCO class with a Primary Key field that is a sql Server Identity column.

```
[DBTableName("People")]
public class Person{
	[PrimaryKey]
	[DBIdentity]
	[NotPersisted]
	public int Id {get; set;}
	public string FirstName {get; set;}
	public string LastName {get; set;}
}

var dbp = new FFLib.Data.DBProviders.SqlServerProvider();
var dbc = new DBConnection(dbp,"Typical SqlServer connectionstring");
var personTbl = new DBTable<Person>(dbc);

var person = personTbl.Load(1);

```

The above code delares an DTO of type 'Person' and loads a 'Person' from the database with the Id value of '1'.
The Load function has several overloads, the ones that take a single primary key will load a single object from the database or return a Default(T) if it is not found, they never return null. The overrides that accept a SQL string or a `SqlQuery` Load one or more database records as objects in an array. 

##The Attributes
`[DBTableName(string tanlename)] `
This attribute sets the table name of the entity in the database. The table name can also be set in the `DBTable<>` contructor. If the table name is provided in the constructor it overrides the value of the attribute.

`[PrimaryKey]`
This attribute identifies which property is the primary key. The overloads of `Load()` that take a primary key depend on this attribute to identify the name of the promary key when dynamicly generating the SQL to retreive the record from the database.

`[DBIdentity]`
This attribute indicates the field maps to a SQL Server 'Identity' value and therefore the field must be populated with the server assigned value after an insert operation. When a field is attributed with `[DBIdentity]` and the `Save()` method is called, if it is determined an insert will be performed, the Library performs the insert and then updates the field of the DTO with the 'Identity' value assigned by the database. This attribute is almost always accompanied by the `[NotPersisted]` attribute.

`[NotPersisted]` 
This attribute indicates that the associated property shoudl not be inserted or updated in teh database. The attributed property is ignored for save functions. However it does not stop the field from being populated by functions that read data from the database.

`[MappedTo(string fieldName)]`
This Attribute provides an alternate name for the field when mapping to the database. If this attribute is used the `fieldName` effectively replaces the property name for all SELECT,INSERT,UPDATE operations.


