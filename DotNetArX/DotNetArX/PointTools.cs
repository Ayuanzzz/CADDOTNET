using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetArX
{
    public static class PointTools
    {
        //转换函数
        public static Point2d ToPoint2d(Point3d point3d)
        {
            return new Point2d(point3d.X, point3d.Y);
        }

        public static Point3d ToPoint3d(Point2d point2d)
        {
            return new Point3d(point2d.X, point2d.Y, 0);
        }
    }
}
