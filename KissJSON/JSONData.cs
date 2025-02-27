/*
 * KissJson : Keep It Simple Stupid JSON
 * Copyright © 2022-2025 RongRong. All right reserved.
 */
using System.Text;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;

namespace CSharpLike
{
    /// <summary>
    /// A simple JSON data.
    /// It's not the most efficient,but the easiest to use.
    /// </summary>
    public sealed class JSONData
    {
        /// <summary>
        /// Data type of the JSON
        /// </summary>
        public enum DataType : byte
        {
            /// <summary>
            /// This JSON object is null
            /// </summary>
            DataTypeNull,
            /// <summary>
            /// Is Dictionary type
            /// </summary>
            DataTypeDictionary,
            /// <summary>
            /// Is List type
            /// </summary>
            DataTypeList,
            /// <summary>
            /// Value type is string
            /// </summary>
            DataTypeString,
            /// <summary>
            /// Value type is boolean
            /// </summary>
            DataTypeBoolean,
            /// <summary>
            /// IValue type is a nt
            /// </summary>
            DataTypeInt,
            /// <summary>
            /// Value type is long
            /// </summary>
            DataTypeLong,
            /// <summary>
            /// Value type is ulong
            /// </summary>
            DataTypeULong,
            /// <summary>
            /// Value type is double
            /// </summary>
            DataTypeDouble,
            /// <summary>
            /// Value type is nullable boolean
            /// </summary>
            DataTypeBooleanNullable,
            /// <summary>
            /// Value type is nullable int
            /// </summary>
            DataTypeIntNullable,
            /// <summary>
            /// Value type is nullable long
            /// </summary>
            DataTypeLongNullable,
            /// <summary>
            /// Value type is nullable ulong
            /// </summary>
            DataTypeULongNullable,
            /// <summary>
            /// Value type is nullable double
            /// </summary>
            DataTypeDoubleNullable,
        }
        /// <summary>
        /// The packet type save as integer or string. 
        /// Default is string, that easy identify in log.
        /// </summary>
        public static bool packetIsInteger = false;
#if !_CSHARP_LIKE_ //That prevent you call this from hot update script
        /// <summary>
        /// Create a JSONData with packet type for network packet, that use in NONE hot update script.
        /// </summary>
        /// <param name="packetType">Add to JSON object with key 'packetType'</param>
        public static JSONData NewPacket<T>(T packetType) where T : Enum
        {
            JSONData data = new JSONData();
            data.dictValue = new Dictionary<string, JSONData>();
            data.dataType = DataType.DataTypeDictionary;
            if (packetIsInteger)
                data["packetType"] = Convert.ToInt32(packetType);
            else
                data["packetType"] = packetType.ToString();
            return data;
        }
        /// <summary>
        /// Get the packet type, that use in NONE hot update script.
        /// </summary>
        public T GetPacketType<T>()
        {
            return (T)Enum.Parse(typeof(T), this["packetType"]);
        }
#endif
        /// <summary>
        /// Create a JSONData with packet type for network packet, that use in hot update script.
        /// </summary>
        /// <param name="type">The type of the Packet</param>
        /// <param name="value">The value of 'packetType'</param>
        public static JSONData NewPacket(Type type, object value)
        {
            JSONData data = new JSONData();
            data.dictValue = new Dictionary<string, JSONData>();
            data.dataType = DataType.DataTypeDictionary;
            if (packetIsInteger)
                data["packetType"] = (int)value;
            else
                data["packetType"] = value.ToString();
            return data;
        }
        /// <summary>
        /// Get the packet type, that use in hot update script.
        /// </summary>
        public int GetPacketType(Type type)
        {
            if (packetIsInteger)
                return this["packetType"];
            return (int)Enum.Parse(type, this["packetType"]);
        }
#if _CSHARP_LIKE_
        public static JSONData NewPacket(Internal.SType type, object value)
        {
            JSONData data = new JSONData();
            data.dictValue = new Dictionary<string, JSONData>();
            data.dataType = DataType.DataTypeDictionary;
            if (packetIsInteger)
                data["packetType"] = (int)value;
            else
                data["packetType"] = HotUpdateManager.ConvertEnumString(type, value);
            return data;
        }
        public int GetPacketType(Internal.SType type)
        {
            if (packetIsInteger)
                return this["packetType"];
            return (int)HotUpdateManager.ConvertEnumNumber(type, this["packetType"]);
        }
#endif
        /// <summary>
        /// Create a JSONData which is Dictionary
        /// </summary>
        public static JSONData NewDictionary()
        {
            JSONData data = new JSONData();
            data.dictValue = new Dictionary<string, JSONData>();
            data.dataType = DataType.DataTypeDictionary;
            return data;
        }
        /// <summary>
        /// Create a JSONData which is List
        /// </summary>
        public static JSONData NewList()
        {
            JSONData data = new JSONData();
            data.listValue = new List<JSONData>();
            data.dataType = DataType.DataTypeList;
            return data;
        }
        static Dictionary<Type, MethodInfo> mMethodInfos = null;
        /// <summary>
        /// Get implicit function by Type
        /// </summary>
        /// <param name="type">Specific type</param>
        /// <returns>Method infomation</returns>
        public static MethodInfo GetImplicit(Type type)
        {
            if (type == null)
                return null;
            if (mMethodInfos == null)
            {
                mMethodInfos = new Dictionary<Type, MethodInfo>();
                foreach (MethodInfo mi in typeof(JSONData).GetMethods())
                {
                    if (mi.Name == "op_Implicit")
                        mMethodInfos[mi.ReturnType] = mi;
                }
            }
            return mMethodInfos[type];
        }
        /// <summary>
        /// the value of this JSONData
        /// </summary>
        public object Value
        {
            get
            {
                switch (dataType)
                {
                    case DataType.DataTypeDictionary: return dictValue;
                    case DataType.DataTypeList: return listValue;
                    case DataType.DataTypeString: return strValue;
                    case DataType.DataTypeBoolean: return bValue;
                    case DataType.DataTypeInt: return iValue;
                    case DataType.DataTypeLong: return lValue;
                    case DataType.DataTypeULong: return ulValue;
                    case DataType.DataTypeDouble: return dValue;
                    case DataType.DataTypeBooleanNullable: return bValueNullable;
                    case DataType.DataTypeIntNullable: return iValueNullable;
                    case DataType.DataTypeLongNullable: return lValueNullable;
                    case DataType.DataTypeULongNullable: return ulValueNullable;
                    case DataType.DataTypeDoubleNullable: return dValueNullable;
                    default: return null;
                }
            }
        }
        /// <summary>
        /// the type of this JSONData
        /// </summary>
        public Type ValueType
        {
            get
            {
                switch (dataType)
                {
                    case DataType.DataTypeDictionary: return typeof(Dictionary<string, JSONData>);
                    case DataType.DataTypeList: return typeof(List<JSONData>);
                    case DataType.DataTypeString: return typeof(string);
                    case DataType.DataTypeBoolean: return typeof(bool);
                    case DataType.DataTypeInt: return typeof(int);
                    case DataType.DataTypeLong: return typeof(long);
                    case DataType.DataTypeULong: return typeof(ulong);
                    case DataType.DataTypeDouble: return typeof(double);
                    case DataType.DataTypeBooleanNullable: return typeof(bool?);
                    case DataType.DataTypeIntNullable: return typeof(int?);
                    case DataType.DataTypeLongNullable: return typeof(long?);
                    case DataType.DataTypeULongNullable: return typeof(ulong?);
                    case DataType.DataTypeDoubleNullable: return typeof(double?);
                    default: return null;
                }
            }
        }
        /// <summary>
        /// Get a deep clone JSONData object
        /// </summary>
        /// <param name="sourceJsonData">The source JSONData object</param>
        public static JSONData DeepClone(JSONData sourceJsonData)
        {
            if (sourceJsonData == null)
                return null;
            switch (sourceJsonData.dataType)
            {
                case DataType.DataTypeDictionary:
                    {
                        JSONData json = NewDictionary();
                        foreach (var one in sourceJsonData.Value as Dictionary<string, JSONData>)
                        {
                            json.Add(one.Key, DeepClone(one.Value));
                        }
                        return json;
                    }
                case DataType.DataTypeList:
                    {
                        JSONData json = NewList();
                        foreach (JSONData one in sourceJsonData.Value as List<JSONData>)
                        {
                            json.Add(DeepClone(one));
                        }
                        return json;
                    }
                case DataType.DataTypeString: return sourceJsonData.strValue;
                case DataType.DataTypeBoolean: return sourceJsonData.bValue;
                case DataType.DataTypeInt: return sourceJsonData.iValue;
                case DataType.DataTypeLong: return sourceJsonData.lValue;
                case DataType.DataTypeULong: return sourceJsonData.ulValue;
                case DataType.DataTypeDouble: return sourceJsonData.dValue;
                case DataType.DataTypeBooleanNullable: return sourceJsonData.bValueNullable;
                case DataType.DataTypeIntNullable: return sourceJsonData.iValueNullable;
                case DataType.DataTypeLongNullable: return sourceJsonData.ulValueNullable;
                case DataType.DataTypeULongNullable: return sourceJsonData.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return sourceJsonData.dValueNullable;
                default: return null;
            }
        }
        /// <summary>
        /// Throw exception when should throw exception, otherwise will return default or ignore action and with no exception. 
        /// Default value is false.
        /// </summary>
        public static bool ThrowException = false;
        /// <summary>
        /// get/set item of this(this is a Dictionary),Same with Dictionary.this[]
        /// </summary>
        public JSONData this[string key]
        {
            get
            {
                if (dataType == DataType.DataTypeDictionary && dictValue != null)
                {
                    if (dictValue.TryGetValue(key, out JSONData value))
                        return value;
                    if (ThrowException) throw new Exception(ToString() + " !dictValue.ContainsKey(" + key + ")");
                    return null;
                }
                if (ThrowException) throw new Exception(ToString() + " not a dictionary");
                return null;
            }
            set
            {
                if (dataType == DataType.DataTypeDictionary && dictValue != null)
                    dictValue[key] = value;
                else if (ThrowException) throw new Exception(ToString() + " not a dictionary");
            }
        }
        /// <summary>
        /// get/set item of this(this is a List),Same with List.this[]
        /// </summary>
        public JSONData this[int index]
        {
            get
            {
                if (dataType == DataType.DataTypeList && listValue != null)
                {
                    if (index >= listValue.Count)
                    {
                        if (ThrowException) throw new Exception(ToString() + " index >= listValue.Count");
                        return null;
                    }
                    return listValue[index];
                }
                if (ThrowException) throw new Exception(ToString() + " not a list");
                return null;
            }
            set
            {
                if (dataType == DataType.DataTypeList && listValue != null)
                {
                    if (index >= listValue.Count)
                    {
                        if (ThrowException) throw new Exception(ToString() + " index >= listValue.Count");
                        return;
                    }
                    listValue[index] = value;
                }
                else if (ThrowException) throw new Exception(ToString() + " not a list");
            }
        }
        /// <summary>
        /// Add an JSONData to this(this is a List).Same with List.Add().
        /// </summary>
        public void Add(JSONData item)
        {
            if (dataType == DataType.DataTypeList && listValue != null)
                listValue.Add(item);
        }
        /// <summary>
        /// Insert item to this(this is a List),Same with List.Insert().
        /// </summary>
        public void Insert(int index, JSONData item)
        {
            if (dataType == DataType.DataTypeList && listValue != null)
                listValue.Insert(index, item);
        }
        /// <summary>
        /// Remove item from this(this is a List),Same with List.Remove().
        /// </summary>
        public void Remove(JSONData item)
        {
            if (dataType == DataType.DataTypeList && listValue != null)
                listValue.Remove(item);
            else if (dataType == DataType.DataTypeDictionary && dictValue != null)
                dictValue.Remove(item);
        }
        /// <summary>
        /// Remove item from this by index(this is a List),Same with List.RemoveAt().
        /// </summary>
        public void RemoveAt(int index)
        {
            if (dataType == DataType.DataTypeList && listValue != null)
                listValue.RemoveAt(index);
        }
        /// <summary>
        /// Reverse item of this(this is a List),Same with List.Reverse().
        /// </summary>
        public void Reverse()
        {
            if (dataType == DataType.DataTypeList && listValue != null)
                listValue.Reverse();
        }
        /// <summary>
        /// Check this whether contains item(this is a List),Same with List.Contains().
        /// </summary>
        public bool Contains(JSONData item)
        {
            return dataType == DataType.DataTypeList && listValue != null && listValue.Contains(item);
        }
        /// <summary>
        /// item count of this(this is a List or Dictionary),Same with List.Count or Dictionary.Count
        /// </summary>
        public int Count
        {
            get
            {
                if (dataType == DataType.DataTypeList && listValue != null)
                    return listValue.Count;
                else if (dataType == DataType.DataTypeDictionary && dictValue != null)
                    return dictValue.Count;
                return 0;
            }
        }
        /// <summary>
        /// clear item of this(this is a List or Dictionary),Same with List.Clear() or Dictionary.Clear()
        /// </summary>
        public void Clear()
        {
            if (dataType == DataType.DataTypeList && listValue != null)
                listValue.Clear();
            else if (dataType == DataType.DataTypeDictionary && dictValue != null)
                dictValue.Clear();
        }
        /// <summary>
        /// add item to this(this is a Dictionary),Same with Dictionary.Add()
        /// </summary>
        public void Add(string key, JSONData value)
        {
            if (dataType == DataType.DataTypeDictionary && dictValue != null)
                dictValue.Add(key, value);
        }
        /// <summary>
        /// remove item from this(this is a Dictionary),Same with Dictionary.RemoveKey()
        /// </summary>
        public void RemoveKey(string key)
        {
            if (dataType == DataType.DataTypeDictionary && dictValue != null)
                dictValue.Remove(key);
        }
        /// <summary>
        /// Check this whether contains key(this is a Dictionary),Same with Dictionary.ContainsKey()
        /// </summary>
        public bool ContainsKey(string key)
        {
            return dataType == DataType.DataTypeDictionary && dictValue != null && dictValue.ContainsKey(key);
        }
        /// <summary>
        /// Check this whether contains value(this is a Dictionary),Same with Dictionary.ContainsValue()
        /// </summary>
        public bool ContainsValue(JSONData value)
        {
            return dataType == DataType.DataTypeDictionary && dictValue != null && dictValue.ContainsValue(value);
        }
        /// <summary>
        /// try get value from this(this is a Dictionary),Same with Dictionary.TryGetValue()
        /// </summary>
        public bool TryGetValue(string key, out JSONData value)
        {
            if (dataType == DataType.DataTypeDictionary && dictValue != null)
                return dictValue.TryGetValue(key, out value);
            value = new JSONData();
            value.dataType = DataType.DataTypeNull;
            return false;
        }
        /// <summary>
        /// The data type of this object 
        /// </summary>
        public DataType dataType;
        /// <summary>
        /// Get extern object from this JSONData
        /// </summary>
        public object GetObjectExtern(string key)
        {
            if (externObjects != null)
            {
                object obj;
                if (externObjects.TryGetValue(key, out obj))
                    return obj;
            }
            return null;
        }
        /// <summary>
        /// Set extern object to this JSONData
        /// </summary>
        public void SetObjectExtern(string key, object obj)
        {
            if (externObjects == null)
                externObjects = new Dictionary<string, object>();
            externObjects[key] = obj;
        }
        #region PrivateImp
        private List<JSONData> listValue;
        private Dictionary<string, JSONData> dictValue;
        private string strValue;
        private bool bValue;
        private int iValue;
        private long lValue;
        private ulong ulValue;
        private double dValue;
        private bool? bValueNullable;
        private int? iValueNullable;
        private long? lValueNullable;
        private ulong? ulValueNullable;
        private double? dValueNullable;
        private Dictionary<string, object> externObjects;

