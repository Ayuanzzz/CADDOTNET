using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace DotNetArX
{
    public static class CircleTools
    {
        public static bool CreateCircle(this Circle circle,Point3d pt1,Point3d pt2,Point3d pt3)
        {
            //先判断三点是否共线,得到pt1点指向Pt2，pt3点的矢量
            Vector3d va = pt1.GetVectorTo(pt2);
            Vector3d vb = pt1.GetVectorTo(pt3);
            //如两矢量夹角为0或180度，则三点共线
            if (va.GetAngleTo(vb) == 0 | va.GetAngleTo(vb) == Math.PI)
            {
                return false;
            }
            else
            {
                //创建一个几何类的圆弧对象
                CircularArc3d geArc = new CircularArc3d(pt1, pt2, pt3);
                //将圆弧对象的圆心和半径赋值给圆
                circle.Center = geArc.Center;
                circle.Radius = geArc.Radius;
                return true;
            }
        }
    }
}
