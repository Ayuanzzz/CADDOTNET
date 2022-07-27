using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace DotNetArX
{
    public static class SelectionTools
    {
        //选择通过某点的实体
        public static PromptSelectionResult SelectAtPoint(this Editor ed,Point3d point,SelectionFilter filter)
        {
            return ed.SelectCrossingWindow(point, point, filter);
        }
        public static PromptSelectionResult SelectAtPoint(this Editor ed,Point3d point)
        {
            return ed.SelectCrossingWindow(point, point);
        }
    }
}
