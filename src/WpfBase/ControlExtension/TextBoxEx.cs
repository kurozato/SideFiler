using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BlackSugar.Wpf.Extension
{
    public static class TextBoxEx
    {
        /// <summary>
        /// SelectAll when TextBox Focused 
        /// </summary>
        /// <param name="textBox"></param>
        public static void SetFocusSelectAll(this TextBox textBox)
        {
            textBox.GotFocus += (s, e) => textBox.SelectAll();
            textBox.PreviewMouseLeftButtonDown += (s, e) =>
            {
                if (textBox.IsFocused) return;

                e.Handled = true;
                textBox.Focus();
            };
        }

    }
}
