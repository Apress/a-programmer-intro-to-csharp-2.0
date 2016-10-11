// 33 - Windows Forms\Getting Started
// copyright 2000 Eric Gunnerson
namespace DiskDiff
{
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Data;
    
    public class Form1 : System.Windows.Forms.Form
    {
        private System.ComponentModel.Container components;
        
        public Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        
        public override void Dispose()
        {
            base.Dispose();
            components.Dispose();
        }
        
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Size = new System.Drawing.Size(300,300);
            this.Text = "Form1";
        }
        
        public static void Main(string[] args) 
        {
            Application.Run(new Form1());
        }
    }
}