﻿using BlackSugar.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace BlackSugar.Views
{
    public class UIHelper
    {
        public static MessageBoxResult ShowErrorMessage(Exception ex)
            => MessageBox.Show(
                ex.Message, 
                "Error", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
    
        public static void Executor(ICommand? command, object? parameter = null)
        {
            if (command?.CanExecute(parameter) == true)
                command?.Execute(parameter);
        }

        public static void Executor<TViewModel>(object viewModel, Func<TViewModel?, ICommand?> getCommand, object? parameter = null)
            where TViewModel : class
        {
            ICommand? command = getCommand(viewModel as TViewModel);
            if (command?.CanExecute(parameter) == true)
                command?.Execute(parameter);
        }

        public static void Refill<T>(ObservableCollection<T>? target, IEnumerable<T>? source, bool clear = true)
        {
            if (clear) target?.Clear();

            foreach(T item in source ?? Enumerable.Empty<T>())
                target?.Add(item);
        }
    }
}
