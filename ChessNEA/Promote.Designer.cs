namespace ChessNEA
{
    partial class Promote
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
            this.queenBtn = new System.Windows.Forms.Button();
            this.rookBtn = new System.Windows.Forms.Button();
            this.bishopBtn = new System.Windows.Forms.Button();
            this.btnKnight = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // queenBtn
            // 
            this.queenBtn.Location = new System.Drawing.Point(25, 40);
            this.queenBtn.Name = "queenBtn";
            this.queenBtn.Size = new System.Drawing.Size(80, 80);
            this.queenBtn.TabIndex = 0;
            this.queenBtn.UseVisualStyleBackColor = true;
            this.queenBtn.Click += new System.EventHandler(this.queenBtn_Click);
            // 
            // rookBtn
            // 
            this.rookBtn.Location = new System.Drawing.Point(111, 40);
            this.rookBtn.Name = "rookBtn";
            this.rookBtn.Size = new System.Drawing.Size(80, 80);
            this.rookBtn.TabIndex = 1;
            this.rookBtn.UseVisualStyleBackColor = true;
            this.rookBtn.Click += new System.EventHandler(this.rookBtn_Click);
            // 
            // bishopBtn
            // 
            this.bishopBtn.Location = new System.Drawing.Point(197, 40);
            this.bishopBtn.Name = "bishopBtn";
            this.bishopBtn.Size = new System.Drawing.Size(80, 80);
            this.bishopBtn.TabIndex = 2;
            this.bishopBtn.UseVisualStyleBackColor = true;
            this.bishopBtn.Click += new System.EventHandler(this.bishopBtn_Click);
            // 
            // btnKnight
            // 
            this.btnKnight.Location = new System.Drawing.Point(283, 40);
            this.btnKnight.Name = "btnKnight";
            this.btnKnight.Size = new System.Drawing.Size(80, 80);
            this.btnKnight.TabIndex = 3;
            this.btnKnight.UseVisualStyleBackColor = true;
            this.btnKnight.Click += new System.EventHandler(this.btnKnight_Click);
            // 
            // Promote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.btnKnight);
            this.Controls.Add(this.bishopBtn);
            this.Controls.Add(this.rookBtn);
            this.Controls.Add(this.queenBtn);
            this.Name = "Promote";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Promote_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button queenBtn;
        private System.Windows.Forms.Button rookBtn;
        private System.Windows.Forms.Button bishopBtn;
        private System.Windows.Forms.Button btnKnight;
    }
}