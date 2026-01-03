/*
 * KissJson : Keep It Simple Stupid JSON
 * Copyright © 2022-2026 RongRong. All right reserved.
 */

//Whether spport nullable type, if you will never use the nullable type, you can remove this define `SUPPORT_NULLABLE` to make this `KissJSON.dll` smaller, you also can set that define in project config.
//Chinese:是否支持可空类型,如果你将不会使用可空类型,你可以把这个`SUPPORT_NULLABLE`定义注释掉,可以使得`KissJSON.dll`更小一些,你也可以在项目设置里设置该宏.
#define SUPPORT_NULLABLE

using System.Text;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;

namespace CSharpLike
{
    /// <summary>
    /// A simple JSON data.
    /// It's not the most efficient,but the easiest to use.
    /// <br/><br/>Chinese:<br/>一个简单的JSON对象类.这未必是最高效的,但是是非常易用.
    /// </summary>
    public sealed class JSONData : IEnumerable
    {
        /// <summary>
        /// Data type of the JSON
        /// <br/><br/>Chinese:<br/>JSON的数据类型
        /// </summary>
        public enum DataType : byte
        {
            /// <summary>
            /// This JSON object is null
            /// <br/><br/>Chinese:<br/>数据类型为空
            /// </summary>
            DataTypeNull,
            /// <summary>
            /// Is Dictionary type, it like `{}`
            /// <br/><br/>Chinese:<br/>数据类型为字典,即`{}`这种类型
            /// </summary>
            DataTypeDictionary,
            /// <summary>
            /// Is List type, it like `[]`
            /// <br/><br/>Chinese:<br/>数据类型为列表,即`[]`这种类型
            /// </summary>
            DataTypeList,
            /// <summary>
            /// Value type is string
            /// <br/><br/>Chinese:<br/>数据类型为字符串string
            /// </summary>
            DataTypeString,
            /// <summary>
            /// Value type is boolean
            /// <br/><br/>Chinese:<br/>数据类型为布尔值bool
            /// </summary>
            DataTypeBoolean,
            /// <summary>
            /// IValue type is a int
            /// <br/><br/>Chinese:<br/>数据类型为整数int
            /// </summary>
            DataTypeInt,
            /// <summary>
            /// Value type is long
            /// <br/><br/>Chinese:<br/>数据类型为64位整数long
            /// </summary>
            DataTypeLong,
            /// <summary>
            /// Value type is ulong
            /// <br/><br/>Chinese:<br/>数据类型为无符号64位整数ulong
            /// </summary>
            DataTypeULong,
            /// <summary>
            /// Value type is double
            /// <br/><br/>Chinese:<br/>类型为双精度浮点数double
            /// </summary>
            DataTypeDouble,
            /// <summary>
            /// Value type is decimal
            /// <br/><br/>Chinese:<br/>数据类型为十进制
            /// </summary>
            DataTypeDecimal,
#if SUPPORT_NULLABLE
            /// <summary>
            /// Value type is nullable boolean
            /// <br/><br/>Chinese:<br/>类型为可空布尔值bool
            /// </summary>
            DataTypeBooleanNullable,
            /// <summary>
            /// Value type is nullable int
            /// <br/><br/>Chinese:<br/>类型为可空整数int
            /// </summary>
            DataTypeIntNullable,
            /// <summary>
            /// Value type is nullable long
            /// <br/><br/>Chinese:<br/>类型为可空64位整数long
            /// </summary>
            DataTypeLongNullable,
            /// <summary>
            /// Value type is nullable ulong
            /// <br/><br/>Chinese:<br/>类型为可空无符号64位整数long
            /// </summary>
            DataTypeULongNullable,
            /// <summary>
            /// Value type is nullable double
            /// <br/><br/>Chinese:<br/>类型为可空双精度浮点数long
            /// </summary>
            DataTypeDoubleNullable,
#else
            /// <summary>
            /// Value type is nullable boolean
            /// <br/><br/>Chinese:<br/>类型为可空布尔值bool
            /// </summary>
            [Obsolete("Not support nullable type, you should set `#define SUPPORT_NULLABLE` in `JSONData.cs`", false)]
            DataTypeBooleanNullable,
            /// <summary>
            /// Value type is nullable int
            /// <br/><br/>Chinese:<br/>类型为可空整数int
            /// </summary>
            [Obsolete("Not support nullable type, you should set `#define SUPPORT_NULLABLE` in `JSONData.cs`", false)]
            DataTypeIntNullable,
            /// <summary>
            /// Value type is nullable long
            /// <br/><br/>Chinese:<br/>类型为可空64位整数long
            /// </summary>
            [Obsolete("Not support nullable type, you should set `#define SUPPORT_NULLABLE` in `JSONData.cs`", false)]
            DataTypeLongNullable,
            /// <summary>
            /// Value type is nullable ulong
            /// <br/><br/>Chinese:<br/>类型为可空无符号64位整数long
            /// </summary>
            [Obsolete("Not support nullable type, you should set `#define SUPPORT_NULLABLE` in `JSONData.cs`", false)]
            DataTypeULongNullable,
            /// <summary>
            /// Value type is nullable double
            /// <br/><br/>Chinese:<br/>类型为可空双精度浮点数long
            /// </summary>
            [Obsolete("Not support nullable type, you should set `#define SUPPORT_NULLABLE` in `JSONData.cs`", false)]
            DataTypeDoubleNullable,
#endif //SUPPORT_NULLABLE
        }
        /// <summary>
        /// The packet type save as integer or string. 
        /// Default is string, that easy identify in log. It's for method `NewPacket`.
        /// <br/><br/>Chinese:<br/>封包类型为整数还是字符串.默认为字符串,方便日志里查看. 用于函数`NewPacket`
        /// </summary>
        public static bool packetIsInteger = false;
#if !UNITY_2020_1_OR_NEWER //That prevent you call this from hot update script
        /// <summary>
        /// Create a JSONData with packet type for network packet, that use in NONE hot update script.
        /// <br/><br/>Chinese:<br/>创建一个JSON对象用于网络封包,用于非热更新代码的
        /// </summary>
        /// <param name="packetType">New to JSON object with key 'packetType'<br/><br/>Chinese:<br/>新建一个带`packetType`的JSON对象</param>
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
        /// <br/><br/>Chinese:<br/>获取该JSON对象里的`packetType`数据,用于非热更新代码的
        /// </summary>
        public T GetPacketType<T>()
        {
            return (T)Enum.Parse(typeof(T), this["packetType"]);
        }
#endif
        /// <summary>
        /// Create a JSONData with packet type for network packet, that use in hot update script.
        /// <br/><br/>Chinese:<br/>创建一个JSON对象用于网络封包,用于热更新代码的
        /// </summary>
        /// <param name="type">The type of the Packet<br/><br/>Chinese:<br/>枚举类型</param>
        /// <param name="value">The value of 'packetType'<br/><br/>Chinese:<br/>'packetType'的数值</param>
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
        /// <br/><br/>Chinese:<br/>获取该JSON对象里的`packetType`数据,用于热更新代码的
        /// <param name="type">The type of the Packet<br/><br/>Chinese:<br/>枚举类型</param>
        /// </summary>
        public int GetPacketType(Type type)
        {
            if (packetIsInteger)
                return this["packetType"];
            return (int)Enum.Parse(type, this["packetType"]);
        }
#if UNITY_2020_1_OR_NEWER
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
        /// Create a JSONData which type is Dictionary, equal to `{}`
        /// <br/><br/>Chinese:<br/>新建一个字典类型的JSON对象,即`[]`
        /// </summary>
        public static JSONData NewDictionary() => new JSONData() { dictValue = new Dictionary<string, JSONData>(), dataType = DataType.DataTypeDictionary };
        /// <summary>
        /// Create an JSONData which type is List, equal to `[]`
        /// <br/><br/>Chinese:<br/>新建一个列表类型的JSON对象,即`[]`
        /// </summary>
        public static JSONData NewList() => new JSONData() { listValue = new List<JSONData>(), dataType = DataType.DataTypeList };
        static Dictionary<Type, MethodInfo> mMethodInfos = null;
        static Dictionary<Type, MethodInfo> mMethodInfosFrom = null;
        /// <summary>
        /// Get implicit function by Type
        /// <br/><br/>Chinese:<br/>根据类型获取隐式转换函数
        /// </summary>
        /// <param name="type">Specific type<br/><br/>Chinese:<br/>指定类型</param>
        /// <returns>Method infomation</returns>
        public static MethodInfo GetImplicit(Type type)
        {
            if (type == null)
                return null;
            if (mMethodInfos == null)
            {
                mMethodInfos = new Dictionary<Type, MethodInfo>();
                mMethodInfosFrom = new Dictionary<Type, MethodInfo>();
                Type typeSelf = typeof(JSONData);
                foreach (MethodInfo mi in typeSelf.GetMethods())
                {
                    if (mi.Name == "op_Implicit")
                    {
                        if (typeSelf == mi.ReturnType)//Other type implicit convert to JSONData.
                        {
                            ParameterInfo[] ps = mi.GetParameters();
                            if (ps.Length == 1)
                                mMethodInfosFrom[ps[0].ParameterType] = mi;
                        }
                        else//JSONData implicit convert to other type.
                            mMethodInfos[mi.ReturnType] = mi;
                    }
                }
            }
            return mMethodInfos[type];
        }
        /// <summary>
        /// Get implicit function by the from type
        /// <br/><br/>Chinese:<br/>根据目标类型获取隐式转换函数
        /// </summary>
        /// <param name="type">Specific type<br/><br/>Chinese:<br/>指定目标类型</param>
        /// <returns>Method infomation</returns>
        public static MethodInfo GetImplicitFrom(Type type)
        {
            if (type == null)
                return null;
            if (mMethodInfos == null)
            {
                mMethodInfos = new Dictionary<Type, MethodInfo>();
                mMethodInfosFrom = new Dictionary<Type, MethodInfo>();
                Type typeSelf = typeof(JSONData);
                foreach (MethodInfo mi in typeSelf.GetMethods())
                {
                    if (mi.Name == "op_Implicit")
                    {
                        if (typeSelf == mi.ReturnType)//Other type implicit convert to JSONData.
                        {
                            ParameterInfo[] ps = mi.GetParameters();
                            if (ps.Length == 1)
                                mMethodInfosFrom[ps[0].ParameterType] = mi;
                        }
                        else//JSONData implicit convert to other type.
                            mMethodInfos[mi.ReturnType] = mi;
                    }
                }
            }
            return mMethodInfosFrom[type];
        }
        /// <summary>
        /// the value of this JSONData
        /// <br/><br/>Chinese:<br/>JSON对象的值
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
                    case DataType.DataTypeDecimal: return mValue;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeBooleanNullable: return bValueNullable;
                    case DataType.DataTypeIntNullable: return iValueNullable;
                    case DataType.DataTypeLongNullable: return lValueNullable;
                    case DataType.DataTypeULongNullable: return ulValueNullable;
                    case DataType.DataTypeDoubleNullable: return dValueNullable;
#endif //SUPPORT_NULLABLE
                    default: return null;
                }
            }
        }
        /// <summary>
        /// the type of this JSONData
        /// <br/><br/>Chinese:<br/>JSON对象的类型
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
                    case DataType.DataTypeDecimal: return typeof(decimal);
#if SUPPORT_NULLABLE
                    case DataType.DataTypeBooleanNullable: return typeof(bool?);
                    case DataType.DataTypeIntNullable: return typeof(int?);
                    case DataType.DataTypeLongNullable: return typeof(long?);
                    case DataType.DataTypeULongNullable: return typeof(ulong?);
                    case DataType.DataTypeDoubleNullable: return typeof(double?);
#endif //SUPPORT_NULLABLE
                    default: return null;
                }
            }
        }
        /// <summary>
        /// Get a deep clone JSONData object, create a new JSON object not just refer to.
        /// <br/><br/>Chinese:<br/>深复制JSON对象,创建一个新独立对象而非简单的引用
        /// </summary>
        /// <param name="sourceJsonData">The source JSONData object<br/><br/>Chinese:<br/>被复制的源JSON对象</param>
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
                case DataType.DataTypeDecimal: return sourceJsonData.mValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeBooleanNullable: return sourceJsonData.bValueNullable;
                case DataType.DataTypeIntNullable: return sourceJsonData.iValueNullable;
                case DataType.DataTypeLongNullable: return sourceJsonData.ulValueNullable;
                case DataType.DataTypeULongNullable: return sourceJsonData.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return sourceJsonData.dValueNullable;
