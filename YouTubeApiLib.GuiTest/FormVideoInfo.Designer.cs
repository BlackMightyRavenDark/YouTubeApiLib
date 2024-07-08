namespace YouTubeApiLib.GuiTest
{
	partial class FormVideoInfo
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
			this.textBoxFullInfo = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textBoxFullInfo
			// 
			this.textBoxFullInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxFullInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.textBoxFullInfo.Location = new System.Drawing.Point(4, 12);
			this.textBoxFullInfo.Multiline = true;
			this.textBoxFullInfo.Name = "textBoxFullInfo";
			this.textBoxFullInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxFullInfo.Size = new System.Drawing.Size(446, 204);
			this.textBoxFullInfo.TabIndex = 3;
			this.textBoxFullInfo.WordWrap = false;
			// 
			// FormVideoInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(462, 228);
			this.Controls.Add(this.textBoxFullInfo);
			this.MinimizeBox = false;
			this.Name = "FormVideoInfo";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Информацияя о видео";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox textBoxFullInfo;
	}
}