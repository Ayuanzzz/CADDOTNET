using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using DotNetArX;

namespace Chap02
{
    public static class EntEdit
    {
        [CommandMethod("EditLine")]
        public static void EditLine()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            //事务处理管理器
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            //开始第一个事务处理
            using (Transaction tr1 = tm.StartTransaction())
            {
                Point3d ptStart = Point3d.Origin;
                Point3d ptEnd = new Point3d(100, 0, 0);
                Line line1 = new Line(ptStart, ptEnd);
                ObjectId id1 = db.AddToModelSpace(line1);
                //开始第二个事务处理
                using (Transaction tr2 = tm.StartTransaction())
                {
                    //切换line1的状态为可写
                    line1.UpgradeOpen();
                    line1.ColorIndex = 1;
                    ObjectId id2 = id1.Copy(ptStart, ptEnd);
                    //以直线的起点旋转90度
                    id2.Rotate(ptEnd, Math.PI / 2);
                    //开始第三个事务处理
                    using (Transaction tr3 = tm.StartTransaction())
                    {
                        Line line2 = (Line)tr3.GetObject(id2, OpenMode.ForWrite);
                        line2.ColorIndex = 3;
                        //撤销事务处理三，line2颜色不变
                        tr3.Abort();
                    }
                    tr2.Commit();
                }
                tr1.Commit();
            }
        }
    }
}
