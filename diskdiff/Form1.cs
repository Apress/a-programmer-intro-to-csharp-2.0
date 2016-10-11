namespace DiskDiff
{
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Data;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Soap;
	using System.Diagnostics;

    /// <summary>
    ///    Summary description for Form1.
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem_MRU4;
		private System.Windows.Forms.MenuItem menuItem_MRU3;
		private System.Windows.Forms.MenuItem menuItem_MRU2;
		private System.Windows.Forms.MenuItem menuItem_MRU1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem FileSave;
		private System.Windows.Forms.MenuItem FileOpen;
		private System.Windows.Forms.ContextMenu treeContextMenu;
		private System.Windows.Forms.ImageList imageTreeView;
		private System.Windows.Forms.Button Cancel;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.MenuItem Configure;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.TreeView treeView1;

		DirectoryNode directoryNode = null;
		DirectoryNode directoryNodeBaseline = null;
		string rootDirectory = @"c:\windows";

		DateTime scanStartTime;
  MRU_Config mru = new MRU_Config();

		const int menuIndexFilename = 0;
		const int menuIndexDelete = 1;
		const int menuIndexDeleteContents = 2;
		const int menuIndexViewInNotepad = 4;
		const int menuIndexLaunch = 5;
        public Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
			AddContextMenuItem("Filename");
			AddContextMenuItem("Delete");
			AddContextMenuItem("Delete Contents");
			AddContextMenuItem("-");
			AddContextMenuItem("View in Notepad");
			AddContextMenuItem("Launch");

			Console.WriteLine("Start");

			DoTree();

			UpdateMRUList();
        }

		void UpdateMRUList()
		{
				// do the MRU list...
			this.menuItem_MRU1.Text = "&1: " + mru[0];
			this.menuItem_MRU2.Text = "&2: " + mru[1];
			this.menuItem_MRU3.Text = "&3: " + mru[2];
			this.menuItem_MRU4.Text = "&4: " + mru[3];
		}			

		void AddContextMenuItem(string itemString)
		{
			MenuItem menuItem;
			menuItem = new MenuItem(itemString,  new EventHandler(treeContextMenuClick));
			treeContextMenu.MenuItems.Add(menuItem);
		}

		void DoTree()
		{
			//ClusterSize cs = new ClusterSize("c:");

			try
			{
				DirectoryNode newNode = new DirectoryNode(rootDirectory);

				directoryNode = newNode;
				directoryNode.DirectoryScanned += new DirectoryNode.ScannedEventHandler(myDirectoryScanned);
				directoryNode.PopulateComplete += new DirectoryNode.PopulateDoneEventHandler(DoTreeDone);
				directoryNode.FileScanned += new DirectoryNode.ScannedEventHandler(myFileScanned);
				scanStartTime = DateTime.Now;
				directoryNode.Populate();
				Cancel.Visible = true;
			//	statusBar1.Text = "";

			//	treeView1.Nodes.Clear();

			//	PopulateTree(treeView1.Nodes, directoryNode);
			}
			catch (DirectoryNotFoundException)
			{
				// don't do anything...

			}
		}
		
		void DoTreeDone(object sender, ScanCompleteEventArgs e)
		{
   if (InvokeRequired)
   {
    Invoke(new DirectoryNode.PopulateDoneEventHandler(DoTreeDone), sender, e);
    return;
   }
			Cancel.Visible = false;
			statusBar1.Text = "";

			if (e.Success)
			{
				if (directoryNodeBaseline != null)
				{
					directoryNode.CompareTrees(directoryNodeBaseline);
				}
				//statusBar1.Text = String.Format("Scan Time: {0}", DateTime.Now - scanStartTime);
				treeView1.Nodes.Clear();

				PopulateTreeNode(treeView1.Nodes, directoryNode);
			}
		}

		void myFileScanned(object sender, ScannedItemEventArgs e)
		{
   if (InvokeRequired)
   {
    Invoke(new DirectoryNode.ScannedEventHandler(myFileScanned), sender, e);
    return;
   }
			statusBar1.Text = "Scanning: " + e.Name;
		}

		void myDirectoryScanned(object sender, ScannedItemEventArgs e)
		{
   if (InvokeRequired)
   {
    Invoke(new DirectoryNode.ScannedEventHandler(myDirectoryScanned), sender, e);
    return;
   }

			statusBar1.Text = "Scanning: " + e.Name;
		}

		// This function takes a percentage used by a directory or file
		// and returns the appropriate image index to use in in the tree
		// node for this item
		int FractionToIndex(BaseNode baseNode)
		{
			int offset = 2;
			if (baseNode.FlagRed)
			{
				offset += 10;
			}
			int index = offset + (int) (8 * baseNode.FractionUsed);
			return(index);
		}

		delegate int AddDelegate(TreeNode treeNode);

		public void PopulateTreeNode(TreeNodeCollection treeNodeCollection, 
								 DirectoryNode directoryNode)
		{
			TreeNode treeNode = new MyTreeNode(directoryNode.NameSize, directoryNode);
			treeNode.ImageIndex = FractionToIndex(directoryNode);
			treeNode.SelectedImageIndex = treeNode.ImageIndex;
		
				// Adding an item to the list has to be done on the
				// main thread of the control. We can get to it by
				// setting up a delegate that we want to call, 
				// and then calling Invoke() on the treeview control. 
			AddDelegate addDelegate = new AddDelegate(treeNodeCollection.Add);
			// Bug: Invoke should be declared with params. 
			treeView1.Invoke(addDelegate, new object[] {treeNode});

			if (directoryNode.SizeTree != 0)
			{
					// Add a fake entry to this node so that there will be
					// a + sign in front of it.
					// Use invoke to delegate this call to the control.
				addDelegate = new AddDelegate(treeNode.Nodes.Add);
				treeView1.Invoke(addDelegate, new Object[] {new MyTreeNode("", null)});
			}
		}

		public void ExpandTreeNode(MyTreeNode treeNode) 
		{
				// look at the first child of this tree. If the node
				// associated with it isn't null, then we've already
				// done the expansion before.
			MyTreeNode childTreeNode = (MyTreeNode) treeNode.Nodes[0];
			if (childTreeNode.Node != null)
			{
				return;
			}

			treeNode.Nodes.Clear();		// get rid of null entry

			DirectoryNode directoryNode = (DirectoryNode) treeNode.Node;

				// As we walk though the tree, we need to figure out the
				// percentages for each item. We do that based upon the 
				// full size of this directory.
			float dirSize = directoryNode.SizeTree;
			foreach (DirectoryNode subdir in directoryNode.GetDirectories())
			{

				PopulateTreeNode(treeNode.Nodes, subdir);
			}

			foreach (FileNode fileNode in directoryNode.GetFiles())
			{
				TreeNode treeFileNode = new MyTreeNode(fileNode.NameSize, fileNode);
				treeFileNode.ImageIndex = FractionToIndex(fileNode);
				treeFileNode.SelectedImageIndex = treeFileNode.ImageIndex;
				treeNode.Nodes.Add(treeFileNode);
			}
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.FileSave = new System.Windows.Forms.MenuItem();
			this.FileOpen = new System.Windows.Forms.MenuItem();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.Cancel = new System.Windows.Forms.Button();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.imageTreeView = new System.Windows.Forms.ImageList();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.Configure = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem_MRU1 = new System.Windows.Forms.MenuItem();
			this.menuItem_MRU3 = new System.Windows.Forms.MenuItem();
			this.menuItem_MRU2 = new System.Windows.Forms.MenuItem();
			this.menuItem_MRU4 = new System.Windows.Forms.MenuItem();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.treeContextMenu = new System.Windows.Forms.ContextMenu();
			this.FileSave.Index = 1;
			this.FileSave.Text = "&Save";
			this.FileSave.Click += new System.EventHandler(this.FileSave_Click);
			this.FileOpen.Index = 0;
			this.FileOpen.Text = "&Open";
			this.FileOpen.Click += new System.EventHandler(this.FileOpen_Click);
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.menuItem1,
																					  this.menuItem4,
																					  this.menuItem2,
																					  this.menuItem5});
			this.treeView1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.treeView1.ImageList = this.imageTreeView;
			this.treeView1.Size = new System.Drawing.Size(656, 356);
			this.treeView1.TabIndex = 0;
			this.toolTip1.SetToolTip(this.treeView1, "View of files");
			this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
			this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
			this.Cancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.Cancel.Location = new System.Drawing.Point(8, 328);
			this.Cancel.TabIndex = 2;
			this.Cancel.Text = "Cancel";
			this.toolTip1.SetToolTip(this.Cancel, "Select to cancel updating file sizes");
			this.Cancel.Visible = false;
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			this.menuItem9.Index = 3;
			this.menuItem9.Text = "-";
			this.imageTreeView.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageTreeView.ImageSize = new System.Drawing.Size(16, 16);
			this.imageTreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageTreeView.ImageStream")));
			this.imageTreeView.TransparentColor = System.Drawing.Color.Transparent;
			this.menuItem4.Index = 1;
			this.menuItem4.Text = "&Edit";
			this.Configure.Index = 0;
			this.Configure.Text = "&Configure";
			this.Configure.Click += new System.EventHandler(this.Configure_Click);
			this.menuItem6.Index = 0;
			this.menuItem6.Text = "About";
			this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.FileOpen,
																					  this.FileSave,
																					  this.menuItem3,
																					  this.menuItem9,
																					  this.menuItem_MRU1,
																					  this.menuItem_MRU2,
																					  this.menuItem_MRU3,
																					  this.menuItem_MRU4});
			this.menuItem1.Text = "&File";
			this.menuItem2.Index = 2;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.Configure});
			this.menuItem2.Text = "&Options";
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "E&xit";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			this.menuItem5.Index = 3;
			this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.menuItem6});
			this.menuItem5.Text = "Help";
			this.menuItem_MRU1.Index = 4;
			this.menuItem_MRU1.Text = "&1:";
			this.menuItem_MRU1.Click += new System.EventHandler(this.menuItemMRU_Click);
			this.menuItem_MRU3.Index = 6;
			this.menuItem_MRU3.Text = "&3:";
			this.menuItem_MRU3.Click += new System.EventHandler(this.menuItemMRU_Click);
			this.menuItem_MRU2.Index = 5;
			this.menuItem_MRU2.Text = "&2:";
			this.menuItem_MRU2.Click += new System.EventHandler(this.menuItemMRU_Click);
			this.menuItem_MRU4.Index = 7;
			this.menuItem_MRU4.Text = "&4:";
			this.menuItem_MRU4.Click += new System.EventHandler(this.menuItemMRU_Click);
			this.statusBar1.BackColor = System.Drawing.SystemColors.Control;
			this.statusBar1.Location = new System.Drawing.Point(0, 357);
			this.statusBar1.Size = new System.Drawing.Size(656, 20);
			this.statusBar1.TabIndex = 1;
			this.treeContextMenu.Popup += new System.EventHandler(this.treeContextMenu_Popup);
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(656, 377);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {this.Cancel,
																		  this.statusBar1,
																		  this.treeView1});
			this.Menu = this.mainMenu1;
			this.Text = "Disk Diff";

		}

		protected void menuItem6_Click (object sender, System.EventArgs e)
		{
			About about = new About();
			about.ShowDialog();
		}

		protected void menuItemMRU_Click (object sender, System.EventArgs e)
		{
			string numberString = ((MenuItem)sender).Text.Substring(1, 1);
			int number = Convert.ToInt32(numberString);

			string filename = mru[number - 1];
			if (filename != "")
			{
				OpenFile(filename);
			}
		}

		protected void menuItem3_Click (object sender, System.EventArgs e)
		{
			this.Close();
		}

			// bring up context menu. This is a workaround for a bug
			// in the Beta1 release where the node isn't selected 
			// before the context menu comes up.
		protected void treeView1_MouseUp (object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
				if (treeView1.SelectedNode == null)
				{
					Console.WriteLine("null");
				}

				treeContextMenu_Popup(sender, new EventArgs());
				treeContextMenu.Show(this, new Point(e.X, e.Y));
			}
		}

		protected void treeContextMenu_Popup (object sender, System.EventArgs e)
		{
			BaseNode baseNode = ((MyTreeNode)treeView1.SelectedNode).Node;

			treeContextMenu.MenuItems[menuIndexFilename].Text =
				baseNode.Name;
			treeContextMenu.MenuItems[menuIndexDeleteContents].Enabled =
				baseNode.EnableDeleteContents;
			treeContextMenu.MenuItems[menuIndexViewInNotepad].Enabled =
				baseNode.EnableViewInNotepad;
		}

		protected void treeContextMenuClick(object sender, EventArgs e)
		{
			MenuItem menuItem = (MenuItem) sender;
			MyTreeNode treeNode = (MyTreeNode) treeView1.SelectedNode;
			BaseNode baseNode = treeNode.Node;

			bool updateSizes = false;
			switch (menuItem.Index)
			{
				case menuIndexDelete:
					if (baseNode.Delete(true))
					{
						updateSizes = true;
						treeNode.Remove();
					}
					break;
				case menuIndexDeleteContents:
					if (((DirectoryNode)baseNode).DeleteContents())
					{
						foreach (MyTreeNode childNode in treeNode.Nodes)
						{
							childNode.Remove();
						}
						updateSizes = true;
					}
					break;
				case menuIndexViewInNotepad:
					ProcessStartInfo processStartInfo = new ProcessStartInfo();
					processStartInfo.FileName = "notepad";
					processStartInfo.Arguments = baseNode.FullName;
					Process.Start(processStartInfo);
					break;
				case menuIndexLaunch:
					try
					{
						processStartInfo = new ProcessStartInfo();
						processStartInfo.FileName = baseNode.FullName;
						Process.Start(processStartInfo);
					}
					catch (Exception exc)
					{
						MessageBox.Show(exc.ToString());
					}
					break;
			}

			if (updateSizes)
			{
				directoryNode.ClearSizeCache();
				UpdateTreeNodes(this.treeView1.Nodes);
				if (directoryNodeBaseline != null)
				{
					directoryNode.CompareTrees(directoryNodeBaseline);
				}
			}
		}

		void UpdateTreeNodes(TreeNodeCollection nodes)
		{
			if (nodes == null)
				return;

			foreach (MyTreeNode myTreeNode in nodes)
			{
				if (myTreeNode.Node != null)
				{
					//Console.WriteLine("Update: {0}", myTreeNode.Node.NameSize);
					myTreeNode.Text = myTreeNode.Node.NameSize;
					myTreeNode.ImageIndex = FractionToIndex(myTreeNode.Node);
					myTreeNode.SelectedImageIndex = myTreeNode.ImageIndex;
					UpdateTreeNodes(myTreeNode.Nodes);
				}
			}
		}

		void DeleteTreeNode(MyTreeNode nodeToDelete)
		{
			MyTreeNode parentNode = (MyTreeNode) nodeToDelete.Parent;
			
				// Delete the node from this directory, and update
				// the sizes...
			((DirectoryNode)parentNode.Node).DeleteNode(nodeToDelete.Node);

		}

		protected void FileSave_Click (object sender, System.EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "DiskDiff files (*.diskdiff)|*.diskdiff|All files (*.*)|*.*";
			dialog.ShowDialog();

			this.statusBar1.Text = "Saving...";
			directoryNode.Serialize(dialog.FileName);
			this.statusBar1.Text = "";

			directoryNodeBaseline = directoryNode;

			mru.AddEntry(dialog.FileName);
			UpdateMRUList();
		}

		protected void FileOpen_Click (object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "DiskDiff files (*.diskdiff)|*.diskdiff|All files (*.*)|*.*";
			dialog.ShowDialog();

			OpenFile(dialog.FileName);
		}

		void OpenFile(string filename)
		{
			try
			{
					// deserialize the save version, and read in the new version
				this.statusBar1.Text = "Opening...";
				directoryNodeBaseline = DirectoryNode.Deserialize(filename);
				directoryNode = directoryNodeBaseline;
				rootDirectory = directoryNodeBaseline.Root;
				DoTree();
			}
			catch (Exception exception)
			{
				this.statusBar1.Text = "";
				MessageBox.Show(exception.ToString());
			}
		}

		protected void treeView1_BeforeExpand (object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			ExpandTreeNode((MyTreeNode) e.Node);
		}

		protected void Cancel_Click (object sender, System.EventArgs e)
		{
			directoryNode.CancelPopulate();
		}

		protected void Form1_KeyPress (object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Console.WriteLine("Key: {0}", e.KeyChar);
		}

		protected void Configure_Click (object sender, System.EventArgs e)
		{
   FolderBrowserDialog dialog = new FolderBrowserDialog();
   dialog.ShowNewFolderButton = false;
   if (dialog.ShowDialog() == DialogResult.OK)
			{
				directoryNodeBaseline = null;
    rootDirectory = dialog.SelectedPath;
				DoTree();
			}
		}

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
     [STAThread()]
        public static void Main(string[] args) 
        {
            Application.Run(new Form1());
        }
    }


}
