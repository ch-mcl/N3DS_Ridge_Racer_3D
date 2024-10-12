
namespace RidgeRacer3DTableTool
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgWorkerPack = new System.ComponentModel.BackgroundWorker();
            this.bgWorkerUnpack = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(241, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please Drag&Drop TBL file for Ridge Racer 3D.";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 12);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 18);
            this.progressBar1.TabIndex = 1;
            // 
            // bgWorkerPack
            // 
            this.bgWorkerPack.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerPack_DoWork);
            this.bgWorkerPack.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorkerPack_ProgressChanged);
            this.bgWorkerPack.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerPack_RunWorkerCompleted);
            // 
            // bgWorkerUnpack
            // 
            this.bgWorkerUnpack.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerUnpack_DoWork);
            this.bgWorkerUnpack.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorkerUnpack_ProgressChanged);
            this.bgWorkerUnpack.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerUnpack_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 110);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Ridge Racer 3D String Table Tool";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgWorkerUnpack;
        private System.ComponentModel.BackgroundWorker bgWorkerPack;
        private System.Windows.Forms.RadioButton radioJP;
        private System.Windows.Forms.RadioButton radioUS;
        private System.Windows.Forms.RadioButton radioPAL;
        private System.Windows.Forms.RadioButton radioACV3A;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

