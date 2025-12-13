# Introduction
This is a most simple and stupid JSON. It part of  [C#Like](https://assetstore.unity.com/packages/tools/integration/c-likefree-hot-update-framework-222880) and [C#LikeFree](https://assetstore.unity.com/packages/tools/integration/c-like-hot-update-framework-222256) that I uploaded to the Unity Asset Store. You can get the [C#LikeFree](https://github.com/ChengShaoRong/CSharpLikeFree) from GitHub too.    
* Compared with other JSON libraries, what's special?
>**Fully compatible with C#Like and can be used in hot update scripts**.  
>**JSONData and built-in types are seamlessly converted and used directly as built-in types,e.g. math operation**.  
>**Easy conversion between JSON string and JSONData and class/struct**.
***
这个是最简单易用JSON. 它是我上传到Unity资源商店里的[C#Like](https://assetstore.unity.com/packages/tools/integration/c-likefree-hot-update-framework-222880) 和 [C#Like免费版](https://assetstore.unity.com/packages/tools/integration/c-like-hot-update-framework-222256) 的一部分. 你也可以在GitHub里下载到[C#Like免费版](https://github.com/ChengShaoRong/CSharpLikeFree).  
* 跟其他的JSON库比,有什么特别过人之处?  
> **完全兼容C#Like,可在热更脚本里使用**  
> **JSONData与内置类型无缝互转,直接当做内置类型使用,例如数值计算**  
> **JSON字符串与JSONData与类/结构体之间轻松转换**

# Install
Package had been uploaded to Nuget, dependent library: **.NET Standard 2.0**. You can install by command ``Install-Package KissJSON``
***
包已上传至Nuget,依赖库为: **.NET Standard 2.0**. 你可以通过``Install-Package KissJSON``来安装.

# Usage
* **Built-in type that can direct convert with JSONData**
```
	Built-in type: string sbyte ushort uint ulong byte short int long bool float double DateTime
	Built-in type nullable: sbyte? ushort? uint? ulong? byte? short? int? long? bool? float? double? DateTime?
	Enum  
	List<Built-in type>
	Dictionary<string,Built-in type>
```
*  **JSONData and built-in types are seamlessly converted and used directly as built-in types,e.g. math operation**  
>* Built-in type ->JSONData
```
	JSONData iData = 2;
	Console.WriteLine("JSONData iData = 2;  test iData = " + iData);//output 2
	JSONData fData = 88.8f;
	Console.WriteLine("JSONData fData = 88.8f;  test fData = " + fData);//output 88.8
	List<string> listValue = new List<string>();
	listValue.Add("test list str1");
	listValue.Add("test list str2");
	JSONData listData = listValue;
	Console.WriteLine("JSONData listData = listValue;  test listData = " + listData);//output ["test list str1","test list str2"]
	Dictionary<string, int> dictValue = new Dictionary<string, int>();
	dictValue.Add("key1", 11);
	dictValue.Add("key2", 22);
	JSONData dictData = dictValue;
	Console.WriteLine("JSONData dictData = dictValue;  test dictData = " + dictData);//output {"key1":11,"key2":22}
```
> * JSONData -> Built-in type
```
	int iValue = iData;
	Console.WriteLine("int iValue = iData;  test iValue = " + iValue);//output 2
	float fValue = fData;
	Console.WriteLine("float fValue = fData;  test fValue = " + fValue);//output 88.8
	List<string> listValue2 = listData;
	string strTemp = "";
	foreach (var str in listValue2)
	    strTemp += str + ",";
	Console.WriteLine("List<string> listValue2 = listData;  test listValue2 = " + strTemp);//output test list str1,test list str2,
	Dictionary<string, int> dictValue2 = dictData;
	strTemp = "";
	foreach (var item in dictValue2)
	    strTemp += "(" + item.Key + "=" + item.Value + ")";
	Console.WriteLine("Dictionary<string, int> dictValue2 = dictData;  test dictValue2 = " + strTemp);//output (key1=11)(key2=22)
```
> * Math operation between JSONData and Built-in type.
```
	JSONData exprData1 = 2;
	JSONData exprData2 = 3;
	JSONData exprData3 = exprData1 * exprData2;
	Console.WriteLine("test Math Expression;  exprData3 = exprData1 * exprData2; exprData3 = " + exprData3);//output 6
	exprData3 = exprData1 << 5;
	Console.WriteLine("test Math Expression;  exprData3 = exprData1 << 5; exprData3 = " + exprData3);//output 64
	exprData3 = exprData1 - exprData2;
	Console.WriteLine("test Math Expression;  exprData3 = exprData1 - exprData2; exprData3 = " + exprData3);//output -1
	exprData3 *= exprData2;//exprData3=-1;exprData2=3
	Console.WriteLine("test Math Expression;  exprData3 *= exprData2; exprData3 = " + exprData3);//output -3

	iData = 2;
	if (iData > 1)
	    Console.WriteLine("test Math Expression;  iData = 2, enter if (iData > 1)");//output
	else
	    Console.WriteLine("test Math Expression;  iData = 2, not enter if (iData > 1)");
```

* **Easy conversion between JSON string and JSONData and class/struct***
>* JSON string -> JSONData
```
	string strJson = @"{
  "str": "{test \"str", 
  "i": 11, 
  "j": 2.3, 
  "k": [
    3, 
    null, 
    {
      "m": true
    }
  ], 
  "l": {
    "x": 1, 
    "y": "abc"
  }
}";
	JSONData data = KissJson.ToJSONData(strJson);
	//accept JSONData by ["key"] and [index]
	Console.WriteLine("JSON string => JSONData; test data[\"str\"] = " + data["str"]);//output {test "str
	Console.WriteLine("JSON string => JSONData; test data[\"i\"] = " + data["i"]);//output 11
	Console.WriteLine("JSON string => JSONData; test data[\"j\"] = " + data["j"]);//output 2.3
	Console.WriteLine("JSON string => JSONData; test data[\"k\"] = " + data["k"]);//output [3,null,{"m":true}]
	Console.WriteLine("JSON string => JSONData; test data[\"l\"] = " + data["l"]);//output {"x":1,"y":"abc"}
	Console.WriteLine("JSON string => JSONData; test data[\"k\"][0] = " + data["k"][0]);//output 3
	Console.WriteLine("JSON string => JSONData; test data[\"l\"][\"y\"] = " + data["l"]["y"]);//output abc
	Console.WriteLine("JSON string => JSONData; test data[\"k\"][2][\"m\"] = " + data["k"][2]["m"]);//output true
```
>* JSONData -> JSON string
```
	JSONData listData3 = JSONData.NewDictionary();
	listData3.Add("key1", 10);         //add data like Dictionary use function 'Add(key,value)'
	listData3["key2"] = "test string"; //add data like Dictionary use index set 'this[]'
	listData3["key3"] = JSONData.NewList(); //we add a list
	if (listData3.ContainsKey("key3"))  //make sure the key 'key3' exist if you don't know whether exist
	    listData3["key3"].Add(1);       //add some data to the list
	listData3["key3"].Insert(0,"string2"); //insert some data to the list,we don't check the key 'key3' exist because we just know it exist!
	listData3["key4"] = JSONData.NewDictionary(); //we add a Dictionary
	listData3["key4"]["x"] = 1;
	listData3["key4"]["y"] = 2;
	listData3["key4"]["z"] = 3;
	Debug.Log("test JSONData => JSON string; strJson = " + listData3.ToJson());//output {"key1":10,"key2":"test string","key3":["string2",1],"key4":{"x":1,"y":2,"z":3}}
```
>* The demo class/struct that using in convert with JSON string and JSONData
```
	/// <summary>
	/// test class <=> JSON
	/// </summary>
	public class TestJsonDataSub
	{
	    public int? id;//test nullable
	    public string name;
	    public Vector2 v2;//you can add other class/struct type,such as Color/Rect/Vector3/...
	    public List<string> info;
	    public Dictionary<string, int> maps;
	}
	//mark the class as KissJsonDontSerialize, will ignore while serialize JSON
	[KissJsonDontSerialize]
	public class TestJsonDataSub2
	{
	    public int id;
	}
	/// <summary>
	/// test class <=> JSON
	/// </summary>
	public class TestJsonData
	{
	    [KissJsonDontSerialize]
	    public string str;//will ignore while serialize JSON because be mark as KissJsonDontSerialize
	    public int i;
	    public DayOfWeek j;//test enum of not hot update
	    public TestHotUpdateEnum z;//test enum of hot update
	    public List<int?> k;
	    public Dictionary<string, TestJsonDataSub> datas;//test Dictionary for class/struct
	    public TestJsonDataSub data;//test single class/struct
	    public TestJsonDataSub2 data2;//will ignore while serialize JSON because the class 'TestJsonDataSub2' mark as KissJsonDontSerialize
	}
	/// <summary>
	/// test convert hot update enum
	/// </summary>
	public enum TestHotUpdateEnum
	{
	    Morning,
	    Afternoon,
	    Evening
	}  
	/// <summary>
	/// test struct
	/// </summary>
	public struct Vector2
	{
	    public float x;
	    public float y;
	    public override string ToString()
	    {
	        return $"({x:F2},{y:F2})";
	    }
	}
```
>*  JSON string -> class/struct
```
	strJson = "{\"str\":\"{test str\",\"i\":11,\"j\":1,\"z\":2,\"k\":[3,null,7],\"datas\":{\"aa\":{\"id\":1,\"name\":\"aaa\",\"v2\":{\"x\":1,\"y\":2},\"info\":[\"a\",\"xd\",\"dt\"],\"maps\":{\"x\":1,\"y\":2}},\"bb\":{\"id\":2,\"name\":\"bbb\",\"v2\":{\"x\":3,\"y\":4},\"info\":[\"x\",\"x3d\",\"ddt\"],\"maps\":{\"x\":2,\"y\":3}}},\"data\":{\"id\":3,\"name\":\"ccc\",\"v2\":{\"x\":3,\"y\":1},\"info\":[\"ya\",\"xyd\",\"drt\"],\"maps\":{\"x\":3,\"y\":4}}}";
	TestJsonData testJsonData = (TestJsonData)KissJson.ToObject(typeof(TestJsonData), strJson);//JSON string => class
	////test JSONData => class/struct
	//TestJsonData testJsonData = (TestJsonData)KissJson.ToObject(typeof(TestJsonData), KissJson.ToJSONData(strJson));//JSONData => class
	Console.WriteLine(testJsonData.str);//output Null
	Console.WriteLine(testJsonData.i);//output 11
	//"j":"Monday" or "j":"1" or "j":1 are both identified as 'DayOfWeek.Monday'
	//recommend use "j":1 because ToJson output as number
	Console.WriteLine(testJsonData.j);//output Monday
	Console.WriteLine((int)testJsonData.j);//output 1
	Console.WriteLine(testJsonData.z);//output 2
	foreach (var item in testJsonData.k)
	    Console.WriteLine(item);//output 3/output null/output 7
	foreach (var datas in testJsonData.datas)
	{
	    Console.WriteLine(datas.Key);//output aa/output bb
	    Console.WriteLine(datas.Value.v2);//output (1.0, 2.0)/output (3.0, 4.0)
	}
```
>*  class/struct -> JSON string
```
	strTemp = KissJson.ToJson(testJsonData);//class/struct => JSON string
	Console.WriteLine(strTemp);//output {"i":11,"j":1,"z":2,"k":[3,null,7],"datas":{"aa":{"id":1,"name":"aaa","v2":{"x":1,"y":2},"info":["a","xd","dt"],"maps":{"x":1,"y":2}},"bb":{"id":2,"name":"bbb","v2":{"x":3,"y":4},"info":["x","x3d","ddt"],"maps":{"x":2,"y":3}}},"data":{"id":3,"name":"ccc","v2":{"x":3,"y":1},"info":["ya","xyd","drt"],"maps":{"x":3,"y":4}}}
```
>*  class/struct -> JSONData
```
	data = KissJson.ToJSONData(testJsonData);
```

>*  Format JSON string
```
	Console.WriteLine(data.ToJson(true));//Formatting JSON strings for better readability
	Console.WriteLine(data.ToJson());//Not formatting JSON strings, poor readability, but JSON strings are short, more suitable for transmission

Formatting JSON string:
{
    "i": 11,
    "j": 1,
    "z": 2,
    "k": [
        3,
        null,
        7
    ],
    "datas": {
        "aa": {
            "id": 1,
            "name": "aaa",
            "v2": {
                "x": 1,
                "y": 2
            },
            "info": [
                "a",
                "xd",
                "dt"
            ],
            "maps": {
                "x": 1,
                "y": 2
            }
        },
        "bb": {
            "id": 2,
            "name": "bbb",
            "v2": {
                "x": 3,
                "y": 4
            },
            "info": [
                "x",
                "x3d",
                "ddt"
            ],
            "maps": {
                "x": 2,
                "y": 3
            }
        }
    },
    "data": {
        "id": 3,
        "name": "ccc",
        "v2": {
            "x": 3,
            "y": 1
        },
        "info": [
            "ya",
            "xyd",
            "drt"
        ],
        "maps": {
            "x": 3,
            "y": 4
        }
    }
}
Not formatting JSON string:
{"i":11,"j":1,"z":2,"k":[3,null,7],"datas":{"aa":{"id":1,"name":"aaa","v2":{"x":1,"y":2},"info":["a","xd","dt"],"maps":{"x":1,"y":2}},"bb":{"id":2,"name":"bbb","v2":{"x":3,"y":4},"info":["x","x3d","ddt"],"maps":{"x":2,"y":3}}},"data":{"id":3,"name":"ccc","v2":{"x":3,"y":1},"info":["ya","xyd","drt"],"maps":{"x":3,"y":4}}}

```

>*  Deep clone JSONData object
```
	JSONData clone = JSONData.DeepClone(data);//Deep clone JSONData, 'clone' object is not the same object with 'data'.
	JSONData notClone = data;  //Just set value, 'notClone' is alias of the 'data', they are the same object.
	data["i"] = 100;//Modify the 'data' object, that will not effect the 'clone' object.
	Console.WriteLine(clone.ToJson());
	Console.WriteLine(notClone.ToJson());
```

>*  Convert Binary JSON file between JSONData object
```
	JSONData object => Binary JSON file
	JSONData data = JSONData.NewDictionary();
	data["test"] = 1;
	File.WriteAllBytes("test.json", JSONData.ToBinaryData(data));
	
	Binary JSON file => JSONData object
	JSONData data = JSONData.ToJSONData(File.ReadAllBytes("test.json"));
```

>*  Convert Enum between JSONData object
```
	Enum => JSONData object (direct assignment)
	JSONData data = JSONData.NewDictionary();
	data["testEnum"] = TestHotUpdateEnum.Morning;
	JSONData testEnum = TestHotUpdateEnum.Afternoon;
	
	JSONData object => Enum (Force convert to base enum type and then force convert to enum type)
	TestHotUpdateEnum enumTest = (TestHotUpdateEnum)(int)testEnum;
```

>*  Initialize JSONData object
```
	///Built-in type:
	JSONData a = 1;
	JSONData b = "abc";
	JSONData c = true;
	
	///Type List: List<Built-in type>
	JSONData d = new List<int>(){1,2,3};
	JSONData e = new JSONData(){1,2,3};
	
	///Type Dictionary: Dictionary<string, Built-in type>
	JSONData f = new Dictionary<string,int>(){{"a",1},{"b",2}};
	JSONData g = new JSONData(){{"a",1},{"b",2}};
	JSONData h = new JSONData(){{"a",1},{"b","c"}};

	///JSON string
	JSONData i = KissJson.ToJSONData("{\"a\":1}");

	///Binary JSON file or normal text JSON file using UTF8 format (Regardless of whether BOM is included or not)
	JSONData j = KissJson.ToJSONData(File.ReadAllBytes("123.json"));

	///class or stuct
	TestJsonData testJsonData = new TestJsonData();//Normal class object
	JSONData i = KissJson.ToJSONData(testJsonData);
```

>*  Traveling through sub objects in a JSONData object using foreach
```
	///Type List: List<Built-in type>
	JSONData list = new JSONData(){1,2,3};
	//The recommended method cannot use 'var', only `JSONData` can be used
	foreach(JSONData item in list)
		Console.WriteLine($"List:{item}");
	//This method can use `var`
	foreach(var item in list.Value as List<JSONData>)
		Console.WriteLine($"List:{item}");
	//Not recommended is a convenient but inefficient method (as it copies a List<int>), which can be done using ` var `.
	//You can using `List<int>` if your JSON data is `int`, and using `List<string>` if your JSON data is `string`...
	foreach(var item in (List<int>)list)
		Console.WriteLine($"List:{item}");
	
	///Type Dictionary: Dictionary<string, Built-in type>
	JSONData dic = new JSONData(){{"a",1},{"b",2}};
	//The recommended method cannot use 'var', only `KeyValuePair<string, JSONData>` can be used
	foreach(KeyValuePair<string, JSONData> item in dic)
		Console.WriteLine($"Dictionary:{item.Key}={item.Value}");
	//This method can use `var`
	foreach(var item in dic.Value as Dictionary<string,JSONData>)
		Console.WriteLine($"Dictionary:{item.Key}={item.Value}");
	//Not recommended is a convenient but inefficient method (as it copies a Dictionary<string,string>), which can be done using ` var `.
	//You can using `Dictionary<string,int>` if your JSON data is `int`, and using `Dictionary<string,string>` if your JSON data is `string`,and using `Dictionary<string,object>` if your JSON data mix with `string` and `int`...
	foreach(var item in (Dictionary<string,int>)dic)
		Console.WriteLine($"Dictionary:{item.Key}={item.Value}");

	//Other type can't using foreach
	JSONData i = 1;
	foreach(JSONData item in i)//This line will throw an exception during runtime
		Console.WriteLine($"Crash:{item}");
```

***

* **可与JSONData之间互转的内置类型**
```
	内置类型: string sbyte ushort uint ulong byte short int long bool float double DateTime
	可空内置类型: sbyte? ushort? uint? ulong? byte? short? int? long? bool? float? double? DateTime?
	枚举  
	List<内置类型>
	Dictionary<string,内置类型>
```
*  **JSONData与内置类型无缝互转,直接当做内置类型使用,例如数值计算**  
>* 内置类型 ->JSONData
```
	JSONData iData = 2;
	Console.WriteLine("JSONData iData = 2;  test iData = " + iData);//输出 2
	JSONData fData = 88.8f;
	Console.WriteLine("JSONData fData = 88.8f;  test fData = " + fData);//输出 88.8
	List<string> listValue = new List<string>();
	listValue.Add("test list str1");
	listValue.Add("test list str2");
	JSONData listData = listValue;
	Console.WriteLine("JSONData listData = listValue;  test listData = " + listData);//输出 ["test list str1","test list str2"]
	Dictionary<string, int> dictValue = new Dictionary<string, int>();
	dictValue.Add("key1", 11);
	dictValue.Add("key2", 22);
	JSONData dictData = dictValue;
	Console.WriteLine("JSONData dictData = dictValue;  test dictData = " + dictData);//输出 {"key1":11,"key2":22}
```
> * JSONData -> 内置类型
```
	int iValue = iData;
	Console.WriteLine("int iValue = iData;  test iValue = " + iValue);//输出 2
	float fValue = fData;
	Console.WriteLine("float fValue = fData;  test fValue = " + fValue);//输出 88.8
	List<string> listValue2 = listData;
	string strTemp = "";
	foreach (var str in listValue2)
	    strTemp += str + ",";
	Console.WriteLine("List<string> listValue2 = listData;  test listValue2 = " + strTemp);//输出 test list str1,test list str2,
	Dictionary<string, int> dictValue2 = dictData;
	strTemp = "";
	foreach (var item in dictValue2)
	    strTemp += "(" + item.Key + "=" + item.Value + ")";
	Console.WriteLine("Dictionary<string, int> dictValue2 = dictData;  test dictValue2 = " + strTemp);//输出 (key1=11)(key2=22)
```
> * JSONData 与 内置类型 之间数值运算
```
	JSONData exprData1 = 2;
	JSONData exprData2 = 3;
	JSONData exprData3 = exprData1 * exprData2;
	Console.WriteLine("test Math Expression;  exprData3 = exprData1 * exprData2; exprData3 = " + exprData3);//输出 6
	exprData3 = exprData1 << 5;
	Console.WriteLine("test Math Expression;  exprData3 = exprData1 << 5; exprData3 = " + exprData3);//输出 64
	exprData3 = exprData1 - exprData2;
	Console.WriteLine("test Math Expression;  exprData3 = exprData1 - exprData2; exprData3 = " + exprData3);//输出 -1
	exprData3 *= exprData2;//exprData3=-1;exprData2=3
	Console.WriteLine("test Math Expression;  exprData3 *= exprData2; exprData3 = " + exprData3);//输出 -3

	iData = 2;
	if (iData > 1)
	    Console.WriteLine("test Math Expression;  iData = 2, enter if (iData > 1)");//进入这里
	else
	    Console.WriteLine("test Math Expression;  iData = 2, not enter if (iData > 1)");
```

* **JSON字符串与JSONData与类之间轻松转换**
>* JSON字符串 -> JSONData
```
	string strJson = @"{
  "str": "{test \"str", 
  "i": 11, 
  "j": 2.3, 
  "k": [
    3, 
    null, 
    {
      "m": true
    }
  ], 
  "l": {
    "x": 1, 
    "y": "abc"
  }
}";
	JSONData data = KissJson.ToJSONData(strJson);
	//accept JSONData by ["key"] and [index]
	Console.WriteLine("JSON string => JSONData; test data[\"str\"] = " + data["str"]);//输出 {test "str
	Console.WriteLine("JSON string => JSONData; test data[\"i\"] = " + data["i"]);//输出 11
	Console.WriteLine("JSON string => JSONData; test data[\"j\"] = " + data["j"]);//输出 2.3
	Console.WriteLine("JSON string => JSONData; test data[\"k\"] = " + data["k"]);//输出 [3,null,{"m":true}]
	Console.WriteLine("JSON string => JSONData; test data[\"l\"] = " + data["l"]);//输出 {"x":1,"y":"abc"}
	Console.WriteLine("JSON string => JSONData; test data[\"k\"][0] = " + data["k"][0]);//输出 3
	Console.WriteLine("JSON string => JSONData; test data[\"l\"][\"y\"] = " + data["l"]["y"]);//输出 abc
	Console.WriteLine("JSON string => JSONData; test data[\"k\"][2][\"m\"] = " + data["k"][2]["m"]);//输出 true
```
>* JSONData -> JSON字符串
```
	JSONData listData3 = JSONData.NewDictionary();
	listData3.Add("key1", 10);         //类似Dictionary的函数'Add(key,value)'的方式加数据
	listData3["key2"] = "test string"; //类似Dictionary/List索引'this[]'的方式加数据
	listData3["key3"] = JSONData.NewList(); //我们加一个List类型的JSONData
	if (listData3.ContainsKey("key3"))  //确保'key3'是否存在,如果你不确认的情况下
	    listData3["key3"].Add(1);       //插入一些数据到list里
	listData3["key3"].Insert(0,"string2"); //插入一些数据到list里, 我们这里不检查'key3'是否存在,因为我们已经确认它是存在的
	listData3["key4"] = JSONData.NewDictionary(); //我们加一个Dictionary类型的JSONData
	listData3["key4"]["x"] = 1;
	listData3["key4"]["y"] = 2;
	listData3["key4"]["z"] = 3;
	Debug.Log("test JSONData => JSON string; strJson = " + listData3.ToJson());//输出 {"key1":10,"key2":"test string","key3":["string2",1],"key4":{"x":1,"y":2,"z":3}}
```
>*  与JSON字符串/JSONData互转相关的类/结构体
```
	/// <summary>
	/// 测试 类 <=> JSON字符串
	/// </summary>
	public class TestJsonDataSub
	{
	    public int? id;//测试可空类型
	    public string name;
	    public Vector2 v2;//你可以添加其他类型,例如Color/Rect/Vector3/...
	    public List<string> info;
	    public Dictionary<string, int> maps;
	}
	//类被标为KissJsonDontSerialize的类会被JSON解析器忽略
	[KissJsonDontSerialize]
	public class TestJsonDataSub2
	{
	    public int id;
	}
	/// <summary>
	/// 测试 类 <=> JSON字符串
	/// </summary>
	public class TestJsonData
	{
	    [KissJsonDontSerialize]
	    public string str;//被标为KissJsonDontSerialize的属性会被JSON解析器忽略
	    public int i;
	    public DayOfWeek j;//测试枚举(非热更脚本)
	    public TestHotUpdateEnum z;//测试枚举(热更脚本)
	    public List<int?> k;
	    public Dictionary<string, TestJsonDataSub> datas;//测试Dictionary
	    public TestJsonDataSub data;//测试单个的类中类
	    public TestJsonDataSub2 data2;//这个会被JSON解析器忽略,因为TestJsonDataSub2这个类被标为KissJsonDontSerialize
	}
	/// <summary>
	/// 测试枚举
	/// </summary>
	public enum TestHotUpdateEnum
	{
	    Morning,
	    Afternoon,
	    Evening
	}  
	/// <summary>
	/// 测试结构体
	/// </summary>
	public struct Vector2
	{
	    public float x;
	    public float y;
	    public override string ToString()
	    {
	        return $"({x:F2},{y:F2})";
	    }
	}
```
>*  JSON字符串 -> 类/结构体
```
	strJson = "{\"str\":\"{test str\",\"i\":11,\"j\":1,\"z\":2,\"k\":[3,null,7],\"datas\":{\"aa\":{\"id\":1,\"name\":\"aaa\",\"v2\":{\"x\":1,\"y\":2},\"info\":[\"a\",\"xd\",\"dt\"],\"maps\":{\"x\":1,\"y\":2}},\"bb\":{\"id\":2,\"name\":\"bbb\",\"v2\":{\"x\":3,\"y\":4},\"info\":[\"x\",\"x3d\",\"ddt\"],\"maps\":{\"x\":2,\"y\":3}}},\"data\":{\"id\":3,\"name\":\"ccc\",\"v2\":{\"x\":3,\"y\":1},\"info\":[\"ya\",\"xyd\",\"drt\"],\"maps\":{\"x\":3,\"y\":4}}}";
	TestJsonData testJsonData = (TestJsonData)KissJson.ToObject(typeof(TestJsonData), strJson);//JSON 字符串 => 类
	////测试 JSONData => 类
	//TestJsonData testJsonData = (TestJsonData)KissJson.ToObject(typeof(TestJsonData), KissJson.ToJSONData(strJson));//JSONData => 类
	Console.WriteLine(testJsonData.str);//输出 Null
	Console.WriteLine(testJsonData.i);//输出 11
	//"j":"Monday"和"j":"1"和"j":1都会识别为'DayOfWeek.Monday'
	//推荐使用"j":1,因为ToJson()会输出为"j":1
	Console.WriteLine(testJsonData.j);//输出 Monday
	Console.WriteLine((int)testJsonData.j);//输出 1
	Console.WriteLine(testJsonData.z);//输出 2
	foreach (var item in testJsonData.k)
	    Console.WriteLine(item);//输出 3/输出 null/输出 7
	foreach (var datas in testJsonData.datas)
	{
	    Console.WriteLine(datas.Key);//输出 aa/输出 bb
	    Console.WriteLine(datas.Value.v2);//输出 (1.0, 2.0)/输出 (3.0, 4.0)
	}
```
>*  类/结构体 -> JSON字符串
```
	strTemp = KissJson.ToJson(testJsonData);//类/结构体 => JSON字符串
	Console.WriteLine(strTemp);//输出 {"i":11,"j":1,"z":2,"k":[3,null,7],"datas":{"aa":{"id":1,"name":"aaa","v2":{"x":1,"y":2},"info":["a","xd","dt"],"maps":{"x":1,"y":2}},"bb":{"id":2,"name":"bbb","v2":{"x":3,"y":4},"info":["x","x3d","ddt"],"maps":{"x":2,"y":3}}},"data":{"id":3,"name":"ccc","v2":{"x":3,"y":1},"info":["ya","xyd","drt"],"maps":{"x":3,"y":4}}}
```
>*  类/结构体 -> JSONData
```
	data = KissJson.ToJSONData(testJsonData);
```
>*  格式化JSON字符串
```
	Console.WriteLine(data.ToJson(true));//格式化JSON字符串,更好的可读性
	Console.WriteLine(data.ToJson());//不格式化JSON字符串,可读性差但JSON字符串很短,更适合传输

格式化JSON字符串为:
{
    "i": 11,
    "j": 1,
    "z": 2,
    "k": [
        3,
        null,
        7
    ],
    "datas": {
        "aa": {
            "id": 1,
            "name": "aaa",
            "v2": {
                "x": 1,
                "y": 2
            },
            "info": [
                "a",
                "xd",
                "dt"
            ],
            "maps": {
                "x": 1,
                "y": 2
            }
        },
        "bb": {
            "id": 2,
            "name": "bbb",
            "v2": {
                "x": 3,
                "y": 4
            },
            "info": [
                "x",
                "x3d",
                "ddt"
            ],
            "maps": {
                "x": 2,
                "y": 3
            }
        }
    },
    "data": {
        "id": 3,
        "name": "ccc",
        "v2": {
            "x": 3,
            "y": 1
        },
        "info": [
            "ya",
            "xyd",
            "drt"
        ],
        "maps": {
            "x": 3,
            "y": 4
        }
    }
}
不格式化JSON字符串:
{"i":11,"j":1,"z":2,"k":[3,null,7],"datas":{"aa":{"id":1,"name":"aaa","v2":{"x":1,"y":2},"info":["a","xd","dt"],"maps":{"x":1,"y":2}},"bb":{"id":2,"name":"bbb","v2":{"x":3,"y":4},"info":["x","x3d","ddt"],"maps":{"x":2,"y":3}}},"data":{"id":3,"name":"ccc","v2":{"x":3,"y":1},"info":["ya","xyd","drt"],"maps":{"x":3,"y":4}}}

```

>*  深复制JSONData
```
	JSONData clone = JSONData.DeepClone(data);//深复制JSONData对象, 'clone'和'data'是两个独立的对象.
	JSONData notClone = data;  //仅仅是复制,相当于'notClone'是'data'的别名,它们是同一个对象.
	data["i"] = 100;//修改'data'对象,不会影响到'clone'对象的数值.
	Console.WriteLine(clone.ToJson());
	Console.WriteLine(notClone.ToJson());
```

>*  二进制JSON文件 互转 JSONData对象
```
	JSONData对象 => 二进制JSON文件
	JSONData data = JSONData.NewDictionary();
	data["test"] = 1;
	File.WriteAllBytes("test.json", JSONData.ToBinaryData(data));
	
	二进制JSON文件 => JSONData对象
	JSONData data = JSONData.ToJSONData(File.ReadAllBytes("test.json"));
```

>*  枚举 互转 JSONData对象
```
	枚举 => JSONData对象 (直接赋值)
	JSONData data = JSONData.NewDictionary();
	data["testEnum"] = TestHotUpdateEnum.Morning;
	JSONData testEnum = TestHotUpdateEnum.Afternoon;
	
	JSONData对象 => 枚举 (必须先转为枚举的基类再强转枚举)
	TestHotUpdateEnum enumTest = (TestHotUpdateEnum)(int)testEnum;
```

>*  初始化JSONData对象
```
	///基础类型
	JSONData a = 1;
	JSONData b = "abc";
	JSONData c = true;
	
	///列表类型:List<基础类型>
	JSONData d = new List<int>(){1,2,3};
	JSONData e = new JSONData(){1,2,3};
	
	///字典类型:Dictionary<string, 基础类型>
	JSONData f = new Dictionary<string,int>(){{"a",1},{"b",2}};
	JSONData g = new JSONData(){{"a",1},{"b",2}};
	JSONData h = new JSONData(){{"a",1},{"b","c"}};

	///JSON字符串
	JSONData i = KissJson.ToJSONData("{\"a\":1}");

	///二进制JSON文件或普通UTF8格式JSON文件(无论是否带BOM)
	JSONData j = KissJson.ToJSONData(File.ReadAllBytes("123.json"));

	///类或结构
	TestJsonData testJsonData = new TestJsonData();//普通的类对象
	JSONData i = KissJson.ToJSONData(testJsonData);
```

>*  用foreach游历JSONData对象里的子对象
```
	///列表类型:List<基础类型>
	JSONData list = new JSONData(){1,2,3};
	//推荐的方式,不可以使用`var`,只能使用JSONData
	foreach(JSONData item in list)
		Console.WriteLine($"List:{item}");
	//这种方式可以使用`var`
	foreach(var item in list.Value as List<JSONData>)
		Console.WriteLine($"List:{item}");
	//不推荐的很方便但低效方式(因为会复制一个List<int>出来),可以使用`var`.
	//如果数据是int就用List<int>,是string就用List<string>...
	foreach(var item in (List<int>)list)
		Console.WriteLine($"List:{item}");
	
	///字典类型:Dictionary<string, 基础类型>
	JSONData dic = new JSONData(){{"a",1},{"b",2}};
	//推荐的方式,不可以使用`var`,只能使用`KeyValuePair<string, JSONData>`
	foreach(KeyValuePair<string, JSONData> item in dic)
		Console.WriteLine($"Dictionary:{item.Key}={item.Value}");
	//这种方式可以使用`var`
	foreach(var item in dic.Value as Dictionary<string,JSONData>)
		Console.WriteLine($"Dictionary:{item.Key}={item.Value}");
	//不推荐的很方便但低效方式(因为会复制一个Dictionary<string,int>出来),可以使用`var`.
	//如果数据是int就用Dictionary<string,int>,是string就用Dictionary<string,string>,是混合就用Dictionary<string,object>...
	foreach(var item in (Dictionary<string,int>)dic)
		Console.WriteLine($"Dictionary:{item.Key}={item.Value}");

	//其他类型不能使用foreach
	JSONData i = 1;
	foreach(JSONData item in i)//这行在运行时会抛出异常
		Console.WriteLine($"Crash:{item}");
```