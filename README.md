# FFLib
FFLib is a library of utilities I use often and like to have available in my projects.

The most important of which is my ORM tool. 

The central classes of the ORM ar `DBConnection` and `DBTable<>`. Creating a connection requires a `IDBProvider` which is a database specific implementation of the basic CRUD functions. A `SqlServerProvider` is included in the library. A `DBTable<>` instance requires a POCO generic type, this will be the type that database fields will be mapped to/from. The name of the table can be passed as a string in the DBTable contructor or as an attribute of the POCO class. Here is an example of a POCO class with a Primary Key field that is a sql Server Identity column.

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
The Load function has several overrides, the ones that take a single primary key will load the object from the database or return a Default(T) if it is not found. The overrides that accept a SQL string or a `SqlQuery` Load one or more database records as objects as an array. 
