using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlackSugar.Wpf
{
    public static class ResourceDictionaryEx
    {
        public static void AddSource(
            this System.Collections.ObjectModel.Collection<ResourceDictionary> dict, 
            string sourceUri) 
            => dict.Add(new ResourceDictionary() { Source = new Uri(sourceUri) });

        public static void AddRangeSource(
            this System.Collections.ObjectModel.Collection<ResourceDictionary> dict,
            params string[] sourceUries)
        {
            foreach(var sourceUri in sourceUries)
                dict.AddSource(sourceUri);
        }

    }
}
