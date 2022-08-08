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
    public partial class UCDockDrag : UserControl
    {
        public UCDockDrag()
        {
            InitializeComponent();
        }

        private void comboBoxDock_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBoxDock.SelectedIndex)
            {
                case 0:
                    Palettes.ps.Dock = DockSides.Bottom;
                    break;
                case 1:
                    Palettes.ps.Dock = DockSides.Left;
                    break;
                case 2:
                    Palettes.ps.Dock = DockSides.Right;
                    break;
                case 3:
                    Palettes.ps.Dock = DockSides.Top;
                    break;
                case 4:
                    Palettes.ps.Dock = DockSides.None;
                    break;
            }
        }
    }
}
