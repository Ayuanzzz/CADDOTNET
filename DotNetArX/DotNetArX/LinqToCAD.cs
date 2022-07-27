using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;

namespace DotNetArX
{
    public static partial class LinqToCAD
    {
        public static List<T> GetEntsInDatabase<T>(this Database db,OpenMode mode,bool openErased) where T : Entity
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            List<T> ents = new List<T>();
            //获取类型T代表的DXF代码用于构建选择集过滤器
            string dxfname = RXClass.GetClass(typeof(T)).DxfName;
            //构建选择集过滤器
            TypedValue[] values = { new TypedValue((int)DxfCode.Start, dxfname) };
            SelectionFilter filter = new SelectionFilter(values);
            //选择符合条件的所有实体
            PromptSelectionResult entSelected = ed.SelectAll(filter);
            if (entSelected.Status == PromptStatus.OK)
            {
                //循环遍历符合条件的实体
                foreach(var id in entSelected.Value.GetObjectIds())
                {
                    //类型转换
                    T t = (T)(object)id.GetObject(mode, openErased);
                    ents.Add(t);
                }
            }
            return ents;
        }
        //只读方式获取数据库中的实体
        public static List<T> GetEntsInDatabase<T>(this Database db)where T : Entity
        {
            return GetEntsInDatabase<T>(db, OpenMode.ForRead, false);
        }

        public static List<T> GetEntsInModelSpace<T>(this Database db, OpenMode mode, bool openErased) where T : Entity
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            List<T> ents = new List<T>();
            string dxfname = RXClass.GetClass(typeof(T)).DxfName;
            //构建选择集过滤器        
            TypedValue[] values = { new TypedValue((int)DxfCode.Start, dxfname),
                                    new TypedValue((int)DxfCode.LayoutName,"Model")};
            SelectionFilter filter = new SelectionFilter(values);
            //选择符合条件的所有实体
            PromptSelectionResult entSelected = ed.SelectAll(filter);
            if (entSelected.Status == PromptStatus.OK)
            {
                foreach (var id in entSelected.Value.GetObjectIds())
                {
                    T t = (T)(object)id.GetObject(mode, openErased);
                    ents.Add(t);
                }
            }
            return ents;
        }

        public static List<T> GetEntsInModelSpace<T>(this Database db) where T:Entity
        {
            return GetEntsInModelSpace<T>(db, OpenMode.ForRead, false);
        }

    }
}
