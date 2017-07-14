## ToDB, a sql generation utility.

ToDB does not aim to abstract away sql, only to make it much easier to write within C#. ToDB gives you the freedom to create your own high quality queries, without the troublesome syntax errors you miss until runtime as with standard inline sql.

You must still understand sql to use this utility. It is also your responsiblity to gaurd against sql injection as this library does not completly mitigate this risk.

ToDB can be used independently, but is geared torwards use with StackExchange's Dapper.net

#### Basic select statement, any order is allowed
```C#
var cmd = new ToDBCommand();
cmd.From("Order")
    .Select("OrderNumber", "OrderId")
    .Where(
        where => where.AreEqual("OrderType", "@OrderType")
    );
if (getMoreInfo)
{
    cmd.Select("OrderDescription,OrderName");
}
string sql = cmd.ToSql();

```
```sql
select OrderNumber,OrderId,OrderDescription,OrderName from Order where OrderType=@OrderType
```

#### Join
```C#
cmd.SelectAllFrom("Order")
    .InnerJoin("Customer", on => 
        on.AreEqual("Order.CustomerId", "Customer.CustomerId")
        .And()
        .AreEqual("Order.RegionId", "Customer.RegionId")
    );
```
```sql
select * from Order 
	inner join Customer 
		on Order.CustomerId=Customer.CustomerId 
		and Order.RegionId=Customer.RegionId
```

### How lucky, our model matches our database schema
```C#
var parameters = new { CompanyId = 5 };
cmd.SelectFrom<Company>()
    .Where(
        where => where.IsEqual(() => parameters.CompanyId)
    );
```
```sql
select CompanyId,CompanyName from Company where CompanyId=@CompanyId
```