#endif //SUPPORT_NULLABLE
                default: return null;
            }
        }
        /// <summary>
        /// Throw exception when should throw exception (e.g. like convert fail, array out of bounds exception), otherwise will return default or ignore action and with no exception. 
        /// Default value is false.
        /// <br/><br/>Chinese:<br/>是否抛出异常当遇到异常的时候(例如转换失败,数组越界之类的),否则会返回默认值或忽略异常.
        /// 默认为false不抛出异常
        /// </summary>
        public static bool ThrowException = false;
        /// <summary>
        /// get/set item of this(this is a Dictionary),Same with Dictionary.this[]
        /// <br/><br/>Chinese:<br/>this的get/set操作(这是一个字典),和Dictionary.this[]一样
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
                CheckValidDictionary();
                dictValue[key] = value;
            }
        }
        void CheckValidDictionary()
        {
            if (dictValue == null)
            {
                if (ThrowException) throw new Exception(ToString() + " not a dictionary");
                dictValue = new Dictionary<string, JSONData>();
                dataType = DataType.DataTypeDictionary;
            }
        }
        /// <summary>
        /// get/set item of this(this is a List),Same with List.this[]
        /// <br/><br/>Chinese:<br/>this的get/set操作(这是一个列表),和List.this[]一样
        /// </summary>
        public JSONData this[int index]
        {
            get
            {
                if (dataType == DataType.DataTypeList && listValue != null)
                {
                    if (index < 0 || index >= listValue.Count)
                    {
                        if (ThrowException) throw new Exception(ToString() + $" index {index} < 0 or >= listValue.Count {listValue.Count}");
                        return null;
                    }
                    return listValue[index];
                }
                if (ThrowException) throw new Exception(ToString() + " not a list");
                return null;
            }
            set
            {
                CheckValidList();
                if (index < 0 || index >= listValue.Count)
                {
                    if (ThrowException) throw new Exception(ToString() + $" index {index} < 0 or >= listValue.Count {listValue.Count}");
                    return;
                }
                listValue[index] = value;
            }
        }
        void CheckValidList()
        {
            if (listValue == null)
            {
                if (ThrowException) throw new Exception(ToString() + " not a list");
                listValue = new List<JSONData>();
                dataType = DataType.DataTypeList;
            }
        }
        /// <summary>
        /// Add an JSONData to this(this is a List).Same with List.Add().
        /// <br/><br/>Chinese:<br/>添加一个JSON对象(这是一个列表),和List.Add()一样
        /// </summary>
        public void Add(JSONData item)
        {
            CheckValidList();
            listValue.Add(item);
        }
        /// <summary>
        /// Insert item to this(this is a List),Same with List.Insert().
        /// <br/><br/>Chinese:<br/>插入一个JSON对象(这是一个列表),和List.Insert()一样
        /// </summary>
        public void Insert(int index, JSONData item)
        {
            CheckValidList();
            listValue.Insert(index, item);
        }
        /// <summary>
        /// Remove item from this(this is a List),Same with List.Remove().
        /// <br/><br/>Chinese:<br/>移除一个JSON对象(这是一个列表),和List.Remove()一样
        /// </summary>
        public void Remove(JSONData item)
        {
            CheckValidList();
            listValue.Remove(item);
        }
        /// <summary>
        /// Remove item from this by index(this is a List),Same with List.RemoveAt().
        /// <br/><br/>Chinese:<br/>移除指定索引的JSON对象(这是一个列表),和List.RemoveAt()一样
        /// </summary>
        public void RemoveAt(int index)
        {
            CheckValidList();
            listValue.RemoveAt(index);
        }
        /// <summary>
        /// Reverse item of this(this is a List),Same with List.Reverse().
        /// <br/><br/>Chinese:<br/>反转列表里的元素(这是一个列表),和List.Reverse()一样
        /// </summary>
        public void Reverse()
        {
            CheckValidList();
            listValue.Reverse();
        }
        /// <summary>
        /// Check this whether contains item(this is a List),Same with List.Contains().
        /// <br/><br/>Chinese:<br/>检查是否包含指定JSON对象(这是一个列表),和List.Contains()一样
        /// </summary>
        public bool Contains(JSONData item)
        {
            return listValue != null && listValue.Contains(item);
        }
        /// <summary>
        /// item count of this(this is a List or Dictionary),Same with List.Count or Dictionary.Count. It'll return 0 if not a List or Dictionary.
        /// <br/><br/>Chinese:<br/>该JSON对象包含的子JSON对象数量(这是一个列表或字典),和List.Count或Dictionary.Count一样.如果不是列表或字典则返回0
        /// </summary>
        public int Count
        {
            get
            {
                if (listValue != null)
                    return listValue.Count;
                else if (dictValue != null)
                    return dictValue.Count;
                return 0;
            }
        }
        /// <summary>
        /// clear item of this(this is a List or Dictionary),Same with List.Clear() or Dictionary.Clear()
        /// <br/><br/>Chinese:<br/>清除该JSON对象所有子JSON对象(这是一个列表或字典),和List.Clear()或Dictionary.Clear()一样.
        /// </summary>
        public void Clear()
        {
            if (listValue != null)
                listValue.Clear();
            else if (dictValue != null)
                dictValue.Clear();
        }
        /// <summary>
        /// add item to this(this is a Dictionary),Same with Dictionary.Add()
        /// <br/><br/>Chinese:<br/>向该JSON对象添加元素(这是一个字典),和Dictionary.Add()一样.
        /// </summary>
        public void Add(string key, JSONData value)
        {
            CheckValidDictionary();
            dictValue.Add(key, value);
        }
        /// <summary>
        /// remove item from this(this is a Dictionary),Same with Dictionary.RemoveKey()
        /// <br/><br/>Chinese:<br/>移除该JSON对象指定索引元素(这是一个字典),和Dictionary.RemoveKey()一样.
        /// </summary>
        public void RemoveKey(string key)
        {
            CheckValidDictionary();
            dictValue.Remove(key);
        }
        /// <summary>
        /// Check this whether contains key(this is a Dictionary),Same with Dictionary.ContainsKey()
        /// <br/><br/>Chinese:<br/>该JSON对象是否包含指定索引(这是一个字典),和Dictionary.ContainsKey()一样.
        /// </summary>
        public bool ContainsKey(string key)
        {
            return dictValue != null && dictValue.ContainsKey(key);
        }
        /// <summary>
        /// Check this whether contains value(this is a Dictionary),Same with Dictionary.ContainsValue()
        /// <br/><br/>Chinese:<br/>该JSON对象是否包含指定值(这是一个字典),和Dictionary.ContainsValue()一样.
        /// </summary>
        public bool ContainsValue(JSONData value)
        {
            return dictValue != null && dictValue.ContainsValue(value);
        }
        /// <summary>
        /// try get value from this(this is a Dictionary),Same with Dictionary.TryGetValue()
        /// <br/><br/>Chinese:<br/>尝试根据索引获取元素(这是一个字典),和Dictionary.TryGetValue()一样.
        /// </summary>
        public bool TryGetValue(string key, out JSONData value)
        {
            if (dictValue != null)
                return dictValue.TryGetValue(key, out value);
            value = new JSONData();
            value.dataType = DataType.DataTypeNull;
            return false;
        }
        /// <summary>
        /// The data type of this object 
        /// <br/><br/>Chinese:<br/>JSON对象的数据类型
        /// </summary>
        public DataType dataType;
        /// <summary>
        /// Get extern temp object from this JSONData, that not format into JSON string.
        /// <br/><br/>Chinese:<br/>该JSON对象里存储的额外临时数据,这个不会转化到JSON字符串中.
        /// </summary>
        public object GetObjectExtern(string key) => (_externObjects != null && _externObjects.TryGetValue(key, out object obj)) ? obj : null;
        /// <summary>
        /// Set extern temp object to this JSONData
        /// <br/><br/>Chinese:<br/>向该JSON对象存储的额外临时数据
        /// </summary>
        public void SetObjectExtern(string key, object obj)
        {
            if (_externObjects == null)
                _externObjects = new Dictionary<string, object>();
            _externObjects[key] = obj;
        }
        /// <summary>
        /// Get an enumerator that iterates through List or Dictionary.
        /// <br/>Usage:
        /// <br/>JSONData list = new JSONData(){ 1, 2, 3 };
        /// <br/>//Recommand style, can't use `var`
        /// <br/>foreach (JSONData item in list) Console.WriteLine($"List {item}");
        /// <br/>//This style, can use `var`
        /// <br/>foreach (var item in list.Value as List&lt;JSONData&gt;) Console.WriteLine($"List {item}");
        /// <br/>//Not recommended style due to it'll copy a new List, can use `var`
        /// <br/>foreach (var item in (List&lt;int&gt;)list) Console.WriteLine($"List {item}");
        /// <br/>JSONData dic = new JSONData(){ { "aa", 1 },{ "bb", "a" } };
        /// <br/>//Recommand style, can't use `var`
        /// <br/>foreach (KeyValuePair&lt;string, JSONData&gt; item in dic) Console.WriteLine($"Dictionary {item.Key}={item.Value}");
        /// <br/>//This style, can use `var`
        /// <br/>foreach (var item in dic.Value as Dictionary&lt;string, JSONData&gt;) Console.WriteLine($"Dictionary {item.Key}={item.Value}");
        /// <br/>//Not recommended style due to it'll copy a new Dictionary, can use `var`
        /// <br/>foreach (var item in (Dictionary&lt;string, object&gt;)dic) Console.WriteLine($"Dictionary {item.Key}={item.Value}");
        /// <br/><br/>Chinese:<br/>从列表或字典里获取枚举器
        /// <br/>用法:
        /// <br/>JSONData list = new JSONData(){ 1, 2, 3 };
        /// <br/>//推荐的方式,不可以使用`var`
        /// <br/>foreach (JSONData item in list) Console.WriteLine($"List {item}");
        /// <br/>//这种方式可以使用`var`
        /// <br/>foreach (var item in list.Value as List&lt;JSONData&gt;) Console.WriteLine($"List {item}");
        /// <br/>//不推荐的方式是因为会复制一个List出来,可以使用`var`
        /// <br/>foreach (var item in (List&lt;int&gt;)list) Console.WriteLine($"List {item}");
        /// <br/>JSONData dic = new JSONData(){ { "aa", 1 },{ "bb", "a" } };
        /// <br/>//推荐的方式,不可以使用`var`
        /// <br/>foreach (KeyValuePair&lt;string, JSONData&gt; item in dic) Console.WriteLine($"Dictionary {item.Key}={item.Value}");
        /// <br/>//这种方式可以使用`var`
        /// <br/>foreach (var item in dic.Value as Dictionary&lt;string, JSONData&gt;) Console.WriteLine($"Dictionary {item.Key}={item.Value}");
        /// <br/>//不推荐的方式是因为会复制一个Dictionary出来,可以使用`var`
        /// <br/>foreach (var item in (Dictionary&lt;string, object&gt;)dic) Console.WriteLine($"Dictionary {item.Key}={item.Value}");
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            switch (dataType)
            {
                case DataType.DataTypeList: return listValue.GetEnumerator();
                case DataType.DataTypeDictionary: return dictValue.GetEnumerator();
                default:
                    throw new Exception($"JSONData {this} is not a List or Dictionary");
            }
        }
        #region PrivateImpl
        private List<JSONData> listValue;
        private Dictionary<string, JSONData> dictValue;
        private string strValue;
        private bool bValue;
        private int iValue;
        private long lValue;
        private ulong ulValue;
        private double dValue;
        private decimal mValue;
#if SUPPORT_NULLABLE
        private bool? bValueNullable;
        private int? iValueNullable;
        private long? lValueNullable;
        private ulong? ulValueNullable;
        private double? dValueNullable;
#endif //SUPPORT_NULLABLE
        private Dictionary<string, object> _externObjects;

        /// <summary>
        /// Convert this JSONData to string no deal with special string '\' '"' '\n' '\r' '\t' '\b' '\f'. 
        /// They are will error if have secial string in it, you should use ToJson() instead.
        /// <br/><br/>Chinese:<br/>转成字符串,不会对特别字符'\' '"' '\n' '\r' '\t' '\b' '\f'进行额外处理,如果带特殊字符将会有异常,建议使用ToJson()来代替
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
                    return dValue.ToString(KissJson.CultureForConvertFloatAndDouble);
                case DataType.DataTypeDecimal:
                    return mValue.ToString(KissJson.CultureForConvertFloatAndDouble);
#if SUPPORT_NULLABLE
                case DataType.DataTypeBooleanNullable:
                    return bValueNullable == null ? "null" : (bValueNullable.Value ? "true" : "false");
                case DataType.DataTypeIntNullable:
                    return iValueNullable == null ? "null" : iValueNullable.Value.ToString();
                case DataType.DataTypeLongNullable:
                    return lValueNullable == null ? "null" : lValueNullable.Value.ToString();
                case DataType.DataTypeULongNullable:
                    return ulValueNullable == null ? "null" : ulValueNullable.Value.ToString();
                case DataType.DataTypeDoubleNullable:
                    return dValueNullable == null ? "null" : dValueNullable.Value.ToString(KissJson.CultureForConvertFloatAndDouble);
