using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace DotNetArX
{
    public static class CommandTools
    {
        public static void SendCommand(this Editor ed,params string[] args)
        {
            //获取Document的COM对象
            Document doc = Application.DocumentManager.MdiActiveDocument;
            //Type AcadDocument = Type.GetTypeFromHandle(Type.GetTypeHandle(doc.AcadDocument));

        }
    }
}
