using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;


namespace BlackSugar.Views
{
    [TypeConstraint(typeof(Selector))]
    public class SelectedItemsBehavior : Behavior<Selector>
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItems), typeof(IEnumerable),
                typeof(SelectedItemsBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
                

        public IEnumerable SelectedItems
        {
            get => (IEnumerable)this.GetValue(SelectedItemsProperty);
            set => this.SetValue(SelectedItemsProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += this.SelectionChanged;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
                AssociatedObject.SelectionChanged -= this.SelectionChanged;
            base.OnDetaching();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic selector = AssociatedObject;
            SelectedItems = Enumerable.ToArray(selector.SelectedItems);

        }
    }
}

