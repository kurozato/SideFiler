﻿using Microsoft.Xaml.Behaviors;
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
using System.Windows.Interop;

namespace BlackSugar.Views
{
    public class WindowHandleBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty WindowHandleProperty =
            DependencyProperty.Register(
                nameof(WindowHandle), typeof(IntPtr),
                typeof(WindowHandleBehavior),
                new FrameworkPropertyMetadata(IntPtr.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IntPtr WindowHandle
        {
            get => (IntPtr)this.GetValue(WindowHandleProperty);
            set => this.SetValue(WindowHandleProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += this.WindowLoaded;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
                AssociatedObject.Loaded -= this.WindowLoaded;
            base.OnDetaching();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            WindowHandle = new WindowInteropHelper(AssociatedObject)?.Handle ?? IntPtr.Zero;
        }
    }
}

