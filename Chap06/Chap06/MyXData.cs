using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetArX;

namespace MyXData
{
    public class MyXData
    {
        //添加扩展数据
        [CommandMethod("AddX")]
        public void AddX()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptEntityOptions opt = new PromptEntityOptions("\n请选择表示董事长的多行文本");
            opt.SetRejectMessage("\n您选择的不是多行文本，请重新选择");
            opt.AddAllowedClass(typeof(MText), true);
            PromptEntityResult entResult = ed.GetEntity(opt);
            if (entResult.Status != PromptStatus.OK) return;
            ObjectId id = entResult.ObjectId;//用户选择多行文本的ObjectId
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                TypedValueList values = new TypedValueList();
                //添加整型(表示用工编号)和字符串(表示职位)扩展数据项;
                values.Add(DxfCode.ExtendedDataInteger32, 1002);
                values.Add(DxfCode.ExtendedDataAsciiString, "董事长");
                //为实体添加应用程序名为"EMPLOYEE"的扩展数据
                id.AddXData("EMPLOYEE", values);
                trans.Commit();
            }
        }

        //鼠标停留显示扩展数据
        [CommandMethod("StartMonitor")]
        public void StartMonitor()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //开始鼠标监控事件
            ed.PointMonitor += new PointMonitorEventHandler(ed_PointMointor);
        }
        //鼠标监控事件处理函数
        void ed_PointMointor(object sender,PointMonitorEventArgs e)
        {
            string employeeInfo = "";
            //获取命令行对象（鼠标监控事件的发起者），用于获取文档对象
            Editor ed = (Editor)sender;
            Document doc = ed.Document;
            //获取鼠标停留出的实体
            FullSubentityPath[] paths = e.Context.GetPickedEntities();
            using(Transaction trans = doc.TransactionManager.StartTransaction())
            {
                //如果鼠标停留处有实体
                if (paths.Length > 0)
                {
                    //获取实体
                    FullSubentityPath path = paths[0];
                    MText mtext = trans.GetObject(path.GetObjectIds()[0], OpenMode.ForRead) as MText;
                    if(mtext != null)
                    {
                        //获取扩展数据
                        TypedValueList xdata = mtext.ObjectId.GetXData("EMPLOYEE");
                        if (xdata != null)
                        {
                            employeeInfo += "员工编号：" + xdata[1].Value.ToString() + "\n职位：" + xdata[2].Value.ToString();
                        }
                    }
                }
                trans.Commit();
            }
            if (employeeInfo != "")
            {
                //在鼠标停留出显示提示信息
                e.AppendToolTipText(employeeInfo);
            }
        }
        //停止鼠标监控事件
        [CommandMethod("StopMonitor")]
        public  void StopMonitor()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.PointMonitor -= new PointMonitorEventHandler(ed_PointMointor);
        }

        //修改扩展数据
        [CommandMethod("ModX")]
        public void ModX()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptEntityOptions opt = new PromptEntityOptions("\n请选择多行文本");
            opt.SetRejectMessage("\n您选择的不是多行文本，请重新选择");
            opt.AddAllowedClass(typeof(MText), true);
            PromptEntityResult entResult = ed.GetEntity(opt);
            if (entResult.Status != PromptStatus.OK) return;
            ObjectId id = entResult.ObjectId;
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                //需改扩展数据1002为1003
                id.ModXData("EMPLOYEE", DxfCode.ExtendedDataInteger32, 1002, 1003);
                trans.Commit();
            }
        }
        
        //删除扩展数据
        [CommandMethod("DelX")]
        public void DelX()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptEntityOptions opt = new PromptEntityOptions("\n请选择多行文本");
            opt.SetRejectMessage("\n您选择的不是多行文本，请重新选择");
            opt.AddAllowedClass(typeof(MText), true);
            PromptEntityResult entResult = ed.GetEntity(opt);
            if (entResult.Status != PromptStatus.OK) return;
            ObjectId id = entResult.ObjectId;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                id.RemoveXData("EMPLOYEE");
                trans.Commit();
            }
        }
    }
}
