/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 *
 * Este código faz parte do Orchard e é livre para distribuição
 * 
 * 
/****************************************************************************************/


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace XNuvem.Utility.Extensions
{
    public static class ReadOnlyCollectionExtensions
    {
        public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }
    }
}