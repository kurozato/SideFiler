using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlackSugar.Wpf.Extension
{
    public static class ListViewEx
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

        private static bool HitTest(ListView listView, MouseButtonEventArgs e)
        {
            if (listView.SelectedIndex < 0) return false;

            var item = listView.ItemContainerGenerator.ContainerFromIndex(listView.SelectedIndex) as ListViewItem;
            return null != item?.InputHitTest(e.GetPosition(item));
        }
    }
}
