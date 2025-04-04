namespace ChessNEA
{
    partial class GameWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameWindow));
            this.panelBoard = new System.Windows.Forms.Panel();
            this.lblCurrentTurn = new System.Windows.Forms.Label();
            this.WhiteTimer = new System.Windows.Forms.Timer(this.components);
            this.WhiteTimerText = new System.Windows.Forms.Label();
            this.BlackTimerText = new System.Windows.Forms.Label();
            this.BlackTimer = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panelBoard
            // 
            this.panelBoard.BackColor = System.Drawing.Color.Transparent;
            this.panelBoard.Location = new System.Drawing.Point(0, 0);
            this.panelBoard.Name = "panelBoard";
            this.panelBoard.Size = new System.Drawing.Size(640, 640);
            this.panelBoard.TabIndex = 0;
            // 
            // lblCurrentTurn
            // 
            this.lblCurrentTurn.AutoSize = true;
            this.lblCurrentTurn.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentTurn.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentTurn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblCurrentTurn.Location = new System.Drawing.Point(716, 35);
            this.lblCurrentTurn.Name = "lblCurrentTurn";
            this.lblCurrentTurn.Size = new System.Drawing.Size(193, 22);
            this.lblCurrentTurn.TabIndex = 1;
            this.lblCurrentTurn.Text = "Current Turn: White";
            // 
            // WhiteTimer
            // 
            this.WhiteTimer.Interval = 120;
            // 
            // WhiteTimerText
            // 
            this.WhiteTimerText.AutoSize = true;
            this.WhiteTimerText.BackColor = System.Drawing.Color.Transparent;
            this.WhiteTimerText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WhiteTimerText.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.WhiteTimerText.Location = new System.Drawing.Point(779, 556);
            this.WhiteTimerText.Name = "WhiteTimerText";
            this.WhiteTimerText.Size = new System.Drawing.Size(71, 31);
            this.WhiteTimerText.TabIndex = 2;
            this.WhiteTimerText.Text = "2:00";
            this.WhiteTimerText.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // BlackTimerText
            // 
            this.BlackTimerText.AutoSize = true;
            this.BlackTimerText.BackColor = System.Drawing.Color.Transparent;
            this.BlackTimerText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BlackTimerText.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.BlackTimerText.Location = new System.Drawing.Point(779, 57);
            this.BlackTimerText.Name = "BlackTimerText";
            this.BlackTimerText.Size = new System.Drawing.Size(71, 31);
            this.BlackTimerText.TabIndex = 3;
            this.BlackTimerText.Text = "2:00";
            this.BlackTimerText.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // BlackTimer
            // 
            this.BlackTimer.Interval = 120;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(785, 590);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 49);
            this.button1.TabIndex = 4;
            this.button1.Text = "Resign";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // GameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(984, 761);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.BlackTimerText);
            this.Controls.Add(this.WhiteTimerText);
            this.Controls.Add(this.lblCurrentTurn);
            this.Controls.Add(this.panelBoard);
            this.Name = "GameWindow";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.GameWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelBoard;
        private System.Windows.Forms.Label lblCurrentTurn;
        private System.Windows.Forms.Timer WhiteTimer;
        private System.Windows.Forms.Label WhiteTimerText;
        private System.Windows.Forms.Label BlackTimerText;
        private System.Windows.Forms.Timer BlackTimer;
        private System.Windows.Forms.Button button1;
    }
}

