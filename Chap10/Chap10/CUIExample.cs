using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Customization;
using DotNetArX;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Chap10
{
    class CUIExample
    {
        //设置CUI文件的名字，将其路径设置为当前运行目录
        string cuiFile = Tools.GetCurrentPath() + "\\MyCustom.cui";
        string menuGroupName = "MyCustom";
        Document activeDoc = Application.DocumentManager.MdiActiveDocument;
        [CommandMethod("AddMenu")]
        public void AddMenu()
        {
            string currentPath = Tools.GetCurrentPath();
            //装载局部CUI文件，若不存在，则创建
            CustomizationSection cs = activeDoc.AddCui(cuiFile, menuGroupName);
            cs.AddMacro("直线", "^C^C_Line ", "ID_MyLine", "创建直线段: LINE", currentPath + "\\Image\\沙冰.png");
            cs.LoadCui();
        }
    }
}