        /// <summary>
        /// Convert this JSONData to string no deal with special string '\' '"' '\n' '\r' '\t' '\b' '\f'. 
        /// They are will error if have secial string in it, you should use ToJson() instead.
        /// </summary>
        public override string ToString()
        {
            switch (dataType)
            {
                case DataType.DataTypeDictionary:
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("{");
                        foreach (var item in dictValue)
                        {
                            if (item.Value == null)
                                sb.AppendFormat("\"{0}\":null,", item.Key);
                            else if (item.Value.dataType == DataType.DataTypeString)
                            {
                                if (item.Value == null)
                                    sb.AppendFormat("\"{0}\":null,", item.Key);
                                else
                                    sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, item.Value.ToString());
                            }
                            else
                                sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value.ToString());
                        }
                        if (sb.Length > 1)
                            sb.Remove(sb.Length - 1, 1);
                        sb.Append("}");
                        return sb.ToString();
                    }
                case DataType.DataTypeList:
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[");
                        foreach (var item in listValue)
                        {
                            if (item == null)
                                sb.Append("null,");
                            else if (item.dataType == DataType.DataTypeString)
                                sb.AppendFormat("\"{0}\",", item.ToString());
                            else
                            {
                                sb.Append(item.ToString());
                                sb.Append(",");
                            }
                        }
                        if (sb.Length > 1)
                            sb.Remove(sb.Length - 1, 1);
                        sb.Append("]");
                        return sb.ToString();
                    }
                case DataType.DataTypeBoolean:
                    return bValue ? "true" : "false";
                case DataType.DataTypeString:
                    return strValue;
                case DataType.DataTypeInt:
                    return iValue.ToString();
                case DataType.DataTypeLong:
                    return lValue.ToString();
                case DataType.DataTypeULong:
                    return ulValue.ToString();
                case DataType.DataTypeDouble:
                    return dValue.ToString();
                case DataType.DataTypeBooleanNullable:
                    return bValueNullable == null ? "null" : (bValueNullable.Value ? "true" : "false");
                case DataType.DataTypeIntNullable:
                    return iValueNullable == null ? "null" : iValueNullable.Value.ToString();
                case DataType.DataTypeLongNullable:
                    return lValueNullable == null ? "null" : lValueNullable.Value.ToString();
                case DataType.DataTypeULongNullable:
                    return ulValueNullable == null ? "null" : ulValueNullable.Value.ToString();
                case DataType.DataTypeDoubleNullable:
                    return dValueNullable == null ? "null" : dValueNullable.Value.ToString();
                default:
                    return "null";
            }
        }
        static string[] spaces = {
            "",
            "    ",
            "        ",
            "            ",
            "                ",
            "                    ",
            "                        ",
            "                            ",
            "                                ",
            "                                    ",
            "                                        ",
            "                                            ",
            "                                                ",
            "                                                    ",
            "                                                        ",
            "                                                            ",
            "                                                                ",
            "                                                                    ",
            "                                                                        ",
            "                                                                            ",
            "                                                                                "};
        /// <summary>
        /// Convert this JSONData to JSON string had deal with special string '\' '"' '\n' '\r' '\t' '\b' '\f'. 
        /// </summary>
        /// <param name="bFormat">Format the JSON string, make it more readability. default is false</param>
        /// <param name="depth">Internal use, you can ignore it</param>
        /// <returns></returns>
        public string ToJson(bool bFormat = false, int depth = 0)
        {
            if (bFormat)
            {
                switch (dataType)
                {
                    case DataType.DataTypeDictionary:
                        {
                            string space;
                            if (depth < 0)
                                space = spaces[0];
                            else if (depth > 20)
                                space = spaces[20];
                            else
                                space = spaces[depth];
                            StringBuilder sb = new StringBuilder();
                            sb.Append("{\n");
                            foreach (var item in dictValue)
                            {
                                if (item.Value == null)
                                    sb.Append($"{space}    \"{item.Key}\": null,\n");
                                else if (item.Value.dataType == DataType.DataTypeString)
                                {
                                    if (item.Value == null)
                                        sb.Append($"{space}    \"{item.Key}\": null,\n");
                                    else
                                        sb.Append($"{space}    \"{item.Key}\": \"{item.Value.ToJson(bFormat, depth + 1)}\",\n");
                                }
                                else
                                    sb.Append($"{space}    \"{item.Key}\": {item.Value.ToJson(bFormat, depth + 1)},\n");
                            }
                            if (dictValue.Count > 0)
                                sb.Remove(sb.Length - 2, 2);
                            sb.Append("\n");
                            sb.Append(space);
                            sb.Append("}");
                            return sb.ToString();
                        }
                    case DataType.DataTypeList:
                        {
                            string space;
                            if (depth < 0)
                                space = spaces[0];
                            else if (depth > 20)
                                space = spaces[20];
                            else
                                space = spaces[depth];
                            StringBuilder sb = new StringBuilder();
                            sb.Append("[\n");
                            foreach (var item in listValue)
                            {
                                if (item == null)
                                    sb.Append($"{space}    null,\n");
                                else if (item.dataType == DataType.DataTypeString)
                                    sb.Append($"{space}    \"{item.ToJson(bFormat, depth + 1)}\",\n");
                                else
                                {
                                    sb.Append(space);
                                    sb.Append("    ");
                                    sb.Append(item.ToJson(bFormat, depth + 1));
                                    sb.Append(",\n");
                                }
                            }
                            if (listValue.Count > 0)
                                sb.Remove(sb.Length - 2, 2);
                            sb.Append("\n");
                            sb.Append(space);
                            sb.Append("]");
                            return sb.ToString();
                        }
                    case DataType.DataTypeBoolean:
                        return bValue ? "true" : "false";
                    case DataType.DataTypeString:
                        {
                            if (strValue == null)
                                return "null";
                            StringBuilder sb = new StringBuilder(strValue.Length * 2);
                            char[] buff = strValue.ToCharArray();
                            int length = buff.Length;
                            char c;
                            for (int i = 0; i < length; i++)
                            {
                                c = buff[i];
                                switch (c)
                                {
                                    case '\"': sb.Append("\\\""); break;
                                    case '\n': sb.Append("\\\n"); break;
                                    case '\r': sb.Append("\\\r"); break;
                                    case '\b': sb.Append("\\\b"); break;
                                    case '\t': sb.Append("\\\t"); break;
                                    case '\f': sb.Append("\\\f"); break;
                                    case '\\': sb.Append("\\\\"); break;
                                    default: sb.Append(c); break;
                                }
                            }
                            return sb.ToString();
                        }
                    case DataType.DataTypeInt:
                        return iValue.ToString();
                    case DataType.DataTypeLong:
                        return lValue.ToString();
                    case DataType.DataTypeULong:
                        return ulValue.ToString();
                    case DataType.DataTypeDouble:
                        return dValue.ToString();
                    case DataType.DataTypeBooleanNullable:
                        return bValueNullable == null ? "null" : (bValueNullable.Value ? "true" : "false");
                    case DataType.DataTypeIntNullable:
                        return iValueNullable == null ? "null" : iValueNullable.Value.ToString();
                    case DataType.DataTypeLongNullable:
                        return lValueNullable == null ? "null" : lValueNullable.Value.ToString();
                    case DataType.DataTypeULongNullable:
                        return ulValueNullable == null ? "null" : ulValueNullable.Value.ToString();
                    case DataType.DataTypeDoubleNullable:
                        return dValueNullable == null ? "null" : dValueNullable.Value.ToString();
                    default:
                        return "null";
                }
            }
            else
            {

                switch (dataType)
                {
                    case DataType.DataTypeDictionary:
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("{");
                            foreach (var item in dictValue)
                            {
                                if (item.Value == null)
                                    sb.AppendFormat("\"{0}\":null,", item.Key);
                                else if (item.Value.dataType == DataType.DataTypeString)
                                {
                                    if (item.Value == null)
                                        sb.AppendFormat("\"{0}\":null,", item.Key);
                                    else
                                        sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, item.Value.ToJson());
                                }
                                else
                                    sb.AppendFormat("\"{0}\":{1},", item.Key, item.Value.ToJson());
                            }
                            if (sb.Length > 1)
                                sb.Remove(sb.Length - 1, 1);
                            sb.Append("}");
                            return sb.ToString();
                        }
                    case DataType.DataTypeList:
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("[");
                            foreach (var item in listValue)
                            {
                                if (item == null)
                                    sb.Append("null,");
                                else if (item.dataType == DataType.DataTypeString)
                                    sb.AppendFormat("\"{0}\",", item.ToJson());
                                else
                                {
                                    sb.Append(item.ToJson());
                                    sb.Append(",");
                                }
                            }
                            if (sb.Length > 1)
                                sb.Remove(sb.Length - 1, 1);
                            sb.Append("]");
                            return sb.ToString();
                        }
                    case DataType.DataTypeBoolean:
                        return bValue ? "true" : "false";
                    case DataType.DataTypeString:
                        {
                            if (strValue == null)
                                return "null";
                            StringBuilder sb = new StringBuilder(strValue.Length * 2);
                            char[] buff = strValue.ToCharArray();
                            int length = buff.Length;
                            char c;
                            for (int i = 0; i < length; i++)
                            {
                                c = buff[i];
                                switch (c)
                                {
                                    case '\"': sb.Append("\\\""); break;
                                    case '\n': sb.Append("\\\n"); break;
                                    case '\r': sb.Append("\\\r"); break;
                                    case '\b': sb.Append("\\\b"); break;
                                    case '\t': sb.Append("\\\t"); break;
                                    case '\f': sb.Append("\\\f"); break;
                                    case '\\': sb.Append("\\\\"); break;
                                    default: sb.Append(c); break;
                                }
                            }
                            return sb.ToString();
                        }
                    case DataType.DataTypeInt:
                        return iValue.ToString();
                    case DataType.DataTypeLong:
                        return lValue.ToString();
                    case DataType.DataTypeULong:
                        return ulValue.ToString();
                    case DataType.DataTypeDouble:
                        return dValue.ToString();
                    case DataType.DataTypeBooleanNullable:
                        return bValueNullable == null ? "null" : (bValueNullable.Value ? "true" : "false");
                    case DataType.DataTypeIntNullable:
                        return iValueNullable == null ? "null" : iValueNullable.Value.ToString();
                    case DataType.DataTypeLongNullable:
                        return lValueNullable == null ? "null" : lValueNullable.Value.ToString();
                    case DataType.DataTypeULongNullable:
                        return ulValueNullable == null ? "null" : ulValueNullable.Value.ToString();
                    case DataType.DataTypeDoubleNullable:
                        return dValueNullable == null ? "null" : dValueNullable.Value.ToString();
                    default:
                        return "null";
                }
            }
        }


        /// <summary>
        /// object Convert To JSONData.
        /// It's suppose as internal use.
        /// </summary>
        /// <param name="obj">only support byte/sbyte/short/ushort/int/uint/long/ulong/string/bool/byte?/sbyte?/short?/ushort?/int?/uint?/long?/ulong?/bool?</param>
        /// <returns></returns>
        public static JSONData ConvertTo(object obj)
        {
            if (obj == null)
                return null;
            switch (obj.GetType().Name)
            {
                case "Byte": return (byte)obj;
                case "SByte": return (sbyte)obj;
                case "Int16": return (short)obj;
                case "UInt16": return (ushort)obj;
                case "Int32": return (int)obj;
                case "UInt32": return (uint)obj;
                case "Int64": return (long)obj;
                case "UInt64": return (ulong)obj;
                case "Single": return (float)obj;
                case "Double": return (double)obj;
                case "String": return (string)obj;
                case "Boolean": return (bool)obj;
                case "DateTime": return (DateTime)obj;
                default:
                    {
                        string fullName = obj.GetType().FullName;
                        if (KissJson.RemoveNullable(ref fullName))
                        {
                            switch (obj.GetType().Name)
                            {
                                case "System.Byte": return (byte?)obj;
                                case "System.SByte": return (sbyte?)obj;
                                case "System.Int16": return (short?)obj;
                                case "System.UInt16": return (ushort?)obj;
                                case "System.Int32": return (int?)obj;
                                case "System.UInt32": return (uint?)obj;
                                case "System.Single": return (float?)obj;
                                case "System.Double": return (double?)obj;
                                case "System.Boolean": return (bool?)obj;
                                case "System.DateTime": return (DateTime?)obj;
                                case "System.Int64": return (long?)obj;
                                case "System.UInt64": return (ulong?)obj;
                            }
                        }
                    }
                    break;
            }
            if (ThrowException) throw new Exception("JSONData ConventTo(object obj) only support byte/sbyte/short/ushort/int/uint/long/ulong/string/bool/DateTime(or with Nullable type '?')." + obj.ToString());
            return null;
        }
        /// <summary>
        /// JSONData Convert To target object.
        /// It's suppose as internal use.
        /// </summary>
        /// <param name="targetType">The target Type</param>
        /// <param name="obj">JSONData</param>
        /// <returns>only support byte/sbyte/short/ushort/int/uint/long/ulong/string/bool/byte?/sbyte?/short?/ushort?/int?/uint?/long?/ulong?/bool?</returns>
        public static object ConvertTo(Type targetType, JSONData obj)
        {
            if (obj == null || obj.dataType == DataType.DataTypeNull)
                return null;
            return Convert.ChangeType(obj, targetType, null);
            //return ConvertObject(obj.Value, targetType);
        }


        static object ConvertObject(object obj, Type type)
        {
            if (type == null) return obj;
            if (obj == null) return type.IsValueType ? Activator.CreateInstance(type) : null;

            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (type.IsAssignableFrom(obj.GetType()))
            {
                return obj;
            }
            else if ((underlyingType ?? type).IsEnum) // 如果待转换的对象的基类型为枚举
            {
                if (underlyingType != null && string.IsNullOrEmpty(obj.ToString())) // 如果目标类型为可空枚举，并且待转换对象为null 则直接返回null值
                {
                    return null;
                }
                else
                {
                    return Enum.Parse(underlyingType ?? type, obj.ToString());
                }
            }
            else if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? type)) // 如果目标类型的基类型实现了IConvertible，则直接转换
            {
                try
                {
                    return Convert.ChangeType(obj, underlyingType ?? type, null);
                }
                catch
                {
                    return underlyingType == null ? Activator.CreateInstance(type) : null;
                }
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                if (converter.CanConvertFrom(obj.GetType()))
                {
                    return converter.ConvertFrom(obj);
                }
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    object o = constructor.Invoke(null);
                    PropertyInfo[] propertys = type.GetProperties();
                    Type oldType = obj.GetType();
                    foreach (PropertyInfo property in propertys)
                    {
                        PropertyInfo p = oldType.GetProperty(property.Name);
                        if (property.CanWrite && p != null && p.CanRead)
                        {
                            property.SetValue(o, ConvertObject(p.GetValue(obj, null), property.PropertyType), null);
                        }
                    }
                    return o;
                }
            }
            return obj;
        }
        /// <summary>
        /// Implicit convert from List&lt;byte&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<byte> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;byte&gt;
        /// </summary>
        public static implicit operator List<byte>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<byte> data = new List<byte>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;sbyte&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<sbyte> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;sbyte&gt;
        /// </summary>
        public static implicit operator List<sbyte>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<sbyte> data = new List<sbyte>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;short&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<short> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;short&gt;
        /// </summary>
        public static implicit operator List<short>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<short> data = new List<short>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;ushort&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<ushort> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;ushort&gt;
        /// </summary>
        public static implicit operator List<ushort>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<ushort> data = new List<ushort>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;int&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<int> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;int&gt;
        /// </summary>
        public static implicit operator List<int>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<int> data = new List<int>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;uint&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<uint> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;uint&gt;
        /// </summary>
        public static implicit operator List<uint>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<uint> data = new List<uint>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;long&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<long> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;long&gt;
        /// </summary>
        public static implicit operator List<long>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<long> data = new List<long>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;ulong&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<ulong> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;ulong&gt;
        /// </summary>
        public static implicit operator List<ulong>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<ulong> data = new List<ulong>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;float&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<float> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;float&gt;
        /// </summary>
        public static implicit operator List<float>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<float> data = new List<float>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;double&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<double> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;double&gt;
        /// </summary>
        public static implicit operator List<double>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<double> data = new List<double>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;bool&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<bool> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;bool&gt;
        /// </summary>
        public static implicit operator List<bool>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<bool> data = new List<bool>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;char&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<char> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;char&gt;
        /// </summary>
        public static implicit operator List<char>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<char> data = new List<char>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;string&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<string> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;string&gt;
        /// </summary>
        public static implicit operator List<string>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<string> data = new List<string>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;JSONData&gt;
        /// </summary>
        public static implicit operator List<JSONData>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<JSONData> data = new List<JSONData>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;byte?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<byte?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;byte?&gt;
        /// </summary>
        public static implicit operator List<byte?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<byte?> data = new List<byte?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;sbyte?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<sbyte?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;sbyte?&gt;
        /// </summary>
        public static implicit operator List<sbyte?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<sbyte?> data = new List<sbyte?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;short?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<short?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;short?&gt;
        /// </summary>
        public static implicit operator List<short?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<short?> data = new List<short?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;ushort?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<ushort?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;ushort?&gt;
        /// </summary>
        public static implicit operator List<ushort?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<ushort?> data = new List<ushort?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;int?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<int?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;int?&gt;
        /// </summary>
        public static implicit operator List<int?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<int?> data = new List<int?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;uint?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<uint?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;uint?&gt;
        /// </summary>
        public static implicit operator List<uint?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<uint?> data = new List<uint?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;long?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<long?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;long?&gt;
        /// </summary>
        public static implicit operator List<long?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<long?> data = new List<long?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;ulong?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<ulong?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;ulong?&gt;
        /// </summary>
        public static implicit operator List<ulong?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<ulong?> data = new List<ulong?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;float?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<float?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;float?&gt;
        /// </summary>
        public static implicit operator List<float?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<float?> data = new List<float?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;double?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<double?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;double?&gt;
        /// </summary>
        public static implicit operator List<double?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<double?> data = new List<double?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;bool?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<bool?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;bool?&gt;
        /// </summary>
        public static implicit operator List<bool?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<bool?> data = new List<bool?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;char?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(List<char?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeList;
            data.listValue = new List<JSONData>();
            foreach (var item in value)
                data.listValue.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to List&lt;char?&gt;
        /// </summary>
        public static implicit operator List<char?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeList)
                return null;
            List<char?> data = new List<char?>();
            foreach (var item in value.listValue)
                data.Add(item);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, bool&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, bool> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, bool&gt;
        /// </summary>
        public static implicit operator Dictionary<string, bool>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, bool> data = new Dictionary<string, bool>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, byte&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, byte> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, byte&gt;
        /// </summary>
        public static implicit operator Dictionary<string, byte>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, byte> data = new Dictionary<string, byte>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, sbyte&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, sbyte> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, sbyte&gt;
        /// </summary>
        public static implicit operator Dictionary<string, sbyte>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, sbyte> data = new Dictionary<string, sbyte>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, short&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, short> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, short&gt;
        /// </summary>
        public static implicit operator Dictionary<string, short>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, short> data = new Dictionary<string, short>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, ushort&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, ushort> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, ushort&gt;
        /// </summary>
        public static implicit operator Dictionary<string, ushort>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, ushort> data = new Dictionary<string, ushort>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, int&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, int> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, int&gt;
        /// </summary>
        public static implicit operator Dictionary<string, int>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, int> data = new Dictionary<string, int>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, uint&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, uint> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, uint&gt;
        /// </summary>
        public static implicit operator Dictionary<string, uint>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, uint> data = new Dictionary<string, uint>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, long&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, long> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, long&gt;
        /// </summary>
        public static implicit operator Dictionary<string, long>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, long> data = new Dictionary<string, long>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, ulong&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, ulong> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, ulong&gt;
        /// </summary>
        public static implicit operator Dictionary<string, ulong>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, ulong> data = new Dictionary<string, ulong>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, float&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, float> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, float&gt;
        /// </summary>
        public static implicit operator Dictionary<string, float>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, float> data = new Dictionary<string, float>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, double&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, double> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, double&gt;
        /// </summary>
        public static implicit operator Dictionary<string, double>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, double> data = new Dictionary<string, double>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, string&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, string> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, string&gt;
        /// </summary>
        public static implicit operator Dictionary<string, string>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, bool?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, bool?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, bool?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, bool?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, bool?> data = new Dictionary<string, bool?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, byte?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, byte?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, byte?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, byte?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, byte?> data = new Dictionary<string, byte?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, sbyte?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, sbyte?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, sbyte?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, sbyte?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, sbyte?> data = new Dictionary<string, sbyte?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, short?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, short?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, short?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, short?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, short?> data = new Dictionary<string, short?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, ushort?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, ushort?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, ushort?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, ushort?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, ushort?> data = new Dictionary<string, ushort?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, int?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, int?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, int?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, int?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, int?> data = new Dictionary<string, int?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, uint?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, uint?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, uint?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, uint?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, uint?> data = new Dictionary<string, uint?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, long?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, long?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, long?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, long?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, long?> data = new Dictionary<string, long?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, ulong?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, ulong?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, ulong?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, ulong?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, ulong?> data = new Dictionary<string, ulong?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, float?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, float?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, float?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, float?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, float?> data = new Dictionary<string, float?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, double?&gt; to JSONData;
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, double?> value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDictionary;
            data.dictValue = new Dictionary<string, JSONData>();
            foreach (var item in value)
                data.dictValue.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, double?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, double?>(JSONData value)
        {
            if (value == null || value.dataType != DataType.DataTypeDictionary)
                return null;
            Dictionary<string, double?> data = new Dictionary<string, double?>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value);
            return data;
        }
        /// <summary>
        /// Implicit convert from byte to JSONData;
        /// </summary>
        public static implicit operator JSONData(byte value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeInt;
            data.iValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to byte
        /// </summary>
        public static implicit operator byte(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to byte");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (byte)value.iValue;
                case DataType.DataTypeLong: return (byte)value.lValue;
                case DataType.DataTypeULong: return (byte)value.ulValue;
                case DataType.DataTypeDouble: return (byte)value.dValue;
                case DataType.DataTypeIntNullable: return (byte)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (byte)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (byte)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (byte)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToByte(value.strValue);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to byte");
            return default;
        }
        /// <summary>
        /// Implicit convert from byte? to JSONData
        /// </summary>
        public static implicit operator JSONData(byte? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeIntNullable;
            data.iValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to byte?
        /// </summary>
        public static implicit operator byte?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (byte?)value.iValue;
                case DataType.DataTypeLong: return (byte?)value.lValue;
                case DataType.DataTypeULong: return (byte?)value.ulValue;
                case DataType.DataTypeDouble: return (byte?)value.dValue;
                case DataType.DataTypeIntNullable: return (byte?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (byte?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (byte?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (byte?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToByte(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to byte?");
            return default;
        }
        /// <summary>
        /// Implicit convert from sbyte to JSONData
        /// </summary>
        public static implicit operator JSONData(sbyte value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeInt;
            data.iValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to sbyte
        /// </summary>
        public static implicit operator sbyte(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to sbyte");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (sbyte)value.iValue;
                case DataType.DataTypeLong: return (sbyte)value.lValue;
                case DataType.DataTypeULong: return (sbyte)value.ulValue;
                case DataType.DataTypeDouble: return (sbyte)value.dValue;
                case DataType.DataTypeIntNullable: return (sbyte)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (sbyte)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (sbyte)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (sbyte)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToSByte(value.strValue);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to sbyte");
            return default;
        }
        /// <summary>
        /// Implicit convert from JSONData to sbyte?
        /// </summary>
        public static implicit operator sbyte?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (sbyte?)value.iValue;
                case DataType.DataTypeLong: return (sbyte?)value.lValue;
                case DataType.DataTypeULong: return (sbyte?)value.ulValue;
                case DataType.DataTypeDouble: return (sbyte?)value.dValue;
                case DataType.DataTypeIntNullable: return (sbyte?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (sbyte?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (sbyte?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (sbyte?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToSByte(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to sbyte?");
            return default;
        }
        /// <summary>
        /// Implicit convert from sbyte? to JSONData
        /// </summary>
        public static implicit operator JSONData(sbyte? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeIntNullable;
            data.iValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from short to JSONData
        /// </summary>
        public static implicit operator JSONData(short value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeInt;
            data.iValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to short
        /// </summary>
        public static implicit operator short(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to short");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (short)value.iValue;
                case DataType.DataTypeLong: return (short)value.lValue;
                case DataType.DataTypeULong: return (short)value.ulValue;
                case DataType.DataTypeDouble: return (short)value.dValue;
                case DataType.DataTypeIntNullable: return (short)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (short)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (short)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (short)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToInt16(value.strValue);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to short");
            return default;
        }
        /// <summary>
        /// Implicit convert from ushort to JSONData
        /// </summary>
        public static implicit operator JSONData(ushort value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeInt;
            data.iValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to ushort
        /// </summary>
        public static implicit operator ushort(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to ushort");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (ushort)value.iValue;
                case DataType.DataTypeLong: return (ushort)value.lValue;
                case DataType.DataTypeULong: return (ushort)value.ulValue;
                case DataType.DataTypeDouble: return (ushort)value.dValue;
                case DataType.DataTypeIntNullable: return (ushort)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (ushort)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (ushort)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (ushort)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToUInt16(value.strValue);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to ushort");
            return default;
        }
        /// <summary>
        /// Implicit convert from DateTime to JSONData
        /// </summary>
        public static implicit operator JSONData(DateTime value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeString;
            data.strValue = Convert.ToString(value, KissJson.CultureForConvertDateTime);
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to DateTime
        /// </summary>
        public static implicit operator DateTime(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to DateTime");
                return default;
            }
            if (value.dataType == DataType.DataTypeString)
            {
                if (value.strValue == null)
                    return new DateTime(1970, 1, 1);
                return Convert.ToDateTime(value.strValue, KissJson.CultureForConvertDateTime);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to DateTime");
            return default;
        }
        /// <summary>
        /// Implicit convert from DateTime? to JSONData
        /// </summary>
        public static implicit operator JSONData(DateTime? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeString;
            data.strValue = value != null ? Convert.ToString(value.Value, KissJson.CultureForConvertDateTime) : null;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to DateTime?
        /// </summary>
        public static implicit operator DateTime?(JSONData value)
        {
            if (value == null)
                return null;
            if (value.dataType == DataType.DataTypeString)
            {
                if (value.strValue == null)
                    return null;
                return Convert.ToDateTime(value.strValue, KissJson.CultureForConvertDateTime);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to DateTime");
            return default;
        }
        /// <summary>
        /// Implicit convert from int to JSONData
        /// </summary>
        public static implicit operator JSONData(int value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeInt;
            data.iValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to int
        /// </summary>
        public static implicit operator int(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to int");
                return default;

            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return value.iValue;
                case DataType.DataTypeLong: return (int)value.lValue;
                case DataType.DataTypeULong: return (int)value.ulValue;
                case DataType.DataTypeDouble: return (int)value.dValue;
                case DataType.DataTypeIntNullable: return (int)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (int)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (int)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (int)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToInt32(value.strValue);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to int");
            return default;
        }
        /// <summary>
        /// Implicit convert from uint to JSONData
        /// </summary>
        public static implicit operator JSONData(uint value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeLong;
            data.lValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to uint
        /// </summary>
        public static implicit operator uint(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to uint");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (uint)value.iValue;
                case DataType.DataTypeLong: return (uint)value.lValue;
                case DataType.DataTypeULong: return (uint)value.ulValue;
                case DataType.DataTypeDouble: return (uint)value.dValue;
                case DataType.DataTypeIntNullable: return (uint)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (uint)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (uint)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (uint)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToUInt32(value.strValue);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to uint");
            return default;
        }
        /// <summary>
        /// Implicit convert from long to JSONData
        /// </summary>
        public static implicit operator JSONData(long value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeLong;
            data.lValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to long
        /// </summary>
        public static implicit operator long(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to long");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return value.iValue;
                case DataType.DataTypeLong: return value.lValue;
                case DataType.DataTypeULong: return (long)value.ulValue;
                case DataType.DataTypeDouble: return (long)value.dValue;
                case DataType.DataTypeIntNullable: return (long)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (long)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (long)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (long)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToInt64(value.strValue);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to long");
            return default;
        }
        /// <summary>
        /// Implicit convert from ulong to JSONData
        /// </summary>
        public static implicit operator JSONData(ulong value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeULong;
            data.ulValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to ulong
        /// </summary>
        public static implicit operator ulong(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to ulong");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (ulong)value.iValue;
                case DataType.DataTypeLong: return (ulong)value.lValue;
                case DataType.DataTypeULong: return value.ulValue;
                case DataType.DataTypeDouble: return (ulong)value.dValue;
                case DataType.DataTypeIntNullable: return (ulong)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (ulong)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (ulong)value.lValueNullable;
                case DataType.DataTypeDoubleNullable: return (ulong)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToUInt64(value.strValue);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to ulong");
            return default;
        }
        /// <summary>
        /// Implicit convert from double to JSONData
        /// </summary>
        public static implicit operator JSONData(double value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDouble;
            data.dValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to double
        /// </summary>
        public static implicit operator double(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to double");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return value.iValue;
                case DataType.DataTypeLong: return value.lValue;
                case DataType.DataTypeULong: return value.ulValue;
                case DataType.DataTypeDouble: return value.dValue;
                case DataType.DataTypeIntNullable: return (double)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (double)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (double)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (double)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToDouble(value.strValue, KissJson.CultureForConvertFloatAndDouble);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to double");
            return default;
        }
        /// <summary>
        /// Implicit convert from float to JSONData
        /// </summary>
        public static implicit operator JSONData(float value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDouble;
            data.dValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to float
        /// </summary>
        public static implicit operator float(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to float");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return value.iValue;
                case DataType.DataTypeLong: return value.lValue;
                case DataType.DataTypeULong: return value.ulValue;
                case DataType.DataTypeDouble: return (float)value.dValue;
                case DataType.DataTypeIntNullable: return (float)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (float)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (float)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (float)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToSingle(value.strValue, KissJson.CultureForConvertFloatAndDouble);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to float");
            return default;
        }
        /// <summary>
        /// Implicit convert from char to JSONData
        /// </summary>
        public static implicit operator JSONData(char value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeString;
            data.strValue = value.ToString();
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to char
        /// </summary>
        public static implicit operator char(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to char");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (char)value.iValue;
                case DataType.DataTypeLong: return (char)value.lValue;
                case DataType.DataTypeULong: return (char)value.ulValue;
                case DataType.DataTypeDouble: return (char)value.dValue;
                case DataType.DataTypeIntNullable: return (char)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (char)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (char)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (char)value.dValueNullable;
                case DataType.DataTypeString: return Convert.ToChar(value.strValue);
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to char");
            return default;
        }
        /// <summary>
        /// Implicit convert from string to JSONData
        /// </summary>
        public static implicit operator JSONData(string value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeString;
            data.strValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to string
        /// </summary>
        public static implicit operator string(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeString: return value.strValue;
                case DataType.DataTypeInt: return value.iValue.ToString();
                case DataType.DataTypeLong: return value.lValue.ToString();
                case DataType.DataTypeULong: return value.ulValue.ToString();
                case DataType.DataTypeDouble: return value.dValue.ToString();
                case DataType.DataTypeBoolean: return value.bValue.ToString();
                case DataType.DataTypeIntNullable: return value.iValueNullable.ToString();
                case DataType.DataTypeLongNullable: return value.lValueNullable.ToString();
                case DataType.DataTypeULongNullable: return value.ulValueNullable.ToString();
                case DataType.DataTypeDoubleNullable: return value.dValueNullable.ToString();
                case DataType.DataTypeBooleanNullable: return value.bValueNullable.ToString();
            }
            return value.ToString();
        }
        /// <summary>
        /// Implicit convert from short? to JSONData
        /// </summary>
        public static implicit operator JSONData(short? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeIntNullable;
            data.iValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to short?
        /// </summary>
        public static implicit operator short?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (short?)value.iValue;
                case DataType.DataTypeLong: return (short?)value.lValue;
                case DataType.DataTypeULong: return (short?)value.ulValue;
                case DataType.DataTypeDouble: return (short?)value.dValue;
                case DataType.DataTypeIntNullable: return (short?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (short?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (short?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (short?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToInt16(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to short?");
            return default;
        }
        /// <summary>
        /// Implicit convert from ushort? to JSONData
        /// </summary>
        public static implicit operator JSONData(ushort? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeIntNullable;
            data.iValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to ushort?
        /// </summary>
        public static implicit operator ushort?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (ushort?)value.iValue;
                case DataType.DataTypeLong: return (ushort?)value.lValue;
                case DataType.DataTypeULong: return (ushort?)value.ulValue;
                case DataType.DataTypeDouble: return (ushort?)value.dValue;
                case DataType.DataTypeIntNullable: return (ushort?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (ushort?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (ushort?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (ushort?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToUInt16(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to ushort?");
            return null;
        }
        /// <summary>
        /// Implicit convert from int? to JSONData
        /// </summary>
        public static implicit operator JSONData(int? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeIntNullable;
            data.iValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to int?
        /// </summary>
        public static implicit operator int?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return value.iValue;
                case DataType.DataTypeLong: return (int?)value.lValue;
                case DataType.DataTypeULong: return (int?)value.ulValue;
                case DataType.DataTypeDouble: return (int?)value.dValue;
                case DataType.DataTypeIntNullable: return value.iValueNullable;
                case DataType.DataTypeLongNullable: return (int?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (int?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (int?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToInt32(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to int?");
            return null;
        }
        /// <summary>
        /// Implicit convert from uint? to JSONData
        /// </summary>
        public static implicit operator JSONData(uint? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeLongNullable;
            data.lValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to uint?
        /// </summary>
        public static implicit operator uint?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (uint?)value.iValue;
                case DataType.DataTypeLong: return (uint?)value.lValue;
                case DataType.DataTypeULong: return (uint?)value.ulValue;
                case DataType.DataTypeDouble: return (uint?)value.dValue;
                case DataType.DataTypeIntNullable: return (uint?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (uint?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (uint?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (uint?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToUInt32(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to uint?");
            return null;
        }
        /// <summary>
        /// Implicit convert from long? to JSONData
        /// </summary>
        public static implicit operator JSONData(long? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeLongNullable;
            data.lValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to long?
        /// </summary>
        public static implicit operator long?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return value.iValue;
                case DataType.DataTypeLong: return value.lValue;
                case DataType.DataTypeULong: return (long?)value.ulValue;
                case DataType.DataTypeDouble: return (long?)value.dValue;
                case DataType.DataTypeIntNullable: return value.iValueNullable;
                case DataType.DataTypeLongNullable: return value.lValueNullable;
                case DataType.DataTypeULongNullable: return (long?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (long?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToInt64(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to long?");
            return null;
        }
        /// <summary>
        /// Implicit convert from ulong? to JSONData
        /// </summary>
        public static implicit operator JSONData(ulong? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeLongNullable;
            data.ulValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to ulong?
        /// </summary>
        public static implicit operator ulong?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (ulong?)value.iValue;
                case DataType.DataTypeLong: return (ulong?)value.lValue;
                case DataType.DataTypeULong: return value.ulValue;
                case DataType.DataTypeDouble: return (ulong?)value.dValue;
                case DataType.DataTypeIntNullable: return (ulong?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (ulong?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (ulong?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToUInt64(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to ulong?");
            return null;
        }
        /// <summary>
        /// Implicit convert from double? to JSONData
        /// </summary>
        public static implicit operator JSONData(double? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDoubleNullable;
            data.dValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to double?
        /// </summary>
        public static implicit operator double?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return value.iValue;
                case DataType.DataTypeLong: return value.lValue;
                case DataType.DataTypeULong: return value.ulValue;
                case DataType.DataTypeDouble: return value.dValue;
                case DataType.DataTypeIntNullable: return value.iValueNullable;
                case DataType.DataTypeLongNullable: return value.lValueNullable;
                case DataType.DataTypeULongNullable: return value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToDouble(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to double?");
            return null;
        }
        /// <summary>
        /// Implicit convert from float? to JSONData
        /// </summary>
        public static implicit operator JSONData(float? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeDoubleNullable;
            data.dValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to float?
        /// </summary>
        public static implicit operator float?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return value.iValue;
                case DataType.DataTypeLong: return value.lValue;
                case DataType.DataTypeULong: return value.ulValue;
                case DataType.DataTypeDouble: return (float?)value.dValue;
                case DataType.DataTypeIntNullable: return value.iValueNullable;
                case DataType.DataTypeLongNullable: return value.lValueNullable;
                case DataType.DataTypeULongNullable: return value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (float?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToSingle(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to float?");
            return null;
        }
        /// <summary>
        /// Implicit convert from char? to JSONData
        /// </summary>
        public static implicit operator JSONData(char? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeString;
            data.strValue = value.ToString();
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to char?
        /// </summary>
        public static implicit operator char?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return (char?)value.iValue;
                case DataType.DataTypeLong: return (char?)value.lValue;
                case DataType.DataTypeULong: return (char?)value.ulValue;
                case DataType.DataTypeDouble: return (char?)value.dValue;
                case DataType.DataTypeIntNullable: return (char?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (char?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (char?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (char?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                return Convert.ToChar(value.strValue);
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to char?");
            return null;
        }
        /// <summary>
        /// Implicit convert from bool? to JSONData
        /// </summary>
        public static implicit operator JSONData(bool? value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeBooleanNullable;
            data.bValueNullable = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from bool to JSONData
        /// </summary>
        public static implicit operator JSONData(bool value)
        {
            JSONData data = new JSONData();
            data.dataType = DataType.DataTypeBoolean;
            data.bValue = value;
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to bool
        /// </summary>
        public static implicit operator bool(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to bool");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeBoolean: return value.bValue;
                case DataType.DataTypeInt: return value.iValue > 0;
                case DataType.DataTypeLong: return value.lValue > 0;
                case DataType.DataTypeULong: return value.ulValue > 0;
                case DataType.DataTypeDouble: return value.dValue > 0;
                case DataType.DataTypeBooleanNullable: return value.bValueNullable != null && value.bValueNullable.Value;
                case DataType.DataTypeIntNullable: return value.iValueNullable != null && value.iValueNullable.Value > 0;
                case DataType.DataTypeLongNullable: return value.lValueNullable != null && value.lValueNullable.Value > 0;
                case DataType.DataTypeULongNullable: return value.ulValueNullable != null && value.ulValueNullable.Value > 0;
                case DataType.DataTypeDoubleNullable: return value.dValueNullable != null && value.dValueNullable.Value > 0;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "true":
                            case "True":
                            case "TRUE":
                                return true;
                            case "false":
                            case "False":
                            case "FALSE":
                                return false;
                            default:
                                if (ThrowException) throw new Exception(value.ToString() + " can't convert to bool");
                                return default;
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to bool");
            return default;
        }
        /// <summary>
        /// Implicit convert from JSONData to bool?
        /// </summary>
        public static implicit operator bool?(JSONData value)
        {
            if (value == null)
                return null;
            switch (value.dataType)
            {
                case DataType.DataTypeBoolean: return value.bValue;
                case DataType.DataTypeInt: return value.iValue > 0;
                case DataType.DataTypeLong: return value.lValue > 0;
                case DataType.DataTypeULong: return value.ulValue > 0;
                case DataType.DataTypeDouble: return value.dValue > 0;
                case DataType.DataTypeBooleanNullable: return value.bValueNullable;
                case DataType.DataTypeIntNullable: if (value.iValueNullable != null) return value.iValueNullable.Value > 0; else return null;
                case DataType.DataTypeLongNullable: if (value.lValueNullable != null) return value.lValueNullable.Value > 0; else return null;
                case DataType.DataTypeULongNullable: if (value.ulValueNullable != null) return value.ulValueNullable.Value > 0; else return null;
                case DataType.DataTypeDoubleNullable: if (value.dValueNullable != null) return value.dValueNullable.Value > 0; else return null;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        switch (value.strValue)
                        {
                            case "true":
                            case "True":
                            case "TRUE":
                                return true;
                            case "false":
                            case "False":
                            case "FALSE":
                                return false;
                            case "null":
                            case "Null":
                            case "NULL":
                                return null;
                            default:
                                if (ThrowException) throw new Exception(value.ToString() + " can't convert to bool?");
                                return null;
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to bool?");
            return null;
        }
        internal static void ToBinaryData(JSONData value, CSL_Stream stream)
        {
            stream.Write((byte)value.dataType);
            switch (value.dataType)
            {
                case DataType.DataTypeDictionary:
                    stream.Write(value.dictValue.Count);
                    foreach (var one in value.dictValue)
                    {
                        stream.Write(one.Key);
                        ToBinaryData(one.Value, stream);
                    }
                    break;
                case DataType.DataTypeList:
                    stream.Write(value.listValue.Count);
                    foreach (JSONData one in value.listValue)
                    {
                        ToBinaryData(one, stream);
                    }
                    break;
                case DataType.DataTypeString: stream.Write(value.strValue); break;
                case DataType.DataTypeBoolean: stream.Write(value.bValue); break;
                case DataType.DataTypeInt: stream.Write(value.iValue); break;
                case DataType.DataTypeLong: stream.Write(value.lValue); break;
                case DataType.DataTypeULong: stream.Write(value.ulValue); break;
                case DataType.DataTypeDouble: stream.Write(value.dValue); break;
                case DataType.DataTypeBooleanNullable: stream.Write(value.bValueNullable); break;
                case DataType.DataTypeIntNullable: stream.Write(value.iValueNullable); break;
                case DataType.DataTypeLongNullable: stream.Write(value.lValueNullable); break;
                case DataType.DataTypeULongNullable: stream.Write(value.ulValueNullable); break;
                case DataType.DataTypeDoubleNullable: stream.Write(value.dValueNullable); break;
                case DataType.DataTypeNull: break;
            }
        }
        internal static void ToJSONData(JSONData value, CSL_Stream stream)
        {
            stream.Read(out byte dt);
            value.dataType = (DataType)dt;
            switch (value.dataType)
            {
                case DataType.DataTypeDictionary:
                    {
                        value.dictValue = new Dictionary<string, JSONData>();
                        stream.Read(out int count);
                        for (int i = 0; i < count; i++)
                        {
                            stream.Read(out string key);
                            JSONData data = new JSONData();
                            ToJSONData(data, stream);
                            value[key] = data;
                        }
                    }
                    break;
                case DataType.DataTypeList:
                    {
                        value.listValue = new List<JSONData>();
                        stream.Read(out int count);
                        for (int i = 0; i < count; i++)
                        {
                            JSONData data = new JSONData();
                            ToJSONData(data, stream);
                            value.Add(data);
                        }
                    }
                    break;
                case DataType.DataTypeString: stream.Read(out value.strValue); break;
                case DataType.DataTypeBoolean: stream.Read(out value.bValue); break;
                case DataType.DataTypeInt: stream.Read(out value.iValue); break;
                case DataType.DataTypeLong: stream.Read(out value.lValue); break;
                case DataType.DataTypeULong: stream.Read(out value.ulValue); break;
                case DataType.DataTypeDouble: stream.Read(out value.dValue); break;
                case DataType.DataTypeBooleanNullable: stream.Read(out value.bValueNullable); break;
                case DataType.DataTypeIntNullable: stream.Read(out value.iValueNullable); break;
                case DataType.DataTypeLongNullable: stream.Read(out value.lValueNullable); break;
                case DataType.DataTypeULongNullable: stream.Read(out value.ulValueNullable); break;
                case DataType.DataTypeDoubleNullable: stream.Read(out value.dValueNullable); break;
                case DataType.DataTypeNull: break;
            }
        }
    }
    #endregion //PrivateImp
}