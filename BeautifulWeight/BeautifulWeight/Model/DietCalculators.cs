﻿using BeautifulWeight.Menu;
using BeautifulWeight.Model;
using BeautifulWeight.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BeautifulWeight.DietCalculators
{
    public interface DietCalculator
    {
        string Description { get; }
        WeeklyMenu NewMenu(UserProfile userProfile);
        Serving GetEquivalent(Serving serving, UserProfile userProfile);
        bool IsCompatibleWith(Goal goal);
    }

    public static class DietCalculatorLoader
    {
        private static IEnumerable<DietCalculator> _loadedDietCalculators;

        public static IEnumerable<DietCalculator> LoadedDietCalculators
        {
            get
            {
                if (_loadedDietCalculators == null)
                {
                    Load();
                }
                return _loadedDietCalculators;
            }
        }

        private static void Load()
        {
            // Check if user has access to requested .dll
            IEnumerable<String> dllFiles = Directory.EnumerateFiles("../Calculators", "*.dll");
            foreach (string dllFile in dllFiles)
            {
                Assembly DLL = Assembly.LoadFrom(dllFile);
                _loadedDietCalculators = from type in DLL.GetTypes()
                                         where typeof(DietCalculator).IsAssignableFrom(type)
                                         select (DietCalculator)Activator.CreateInstance(type);
            }
        }
    }
}
