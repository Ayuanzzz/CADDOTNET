using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;

namespace DotNetArX
{
    public static class EntTools
    {
        //移动
        public static void Move(this ObjectId id,Point3d sourcePt,Point3d targetPt)
        {
            //构建用于移动实体的矩阵
            Vector3d vector = targetPt.GetVectorTo(sourcePt);
            Matrix3d mt = Matrix3d.Displacement(vector);
            //以写的方式打开id表示的实体对象
            Entity ent = (Entity)id.GetObject(OpenMode.ForWrite);
            //移动实体
            ent.TransformBy(mt);
            //为防止错误，切换实体为读的状态
            ent.DowngradeOpen();
        }
        public static void Move(this Entity ent,Point3d sourcePt,Point3d targetPt)
        {
            //构建用于移动实体的矩阵
            if (ent.IsNewObject)
            {
                //构建用于移动实体的矩阵
                Vector3d vector = targetPt.GetVectorTo(sourcePt);
                Matrix3d mt = Matrix3d.Displacement(vector);
                ent.TransformBy(mt);
            }
            //如果是已经添加到数据库中的实体
            else
            {
                ent.ObjectId.Move(sourcePt, targetPt);
            }
        }
        //复制
        public static ObjectId Copy(this ObjectId id,Point3d sourcePt,Point3d targetPt)
        {
            //构建用于复制实体的矩阵
            Vector3d vector = targetPt.GetVectorTo(sourcePt);
            Matrix3d mt = Matrix3d.Displacement(vector);
            //获取id表示的实体对象
            Entity ent = (Entity)id.GetObject(OpenMode.ForRead);
            //获取实体的复制件
            Entity entCopy = ent.GetTransformedCopy(mt);
            //将复制的实体对象添加到模型空间
            ObjectId copyId = id.Database.AddToModelSpace(entCopy);
            return copyId;
        }
        //旋转
        public static void Rotate(this ObjectId id,Point3d basePt,double angle)
        {
            Matrix3d mt = Matrix3d.Rotation(angle, Vector3d.ZAxis, basePt);
            Entity ent = (Entity)id.GetObject(OpenMode.ForWrite);
            ent.TransformBy(mt);
            ent.DowngradeOpen();
        }
        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="basePt">缩放中心</param>
        /// <param name="scaleFactor">缩放比例</param>
        public static void Scale(this ObjectId id,Point3d basePt,double scaleFactor)
        {
            Matrix3d mt = Matrix3d.Scaling(scaleFactor, basePt);
            Entity ent = (Entity)id.GetObject(OpenMode.ForWrite);
            ent.TransformBy(mt);
            ent.DowngradeOpen();
        }
        //偏移
        public static ObjectIdCollection Offset(this ObjectId id, double dis)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            Curve cur = id.GetObject(OpenMode.ForWrite) as Curve;
            if (cur != null)
            {
                try
                {
                    //获取偏移的对象集合
                    DBObjectCollection offsetCurves = cur.GetOffsetCurves(dis);
                    //将对象集合类型转换为实体类的数组，以方便加入实体的操作
                    Entity[] offsetEnts = new Entity[offsetCurves.Count];
                    offsetCurves.CopyTo(offsetEnts, 0);
                    //将偏移的对象加入到数据库
                    ids = id.Database.AddToModelSpace(offsetEnts);
                }
                //捕获异常
                catch
                {
                    Application.ShowAlertDialog("无法偏移！");
                }
            }
            //如果不是曲线
            else
                Application.ShowAlertDialog("无法偏移！");
            //返回偏移后的实体id集合 
            return ids;
        }
    }
}