#endif //SUPPORT_NULLABLE
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
        /// <br/><br/>Chinese:<br/>转成字符串,已对特别字符'\' '"' '\n' '\r' '\t' '\b' '\f'进行额外处理
        /// </summary>
        /// <param name="bFormat">Format the JSON string with many spaces and wraps, make it more readability. default is false<br/><br/>Chinese:<br/>是否采用更可读性的格式化(含大量空格和换行),默认false表示采用最精短的方式</param>
        /// <param name="depth">Internal use, you can ignore it<br/><br/>Chinese:<br/>可读性加强方式下的层数,仅供内部使用,外部调用请直接使用默认值0</param>
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
                        return dValue.ToString(KissJson.CultureForConvertFloatAndDouble);
                    case DataType.DataTypeDecimal:
                        return mValue.ToString(KissJson.CultureForConvertFloatAndDouble);
#if SUPPORT_NULLABLE
                    case DataType.DataTypeBooleanNullable:
                        return bValueNullable == null ? "null" : (bValueNullable.Value ? "true" : "false");
                    case DataType.DataTypeIntNullable:
                        return iValueNullable == null ? "null" : iValueNullable.Value.ToString();
                    case DataType.DataTypeLongNullable:
                        return lValueNullable == null ? "null" : lValueNullable.Value.ToString();
                    case DataType.DataTypeULongNullable:
                        return ulValueNullable == null ? "null" : ulValueNullable.Value.ToString();
                    case DataType.DataTypeDoubleNullable:
                        return dValueNullable == null ? "null" : dValueNullable.Value.ToString(KissJson.CultureForConvertFloatAndDouble);
#endif //SUPPORT_NULLABLE
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
                        return dValue.ToString(KissJson.CultureForConvertFloatAndDouble);
                    case DataType.DataTypeDecimal:
                        return mValue.ToString(KissJson.CultureForConvertFloatAndDouble);
#if SUPPORT_NULLABLE
                    case DataType.DataTypeBooleanNullable:
                        return bValueNullable == null ? "null" : (bValueNullable.Value ? "true" : "false");
                    case DataType.DataTypeIntNullable:
                        return iValueNullable == null ? "null" : iValueNullable.Value.ToString();
                    case DataType.DataTypeLongNullable:
                        return lValueNullable == null ? "null" : lValueNullable.Value.ToString();
                    case DataType.DataTypeULongNullable:
                        return ulValueNullable == null ? "null" : ulValueNullable.Value.ToString();
                    case DataType.DataTypeDoubleNullable:
                        return dValueNullable == null ? "null" : dValueNullable.Value.ToString(KissJson.CultureForConvertFloatAndDouble);
#endif //SUPPORT_NULLABLE
                    default:
                        return "null";
                }
            }
        }


        /// <summary>
        /// object Convert To JSONData.
        /// It's suppose as internal use.
        /// <br/><br/>Chinese:<br/>对象转为JSON对象,这个内部使用的
        /// </summary>
        /// <param name="obj">The source object, only support type: byte/sbyte/short/ushort/int/uint/long/ulong/string/bool/DateTime/byte?/sbyte?/short?/ushort?/int?/uint?/long?/ulong?/bool?<br/><br/>Chinese:<br/>原对象,仅支持类型:byte/sbyte/short/ushort/int/uint/long/ulong/string/bool/DateTime/byte?/sbyte?/short?/ushort?/int?/uint?/long?/ulong?/bool?</param>
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
        /// only support type : byte/sbyte/short/ushort/int/uint/long/ulong/string/bool/DateTime/byte?/sbyte?/short?/ushort?/int?/uint?/long?/ulong?/bool?
        /// <br/><br/>Chinese:<br/>JSON对象转为指定类型的对象,这个内部使用的,仅支持类型:byte/sbyte/short/ushort/int/uint/long/ulong/string/bool/DateTime/byte?/sbyte?/short?/ushort?/int?/uint?/long?/ulong?/bool?
        /// </summary>
        /// <param name="targetType">The target Type<br/><br/>Chinese:<br/>目标类型</param>
        /// <param name="obj">The JSON object to be convert.<br/><br/>Chinese:<br/>待转换的JSON对象</param>
        public static object ConvertTo(Type targetType, JSONData obj)
        {
            if (obj == null || obj.dataType == DataType.DataTypeNull)
                return null;
            return Convert.ChangeType(obj, targetType, null);
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
        static bool IsInValidList(ref JSONData value)
        {
            if (value == null) return true;
            if (value.dataType == DataType.DataTypeString && value.strValue != null)
            {
                //1 Normal JSON string format. e.g.`{"aa":1,"bb":2}`
                JSONData value2 = KissJson.ToJSONData(value.strValue);
                if (value2 == null)//We still return an empty JSONData with no item
                {
                    value.dataType = DataType.DataTypeList;
                    value.listValue = new List<JSONData>();
                    //2 Split by `|`. e.g. `1|2|3`
                    //3 Split by `,`. e.g. `1,2,3`
                    foreach (string item in value.strValue.Split(value.strValue.IndexOf('|') >= 0
                        && value.strValue.IndexOf(',') < 0 ? '|' : ','))
                        value.listValue.Add(item);
                    return false;
                }
                else if (value2.dataType == DataType.DataTypeList)
                {
                    value.dataType = DataType.DataTypeList;
                    value.listValue = value2.listValue;
                    return false;
                }
            }
            return value == null || value.dataType != DataType.DataTypeList;
        }
        static JSONData FromList(IList value, Type type)
        {
            JSONData data = NewList();
            MethodInfo mi = GetImplicitFrom(type);
            if (mi != null)
            {
                IList list = data.listValue;
                foreach (var item in value)
                    list.Add(mi.Invoke(null, new object[1] { item }));
            }
            else if (ThrowException) throw new Exception($"{type.Name} can't convert to JSONData");
            return data;
        }
        static Dictionary<Type, ConstructorInfo> mTypeLists = new Dictionary<Type, ConstructorInfo>();
        internal static object CreateList(Type type)
        {
            if (!mTypeLists.TryGetValue(type, out ConstructorInfo ci))
            {
                Type t = typeof(List<>).MakeGenericType(type);
                ci = t.GetConstructor(Type.EmptyTypes);
                mTypeLists[type] = ci;
            }
            return ci.Invoke(null);
        }
        static IList ToList(JSONData value, Type type)
        {
            if (IsInValidList(ref value)) return null;
            IList data = CreateList(type) as IList;
            MethodInfo mi = GetImplicit(type);
            if (mi != null)
            {
                foreach (var item in value.listValue)
                    data.Add(mi.Invoke(null, new object[1] { item }));
            }
            else if (ThrowException) throw new Exception($"JSONData can't convert to {type.Name}");
            return data;
        }
        /// <summary>
        /// Implicit convert from List&lt;byte&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;byte&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<byte> value) => FromList(value, typeof(byte));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;byte&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;byte&gt;
        /// </summary>
        public static implicit operator List<byte>(JSONData value) => ToList(value, typeof(byte)) as List<byte>;
        /// <summary>
        /// Implicit convert from List&lt;sbyte&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;sbyte&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<sbyte> value) => FromList(value, typeof(sbyte));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;sbyte&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;sbyte&gt;
        /// </summary>
        public static implicit operator List<sbyte>(JSONData value) => ToList(value, typeof(sbyte)) as List<sbyte>;
        /// <summary>
        /// Implicit convert from List&lt;short&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;short&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<short> value) => FromList(value, typeof(short));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;short&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;short&gt;
        /// </summary>
        public static implicit operator List<short>(JSONData value) => ToList(value, typeof(short)) as List<short>;
        /// <summary>
        /// Implicit convert from List&lt;ushort&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;ushort&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<ushort> value) => FromList(value, typeof(ushort));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;ushort&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;ushort&gt;
        /// </summary>
        public static implicit operator List<ushort>(JSONData value) => ToList(value, typeof(ushort)) as List<ushort>;
        /// <summary>
        /// Implicit convert from List&lt;int&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;int&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<int> value) => FromList(value, typeof(int));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;int&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;int&gt;
        /// </summary>
        public static implicit operator List<int>(JSONData value) => ToList(value, typeof(int)) as List<int>;
        /// <summary>
        /// Implicit convert from List&lt;uint&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;uint&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<uint> value) => FromList(value, typeof(uint));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;uint&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;uint&gt;
        /// </summary>
        public static implicit operator List<uint>(JSONData value) => ToList(value, typeof(uint)) as List<uint>;
        /// <summary>
        /// Implicit convert from List&lt;long&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;long&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<long> value) => FromList(value, typeof(long));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;long&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;long&gt;
        /// </summary>
        public static implicit operator List<long>(JSONData value) => ToList(value, typeof(long)) as List<long>;
        /// <summary>
        /// Implicit convert from List&lt;ulong&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;ulong&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<ulong> value) => FromList(value, typeof(ulong));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;ulong&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;ulong&gt;
        /// </summary>
        public static implicit operator List<ulong>(JSONData value) => ToList(value, typeof(ulong)) as List<ulong>;
        /// <summary>
        /// Implicit convert from List&lt;float&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;float&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<float> value) => FromList(value, typeof(float));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;float&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;float&gt;
        /// </summary>
        public static implicit operator List<float>(JSONData value) => ToList(value, typeof(float)) as List<float>;
        /// <summary>
        /// Implicit convert from List&lt;double&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;double&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<double> value) => FromList(value, typeof(double));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;double&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;double&gt;
        /// </summary>
        public static implicit operator List<double>(JSONData value) => ToList(value, typeof(double)) as List<double>;
        /// <summary>
        /// Implicit convert from List&lt;bool&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;bool&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<bool> value) => FromList(value, typeof(bool));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;bool&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;bool&gt;
        /// </summary>
        public static implicit operator List<bool>(JSONData value) => ToList(value, typeof(bool)) as List<bool>;
        /// <summary>
        /// Implicit convert from List&lt;char&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;char&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<char> value) => FromList(value, typeof(char));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;char&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;char&gt;
        /// </summary>
        public static implicit operator List<char>(JSONData value) => ToList(value, typeof(char)) as List<char>;
        /// <summary>
        /// Implicit convert from List&lt;string&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;string&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<string> value) => FromList(value, typeof(string));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;string&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;string&gt;
        /// </summary>
        public static implicit operator List<string>(JSONData value) => ToList(value, typeof(string)) as List<string>;
        /// <summary>
        /// Implicit convert from List&lt;JSONData&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;JSONData&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<JSONData> value) => FromList(value, typeof(JSONData));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;JSONData&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;JSONData&gt;
        /// </summary>
        public static implicit operator List<JSONData>(JSONData value) => ToList(value, typeof(JSONData)) as List<JSONData>;
#if SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from List&lt;byte?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;byte?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<byte?> value) => FromList(value, typeof(byte?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;byte?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;byte?&gt;
        /// </summary>
        public static implicit operator List<byte?>(JSONData value) => ToList(value, typeof(byte?)) as List<byte?>;
        /// <summary>
        /// Implicit convert from List&lt;sbyte?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;sbyte?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<sbyte?> value) => FromList(value, typeof(sbyte?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;sbyte?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;sbyte?&gt;
        /// </summary>
        public static implicit operator List<sbyte?>(JSONData value) => ToList(value, typeof(sbyte?)) as List<sbyte?>;
        /// <summary>
        /// Implicit convert from List&lt;short?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;short?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<short?> value) => FromList(value, typeof(short?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;short?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;short?&gt;
        /// </summary>
        public static implicit operator List<short?>(JSONData value) => ToList(value, typeof(short?)) as List<short?>;
        /// <summary>
        /// Implicit convert from List&lt;ushort?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;ushort?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<ushort?> value) => FromList(value, typeof(ushort?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;ushort?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;ushort?&gt;
        /// </summary>
        public static implicit operator List<ushort?>(JSONData value) => ToList(value, typeof(ushort?)) as List<ushort?>;
        /// <summary>
        /// Implicit convert from List&lt;int?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;int?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<int?> value) => FromList(value, typeof(int?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;int?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;int?&gt;
        /// </summary>
        public static implicit operator List<int?>(JSONData value) => ToList(value, typeof(int?)) as List<int?>;
        /// <summary>
        /// Implicit convert from List&lt;uint?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;uint?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<uint?> value) => FromList(value, typeof(uint?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;uint?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;uint?&gt;
        /// </summary>
        public static implicit operator List<uint?>(JSONData value) => ToList(value, typeof(uint?)) as List<uint?>;
        /// <summary>
        /// Implicit convert from List&lt;long?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;long?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<long?> value) => FromList(value, typeof(long?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;long?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;long?&gt;
        /// </summary>
        public static implicit operator List<long?>(JSONData value) => ToList(value, typeof(long?)) as List<long?>;
        /// <summary>
        /// Implicit convert from List&lt;ulong?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;ulong?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<ulong?> value) => FromList(value, typeof(ulong?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;ulong?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;ulong?&gt;
        /// </summary>
        public static implicit operator List<ulong?>(JSONData value) => ToList(value, typeof(ulong?)) as List<ulong?>;
        /// <summary>
        /// Implicit convert from List&lt;float?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;float?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<float?> value) => FromList(value, typeof(float?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;float?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;float?&gt;
        /// </summary>
        public static implicit operator List<float?>(JSONData value) => ToList(value, typeof(float?)) as List<float?>;
        /// <summary>
        /// Implicit convert from List&lt;double?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;double?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<double?> value) => FromList(value, typeof(double?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;double?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;double?&gt;
        /// </summary>
        public static implicit operator List<double?>(JSONData value) => ToList(value, typeof(double?)) as List<double?>;
        /// <summary>
        /// Implicit convert from List&lt;bool?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;bool?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<bool?> value) => FromList(value, typeof(bool?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;bool?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;bool?&gt;
        /// </summary>
        public static implicit operator List<bool?>(JSONData value) => ToList(value, typeof(bool?)) as List<bool?>;
        /// <summary>
        /// Implicit convert from List&lt;char?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把List&lt;char?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(List<char?> value) => FromList(value, typeof(char?));
        /// <summary>
        /// Implicit convert from JSONData to List&lt;char?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换List&lt;char?&gt;
        /// </summary>
        public static implicit operator List<char?>(JSONData value) => ToList(value, typeof(char?)) as List<char?>;
#endif //SUPPORT_NULLABLE
        static bool IsInValidDictionary(ref JSONData value)
        {
            if (value == null) return true;
            if (value.dataType == DataType.DataTypeString && value.strValue != null)
            {
                //1 Normal JSON string format. e.g.`{"aa":1,"bb":2}`
                JSONData value2 = KissJson.ToJSONData(value.strValue);
                if (value2 == null)//We still return an empty JSONData with no item
                {
                    value.dataType = DataType.DataTypeDictionary;
                    value.dictValue = new Dictionary<string, JSONData>();
                    //2 Split by `|` and `_`. e.g. `aa_1|bb_2`
                    if (value.strValue.IndexOf('|') >= 0 && value.strValue.IndexOf('_') >= 0)
                    {
                        foreach (string item in value.strValue.Split('|'))
                        {
                            string[] item2 = item.Split('_');
                            if (item2.Length == 2)
                                value.dictValue[item2[0]] = item2[1];
                        }
                    }
                    //3 Split by `,`. e.g. `aa,1,bb,2`
                    else if (value.strValue.IndexOf(',') >= 0)
                    {
                        string[] item = value.strValue.Split(',');
                        int count = item.Length - 1;
                        for (int i = 0; i < count; i += 2)
                            value.dictValue[item[i]] = item[i + 1];
                    }
                    return false;
                }
                else if (value2.dataType == DataType.DataTypeDictionary)
                {
                    value.dataType = DataType.DataTypeDictionary;
                    value.dictValue = value2.dictValue;
                    return false;
                }
            }
            return value == null || value.dataType != DataType.DataTypeDictionary;
        }
        static JSONData FromDictionary(IDictionary value, Type type)
        {
            JSONData data = NewDictionary();
            MethodInfo mi = GetImplicitFrom(type);
            if (mi != null)
            {
                IDictionary dic = data.dictValue;
                foreach (DictionaryEntry item in value)
                    dic.Add(item.Key, mi.Invoke(null, new object[1] { item.Value }));
            }
            else if (ThrowException) throw new Exception($"{type.Name} can't convert to JSONData");
            return data;
        }
        static Dictionary<Type, ConstructorInfo> mTypeDictionarys = new Dictionary<Type, ConstructorInfo>();
        internal static object CreateDictionary(Type type)
        {
            if (!mTypeDictionarys.TryGetValue(type, out ConstructorInfo ci))
            {
                Type t = typeof(Dictionary<,>).MakeGenericType(typeof(string), type);
                ci = t.GetConstructor(Type.EmptyTypes);
                mTypeDictionarys[type] = ci;
            }
            return ci.Invoke(null);
        }
        static IDictionary ToDictionary(JSONData value, Type type)
        {
            if (IsInValidDictionary(ref value)) return null;
            IDictionary data = CreateDictionary(type) as IDictionary;
            MethodInfo mi = GetImplicit(type);
            if (mi != null)
            {
                foreach (var item in value.dictValue)
                    data.Add(item.Key, mi.Invoke(null, new object[1] { item.Value }));
            }
            else if (ThrowException) throw new Exception($"JSONData can't convert to {type.Name}");
            return data;
        }
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, bool&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, bool&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, bool> value) => FromDictionary(value, typeof(bool));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, bool&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, bool&gt;
        /// </summary>
        public static implicit operator Dictionary<string, bool>(JSONData value) => ToDictionary(value, typeof(bool)) as Dictionary<string, bool>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, byte&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, byte&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, byte> value) => FromDictionary(value, typeof(byte));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, byte&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, byte&gt;
        /// </summary>
        public static implicit operator Dictionary<string, byte>(JSONData value) => ToDictionary(value, typeof(byte)) as Dictionary<string, byte>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, sbyte&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, sbyte&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, sbyte> value) => FromDictionary(value, typeof(sbyte));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, sbyte&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, sbyte&gt;
        /// </summary>
        public static implicit operator Dictionary<string, sbyte>(JSONData value) => ToDictionary(value, typeof(sbyte)) as Dictionary<string, sbyte>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, short&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, short&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, short> value) => FromDictionary(value, typeof(short));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, short&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, short&gt;
        /// </summary>
        public static implicit operator Dictionary<string, short>(JSONData value) => ToDictionary(value, typeof(short)) as Dictionary<string, short>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, ushort&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, ushort&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, ushort> value) => FromDictionary(value, typeof(ushort));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, ushort&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, ushort&gt;
        /// </summary>
        public static implicit operator Dictionary<string, ushort>(JSONData value) => ToDictionary(value, typeof(ushort)) as Dictionary<string, ushort>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, int&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, int&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, int> value) => FromDictionary(value, typeof(int));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, int&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, int&gt;
        /// </summary>
        public static implicit operator Dictionary<string, int>(JSONData value) => ToDictionary(value, typeof(int)) as Dictionary<string, int>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, uint&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, uint&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, uint> value) => FromDictionary(value, typeof(uint));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, uint&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, uint&gt;
        /// </summary>
        public static implicit operator Dictionary<string, uint>(JSONData value) => ToDictionary(value, typeof(uint)) as Dictionary<string, uint>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, long&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, long&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, long> value) => FromDictionary(value, typeof(long));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, long&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, long&gt;
        /// </summary>
        public static implicit operator Dictionary<string, long>(JSONData value) => ToDictionary(value, typeof(long)) as Dictionary<string, long>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, ulong&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, ulong&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, ulong> value) => FromDictionary(value, typeof(ulong));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, ulong&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, ulong&gt;
        /// </summary>
        public static implicit operator Dictionary<string, ulong>(JSONData value) => ToDictionary(value, typeof(ulong)) as Dictionary<string, ulong>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, float&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, float&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, float> value) => FromDictionary(value, typeof(float));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, float&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, float&gt;
        /// </summary>
        public static implicit operator Dictionary<string, float>(JSONData value) => ToDictionary(value, typeof(float)) as Dictionary<string, float>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, double&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, double&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, double> value) => FromDictionary(value, typeof(double));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, double&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, double&gt;
        /// </summary>
        public static implicit operator Dictionary<string, double>(JSONData value) => ToDictionary(value, typeof(double)) as Dictionary<string, double>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, string&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, string&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, string> value) => FromDictionary(value, typeof(string));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, string&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, string&gt;
        /// </summary>
        public static implicit operator Dictionary<string, string>(JSONData value) => ToDictionary(value, typeof(string)) as Dictionary<string, string>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, DateTime&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, DateTime&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, DateTime> value) => FromDictionary(value, typeof(DateTime));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, DateTime&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, DateTime&gt;
        /// </summary>
        public static implicit operator Dictionary<string, DateTime>(JSONData value) => ToDictionary(value, typeof(DateTime)) as Dictionary<string, DateTime>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, object&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, object&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, object> value)
        {
            JSONData data = NewDictionary();
            IDictionary dic = data.dictValue;
            foreach (var item in value)
            {
                object o = item.Value;
                if (o == null)
                    dic.Add(item.Key, new JSONData() { dataType = DataType.DataTypeNull });
                else
                {
                    MethodInfo mi = GetImplicitFrom(o.GetType());
                    if (mi != null)
                        dic.Add(item.Key, mi.Invoke(null, new object[1] { o }));
                    else if (ThrowException) throw new Exception($"{o.GetType().Name} can't convert to JSONData");
                }
            }
            return data;
        }
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, object&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, object&gt;
        /// </summary>
        public static implicit operator Dictionary<string, object>(JSONData value)
        {
            if (IsInValidDictionary(ref value)) return null;
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (var item in value.dictValue)
                data.Add(item.Key, item.Value.Value);
            return data;
        }
