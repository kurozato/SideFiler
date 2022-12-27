using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.SimpleMvp
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAutoLinkAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ActionManualLinkAttribute : Attribute
    {
        private string name;
        public string Name => name;

        public ActionManualLinkAttribute(string name) 
        {
            this.name = name; 
        }
     
    }
}
