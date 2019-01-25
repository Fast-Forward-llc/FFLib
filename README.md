# FFLib
FFLib is a library of utilities I use often and like to have available in my projects.

The most common thing I use is my ORM tool. 

# Getting Started

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

## The Attributes
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

## DBConnection
`DBConnection(IDBProvider dbProvider, string connectionstring)`
The DBConnection constructor takes an IDBProvider and a connection string. `IDBProvider` is the Database specific class that knows how to execute commands on teh target database. A Sql Server Provider `SqlServerProvider` is included in the library. I've always planned to create providers for Postgresql and MySql (and work out the issues and refactor to support them) but have not so far. The connection string is the same connectionstring you would use with System.Data.SqlClient.DbConnection. 

## DBTable\<entityType\>
DBTable is the work horse of the ORM. The idea is that an C# POCO maps to a table and the interaction with that table happens via DBTable. the POCO type is provided as the generic type to the instance of DBTable. The main functions are `Load()`, `LoadOne()`, `ExecuteScalar<resultType>()`, and `Execute()`. Each one has several overloads.

ex: `var orderTable = new DBTable<Order>(dbc);`
	
### Load(int id)
This function requires an integer primary key value. It reads the record from the database with the specified 'id' and maps the fields to an instance of the POCO type of the DBTable instance.

### Load\<priKeyType\>(priKeyType id)
This function requires a generic type i.e. int, string, decimal etc. to specify the data type of the primary key and the primary key value as the parameter. It reads the record from the database with the specified 'id' and maps the fields to an instance of the POCO type of the DBTable instance.
	
### Load(string sqlText)
This function loads one or more records from the database by executing the Sql query in the SqlText string. a typical sqlText looks like "SELECT * FROM Orders". The result is Null if the query returns no rows. If the query return 1 or more rows the result will be an array of the DBTable POCO type. It could be an array of one value.

### Load(string sqlText, dynamic parameters)
This function loads one or more records from the database by executing the Sql query in the SqlText string. a typical sqlText looks like "SELECT * FROM Orders WHERE Status = @status". The value of @status is passed as the second parameter as an anonymous object and woudl look like 'new{status="new"}. The whole function might look like: Load("SELECT * FROM Orders WHERE Status = @status",new{status="new"}). The result is Null if the query returns no rows. If the query return 1 or more rows the result will be an array of the DBTable POCO type. It could be an array of only one value.
