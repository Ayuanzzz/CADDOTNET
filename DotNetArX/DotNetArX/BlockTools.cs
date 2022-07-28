using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace DotNetArX
{
    public static class BlockTools
    {

        //添加块到数据库
        public static ObjectId AddBlockTableRecord(this Database db,string blockName,List<Entity> ents)
        {
            BlockTable bt = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForRead);
            if (!bt.Has(blockName))
            {
                BlockTableRecord btr = new BlockTableRecord();
                btr.Name = blockName;
                ents.ForEach(ent => btr.AppendEntity(ent));
                bt.UpgradeOpen();
                bt.Add(btr);
                db.TransactionManager.AddNewlyCreatedDBObject(btr, true);
                bt.DowngradeOpen();
            }
            return bt[blockName];
        }

        public static ObjectId AddBlockTableRecord(this Database db,string blockName,params Entity[] ents)
        {
            return AddBlockTableRecord(db, blockName, ents.ToList());
        }

        //插入块参照
        public static ObjectId InsertBlockReference(this ObjectId spaceId,string layer,string blockName,Point3d position,Scale3d scale,double rotateAngle)
        {
            //存储要插入的块参照的Id
            ObjectId blockRefId;
            //获取数据库对象
            Database db = spaceId.Database;
            //以读的方式打开块表
            BlockTable bt = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForRead);
            if (!bt.Has(blockName)) return ObjectId.Null;
            BlockTableRecord space = (BlockTableRecord)spaceId.GetObject(OpenMode.ForWrite);
            //创建一个块参照并设置插入点
            BlockReference br = new BlockReference(position, bt[blockName]);
            br.ScaleFactors = scale;
            br.Layer = layer;
            br.Rotation = rotateAngle;
            blockRefId = space.AppendEntity(br);
            db.TransactionManager.AddNewlyCreatedDBObject(br, true);
            space.DowngradeOpen();
            return blockRefId;
        }

    }
}
