using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetArX;

namespace Chap04
{
    public class Layers
    {
        [CommandMethod("CreateLayer")]
        public void CreateLayer()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            string layerName = "";
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                while(layerName=="")//如果没有输入，则循环
                {
                    //提示用户输入图层名
                    PromptResult pr = ed.GetString("请输入图层名称");
                    if (pr.Status != PromptStatus.OK) return;
                    try
                    {
                        //验证输入字符串是否符合表命名规则
                        SymbolUtilityServices.ValidateSymbolName(pr.StringResult, false);
                        layerName = pr.StringResult;
                        if (db.AddLayer(layerName) != ObjectId.Null)
                        {
                            //提示用户输入图层颜色
                            PromptIntegerResult pir = ed.GetInteger("请输入图层颜色值");
                            if (pir.Status != PromptStatus.OK) return;
                            //设置图层颜色
                            db.SetLayerColor(layerName, (short)pir.Value);
                            //设置新添加的图层为当前层
                            db.SetCurrentLayer(layerName);
                            break;//添加图层成功，跳出循环
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex)
                    {
                        //捕捉到异常
                        ed.WriteMessage(ex.Message + "\n");
                    }
                }
                trans.Commit();
            }
        }

        [CommandMethod("DelRedLayer")]
        public void DelRedLayer()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                var redLayers = (from layer in db.GetAllLayers()
                                 where layer.Color == Color.FromColorIndex(ColorMethod.ByAci, 1)
                                 select layer.Name).ToList();
                redLayers.ForEach(layer => db.DeleteLayer(layer));
                trans.Commit();
            }
        }
    }
}
