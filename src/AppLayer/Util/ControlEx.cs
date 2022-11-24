using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlackSugar.Views.Extension
{
    public static class ControlEx
    {
        public static void ItemDoubleClick(this ListView listView, Action action)
        {
            listView.MouseDoubleClick += (s, e) => {
                if (HitTest(listView, e))
                    action?.Invoke();
            };
        }

        public static void BlankAreaClick(this ListView listView, Action action)
        {
            listView.MouseDown += (s, e) => {
                if (HitTest(listView, e) == false)
                    action?.Invoke();
            };

        }

        private static void MouseBindAction(MouseButtonState mouseButton, Action action)
        {
            if(mouseButton == MouseButtonState.Pressed)
                action?.Invoke();
        }

        public static void MouseBind(this ListView listView, MouseButton mouse, Action action)
        {
            listView.MouseDown += (s, e) =>
            {
               
                switch (mouse)
                {
                    case MouseButton.Left:
                        MouseBindAction(e.LeftButton, action);
                        break;
                    case MouseButton.Middle:
                        MouseBindAction(e.MiddleButton, action);
                        break;
                    case MouseButton.Right:
                        MouseBindAction(e.RightButton, action);
                        break;
                    case MouseButton.XButton1:
                        MouseBindAction(e.XButton1, action);
                        break;
                    case MouseButton.XButton2:
                        MouseBindAction(e.XButton2, action);
                        break;
                    default:
                        break;
                }
            };
        }

        public static void KeyBind(this ListView listView, Key key ,Action action)
        {
            listView.KeyDown += (s, e) => {
                if(e.Key == key)
                    action?.Invoke();   
            };
        }

        private static bool HitTest(ListView listView, MouseButtonEventArgs e)
        {
            if (listView.SelectedIndex < 0) return false;

            var item = listView.ItemContainerGenerator.ContainerFromIndex(listView.SelectedIndex) as ListViewItem;
            return null != item.InputHitTest(e.GetPosition(item));
        }

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