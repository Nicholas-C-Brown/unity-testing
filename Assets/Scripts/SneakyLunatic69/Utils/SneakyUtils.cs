
using System;
using System.Collections.Generic;
using System.Linq;

namespace SneakyLunatic.Utils { 
    public static class SneakyUtils
    {
        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}

