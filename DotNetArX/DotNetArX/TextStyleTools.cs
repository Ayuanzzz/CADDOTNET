using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;

namespace DotNetArX
{
    //0 英文
    //1 与当前操作系统语言有关
    //134 简体中文
    //136 繁体中文
    //255 与操作系统有关
    public enum FontCharset
    {
        Ansi=0,Default=1,GB2312=134,Big5=136,OEM=255
    }

    //0 默认字宽
    //1 固定字宽
    //2 可变字宽
    public enum FontPitch
    {
        Default=0,Fixed=1,Variable=2
    }

    public enum FontFamily
    {
        Dontcare=0,Roman=16,Swiss=32,Modern=48,Script=64,Decorative=80
    }
    public static class TextStyleTools
    {
        public static ObjectId AddTextStyle(
            this Database db,string styleName,string fontFilename,string bigFontFilename)
        {
            TextStyleTable st = (TextStyleTable)db.TextStyleTableId.GetObject(OpenMode.ForRead);
            if (!st.Has(styleName))
            {
                TextStyleTableRecord str = new TextStyleTableRecord();
                str.Name = styleName;
                str.FileName = fontFilename;
                str.BigFontFileName = bigFontFilename;
                str.UpgradeOpen();
                st.Add(str);
                db.TransactionManager.AddNewlyCreatedDBObject(str, true);
                st.DowngradeOpen();
            }
            return st[styleName];
        }
        //不需要大字体样式
        public static ObjectId AddTextStyle(
            this Database db, string styleName, string fontFilename)
        {
            return db.AddTextStyle(styleName, fontFilename, "");
        }
        //文字加粗，倾斜
        public static ObjectId AddTextStyle(
            this Database db, string styleName, string fontName, bool bold,bool italic,int charset,int pitchAndFamily)
        {
            TextStyleTable st = (TextStyleTable)db.TextStyleTableId.GetObject(OpenMode.ForRead);
            if (!st.Has(styleName))
            {
                TextStyleTableRecord str = new TextStyleTableRecord();
                str.Name = styleName;
                str.Font = new FontDescriptor(fontName, bold, italic, charset, pitchAndFamily);
                st.UpgradeOpen();
                st.Add(str);
                db.TransactionManager.AddNewlyCreatedDBObject(str, true);
                st.DowngradeOpen();
            }
            return st[styleName];
        }
        /// <summary>
        /// 设置文字样式
        /// </summary>
        /// <param name="styleId"></param>
        /// <param name="textSize"></param>
        /// <param name="xscale">宽度因子</param>
        /// <param name="obliquingAngle">倾斜角度</param>
        /// <param name="isVertical"></param>
        /// <param name="upsideDown">是否上下颠倒</param>
        /// <param name="backwards">是否反向</param>
        /// <param name="annotative"></param>
        /// <param name="paperOrientation"></param>
        public static void SetTextStyleProp(this ObjectId styleId,double textSize,double xscale,
            double obliquingAngle,bool isVertical,bool upsideDown,bool backwards,AnnotativeStates annotative,
            bool paperOrientation)
        {
            TextStyleTableRecord str =styleId.GetObject(OpenMode.ForWrite) as TextStyleTableRecord;
            if (str == null) return;
            str.TextSize = textSize;
            str.XScale = xscale;
            str.ObliquingAngle = obliquingAngle;
            str.IsVertical = isVertical;
            str.FlagBits = (byte)0;
            str.FlagBits += upsideDown ? (byte)2 : (byte)0;//是否上下颠倒
            str.FlagBits += backwards ? (byte)4 : (byte)0;//是否反向
            str.Annotative = annotative;
            str.SetPaperOrientation(paperOrientation);
            str.DowngradeOpen();
        }
    }

}
