using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetArX;

namespace Chap04
{
    public class TextStyles
    {
        [CommandMethod("NewStyle")]
        public void NewStyle()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            using(Transaction trans= db.TransactionManager.StartTransaction())
            {
                //设置TrueType字体（仿宋体)
                //设置TrueType字体(仿宋体）
                ObjectId styleId = db.AddTextStyle("仿宋体", "simfang.ttf");
                DBText txt1 = new DBText();
                txt1.TextString = "仿宋体";
                //txt1.TextStyle = styleId;
                db.AddToModelSpace(txt1);
                trans.Commit();
            }
        }
    }
}
