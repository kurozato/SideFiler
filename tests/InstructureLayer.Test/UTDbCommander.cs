using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlackSugar.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Repository.Tests
{
    [TestClass()]
    public class UTDbCommander
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            var qry = "CREATE TABLE IF NOT EXISTS User(" +
                "ID TEXT NOT NULL, " +
                "name TEXT NOT NULL) ";

            var commander = new DbCommander();
            var connect = commander.ConnectionString("test.db");
            commander.Execute(qry, null, connect);

            var inert = "INSERT INTO User(ID, name)VALUES('001','Sato')";

            var result = commander.Execute(inert, null, connect);
            result.Is(1);
        }

        [TestMethod()]
        public void GetTest()
        {
            var qry = "Select * From User";

            var commander = new DbCommander();
            var connect = commander.ConnectionString("test.db");
            var results = commander.Get<dynamic>(qry, null, connect);

            results.Any().Is(true);
        }
    }
}