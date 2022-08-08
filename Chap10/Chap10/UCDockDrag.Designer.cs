namespace Chap10
{
    partial class UCDockDrag
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.comboBoxDock = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxDrag = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxDock
            // 
            this.comboBoxDock.FormattingEnabled = true;
            this.comboBoxDock.Items.AddRange(new object[] {
            "底部",
            "左侧",
            "右侧",
            "顶部",
            "浮动"});
            this.comboBoxDock.Location = new System.Drawing.Point(36, 25);
            this.comboBoxDock.Name = "comboBoxDock";
            this.comboBoxDock.Size = new System.Drawing.Size(121, 26);
            this.comboBoxDock.TabIndex = 0;
            this.comboBoxDock.SelectedIndexChanged += new System.EventHandler(this.comboBoxDock_SelectedIndexChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxDrag);
            this.groupBox1.Location = new System.Drawing.Point(36, 102);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "拖放演示";
            // 
            // textBoxDrag
            // 
            this.textBoxDrag.Location = new System.Drawing.Point(6, 41);
            this.textBoxDrag.Name = "textBoxDrag";
            this.textBoxDrag.Size = new System.Drawing.Size(100, 28);
            this.textBoxDrag.TabIndex = 3;
            // 
            // UCDockDrag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBoxDock);
            this.Name = "UCDockDrag";
            this.Size = new System.Drawing.Size(297, 288);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDock;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxDrag;
    }
}
