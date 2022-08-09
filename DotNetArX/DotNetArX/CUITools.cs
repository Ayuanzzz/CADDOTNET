using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Specialized;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;

namespace DotNetArX
{
    public static class CUITools
    {
        //获取打开主CUI文件
        public static CustomizationSection GetMainCustomizationSection(this Document doc)
        {
            //获得主CUI文件所在的位置
            string mainCuiFile = Application.GetSystemVariable("MENUNAME") + ".cuix";
            //打开主CUI文件
            return new CustomizationSection(mainCuiFile);
        }
        //创建局部CUI文件
        public static CustomizationSection AddCui(this Document doc,string cuiFile,string menuGroupName)
        {
            CustomizationSection cs;//声明CUI文件对象
            if (!File.Exists(cuiFile))
            {
                cs = new CustomizationSection();
                cs.MenuGroupName = menuGroupName;
                cs.SaveAs(cuiFile);
            }
            else cs = new CustomizationSection(cuiFile);
            return cs;
        }
        //装载指定的局部CUI文件
        public static void LoadCui(this CustomizationSection cs)
        {
            if (cs.IsModified) cs.Save();//若CUI文件被修改，则保存
            //保存CMDECHO及FILEDIA系统变量
            object oldCmdEcho = Application.GetSystemVariable("CMDECHO");
            object oldFileDia = Application.GetSystemVariable("FILEDIA");
            //设置CMDECHO=0，控制不在命令行上回显提示和输入信息
            Application.SetSystemVariable("CMDECHO", 0);
            //禁止显示文件对话框
            Application.SetSystemVariable("FILEDIA", 0);
            Document doc = Application.DocumentManager.MdiActiveDocument;
            //获取主CUI文件
            CustomizationSection mainCs = doc.GetMainCustomizationSection();
            //如果已存在局部CUI文件，则先卸载
            if (mainCs.PartialCuiFiles.Contains(cs.CUIFileName))
                doc.SendStringToExecute("_.cuiunload " + cs.CUIFileBaseName + " ", false, false, false);
            //装载CUI文件
            doc.SendStringToExecute("_.cuiload " + cs.CUIFileName + " ", false, false, false);
            //恢复CMDECHO和FILEDIA系统变量的初始值
            doc.SendStringToExecute("(setvar \"FILEDIA\" " + oldFileDia.ToString() + ")(princ) ", false, false, false);
            doc.SendStringToExecute("(setvar \"CMDECHO\" " + oldCmdEcho.ToString() + ")(princ) ", false, false, false);
        }
        //添加菜单项所要执行的宏
        public static MenuMacro AddMacro(this CustomizationSection source,string name,string command,string tag,string helpString,string imagePath)
        {
            MenuGroup menuGroup = source.MenuGroup;//获取CUI文件中的菜单组
            //判断菜单组中是否已经定义与菜单组名相同的宏集合
            MacroGroup mg = menuGroup.FindMacroGroup(menuGroup.Name);
            if (mg == null)
                mg = new MacroGroup(menuGroup.Name, menuGroup);
            foreach (MenuMacro macro in mg.MenuMacros)
                if (macro.ElementID == tag) return null;
            //在宏集合中创建一个命令宏
            MenuMacro MenuMacro = new MenuMacro(mg, name, command, tag);
            //指定命令宏的说明信息，在状态栏中显示
            MenuMacro.macro.HelpString = helpString;
            //指定命令宏的大小图像的路径
            MenuMacro.macro.LargeImage = MenuMacro.macro.SmallImage = imagePath;
            return MenuMacro;
        }
        //添加下拉菜单
        public static PopMenu AddPopMenu(this MenuGroup menuGroup,string name,StringCollection aliasList,string tag)
        {
            PopMenu pm = null;//声明下拉菜单对象
            if (menuGroup.PopMenus.IsNameFree(name))
            {
                //为下拉菜单指定显示名称，别名，标识符和所属的菜单组
                pm = new PopMenu(name, aliasList, tag, menuGroup);
            }
            return pm;
        }
        //为菜单添加菜单项
        public static PopMenuItem AddMenuItem(this PopMenu parentMenu,int index,string name,string macroId)
        {
            PopMenuItem newPmi = null;
            foreach (PopMenuItem pmi in parentMenu.PopMenuItems)
                if (pmi.Name == name) return newPmi;
            //定义一个菜单项，指定所属菜单及位置
            newPmi = new PopMenuItem(parentMenu, index);
            //如果name不为空，则指定菜单项的显示名为name，否则使用命令宏的名称
            if (name != null) newPmi.Name = name;
            newPmi.MacroID = macroId;
            return newPmi;
        }
        //添加工具栏
        public static Toolbar AddToolbar(this MenuGroup menuGroup,string name)
        {
            Toolbar tb = null;
            if (menuGroup.Toolbars.IsNameFree(name))
            {
                tb = new Toolbar(name, menuGroup);
                tb.ToolbarOrient = ToolbarOrient.floating;
                tb.ToolbarVisible = ToolbarVisible.show;
            }
            return tb;
        }
        /// <summary>
        /// 向工具栏添加按钮
        /// </summary>
        /// <param name="parent">按钮所属的工具栏</param>
        /// <param name="index">按钮在工具栏上的位置</param>
        /// <param name="name">按钮的显示名称</param>
        /// <param name="macroId">按钮的命令宏的Id</param>
        /// <returns>返回工具栏按钮对象</returns>
        public static ToolbarButton AddToolbarButton(this Toolbar parent, int index, string name, string macroId)
        {
            //创建一个工具栏按钮对象，指定其命令宏Id、显示名称、所属的工具栏和位置
            ToolbarButton button = new ToolbarButton(macroId, name, parent, index);
            return button;//返回工具栏按钮对象
        }
    }
}
