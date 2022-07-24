using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using DotNetArX;

namespace Regions
{
    public class Regions
    {
        [CommandMethod("AddRegion")]
        public void AddRegion()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                //创建一个三角形
                Polyline triangle = new Polyline();
                triangle.CreatePolygon(new Point2d(550, 200), 3, 30);
                //根据三角形创建面域
                List<Region> regions = RegionTools.CreateRegion(triangle);
                //如果面域创建未成功，则返回
                if (regions.Count == 0) return;
                //将创建的面域添加到数据库中
                db.AddToModelSpace(regions[0]);
                trans.Commit();
            }
        }
    }
}
