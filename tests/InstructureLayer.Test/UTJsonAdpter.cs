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
    public class UTJsonAdpter
    {
        [TestMethod()]
        public void ConvertFullPathTest()
        {
            var file = "ut.test";
            var expect = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file + ".json");
            var adpter = new JsonAdpter();
            var actual = adpter.ConvertFullPath(file, true);
            actual.Is(expect);
        }

        [TestMethod()]
        public void SaveTest()
        {
            var json = new { name = "aaa", arg = "bbbb" };
            var adpter = new JsonAdpter();
            adpter.Save(json, adpter.ConvertFullPath("ut.test", true));
        }

        [TestMethod()]
        public void GetTest()
        {
            var adpter = new JsonAdpter();
            var node = adpter.Get(adpter.ConvertFullPath("ut.test", true));
            var name = node?["name"]?.ToString();
            var arg = node?["arg"]?.ToString();

            name.Is("aaa");
            arg.Is("bbbb");
        }

        private class NameArg
        {
            public string name { get; set; }    
            public string arg { get; set; }
        }

        [TestMethod()]
        public void GetTValueTest()
        {
            var adpter = new JsonAdpter();
            var value = adpter.Get<NameArg>(adpter.ConvertFullPath("ut.test", true));

            value.name.Is("aaa");
            value.arg.Is("bbbb");
        }

    }
}


