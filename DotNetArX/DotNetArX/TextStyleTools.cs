using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;

namespace DotNetArX
{
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
    }

}
