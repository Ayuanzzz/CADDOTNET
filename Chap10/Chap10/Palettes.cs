using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using AcadAPP = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using System.Drawing;
using Autodesk.AutoCAD.Geometry;
using DotNetArX;

namespace Chap10
{
    public class Palettes
    {
        internal static PaletteSet ps = null;//面板对象
        static UCTreeView treeControl = null;//第一个面板中的控件
        static UCDockDrag dockControl = null;//第二个面板中的控件

        [CommandMethod("ShowPalette")]
        public void ShowPalette()
        {
            if (ps == null)//若面板未被创建
            {
                ps = new PaletteSet("工作空间", typeof(Palettes).GUID);
                treeControl = new UCTreeView();
                ps.Add("门窗统计", treeControl);
                dockControl = new UCDockDrag();
                ps.Add("停靠及拖放", dockControl);
                //添加文档打开事件
                AcadAPP.DocumentManager.DocumentCreated += new DocumentCollectionEventHandler(DocumentManger_DocumnetCreated);
                //添加文档关闭事件
                AcadAPP.DocumentManager.DocumentDestroyed += new DocumentDestroyedEventHandler(DocumentManger_DocumentDestroyed);
                //添加面板装载事件
                ps.Load += new PalettePersistEventHandler(ps_Load);
                //添加面板保存事件
                ps.Save += new PalettePersistEventHandler(ps_Save);
            }
            ps.Visible = true;
            updateTree();
        }

        void ps_Save(object sender, PalettePersistEventArgs e)
        {
            //if (e.ConfigurationSection.Contains("Drag"))
            //    dockControl.textBoxDrag.Text = e.ConfigurationSection.ReadProperty("Drag", "test").ToString();
        }

        void ps_Load(object sender, PalettePersistEventArgs e)
        {
            //将文本框中的文本保存到配置文件中
            //e.ConfigurationSection.WriteProperty("Drag", dockControl.textBoxDrag.Text);
        }

        void DocumentManger_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            updateTree();
        }

        void DocumentManger_DocumnetCreated(object sender, DocumentCollectionEventArgs e)
        {
            updateTree();
        }

        private static void updateTree()
        {
            //清空树形列表中的内容
            treeControl.treeViewEnts.Nodes.Clear();
            //遍历CAD文档
            foreach (Document doc in AcadAPP.DocumentManager)
            {
                TreeNode nodeDoor = new TreeNode("门");
                nodeDoor.ImageIndex = 1;
                nodeDoor.SelectedImageIndex = 1;
                TreeNode nodeWindow = new TreeNode("窗");
                nodeWindow.ImageIndex = 2;
                nodeWindow.SelectedImageIndex = 2;
                TreeNode[] nodes = new TreeNode[] { nodeDoor, nodeWindow };
                //定义一个以文档名称为标题的节点，该节点包含两个子节点：门、窗
                TreeNode nodeDoc = new TreeNode(doc.Name, nodes);
                nodeDoc.ImageIndex = 0;
                treeControl.treeViewEnts.Nodes.Add(nodeDoc);
            }
        }
    }
    public class MyDropTarget:DropTarget
    {
        static string dropText;
        Database db = HostApplicationServices.WorkingDatabase;
        Document doc = AcadAPP.DocumentManager.MdiActiveDocument;
        public override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                dropText = (string)e.Data.GetData(typeof(string));
                string cmd = string.Format("Drop\n{0},{1},0\n", e.X, e.Y);
                doc.SendStringToExecute(cmd, false, false, false);
            }
        }
        [CommandMethod("Drop")]
        public void Drop()
        {
            Editor ed = doc.Editor;
            if (dropText != null)
            {
                PromptPointOptions opt = new PromptPointOptions("请输入文本放置的位置");
                PromptPointResult ppr = ed.GetPoint(opt);
                if (ppr.Status != PromptStatus.OK) return;
                Point3d pos = ppr.Value;
                using(Transaction trans = db.TransactionManager.StartTransaction())
                {
                    DBText txt = new DBText();
                    txt.TextString = dropText;
                    //txt.Position = ed.PointToWorld(new Point((int)pos.X, (int)pos.Y));
                    db.AddToModelSpace(txt);
                    trans.Commit();
                }
                dropText = null;
            }
        }
    }

}


