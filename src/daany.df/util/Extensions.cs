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
		/// <summary>
		/// Converts a DateTime to Unix timestamp (seconds since 1970-01-01).
		/// </summary>
		public static long ToUnixTimestamp(this DateTime dateTime)
		{
			return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeSeconds();
		}

		/// <summary>
		/// Converts a DateTime to Unix timestamp in milliseconds.
		/// </summary>
		public static long ToUnixTimestampMilliseconds(this DateTime dateTime)
		{
			return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
		}

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
	}
}
