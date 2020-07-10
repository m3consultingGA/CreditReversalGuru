using CreditReversal.DAL;
using CreditReversal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;

namespace CreditReversal.Utilities
{
    public static class ExtensionMethods
    {
        public static string PadUpto2(this String str)
        {
            if (str.Contains('.'))
            {
                int index = str.IndexOf('.');
                string first = str.Substring(0, index);
                string sub = str.Substring(index + 1);
                if (sub.Length == 1)
                {
                    sub += "0";
                    str = first + "." + sub;
                    return str;
                }
                if (sub.Length == 2)
                {
                    sub += "";
                    str = first + "." + sub;
                    return str;
                }
                if (sub.Length >= 3)
                {
                    string s1 = sub.Substring(0, 2);
                    str = first + "." + s1;
                    return str;
                }
            }
            else
            {
                str = str + ".00";
            }
            return str;
        }
        public static string PadUpto3(this String str)
        {
            if (str.Contains('.'))
            {
                int index = str.IndexOf('.');
                string first = str.Substring(0, index);
                string sub = str.Substring(index + 1);
                if (sub.Length == 1)
                {
                    sub += "00";
                    str = first + "." + sub;
                    return str;
                }
                if (sub.Length == 2)
                {
                    sub += "0";
                    str = first + "." + sub;
                    return str;
                }
                if (sub.Length == 3)
                {
                    sub += "";
                    str = first + "." + sub;
                    return str;
                }
                if (sub.Length >= 4)
                {
                    string s1 = sub.Substring(0, 3);
                    str = first + "." + s1;
                    return str;
                }
            }
            else
            {
                str = str + ".000";
            }
            return str;
        }
        public static string PadUpto4(this String str)
        {
            if (str.Contains('.'))
            {
                int index = str.IndexOf('.');
                string first = str.Substring(0, index);
                string sub = str.Substring(index + 1);
                if (sub.Length == 1)
                {
                    sub += "000";
                }
                else if (sub.Length == 2)
                {
                    sub += "00";
                }
                else if (sub.Length == 3)
                {
                    sub += "0";
                }
                str = first + "." + sub;
            }
            else
            {
                if (str == "")
                {
                    str = str + "0.0000";
                }
                else
                {
                    str = str + ".0000";
                }
            }
            return str;
        }
        public static IEnumerable<T> RetVal<T>(this IEnumerable<T> en, string search)
        {
            foreach (T val in en)
            {
                if (val.ToString().Contains(search))
                    yield return val;
            }
        }
        public static string RemoveSpecialCharacters(this String str, string allowedCharacters)
        {
            char[] buffer = new char[str.Length];
            int index = 0;

            char[] allowedSpecialCharacters = allowedCharacters.ToCharArray();
            foreach (char c in str.Where(c => char.IsLetterOrDigit(c) || allowedSpecialCharacters.Any(x => x == c)))
            {
                buffer[index] = c;
                index++;
            }
            return new string(buffer, 0, index);
        }
        public static string RemoveSpecialCharactersForAzureContainer(this String str)
        {
            StringBuilder sb = new StringBuilder();
            str = str.ToLowerInvariant();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || c == '-')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public static string TrimIfNotNull(this String str)
        {
            if (!(str == null || str.Length == 0))
                return str.Trim();
            return str;
        }
        public static string ConvertObjectToStringIfNotNull(this object obj)
        {
            string retval = "";
            try
            {
                if (obj != null)
                {
                    var stringObject = obj as String;
                    if (stringObject != null)
                        retval = obj.ToString();
                }
            }
            catch (Exception e)
            {
                return retval;
            }
            return retval;
        }
        public static int ConvertObjectToIntIfNotNull(this object obj)
        {
            int retval = 0;
            try
            {
                if (obj != null)
                {
                    
                        retval = Convert.ToInt32(obj);
                }
            }
            catch (Exception e)
            {
                return retval;
            }
            return retval;
        }
        public static bool CheckIfStringContainsAnotherStringIgnoringCase(this String str, string sourceString)
        {
            bool isExists = false;
            isExists = sourceString.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0;
            return isExists;
        }
        public static int ToInt32(this int? str)
        {
            return str != null ? Convert.ToInt32(str) : 0;
        }
        public static float StringToFloat(this String ipStrValue, float ifFail)
        {
            float output, output1;

            if (float.TryParse(ipStrValue, out output1))
            {
                output = output1;
            }
            else
            {
                output = ifFail;
            }

            return output;
        }
        public static double StringToDouble(this String ipDoubleStrValue, double defaultValue)
        {
            double retval = defaultValue;
            try
            {
                if (!string.IsNullOrEmpty(ipDoubleStrValue))
                    retval = Convert.ToDouble(ipDoubleStrValue);
            }
            catch
            {
            }
            return retval;
        }
        public static float IntToFloat(this int? ipIntValue)
        {
            return Convert.ToSingle(ipIntValue);

        }
        public static double IntToDouble(this int? ipIntValue)
        {
            return Convert.ToDouble(ipIntValue);
        }
        /// <summary>
        /// Used to convert a string to int
        /// </summary>
        /// <param name="strVal">string to convert</param>
        /// <param name="defaultValue">default value if the string is not in the correct format</param>
        /// <returns>integer</returns>
        public static int StringToInt(this String strVal, int defaultValue)
        {
            int retval = defaultValue;
            try
            {
                if (!string.IsNullOrEmpty(strVal))
                    retval = Convert.ToInt32(strVal);
            }
            catch
            {
            }
            return retval;
        }
        public static long StringToLong(this String strVal, int defaultValue)
        {
            long retval = defaultValue;
            try
            {
                if (!string.IsNullOrEmpty(strVal))
                    retval = Convert.ToInt64(strVal);
            }
            catch
            {
            }
            return retval;
        }
        public static int? StringToNullableInt(this String strVal)
        {
            int? retval = null;
            try
            {
                if (!string.IsNullOrEmpty(strVal))
                    retval = Convert.ToInt32(strVal);
            }
            catch
            {
            }
            return retval;
        }
        public static string TruncateStringCharactersUpTo(this string str, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", "Length must be >= 0");
            }

            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            int maxLength = Math.Min(str.Length, length);
            return str.Substring(0, maxLength);
        }
        public static double RoundDouble(this double ipDoubleValue, char type)
        {
            int noOfDigits = 0;
            if (type == 'P')
            {
                noOfDigits = 4;

            }
            else if (type == 'A')
            {
                noOfDigits = 2;


            }

            return Math.Round(ipDoubleValue, noOfDigits, MidpointRounding.ToEven);
        }
        public static DateTime MMDDYYStringToDateTime(this string dateString, string formats)
        {
            return DateTime.ParseExact(dateString, formats,
                                             new CultureInfo("en-US"),
                                             DateTimeStyles.None);
        }
        public static DateTime stringToCultureInfoDateTime(this string dateString)
        {

            return Convert.ToDateTime(dateString.Replace("/", "-"), CultureInfo.InvariantCulture);
        }
        public static string DateTimeToMMDDYYYYString(this DateTime dt)
        {
            return String.Format("{0:MM-dd-yyyy}", dt).Replace('/', '-');
        }
        public static string DateTimeToMMDDYYYYString(this DateTime? dt)
        {
            return dt != null ? String.Format("{0:MM-dd-yyyy}", dt).Replace('/', '-') : string.Empty;
        }
        public static string DateTimeToMMDDYYYYStringWithHours(this DateTime dt)
        {
            return String.Format("{0:MM-dd-yyyy hh:mm:ss}", dt).Replace('/', '-');
        }
        public static string DateTimeToMMDDYYYYStringWith24Hours(this DateTime dt)
        {
            return String.Format("{0:MM-dd-yyyy HH:mm:ss}", dt).Replace('/', '-');
        }
        public static string DateTimeToDDMMYYYYString(this DateTime dt)
        {
            return String.Format("{0:dd-MM-yyyy}", dt).Replace('/', '-');
        }
        public static string DateTimeToDDMMYYYYStringWithHours(this DateTime dt)
        {
            return String.Format("{0:dd-MM-yyyy hh:mm:ss}", dt).Replace('/', '-');
        }
        public static string DateTimeToDDMMYYYYStringWith24Hours(this DateTime dt)
        {
            return String.Format("{0:dd-MM-yyyy HH:mm:ss}", dt).Replace('/', '-');
        }
        public static string ToMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }
        public static string ToShortMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }
        public static string ReplaceIfAny(this String str, string rep, [Optional]string newString)
        {
            if (str.Contains(rep))
            {
                if (string.IsNullOrEmpty(newString))
                    return str.Replace(rep, string.Empty);
                else
                    return str.Replace(rep, newString);
            }
            else
                return str;
        }
        /// <summary>
        /// Converts String to a title case
        /// </summary>
        /// <param name="str">cHeCk mArk</param>
        /// <returns>Check Mark</returns>
        public static string ToProperCase(this String str)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(str);
        }
        /// <summary>
        /// This extension allows to convert an anonymous type object into a specified type. If you have an object of an anonymous type and want to covert it to "Client" type, you need to call this extension:
        /// object obj=getSomeObjectOfAnonymoustype();
        /// Client client = obj.ToType(typeof(Client));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToType<T>(this object obj, T type)
        {
            //create instance of T type object:
            var tmp = Activator.CreateInstance(System.Type.GetType(type.ToString()));

            //loop through the properties of the object you want to covert:          
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                try
                {
                    //get the value of property and try 
                    //to assign it to the property of T type object:
                    tmp.GetType().GetProperty(pi.Name).SetValue(tmp,
                                              pi.GetValue(obj, null), null);
                }
                catch { }
            }

            //return the T type object:         
            return tmp;
        }
        /// <summary>
        /// This extension is to transform a List of Anonymous type objects into a List of a specific type objects:
        /// Calling ToNonAnonymousList(typeof(Client)) converts the List of anonymous type to a List of Client type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object ToNonAnonymousList<T>(this List<T> list, System.Type t)
        {

            //define system Type representing List of objects of T type:
            var genericType = typeof(List<>).MakeGenericType(t);

            //create an object instance of defined type:
            var l = Activator.CreateInstance(genericType);

            //get method Add from from the list:
            MethodInfo addMethod = l.GetType().GetMethod("Add");

            //loop through the calling list:
            foreach (T item in list)
            {

                //convert each object of the list into T object 
                //by calling extension ToType<T>()
                //Add this object to newly created list:
                addMethod.Invoke(l, new object[] { item.ToType(t) });
            }

            //return List of T objects:
            return l;
        }
        /// <summary>
        /// Used for getting errors from ModelStateDictionary
        /// </summary>
        /// <param name="modelState">modelState</param>
        /// <returns></returns>
        public static object GetErrorsObj(this ModelStateDictionary modelState)
        {
            var errorList = modelState.Where(o => o.Value.Errors.Count > 0)
                                .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());

            return new { Errors = errorList };
        }
        /// <summary>
        /// Converst this instance to delimited text.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="trimTrailingNewLineIfExists">
        /// If set to <c>true</c> then trim trailing new line if it exists.
        /// </param>
        /// <returns></returns>
        public static string ToDelimitedText<T>(this List<T> instance,
            string delimiter,
            bool trimTrailingNewLineIfExists = false)
            where T : class, new()
        {
            int itemCount = instance.Count;
            if (itemCount == 0) return string.Empty;

            var properties = GetPropertiesOfType<T>();
            int propertyCount = properties.Length;
            var outputBuilder = new StringBuilder();

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                T listItem = instance[itemIndex];
                AppendListItemToOutputBuilder(outputBuilder, listItem, properties, propertyCount, delimiter);

                AddNewLineIfRequired(trimTrailingNewLineIfExists, itemIndex, itemCount, outputBuilder);
            }

            var output = TrimTrailingNewLineIfExistsAndRequired(outputBuilder.ToString(), trimTrailingNewLineIfExists);
            return output;
        }
        private static void AddDelimiterIfRequired(StringBuilder outputBuilder, int propertyCount, string delimiter,
            int propertyIndex)
        {
            bool isLastProperty = (propertyIndex + 1 == propertyCount);
            if (!isLastProperty)
            {
                outputBuilder.Append(delimiter);
            }
        }
        private static void AddNewLineIfRequired(bool trimTrailingNewLineIfExists, int itemIndex, int itemCount,
            StringBuilder outputBuilder)
        {
            bool isLastItem = (itemIndex + 1 == itemCount);
            if (!isLastItem || !trimTrailingNewLineIfExists)
            {
                outputBuilder.Append(Environment.NewLine);
            }
        }
        private static void AppendListItemToOutputBuilder<T>(StringBuilder outputBuilder,
            T listItem,
            PropertyInfo[] properties,
            int propertyCount,
            string delimiter)
            where T : class, new()
        {

            for (int propertyIndex = 0; propertyIndex < properties.Length; propertyIndex += 1)
            {
                var property = properties[propertyIndex];
                var propertyValue = property.GetValue(listItem);
                outputBuilder.Append(propertyValue);

                AddDelimiterIfRequired(outputBuilder, propertyCount, delimiter, propertyIndex);
            }
        }
        private static PropertyInfo[] GetPropertiesOfType<T>() where T : class, new()
        {
            System.Type itemType = typeof(T);
            var properties = itemType.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
            return properties;
        }
        private static string TrimTrailingNewLineIfExistsAndRequired(string output, bool trimTrailingNewLineIfExists)
        {
            if (!trimTrailingNewLineIfExists || !output.EndsWith(Environment.NewLine)) return output;

            int outputLength = output.Length;
            int newLineLength = Environment.NewLine.Length;
            int startIndex = outputLength - newLineLength;
            output = output.Substring(startIndex, newLineLength);
            return output;
        }
        public static int GetTotalPageCountForGrid(this int totalcount, int? pageSize)
        {
            if (totalcount == 0)
                totalcount = 0;
            else
                totalcount = totalcount < pageSize ? 1 : (int)Math.Ceiling((float)(totalcount / pageSize));
            return totalcount;
        }
        public static bool ContainsSpecialCharacters(this string value)
        {
            bool isAvailable = false;
            try
            {
                if (value.Contains("\'") || value.Contains("\"") || value.Contains("“") || value.Contains("”") || value.Contains("\"") || value.Contains("‘") || value.Contains("’"))
                {
                    return isAvailable = true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return isAvailable;
        }
        public static string ReplaceUnicodeWithSpecialCharacter(this string value)
        {
            string formatedValue = string.Empty;
            try
            {
                foreach (var Character in value)
                {
                    switch (Character)
                    {
                        case '\'':
                            formatedValue += String.Format("\\U{0:X4}", (int)Character).ToLower();
                            break;
                        case '\"':
                            formatedValue += String.Format("\\U{0:X4}", (int)Character).ToLower();
                            break;
                        case '“':
                            formatedValue += String.Format("\\U{0:X4}", (int)Character).ToLower();
                            break;
                        case '”':
                            formatedValue += String.Format("\\U{0:X4}", (int)Character).ToLower();
                            break;
                        case '‘':
                            formatedValue += String.Format("\\U{0:X4}", (int)Character).ToLower();
                            break;
                        case '’':
                            formatedValue += String.Format("\\U{0:X4}", (int)Character).ToLower();
                            break;
                        default:
                            formatedValue += Character;
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return formatedValue;
        }
        public static string DoubleToStringIfNotNull(this double value, int decimals = 2)
        {
            string retval = (0).ToString("0.00");
            try
            {
                switch (decimals)
                {
                    case 2:
                        retval = value.ToString("0.00");
                        break;
                    case 3:
                        retval = value.ToString("0.000");
                        break;
                    case 4:
                        retval = value.ToString("0.0000");
                        break;
                    default:
                        retval = value.ToString("0.00");
                        break;
                }
            }
            catch (Exception ex)
            {
                return retval;
            }
            return retval;
        }
        public static string NullableDoubleToStringIfNull(this double? value, int decimals = 2)
        {
            string retval = (0).ToString("0.00");
            try
            {
                switch (decimals)
                {
                    case 2:
                        retval = Convert.ToDouble(value).ToString("0.00");
                        break;
                    case 3:
                        retval = Convert.ToDouble(value).ToString("0.000");
                        break;
                    case 4:
                        retval = Convert.ToDouble(value).ToString("0.0000");
                        break;
                    default:
                        retval = Convert.ToDouble(value).ToString("0.00");
                        break;
                }
            }
            catch (Exception ex)
            {
                return retval;
            }
            return retval;
        }
        public static string StringToStringDecimal(this string val, int decimals = 2)
        {
            string retval = (0).ToString("0.00");
            double value = 0.00;
            try
            {
                if (!string.IsNullOrEmpty(val))
                {
                    double.TryParse(val, out value);
                    switch (decimals)
                    {
                        case 2:
                            retval = value.ToString("0.00");
                            break;
                        case 3:
                            retval = value.ToString("0.000");
                            break;
                        case 4:
                            retval = value.ToString("0.0000");
                            break;
                        default:
                            retval = value.ToString("0.00");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                return retval;
            }
            return retval;
        }
        public static string ReplaceSpacesWithPlusForQueryStrings(this string val)
        {
            string retval = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(val))
                {
                    retval = Regex.Replace(val, @"\s", "+");
                }
            }
            catch (Exception e)
            {
                return retval;
            }
            return retval;
        }
        public static string RemoveSpaces(this string val)
        {
            string retval = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(val))
                {
                    return val.Replace(" ", string.Empty);
                }
            }
            catch (Exception e)
            {
                return retval;
            }
            return retval;

        }
        public static string RemoveSpecialCharacters(this string val)
        {
            string retval = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(val))
                {
                    return Regex.Replace(val, @"[^0-9a-zA-Z]+", string.Empty);
                }
            }
            catch (Exception e)
            {
                return retval;
            }
            return retval;

        }
        public static string[] getSeperatedValuesInStringArray(this string val,char seperator)
        {
            string[] strValues = null;
            try
            {
                if(!string.IsNullOrEmpty(val))
                {
                    strValues = val.Split(seperator);
                }
            }
            catch (Exception ex)
            {}
            return strValues;
        }
        public static List<string> getDataFromDTtoListString(this DataTable dt,string column)
        {
            List<string> str = new List<string>();
            try
            {
                if(dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        str.Add(dr[column].ConvertObjectToStringIfNotNull());
                    }
                }
            }
            catch (Exception ex)
            {}
            return str;
        }
        public static long ConvertObjectToLongIfNotNull(this object obj)
        {
            long retval = 0;
            try
            {
                if (obj != null)
                {
                    var stringObject = obj as String;
                    if (stringObject != null)
                        retval = Convert.ToInt64(obj);
                }
            }
            catch (Exception e)
            {
                return retval;
            }
            return retval;
        }
        public static int objectToInt(this object strVal, int defaultValue)
        {
            int retval = defaultValue;
            try
            {
                if (strVal != null)
                    retval = Convert.ToInt32(strVal);
            }
            catch
            {
            }
            return retval;
        }
        public static string insertTrace(this Exception strVal, string defaultValue)
        {
            Traces traces = new Traces();
            string retval = defaultValue;
            try
            {
                if (strVal != null)
                {
                    retval = strVal.StackTrace + Environment.NewLine + "Error: " + strVal.Message;
                    Trace trace = new Trace();
                    trace.Exception = strVal.StackTrace;
                    trace.Error = strVal.Message;
                    traces.InsertTrace(trace);
                }
            }
            catch
            {
            }
            return retval;
        }
        public static decimal ConvertObjectToDecimal(this object obj)
        {
            decimal retval = 0;
            try
            {
                if (obj != null)
                {

                    retval = System.Math.Round(Convert.ToDecimal(obj),2);
                }
            }
            catch (Exception e)
            {
                return retval;
            }
            return retval;
        }
    }
}