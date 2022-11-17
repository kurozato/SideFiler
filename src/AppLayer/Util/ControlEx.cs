using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static void NonImeTextChanged(this TextBox textBox, Action action)
        {
            textBox.Tag = 0;

            TextCompositionManager.AddPreviewTextInputHandler(textBox, (s, e) => textBox.Tag = 0);
            TextCompositionManager.AddPreviewTextInputStartHandler(textBox, (s, e) => textBox.Tag = 1);
            TextCompositionManager.AddPreviewTextInputUpdateHandler(textBox, (s, e) =>
            {
                if (e.TextComposition.CompositionText.Length == 0) textBox.Tag = 0;
            });

            textBox.TextChanged += (s, e) =>
            {
                if (textBox.Tag.Equals(1)) return;

                action?.Invoke();
            };
      
        }

    }

    public class NonImeTextChanged
    {
        bool ime = false;
        TextBox textBox;
        public NonImeTextChanged(TextBox textBox, Action action)
        {
            this.textBox = textBox;
            TextCompositionManager.AddPreviewTextInputHandler(textBox, (s, e) => ime = false);
            TextCompositionManager.AddPreviewTextInputStartHandler(textBox, (s, e) => ime = true);
            TextCompositionManager.AddPreviewTextInputUpdateHandler(textBox, (s, e) =>
            {
                if (e.TextComposition.CompositionText.Length == 0) 
                    ime = false;
            });

            textBox.TextChanged += (s, e) =>
            {
                if (ime) return;

                action?.Invoke();
            };
        }
    }
}