using System;
using System.Collections.Generic;
using CSharpLike;

namespace KissJSONTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            //test Build-In-Type => JSONData
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

            //test JSONData =>  Build-In-Type
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

            //test JSON string => JSONData
            string strJson = @"{
    ""str"": ""{test \""str"",
    ""i"": 11,
    ""j"": 2.3,
    ""k"": [
        3,
        null,
        {
            ""m"": true
        }
    ],
    ""l"": {
        ""x"": 1,
        ""y"": ""abc""
    }
}";//same with 'string strJson = "{\"str\":\"{test str\",\"i\":11,\"j\":2.3,\"k\":[3,null,{\"m\":true}],\"l\":{\"x\":1,\"y\":\"abc\"}}";'
            JSONData data = KissJson.ToJSONData(strJson);
            //accept JSONData by ["key"] and [index]
            Console.WriteLine("JSON string => JSONData; test data[\"str\"] = " + data["str"]);//output {test str
            Console.WriteLine("JSON string => JSONData; test data[\"i\"] = " + data["i"]);//output 11
            Console.WriteLine("JSON string => JSONData; test data[\"j\"] = " + data["j"]);//output 2.3
            Console.WriteLine("JSON string => JSONData; test data[\"k\"] = " + data["k"]);//output [3,null,{"m":true}]
            Console.WriteLine("JSON string => JSONData; test data[\"l\"] = " + data["l"]);//output {"x":1,"y":"abc"}
            Console.WriteLine("JSON string => JSONData; test data[\"k\"][0] = " + data["k"][0]);//output 3
            Console.WriteLine("JSON string => JSONData; test data[\"l\"][\"y\"] = " + data["l"]["y"]);//output abc
            Console.WriteLine("JSON string => JSONData; test data[\"k\"][2][\"m\"] = " + data["k"][2]["m"]);//output true

            //test Math Expression between JSONData with Build-In-Type
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

            //test JSONData => JSON string
            JSONData listData3 = JSONData.NewDictionary();
            listData3.Add("key1", 10);         //add data like Dictionary use function 'Add(key,value)'
            listData3["key2"] = "test \n\t\"string"; //add data like Dictionary use index set 'this[]'
            listData3["key3"] = JSONData.NewList(); //we add a list
            if (listData3.ContainsKey("key3"))  //make sure the key 'key3' exist if you don't know whether exist
                listData3["key3"].Add(1);       //add some data to the list
            listData3["key3"].Insert(0, "string2"); //insert some data to the list,we don't check the key 'key3' exist because we just know it exist!
            listData3["key4"] = JSONData.NewDictionary(); //we add a Dictionary
            listData3["key4"]["x"] = 1;
            listData3["key4"]["y"] = 2;
            listData3["key4"]["z"] = 3;
            //Be careful that ToJson() of JSONData is different between ToString()
            //You must use ToJson() if you want to convert to a JSON string, OR ELSE may be error if have special string in it (such as \n \t \r \\ \");
            Console.WriteLine("test JSONData => JSON string; strJson = " + listData3.ToJson());//output {"key1":10,"key2":"test string","key3":["string2",1],"key4":{"x":1,"y":2,"z":3}}

            //test looping through the List or Dictionary in JSONData
            foreach (var item in listData3["key3"].Value as List<JSONData>)
            {
                Console.WriteLine("test looping through the List in JSONData using foreach; stype 1: listData3[\"key3\"].Value = " + item);//output string2/output 1
            }
            foreach (var item in listData3["key3"])
            {
                Console.WriteLine("test looping through the List in JSONData using foreach; stype 2: listData3[\"key3\"].Value = " + item);//output string2/output 1
            }
            foreach (JSONData item in listData3["key3"])
            {
                Console.WriteLine("test looping through the List in JSONData using foreach; stype 3: listData3[\"key3\"].Value = " + item);//output string2/output 1
            }
            List<JSONData> tempList = listData3["key3"].Value as List<JSONData>;
            for (int i = 0; i < tempList.Count; i++)
                Console.WriteLine("test looping through the List in JSONData using for; listData3[\"key3\"].Value = " + tempList[i]);//output string2/output 1
            foreach (var item in listData3["key4"].Value as Dictionary<string, JSONData>)
            {
                Console.WriteLine("test looping through the Dictionary in JSONData using foreach; stype 1: listData3[\"key4\"], Key=" + item.Key + ",Value=" + item.Value);//output Key=x,Value=1/output Key=y,Value=2/output Key=z,Value=3
            }
            foreach (KeyValuePair<string, JSONData> item in listData3["key4"])//Must explicitly using `KeyValuePair<string, JSONData>`, can't using `var` instead.
            {
                Console.WriteLine("test looping through the Dictionary in JSONData using foreach; stype 2: listData3[\"key4\"], Key=" + item.Key + ",Value=" + item.Value);//output Key=x,Value=1/output Key=y,Value=2/output Key=z,Value=3
            }

            //test JSON string => class/struct
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

            //test class/struct => JSON string
            strTemp = KissJson.ToJson(testJsonData);//class/struct => JSON string
            Console.WriteLine(strTemp);//output {"i":11,"j":1,"z":2,"k":[3,null,7],"datas":{"aa":{"id":1,"name":"aaa","v2":{"x":1,"y":2},"info":["a","xd","dt"],"maps":{"x":1,"y":2}},"bb":{"id":2,"name":"bbb","v2":{"x":3,"y":4},"info":["x","x3d","ddt"],"maps":{"x":2,"y":3}}},"data":{"id":3,"name":"ccc","v2":{"x":3,"y":1},"info":["ya","xyd","drt"],"maps":{"x":3,"y":4}}}

            //class/struct => JSONData
            data = KissJson.ToJSONData(testJsonData);

            //format JSON
            Console.WriteLine(data.ToJson(true));//Formatting JSON strings for better readability
            Console.WriteLine(data.ToJson());//Not formatting JSON strings, poor readability, but JSON strings are short, more suitable for transmission

            //Deep clone JSONData
            JSONData clone = JSONData.DeepClone(data);//Deep clone JSONData, 'clone' object is not the same object with 'data'.
            JSONData notClone = data;  //Just set value, 'notClone' is alias of the 'data', they are the same object.
            data["i"] = 100;//Modify the 'data' object, that will not effect the 'clone' object.
            Console.WriteLine(clone.ToJson());
            Console.WriteLine(notClone.ToJson());

            //Enum => JSONData
            JSONData testJSONData2Enum = TestHotUpdateEnum.Afternoon;//direct assignment
            //JSONData => Enum
            TestHotUpdateEnum testEnum2JSONData = (TestHotUpdateEnum)(int)testJSONData2Enum;//Force convert to base enum type (byte/sbyte/short/ushort/int/uint/long/ulong) first and then force convert to enum type

            JSONData tttt1 = new JSONData(){ 1, 2, 3 };
            //Recommand this way to foreach the List style JSONData
            foreach (JSONData one in tttt1)
            {
                Console.WriteLine($"Test list1:{one}");
            }
            foreach (var one in tttt1.Value as List<JSONData>)
            {
                Console.WriteLine($"Test list2:{one}");
            }
            //Not recommand this way to foreach the List style JSONData due to it will new a List<int>.
            foreach (int one in (List<int>)tttt1)
            {
                Console.WriteLine($"Test list3:{one}");
            }
            JSONData tttt2 = new JSONData { { "aa", 1 },{ "bb", "a" } };
            //Recommand this way to foreach the List style JSONData
            foreach (KeyValuePair<string, JSONData> one in tttt2)
            {
                Console.WriteLine($"Test Dictionary1:{one.Key}={one.Value}");
            }
            foreach (var one in tttt2.Value as Dictionary<string, JSONData>)
            {
                Console.WriteLine($"Test Dictionary2:{one.Key}={one.Value}");
            }
            //Not recommand this way to foreach the Dictionary style JSONData due to it will new a Dictionary<string, object>.
            foreach (var one in (Dictionary<string, object>)tttt2)
            {
                Console.WriteLine($"Test Dictionary3:{one.Key}={one.Value}");
            }

            //The below 5 dictionary values are same:
            Dictionary<string, int> dic1 = new JSONData() { { "aa", 1 }, { "bb", 2 } };//Normal dictionary type JSONData initialization
            Dictionary<string, int> dic2 = (JSONData)"{\"aa\":1,\"bb\":2}";//Normal JSON string convert to Dictionary
            Dictionary<string, int> dic3 = (JSONData)"aa_1|bb_2";//The string split with `|` and `_`, convert to Dictionary
            Dictionary<string, int> dic4 = (JSONData)"aa,1,bb,2";//The string split with `,`, convert to Dictionary
            Dictionary<string, int> dic5 = new Dictionary<string, int>() { { "aa", 1 }, { "bb", 2 } };//Normal dictionary initialization

            //The below 5 list values are same:
            List<int> list1 = new JSONData() { 1, 2, 3 };//Normal list type JSONData initialization
            List<int> list2 = (JSONData)"[1,2,3]";//Normal JSON string convert to List
            List<int> list3 = (JSONData)"1,2,3";//The string split with `,`, convert to List
            List<int> list4 = (JSONData)"1|2|3";//The string split with `|`, convert to List
            List<int> list5 = new List<int>() { 1, 2, 3 };//Normal list initialization
            Console.ReadKey();
        }
    }

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
    //mark the class as KissJsonDontSerialize,will ignore while serialize JSON
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

    public struct Vector2
    {
        public float x;
        public float y;
        public override string ToString()
        {
            return $"({x:F2},{y:F2})";
        }
    }
}
