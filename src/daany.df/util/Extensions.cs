//////////////////////////////////////////////////////////////////////////////////////////
// Daany - DAta ANalYtics Library                                                        //
// https://github.com/bhrnjica/daany                                                    //
//                                                                                      //
// Copyright 2006-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/daany/blob/master/LICENSE        //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica at hotmail.com                                                              //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace Daany
{
    public static class ExtensionMethods
    {
        
        public static string GetEnumDescription(this Enum value)
        {
            
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
#if NETSTANDARD2_0
    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
		{
			var buffer = new List<T>();
			int pos = 0;
	
			foreach (var item in source)
			{
				if (buffer.Count < count)
				{
					// phase 1
					buffer.Add(item);
				}
				else
				{
					// phase 2
					buffer[pos] = item;
					pos = (pos+1) % count;
				}
			}
	
			for (int i = 0; i < buffer.Count; i++)
			{
				yield return buffer[pos];
				pos = (pos+1) % count;
			}
		}

	public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
		{
			var buffer = new List<T>();
			int pos = 0;
	
			foreach (var item in source)
			{
				if (buffer.Count < count)
				{
					// phase 1
					buffer.Add(item);
				}
				else
				{
					// phase 2
					yield return buffer[pos];
					buffer[pos] = item;
					pos = (pos+1) % count;
				}
			}
		}
#endif
	}
}
