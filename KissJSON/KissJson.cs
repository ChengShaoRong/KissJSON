/*
 *           C#Like
 * KissJson : Keep It Simple Stupid JSON
 * Copyright © 2022-2025 RongRong. All right reserved.
 */
using System.Text;
using System.Collections.Generic;
using System;
#if _CSHARP_LIKE_
using CSharpLike.Internal;
#endif
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace CSharpLike
{
    /// <summary>
    /// Mark as don't serialize by KISS JSON
    /// </summary>
    public sealed class KissJsonDontSerialize : Attribute
    {
    }
    /// <summary>
    /// Mark as serialize Property  by KISS JSON
    /// </summary>
    public sealed class KissJsonSerializeProperty : Attribute
    {
    }
    /// <summary>
    /// Keep It Simple Stupid JSON.
    /// Convert to each other between JSONData and JSON string and class/struct object.
    /// Why I have to this JSON library but not the LitJson/NewtonJson/...?
    /// Because KissJson is the unique one which SUPPORT the class in hot update script!
    /// </summary>
    public sealed class KissJson
    {
        /// <summary>
        /// The FormatProvider for 'Convert.ToSingle' and 'Convert.ToDouble'. Default is 'CultureInfo.InvariantCulture'.
        /// </summary>
        public static CultureInfo CultureForConvertFloatAndDouble { get; set; } = CultureInfo.InvariantCulture;
        /// <summary>
        /// The FormatProvider for 'Convert.ToDateTime'. Default is 'CultureInfo.InvariantCulture'.
        /// You should set this to fit your DateTime format in your JSON string.
        /// </summary>
        public static CultureInfo CultureForConvertDateTime { get; set; } = CultureInfo.InvariantCulture;
        /// <summary>
        /// ignore null
        /// </summary>
        public static bool ignoreNull
        {
            get { return KISSJsonImp.ignoreNull; }
            set { KISSJsonImp.ignoreNull = value; }
        }
        /// <summary>
        /// Convert JSONData to binary JSON data.
        /// Loading binary JSON file is abuot 20 times faster than string JSON file.
        /// And the size is very smaller too, only about 5% percent of the string JSON file.
        /// The binary JSON file weakness is unreadable.
        /// </summary>
        /// <param name="value">The JSON need to be convert</param>
        /// <param name="bCompress">Whether compress data. Default value is true</param>
        /// <returns></returns>
        public static byte[] ToBinaryData(JSONData value, bool bCompress = true)
        {
            if (value == null)
            {
                if (JSONData.ThrowException) throw new Exception("JSONData null, can't convert to byte[]");
                return default;
            }
            CSL_Stream stream = new CSL_Stream();
            Array.Copy(mHeader, 0, stream.buff, 0, mHeader.Length);
            stream.pos = mHeader.Length;
            stream.Write(bCompress);
            JSONData.ToBinaryData(value, stream);
            if (bCompress)
            {
                int length = mHeader.Length + 1;
                byte[] compressBuff = Compress(stream.buff, length, stream.pos - length);
                byte[] finalBuff = new byte[compressBuff.Length + length];
                Array.Copy(stream.buff, finalBuff, length);
                Array.Copy(compressBuff, 0, finalBuff, length, compressBuff.Length);
                return finalBuff;
            }
            return stream.GetBuff();
        }
        /// <summary>
        /// Convert binary/string JSON data to JSONData object.
        /// Convert binary JSON file is abuot 20 times faster than string JSON file, and only has 5% size of the string JSON file;
        /// Usage:
        /// e.g.
        ///  JSONData json = JSONData.ToJSONData(File.ReadAllBytes("test.json"));
        ///  
        /// e.g.
        /// using (UnityWebRequest uwr = UnityWebRequest.Get(configJsonUrl))
        /// {
        ///     yield return uwr.SendWebRequest();
        ///     if (string.IsNullOrEmpty(uwr.error))
        ///         JSONData json = KissJson.ToJSONData(uwr.downloadHandler.data);
        /// }
        /// </summary>
        /// <param name="value">Binary JSON data or normal string JSON data. Normal string JSON data MUST using UTF8 file format, no matter with bom or not.</param>
        /// <returns>JSONData object</returns>
        public static JSONData ToJSONData(byte[] value)
        {
            if (value == null || value.Length == 0)
                return null;
            bool bNotMatch = mHeader.Length > value.Length;
            if (!bNotMatch)
            {
                for (int i = 0; i < mHeader.Length && i < value.Length; i++)
                {
                    if (mHeader[i] != value[i])
                    {
                        bNotMatch = true;
                        break;
                    }
                }
            }
            if (bNotMatch)//Is normal JSON string
            {
                if (value.Length >= 3
                    && value[0] == 0xef
                    && value[1] == 0xbb
                    && value[2] == 0xbf)
                    return ToJSONData((new UTF8Encoding(false)).GetString(value, 3, value.Length - 3));//With UTF8 bom
                else
                    return ToJSONData(Encoding.UTF8.GetString(value));//No UTF8 bom
            }
            else//Is binary JSON
            {
                JSONData data = new JSONData();
                CSL_Stream stream = new CSL_Stream(value);
                stream.pos = mHeader.Length;
                stream.Read(out bool bCompress);
                if (bCompress)
                {
                    stream.buff = Decompress(stream.buff, stream.pos, stream.buff.Length - stream.pos);
                    stream.pos = 0;
                    stream.maxSize = stream.buff.Length;
                }
                JSONData.ToJSONData(data, stream);
                return data;
            }
        }
        class _ExcelData_
        {
            Dictionary<string, object> cache = new Dictionary<string, object>();
            JSONData table;
            Type type;
#if _CSHARP_LIKE_
            SType stype;
            public _ExcelData_(SType type, JSONData json) : this(json)
            {
                stype = type;
            }
#endif
            public object Get(string strUniqueKey)
            {
                if (cache.TryGetValue(strUniqueKey, out object value))
                    return value;
                if (table != null && table.TryGetValue(strUniqueKey, out JSONData json))
                {
                    table.RemoveKey(strUniqueKey);
                    JSONData jsonDic = JSONData.NewDictionary();
                    for (int i = 0, max = Math.Min(Headers.Count, json.Count); i < max; i++)
                        jsonDic[Headers[i]] = json[i];
                    if (type != null)
                        value = ToObject(type, jsonDic);
#if _CSHARP_LIKE_
                    else if (stype != null)
                        value = ToObject(stype, jsonDic);
#endif
                }
                cache[strUniqueKey] = value;
                return value;
            }
            public JSONData GetJSON(string strUniqueKey, string strColumnName)
            {
                if (table != null
                    && table.TryGetValue(strUniqueKey, out JSONData json))
                {
                    int index = Headers.IndexOf(strColumnName);
                    if (index >= 0 && index < json.Count)
                        return json[index];
                }
                return null;
            }
            public List<string> Headers { get; set; }
            public List<string> Keys { get; set; }
            public _ExcelData_(Type type, JSONData json) : this(json)
            {
                this.type = type;
            }
            public _ExcelData_(JSONData json)
            {
                table = json["table"];
                Headers = json["headers"];
                Keys = new List<string>();
                foreach (string str in (table.Value as Dictionary<string, JSONData>).Keys)
                    Keys.Add(str);
            }
        }
        static Dictionary<string, _ExcelData_> excelDatas = new Dictionary<string, _ExcelData_>();
        /// <summary>
        /// Clear Excel data by Type
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        public static void Clear(Type type)
        {
            if (type == null) excelDatas.Clear();
            else excelDatas.Remove(type.Name);
        }
        /// <summary>
        /// Clear Excel data by file name.
        /// Only for load by file name.
        /// </summary>
        /// <param name="fileName">file name of Excel</param>
        public static void Clear(string fileName)
        {
            if (fileName == null) excelDatas.Clear();
            else excelDatas.Remove(fileName);
        }
#if _CSHARP_LIKE_
        /// <summary>
        /// Clear Excel data by SType
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        public static void Clear(SType type)
        {
            if (type == null) excelDatas.Clear();
            else excelDatas.Remove(type.Name);
        }
        /// <summary>
        /// Get Excel data by unique key
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        /// <param name="strUniqueKey">Unique key of the Excel data</param>
        /// <returns>Excel data</returns>
        public static object Get(SType type, string strUniqueKey) => excelDatas.TryGetValue(type.Name, out _ExcelData_ excel) ? excel.Get(strUniqueKey) : null;
        /// <summary>
        /// Get Excel data keys.
        /// It's not sorted keys, just the order in your excel file.
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        /// <returns>Keys that not sorted, just the order in your excel file.</returns>
        public static List<string> GetKeys(SType type) => excelDatas.TryGetValue(type.Name, out _ExcelData_ excel) ? excel.Keys : new List<string>();
        /// <summary>
        /// Get Excel data headers (column names).
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        /// <returns>Headers</returns>
        public static List<string> GetHeaders(SType type) => excelDatas.TryGetValue(type.Name, out _ExcelData_ excel) ? excel.Headers : new List<string>();
        /// <summary>
        /// Load Excel data
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        /// <param name="json">Source KissJSON object</param>
        public static void Load(SType type, JSONData json) => excelDatas[type.Name] = new _ExcelData_(type, json);
        /// <summary>
        /// Load Excel data by file name.
        /// That for mothed `GetJSON`.
        /// </summary>
        /// <param name="fileName">File name of Excel data</param>
        /// <param name="json">Source KissJSON object</param>
        public static void Load(string fileName, JSONData json) => excelDatas[fileName] = new _ExcelData_(json);
#endif
        /// <summary>
        /// Get Excel data by unique key
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        /// <param name="strUniqueKey">Unique key of the Excel data</param>
        /// <returns>Excel data</returns>
        public static object Get(Type type, string strUniqueKey) => excelDatas.TryGetValue(type.Name, out _ExcelData_ excel) ? excel.Get(strUniqueKey) : null;
        /// <summary>
        /// Get Excel data keys.
        /// It's not sorted keys, just the order in your excel file..
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        /// <returns>Keys that not sorted, just the order in your excel file.</returns>
        public static List<string> GetKeys(Type type) => excelDatas.TryGetValue(type.Name, out _ExcelData_ excel) ? excel.Keys : new List<string>();
        /// <summary>
        /// Get Excel data headers (column names).
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        /// <returns>Headers</returns>
        public static List<string> GetHeaders(Type type) => excelDatas.TryGetValue(type.Name, out _ExcelData_ excel) ? excel.Headers : new List<string>();
        /// <summary>
        /// Get JSON data of the Excel data by unique key and column name.
        /// Use this must call `public static void Load(string fileName, JSONData json)` once.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="strUniqueKey"></param>
        /// <param name="strColumnName"></param>
        /// <returns>JSON data</returns>
        public static JSONData GetJSON(string fileName, string strUniqueKey, string strColumnName) => excelDatas.TryGetValue(fileName, out _ExcelData_ excel) ? excel.GetJSON(strUniqueKey, strColumnName) : null;
        /// <summary>
        /// Load Excel data
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        /// <param name="json">Source KissJSON object</param>
        public static void Load(Type type, JSONData json) => excelDatas[type.Name] = new _ExcelData_(type, json);
        /// <summary>
        /// Load Excel data
        /// </summary>
        /// <param name="type">Type of Excel data, it's for hot update script</param>
        /// <param name="json">Source KissJSON object</param>
        public static void Load(object type, JSONData json)
        {
            if (type == null) return;
#if _CSHARP_LIKE_
            if (type is SType) { Load(type as SType, json); return; }
#endif
            Load(type as Type, json);
        }
        /// <summary>
        /// Synchronizing load all Excel data by Type.
        /// </summary>
        /// <param name="type">Type of your data. e.g. typeof(ItemJSON)</param>
        /// <param name="fileName">File name in AssetBundle</param>
        public static void Load(object type, string fileName)
        {
            byte[] buff = File.ReadAllBytes(fileName);
            if (buff != null)
            {
                if (type is string)
                    Load(type as string, ToJSONData(buff));
                else
                    Load(type, ToJSONData(buff));
            }
        }
        /// <summary>
        /// Get Excel data by unique key
        /// </summary>
        /// <param name="type">Type of Excel data, it's for hot update script</param>
        /// <param name="strUniqueKey">Unique key of the Excel data</param>
        /// <returns>Excel data</returns>
        public static object Get(object type, string strUniqueKey)
        {
            if (type == null) return null;
#if _CSHARP_LIKE_
            if (type is SType) return Get(type as SType, strUniqueKey);
#endif
            return Get(type as Type, strUniqueKey);
        }
        /// <summary>
        /// Clear all Excel data
        /// </summary>
        public static void ClearAll() => excelDatas.Clear();
        /// <summary>
        /// Clear Excel data
        /// </summary>
        /// <param name="type">Type of Excel data, it's for hot update script</param>
        public static void Clear(object type)
        {
            if (type == null) { excelDatas.Clear(); return; }
#if _CSHARP_LIKE_
            if (type is SType) { Clear(type as SType); return; }
#endif
            Clear(type as Type);
        }
        /// <summary>
        /// Get Excel data keys.
        /// It's not sorted keys, just the order in your excel file.
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        /// <returns>Keys that not sorted, just the order in your excel file.</returns>
        public static List<string> GetKeys(object type)
        {
            if (type == null) return new List<string>();
#if _CSHARP_LIKE_
            if (type is SType) return GetKeys(type as SType);
#endif
            return GetKeys(type as Type);
        }
        /// <summary>
        /// Get Excel data headers (column names).
        /// </summary>
        /// <param name="type">Type of Excel data</param>
        /// <returns>Headers</returns>
        public static List<string> GetHeaders(object type)
        {
            if (type == null) return new List<string>();
#if _CSHARP_LIKE_
            if (type is SType) return GetKeys(type as SType);
#endif
            return GetKeys(type as Type);
        }
        /// <summary>
        /// Compress byte array using GZipStream
        /// </summary>
        /// <param name="buff">The source array.</param>
        /// <param name="offset">The offset at which the source array begins.</param>
        /// <param name="count">The length of the source array from that offset.</param>
        /// <returns>Compressed byte array</returns>
        public static byte[] Compress(byte[] buff, int offset = 0, int count = 0)
        {
            if (count == 0)
                count = buff.Length - offset;
            //Compress with zip
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(buff, offset, count);
                    zipStream.Close();
                    return outStream.ToArray();
                }
            }
        }
        /// <summary>
        /// Decompress byte array using GZipStream
        /// </summary>
        /// <param name="buff">The source array.</param>
        /// <param name="offset">The offset at which the source array begins.</param>
        /// <param name="count">The length of the source array from that offset.</param>
        /// <returns>Decompressed byte array</returns>
        public static byte[] Decompress(byte[] buff, int offset = 0, int count = 0)
        {
            if (count == 0)
                count = buff.Length - offset;
            //Decompress with zip
            using (MemoryStream inputStream = new MemoryStream(buff, offset, count))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }
            }
        }
        /// <summary>
        /// Convert object to JSONData
        /// </summary>
        /// <param name="obj">object want to be converted</param>
        public static JSONData ToJSONData(object obj)
        {
            return ToJSONData(ToJson(obj));
        }
