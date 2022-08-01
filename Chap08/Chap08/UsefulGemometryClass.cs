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

namespace Chap08
{
    public class UsefulGemometryClass
    {
        //计算直线和圆的交点
        [CommandMethod("IntersectWith")]
        public void IntersectWith()
        {
            CircularArc2d geArc = new CircularArc2d(Point2d.Origin, 50, 0, 6, Vector2d.XAxis, false);
            Line2d geLine = new Line2d(Point2d.Origin, new Point2d(10, 10));
            //计算并输出交点
            Point2d[] intPoints = geArc.IntersectWith(geLine);
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\n 直线和圆弧有{0}个交点.", intPoints.Length);
            for(int i = 0; i < intPoints.Length; i++)
            {
                ed.WriteMessage("\n交点{0}坐标:({1},{2})", i + 1, intPoints[i].X, intPoints[i].Y);
            }
        }
        //计算两条曲线相交之后形成的边界线
        [CommandMethod("CurveBoolean")]
        public void CurveBoolean()
        {
            //提示用户选择要计算距离的两条直线
            ObjectIdCollection polyIds = new ObjectIdCollection();
            if(PromptSelectEnts("\n选择两条多段线:","LWPOLYLINE",ref polyIds))
            {
                Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                if (polyIds.Count != 2)
                {
                    ed.WriteMessage("\n必须选择两条多段线进行操作.");
                    return;
                }
                Database db = HostApplicationServices.WorkingDatabase;
                using(Transaction trans = db.TransactionManager.StartTransaction())
                {
                    //获得两条曲线的交点
                    Polyline poly1 = (Polyline)trans.GetObject(polyIds[0], OpenMode.ForRead);
                    Polyline poly2 = (Polyline)trans.GetObject(polyIds[1], OpenMode.ForRead);
                    Point3dCollection intPoints = new Point3dCollection();
                    poly1.IntersectWith(poly2, Intersect.OnBothOperands, intPoints, 0, 0);
                    if (intPoints.Count < 2)
                    {
                        ed.WriteMessage("\n曲线交点少于2ge，无法进行计算.");
                    }
                    //根据交点和参数值获得交点之间的曲线
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    GetCurveBetweenIntPoints(trans, btr, poly1, intPoints);
                    GetCurveBetweenIntPoints(trans, btr, poly2, intPoints);
                    trans.Commit();
                }
            }
        }

