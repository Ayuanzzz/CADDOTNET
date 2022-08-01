using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using DotNetArX;

namespace SouthLINQ
{
    public class SouthLINQ
    {
        [CommandMethod("SouthNull")]
        public static void SouthNull()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //提示用户输入扩展应用名
            PromptStringOptions optStr = new PromptStringOptions("\n请输入扩展应用名：");
            PromptResult strRes = ed.GetString(optStr);
            //构建过滤器列表
            TypedValueList values = new TypedValueList();
            //选择layer1上的直线对象
            values.Add(DxfCode.ExtendedDataRegAppName, "Grade");
            values.Add(DxfCode.ExtendedDataAsciiString, "次干道");
            //构建过滤器列表
            SelectionFilter filter = new SelectionFilter(values);
            //选择图形中所有满足过滤器的对象
            PromptSelectionResult psr = ed.SelectAll(filter);
            //if (psr.Status == PromptStatus.OK)
            //{
            //    Application.ShowAlertDialog("选择集中实体的数量:" + psr.Value.Count.ToString());
            //}
            SelectionSet ss = psr.Value;
            using(Transaction trans = doc.TransactionManager.StartTransaction())
            {
                foreach(ObjectId id in ss.GetObjectIds())
                {
                    Entity ent = (Entity)trans.GetObject(id, OpenMode.ForWrite);
                    ent.ColorIndex = 1;
                }
                trans.Commit();
            }
        }
    }
}
