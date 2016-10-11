namespace DiskDiff
{
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    ///    Summary description for About.
    /// </summary>
    public class About : System.Windows.Forms.Form
    {
        /// <summary>
        ///    Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button OK;

        public About()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container ();
			this.OK = new System.Windows.Forms.Button ();
			this.textBox1 = new System.Windows.Forms.TextBox ();
			//@this.TrayHeight = 0;
			//@this.TrayLargeIcon = false;
			//@this.TrayAutoArrange = true;
			OK.Location = new System.Drawing.Point (88, 136);
			OK.BackColor = System.Drawing.Color.AliceBlue;
			OK.Size = new System.Drawing.Size (75, 23);
			OK.TabIndex = 1;
			OK.Text = "OK";
			textBox1.Location = new System.Drawing.Point (24, 16);
			textBox1.Text = "DiskDiff - a Disk difference program Copyright 2001, Eric Gunnerson";
			textBox1.Multiline = true;
			textBox1.TabIndex = 2;
			textBox1.Size = new System.Drawing.Size (192, 112);
			this.Text = "About";
			//this.AutoScaleBaseSize = new System.Drawing.Size (5, 13);
			this.AcceptButton = this.OK;
			this.BackColor = System.Drawing.Color.MidnightBlue;
			this.ClientSize = new System.Drawing.Size (240, 181);
			this.Controls.Add (this.textBox1);
			this.Controls.Add (this.OK);
		}
    }
}
