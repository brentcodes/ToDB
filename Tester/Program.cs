using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDB;

namespace Tester
{
    class Program
    {
        class Seller
        {
            public string CompanyId { get; set; }
            public string SellerName { get; set; }
            public string SellerId { get; set; }
            class Company { public string CompanyId { get; set; } }
            static void Main(string[] args)
            {
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

                Console.WriteLine(sql);
                Console.ReadLine();
            }
        }
    }
}
