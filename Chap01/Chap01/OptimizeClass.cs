using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chap01
{
    public class OptimizeClass
    {
        [CommandMethod("OptCommand")]
        public void OptCommand()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            string fileName = @"\\vmware-host\Shared Folders\桌面\C#\demo\demo\bin\Debug\demo.dll";
            ExtensionLoader.Load(fileName);
            ed.WriteMessage("\n" + fileName + "被载入,请测试");
        }
    }
}
