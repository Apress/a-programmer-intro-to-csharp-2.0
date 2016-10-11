namespace DiskDiff
{
    using System;

    /// <summary>
    /// This class encapsulates the information passed when a 
    /// directory or file is scanned.
    /// </summary>
    public class ScannedItemEventArgs: EventArgs
    {
		string name;

        public ScannedItemEventArgs(string name)
        {
			this.name = name;
        }

		public string Name
		{
			get
			{
				return name;
			}
		}
    }
}
