using System;
using System.Collections.Generic;
using System.Text;
using IFC4;

namespace STEPExtensions
{
    /// <summary>
    /// An extention to write lists to STEP
    /// </summary>
    public static class ListExtensions
    {
        public static string STEPValue(this IEnumerable<BaseIfc> baseIfcs, ref Dictionary<Guid, int> indexDictionnary)
        {
            List<string> values = new List<string>();
            foreach (BaseIfc baseIfc in baseIfcs)
            {
                values.Add(baseIfc != null ? baseIfc.STEPValue(ref indexDictionnary) : "$");
            }
            if (values.Count == 0) return "$";
            return "(" + string.Join(", ", values.ToArray()) + ")";
        }
    }

    /// <summary>
    /// An extention to write interger lists to STEP
    /// </summary>
    public static class ListIntExtensions
    {
        public static string STEPValue(this IEnumerable<int> baseIfcs, ref Dictionary<Guid, int> indexDictionnary)
        {
            List<string> values = new List<string>();
            foreach (int baseIfc in baseIfcs)
            {
                values.Add(baseIfc.STEPValue(ref indexDictionnary));
            }
            if (values.Count == 0) return "$";
            return "(" + string.Join(", ", values.ToArray()) + ")";
        }
    }

    /// <summary>
    /// An extention to write double lists to STEP
    /// </summary>
    public static class ListDoubleExtensions
    {
        public static string STEPValue(this IEnumerable<double> baseIfcs, ref Dictionary<Guid, int> indexDictionnary)
        {
            List<string> values = new List<string>();
            foreach (int baseIfc in baseIfcs)
            {
                values.Add(baseIfc.STEPValue(ref indexDictionnary));
            }
            if (values.Count == 0) return "$";
            return "(" + string.Join(", ", values.ToArray()) + ")";
        }
    }

    /// <summary>
    /// An extention to write byte[] lists to STEP
    /// </summary>
    public static class ListBytesExtensions
    {
        public static string STEPValue(this IEnumerable<byte[]> baseIfcs, ref Dictionary<Guid, int> indexDictionnary)
        {
            List<string> values = new List<string>();
            foreach (byte[] baseIfc in baseIfcs)
            {
                values.Add(baseIfc.STEPValue(ref indexDictionnary));
            }
            if (values.Count == 0) return "$";
            return "(" + string.Join(", ", values.ToArray()) + ")";
        }
    }

    /// <summary>
    /// An extention to write embedded lists to STEP
    /// </summary>
    public static class ListsExtensions
    {
        public static string STEPValue(this IEnumerable<IEnumerable<BaseIfc>> baseIfcs, ref Dictionary<Guid, int> indexDictionnary)
        {
            List<string> values = new List<string>();
            foreach (IEnumerable<BaseIfc> baseIfcList in baseIfcs)
            {
                List<string> subValues = new List<string>();
                foreach (BaseIfc baseIfc in baseIfcList)
                {
                    subValues.Add(baseIfc.STEPValue(ref indexDictionnary));
                }
                values.Add("(" + string.Join(", ", subValues.ToArray()) + ")");
                subValues.Clear();
            }
            if (values.Count == 0) return "$";
            return "(" + string.Join(", ", values.ToArray()) + ")";
        }
    }


    /// <summary>
    /// An extention to write embedded int lists to STEP
    /// </summary>
    public static class ListsIntExtensions
    {
        public static string STEPValue(this IEnumerable<IEnumerable<int>> baseIfcs, ref Dictionary<Guid, int> indexDictionnary)
        {
            List<string> values = new List<string>();
            foreach (IEnumerable<int> baseIfcList in baseIfcs)
            {
                List<string> subValues = new List<string>();
                foreach (int baseIfc in baseIfcList)
                {
                    subValues.Add(baseIfc.STEPValue(ref indexDictionnary));
                }
                values.Add("(" + string.Join(", ", subValues.ToArray()) + ")");
                subValues.Clear();
            }
            if (values.Count == 0) return "$";
            return "(" + string.Join(", ", values.ToArray()) + ")";
        }
    }

    /// <summary>
    /// An extention to write embedded double lists to STEP
    /// </summary>
    public static class ListsDoubleExtensions
    {
        public static string STEPValue(this IEnumerable<IEnumerable<double>> baseIfcs, ref Dictionary<Guid, int> indexDictionnary)
        {
            List<string> values = new List<string>();
            foreach (IEnumerable<double> baseIfcList in baseIfcs)
            {
                List<string> subValues = new List<string>();
                foreach (double baseIfc in baseIfcList)
                {
                    subValues.Add(baseIfc.STEPValue(ref indexDictionnary));
                }
                values.Add("(" + string.Join(", ", subValues.ToArray()) + ")");
                subValues.Clear();
            }
            if (values.Count == 0) return "$";
            return "(" + string.Join(", ", values.ToArray()) + ")";
        }
    }

    /// <summary>
    /// An extention to write bool to STEP
    /// </summary>
    public static class BoolExtensions
    {
        public static string STEPValue(this bool value, ref Dictionary<Guid, int> indexDictionnary)
        {
            if (value)
            {
                return ".TRUE.";
            }
            else
            {
                return ".FALSE.";
            }
        }
    }

    /// <summary>
    /// An extention to write nullable bool to STEP
    /// </summary>
    public static class BoolNullableExtensions
    {
        public static string STEPValue(this bool? value, ref Dictionary<Guid, int> indexDictionnary)
        {
            if (value == null)
            {
                return "$";
            }
            else if (value == true)
            {
                return ".TRUE.";
            }
            else
            {
                return ".FALSE.";
            }
        }
    }
    /// <summary>
    /// An extention to write int to STEP
    /// </summary>
    public static class IntExtensions
    {
        public static string STEPValue(this int value, ref Dictionary<Guid, int> indexDictionnary)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// An extention to write string to STEP
    /// </summary>
    public static class StringExtensions
    {
        public static string STEPValue(this string value, ref Dictionary<Guid, int> indexDictionnary)
        {
            return "'" + value.ToString() + "'";
        }
    }

    /// <summary>
    /// An extention to write byte to STEP
    /// </summary>
    public static class byteExtensions
    {
        public static string STEPValue(this byte[] value, ref Dictionary<Guid, int> indexDictionnary)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// An extention to write double to STEP
    /// </summary>
    public static class DoubleExtensions
    {
        public static string STEPValue(this double value, ref Dictionary<Guid, int> indexDictionnary)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// An extention to write enum to STEP
    /// </summary>
    public static class EnumExtensions
    {
        public static string STEPValue(this Enum value, ref Dictionary<Guid, int> indexDictionnary)
        {
            return "." + value.ToString() + ".";
        }
    }

}
