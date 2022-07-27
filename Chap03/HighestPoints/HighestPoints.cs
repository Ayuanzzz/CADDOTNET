using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using DotNetArX;

namespace HighestPoints
{
    public class HighestPoints
    {
        [CommandMethod("HighestPoints")]
        public void GetHighestPoints()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                var dbpoints = db.GetEntsInModelSpace<DBPoint>();
                //按z值降序排列点，选择最大的三个，并强制执行查询
                var highestPoints = (from p in dbpoints
                                     orderby p.Position.Z descending
                                     select p.Position).Take(3).ToList();
                //命令行输出查询点
                for(int i = 0; i < highestPoints.Count; i++)
                {
                    ed.WriteMessage("\n {0}:{1}", i, highestPoints[i]);
                }
                trans.Commit();
            }
        }
    }
}
