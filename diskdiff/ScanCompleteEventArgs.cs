namespace DiskDiff
{
    using System;

    /// <summary>
    ///    Summary description for ScanCompleteEventArgs.
    /// </summary>
    public class ScanCompleteEventArgs: EventArgs
    {
		bool success;

        public ScanCompleteEventArgs(bool success)
        {
			this.success = success;
        }

		public bool Success
		{
			get
			{
				return(success);
			}
		}
    }
}