        //提示用户选择一组实体
        public static bool PromptSelectEnts(string prompt,string entTypeFilter,ref ObjectIdCollection entIds)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = prompt;
            SelectionFilter sf = new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.Start, entTypeFilter) });
            PromptSelectionResult psr = ed.GetSelection(pso, sf);
            SelectionSet ss = psr.Value;
            if (ss != null)
            {
                entIds = new ObjectIdCollection(ss.GetObjectIds());
                return entIds.Count > 0;
            }
            else
            {
                return false;
            }
        }

        //使用一个点数组来分割多段线，将得到的分段曲线都添加到模型空间，并从中删除第一段和最后一段曲线
        private void GetCurveBetweenIntPoints(Transaction trans,BlockTableRecord btr,Polyline poly,Point3dCollection points)
        {
            DBObjectCollection curves = poly.GetSplitCurves(points);
            for(int i = 0; i < curves.Count; i++)
            {
                if (i > 0 && i < curves.Count - 1)
                {
                    Entity ent = curves[i] as Entity;
                    ent.ColorIndex = 1;
                    btr.AppendEntity(ent);
                    trans.AddNewlyCreatedDBObject(ent, true);
                }
                else
                {
                    curves[i].Dispose();
                }
            }
        }

        //多段线和直线求交点
        [CommandMethod("PolyIntersect")]
        public void PolyIntersect()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //提示用户输入误差范围
            PromptDoubleOptions pdo = new PromptDoubleOptions("\n输入判断容差<0.0001>:");
            pdo.DefaultValue = 0.0001;
            pdo.AllowNone = true;
            PromptDoubleResult pdr = ed.GetDouble(pdo);
            if (pdr.Status == PromptStatus.OK)
            {
                //提示用户选择多段线
                ObjectId polyId = new ObjectId();
                if(PromptSelectEntity("\n选择多段线：",out polyId))
                {
                    Database db = HostApplicationServices.WorkingDatabase;
                    using(Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = polyId.GetObject(OpenMode.ForRead) as Polyline;
                        if (poly != null)
                        {
                            //提示用户选择直线
                            ObjectId lineId = new ObjectId();
                            if(PromptSelectEntity("\n选择直线:",out lineId))
                            {
                                Line line = lineId.GetObject(OpenMode.ForRead) as Line;
                                //进行相交判断
                                if (line != null)
                                {
                                    Point3dCollection intPoints = new Point3dCollection();
                                    PolyIntersectWithLine(poly, line, pdr.Value, ref intPoints);
                                    ed.WriteMessage("\n两个实体交点数量：{0}", intPoints.Count);
                                    for(int i = 0; i < intPoints.Count; i++)
                                    {
                                        ed.WriteMessage("\n交点{0}:({1}{2})", i + 1, intPoints[i].X, intPoints[i].Y);
                                    }
                                }
                                else
                                {
                                    ed.WriteMessage("\n选择的实体不是直线.");
                                }
                            }
                        }else
                        {
                            ed.WriteMessage("\n选择的实体不是多段线");
                        }
                    }
                }
            }
        }
        // 提示用户选择一个实体
        public static bool PromptSelectEntity(string prompt, out ObjectId entId)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityOptions peo = new PromptEntityOptions(prompt);
            peo.AllowObjectOnLockedLayer = true;
            PromptEntityResult per = ed.GetEntity(peo);
            if (per.Status == PromptStatus.OK)
            {
                entId = per.ObjectId;
                return true;
            }
            else
            {
                entId = new ObjectId();
                return false;
            }
        }

        //构造几何对象
        private void PolyIntersectWithLine(Polyline poly,Line line,double tol,ref Point3dCollection points)
        {
            Point2dCollection intPoints2d = new Point2dCollection();
            //获取直线对应的几何类
            LineSegment2d geLine = new LineSegment2d(ToPoint2d(line.StartPoint), ToPoint2d(line.EndPoint));
            //每一段分别计算交点
            Tolerance tolerance = new Tolerance(tol, tol);
            for(int i = 0; i < poly.NumberOfVertices; i++)
            {
                if (i < poly.NumberOfVertices - 1 || poly.Closed)
                {
                    SegmentType st = poly.GetSegmentType(i);
                    if (st == SegmentType.Line)
                    {
                        LineSegment2d geLineSeg = poly.GetLineSegment2dAt(i);
                        Point2d[] pts = geLineSeg.IntersectWith(geLine, tolerance);
                        if (pts != null)
                        {
                            for(int j = 0; j < pts.Length; j++)
                            {
                                if (FindPointIn(intPoints2d, pts[j], tol) < 0)
                                {
                                    intPoints2d.Add(pts[j]);
                                }
                            }
                        }
                    }
                    else if (st == SegmentType.Arc)
                    {
                        CircularArc2d geArcSeg = poly.GetArcSegment2dAt(i);
                        Point2d[] pts = geArcSeg.IntersectWith(geLine, tolerance);
                        if (pts != null)
                        {
                            for (int j = 0; j < pts.Length; j++)
                            {
                                if (FindPointIn(intPoints2d, pts[j], tol) < 0)
                                {
                                    intPoints2d.Add(pts[j]);
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < intPoints2d.Count; i++)
            {
                points.Add(ToPoint3d(intPoints2d[i]));
            }
        }

        //重复检测
        private int FindPointIn(Point2dCollection points,Point2d pt,double tol)
        {
            for(int i = 0; i < points.Count; i++)
            {
                if (Math.Abs(points[i].X - pt.X) < tol && Math.Abs(points[i].Y - pt.Y) < tol)
                {
                    return i;
                }
            }
            return -1;
        }

        //转换函数
        private static Point2d ToPoint2d(Point3d point3d)
        {
            return new Point2d(point3d.X, point3d.Y);
        }

        private static Point3d ToPoint3d(Point2d point2d)
        {
            return new Point3d(point2d.X, point2d.Y, 0);
        }
    }
    
}
