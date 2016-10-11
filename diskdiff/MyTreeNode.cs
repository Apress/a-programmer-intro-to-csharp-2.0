namespace DiskDiff
{
    using System;
	using System.Windows.Forms;

    /// <summary>
    ///    Summary description for MyTreeNode.
    /// </summary>
    public class MyTreeNode: TreeNode
    {
		BaseNode node;	// DirectoryNode or TreeNode

        public MyTreeNode(string text, BaseNode node): base(text)
        {
			this.node = node;
		}

		public BaseNode Node
		{
			get
			{
				return(node);	
			}
		}
    }
}
