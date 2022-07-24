using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace DotNetArX
{
    public static class GeTools
    {
        public static double AngleFromXAxis(this Point3d pt1,Point3d pt2)
        {
            //构建一个从第一点到第二点所确定的矢量
            Vector2d vector = new Vector2d(pt1.X - pt2.X, pt1.Y - pt2.Y);
            //返回该矢量和x轴正半轴的角度（弧度）
            return vector.Angle;
        }
    }
}
