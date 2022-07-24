using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using DotNetArX;

namespace PolylinesDemo
{
    public static class Polylines
    {
        [CommandMethod("AddPolyLine")]
        public static void AddPolyLine()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Point2d startPoint = Point2d.Origin;
                Point2d endPoint = new Point2d(100, 100);
                Point2d pt = new Point2d(60, 70);
                Point2d center = new Point2d(50, 50);
                //创建直线
                Polyline pline = new Polyline();
                pline.CreatePolyline(startPoint, endPoint);
                //创建矩形
                Polyline rectangle = new Polyline();
                pline.CreateRectangle(pt, endPoint);
                //创建半径为30的圆
                Polyline circle = new Polyline();
                circle.CreatePolyCircle(center, 30);
                db.AddToModelSpace(pline);
                trans.Commit();
            }
        }
    }
}
