using System;

namespace DiskDiff
{
    /// <summary>
    ///    Summary description for BaseNode
    /// </summary>
    public abstract class BaseNode
    {
		DirectoryNode	parent = null;

		public abstract string Name
		{
			get;
		}
		public abstract string FullName
		{
			get;
		}
		public abstract string NameSize
		{
			get;
		}

		public abstract bool FlagRed
		{
			get;
		}

		public abstract bool EnableDeleteContents
		{
			get;
		}

		public abstract bool EnableViewInNotepad
		{
			get;
		}

		public abstract bool Delete(bool query);

		public DirectoryNode Parent
		{
			get
			{
				return(parent);
			}
			set
			{
				parent = value;
			}
		}

		
		public abstract float FractionUsed
		{
			get;
		}
    }
}