#if SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, bool?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, bool?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, bool?> value) => FromDictionary(value, typeof(bool?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, bool?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, bool?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, bool?>(JSONData value) => ToDictionary(value, typeof(bool?)) as Dictionary<string, bool?>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, byte?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, byte?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, byte?> value) => FromDictionary(value, typeof(byte?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, byte?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, byte?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, byte?>(JSONData value) => ToDictionary(value, typeof(byte?)) as Dictionary<string, byte?>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, sbyte?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, sbyte?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, sbyte?> value) => FromDictionary(value, typeof(sbyte?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, sbyte?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, sbyte?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, sbyte?>(JSONData value) => ToDictionary(value, typeof(sbyte?)) as Dictionary<string, sbyte?>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, short?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, short?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, short?> value) => FromDictionary(value, typeof(short?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, short?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, short?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, short?>(JSONData value) => ToDictionary(value, typeof(short?)) as Dictionary<string, short?>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, ushort?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, ushort?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, ushort?> value) => FromDictionary(value, typeof(ushort?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, ushort?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, ushort?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, ushort?>(JSONData value) => ToDictionary(value, typeof(ushort?)) as Dictionary<string, ushort?>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, int?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, int?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, int?> value) => FromDictionary(value, typeof(int?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, int?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, int?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, int?>(JSONData value) => ToDictionary(value, typeof(int?)) as Dictionary<string, int?>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, uint?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, uint?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, uint?> value) => FromDictionary(value, typeof(uint?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, uint?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, uint?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, uint?>(JSONData value) => ToDictionary(value, typeof(uint?)) as Dictionary<string, uint?>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, long?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, long?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, long?> value) => FromDictionary(value, typeof(long?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, long?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, long?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, long?>(JSONData value) => ToDictionary(value, typeof(long?)) as Dictionary<string, long?>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, ulong?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, ulong?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, ulong?> value) => FromDictionary(value, typeof(ulong?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, ulong?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, ulong?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, ulong?>(JSONData value) => ToDictionary(value, typeof(ulong?)) as Dictionary<string, ulong?>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, float?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, float?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, float?> value) => FromDictionary(value, typeof(float?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, float?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, float?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, float?>(JSONData value) => ToDictionary(value, typeof(float?)) as Dictionary<string, float?>;
        /// <summary>
        /// Implicit convert from Dictionary&lt;string, double?&gt; to JSONData;
        /// <br/><br/>Chinese:<br/>把Dictionary&lt;string, double?&gt;隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Dictionary<string, double?> value) => FromDictionary(value, typeof(double?));
        /// <summary>
        /// Implicit convert from JSONData to Dictionary&lt;string, double?&gt;
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换Dictionary&lt;string, double?&gt;
        /// </summary>
        public static implicit operator Dictionary<string, double?>(JSONData value) => ToDictionary(value, typeof(double?)) as Dictionary<string, double?>;
