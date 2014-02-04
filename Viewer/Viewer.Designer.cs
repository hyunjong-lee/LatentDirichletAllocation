namespace Viewer
{
    partial class Viewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.modelFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonLoadModel = new System.Windows.Forms.Button();
            this.topicGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.topicGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonLoadModel
            // 
            this.buttonLoadModel.Location = new System.Drawing.Point(13, 13);
            this.buttonLoadModel.Name = "buttonLoadModel";
            this.buttonLoadModel.Size = new System.Drawing.Size(106, 43);
            this.buttonLoadModel.TabIndex = 0;
            this.buttonLoadModel.Text = "&Load Model";
            this.buttonLoadModel.UseVisualStyleBackColor = true;
            this.buttonLoadModel.Click += new System.EventHandler(this.buttonLoadModel_Click);
            // 
            // topicGridView
            // 
            this.topicGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.topicGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.topicGridView.Location = new System.Drawing.Point(125, 13);
            this.topicGridView.Name = "topicGridView";
            this.topicGridView.RowTemplate.Height = 23;
            this.topicGridView.Size = new System.Drawing.Size(655, 444);
            this.topicGridView.TabIndex = 1;
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 469);
            this.Controls.Add(this.topicGridView);
            this.Controls.Add(this.buttonLoadModel);
            this.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Viewer";
            this.Text = "LDA Visualization";
            ((System.ComponentModel.ISupportInitialize)(this.topicGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog modelFolderDialog;
        private System.Windows.Forms.Button buttonLoadModel;
        private System.Windows.Forms.DataGridView topicGridView;
    }
}

