using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace DotNetArX
{
    public static class Tools
    {
        /// <summary>
        /// 将实体添加到模型空间
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="ent">要添加的实体</param>
        /// <returns>返回添加到模型空间中的实体</returns>
        public static ObjectId AddToModelSpace(this Database db, Entity ent)
        {
            ObjectId entId;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                entId = btr.AppendEntity(ent);
                trans.AddNewlyCreatedDBObject(ent, true);
                trans.Commit();
            }
            return entId;
        }

        //可变参数形式封装
        public static ObjectIdCollection AddToModelSpace(this Database db, params Entity[] ents)
        {
            //声明ObjectId,用于返回
            ObjectIdCollection ids = new ObjectIdCollection();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                foreach (var ent in ents)
                {
                    ids.Add(btr.AppendEntity(ent));
                    trans.AddNewlyCreatedDBObject(ent, true);
                }
                //事务提交
                trans.Commit();
            }
            return ids;
        }

        /// <summary>
        /// 获取数据库对应的文档对象
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <returns>返回数据库对应的文档对象</returns>
        public static Document GetDocument(this Database db)
        {
            return Application.DocumentManager.GetDocument(db);
        }

        /// <summary>
        /// 获取块参照的块名（包括动态块）
        /// </summary>
        /// <param name="id">块参照的Id</param>
        /// <returns>返回块名</returns>
        public static string GetBlockName(this ObjectId id)
        {
            //获取块参照
            BlockReference bref = id.GetObject(OpenMode.ForRead) as BlockReference;
            if (bref != null)//如果是块参照
                return GetBlockName(bref);
            else
                return null;
        }

        /// <summary>
        /// 获取块参照的块名（包括动态块）
        /// </summary>
        /// <param name="bref">块参照</param>
        /// <returns>返回块名</returns>
        public static string GetBlockName(this BlockReference bref)
        {
            string blockName;//存储块名
            if (bref == null) return null;//如果块参照不存在，则返回
            if (bref.IsDynamicBlock) //如果是动态块
            {
                //获取动态块所属的动态块表记录
                ObjectId idDyn = bref.DynamicBlockTableRecord;
                //打开动态块表记录
                BlockTableRecord btr = (BlockTableRecord)idDyn.GetObject(OpenMode.ForRead);
                blockName = btr.Name;//获取块名
            }
            else //非动态块
                blockName = bref.Name; //获取块名
            return blockName;//返回块名
        }

        /// <summary>
        /// 获取指定名称的块属性值
        /// </summary>
        /// <param name="blockReferenceId">块参照的Id</param>
        /// <param name="attributeName">属性名</param>
        /// <returns>返回指定名称的块属性值</returns>
        public static string GetAttributeInBlockReference(this ObjectId blockReferenceId, string attributeName)
        {
            string attributeValue = string.Empty; // 属性值
            Database db = blockReferenceId.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 获取块参照
                BlockReference bref = (BlockReference)trans.GetObject(blockReferenceId, OpenMode.ForRead);
                // 遍历块参照的属性
                foreach (ObjectId attId in bref.AttributeCollection)
                {
                    // 获取块参照属性对象
                    AttributeReference attRef = (AttributeReference)trans.GetObject(attId, OpenMode.ForRead);
                    //判断属性名是否为指定的属性名
                    if (attRef.Tag.ToUpper() == attributeName.ToUpper())
                    {
                        attributeValue = attRef.TextString;//获取属性值
                        break;
                    }
                }
                trans.Commit();
            }
            return attributeValue; //返回块属性值
        }

        /// <summary>
        /// 获取指定块名的块参照
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="blockName">块名</param>
        /// <returns>返回指定块名的块参照</returns>
        public static List<BlockReference> GetAllBlockReferences(this Database db, string blockName)
        {
            List<BlockReference> blocks = new List<BlockReference>();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //打开指定块名的块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[blockName], OpenMode.ForRead);
                //获取指定块名的块参照集合的Id
                ObjectIdCollection blockIds = btr.GetBlockReferenceIds(true, true);
                foreach (ObjectId id in blockIds) // 遍历块参照的Id
                {
                    //获取块参照
                    BlockReference block = (BlockReference)trans.GetObject(id, OpenMode.ForRead);
                    blocks.Add(block); // 将块参照添加到返回列表 
                }
                trans.Commit();
            }
            return blocks; //返回块参照列表
        }

        /// <summary>
        /// 获取当前.NET程序所在的目录
        /// </summary>
        /// <returns>返回当前.NET程序所在的目录</returns>
        public static string GetCurrentPath()
        {
            var moudle = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0];
            return System.IO.Path.GetDirectoryName(moudle.FullyQualifiedName);
        }
    }
}
