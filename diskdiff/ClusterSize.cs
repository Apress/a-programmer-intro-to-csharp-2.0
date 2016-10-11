namespace DiskDiff
{
    using System;
	using System.Collections;
	using System.Runtime.InteropServices;

    /// <summary>
    ///    Summary description for ClusterSize.
    /// </summary>
    public class ClusterSize
    {
		private ClusterSize() {}

		static Hashtable sizeCache = new Hashtable();

		public static int GetClusterSize(string root)
        {
			string diskName = root.Substring(0, 1) + @":\";
			diskName = diskName.ToUpper();

			object lookup;
			lookup = sizeCache[diskName];

			if (lookup != null)
			{
				return((int) lookup);
			}
			
			int sectorsPerCluster = 0;
			int bytesPerSector = 0;
			int numberOfFreeClusters = 0;
			int totalNumberOfClusters = 0;
			bool result = GetDiskFreeSpace(
					diskName,
					ref sectorsPerCluster,
					ref bytesPerSector,
					ref numberOfFreeClusters,
					ref totalNumberOfClusters);

			//Console.WriteLine("Result: {0}", result);
			//Console.WriteLine("s, b: {0} {1}", sectorsPerCluster, bytesPerSector);

			sizeCache[diskName] = sectorsPerCluster * bytesPerSector;

			return(sectorsPerCluster * bytesPerSector);
        }

		[DllImport("kernel32.dll", SetLastError=true)]
		static extern bool GetDiskFreeSpace(
			string rootPathName,
			ref int sectorsPerCluster,
			ref int bytesPerSector,
			ref int numberOfFreeClusters,
			ref int totalNumberOfClusters);
    }
}
