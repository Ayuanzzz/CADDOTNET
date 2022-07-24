using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetArX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chap02
{
    public class Lines
    {
        [CommandMethod("FirstLine")]
        public static void FirstLine()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Point3d startPoint = new Point3d(0, 100, 0);
            Point3d endPoint = new Point3d(100, 100, 0);
            Line line = new Line(startPoint, endPoint);
            using (Transaction trans = db.TransactionManager.StartOpenCloseTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                btr.AppendEntity(line);
                trans.AddNewlyCreatedDBObject(line, true);
                trans.Commit();
            }
        }
        [CommandMethod("SecondLine")]
        public static void SecondLine()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Point3d startPoint = new Point3d(1, 100, 0);
            Point3d endPoint = new Point3d(0, 200, 0);
            Line line = new Line(startPoint, endPoint);
            db.AddToModelSpace(line);
        }
    }
}
