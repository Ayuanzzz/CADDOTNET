using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace DotNetArX
{
    public static class PolylineTools
    {
        public static void CreatePolyline(this Polyline pline,Point2dCollection pts)
        {
            for(int i = 0; i < pts.Count; i++)
            {
                //添加多段线的顶点
                pline.AddVertexAt(i, pts[i], 0, 0, 0);
            }
        }
        public static void CreatePolyline(this Polyline pline,params Point2d[] pts)
        {
            pline.CreatePolyline(new Point2dCollection(pts));
        }
        //根据两个角点创建矩形
        public static void CreateRectangle(this Polyline pline,Point2d pt1,Point2d pt2)
        {
            //设置矩形的四个顶点
            double minX = Math.Min(pt1.X, pt2.X);
            double maxX = Math.Max(pt1.X, pt2.X);
            double minY = Math.Min(pt1.Y, pt2.Y);
            double maxY = Math.Max(pt1.Y, pt2.Y);
            Point2dCollection pts = new Point2dCollection();
            pts.Add(new Point2d(minX, minY));
            pts.Add(new Point2d(minX, maxY));
            pts.Add(new Point2d(maxX, maxY));
            pts.Add(new Point2d(maxX, minY));
            pline.CreatePolyline(pts);
            pline.Closed = true;
        }
        //根据圆心和半径创建多段线形式的圆
        public static void CreatePolyCircle(this Polyline pline,Point2d centerPoint,double radius)
        {
            //计算多段线的顶点
            Point2d pt1 = new Point2d(centerPoint.X + radius, centerPoint.Y);
            Point2d pt2 = new Point2d(centerPoint.X - radius, centerPoint.Y);
            Point2d pt3 = new Point2d(centerPoint.X + radius, centerPoint.Y);
            Point2dCollection pts = new Point2dCollection();
            //添加多段线的顶点
            pline.AddVertexAt(0, pt1, 1, 0, 0);
            pline.AddVertexAt(0, pt2, 1, 0, 0);
            pline.AddVertexAt(0, pt3, 1, 0, 0);
            pline.Closed = true;
        }
        //根据中心点，边数，和外接圆半径来创建正多边形
        public static void CreatePolygon(this Polyline pline,Point2d centerPoint,int number,double radius)
        {
            Point2dCollection pts = new Point2dCollection(number);
            //计算每条边对应的角度
            double angle = 2 * Math.PI / number;
            //计算多边形的顶点
            for(int i = 0; i < number; i++)
            {
                Point2d pt = new Point2d(centerPoint.X + radius * Math.Cos(i * angle), centerPoint.Y + radius * Math.Sin(i * angle));
                pts.Add(pt);
            }
            pline.CreatePolyline(pts);
            pline.Closed = true;
        }
    }
}
