using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace Chap06
{
    public class TXData1
    {
        [CommandMethod("GetXD")]

        static public void GetXData()

        {

            Document doc = Application.DocumentManager.MdiActiveDocument;

            Editor ed = doc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("\n请选择实体：");

            PromptEntityResult per = ed.GetEntity(peo);

            if (per.Status == PromptStatus.OK)

            {

                Transaction trans = doc.TransactionManager.StartTransaction();

                DBObject obj = trans.GetObject(per.ObjectId, OpenMode.ForRead);

                ResultBuffer rb = obj.XData;

                if (rb == null)

                    ed.WriteMessage("\n实体不包括扩展数据");

                else

                {

                    int n = 0;

                    foreach (TypedValue tv in rb)

                    {

                        ed.WriteMessage("\n类型值{0} - 类型: {1}, 值: {2}", n, tv.TypeCode, tv.Value);

                        n++;

                    }

                    rb.Dispose();

                }

                trans.Dispose();

            }

        }
    }
}
