using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace DotNetArX
{
    public static class DictTools
    {
        /// <summary>
        /// 添加扩展记录
        /// </summary>
        /// <param name="id">对象id</param>
        /// <param name="searchKey">扩展记录名称</param>
        /// <param name="values">扩展记录内容</param>
        /// <returns></returns>
        public static ObjectId AddXrecord(this ObjectId id,string searchKey,TypedValueList values)
        {
            DBObject obj = id.GetObject(OpenMode.ForRead);
            if (obj.ExtensionDictionary.IsNull)
            {
                obj.UpgradeOpen();
                obj.CreateExtensionDictionary();
                obj.DowngradeOpen();
            }
            DBDictionary dict = (DBDictionary)obj.ExtensionDictionary.GetObject(OpenMode.ForRead);
            //如果扩展字典中已包含指定的扩展记录，则返回
            if (dict.Contains(searchKey)) return ObjectId.Null;
            //为对象新建一个扩展记录
            Xrecord xrec = new Xrecord();
            xrec.Data = values;
            dict.UpgradeOpen();
            //在扩展字典中加入新建的扩展记录，并指定它的搜索关键字
            ObjectId idXrec = dict.SetAt(searchKey, xrec);
            id.Database.TransactionManager.AddNewlyCreatedDBObject(xrec, true);
            dict.DowngradeOpen();
            return idXrec;
        }

        //获取扩展记录
        public static TypedValueList GetXrecord(this ObjectId id,string searchKey)
        {
            DBObject obj = id.GetObject(OpenMode.ForRead);
            ObjectId dictId = obj.ExtensionDictionary;
            if (dictId.IsNull) return null;
            DBDictionary dict = (DBDictionary)dictId.GetObject(OpenMode.ForRead);
            if (!dict.Contains(searchKey)) return null;
            ObjectId xrecordId = dict.GetAt(searchKey);//获取扩展记录的Id
            Xrecord xrecord = (Xrecord)xrecordId.GetObject(OpenMode.ForRead);
            return xrecord.Data;
        }
    }
}