#if _CSHARP_LIKE_
        public static JSONData ToJSONData(SInstance obj)
        {
            return ToJSONData(ToJson(obj));
        }
#endif
        /// <summary>
        /// an object convert to JSON string
        /// </summary>
        public static string ToJson(object obj)
        {
            if (obj == null)
                return "{}";
            Type type = obj.GetType();
#if _CSHARP_LIKE_
            if (type == typeof(SInstance))
                return ToJson(obj as SInstance);
#endif
            if (type.IsEnum)
                return ((IConvertible)((Enum)obj)).ToInt64(null).ToString();
            else if (type.IsPrimitive)
                return obj.ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            var fs = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var f in fs)
            {
                if (f.Name.EndsWith("__BackingField") || Attribute.IsDefined(f, typeof(KissJsonDontSerialize)))
                    continue;
                object value = f.GetValue(obj);
                if (value == null)
                {
                    if (!ignoreNull)
                    {
                        Type t = f.FieldType;
#if _CSHARP_LIKE_
                        if (t == typeof(SInstance))
                        {
                            SType st = HotUpdateManager.vm.GetTypeByKeyword(t.FullName).type;
                            if (st.IsDefined("CSharpLike.KissJsonDontSerialize"))
                                continue;
                        }
                        else
                        {
#endif
                            if (t.IsDefined(typeof(KissJsonDontSerialize)))
                                continue;
#if _CSHARP_LIKE_
                        }
#endif
                        sb.AppendFormat("\"{0}\":null,", f.Name);
                    }
                    continue;
                }
                string valueTypeFullName = value.GetType().FullName;
                bool valueTypeNullable = RemoveNullable(ref valueTypeFullName);
                switch (valueTypeFullName)
                {
                    case "System.String":
                        if (value == null)
                        {
                            if (!ignoreNull)
                                sb.AppendFormat("\"{0}\":\"{1}\",", f.Name, value);
                        }
                        else
                            sb.AppendFormat("\"{0}\":\"{1}\",", f.Name, value);
                        break;
                    case "System.SByte":
                    case "System.UInt16":
                    case "System.UInt32":
                    case "System.UInt64":
                    case "System.Byte":
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Single":
                    case "System.Double":
                        if (valueTypeNullable)
                        {
                            if (value == null)
                                sb.AppendFormat("\"{0}\":null,", f.Name);
                            else
                                sb.AppendFormat("\"{0}\":{1},", f.Name, value);
                        }
                        else
                            sb.AppendFormat("\"{0}\":{1},", f.Name, value);
                        break;
                    case "System.Boolean":
                        if (valueTypeNullable)
                        {
                            if (value == null)
                                sb.AppendFormat("\"{0}\":null,", f.Name);
                            else
                                sb.AppendFormat("\"{0}\":{1},", f.Name, (bool)value ? "true" : "false");
                        }
                        else
                            sb.AppendFormat("\"{0}\":{1},", f.Name, (bool)value ? "true" : "false");
                        break;
                    case "System.DateTime":
                        if (valueTypeNullable)
                        {
                            
                            if (value == null)
                                sb.AppendFormat("\"{0}\":\"{1}\",", f.Name, (new DateTime(1970, 1, 1)).ToString(CultureForConvertDateTime));
                            else
                                sb.AppendFormat("\"{0}\":\"{1}\",", f.Name, ((DateTime)value).ToString(CultureForConvertDateTime));
                        }
                        else
                            sb.AppendFormat("\"{0}\":\"{1}\",", f.Name, ((DateTime)value).ToString(CultureForConvertDateTime));
                        break;
                    default:
                        if (valueTypeFullName.StartsWith("System.Collections.Generic.Dictionary`2[[System.String,"))
                        {
                            RemoveDictionary(ref valueTypeFullName);
                            bool bNullable = RemoveNullable(ref valueTypeFullName);
                            sb.AppendFormat("\"{0}\":", f.Name);
                            sb.Append("{");
                            IDictionary dic = value as IDictionary;
                            switch (valueTypeFullName)
                            {
                                case "System.Boolean":
                                    if (dic.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (DictionaryEntry item in dic)
                                            {
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value == null ? "null" : ((bool)item.Value ? "true" : "false"));
                                            }
                                        }
                                        else
                                        {
                                            foreach (DictionaryEntry item in dic)
                                            {
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, (bool)item.Value ? "true" : "false");
                                            }
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.DateTime":
                                    if (dic.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (DictionaryEntry item in dic)
                                            {
                                                sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, item.Value == null ? (new DateTime(1970, 1, 1)).ToString(CultureForConvertDateTime) : ((DateTime)item.Value).ToString(CultureForConvertDateTime));
                                            }
                                        }
                                        else
                                        {
                                            foreach (DictionaryEntry item in dic)
                                            {
                                                sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, item.Value == null ? (new DateTime(1970, 1, 1)).ToString(CultureForConvertDateTime) : ((DateTime)item.Value).ToString(CultureForConvertDateTime));
                                            }
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.String":
                                    if (dic.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (DictionaryEntry item in dic)
                                        {
                                            if (item.Value != null)
                                                sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, item.Value);
                                            else if (!ignoreNull)
                                                sb.AppendFormat("\"{0}\":null,", item.Key);
                                            else
                                                count++;
                                        }
                                        if (count != dic.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.SByte":
                                case "System.UInt16":
                                case "System.UInt32":
                                case "System.UInt64":
                                case "System.Byte":
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Single":
                                case "System.Double":
                                    if (dic.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (DictionaryEntry item in dic)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value == null ? "null" : item.Value);
                                        }
                                        else
                                        {
                                            foreach (DictionaryEntry item in dic)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value);
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                default:
                                    if (dic.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (DictionaryEntry item in dic)
                                        {
                                            if (item.Value != null)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, ToJson(item.Value));
                                            else if (!ignoreNull)
                                                sb.AppendFormat("\"{0}\":null,", item.Key);
                                            else
                                                count++;
                                        }
                                        if (count != dic.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                            }
                            sb.Append("},");
                        }
                        else if (valueTypeFullName.StartsWith("System.Collections.Generic.List`1["))
                        {
                            RemoveList(ref valueTypeFullName);
                            bool bNullable = RemoveNullable(ref valueTypeFullName);
                            sb.AppendFormat("\"{0}\":[", f.Name);
                            IList list = value as IList;
                            switch (valueTypeFullName)
                            {
                                case "System.String":
                                    if (list.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (var item in list)
                                        {
                                            if (item != null)
                                                sb.AppendFormat("\"{0}\",", item);
                                            else if (!ignoreNull)
                                                sb.Append("null,");
                                            else
                                                count++;
                                        }
                                        if (count != list.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.SByte":
                                case "System.UInt16":
                                case "System.UInt32":
                                case "System.UInt64":
                                case "System.Byte":
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Single":
                                case "System.Double":
                                    if (list.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (var item in list)
                                            {
                                                if (item == null)
                                                    sb.Append("null,");
                                                else
                                                    sb.Append(item + ",");
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in list)
                                                sb.Append(item + ",");
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.Boolean":
                                    if (list.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (var item in list)
                                            {
                                                if (item == null)
                                                    sb.Append("null,");
                                                else
                                                    sb.Append((bool)item ? "true," : "false,");
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in list)
                                                sb.Append((bool)item ? "true," : "false,");
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.DateTime":
                                    if (list.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (var item in list)
                                            {
                                                if (item == null)
                                                    sb.Append("\"" + (new DateTime(1970, 1, 1)).ToString(CultureForConvertDateTime) + "\",");
                                                else
                                                    sb.Append("\"" + ((DateTime)item).ToString(CultureForConvertDateTime) + "\",");
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in list)
                                                sb.Append("\"" + ((DateTime)item).ToString(CultureForConvertDateTime) + "\",");
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                default:
                                    if (list.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (var item in list)
                                        {
                                            if (item != null)
                                                sb.Append(ToJson(item) + ",");
                                            else if (!ignoreNull)
                                                sb.Append("null,");
                                            else
                                                count++;
                                        }
                                        if (count != list.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                            }
                            sb.Append("],");
                        }
                        else
                        {
                            if (valueTypeNullable && value == null)
                                sb.AppendFormat("\"{0}\":null,", f.Name);
                            else
                                sb.AppendFormat("\"{0}\":{1},", f.Name, ToJson(value));
                        }
                        break;
                }
            }
            var ps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            foreach (var f in ps)
            {
                if (!Attribute.IsDefined(f, typeof(KissJsonSerializeProperty)))
                    continue;
                object value = f.GetValue(obj);
                if (value == null)
                {
                    if (!ignoreNull)
                    {
                        Type t = f.PropertyType;
#if _CSHARP_LIKE_
                        if (t == typeof(SInstance))
                        {
                            SType st = HotUpdateManager.vm.GetTypeByKeyword(t.FullName).type;
                            if (st.IsDefined("CSharpLike.KissJsonDontSerialize"))
                                continue;
                        }
                        else
                        {
#endif
                        if (t.IsDefined(typeof(KissJsonDontSerialize)))
                            continue;
#if _CSHARP_LIKE_
                        }
#endif
                        sb.AppendFormat("\"{0}\":null,", f.Name);
                    }
                    continue;
                }
                string valueTypeFullName = value.GetType().FullName;
                bool valueTypeNullable = RemoveNullable(ref valueTypeFullName);
                switch (valueTypeFullName)
                {
                    case "System.String":
                        if (value == null)
                        {
                            if (!ignoreNull)
                                sb.AppendFormat("\"{0}\":\"{1}\",", f.Name, value);
                        }
                        else
                            sb.AppendFormat("\"{0}\":\"{1}\",", f.Name, value);
                        break;
                    case "System.SByte":
                    case "System.UInt16":
                    case "System.UInt32":
                    case "System.UInt64":
                    case "System.Byte":
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Single":
                    case "System.Double":
                        if (valueTypeNullable)
                        {
                            if (value == null)
                                sb.AppendFormat("\"{0}\":null,", f.Name);
                            else
                                sb.AppendFormat("\"{0}\":{1},", f.Name, value);
                        }
                        else
                            sb.AppendFormat("\"{0}\":{1},", f.Name, value);
                        break;
                    case "System.Boolean":
                        if (valueTypeNullable)
                        {
                            if (value == null)
                                sb.AppendFormat("\"{0}\":null,", f.Name);
                            else
                                sb.AppendFormat("\"{0}\":{1},", f.Name, (bool)value ? "true" : "false");
                        }
                        else
                            sb.AppendFormat("\"{0}\":{1},", f.Name, (bool)value ? "true" : "false");
                        break;
                    case "System.DateTime":
                        if (valueTypeNullable)
                        {
                            if (value == null)
                                sb.AppendFormat("\"{0}\":\"{1}\",", f.Name, (new DateTime(1970, 1, 1)).ToString(CultureForConvertDateTime));
                            else
                                sb.AppendFormat("\"{0}\":\"{1}\",", f.Name, ((DateTime)value).ToString(CultureForConvertDateTime));
                        }
                        else
                            sb.AppendFormat("\"{0}\":\"{1}\",", f.Name, ((DateTime)value).ToString(CultureForConvertDateTime));
                        break;
                    default:
                        if (valueTypeFullName.StartsWith("System.Collections.Generic.Dictionary`2[[System.String,"))
                        {
                            RemoveDictionary(ref valueTypeFullName);
                            bool bNullable = RemoveNullable(ref valueTypeFullName);
                            sb.AppendFormat("\"{0}\":", f.Name);
                            sb.Append("{");
                            IDictionary dic = value as IDictionary;
                            switch (valueTypeFullName)
                            {
                                case "System.Boolean":
                                    if (dic.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (DictionaryEntry item in dic)
                                            {
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value == null ? "null" : ((bool)item.Value ? "true" : "false"));
                                            }
                                        }
                                        else
                                        {
                                            foreach (DictionaryEntry item in dic)
                                            {
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, (bool)item.Value ? "true" : "false");
                                            }
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.DateTime":
                                    if (dic.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (DictionaryEntry item in dic)
                                            {
                                                sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, item.Value == null ? (new DateTime(1970, 1, 1)).ToString(CultureForConvertDateTime) : ((DateTime)item.Value).ToString(CultureForConvertDateTime));
                                            }
                                        }
                                        else
                                        {
                                            foreach (DictionaryEntry item in dic)
                                            {
                                                sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, ((DateTime)item.Value).ToString(CultureForConvertDateTime));
                                            }
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.String":
                                    if (dic.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (DictionaryEntry item in dic)
                                        {
                                            if (item.Value != null)
                                                sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, item.Value);
                                            else if (!ignoreNull)
                                                sb.AppendFormat("\"{0}\":null,", item.Key);
                                            else
                                                count++;
                                        }
                                        if (count != dic.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.SByte":
                                case "System.UInt16":
                                case "System.UInt32":
                                case "System.UInt64":
                                case "System.Byte":
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Single":
                                case "System.Double":
                                    if (dic.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (DictionaryEntry item in dic)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value == null ? "null" : item.Value);
                                        }
                                        else
                                        {
                                            foreach (DictionaryEntry item in dic)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value);
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                default:
                                    if (dic.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (DictionaryEntry item in dic)
                                        {
                                            if (item.Value != null)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, ToJson(item.Value));
                                            else if (!ignoreNull)
                                                sb.AppendFormat("\"{0}\":null,", item.Key);
                                            else
                                                count++;
                                        }
                                        if (count != dic.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                            }
                            sb.Append("},");
                        }
                        else if (valueTypeFullName.StartsWith("System.Collections.Generic.List`1["))
                        {
                            RemoveList(ref valueTypeFullName);
                            bool bNullable = RemoveNullable(ref valueTypeFullName);
                            sb.AppendFormat("\"{0}\":[", f.Name);
                            IList list = value as IList;
                            switch (valueTypeFullName)
                            {
                                case "System.String":
                                    if (list.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (var item in list)
                                        {
                                            if (item != null)
                                                sb.AppendFormat("\"{0}\",", item);
                                            else if (!ignoreNull)
                                                sb.Append("null,");
                                            else
                                                count++;
                                        }
                                        if (count != list.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.SByte":
                                case "System.UInt16":
                                case "System.UInt32":
                                case "System.UInt64":
                                case "System.Byte":
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Single":
                                case "System.Double":
                                    if (list.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (var item in list)
                                            {
                                                if (item == null)
                                                    sb.Append("null,");
                                                else
                                                    sb.Append(item + ",");
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in list)
                                                sb.Append(item + ",");
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.Boolean":
                                    if (list.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (var item in list)
                                            {
                                                if (item == null)
                                                    sb.Append("null,");
                                                else
                                                    sb.Append((bool)item ? "true," : "false,");
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in list)
                                                sb.Append((bool)item ? "true," : "false,");
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.DateTime":
                                    if (list.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (var item in list)
                                            {
                                                if (item == null)
                                                    sb.Append("\"" + (new DateTime(1970, 1, 1)).ToString(CultureForConvertDateTime) + "\",");
                                                else
                                                    sb.Append("\"" + ((DateTime)item).ToString(CultureForConvertDateTime) + "\",");
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in list)
                                                sb.Append("\"" + ((DateTime)item).ToString(CultureForConvertDateTime) + "\",");
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                default:
                                    if (list.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (var item in list)
                                        {
                                            if (item != null)
                                                sb.Append(ToJson(item) + ",");
                                            else if (!ignoreNull)
                                                sb.Append("null,");
                                            else
                                                count++;
                                        }
                                        if (count != list.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                            }
                            sb.Append("],");
                        }
                        else
                        {
                            if (valueTypeNullable && value == null)
                                sb.AppendFormat("\"{0}\":null,", f.Name);
                            else
                                sb.AppendFormat("\"{0}\":{1},", f.Name, ToJson(value));
                        }
                        break;
                }
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }
        /// <summary>
        /// JSON string convert to JSONData
        /// </summary>
        /// <param name="strJSON">JSON string</param>
        /// <returns>JSONData</returns>
        public static JSONData ToJSONData(string strJSON)
        {
            if (strJSON.Length == 0)
                return null;
            if (strJSON[0] == '{')
            {
                lock (KISSJsonImp.mObjLock)
                    return KISSJsonImp.DeserializeObject(strJSON);
            }
            if (strJSON[0] == '[')
            {
                lock (KISSJsonImp.mObjLock)
                    return KISSJsonImp.DeserializeArray(strJSON);
            } 
            return null;
        }
        /// <summary>
        /// cast JSON string to object (class/struct).
        /// e.g. TestJsonData testJsonData = (TestJsonData)KissJson.ToObject(typeof(TestJsonData), strJson);
        /// Why not make it like LitJson 'TestJsonData testJsonData = KissJson.ToObject＜TestJsonData>(strJson);'?
        /// Because that style not support the class in hot update script.
        /// </summary>
        /// <param name="type">type of object you want to cast</param>
        /// <param name="strJSON">JSON string</param>
        /// <returns>object</returns>
        public static object ToObject(Type type, string strJSON)
        {
            return ToObject(type, ToJSONData(strJSON));
        }
        /// <summary>
        /// cast JSONData to object (class/struct).
        /// e.g. TestJsonData testJsonData = (TestJsonData)KissJson.ToObject(typeof(TestJsonData), jsonData);
        /// </summary>
        /// <param name="type">type of object you want to cast</param>
        /// <param name="jsonData">JSONData object</param>
        /// <returns>object</returns>
        public static object ToObject(Type type, JSONData jsonData)
        {
            if (jsonData.dataType == JSONData.DataType.DataTypeDictionary)
                return _ToObject(type, jsonData);
            return null;
        }
        /// <summary>
        /// cast JSON string to objects (class/struct).
        /// e.g. List&lt;object&gt; testJsonDdata = KissJson.ToObject(typeof(TestJsonData), strJson);
        /// </summary>
        /// <param name="type">type of object you want to cast</param>
        /// <param name="strJSON">JSON string</param>
        /// <returns>List of object</returns>
        public static List<object> ToObjects(Type type, string strJSON)
        {
            return ToObjects(type, ToJSONData(strJSON));
        }
        /// <summary>
        /// cast JSONData to objects (class/struct).
        /// e.g. List&lt;object&gt; testJsonDdata = KissJson.ToObject(typeof(TestJsonData), strJson);
        /// </summary>
        /// <param name="type">type of object you want to cast</param>
        /// <param name="jsonData">JSONData object</param>
        /// <returns>List of object</returns>
        public static List<object> ToObjects(Type type, JSONData jsonData)
        {
            if (jsonData.dataType == JSONData.DataType.DataTypeList)
            {
                List<object> list = new List<object>();
                for (int i = 0; i < jsonData.Count; i++)
                    list.Add(ToObject(type, jsonData[i]));
                return list;
            }
            return null;
        }
        #region Internal implementation
#if _CSHARP_LIKE_
        public static object ToObject(SType type, string strJSON)
        {
            return ToObject(type, ToJSONData(strJSON));
        }
        public static object ToObject(SType type, JSONData jsonData)
        {
            if (jsonData.dataType == JSONData.DataType.DataTypeDictionary)
                return _ToObject(type, jsonData);
            return null;
        }
        public static List<object> ToObjects(SType type, string strJSON)
        {
            return ToObjects(type, ToJSONData(strJSON));
        }
        public static List<object> ToObjects(SType type, JSONData jsonData)
        {
            if (jsonData.dataType == JSONData.DataType.DataTypeList)
            {
                List<object> list = new List<object>();
                for (int i = 0; i < jsonData.Count; i++)
                    list.Add(ToObject(type, jsonData[i]));
                return list;
            }
            return null;
        }
        static Dictionary<string, SInstance> cacheSInstances = new Dictionary<string, SInstance>();
        static SInstance _ToObject(SType type, JSONData jsonObj)
        {
            if (jsonObj == null || type == null || type.IsDefined("CSharpLike.KissJsonDontSerialize"))
                return null;
            SInstance obj;
            if (jsonObj.ContainsKey("_uid_"))
            {
                string _uid_ = jsonObj["_uid_"];
                if (!cacheSInstances.TryGetValue(_uid_, out obj))
                {
                    obj = type.New().value as SInstance;
                    cacheSInstances[_uid_] = obj;
                }
            }
            else
                obj = type.New().value as SInstance;
            foreach (var member in obj.members)
            {
                if (type.IsMemberDefined(member.Key, "CSharpLike.KissJsonDontSerialize"))
                    continue;
                JSONData value;
                if (jsonObj.TryGetValue(member.Key, out value) && value != null)
                {
                    string FullName = member.Value.type.FullName;
                    if (FullName == "CSharpLike.JSONData")
                    {
                        member.Value.value = value;
                        continue;
                    }
                    else if (FullName == "System.DateTime")
                    {
                        member.Value.value = (DateTime)value;
                        continue;
                    }
                    switch (value.dataType)
                    {
                        case JSONData.DataType.DataTypeString:
                        case JSONData.DataType.DataTypeBoolean:
                        case JSONData.DataType.DataTypeInt:
                        case JSONData.DataType.DataTypeLong:
                        case JSONData.DataType.DataTypeDouble:
                        case JSONData.DataType.DataTypeBooleanNullable:
                        case JSONData.DataType.DataTypeIntNullable:
                        case JSONData.DataType.DataTypeLongNullable:
                        case JSONData.DataType.DataTypeDoubleNullable:
                            {
                                Type t = member.Value.type;
                                if (t != null && t.IsEnum)
                                    member.Value.value = Enum.Parse(t, value.Value.ToString());
                                else
                                    member.Value.value = value.Value;
                            }
                            break;
                        case JSONData.DataType.DataTypeDictionary:
                            {
                                if (FullName.StartsWith("System.Collections.Generic.Dictionary`2[[System.String,"))
                                {
                                    string strType = FullName;
                                    RemoveDictionary(ref strType);
                                    RemoveNullable(ref strType);
                                    var newDics = ((Type)member.Value.type).Assembly.CreateInstance(FullName) as IDictionary;
                                    Dictionary<string, JSONData> dics = value.Value as Dictionary<string, JSONData>;
                                    switch (strType)
                                    {
                                        case "System.String":
                                        case "System.SByte":
                                        case "System.UInt16":
                                        case "System.UInt32":
                                        case "System.UInt64":
                                        case "System.Byte":
                                        case "System.Int16":
                                        case "System.Int32":
                                        case "System.Int64":
                                        case "System.Boolean":
                                        case "System.Single":
                                        case "System.Double":
                                            foreach (var dic in dics)
                                                newDics.Add(dic.Key, dic.Value != null ? dic.Value.Value : null);
                                            break;
                                        case "System.DateTime":
                                            foreach (var dic in dics)
                                            {
                                                if (dic.Value != null)
                                                    newDics.Add(dic.Key, (DateTime)dic.Value.Value);
                                                else
                                                    newDics.Add(dic.Key, null);
                                            }
                                            break;
                                        case "System.Object":
                                            {
                                                string keyword = obj.type.GetMember(member.Key).type.Keyword.Substring(45);
                                                CSL_Type iType = HotUpdateManager.vm.GetTypeByKeyword(keyword.Substring(0, keyword.Length - 1)).type;
                                                Type t = iType;
                                                SType st = iType;
                                                if (t != null)
                                                {
                                                    foreach (var dic in dics)
                                                        if (!IsSkip(dic.Value))
                                                            newDics.Add(dic.Key, _ToObject(t, dic.Value));
                                                }
                                                else
                                                {
                                                    foreach (var dic in dics)
                                                        if (!IsSkip(dic.Value))
                                                            newDics.Add(dic.Key, _ToObject(st, dic.Value));
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    member.Value.value = newDics;
                                }
                                else if (FullName.StartsWith("System.Collections.Generic.Dictionary`2[[System.Int32,"))
                                {
                                    string strType = FullName;
                                    RemoveDictionaryInt32(ref strType);
                                    RemoveNullable(ref strType);
                                    var newDics = ((Type)member.Value.type).Assembly.CreateInstance(FullName) as IDictionary;
                                    Dictionary<string, JSONData> dics = value.Value as Dictionary<string, JSONData>;
                                    switch (strType)
                                    {
                                        case "System.String":
                                        case "System.SByte":
                                        case "System.UInt16":
                                        case "System.UInt32":
                                        case "System.UInt64":
                                        case "System.Byte":
                                        case "System.Int16":
                                        case "System.Int32":
                                        case "System.Int64":
                                        case "System.Boolean":
                                        case "System.Single":
                                        case "System.Double":
                                            foreach (var dic in dics)
                                                newDics.Add(Convert.ToInt32(dic.Key), dic.Value != null ? dic.Value.Value : null);
                                            break;
                                        case "System.DateTime":
                                            foreach (var dic in dics)
                                            {
                                                if (dic.Value != null)
                                                    newDics.Add(Convert.ToInt32(dic.Key), (DateTime)dic.Value.Value);
                                                else
                                                    newDics.Add(Convert.ToInt32(dic.Key), null);
                                            }
                                            break;
                                        case "System.Object":
                                            {
                                                string keyword = obj.type.GetMember(member.Key).type.Keyword.Substring(45);
                                                CSL_Type iType = HotUpdateManager.vm.GetTypeByKeyword(keyword.Substring(0, keyword.Length - 1)).type;
                                                Type t = iType;
                                                SType st = iType;
                                                if (t != null)
                                                {
                                                    foreach (var dic in dics)
                                                        if (!IsSkip(dic.Value))
                                                            newDics.Add(Convert.ToInt32(dic.Key), _ToObject(t, dic.Value));
                                                }
                                                else
                                                {
                                                    foreach (var dic in dics)
                                                        if (!IsSkip(dic.Value))
                                                            newDics.Add(Convert.ToInt32(dic.Key), _ToObject(st, dic.Value));
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    member.Value.value = newDics;
                                }
                                else if (!IsSkip(value))
                                {
                                    Type t = member.Value.type;
                                    SType st = member.Value.type;
                                    if (t != null)
                                        member.Value.value = _ToObject(t, value);
                                    else
                                        member.Value.value = _ToObject(st, value);
                                }
                            }
                            break;
                        case JSONData.DataType.DataTypeList:
                            {
                                string strType = FullName;
                                RemoveList(ref strType);
                                RemoveNullable(ref strType);
                                var newArray = ((Type)member.Value.type).Assembly.CreateInstance(FullName) as IList;
                                List<JSONData> arrays = value.Value as List<JSONData>;
                                switch (strType)
                                {
                                    case "System.String":
                                    case "System.SByte":
                                    case "System.UInt16":
                                    case "System.UInt32":
                                    case "System.UInt64":
                                    case "System.Byte":
                                    case "System.Int16":
                                    case "System.Int32":
                                    case "System.Int64":
                                    case "System.Boolean":
                                    case "System.Single":
                                    case "System.Double":
                                        foreach (var array in arrays)
                                            newArray.Add(array != null ? array.Value : null);
                                        break;
                                    case "System.DateTime":
                                        foreach (var array in arrays)
                                        {
                                            if (array != null)
                                                newArray.Add((DateTime)array.Value);
                                            else
                                                newArray.Add(null);
                                        }
                                        break;
                                    case "System.Object":
                                        {
                                            string keyword = obj.type.GetMember(member.Key).type.Keyword.Substring(32);
                                            CSL_Type iType = HotUpdateManager.vm.GetTypeByKeyword(keyword.Substring(0, keyword.Length - 1)).type;
                                            Type t = iType;
                                            SType st = iType;
                                            if (t != null)
                                            {
                                                foreach (var array in arrays)
                                                    if (!IsSkip(array))
                                                        newArray.Add(_ToObject(t, array));
                                            }
                                            else
                                            {
                                                foreach (var array in arrays)
                                                    if (!IsSkip(array))
                                                        newArray.Add(_ToObject(st, array));
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                member.Value.value = newArray;
                            }
                            break;
                    }
                }
            }
            return obj;
        }
#endif
        static Dictionary<string, object> cacheObjects = new Dictionary<string, object>();
        /// <summary>
        /// Clear the instance in cache that contain '_uid_' key.
        /// That object from server with '_uid_' will keep in cache and auto sync the value from server.
        /// </summary>
        /// <param name="key">The value of '_uid_', if passing null will clear all cache(You may call 'KissJson.ClearCache()' after you logout).
        /// You should remove it when you don't need that object.</param>
        public static void ClearCache(string key = null)
        {
#if _CSHARP_LIKE_
            if (key != null)
            {
                cacheObjects.Remove(key);
                cacheSInstances.Remove(key);
            }
            else
            {
                cacheObjects.Clear();
                cacheSInstances.Clear();
            }
#else
            if (key != null)
            {
                cacheObjects.Remove(key);
            }
            else
            {
                cacheObjects.Clear();
            }
#endif
        }
        static Assembly[] _mAssemblys = AppDomain.CurrentDomain.GetAssemblies();
        static Dictionary<string, Type> _TypeCache = new Dictionary<string, Type>();
        static Type _GetType(string typeName)
        {
            Type type;
            if (_TypeCache.TryGetValue(typeName, out type))
                return type;
            if (_mAssemblys == null)
                _mAssemblys = AppDomain.CurrentDomain.GetAssemblies();
            type = Type.GetType(typeName);
            if (type == null)
            {
                foreach (var item in _mAssemblys)
                {
                    type = Type.GetType(typeName + "," + item);
                    if (type != null)
                        break;
                }
            }
            _TypeCache[typeName] = type;
            return type;
        }
        static object _ToObject(Type type, JSONData jsonObj)
        {
            if (jsonObj == null)
                return null;
            object obj;
            if (jsonObj.ContainsKey("_uid_"))
            {
                string _uid_ = jsonObj["_uid_"];
                if (!cacheObjects.TryGetValue(_uid_, out obj))
                {
                    obj = type.Assembly.CreateInstance(type.FullName);
                    cacheObjects[_uid_] = obj;
                }
            }
            else
                obj = type.Assembly.CreateInstance(type.FullName);
            var fs = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var f in fs)
            {
                if (Attribute.IsDefined(f, typeof(KissJsonDontSerialize)))
                    continue;
                JSONData value;
                if (jsonObj.TryGetValue(f.Name, out value) && value != null)
                {
                    if (f.FieldType.IsEnum)
                    {
                        f.SetValue(obj, Enum.Parse(f.FieldType, value.Value.ToString())); 
                        continue;
                    }
                    if (f.FieldType.FullName == "CSharpLike.JSONData")
                    {
                        f.SetValue(obj, value);
                        continue;
                    }
                    else if (f.FieldType.FullName == "System.DateTime")
                    {
                        f.SetValue(obj, (DateTime)value);
                        continue;
                    }
                    switch (value.dataType)
                    {
                        case JSONData.DataType.DataTypeString:
                        case JSONData.DataType.DataTypeBoolean:
                        case JSONData.DataType.DataTypeInt:
                        case JSONData.DataType.DataTypeLong:
                        case JSONData.DataType.DataTypeDouble:
                        case JSONData.DataType.DataTypeBooleanNullable:
                        case JSONData.DataType.DataTypeIntNullable:
                        case JSONData.DataType.DataTypeLongNullable:
                        case JSONData.DataType.DataTypeDoubleNullable:
                            switch (f.FieldType.Name)
                            {
                                case "Boolean": f.SetValue(obj, Convert.ToBoolean(value.Value)); break;
                                case "Byte": f.SetValue(obj, Convert.ToByte(value.Value)); break;
                                case "SByte": f.SetValue(obj, Convert.ToSByte(value.Value)); break;
                                case "Int16": f.SetValue(obj, Convert.ToInt16(value.Value)); break;
                                case "UInt16": f.SetValue(obj, Convert.ToUInt16(value.Value)); break;
                                case "Int32": f.SetValue(obj, Convert.ToInt32(value.Value)); break;
                                case "UInt32": f.SetValue(obj, Convert.ToUInt32(value.Value)); break;
                                case "Char": f.SetValue(obj, Convert.ToChar(value.Value)); break;
                                case "Single": f.SetValue(obj, Convert.ToSingle(value.Value, CultureForConvertFloatAndDouble)); break;
                                case "Double": f.SetValue(obj, Convert.ToDouble(value.Value, CultureForConvertFloatAndDouble)); break;
                                case "Int64": f.SetValue(obj, Convert.ToInt64(value.Value)); break;
                                case "UInt64": f.SetValue(obj, Convert.ToUInt64(value.Value)); break;
                                case "Decimal": f.SetValue(obj, Convert.ToDecimal(value.Value)); break;
                                case "String": f.SetValue(obj, Convert.ToString(value.Value)); break;
                                default: f.SetValue(obj, value.Value); break;
                            }
                            break;
                        case JSONData.DataType.DataTypeDictionary:
                            {
                                if (f.FieldType.FullName.StartsWith("System.Collections.Generic.Dictionary`2[[System.String,"))
                                {
                                    string strType = f.FieldType.FullName;
                                    RemoveDictionary(ref strType);
                                    RemoveNullable(ref strType);
                                    Dictionary<string, JSONData> dics = value.Value as Dictionary<string, JSONData>;
                                    var newDics = f.FieldType.Assembly.CreateInstance(f.FieldType.FullName) as IDictionary;
                                    Type subType = _GetType(strType);
                                    switch (strType)
                                    {
                                        case "System.String":
                                        case "System.SByte":
                                        case "System.UInt16":
                                        case "System.UInt32":
                                        case "System.UInt64":
                                        case "System.Byte":
                                        case "System.Int16":
                                        case "System.Int32":
                                        case "System.Int64":
                                        case "System.Boolean":
                                        case "System.Single":
                                        case "System.Double":
                                            foreach (var dic in dics)
                                                newDics.Add(dic.Key, dic.Value != null ? dic.Value.Value : null);
                                            break;
                                        case "System.DateTime":
                                            foreach (var dic in dics)
                                            {
                                                if (dic.Value != null)
                                                    newDics.Add(dic.Key, (DateTime)dic.Value.Value);
                                                else
                                                    newDics.Add(dic.Key, null);
                                            }
                                            break;
                                        default:
                                            foreach (var dic in dics)
                                                if (!IsSkip(dic.Value))
                                                    newDics.Add(dic.Key, _ToObject(subType, dic.Value));
                                            break;
                                    }
                                    f.SetValue(obj, newDics);
                                }
                                else if (f.FieldType.FullName.StartsWith("System.Collections.Generic.Dictionary`2[[System.Int32,"))
                                {
                                    string strType = f.FieldType.FullName;
                                    RemoveDictionaryInt32(ref strType);
                                    RemoveNullable(ref strType);
                                    Dictionary<string, JSONData> dics = value.Value as Dictionary<string, JSONData>;
                                    var newDics = f.FieldType.Assembly.CreateInstance(f.FieldType.FullName) as IDictionary;
                                    Type subType = _GetType(strType);
                                    switch (strType)
                                    {
                                        case "System.String":
                                        case "System.SByte":
                                        case "System.UInt16":
                                        case "System.UInt32":
                                        case "System.UInt64":
                                        case "System.Byte":
                                        case "System.Int16":
                                        case "System.Int32":
                                        case "System.Int64":
                                        case "System.Boolean":
                                        case "System.Single":
                                        case "System.Double":
                                            foreach (var dic in dics)
                                                newDics.Add(Convert.ToInt32(dic.Key), dic.Value != null ? dic.Value.Value : null);
                                            break;
                                        case "System.DateTime":
                                            foreach (var dic in dics)
                                            {
                                                if (dic.Value != null)
                                                    newDics.Add(Convert.ToInt32(dic.Key), (DateTime)dic.Value.Value);
                                                else
                                                    newDics.Add(Convert.ToInt32(dic.Key), null);
                                            }
                                            break;
                                        default:
                                            foreach (var dic in dics)
                                                if (!IsSkip(dic.Value))
                                                    newDics.Add(Convert.ToInt32(dic.Key), _ToObject(subType, dic.Value));
                                            break;
                                    }
                                    f.SetValue(obj, newDics);
                                }
                                else
                                {
                                    if (!IsSkip(value))
                                        f.SetValue(obj, _ToObject(f.FieldType, value));
                                }
                            }
                            break;
                        case JSONData.DataType.DataTypeList:
                            {
                                string strType = f.FieldType.FullName;
                                RemoveList(ref strType);
                                RemoveNullable(ref strType);
                                var newArray = f.FieldType.Assembly.CreateInstance(f.FieldType.FullName) as IList;
                                Type subType = _GetType(strType);
                                List<JSONData> arrays = value.Value as List<JSONData>;
                                switch (strType)
                                {
                                    case "System.String":
                                    case "System.SByte":
                                    case "System.UInt16":
                                    case "System.UInt32":
                                    case "System.UInt64":
                                    case "System.Byte":
                                    case "System.Int16":
                                    case "System.Int32":
                                    case "System.Int64":
                                    case "System.Boolean":
                                    case "System.Single":
                                    case "System.Double":
                                        foreach (var array in arrays)
                                            newArray.Add(array != null ? array.Value : null);
                                        break;
                                    case "System.DateTime":
                                        foreach (var array in arrays)
                                        {
                                            if (array != null)
                                                newArray.Add((DateTime)array.Value);
                                            else
                                                newArray.Add(null);
                                        }
                                        break;
                                    default:
                                        foreach (var array in arrays)
                                        {
                                            if (!IsSkip(array))
                                                newArray.Add(_ToObject(subType, array));
                                        }
                                        break;
                                }
                                f.SetValue(obj, newArray);
                            }
                            break;
                    }
                }
            }
            var ps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
            foreach (var f in ps)
            {
                if (!Attribute.IsDefined(f, typeof(KissJsonSerializeProperty)))
                    continue;
                JSONData value;
                if (jsonObj.TryGetValue(f.Name, out value) && value != null)
                {
                    if (f.PropertyType.IsEnum)
                    {
                        f.SetValue(obj, Enum.Parse(f.PropertyType, value.Value.ToString()));
                        continue;
                    }
                    if (f.PropertyType.FullName == "CSharpLike.JSONData")
                    {
                        f.SetValue(obj, value);
                        continue;
                    }
                    else if (f.PropertyType.FullName == "System.DateTime")
                    {
                        f.SetValue(obj, (DateTime)value);
                        continue;
                    }
                    switch (value.dataType)
                    {
                        case JSONData.DataType.DataTypeString:
                        case JSONData.DataType.DataTypeBoolean:
                        case JSONData.DataType.DataTypeInt:
                        case JSONData.DataType.DataTypeLong:
                        case JSONData.DataType.DataTypeDouble:
                        case JSONData.DataType.DataTypeBooleanNullable:
                        case JSONData.DataType.DataTypeIntNullable:
                        case JSONData.DataType.DataTypeLongNullable:
                        case JSONData.DataType.DataTypeDoubleNullable:
                            f.SetValue(obj, value.Value);
                            break;
                        case JSONData.DataType.DataTypeDictionary:
                            {
                                if (f.PropertyType.FullName.StartsWith("System.Collections.Generic.Dictionary`2[[System.String,"))
                                {
                                    string strType = f.PropertyType.FullName;
                                    RemoveDictionary(ref strType);
                                    RemoveNullable(ref strType);
                                    Dictionary<string, JSONData> dics = value.Value as Dictionary<string, JSONData>;
                                    var newDics = f.PropertyType.Assembly.CreateInstance(f.PropertyType.FullName) as IDictionary;
                                    Type subType = _GetType(strType);
                                    switch (strType)
                                    {
                                        case "System.String":
                                        case "System.SByte":
                                        case "System.UInt16":
                                        case "System.UInt32":
                                        case "System.UInt64":
                                        case "System.Byte":
                                        case "System.Int16":
                                        case "System.Int32":
                                        case "System.Int64":
                                        case "System.Boolean":
                                        case "System.Single":
                                        case "System.Double":
                                            foreach (var dic in dics)
                                                newDics.Add(dic.Key, dic.Value != null ? dic.Value.Value : null);
                                            break;
                                        case "System.DateTime":
                                            foreach (var dic in dics)
                                            {
                                                if (dic.Value != null)
                                                    newDics.Add(dic.Key, (DateTime)dic.Value.Value);
                                                else
                                                    newDics.Add(dic.Key, null);
                                            }
                                            break;
                                        default:
                                            foreach (var dic in dics)
                                                if (!IsSkip(dic.Value))
                                                    newDics.Add(dic.Key, _ToObject(subType, dic.Value));
                                            break;
                                    }
                                    f.SetValue(obj, newDics);
                                }
                                else if (f.PropertyType.FullName.StartsWith("System.Collections.Generic.Dictionary`2[[System.Int32,"))
                                {
                                    string strType = f.PropertyType.FullName;
                                    RemoveDictionaryInt32(ref strType);
                                    RemoveNullable(ref strType);
                                    Dictionary<string, JSONData> dics = value.Value as Dictionary<string, JSONData>;
                                    var newDics = f.PropertyType.Assembly.CreateInstance(f.PropertyType.FullName) as IDictionary;
                                    Type subType = _GetType(strType);
                                    switch (strType)
                                    {
                                        case "System.String":
                                        case "System.SByte":
                                        case "System.UInt16":
                                        case "System.UInt32":
                                        case "System.UInt64":
                                        case "System.Byte":
                                        case "System.Int16":
                                        case "System.Int32":
                                        case "System.Int64":
                                        case "System.Boolean":
                                        case "System.Single":
                                        case "System.Double":
                                            foreach (var dic in dics)
                                                newDics.Add(Convert.ToInt32(dic.Key), dic.Value != null ? dic.Value.Value : null);
                                            break;
                                        case "System.DateTime":
                                            foreach (var dic in dics)
                                            {
                                                if (dic.Value != null)
                                                    newDics.Add(Convert.ToInt32(dic.Key), (DateTime)dic.Value.Value);
                                                else
                                                    newDics.Add(Convert.ToInt32(dic.Key), null);
                                            }
                                            break;
                                        default:
                                            foreach (var dic in dics)
                                                if (!IsSkip(dic.Value))
                                                    newDics.Add(Convert.ToInt32(dic.Key), _ToObject(subType, dic.Value));
                                            break;
                                    }
                                    f.SetValue(obj, newDics);
                                }
                                else
                                {
                                    if (!IsSkip(value))
                                        f.SetValue(obj, _ToObject(f.PropertyType, value));
                                }
                            }
                            break;
                        case JSONData.DataType.DataTypeList:
                            {
                                string strType = f.PropertyType.FullName;
                                RemoveList(ref strType);
                                RemoveNullable(ref strType);
                                var newArray = f.PropertyType.Assembly.CreateInstance(f.PropertyType.FullName) as IList;
                                Type subType = _GetType(strType);
                                List<JSONData> arrays = value.Value as List<JSONData>;
                                switch (strType)
                                {
                                    case "System.String":
                                    case "System.SByte":
                                    case "System.UInt16":
                                    case "System.UInt32":
                                    case "System.UInt64":
                                    case "System.Byte":
                                    case "System.Int16":
                                    case "System.Int32":
                                    case "System.Int64":
                                    case "System.Boolean":
                                    case "System.Single":
                                    case "System.Double":
                                        foreach (var array in arrays)
                                            newArray.Add(array != null ? array.Value : null);
                                        break;
                                    case "System.DateTime":
                                        foreach (var array in arrays)
                                        {
                                            if (array != null)
                                                newArray.Add((DateTime)array.Value);
                                            else
                                                newArray.Add(null);
                                        }
                                        break;
                                    default:
                                        foreach (var array in arrays)
                                        {
                                            if (!IsSkip(array))
                                                newArray.Add(_ToObject(subType, array));
                                        }
                                        break;
                                }
                                f.SetValue(obj, newArray);
                            }
                            break;
                    }
                }
            }
            return obj;
        }
        static bool IsSkip(object obj)
        {
            if (obj == null)
                return true;
#if _CSHARP_LIKE_
            SInstance sObj = obj as SInstance;
            if (sObj != null)
            {
                return sObj.type.IsDefined("CSharpLike.KissJsonDontSerialize");
            }
            else
#endif
                return obj.GetType().IsDefined(typeof(KissJsonDontSerialize));
        }
        /// <summary>
        /// Remove nullable string
        /// </summary>
        public static bool RemoveNullable(ref string str)
        {
            bool bRet = false;
            if (str.StartsWith("System.Nullable`1["))
            {
                bRet = true; 
                str = str.Substring(19);
                int i = str.IndexOf(',');
                if (i > 0)
                    str = str.Substring(0, i);
                else if (str.EndsWith("]"))
                    str = str.Substring(0, str.Length - 1);
            }
            return bRet;
        }
        static bool RemoveDictionary(ref string str)
        {
            bool bRet = false;
            if (str.StartsWith("System.Collections.Generic.Dictionary`2[[System.String,"))
            {
                bRet = true;
                int i = str.IndexOf(']');
                str = str.Substring(i + 3);
                i = str.IndexOf(',');
                if (i > 0)
                    str = str.Substring(0, i);
                else if (str.EndsWith("]"))
                    str = str.Substring(0, str.Length - 1);
            }
            return bRet;
        }
        static bool RemoveDictionaryInt32(ref string str)
        {
            bool bRet = false;
            if (str.StartsWith("System.Collections.Generic.Dictionary`2[[System.Int32,"))
            {
                bRet = true;
                int i = str.IndexOf(']');
                str = str.Substring(i + 3);
                i = str.IndexOf(',');
                if (i > 0)
                    str = str.Substring(0, i);
                else if (str.EndsWith("]"))
                    str = str.Substring(0, str.Length - 1);
            }
            return bRet;
        }
        static bool RemoveList(ref string str)
        {
            bool bRet = false;
            if (str.StartsWith("System.Collections.Generic.List`1["))
            {
                bRet = true;
                str = str.Substring(35);
                int i = str.IndexOf(',');
                if (i > 0)
                    str = str.Substring(0, i);
                else if (str.EndsWith("]"))
                    str = str.Substring(0, str.Length - 1);
            }
            return bRet;
        }
#if _CSHARP_LIKE_
        public static string ToJson(SInstance obj)
        {
            if (obj == null || obj.type.IsDefined("CSharpLike.KissJsonDontSerialize"))
                return "{}";
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var member in obj.members)
            {
                if (obj.type.IsMemberDefined(member.Key, "CSharpLike.KissJsonDontSerialize"))
                    continue;
                if (member.Value.value == null)
                {
                    if (!ignoreNull)
                    {
                        Type t = member.Value.type;
                        SType st = member.Value.type;
                        if (st != null)
                        {
                            if (st.IsDefined("CSharpLike.KissJsonDontSerialize"))
                                continue;
                        }
                        else
                        {
                            if (t.IsDefined(typeof(KissJsonDontSerialize)))
                                continue;
                        }
                        sb.AppendFormat("\"{0}\":null,", member.Key);
                    }
                    continue;
                }
                string valueTypeFullName = member.Value.type.FullName;
                bool valueTypeNullable = RemoveNullable(ref valueTypeFullName);
                switch (valueTypeFullName)
                {
                    case "System.String":
                        if (member.Value.value == null)
                        {
                            if (!ignoreNull)
                                sb.AppendFormat("\"{0}\":null,", member.Key);
                        }
                        else
                            sb.AppendFormat("\"{0}\":\"{1}\",", member.Key, member.Value.value);
                        break;
                    case "System.SByte":
                    case "System.UInt16":
                    case "System.UInt32":
                    case "System.UInt64":
                    case "System.Byte":
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Single":
                    case "System.Double":
                        if (valueTypeNullable)
                        {
                            if (member.Value.value == null)
                                sb.AppendFormat("\"{0}\":null,", member.Key);
                            else
                                sb.AppendFormat("\"{0}\":{1},", member.Key, member.Value.value);
                        }
                        else
                            sb.AppendFormat("\"{0}\":{1},", member.Key, member.Value.value);
                        break;
                    case "System.Boolean":
                        if (valueTypeNullable)
                        {
                            if (member.Value.value == null)
                                sb.AppendFormat("\"{0}\":null,", member.Key);
                            else
                                sb.AppendFormat("\"{0}\":{1},", member.Key, (bool)member.Value.value ? "true" : "false");
                        }
                        else
                            sb.AppendFormat("\"{0}\":{1},", member.Key, (bool)member.Value.value ? "true" : "false");
                        break;
                    case "System.DateTime":
                        if (valueTypeNullable)
                        {
                            if (member.Value.value == null)
                                sb.AppendFormat("\"{0}\":null,", member.Key);
                            else
                                sb.AppendFormat("\"{0}\":\"{1}\",", member.Key, ((DateTime)member.Value.value).ToString(CultureForConvertDateTime));
                        }
                        else
                            sb.AppendFormat("\"{0}\":\"{1}\",", member.Key, ((DateTime)member.Value.value).ToString(CultureForConvertDateTime));
                        break;
                    default:
                        if (valueTypeFullName.StartsWith("System.Collections.Generic.Dictionary`2[[System.String,"))
                        {
                            RemoveDictionary(ref valueTypeFullName);
                            bool bNullable = RemoveNullable(ref valueTypeFullName);
                            sb.AppendFormat("\"{0}\":", member.Key);
                            sb.Append("{");
                            IDictionary dic = member.Value.value as IDictionary;
                            switch (valueTypeFullName)
                            {
                                case "System.Boolean":
                                    if (dic.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (DictionaryEntry item in dic)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value == null ? "null" : ((bool)item.Value ? "true" : "false"));
                                        }
                                        else
                                        {
                                            foreach (DictionaryEntry item in dic)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, (bool)item.Value ? "true" : "false");
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.DateTime":
                                    if (dic.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (DictionaryEntry item in dic)
                                            {
                                                if (item.Value == null)
                                                    sb.AppendFormat("\"{0}\":null,", item.Key);
                                                else
                                                    sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, ((DateTime)item.Value).ToString(CultureForConvertDateTime));
                                            }
                                        }
                                        else
                                        {
                                            foreach (DictionaryEntry item in dic)
                                                sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, ((DateTime)item.Value).ToString(CultureForConvertDateTime));
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.String":
                                    if (dic.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (DictionaryEntry item in dic)
                                        {
                                            if (item.Value != null)
                                                sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, item.Value);
                                            else if (!ignoreNull)
                                                sb.AppendFormat("\"{0}\":null,", item.Key);
                                            else
                                                count++;
                                        }
                                        if (count != dic.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.SByte":
                                case "System.UInt16":
                                case "System.UInt32":
                                case "System.UInt64":
                                case "System.Byte":
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Single":
                                case "System.Double":
                                    if (dic.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (DictionaryEntry item in dic)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value == null ? "null" : item.Value);
                                        }
                                        else
                                        {
                                            foreach (DictionaryEntry item in dic)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value);
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                default:
                                    if (dic.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (DictionaryEntry item in dic)
                                        {
                                            if (item.Value != null)
                                                sb.AppendFormat("\"{0}\":{1},", item.Key, ToJson(item.Value));
                                            else if (!ignoreNull)
                                                sb.AppendFormat("\"{0}\":null,", item.Key);
                                            else
                                                count++;
                                        }
                                        if (count != dic.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                            }
                            sb.Append("},");
                        }
                        else if (valueTypeFullName.StartsWith("System.Collections.Generic.List`1["))
                        {
                            RemoveList(ref valueTypeFullName);
                            bool bNullable = RemoveNullable(ref valueTypeFullName);
                            sb.AppendFormat("\"{0}\":[", member.Key);
                            IList list = member.Value.value as IList;
                            switch (valueTypeFullName)
                            {
                                case "System.String":
                                    if (list.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (var item in list)
                                        {
                                            if (item != null)
                                                sb.AppendFormat("\"{0}\",", item);
                                            else if (!ignoreNull)
                                                sb.Append("null,");
                                            else
                                                count++;
                                        }
                                        if (count != list.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.SByte":
                                case "System.UInt16":
                                case "System.UInt32":
                                case "System.UInt64":
                                case "System.Byte":
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Single":
                                case "System.Double":
                                    if (list.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (var item in list)
                                            {
                                                if (item == null)
                                                    sb.Append("null,");
                                                else
                                                    sb.Append(item + ",");
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in list)
                                                sb.Append(item + ",");
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.Boolean":
                                    if (list.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (var item in list)
                                            {
                                                if (item == null)
                                                    sb.Append("null,");
                                                else
                                                    sb.Append((bool)item ? "true," : "false,");
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in list)
                                                sb.Append((bool)item ? "true," : "false,");
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                case "System.DateTime":
                                    if (list.Count > 0)
                                    {
                                        if (bNullable)
                                        {
                                            foreach (var item in list)
                                            {
                                                if (item == null)
                                                    sb.Append("null,");
                                                else
                                                    sb.Append("\"" + ((DateTime)item).ToString(CultureForConvertDateTime) + "\",");
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in list)
                                                sb.Append("\"" + ((DateTime)item).ToString(CultureForConvertDateTime) + "\",");
                                        }
                                        sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                                default:
                                    if (list.Count > 0)
                                    {
                                        int count = 0;
                                        foreach (var item in list)
                                        {
                                            if (item != null)
                                                sb.Append(ToJson(item) + ",");
                                            else if (!ignoreNull)
                                                sb.Append("null,");
                                            else
                                                count++;
                                        }
                                        if (count != list.Count)
                                            sb.Remove(sb.Length - 1, 1);
                                    }
                                    break;
                            }
                            sb.Append("],");
                        }
                        else
                        {
                            if (valueTypeNullable && member.Value.value == null)
                                sb.AppendFormat("\"{0}\":null,", member.Key);
                            else
                                sb.AppendFormat("\"{0}\":{1},", member.Key, ToJson(member.Value.value));
                        }
                        break;
                }
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }
#endif
        private class KISSJsonImp
        {
            public static object mObjLock = new object();
            public static bool ignoreNull = false;
            private static char[] mCharBuff;
            private static int mCharLenght;

            private static JSONData DeserializeSingletonObject(ref int left)
            {
                JSONData localjson = JSONData.NewDictionary();
                while (left <= mCharLenght)
                {
                    char c = mCharBuff[left];
                    if (c == ' ' || c == '\r' || c == '\n' || c == '\t')  //skip empty char
                    {
                        left++;
                        continue;
                    }
                    if (c == ',')
                    {
                        left++;
                        continue;
                    }
                    char r = '\0';
                    if (c == '\"' || c == '\'')     //beginning of key
                    {
                        left++;
                        r = c;
                    }
                    else if (c == '}')      //end of JSONObject
                    {
                        left++;
                        break;
                    }
                    int column = left;
                    if (r == '\0')
                    {
                        while (mCharBuff[column] != ':') column++;
                    }
                    else
                    {
                        while (!(mCharBuff[column] == r && mCharBuff[column - 1] != '\\' && mCharBuff[column + 1] == ':')) column++;
                    }
                    string key = parseString(left, column);         //get the key
                    if (r == '\0')
                        left = column + 1;
                    else
                        left = column + 2;
                    c = mCharBuff[left];
                    while (c == ' ' || c == '\r' || c == '\n' || c == '\t')  //skip empty char
                    {
                        left++;
                        c = mCharBuff[left];
                    }
                    if (c == '\"' || c == '\'')     //if value is string
                    {
                        left++;
                        int strend = left;
                        while (mCharBuff[strend] != c || mCharBuff[strend - 1] == '\\') strend++;
                        localjson[key] = parseString(left, strend);
                        left = strend + 1;
                    }
                    else if (c == '{') // JSONObject
                    {
                        left++;
                        localjson[key] = DeserializeSingletonObject(ref left);
                    }
                    else if (c == '[')     //JSONArray
                    {
                        left++;
                        localjson[key] = DeserializeSingletonArray(ref left);
                    }
                    else
                    {
                        int comma = left;
                        char co = mCharBuff[comma];
                        while (co != ',' && co != '}')
                        {
                            comma++;
                            co = mCharBuff[comma];
                        }
                        int em = comma - 1;
                        co = mCharBuff[em];
                        while (co == ' ' || co == '\r' || co == '\n' || co == '\t')
                        {
                            em--;
                            co = mCharBuff[em];
                        }
                        localjson[key] = parseNumber(left, em);
                        left = comma;
                    }
                }
                return localjson;
            }

            static string parseString(int start, int end)
            {
                string str = "";
                for(int i=start; i<end; i++)
                {
                    char c = mCharBuff[i];
                    if (c == '\\' && i < (end - 1))
                    {
                        switch(mCharBuff[i + 1])
                        {
                            case '"': str += "\""; ++i; break;
                            case '\\': str += "\\"; ++i; break;
                            case 'b': str += "\b"; ++i; break;
                            case 'f': str += "\f"; ++i; break;
                            case 'n': str += "\n"; ++i; break;
                            case 'r': str += "\r"; ++i; break;
                            case 't': str += "\t"; ++i; break;
                            case 'u':
                                if (end - i >= 4)
                                {
                                    byte[] buff = new byte[2];
                                    i++;
                                    string sss = "" + mCharBuff[++i] + mCharBuff[++i];
                                    buff[1] = (byte)Convert.ToInt32(sss, 16);
                                    sss = "" + mCharBuff[++i] + mCharBuff[++i];
                                    buff[0] = (byte)Convert.ToInt32(sss, 16);
                                    str += Encoding.Unicode.GetString(buff);
                                }
                                break;
                        }
                    }
                    else
                        str += c;
                }
                return str;
            }

            static JSONData parseNumber(int start, int end)
            {
                char c = mCharBuff[start];
                if (c == 't' || c == 'T')//true TRUE, just check first character only
                    return true;
                else if (c == 'f' || c == 'F')//false FALSE, just check first character only
                    return false;
                else if (c == 'n' || c == 'N')//null NULL, just check first character only
                    return null;
                bool bDot = false;
                for (int i = start; i <= end; i++)
                {
                    if (mCharBuff[i] == '.')
                    {
                        bDot = true;
                        break;
                    }
                }
                if (bDot)
                    return Convert.ToDouble(new string(mCharBuff, start, end - start + 1), CultureForConvertFloatAndDouble);
                else
                {
                    if (c == '-' || (end - start) < 19)
                    {
                        long v = Convert.ToInt64(new string(mCharBuff, start, end - start + 1));
                        if (v <= int.MaxValue && v >= int.MinValue)
                            return (int)v;
                        else
                            return v;
                    }
                    else
                    {
                        try
                        {
                            long v = Convert.ToInt64(new string(mCharBuff, start, end - start + 1));
                            if (v <= int.MaxValue && v >= int.MinValue)
                                return (int)v;
                            else
                                return v;
                        }
                        catch
                        {
                            ulong v = Convert.ToUInt64(new string(mCharBuff, start, end - start + 1));
                            if (v <= uint.MaxValue && v >= uint.MinValue)
                                return (uint)v;
                            else
                                return v;
                        }
                    }
                }
            }

            private static JSONData DeserializeSingletonArray(ref int left)
            {
                JSONData jsary = JSONData.NewList();
                while (left <= mCharLenght)
                {
                    char c = mCharBuff[left];
                    if (c == ' ' || c == '\r' || c == '\n' || c == '\t')  //skip empty char
                    {
                        left++;
                        continue;
                    }
                    if (c == ',')
                    {
                        left++;
                        continue;
                    }
                    if (c == ']')
                    {
                        left++;
                        break;
                    }
                    if (c == '{') //Dictionary
                    {
                        left++;
                        jsary.Add(DeserializeSingletonObject(ref left));
                    }
                    else if (c == '[')     //List
                    {
                        left++;
                        jsary.Add(DeserializeSingletonArray(ref left));
                    }
                    else if (c == '\"' || c == '\'')            //string
                    {
                        left++;
                        int strend = left;
                        while (mCharBuff[strend] != c || mCharBuff[strend - 1] == '\\') strend++;
                        jsary.Add(parseString(left, strend));
                        left = strend + 1;
                    }
                    else
                    {
                        int comma = left;
                        char co = mCharBuff[comma];
                        while (co != ',' && co != ']')
                        {
                            comma++;
                            co = mCharBuff[comma];
                        }
                        int em = comma - 1;
                        co = mCharBuff[em];
                        while (co == ' ' || co == '\r' || co == '\n' || co == '\t')
                        {
                            em--;
                            co = mCharBuff[em];
                        }
                        jsary.Add(parseNumber(left, em));
                        left = comma;
                    }
                }
                return jsary;
            }

            private static JSONData DeserializeCharToObject(char[] input)
            {
                mCharBuff = input;
                mCharLenght = mCharBuff.Length - 1;
                while (mCharLenght > 0)
                    if (mCharBuff[mCharLenght] != '}')
                        mCharLenght--;
                    else
                        break;
                int start = 0;
                while (start < mCharLenght)
                    if (mCharBuff[start] != '{')
                        start++;
                    else
                        break;
                start++;
                if (mCharLenght < start + 1)
                    return null;
                return DeserializeSingletonObject(ref start);
            }

            public static JSONData DeserializeObject(string input)
            {
                return DeserializeCharToObject(input.ToCharArray());     //The first char must be '{'
            }

            private static JSONData DeserializeCharsToArray(char[] input)
            {
                mCharBuff = input;
                mCharLenght = mCharBuff.Length - 1;
                while (mCharLenght > 0)
                    if (mCharBuff[mCharLenght] != ']')
                        mCharLenght--;
                    else
                        break;
                int start = 0;
                while (start < mCharLenght)
                    if (mCharBuff[start] != '[')
                        start++;
                    else
                        break;
                start++;
                if (mCharLenght < start + 1)
                    return null;
                return DeserializeSingletonArray(ref start);
            }

            public static JSONData DeserializeArray(string input)
            {
                return DeserializeCharsToArray(input.ToCharArray());
            }
        }
        static byte[] mHeader = Encoding.UTF8.GetBytes("KissJSON");
#endregion //Internal implementation
    }
}