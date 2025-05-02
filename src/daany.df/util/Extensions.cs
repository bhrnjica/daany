//////////////////////////////////////////////////////////////////////////////
//   ____    _    _   _   _   __  __                                       //
//  |  _ \  / \  | \ | | | \ | |\ \/ /                                     //
//  | | | |/ _ \ |  \| | |  \| | \  /                                      //
//  | |_| / ___ \| |\  | | |\  | | |                                       //
//  |____/_/   \_\_| \_| |_| \_| |_|                                       //
//                                                                         //
//  DAata ANalYtics Library                                                //
//  Daany.DataFrame:Implementation of DataFrame.                           //
//  https://github.com/bhrnjica/daany                                      //
//                                                                         //
//  Copyright © 20019-2025 Bahrudin Hrnjica                                //
//                                                                         //
//  Free. Open Source. MIT Licensed.                                       //
//  https://github.com/bhrnjica/daany/blob/master/LICENSE                  //
//////////////////////////////////////////////////////////////////////////////

using System;
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
            
            FieldInfo fi = value.GetType().GetField(value.ToString())!;

            DescriptionAttribute[]? attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
	}
}
