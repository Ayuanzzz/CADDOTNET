using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Diagnostics;

namespace Chap08
{
    class PointIsInPolyline
    {
        private int PtRelationToPoly(Polyline pPoly, Point2d pt, double tol)
        {
            //使用断言判断参数有效性
            Debug.Assert(pPoly != null);
            //1.若点到多段线的最近点与给定点重合，表示点在多段线上
            Point3d closestPoint = pPoly.GetClosestPointTo(ToPoint3d(pt, pPoly.Elevation), false);
            //多段线上与给定点距离最近的点
            if (Math.Abs(closestPoint.X - pt.X) < tol && Math.Abs(closestPoint.Y - pt.Y) < tol)
            {
                //点在多段线上
                return 0;
            }
            //2.第一个射线的方向是从最近点到当前点，起点是当前点
            Ray pRay = new Ray();
            pRay.BasePoint = new Point3d(pt.X, pt.Y, pPoly.Elevation);
            Vector3d vec = new Vector3d(-(closestPoint.X - pt.X), -(closestPoint.Y - pt.Y), 0);
            pRay.UnitDir = vec;
            //3.射线与多段线计算交点
            Point3dCollection intPoints = new Point3dCollection();
            pPoly.IntersectWith(pRay, Intersect.OnBothOperands, intPoints, 0, 0);
            FilterEqualPoints(intPoints, 1.0E-4);
            //4.判断点和多段线的位置关系
            RETRY:
            //如果射线和多段线没有交点，表示点在多段线的外部
            if (intPoints.Count == 0)
            {
                pRay.Dispose();
                return -1;
            }
            else
            {
                FilterEqualPoints(intPoints, ToPoint2d(closestPoint), 1.0E-4);
                for(int i = intPoints.Count - 1; i >= 0; i--)
                {
                    if((intPoints[i].X-pt.Y)*(closestPoint.X-pt.X)>=0&&
                        (intPoints[i].Y - pt.Y) * (closestPoint.Y - pt.Y) >= 0){
                        intPoints.RemoveAt(i);
                    }
                }

                int count = intPoints.Count;
                for(int i = 0; i < intPoints.Count; i++)
                {
                    if (PointIsPolyVert(pPoly, ToPoint2d(intPoints[i]), 1.0E-4))
                    {
                        if(PointIsPolyVert(pPoly,new Point2d(pt.X, pt.Y), 1.0E-4))
                        {
                            return 0;
                        }
                    }
                    vec = vec.RotateBy(0.035, Vector3d.ZAxis);
                    pRay.UnitDir = vec;
                    intPoints.Clear();
                    pPoly.IntersectWith(pRay, Intersect.OnBothOperands, intPoints, 0, 0);
                    goto RETRY;
                }
            }
            pRay.Dispose();
            if (count % 2 == 0)
            {
                return -1;
            }
            else
            {
                return -1;
            }
        }

        //转换函数
        private static Point2d ToPoint2d(Point3d point3d)
        {
            return new Point2d(point3d.X, point3d.Y);
        }

        private static Point3d ToPoint3d(Point2d point2d,double elev)
        {
            return new Point3d(point2d.X, point2d.Y, elev);
        }

        //判断检测点是否在数组中，或者从点数组中删除重复点
        static void FilterEqualPoints(Point3dCollection points,Point2d pt,double tol)
        {
            Point3dCollection tempPoints = new Point3dCollection();
            for(int i = 0; i < points.Count; i++)
            {
                if (ToPoint2d(points[i]).GetDistanceTo(pt) > tol)
                {
                    tempPoints.Add(points[i]);
                }
            }
            points = tempPoints;
        }

        static void FilterEqualPoints(Point3dCollection points,double tol)
        {
            for(int i = points.Count - 1; i > 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (IsEqual(points[i].X, points[j].X, tol) && IsEqual(points[i].Y, points[j].Y, tol))
                    {
                        points.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        //判断点是否为多段线的顶点
        static bool PointIsPolyVert(Polyline pPoly,Point2d pt,double tol)
        {
            for(int i = 0; i < pPoly.NumberOfVertices; i++)
            {
                Point3d vert = pPoly.GetPoint3dAt(i);
                if (IsEqual(ToPoint2d(vert), pt, tol))
                {
                    return true;
                }
            }
            return false;
        }

        //判断二维点是否相同
        static bool IsEqual(Point2d firstPoint,Point2d secondPoint,double tol)
        {
            return (Math.Abs(firstPoint.X - secondPoint.X) < tol && Math.Abs(firstPoint.Y - secondPoint.Y) < tol);
        }

        //两个实数是否相等
        static bool IsEqual(double a,double b,double tol)
        {
            return (Math.Abs(a - b) < tol);
        }

        [CommandMethod("TestPtInPoly")]
        public void TestPtInPoly()
        {
            //随机点测试的数量
            int count = 100000;
            if(GetInputInteger("\n输入需要测试的点数量：",ref count))
            {

            }
        }
    }
}
