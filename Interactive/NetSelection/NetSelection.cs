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

namespace NetSelection
{
    public class NetSelection
    {
        [CommandMethod("TestGetSelect")]
        public static void TestGetSelect()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            //生成三个同心圆
            Circle cir1 = new Circle(Point3d.Origin, Vector3d.ZAxis, 10);
            Circle cir2 = new Circle(Point3d.Origin, Vector3d.ZAxis, 20);
            Circle cir3 = new Circle(Point3d.Origin, Vector3d.ZAxis, 30);
            db.AddToModelSpace(new Circle[] { cir1, cir2, cir3 });
            //提示用户选择对象
            PromptSelectionResult psr = ed.GetSelection();
            //若未选择，则返回
            if (psr.Status != PromptStatus.OK) return;
            //获取选择集
            SelectionSet ss = psr.Value;
            //信息框提示选择集中包含的实体个数
            Application.ShowAlertDialog("选择集中实体的数量：" + ss.Count.ToString());
        }
        //合并选择集
        [CommandMethod("MergeSelection")]
        public static void MergeSelection()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //第一次选择
            PromptSelectionResult ss1 = ed.GetSelection();
            if (ss1.Status != PromptStatus.OK) return;
            Application.ShowAlertDialog("第一个选择集中实体的数量：" + ss1.Value.Count.ToString());
            //第二次选则
            PromptSelectionResult ss2 = ed.GetSelection();
            if (ss2.Status != PromptStatus.OK) return;
            Application.ShowAlertDialog("第二个选择集中实体的数量：" + ss1.Value.Count.ToString());
            //合并选择集
            var ss3 = ss1.Value.GetObjectIds().Union(ss2.Value.GetObjectIds());
            Application.ShowAlertDialog("合并后选择集中实体的数量:" + ss3.Count().ToString());
        }
        [CommandMethod("TestPickFirst", CommandFlags.UsePickSet)]
        public static void TestPickFirst()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //获取当前已选择的实体
            PromptSelectionResult psr = ed.SelectImplied();
            //在命令发出前已有实体被选中
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet ss1 = psr.Value;
                Application.ShowAlertDialog("PickFirst 示例：当前已选择的实体个数：" + ss1.Count.ToString());
                //清空当前选择集
                ed.SetImpliedSelection(new ObjectId[0]);
                psr = ed.GetSelection();//提示用户进行新的选择
                if (psr.Status == PromptStatus.OK)
                {
                    //设置当前已选择的实体
                    ed.SetImpliedSelection(psr.Value.GetObjectIds());
                    SelectionSet ss2 = psr.Value;
                    Application.ShowAlertDialog("PickFirst 示例：当前已选择的实体个数：" + ss2.Count.ToString());
                }
                else
                {
                    Application.ShowAlertDialog("PickFirst 示例：当前已选择的实体个数：0");
                }
            }
        }
        //选择集过滤器
        [CommandMethod("TestFilter")]
        public static void TestFilter()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //构建过滤器列表
            TypedValueList values = new TypedValueList();
            //选择layer1上的直线对象
            values.Add(DxfCode.LayerName, "layer1");
            values.Add(typeof(Line));
            //构建过滤器列表
            SelectionFilter filter = new SelectionFilter(values);
            //选择图形中所有满足过滤器的对象
            PromptSelectionResult psr = ed.SelectAll(filter);
            if (psr.Status == PromptStatus.OK)
            {
                Application.ShowAlertDialog("选择集中实体的数量:" + psr.Value.Count.ToString());
            }
        }
    }
}
