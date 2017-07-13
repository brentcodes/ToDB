## ToDB, a sql generation utility.

ToDB does not aim to abstract away sql, only to make it much easier to write within C#. ToDB gives you the freedom to create your own high quality queries, without the troublesome syntax errors you miss until runtime as with standard inline sql.

You must still understand sql to use this utility. It is also your responsiblity to gaurd against sql injection as this library does not completly mitigate this risk.

ToDB can be used independently, but is geared torwards use with StackExchange's Dapper.net

#### select statement, any order is allowed
```C#
bool getMoreInfo = true;
Seller instance = new Seller();
var cmd = new ToDBCommand();
cmd.From("Order")
    .Select("OrderNumber")
    .Where(
        where => where.AreEqual("OrderType", "@OrderType")
    );
if (getMoreInfo)
{
    cmd.Select("OrderText");
}
string sql = cmd.ToSql();

```
```sql
select OrderNumber,OrderText from Order where OrderType=@OrderType
```