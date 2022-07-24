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

namespace Interactive
{
    public class Interactive
    {
        //获取用户输入线宽
        public double GetWidth()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //定义一个实数的用户交互类
            PromptDoubleOptions optDou = new PromptDoubleOptions("\n请输入线宽");
            optDou.AllowNegative = false;//不允许输入负数
            optDou.DefaultValue = 0;//默认值
            PromptDoubleResult resDou = ed.GetDouble(optDou);
            if (resDou.Status == PromptStatus.OK)
            {
                double width = resDou.Value;
                return width;
            }
            else
                return 0;
        }
        //获取用户输入颜色
        public short GetColorIndex()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptIntegerOptions optInt = new PromptIntegerOptions("\n请输入颜色索引值(0~256)");
            optInt.DefaultValue = 0;
            //返回一个整数提示类
            PromptIntegerResult resInt = ed.GetInteger(optInt);
            if(resInt.Status == PromptStatus.OK)
            {
                short colorIndex = (short)resInt.Value;
                if (colorIndex > 256 | colorIndex < 0)
                {
                    return 0;
                }
                else
                {
                    return colorIndex;
                }
            }else
            {
                return 0;
            }
        }
    }
}
