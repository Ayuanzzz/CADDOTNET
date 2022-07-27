using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace DotNetArX
{
    public static class LayerTools
    {
        //添加层
        public static ObjectId AddLayer(this Database db,string layerName)
        {
            //打开层表
            LayerTable lt = (LayerTable)db.LayerTableId.GetObject(OpenMode.ForRead);
            if(!lt.Has(layerName))//如果不存在名为layername的图层，则新建一个图层
            {
                LayerTableRecord ltr = new LayerTableRecord();//定以一个新的层表记录
                ltr.Name = layerName;
                lt.UpgradeOpen();//切换层表的状态为写以添加新图层
                lt.Add(ltr);//将层表记录的信息添加到层表中
                //把层表记录添加到事务处理中
                db.TransactionManager.AddNewlyCreatedDBObject(ltr, true);
                lt.DowngradeOpen();//为了安全，将层表的状态切换为读
            }
            return lt[layerName];//返回新添加的层表记录
        }

        //修改图层颜色
        public static bool SetLayerColor(this Database db,string layerName,short colorIndex)
        {
            LayerTable lt = (LayerTable)db.LayerTableId.GetObject(OpenMode.ForRead);
            if (!lt.Has(layerName)) return false;//如果不存在名为layerName的图层，则返回
            ObjectId layerId = lt[layerName];
            LayerTableRecord ltr = (LayerTableRecord)layerId.GetObject(OpenMode.ForWrite);
            ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, colorIndex);
            ltr.DowngradeOpen();
            return true;//设置颜色成功
        }

        //指定当前层
        public static bool SetCurrentLayer(this Database db,string layerName)
        {
            LayerTable lt = (LayerTable)db.LayerTableId.GetObject(OpenMode.ForRead);
            if (!lt.Has(layerName)) return false;//如果不存在名为layerName的图层，则返回
            ObjectId layerId = lt[layerName];
            if (db.Clayer == layerId) return false;//如果指定层为当前层则返回
            db.Clayer = layerId;
            return true;
        }
        //获取所有图层
        public static List<LayerTableRecord> GetAllLayers(this Database db)
        {
            LayerTable lt = (LayerTable)db.LayerTableId.GetObject(OpenMode.ForRead);
            List<LayerTableRecord> ltrs = new List<LayerTableRecord>();
            //遍历层表
            foreach(ObjectId id in lt)
            {
                LayerTableRecord ltr = (LayerTableRecord)id.GetObject(OpenMode.ForRead);
                ltrs.Add(ltr);
            }
            return ltrs;
        }

        //删除层
        public static bool DeleteLayer(this Database db,string layerName)
        {
            LayerTable lt = (LayerTable)db.LayerTableId.GetObject(OpenMode.ForRead);
            //若层命为0或defpoints，则返回
            if (layerName == "0" || layerName == "Defpoints") return false;
            if (!lt.Has(layerName)) return false;
            ObjectId layerId = lt[layerName];
            if (layerId == db.Clayer) return false;
            LayerTableRecord ltr = (LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
            //如果要删除的图层包含对象或依赖外部参照，则返回
            lt.GenerateUsageData();
            if (ltr.IsUsed) return false;
            ltr.UpgradeOpen();
            ltr.Erase(true);
            return true;
        }
    }
}
