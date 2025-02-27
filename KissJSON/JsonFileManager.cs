/*
 *           C#Like
 * KissJson : Keep It Simple Stupid JSON
 * Copyright © 2022-2025 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace CSharpLike
{
    /// <summary>
    /// Json file manager, for load JSON file that convert from Excel file.
    /// </summary>
    public class JsonFileManager
    {
        /// <summary>
        /// Clear all loaded Excel.
        /// Normally you should call this manually after you initializ your hot update script..
        /// </summary>
        public static void ClearAll()
        {
            object o = null;
            KissJson.Clear(o);
            mLoadStates.Clear();
        }
        /// <summary>
        /// Get a row data from Excel by key.
        /// You must make sure that data was initialized.
        /// </summary>
        /// <param name="type">Type of your data. e.g. typeof(ItemJSON)</param>
        /// <param name="key">The unique key of your data. That is string type.</param>
        /// <returns>Data ojbect</returns>
        public static object Get(object type, string key)
        {
            return KissJson.Get(type, key);
        }
        /// <summary>
        /// Get JSONData object from Excel data.
        /// If that Excel data wasn't loaded, it will load it automatically.
        /// </summary>
        /// <param name="fileName">File name of the Excel file name</param>
        /// <param name="key">Row key</param>
        /// <param name="column">Column name</param>
        /// <returns>JSONData object</returns>
        public static JSONData GetJSON(string fileName, string key, string column)
        {
            return KissJson.GetJSON(fileName, key, column);
        }
        /// <summary>
        /// Synchronizing load all Excel data by Type.
        /// </summary>
        /// <param name="type">Type of your data. e.g. typeof(ItemJSON)</param>
        /// <param name="fileName">File name in AssetBundle</param>
        public static void Load(object type, string fileName)
        {
            mLoadStates[type] = false;
            byte[] buff = File.ReadAllBytes(fileName);
            if (buff != null)
            {
                if (type is string)
                    KissJson.Load(type as string, KissJson.ToJSONData(buff));
                else
                    KissJson.Load(type, KissJson.ToJSONData(buff));
                mLoadStates[type] = true;
            }
        }
        static Dictionary<object, bool> mLoadStates = new Dictionary<object, bool>();
    }
}