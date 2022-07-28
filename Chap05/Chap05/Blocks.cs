using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetArX;

namespace Chap05
{
    public class Blocks
    {
        //创建块
        [CommandMethod("MakeDoor")]
        public void MakeDoor()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                //设置门框的左边线
                Point3d pt1 = Point3d.Origin;
                Point3d pt2 = new Point3d(0, 1.0, 0);
                Line leftLine = new Line(pt1, pt2);
                //门框的下边线
                Point3d pt3 = new Point3d(1, 1.0, 0);
                Point3d pt4 = new Point3d(2, 1.0, 0);
                Line bottomLine = new Line(pt3, pt4);
                //添加到door块记录
                db.AddBlockTableRecord("DOOR", leftLine, bottomLine);
                trans.Commit();
            }
        }

        //插入块
        [CommandMethod("InsertDoor")]
        public void InsertDoor()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //获取当前空间
            ObjectId spaceId = db.CurrentSpaceId;
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                spaceId.InsertBlockReference("0", "DOOR", Point3d.Origin, new Scale3d(2), 0);
                trans.Commit();
            }
        }

    }
}
