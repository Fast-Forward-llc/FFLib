# FFLib
FFLib is a library of utilities I use often and like to have available in my projects.
For the full specification see the documentation: https://fast-forward-llc.github.io/FFLib/api/index.html

# Getting Started

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

The above code delares an DTO of type 'Person' and loads a 'Person' from the database table named 'People' with the Id value of '1'.
The Load function has several overloads, the ones that take a single primary key will load a single object from the database or return a Default(T) if it is not found, they never return null. The overrides that accept a SQL string or a `SqlQuery` Load one or more database records as objects in an array. 

## The Attributes
`[DBTableName(string tanlename)] `
This attribute sets the table name of the entity in the database. The table name can also be set in the `DBTable<>` contructor. If the table name is provided in the constructor it overrides the value of the attribute.

`[PrimaryKey]`
This attribute identifies which property is the primary key. The overloads of `Load()` that take a primary key depend on this attribute to identify the name of the primary key when dynamicly generating the SQL to retreive the record from the database.

`[DBIdentity]`
This attribute indicates the field maps to a SQL Server 'Identity' value and therefore the field must be populated with the server assigned value after an insert operation. When a field is attributed with `[DBIdentity]` and the `Save()` method is called, if it is determined an insert will be performed, the Library performs the insert and then updates the field of the DTO with the 'Identity' value assigned by the database. This attribute is almost always accompanied by the `[NotPersisted]` attribute.

`[NotPersisted]` 
This attribute indicates that the associated property should not be inserted or updated in the database. The attributed property is ignored for save functions. However it does not stop the field from being populated by functions that read data from the database resulting in a read-only like behavior.

`[MapsTo(string fieldName)]`
This Attribute provides an alternate name for the field when mapping to the database. If this attribute is used the `fieldName` effectively replaces the property name for all database operations.

## DBConnection
`DBConnection(IDBProvider dbProvider, string connectionstring)`
The DBConnection constructor takes an IDBProvider and a connection string. `IDBProvider` is the Database specific class that knows how to execute commands on teh target database. A Sql Server Provider `SqlServerProvider` is included in the library. I've always planned to create providers for Postgresql and MySql (and work out the issues and refactor to support them) but have not so far. The connection string is the same connectionstring you would use with System.Data.SqlClient.DbConnection. 

## DBTable\<entityType\>
DBTable is the work horse of the ORM. The idea is that an C# POCO maps to a table and the interaction with that table happens via DBTable. the POCO type is provided as the generic type to the instance of DBTable. The properties of the POCO map to the columns of the table based on a case-sensitive comparison. Any additional or unmapped column in the database are ignored for purposes of reading. Additional or unmapped fields in the database may cause an error inserting or updating if they don't allow null since no value will be provided. Additional or unmapped fields in the POCO will be ignored for the purpose of reading and will cause an error for inserts or updates since there will be no column with the same name. The `[MapsTo]` attribute can be used to supply an alternate name for mapping properties to columns. The primary functions of `DBTable<>` are `Load()`, `LoadOne()`, `Save()`, `Delete()` `ExecuteScalar<resultType>()`, and `Execute()`. Each one has several overloads. Fields and Properties are both supported on the POCO and map to database columns using both implicit and explicit mapping.

ex: `var orderTable = new DBTable<Order>(dbc);`
	
### Load(int id)
This function requires an integer primary key value. It reads the record from the database with the specified 'id' and maps the fields to an instance of the POCO type of the DBTable instance.

### Load\<priKeyType\>(priKeyType id)
This function requires a generic type i.e. int, string, decimal etc. to specify the data type of the primary key and the primary key value as the parameter. It reads the record from the database with the specified 'id' and maps the fields to an instance of the POCO type of the DBTable instance.
	
### Load(string sqlText)
This function loads one or more records from the database by executing the Sql query in the SqlText string. a typical sqlText looks like "SELECT * FROM Orders". The result is Null if the query returns no rows. If the query return 1 or more rows the result will be an array of the DBTable POCO type. It could be an array of one value.

### Load(string sqlText, dynamic parameters)
This function loads one or more records from the database by executing the Sql query in the SqlText string. a typical sqlText looks like "SELECT * FROM Orders WHERE Status = @status". The value of @status is passed as the second parameter as an anonymous object and woudl look like 'new{status="new"}. The whole function might look like: Load("SELECT * FROM Orders WHERE Status = @status",new{status="new"}). The result is Null if the query returns no rows. If the query return 1 or more rows the result will be an array of the DBTable POCO type. It could be an array of only one value.

### Save(POCOType obj)
Inserts or Updates the database table with the values of the POCO properties. If the value of the property decorated with the `[PrimaryKey]` attribute is equal to Default(T) then the POCO is considered new and an insert operation will be performed otherwise an update operation is performed.

### Delete(POCOType obj)
Deletes a record in the database table by the primary key value.

## DBEntity\<T\>
DBEntity is an abstract base class that implements ISupportsIsDirty. By default the DBTable will always save the properties of a POCO class when the `Save()` regardless of whether or not any of them have been changed. If a POCO implements ISupportsIsDirty then the DBTable will use the implemented functions to determine if the properties have been changed since it was loaded and only save if changes have been made i.e. the class is dirty. DBEntity implements ISupportsIsDirty and any POCO that inherits from DBEntity is able to report if it is dirty resulting in the optimization that the properties will only be saved if one or more have been change. Note: when the POCO is determined to be dirty all the properties are saved not just the changed ones.

## SqlQuery
The SqlQuery class allows a Sql query to be built using functions that represent the primary Sql Keywords resulting in a more readable, structured and fluent query with fewer string manipulations that can cause runtime errors. Many of teh DBTable function overloads accept a SqlQuery object. The `Select()` and `From()` function default to " Select * " and " From #\_\_TableName " which is often the desired values so that simple queries can be created by only specifying `Where()` and/or `Order()` such as `var sql = new SqlQuery().Where("name=@name") `. The SqlQuery also has a SqlParameters collection allowing the required parameters to be added and carried with the query for execution. Another advantage is that the Sql statement functions can be set in any order. The `Where()` function is special in that each time it is called the passed value is added to the 'Where' criteria allowing the WHERE clause to be built up over a series of C# commands and logic. example: `var sql = new SqlQuery().Where("name=@name"); i++ ; sql.Where(" OR status=@status");` This would produce the following Sql command: `SELECT * FROM #__TableName WHERE name=@name OR status=@status`. `#__TableName` is a a special place holder for the POCO objects table name that will be replaced with the true table name by DBTable when a function is executed. the `#__TableName` placeholder is valid in any DBTable function that accepts a Sql string or a `SqlQuery`.

## Transactions
The FFLib DBConnection supports implicit transactions. If a transaction is started on a connection all operations using that connection are automacticly enrolled in the transaction, down stream operations do not need to be passed the transaction or be aware of it. Nested transactions are also supported. The combination of nested transactions and implicit transactions makes transaction management simple and manageable.

## DTOBinder
The DTOBinder class is very helpful to map values from one class to another. Properties are mapped by a case-sensitive comparison. The `[MapsTo]` attribute can be used on the source or destination class to provide an alternate name for the property for the purposes of mapping.
