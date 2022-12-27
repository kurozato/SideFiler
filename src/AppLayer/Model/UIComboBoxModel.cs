using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Model
{
    public class UIComboBoxModel
    {
        public string? Content { get; }
        public string? Value { get; }

        public UIComboBoxModel(string content, string value)
        {
            Content = content;
            Value = value;
        }

    }
}
