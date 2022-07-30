using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace DotNetArX
{
    public static class XDataTools
    {
        /// <summary>
        /// 添加扩展属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="regAppName">注册应用程序名</param>
        /// <param name="values">扩展数据项目列表</param>
        public static void AddXData(this ObjectId id, string regAppName, TypedValueList values)
        {
            Database db = id.Database;
            //获取数据库的注册应用程序表
            RegAppTable regTable = (RegAppTable)db.RegAppTableId.GetObject(OpenMode.ForWrite);
            if (!regTable.Has(regAppName))
            {
                RegAppTableRecord regRecord = new RegAppTableRecord();
                regRecord.Name = regAppName;//设则扩展数据名字
                //在注册应用程序表中加入扩展数据，并通知事务处理
                regTable.Add(regRecord);
                db.TransactionManager.AddNewlyCreatedDBObject(regRecord, true);
            }
            DBObject obj = id.GetObject(OpenMode.ForWrite);
            //将扩展数据的应用程序名添加到扩展数据项列表的第一项
            values.Insert(0, new TypedValue((int)DxfCode.ExtendedDataRegAppName, regAppName));
            obj.XData = values;
            regTable.DowngradeOpen();
        }
        //获取对象的扩展数据
        public static TypedValueList GetXData(this ObjectId id,string regAppName)
        {
            TypedValueList values = new TypedValueList();
            DBObject obj = id.GetObject(OpenMode.ForRead);
            values = obj.GetXDataForApplication(regAppName);
            return values;
        }
        //修改扩展数据
        public static void ModXData(this ObjectId id,string regAppName,DxfCode code,object oldValue,object newValue)
        {
            DBObject obj = id.GetObject(OpenMode.ForWrite);
            //获取regAppName注册应用程序下的扩展数据列表
            TypedValueList xdata = obj.GetXDataForApplication(regAppName);
            for (int i = 0; i < xdata.Count; i++)
            {
                TypedValue tv = xdata[i];
                //判断扩展数据的类型和值是否满足条件
                if (tv.TypeCode == (short)code && tv.Value.Equals(oldValue))
                {
                    //设置新的扩展数据值
                    xdata[i] = new TypedValue(tv.TypeCode, newValue);
                    break;
                }
            }
            obj.XData = xdata;
            obj.DowngradeOpen();
        }
        //删除扩展数据
        public static void RemoveXData(this ObjectId id,string regAppName)
        {
            DBObject obj = id.GetObject(OpenMode.ForWrite);
            TypedValueList xdata = obj.GetXDataForApplication(regAppName);
            if (xdata != null)
            {
                TypedValueList newValues = new TypedValueList();
                newValues.Add(DxfCode.ExtendedDataRegAppName, regAppName);
                obj.XData = newValues;
            }
            obj.DowngradeOpen();
        }
    }
}
