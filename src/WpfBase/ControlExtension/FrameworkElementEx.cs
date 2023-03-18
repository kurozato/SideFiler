using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace BlackSugar.Wpf.Extension
{
    public static class FrameworkElementEx
    {
        private static void MouseBindAction(MouseButtonState mouseButton, Action action)
        {
            if (mouseButton == MouseButtonState.Pressed)
                action?.Invoke();
        }

        public static void MouseBind(this FrameworkElement element, MouseButton mouse, Action action)
        {
            element.MouseDown += (s, e) =>
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

        public static void KeyBind(this FrameworkElement element, Key key, Action action)
        {
            element.KeyDown += (s, e) => {
                if (e.Key == key)
                    action?.Invoke();
            };
        }

        public static void DragDropFile(this FrameworkElement element, Action<string[]> action)
        {
            element.DragOver += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effects = DragDropEffects.All;
                else
                    e.Effects = DragDropEffects.None;

                e.Handled = true;

            };

            element.Drop += (s, e) =>
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null) action(files);
            };
        }
    }
}