#endif //SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from byte to JSONData;
        /// <br/><br/>Chinese:<br/>把byte隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(byte value) => new JSONData() { dataType = DataType.DataTypeInt, iValue = value };
        /// <summary>
        /// Implicit convert from JSONData to byte
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换byte;
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
                case DataType.DataTypeDecimal: return (byte)value.mValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (byte)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (byte)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (byte)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (byte)value.dValueNullable;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToByte(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToByte(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to byte");
            return default;
        }
#if SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from byte? to JSONData
        /// <br/><br/>Chinese:<br/>把byte?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(byte? value) => new JSONData() { dataType = DataType.DataTypeIntNullable, iValueNullable = value };
        /// <summary>
        /// Implicit convert from JSONData to byte?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换byte?
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
                case DataType.DataTypeDecimal: return (byte?)value.mValue;
                case DataType.DataTypeIntNullable: return (byte?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (byte?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (byte?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (byte?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToByte(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToByte(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to byte?");
            return default;
        }
#endif //SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from sbyte to JSONData
        /// <br/><br/>Chinese:<br/>把sbyte隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(sbyte value) => new JSONData() { dataType = DataType.DataTypeInt, iValue = value };
        /// <summary>
        /// Implicit convert from JSONData to sbyte
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换sbyte
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
                case DataType.DataTypeDecimal: return (sbyte)value.mValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (sbyte)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (sbyte)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (sbyte)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (sbyte)value.dValueNullable;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToSByte(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToSByte(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to sbyte");
            return default;
        }
#if SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from JSONData to sbyte?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换sbyte?
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
                case DataType.DataTypeDecimal: return (sbyte?)value.mValue;
                case DataType.DataTypeIntNullable: return (sbyte?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (sbyte?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (sbyte?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (sbyte?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToSByte(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToSByte(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to sbyte?");
            return default;
        }
        /// <summary>
        /// Implicit convert from sbyte? to JSONData
        /// <br/><br/>Chinese:<br/>把sbyte?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(sbyte? value) => new JSONData() { dataType = DataType.DataTypeIntNullable, iValueNullable = value };
#endif //SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from short to JSONData
        /// <br/><br/>Chinese:<br/>把short隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(short value) => new JSONData() { dataType = DataType.DataTypeInt, iValue = value };
        /// <summary>
        /// Implicit convert from JSONData to short
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换short
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
                case DataType.DataTypeDecimal: return (short)value.mValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (short)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (short)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (short)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (short)value.dValueNullable;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToInt16(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToInt16(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to short");
            return default;
        }
        /// <summary>
        /// Implicit convert from ushort to JSONData
        /// <br/><br/>Chinese:<br/>把ushort隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(ushort value) => new JSONData() { dataType = DataType.DataTypeInt, iValue = value };
        /// <summary>
        /// Implicit convert from JSONData to ushort
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换ushort
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
                case DataType.DataTypeDecimal: return (ushort)value.mValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (ushort)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (ushort)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (ushort)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (ushort)value.dValueNullable;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToUInt16(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToUInt16(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to ushort");
            return default;
        }
        /// <summary>
        /// Implicit convert from DateTime to JSONData
        /// <br/><br/>Chinese:<br/>把DateTime隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(DateTime value) => new JSONData() { dataType = DataType.DataTypeString, strValue = Convert.ToString(value, KissJson.CultureForConvertDateTime) };
        /// <summary>
        /// Implicit convert from JSONData to DateTime
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换DateTime
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
                try
                {
                    return Convert.ToDateTime(value.strValue, KissJson.CultureForConvertDateTime);
                }
                catch
                {
                    try
                    {
                        return Convert.ToDateTime(value.strValue);//Try again
                    }
                    catch
                    {
                    }
                }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to DateTime");
            return default;
        }
#if SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from DateTime? to JSONData
        /// <br/><br/>Chinese:<br/>把DateTime?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(DateTime? value) => new JSONData() { dataType = DataType.DataTypeString, strValue = value != null ? Convert.ToString(value.Value, KissJson.CultureForConvertDateTime) : null };
        /// <summary>
        /// Implicit convert from JSONData to DateTime?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换DateTime?
        /// </summary>
        public static implicit operator DateTime?(JSONData value)
        {
            if (value == null)
                return null;
            if (value.dataType == DataType.DataTypeString)
            {
                if (value.strValue == null)
                    return null;
                try
                {
                    return Convert.ToDateTime(value.strValue, KissJson.CultureForConvertDateTime);
                }
                catch
                {
                    try
                    {
                        return Convert.ToDateTime(value.strValue);//Try again
                    }
                    catch
                    {
                    }
                }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to DateTime");
            return default;
        }
#endif //SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from Enum to JSONData
        /// <br/><br/>Chinese:<br/>把枚举隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(Enum value)
        {
            if (value == null)
                return new JSONData() { dataType = DataType.DataTypeInt, iValue = 0 };
            switch (value.GetType().GetEnumUnderlyingType().Name)
            {
                case "Byte":
                case "SByte":
                case "Int16":
                case "UInt16":
                case "Int32": return new JSONData() { dataType = DataType.DataTypeInt, iValue = Convert.ToInt32(value) };
                case "UInt32":
                case "UInt64": return new JSONData() { dataType = DataType.DataTypeULong, ulValue = Convert.ToUInt64(value) };
                default: return new JSONData() { dataType = DataType.DataTypeLong, lValue = Convert.ToInt64(value) };
            }
        }
        /// <summary>
        /// Implicit convert from int to JSONData
        /// <br/><br/>Chinese:<br/>把int隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(int value) => new JSONData() { dataType = DataType.DataTypeInt, iValue = value };
        /// <summary>
        /// Implicit convert from JSONData to int
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换int
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
                case DataType.DataTypeDecimal: return (int)value.mValue;
                case DataType.DataTypeBoolean: return value.bValue ? 1 : 0;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (int)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (int)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (int)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (int)value.dValueNullable;
                case DataType.DataTypeBooleanNullable: return value.bValueNullable.Value ? 1 : 0;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToInt32(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToInt32(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to int");
            return default;
        }
        /// <summary>
        /// Override operator +
        /// <br/><br/>Chinese:<br/>重载+运算符
        /// </summary>
        public static string operator +(JSONData a, string b) => a.ToJson() + b;
        /// <summary>
        /// Override operator +
        /// <br/><br/>Chinese:<br/>重载+运算符
        /// </summary>
        public static string operator +(string a, JSONData b) => a + b.ToJson();
        /// <summary>
        /// Override operator +
        /// <br/><br/>Chinese:<br/>重载+运算符
        /// </summary>
        public static JSONData operator +(JSONData a, JSONData b)
        {
            JSONData ret = new JSONData();
            if (a.dataType == b.dataType)
            {
                ret.dataType = a.dataType;
                switch(ret.dataType)
                {
                    case DataType.DataTypeInt: ret.iValue = a.iValue + b.iValue; break;
                    case DataType.DataTypeLong: ret.lValue = a.lValue + b.lValue; break;
                    case DataType.DataTypeULong: ret.ulValue = a.ulValue + b.ulValue; break;
                    case DataType.DataTypeDouble: ret.dValue = a.dValue + b.dValue; break;
                    case DataType.DataTypeDecimal: ret.mValue = a.mValue + b.mValue; break;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: ret.iValueNullable = a.iValueNullable + b.iValueNullable; break;
                    case DataType.DataTypeLongNullable: ret.lValueNullable = a.lValueNullable + b.lValueNullable; break;
                    case DataType.DataTypeULongNullable: ret.ulValueNullable = a.ulValueNullable + b.ulValueNullable; break;
                    case DataType.DataTypeDoubleNullable: ret.dValueNullable = a.dValueNullable + b.dValueNullable; break;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString: ret.strValue = a.strValue + b.strValue; break;
                    case DataType.DataTypeList:
                        ret.listValue = new List<JSONData>();
                        foreach (JSONData one in a)
                            ret.listValue.Add(one);
                        foreach (JSONData one in b)
                            ret.listValue.Add(one);
                        break;
                    case DataType.DataTypeDictionary:
                        ret.dictValue = new Dictionary<string, JSONData>();
                        foreach (var one in a.dictValue)
                            ret.dictValue[one.Key] = one.Value;
                        foreach (var one in b.dictValue)
                            ret.dictValue[one.Key] = one.Value;
                        break;
                    default:
                        if (ThrowException) throw new Exception($"operator + error: `{a.ToJson()}` + `{b.ToJson()}`");
                        ret.dataType = DataType.DataTypeNull;
                        break;
                }
            }
            else
            {
                if (a.dataType == DataType.DataTypeString)
                {
                    ret.dataType = DataType.DataTypeString;
                    ret.strValue = a.strValue + b.ToJson();
                }
                else if (b.dataType == DataType.DataTypeString)
                {
                    ret.dataType = DataType.DataTypeString;
                    ret.strValue = a.ToJson() + b.strValue;
                }
                else
                {
                    switch (GetDecimalType(a.ValueType, b.ValueType))
                    {
                        case T_Long:
                        case T_UInt: ret.dataType = DataType.DataTypeLong; ret.lValue = Convert.ToInt64((decimal)a + (decimal)b); break;
                        case T_Double:
                        case T_Float: ret.dataType = DataType.DataTypeDouble; ret.dValue = Convert.ToDouble((decimal)a + (decimal)b); break;
                        case T_ULong: ret.dataType = DataType.DataTypeULong; ret.ulValue = Convert.ToUInt64((decimal)a + (decimal)b); break;
                        case T_Decimal: ret.dataType = DataType.DataTypeDecimal; ret.mValue = (decimal)a + (decimal)b; break;
                        default: ret.dataType = DataType.DataTypeInt; ret.iValue = Convert.ToInt32((decimal)a + (decimal)b); break;
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// Override operator -
        /// <br/><br/>Chinese:<br/>重载-运算符
        /// </summary>
        public static JSONData operator -(JSONData a, JSONData b)
        {
            JSONData ret = new JSONData();
            if (a.dataType == b.dataType)
            {
                ret.dataType = a.dataType;
                switch (ret.dataType)
                {
                    case DataType.DataTypeInt: ret.iValue = a.iValue - b.iValue; break;
                    case DataType.DataTypeLong: ret.lValue = a.lValue - b.lValue; break;
                    case DataType.DataTypeULong: ret.ulValue = a.ulValue - b.ulValue; break;
                    case DataType.DataTypeDouble: ret.dValue = a.dValue - b.dValue; break;
                    case DataType.DataTypeDecimal: ret.mValue = a.mValue - b.mValue; break;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: ret.iValueNullable = a.iValueNullable - b.iValueNullable; break;
                    case DataType.DataTypeLongNullable: ret.lValueNullable = a.lValueNullable - b.lValueNullable; break;
                    case DataType.DataTypeULongNullable: ret.ulValueNullable = a.ulValueNullable - b.ulValueNullable; break;
                    case DataType.DataTypeDoubleNullable: ret.dValueNullable = a.dValueNullable - b.dValueNullable; break;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString: ret.strValue = ((decimal)a - (decimal)b).ToString(KissJson.CultureForConvertFloatAndDouble); break;
                    default:
                        if (ThrowException) throw new Exception($"operator - error: `{a.ToJson()}` - `{b.ToJson()}`");
                        ret.dataType = DataType.DataTypeNull;
                        break;
                }
            }
            else
            {
                switch (GetDecimalType(a.ValueType, b.ValueType))
                {
                    case T_Long:
                    case T_UInt: ret.dataType = DataType.DataTypeLong; ret.lValue = Convert.ToInt64((decimal)a - (decimal)b); break;
                    case T_Double:
                    case T_Float: ret.dataType = DataType.DataTypeDouble; ret.dValue = Convert.ToDouble((decimal)a - (decimal)b); break;
                    case T_ULong: ret.dataType = DataType.DataTypeULong; ret.ulValue = Convert.ToUInt64((decimal)a - (decimal)b); break;
                    case T_Decimal: ret.dataType = DataType.DataTypeDecimal; ret.mValue = (decimal)a - (decimal)b; break;
                    default: ret.dataType = DataType.DataTypeInt; ret.iValue = Convert.ToInt32((decimal)a - (decimal)b); break;
                }
            }
            return ret;
        }
        /// <summary>
        /// Override operator *
        /// <br/><br/>Chinese:<br/>重载*运算符
        /// </summary>
        public static JSONData operator *(JSONData a, JSONData b)
        {
            JSONData ret = new JSONData();
            if (a.dataType == b.dataType)
            {
                ret.dataType = a.dataType;
                switch (ret.dataType)
                {
                    case DataType.DataTypeInt: ret.iValue = a.iValue * b.iValue; break;
                    case DataType.DataTypeLong: ret.lValue = a.lValue * b.lValue; break;
                    case DataType.DataTypeULong: ret.ulValue = a.ulValue * b.ulValue; break;
                    case DataType.DataTypeDouble: ret.dValue = a.dValue * b.dValue; break;
                    case DataType.DataTypeDecimal: ret.mValue = a.mValue * b.mValue; break;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: ret.iValueNullable = a.iValueNullable * b.iValueNullable; break;
                    case DataType.DataTypeLongNullable: ret.lValueNullable = a.lValueNullable * b.lValueNullable; break;
                    case DataType.DataTypeULongNullable: ret.ulValueNullable = a.ulValueNullable * b.ulValueNullable; break;
                    case DataType.DataTypeDoubleNullable: ret.dValueNullable = a.dValueNullable * b.dValueNullable; break;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString: ret.strValue = ((decimal)a * (decimal)b).ToString(KissJson.CultureForConvertFloatAndDouble); break;
                    default:
                        if (ThrowException) throw new Exception($"operator * error: `{a.ToJson()}` * `{b.ToJson()}`");
                        ret.dataType = DataType.DataTypeNull;
                        break;
                }
            }
            else
            {
                switch (GetDecimalType(a.ValueType, b.ValueType))
                {
                    case T_Long:
                    case T_UInt: ret.dataType = DataType.DataTypeLong; ret.lValue = Convert.ToInt64((decimal)a * (decimal)b); break;
                    case T_Double:
                    case T_Float: ret.dataType = DataType.DataTypeDouble; ret.dValue = Convert.ToDouble((decimal)a * (decimal)b); break;
                    case T_ULong: ret.dataType = DataType.DataTypeULong; ret.ulValue = Convert.ToUInt64((decimal)a * (decimal)b); break;
                    case T_Decimal: ret.dataType = DataType.DataTypeDecimal; ret.mValue = (decimal)a * (decimal)b; break;
                    default: ret.dataType = DataType.DataTypeInt; ret.iValue = Convert.ToInt32((decimal)a * (decimal)b); break;
                }
            }
            return ret;
        }
        /// <summary>
        /// Override operator /
        /// <br/><br/>Chinese:<br/>重载/运算符
        /// </summary>
        public static JSONData operator /(JSONData a, JSONData b)
        {
            JSONData ret = new JSONData();
            if (a.dataType == b.dataType)
            {
                ret.dataType = a.dataType;
                switch (ret.dataType)
                {
                    case DataType.DataTypeInt: ret.iValue = a.iValue / b.iValue; break;
                    case DataType.DataTypeLong: ret.lValue = a.lValue / b.lValue; break;
                    case DataType.DataTypeULong: ret.ulValue = a.ulValue / b.ulValue; break;
                    case DataType.DataTypeDouble: ret.dValue = a.dValue / b.dValue; break;
                    case DataType.DataTypeDecimal: ret.mValue = a.mValue / b.mValue; break;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: ret.iValueNullable = a.iValueNullable / b.iValueNullable; break;
                    case DataType.DataTypeLongNullable: ret.lValueNullable = a.lValueNullable / b.lValueNullable; break;
                    case DataType.DataTypeULongNullable: ret.ulValueNullable = a.ulValueNullable / b.ulValueNullable; break;
                    case DataType.DataTypeDoubleNullable: ret.dValueNullable = a.dValueNullable / b.dValueNullable; break;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString: ret.strValue = ((decimal)a / (decimal)b).ToString(KissJson.CultureForConvertFloatAndDouble); break;
                    default:
                        if (ThrowException) throw new Exception($"operator / error: `{a.ToJson()}` / `{b.ToJson()}`");
                        ret.dataType = DataType.DataTypeNull;
                        break;
                }
            }
            else
            {
                switch (GetDecimalType(a.ValueType, b.ValueType))
                {
                    case T_Long:
                    case T_UInt: ret.dataType = DataType.DataTypeLong; ret.lValue = Convert.ToInt64((decimal)a / (decimal)b); break;
                    case T_Double:
                    case T_Float: ret.dataType = DataType.DataTypeDouble; ret.dValue = Convert.ToDouble((decimal)a / (decimal)b); break;
                    case T_ULong: ret.dataType = DataType.DataTypeULong; ret.ulValue = Convert.ToUInt64((decimal)a / (decimal)b); break;
                    case T_Decimal: ret.dataType = DataType.DataTypeDecimal; ret.mValue = (decimal)a / (decimal)b; break;
                    default: ret.dataType = DataType.DataTypeInt; ret.iValue = Convert.ToInt32((decimal)a / (decimal)b); break;
                }
            }
            return ret;
        }
        /// <summary>
        /// Override operator &amp;
        /// <br/><br/>Chinese:<br/>重载&amp;运算符
        /// </summary>
        public static JSONData operator &(JSONData a, JSONData b)
        {
            if (a.dataType == b.dataType)
            {
                switch (a.dataType)
                {
                    case DataType.DataTypeInt: return a.iValue & b.iValue;
                    case DataType.DataTypeLong: return a.lValue & b.lValue;
                    case DataType.DataTypeULong: return a.ulValue & b.ulValue;
                    case DataType.DataTypeBoolean: return a.bValue & b.bValue;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: return a.iValueNullable & b.iValueNullable;
                    case DataType.DataTypeLongNullable: return a.lValueNullable & b.lValueNullable;
                    case DataType.DataTypeULongNullable: return a.ulValueNullable & b.ulValueNullable;
                    case DataType.DataTypeBooleanNullable: return a.bValueNullable & b.bValueNullable;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString: return Convert.ToInt32(a.strValue) & Convert.ToInt32(b.strValue);
                    default:
                        break;
                }
            }
            if (a.dataType == DataType.DataTypeULong) return a.ulValue & (ulong)b;
            if (b.dataType == DataType.DataTypeULong) return b.ulValue & (ulong)a;
            if (a.dataType == DataType.DataTypeLong) return a.lValue & (long)b;
            if (b.dataType == DataType.DataTypeLong) return b.lValue & (long)a;
            if (a.dataType == DataType.DataTypeInt) return a.iValue & (int)b;
            if (b.dataType == DataType.DataTypeInt) return b.iValue & (int)a;
            return (int)a & (int)b;
        }
        /// <summary>
        /// Override operator |
        /// <br/><br/>Chinese:<br/>重载|运算符
        /// </summary>
        public static JSONData operator |(JSONData a, JSONData b)
        {
            if (a.dataType == b.dataType)
            {
                switch (a.dataType)
                {
                    case DataType.DataTypeInt: return a.iValue | b.iValue;
                    case DataType.DataTypeLong: return a.lValue | b.lValue;
                    case DataType.DataTypeULong: return a.ulValue | b.ulValue;
                    case DataType.DataTypeBoolean: return a.bValue | b.bValue;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: return a.iValueNullable | b.iValueNullable;
                    case DataType.DataTypeLongNullable: return a.lValueNullable | b.lValueNullable;
                    case DataType.DataTypeULongNullable: return a.ulValueNullable | b.ulValueNullable;
                    case DataType.DataTypeBooleanNullable: return a.bValueNullable | b.bValueNullable;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString: return Convert.ToInt32(a.strValue) | Convert.ToInt32(b.strValue);
                    default:
                        break;
                }
            }
            if (a.dataType == DataType.DataTypeULong) return a.ulValue | (ulong)b;
            if (b.dataType == DataType.DataTypeULong) return b.ulValue | (ulong)a;
            if (a.dataType == DataType.DataTypeLong) return a.lValue | (long)b;
            if (b.dataType == DataType.DataTypeLong) return b.lValue | (long)a;
            if (a.dataType == DataType.DataTypeInt) return a.iValue | (int)b;
            if (b.dataType == DataType.DataTypeInt) return b.iValue | (int)a;
            return (int)a | (int)b;
        }
        /// <summary>
        /// Override operator ^
        /// <br/><br/>Chinese:<br/>重载^运算符
        /// </summary>
        public static JSONData operator ^(JSONData a, JSONData b)
        {
            if (a.dataType == b.dataType)
            {
                switch (a.dataType)
                {
                    case DataType.DataTypeInt: return a.iValue ^ b.iValue;
                    case DataType.DataTypeLong: return a.lValue ^ b.lValue;
                    case DataType.DataTypeULong: return a.ulValue ^ b.ulValue;
                    case DataType.DataTypeBoolean: return a.bValue ^ b.bValue;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: return a.iValueNullable ^ b.iValueNullable;
                    case DataType.DataTypeLongNullable: return a.lValueNullable ^ b.lValueNullable;
                    case DataType.DataTypeULongNullable: return a.ulValueNullable ^ b.ulValueNullable;
                    case DataType.DataTypeBooleanNullable: return a.bValueNullable ^ b.bValueNullable;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString: return Convert.ToInt32(a.strValue) ^ Convert.ToInt32(b.strValue);
                    default:
                        break;
                }
            }
            if (a.dataType == DataType.DataTypeULong) return a.ulValue ^ (ulong)b;
            if (b.dataType == DataType.DataTypeULong) return b.ulValue ^ (ulong)a;
            if (a.dataType == DataType.DataTypeLong) return a.lValue ^ (long)b;
            if (b.dataType == DataType.DataTypeLong) return b.lValue ^ (long)a;
            if (a.dataType == DataType.DataTypeInt) return a.iValue ^ (int)b;
            if (b.dataType == DataType.DataTypeInt) return b.iValue ^ (int)a;
            return (int)a ^ (int)b;
        }
        /// <summary>
        /// Override operator &lt;&lt;
        /// <br/><br/>Chinese:<br/>重载&lt;&lt;运算符
        /// </summary>
        public static JSONData operator <<(JSONData a, int b)
        {
            switch (a.dataType)
            {
                case DataType.DataTypeInt: return a.iValue << b;
                case DataType.DataTypeLong: return a.lValue << b;
                case DataType.DataTypeULong: return a.ulValue << b;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return a.iValueNullable << b;
                case DataType.DataTypeLongNullable: return a.lValueNullable << b;
                case DataType.DataTypeULongNullable: return a.ulValueNullable << b;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString: return Convert.ToInt32(a.strValue) << b;
                default: return (int)a << b;
            }
        }
        /// <summary>
        /// Override operator &gt;&gt;
        /// <br/><br/>Chinese:<br/>重载&gt;&gt;运算符
        /// </summary>
        public static JSONData operator >>(JSONData a, int b)
        {
            switch (a.dataType)
            {
                case DataType.DataTypeInt: return a.iValue >> b;
                case DataType.DataTypeLong: return a.lValue >> b;
                case DataType.DataTypeULong: return a.ulValue >> b;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return a.iValueNullable >> b;
                case DataType.DataTypeLongNullable: return a.lValueNullable >> b;
                case DataType.DataTypeULongNullable: return a.ulValueNullable >> b;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString: return Convert.ToInt32(a.strValue) >> b;
                default: return (int)a >> b;
            }
        }
        /// <summary>
        /// Override operator ~
        /// <br/><br/>Chinese:<br/>重载~运算符
        /// </summary>
        public static JSONData operator ~(JSONData a)
        {
            switch (a.dataType)
            {
                case DataType.DataTypeInt: return ~a.iValue;
                case DataType.DataTypeLong: return ~a.lValue;
                case DataType.DataTypeULong: return ~a.ulValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return ~a.iValueNullable;
                case DataType.DataTypeLongNullable: return ~a.lValueNullable;
                case DataType.DataTypeULongNullable: return ~a.ulValueNullable;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString: return ~Convert.ToInt32(a.strValue);
                default: return ~(int)a;
            }
        }
        /// <summary>
        /// Override operator %
        /// <br/><br/>Chinese:<br/>重载%运算符
        /// </summary>
        public static JSONData operator %(JSONData a, JSONData b)
        {
            JSONData ret = new JSONData();
            if (a.dataType == b.dataType)
            {
                ret.dataType = a.dataType;
                switch (ret.dataType)
                {
                    case DataType.DataTypeInt: ret.iValue = a.iValue % b.iValue; break;
                    case DataType.DataTypeLong: ret.lValue = a.lValue % b.lValue; break;
                    case DataType.DataTypeULong: ret.ulValue = a.ulValue % b.ulValue; break;
                    case DataType.DataTypeDouble: ret.dValue = a.dValue % b.dValue; break;
                    case DataType.DataTypeDecimal: ret.mValue = a.mValue % b.mValue; break;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: ret.iValueNullable = a.iValueNullable % b.iValueNullable; break;
                    case DataType.DataTypeLongNullable: ret.lValueNullable = a.lValueNullable % b.lValueNullable; break;
                    case DataType.DataTypeULongNullable: ret.ulValueNullable = a.ulValueNullable % b.ulValueNullable; break;
                    case DataType.DataTypeDoubleNullable: ret.dValueNullable = a.dValueNullable % b.dValueNullable; break;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString: ret.strValue = ((decimal)a % (decimal)b).ToString(KissJson.CultureForConvertFloatAndDouble); break;
                    default:
                        if (ThrowException) throw new Exception($"operator % error: `{a.ToJson()}` % `{b.ToJson()}`");
                        ret.dataType = DataType.DataTypeNull;
                        break;
                }
            }
            else
            {
                switch (GetDecimalType(a.ValueType, b.ValueType))
                {
                    case T_Long:
                    case T_UInt: ret.dataType = DataType.DataTypeLong; ret.lValue = Convert.ToInt64((decimal)a % (decimal)b); break;
                    case T_Double:
                    case T_Float: ret.dataType = DataType.DataTypeDouble; ret.dValue = Convert.ToDouble((decimal)a % (decimal)b); break;
                    case T_ULong: ret.dataType = DataType.DataTypeULong; ret.ulValue = Convert.ToUInt64((decimal)a % (decimal)b); break;
                    case T_Decimal: ret.dataType = DataType.DataTypeDecimal; ret.mValue = (decimal)a % (decimal)b; break;
                    default: ret.dataType = DataType.DataTypeInt; ret.iValue = Convert.ToInt32((decimal)a % (decimal)b); break;
                }
            }
            return ret;
        }
        /// <summary>
        /// Override operator >
        /// <br/><br/>Chinese:<br/>重载>运算符
        /// </summary>
        public static bool operator >(JSONData a, JSONData b)
        {
            if (a.dataType == b.dataType)
            {
                switch (a.dataType)
                {
                    case DataType.DataTypeInt: return a.iValue > b.iValue;
                    case DataType.DataTypeLong: return a.lValue > b.lValue;
                    case DataType.DataTypeULong: return a.ulValue > b.ulValue;
                    case DataType.DataTypeDouble: return a.dValue > b.dValue;
                    case DataType.DataTypeDecimal: return a.mValue > b.mValue;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: return a.iValueNullable > b.iValueNullable;
                    case DataType.DataTypeLongNullable: return a.lValueNullable > b.lValueNullable;
                    case DataType.DataTypeULongNullable: return a.ulValueNullable > b.ulValueNullable;
                    case DataType.DataTypeDoubleNullable: return a.dValueNullable > b.dValueNullable;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString: return Convert.ToDecimal(a.strValue, KissJson.CultureForConvertFloatAndDouble)
                            > Convert.ToDecimal(b.strValue, KissJson.CultureForConvertFloatAndDouble);
                    default:
                        break;
                }
            }
            return (decimal)a > (decimal)b;
        }
        /// <summary>
        /// Override operator &lt;
        /// <br/><br/>Chinese:<br/>重载&lt;运算符
        /// </summary>
        public static bool operator <(JSONData a, JSONData b)
        {
            if (a.dataType == b.dataType)
            {
                switch (a.dataType)
                {
                    case DataType.DataTypeInt: return a.iValue < b.iValue;
                    case DataType.DataTypeLong: return a.lValue < b.lValue;
                    case DataType.DataTypeULong: return a.ulValue < b.ulValue;
                    case DataType.DataTypeDouble: return a.dValue < b.dValue;
                    case DataType.DataTypeDecimal: return a.mValue < b.mValue;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: return a.iValueNullable < b.iValueNullable;
                    case DataType.DataTypeLongNullable: return a.lValueNullable < b.lValueNullable;
                    case DataType.DataTypeULongNullable: return a.ulValueNullable < b.ulValueNullable;
                    case DataType.DataTypeDoubleNullable: return a.dValueNullable < b.dValueNullable;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString:
                        return Convert.ToDecimal(a.strValue, KissJson.CultureForConvertFloatAndDouble)
  < Convert.ToDecimal(b.strValue, KissJson.CultureForConvertFloatAndDouble);
                    default:
                        break;
                }
            }
            return (decimal)a < (decimal)b;
        }
        /// <summary>
        /// Override operator >=
        /// <br/><br/>Chinese:<br/>重载>=运算符
        /// </summary>
        public static bool operator >=(JSONData a, JSONData b)
        {
            if (a.dataType == b.dataType)
            {
                switch (a.dataType)
                {
                    case DataType.DataTypeInt: return a.iValue >= b.iValue;
                    case DataType.DataTypeLong: return a.lValue >= b.lValue;
                    case DataType.DataTypeULong: return a.ulValue >= b.ulValue;
                    case DataType.DataTypeDouble: return a.dValue >= b.dValue;
                    case DataType.DataTypeDecimal: return a.mValue >= b.mValue;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: return a.iValueNullable >= b.iValueNullable;
                    case DataType.DataTypeLongNullable: return a.lValueNullable >= b.lValueNullable;
                    case DataType.DataTypeULongNullable: return a.ulValueNullable >= b.ulValueNullable;
                    case DataType.DataTypeDoubleNullable: return a.dValueNullable >= b.dValueNullable;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString:
                        return Convert.ToDecimal(a.strValue, KissJson.CultureForConvertFloatAndDouble)
  >= Convert.ToDecimal(b.strValue, KissJson.CultureForConvertFloatAndDouble);
                    default:
                        break;
                }
            }
            return (decimal)a >= (decimal)b;
        }
        /// <summary>
        /// Override operator &lt;=
        /// <br/><br/>Chinese:<br/>重载&lt;=运算符
        /// </summary>
        public static bool operator <=(JSONData a, JSONData b)
        {
            if (a.dataType == b.dataType)
            {
                switch (a.dataType)
                {
                    case DataType.DataTypeInt: return a.iValue <= b.iValue;
                    case DataType.DataTypeLong: return a.lValue <= b.lValue;
                    case DataType.DataTypeULong: return a.ulValue <= b.ulValue;
                    case DataType.DataTypeDouble: return a.dValue <= b.dValue;
                    case DataType.DataTypeDecimal: return a.mValue <= b.mValue;
#if SUPPORT_NULLABLE
                    case DataType.DataTypeIntNullable: return a.iValueNullable <= b.iValueNullable;
                    case DataType.DataTypeLongNullable: return a.lValueNullable <= b.lValueNullable;
                    case DataType.DataTypeULongNullable: return a.ulValueNullable <= b.ulValueNullable;
                    case DataType.DataTypeDoubleNullable: return a.dValueNullable <= b.dValueNullable;
#endif //SUPPORT_NULLABLE
                    case DataType.DataTypeString:
                        return Convert.ToDecimal(a.strValue, KissJson.CultureForConvertFloatAndDouble)
  <= Convert.ToDecimal(b.strValue, KissJson.CultureForConvertFloatAndDouble);
                    default:
                        break;
                }
            }
            return (decimal)a <= (decimal)b;
        }
        /// <summary>
        /// Override Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is JSONData data) return ToJson() == data.ToJson();
            MethodInfo mi = GetImplicitFrom(obj.GetType());
            if (mi == null) return false;
            object obj2 = mi.Invoke(null, new object[1] { obj });
            if (obj2 == null) return false;
            data = obj2 as JSONData;
            return ToJson() == data.ToJson();
        }
        /// <summary>
        /// Override GetHashCode
        /// </summary>
        public override int GetHashCode() => ToJson().GetHashCode();
        /// <summary>
        /// Override operator ==
        /// <br/><br/>Chinese:<br/>重载==运算符
        /// </summary>
        public static bool operator ==(JSONData a, JSONData b) => a?.ToJson() == b?.ToJson();
        /// <summary>
        /// Override operator !=
        /// <br/><br/>Chinese:<br/>重载!=运算符
        /// </summary>
        public static bool operator !=(JSONData a, JSONData b) => a?.ToJson() != b?.ToJson();
        static int GetDecimalType(Type a, Type b)
        {
            int aIndex = _TypeList.IndexOf(a);
            int bIndex = _TypeList.IndexOf(b);

            //0. decimal
            if (aIndex == T_Decimal || bIndex == T_Decimal) return T_Decimal;
            //1. double and float
            if (aIndex == T_Double || bIndex == T_Double) return T_Double;
            if (aIndex == T_Float || bIndex == T_Float) return T_Float;
            //2. ulong
            if (aIndex == T_ULong || bIndex == T_ULong) return T_ULong;
            //3. long
            if (aIndex == T_Long || bIndex == T_Long) return T_Long;
            //4. int and uint are long.
            if ((aIndex == T_Int && bIndex == T_UInt) || (aIndex == T_UInt && bIndex == T_Int)) return T_Long;
            //5. uint and none int are uint.
            if ((aIndex == T_UInt && bIndex != T_Int) || (bIndex == T_UInt && aIndex != T_Int)) return T_UInt;
            //6 int
            return T_Int;
        }
        static List<Type> _TypeList = new List<Type>(new Type[]{
                        typeof(decimal),
                        typeof(double),
                        typeof(float),
                        typeof(long),
                        typeof(ulong),
                        typeof(int),
                        typeof(uint),
                        typeof(short),
                        typeof(ushort),
                        typeof(sbyte),
                        typeof(byte),
                        typeof(char)
                    });
        private const int T_Decimal = 0;
        private const int T_Double = 1;
        private const int T_Float = 2;
        private const int T_Long = 3;
        private const int T_ULong = 4;
        private const int T_Int = 5;
        private const int T_UInt = 6;

        /// <summary>
        /// Implicit convert from uint to JSONData
        /// <br/><br/>Chinese:<br/>把uint隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(uint value) => new JSONData() { dataType = DataType.DataTypeLong, lValue = value };
        /// <summary>
        /// Implicit convert from JSONData to uint
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换uint
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
                case DataType.DataTypeDecimal: return (uint)value.mValue;
                case DataType.DataTypeBoolean: return value.bValue ? 1u : 0u;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (uint)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (uint)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (uint)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (uint)value.dValueNullable;
                case DataType.DataTypeBooleanNullable: return value.bValueNullable.Value ? 1u : 0u;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToUInt32(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToUInt32(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to uint");
            return default;
        }
        /// <summary>
        /// Implicit convert from long to JSONData
        /// <br/><br/>Chinese:<br/>把long隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(long value) => new JSONData() { dataType = DataType.DataTypeLong, lValue = value };
        /// <summary>
        /// Implicit convert from JSONData to long
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换long
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
                case DataType.DataTypeDecimal: return (long)value.mValue;
                case DataType.DataTypeBoolean: return value.bValue ? 1L : 0L;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (long)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (long)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (long)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (long)value.dValueNullable;
                case DataType.DataTypeBooleanNullable: return value.bValueNullable.Value ? 1L : 0L;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToInt64(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToInt64(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to long");
            return default;
        }
        /// <summary>
        /// Implicit convert from ulong to JSONData
        /// <br/><br/>Chinese:<br/>把ulong隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(ulong value) => new JSONData() { dataType = DataType.DataTypeULong, ulValue = value };
        /// <summary>
        /// Implicit convert from JSONData to ulong
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换ulong
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
                case DataType.DataTypeBoolean: return value.bValue ? 1UL : 0UL;
                case DataType.DataTypeDecimal: return (ulong)value.mValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (ulong)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (ulong)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (ulong)value.lValueNullable;
                case DataType.DataTypeDoubleNullable: return (ulong)value.dValueNullable;
                case DataType.DataTypeBooleanNullable: return value.bValueNullable.Value ? 1UL : 0UL;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToUInt64(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToUInt64(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to ulong");
            return default;
        }
        /// <summary>
        /// Implicit convert from decimal to JSONData, it'll store as double, so the value may be trimed
        /// <br/><br/>Chinese:<br/>把decimal隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(decimal value) => new JSONData() { dataType = DataType.DataTypeLong, dValue = (double)value };
        /// <summary>
        /// Implicit convert from JSONData to decimal
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换decimal
        /// </summary>
        public static implicit operator decimal(JSONData value)
        {
            if (value == null)
            {
                if (ThrowException) throw new Exception("JSONData null, can't convert to decimal");
                return default;
            }
            switch (value.dataType)
            {
                case DataType.DataTypeInt: return value.iValue;
                case DataType.DataTypeLong: return value.lValue;
                case DataType.DataTypeULong: return value.ulValue;
                case DataType.DataTypeDouble: return (decimal)value.dValue;
                case DataType.DataTypeBoolean: return value.bValue ? 1M : 0M;
                case DataType.DataTypeDecimal: return value.mValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (decimal)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (decimal)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (decimal)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (decimal)value.dValueNullable;
                case DataType.DataTypeBooleanNullable: return value.bValueNullable.Value ? 1M : 0M;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToDecimal(value.strValue, KissJson.CultureForConvertFloatAndDouble);
                        else
                        {
                            try
                            {
                                return Convert.ToDecimal(value.strValue, KissJson.CultureForConvertFloatAndDouble);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to decimal");
            return default;
        }
        /// <summary>
        /// Implicit convert from double to JSONData
        /// <br/><br/>Chinese:<br/>把double隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(double value) => new JSONData() { dataType = DataType.DataTypeDouble, dValue = value };
        /// <summary>
        /// Implicit convert from JSONData to double
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换double
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
                case DataType.DataTypeDecimal: return (double)value.mValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (double)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (double)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (double)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (double)value.dValueNullable;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToDouble(value.strValue, KissJson.CultureForConvertFloatAndDouble);
                        else
                        {
                            try
                            {
                                return Convert.ToDouble(value.strValue, KissJson.CultureForConvertFloatAndDouble);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to double");
            return default;
        }
        /// <summary>
        /// Implicit convert from float to JSONData
        /// <br/><br/>Chinese:<br/>把float隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(float value) => new JSONData() { dataType = DataType.DataTypeDouble, dValue = value };
        /// <summary>
        /// Implicit convert from JSONData to float
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换float
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
                case DataType.DataTypeDecimal: return (float)value.mValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (float)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (float)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (float)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (float)value.dValueNullable;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToSingle(value.strValue, KissJson.CultureForConvertFloatAndDouble);
                        else
                        {
                            try
                            {
                                return Convert.ToSingle(value.strValue, KissJson.CultureForConvertFloatAndDouble);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to float");
            return default;
        }
        /// <summary>
        /// Implicit convert from char to JSONData
        /// <br/><br/>Chinese:<br/>把char隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(char value) => new JSONData() { dataType = DataType.DataTypeString, strValue = value.ToString() };
        /// <summary>
        /// Implicit convert from JSONData to char
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换char
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
                case DataType.DataTypeDecimal: return (char)value.mValue;
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return (char)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (char)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (char)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (char)value.dValueNullable;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (ThrowException)
                            return Convert.ToChar(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToChar(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to char");
            return default;
        }
        /// <summary>
        /// Implicit convert from string to JSONData
        /// <br/><br/>Chinese:<br/>把string隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(string value) => new JSONData() { dataType = DataType.DataTypeString, strValue = value };
        /// <summary>
        /// Implicit convert from JSONData to string
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换string
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
                case DataType.DataTypeDouble: return value.dValue.ToString(KissJson.CultureForConvertFloatAndDouble);
                case DataType.DataTypeDecimal: return value.mValue.ToString(KissJson.CultureForConvertFloatAndDouble);
                case DataType.DataTypeBoolean: return value.bValue.ToString();
#if SUPPORT_NULLABLE
                case DataType.DataTypeIntNullable: return value.iValueNullable == null ? "null" : value.iValueNullable.Value.ToString();
                case DataType.DataTypeLongNullable: return value.lValueNullable == null ? "null" : value.lValueNullable.Value.ToString();
                case DataType.DataTypeULongNullable: return value.ulValueNullable == null ? "null" : value.ulValueNullable.Value.ToString();
                case DataType.DataTypeDoubleNullable: return value.dValueNullable == null ? "null" : value.dValueNullable.Value.ToString(KissJson.CultureForConvertFloatAndDouble);
                case DataType.DataTypeBooleanNullable: return value.bValueNullable == null ? "null" : (value.bValueNullable.Value ? "true" : "false");
#endif //SUPPORT_NULLABLE
                default: return value.ToString();
            }
        }
#if SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from short? to JSONData
        /// <br/><br/>Chinese:<br/>把short?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(short? value) => new JSONData() { dataType = DataType.DataTypeIntNullable, iValueNullable = value };
        /// <summary>
        /// Implicit convert from JSONData to short?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换short?
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
                case DataType.DataTypeDecimal: return (short?)value.mValue;
                case DataType.DataTypeIntNullable: return (short?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (short?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (short?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (short?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToInt16(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToInt16(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to short?");
            return default;
        }
        /// <summary>
        /// Implicit convert from ushort? to JSONData
        /// <br/><br/>Chinese:<br/>把ushort?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(ushort? value) => new JSONData() { dataType = DataType.DataTypeIntNullable, iValueNullable = value };
        /// <summary>
        /// Implicit convert from JSONData to ushort?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换ushort?
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
                case DataType.DataTypeDecimal: return (ushort?)value.mValue;
                case DataType.DataTypeIntNullable: return (ushort?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (ushort?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (ushort?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (ushort?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToUInt16(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToUInt16(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to ushort?");
            return null;
        }
        /// <summary>
        /// Implicit convert from int? to JSONData
        /// <br/><br/>Chinese:<br/>把int?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(int? value) => new JSONData() { dataType = DataType.DataTypeIntNullable, iValueNullable = value };
        /// <summary>
        /// Implicit convert from JSONData to int?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换int?
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
                case DataType.DataTypeDecimal: return (int?)value.mValue;
                case DataType.DataTypeIntNullable: return value.iValueNullable;
                case DataType.DataTypeLongNullable: return (int?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (int?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (int?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToInt32(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToInt32(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to int?");
            return null;
        }
        /// <summary>
        /// Implicit convert from uint? to JSONData
        /// <br/><br/>Chinese:<br/>把uint?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(uint? value) => new JSONData() { dataType = DataType.DataTypeLongNullable, lValueNullable = value };
        /// <summary>
        /// Implicit convert from JSONData to uint?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换uint?
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
                case DataType.DataTypeDecimal: return (uint?)value.mValue;
                case DataType.DataTypeIntNullable: return (uint?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (uint?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (uint?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (uint?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToUInt32(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToUInt32(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to uint?");
            return null;
        }
        /// <summary>
        /// Implicit convert from long? to JSONData
        /// <br/><br/>Chinese:<br/>把long?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(long? value) => new JSONData() { dataType = DataType.DataTypeLongNullable, lValueNullable = value };
        /// <summary>
        /// Implicit convert from JSONData to long?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换long?
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
                case DataType.DataTypeDecimal: return (long?)value.mValue;
                case DataType.DataTypeIntNullable: return value.iValueNullable;
                case DataType.DataTypeLongNullable: return value.lValueNullable;
                case DataType.DataTypeULongNullable: return (long?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (long?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToInt64(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToInt64(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to long?");
            return null;
        }
        /// <summary>
        /// Implicit convert from ulong? to JSONData
        /// <br/><br/>Chinese:<br/>把ulong?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(ulong? value) => new JSONData() { dataType = DataType.DataTypeLongNullable, ulValueNullable = value };
        /// <summary>
        /// Implicit convert from JSONData to ulong?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换ulong?
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
                case DataType.DataTypeDecimal: return (ulong?)value.mValue;
                case DataType.DataTypeIntNullable: return (ulong?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (ulong?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (ulong?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToUInt64(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToUInt64(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to ulong?");
            return null;
        }
        /// <summary>
        /// Implicit convert from double? to JSONData
        /// <br/><br/>Chinese:<br/>把double?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(double? value) => new JSONData() { dataType = DataType.DataTypeDoubleNullable, dValueNullable = value };
        /// <summary>
        /// Implicit convert from JSONData to double?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换double?
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
                case DataType.DataTypeDecimal: return (double?)value.mValue;
                case DataType.DataTypeIntNullable: return value.iValueNullable;
                case DataType.DataTypeLongNullable: return value.lValueNullable;
                case DataType.DataTypeULongNullable: return value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToDouble(value.strValue, KissJson.CultureForConvertFloatAndDouble);
                        else
                        {
                            try
                            {
                                return Convert.ToDouble(value.strValue, KissJson.CultureForConvertFloatAndDouble);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to double?");
            return null;
        }
        /// <summary>
        /// Implicit convert from float? to JSONData
        /// <br/><br/>Chinese:<br/>把float?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(float? value) => new JSONData() { dataType = DataType.DataTypeDoubleNullable, dValueNullable = value };
        /// <summary>
        /// Implicit convert from JSONData to float?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换float?
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
                case DataType.DataTypeDecimal: return (float?)value.mValue;
                case DataType.DataTypeIntNullable: return value.iValueNullable;
                case DataType.DataTypeLongNullable: return value.lValueNullable;
                case DataType.DataTypeULongNullable: return value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (float?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToSingle(value.strValue, KissJson.CultureForConvertFloatAndDouble);
                        else
                        {
                            try
                            {
                                return Convert.ToSingle(value.strValue, KissJson.CultureForConvertFloatAndDouble);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to float?");
            return null;
        }
        /// <summary>
        /// Implicit convert from char? to JSONData
        /// <br/><br/>Chinese:<br/>把char?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(char? value) => new JSONData() { dataType = DataType.DataTypeString, strValue = value.ToString() };
        /// <summary>
        /// Implicit convert from JSONData to char?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换char?
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
                case DataType.DataTypeDecimal: return (char?)value.mValue;
                case DataType.DataTypeIntNullable: return (char?)value.iValueNullable;
                case DataType.DataTypeLongNullable: return (char?)value.lValueNullable;
                case DataType.DataTypeULongNullable: return (char?)value.ulValueNullable;
                case DataType.DataTypeDoubleNullable: return (char?)value.dValueNullable;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null || value.strValue.ToLower() == "null")
                            return null;
                        else if (ThrowException)
                            return Convert.ToChar(value.strValue);
                        else
                        {
                            try
                            {
                                return Convert.ToChar(value.strValue);
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to char?");
            return null;
        }
        /// <summary>
        /// Implicit convert from bool? to JSONData
        /// <br/><br/>Chinese:<br/>把bool?隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(bool? value) => new JSONData() { dataType = DataType.DataTypeBooleanNullable, bValueNullable = value };
#endif //SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from bool to JSONData
        /// <br/><br/>Chinese:<br/>把bool隐式转换JSON对象
        /// </summary>
        public static implicit operator JSONData(bool value) => new JSONData() { dataType = DataType.DataTypeBoolean , bValue = value };
        /// <summary>
        /// Implicit convert from JSONData to bool
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换bool
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
                case DataType.DataTypeDecimal: return value.mValue > 0;
#if SUPPORT_NULLABLE
                case DataType.DataTypeBooleanNullable: return value.bValueNullable != null && value.bValueNullable.Value;
                case DataType.DataTypeIntNullable: return value.iValueNullable != null && value.iValueNullable.Value > 0;
                case DataType.DataTypeLongNullable: return value.lValueNullable != null && value.lValueNullable.Value > 0;
                case DataType.DataTypeULongNullable: return value.ulValueNullable != null && value.ulValueNullable.Value > 0;
                case DataType.DataTypeDoubleNullable: return value.dValueNullable != null && value.dValueNullable.Value > 0;
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null)
                            return false;
                        switch (value.strValue.ToLower())
                        {
                            case "true": return true;
                            case "false": return false;
                            default:
                                if (ThrowException) throw new Exception(value.ToString() + " can't convert to bool?");
                                return false;
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to bool");
            return default;
        }
#if SUPPORT_NULLABLE
        /// <summary>
        /// Implicit convert from JSONData to bool?
        /// <br/><br/>Chinese:<br/>把JSON对象隐式转换bool?
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
                case DataType.DataTypeDecimal: return value.mValue > 0;
                case DataType.DataTypeBooleanNullable: return value.bValueNullable;
                case DataType.DataTypeIntNullable: if (value.iValueNullable != null) return value.iValueNullable.Value > 0; else return null;
                case DataType.DataTypeLongNullable: if (value.lValueNullable != null) return value.lValueNullable.Value > 0; else return null;
                case DataType.DataTypeULongNullable: if (value.ulValueNullable != null) return value.ulValueNullable.Value > 0; else return null;
                case DataType.DataTypeDoubleNullable: if (value.dValueNullable != null) return value.dValueNullable.Value > 0; else return null;
                case DataType.DataTypeNull: return null;
                case DataType.DataTypeString:
                    {
                        if (value.strValue == null)
                            return null;
                        switch(value.strValue.ToLower())
                        {
                            case "true": return true;
                            case "null": return null;
                            case "false": return false;
                            default:
                                if (ThrowException) throw new Exception(value.ToString() + " can't convert to bool?");
                                return null;
                        }
                    }
            }
            if (ThrowException) throw new Exception(value.ToString() + " can't convert to bool?");
            return null;
        }
#endif //SUPPORT_NULLABLE
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
                case DataType.DataTypeDecimal: stream.Write(value.mValue); break;
#if SUPPORT_NULLABLE
                case DataType.DataTypeBooleanNullable: stream.Write(value.bValueNullable); break;
                case DataType.DataTypeIntNullable: stream.Write(value.iValueNullable); break;
                case DataType.DataTypeLongNullable: stream.Write(value.lValueNullable); break;
                case DataType.DataTypeULongNullable: stream.Write(value.ulValueNullable); break;
                case DataType.DataTypeDoubleNullable: stream.Write(value.dValueNullable); break;
#endif //SUPPORT_NULLABLE
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
                case DataType.DataTypeDecimal: stream.Read(out value.mValue); break;
#if SUPPORT_NULLABLE
                case DataType.DataTypeBooleanNullable: stream.Read(out value.bValueNullable); break;
                case DataType.DataTypeIntNullable: stream.Read(out value.iValueNullable); break;
                case DataType.DataTypeLongNullable: stream.Read(out value.lValueNullable); break;
                case DataType.DataTypeULongNullable: stream.Read(out value.ulValueNullable); break;
                case DataType.DataTypeDoubleNullable: stream.Read(out value.dValueNullable); break;
#else//This for reset the nullable type, change type and set value as default value.
#pragma warning disable CS0618
                case DataType.DataTypeBooleanNullable: stream.Read(out bool? tempDataTypeBooleanNullable); value.dataType = DataType.DataTypeBoolean; value.bValue = tempDataTypeBooleanNullable ?? default; break;
                case DataType.DataTypeIntNullable: stream.Read(out int? tempDataTypeIntNullable); value.dataType = DataType.DataTypeInt; value.iValue = tempDataTypeIntNullable ?? default; break;
                case DataType.DataTypeLongNullable: stream.Read(out long? tempDataTypeLongNullable); value.dataType = DataType.DataTypeLong; value.lValue = tempDataTypeLongNullable ?? default; break;
                case DataType.DataTypeULongNullable: stream.Read(out ulong? tempDataTypeULongNullable); value.dataType = DataType.DataTypeULong; value.ulValue = tempDataTypeULongNullable ?? default; break;
                case DataType.DataTypeDoubleNullable: stream.Read(out double? tempDataTypeDoubleNullable); value.dataType = DataType.DataTypeDouble; value.dValue = tempDataTypeDoubleNullable ?? default; break;
#pragma warning restore CS0618
#endif //SUPPORT_NULLABLE
                case DataType.DataTypeNull: break;
            }
        }
    }
#endregion //PrivateImpl
}