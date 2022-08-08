using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using AcadAPP = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using DotNetArX;

namespace Chap10
{
    public partial class UCTreeView : UserControl
    {
        public UCTreeView()
        {
            InitializeComponent();
            //设置TreeView使用的图像列表
            this.treeViewEnts.ImageList = imageListNode;
        }

        private void UCTreeView_Load(object sender, EventArgs e)
        {

        }

        private void treeViewEnts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string rootName = findRoot(e.Node).Text;//选中节点的根节点名称
            //如果根节点名称与活动文档的名称不相同，则需要切换活动文档
            if(rootName!= AcadAPP.DocumentManager.MdiActiveDocument.Name)
            {
                //查找文档名为选中节点名的文档
                var docs = from Document doc in AcadAPP.DocumentManager
                           where doc.Name == rootName
                           select doc;
                if (docs.Count() == 1)//如果找到，则切换活动文档
                    AcadAPP.DocumentManager.MdiActiveDocument = docs.First();
            }
            switch (e.Node.Text)
            {
                case "门":
                    GetBlocksFromDwg("DOOR");
                    break;
                case "窗":
                    GetBlocksFromDwg("Window");
                    break;
                default:
                    this.dataGridViewEnts.DataSource = null;
                    break;
            }
        }

        private void GetBlocksFromDwg(string blockName)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
                using(DocumentLock loc = db.GetDocument().LockDocument())
            {
                //查找当前文档中块名以blockName开头的块参照
                var blocks = from block in db.GetEntsInModelSpace<BlockReference>()
                             where block.GetBlockName().StartsWith(blockName)
                             //设置中间变量
                             let SYM = block.ObjectId.GetAttributeInBlockReference("SYM.")
                             group block by SYM into g
                             orderby g.Key
                             select new
                             {
                                 符号 = g.Key,
                                 宽度 = g.First().ObjectId.GetAttributeInBlockReference("Rotation"),
                                 高度 = g.First().ObjectId.GetAttributeInBlockReference("HEIGHT"),
                                 个数 = g.Count()
                             };
                //设置DataGridView的数据源以显示块的分组统计信息
                this.dataGridViewEnts.DataSource = blocks.ToList();
                trans.Commit();
            }
        }
        TreeNode findRoot(TreeNode node)
        {
            if (node.Parent == null) return node;
            else return findRoot(node.Parent);
        }
    }
}
