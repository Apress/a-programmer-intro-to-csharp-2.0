using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;

 
namespace DiskDiff
{

 /// <summary>
 ///    Summary description for Directory.
 /// </summary>
 [Serializable]
 public class DirectoryNode : BaseNode, IComparable<DirectoryNode>, ISerializable
 {
  string root;
  List<FileNode> files = new List<FileNode>();
  List<DirectoryNode> dirs = new List<DirectoryNode>();
  DirectoryInfo directoryInfo;	// this directory

  long? size = null;			// size of dir in bytes
  long? sizeTree = null;		// size of dir and subdirs
  long? sizeTreeUsed = null;	// size in bytes actually used

  [NonSerialized]
  volatile bool cancelled = false;
  [NonSerialized]
  DirectoryNode baselineDirectory = null;

  public delegate void ScannedEventHandler(object sender, ScannedItemEventArgs e);
  //[NonSerialized]
  public event ScannedEventHandler DirectoryScanned;
  //[NonSerialized]
  public event ScannedEventHandler FileScanned;

  public delegate void PopulateDoneEventHandler(object sender, ScanCompleteEventArgs e);
  //[NonSerialized]
  public event PopulateDoneEventHandler PopulateComplete;

  TimeSpan scanUpdateMinimum = new TimeSpan(0, 0, 0, 0, 200);
  DateTime lastUpdateTime;

  public DirectoryNode(string root)
  {
   this.root = root;
   directoryInfo = new DirectoryInfo(root);
  }
  public DirectoryNode(DirectoryInfo directoryInfo)
  {
   this.directoryInfo = directoryInfo;
   this.root = directoryInfo.FullName;
  }
  private DirectoryNode(int size)
  {
   sizeTree = 0;
   size = 0;
  }
  public TimeSpan ScanUpdateMinimum
  {
   get
   {
    return (scanUpdateMinimum);
   }
   set
   {
    scanUpdateMinimum = value;
   }
  }

  public string Root
  {
   get
   {
    return root;
   }
  }

  public override string Name
  {
   get
   {
    return (directoryInfo.Name);
   }
  }
  public override string FullName
  {
   get
   {
    return (directoryInfo.FullName);
   }
  }
  public override string NameSize
  {
   get
   {
    string ratioString = "";
    if (baselineDirectory != null)
    {
     // special case; unmatched dirs have
     // a baseline node with sizeTree set to zero
     if (baselineDirectory.SizeTree == 0)
     {
      ratioString = "new";
     }
     else if (baselineDirectory.SizeTree != SizeTree)
     {
      ratioString = String.Format("{0:f1}%",
          100.0f * SizeTree / baselineDirectory.SizeTree);
     }
    }

    return (String.Format("{0} {1}K {2} {3}",
      directoryInfo.Name, SizeTree / 1024, SizeTreeUsed / 1024, ratioString));
   }
  }


  void UpdateTreeSizes()
  {
   sizeTree = 0;
   sizeTreeUsed = 0;
   foreach (FileNode f in files)
   {
    sizeTree += f.Size;
    sizeTreeUsed += f.SizeUsed;
   }
   foreach (DirectoryNode dirNode in dirs)
   {
    sizeTree += dirNode.SizeTree;
    sizeTreeUsed += dirNode.SizeTreeUsed;
   }
  }

  public long SizeTree
  {
   get
   {
    if (sizeTree == null)
     UpdateTreeSizes();

    return (long)sizeTree;
   }
  }

  public long SizeTreeUsed
  {
   get
   {
    if (sizeTreeUsed == null)
     UpdateTreeSizes();

    return (long)sizeTreeUsed;
   }
  }

  public override float FractionUsed
  {
   get
   {
    if (Parent == null)
    {
     return (1.0f);	// top level, uses 100%
    }
    else
    {
     return ((float)SizeTree / Parent.SizeTree);
    }
   }
  }

  // property that is true if the file is new or it is 
  // has a ratio > 1.0
  public override bool FlagRed
  {
   get
   {
    if (baselineDirectory == null)
     return false;

    return (SizeTree > baselineDirectory.SizeTree);
   }
  }

  public override bool EnableDeleteContents
  {
   get
   {
    return (true);
   }
  }

  public override bool EnableViewInNotepad
  {
   get
   {
    return (false);
   }
  }

  public void CancelPopulate()
  {
   cancelled = true;
  }

  public void ClearSizeCache()
  {
   //Console.WriteLine("Update Size: {0}", direct  ory.FullName);
   size = null;
   sizeTree = null;
   foreach (DirectoryNode dirNode in dirs)
   {
    dirNode.ClearSizeCache();
   }
  }
  public DirectoryNode[] GetDirectories()
  {
   DirectoryNode[] array = dirs.ToArray();
   Array.Sort(array);
   return (array);
  }

  public int CompareTo(DirectoryNode node2)
  {
   if (this.SizeTree < node2.SizeTree)
    return (1);
   else if (this.SizeTree > node2.SizeTree)
    return (-1);
   else
    return (0);
  }

  public bool Equals(DirectoryNode node2)
  {
   return this.CompareTo(node2) == 0;
  }

  public FileNode[] GetFiles()
  {
   FileNode[] array = files.ToArray();
   Array.Sort(array);
   return (array);
  }

  public override bool Equals(object object2)
  {
   DirectoryNode node2 = (DirectoryNode)object2;
   if (this.root == node2.root)
   {
    return (true);
   }
   else
   {
    return (false);
   }
  }

  public override int GetHashCode()
  {
   return (root.GetHashCode());
  }

  public override bool Delete(bool query)
  {
   if (query)
   {
    DialogResult result =
     MessageBox.Show(
      "Delete " + this.directoryInfo.FullName + "?",
      "Directory Delete",
      MessageBoxButtons.OKCancel);

    if (result != DialogResult.OK)
     return false;
   }

   DeleteContents();
   directoryInfo.Delete();
   if (Parent != null)
    Parent.DeleteNode(this);

   return (true);
  }

  // delete the contents of a directory, but leave the
  // directory intact.
  public bool DeleteContents()
  {
   DialogResult result =
    MessageBox.Show(
     "Delete " + this.directoryInfo.FullName + "?",
     "Directory Delete",
     MessageBoxButtons.OKCancel);

   if (result != DialogResult.OK)
    return false;

   // Both directories and files update their parent list
   // if deleted. Therefore, we just delete until there
   // aren't any items left...

   while (files.Count != 0)
   {
    FileNode fileNode = (FileNode)files[0];
    fileNode.Delete(false);
   }
   while (dirs.Count != 0)
   {
    DirectoryNode directoryNode = (DirectoryNode)dirs[0];
    directoryNode.Delete(false);
   }
   return (true);
  }

  public void DeleteNode(BaseNode nodeToDelete)
  {
   if (nodeToDelete is FileNode)
   {
    // locate the node
    for (int index = 0; index < files.Count; index++)
    {
     FileNode fileNode = (FileNode)files[index];
     if ((object)fileNode == (object)nodeToDelete)
     {
      files.RemoveAt(index);
     }
    }
   }
   else if (nodeToDelete is DirectoryNode)
   {
    // locate the node
    for (int index = 0; index < dirs.Count; index++)
    {
     DirectoryNode directoryNode = (DirectoryNode)dirs[index];
     if ((object)directoryNode == (object)nodeToDelete)
     {
      dirs.RemoveAt(index);
     }
    }
   }
  }

  public void Populate()
  {
   cancelled = false;
   Thread t = new Thread(delegate() {
    DoPopulate(this);
    OnPopulateComplete(true);
   });
   t.Start();
  }

  void DoPopulate(DirectoryNode rootDirectoryNode)
  {
   rootDirectoryNode.OnDirectoryScanned(this.directoryInfo.FullName);

   int clusterSize = ClusterSize.GetClusterSize(root);
   try
   {
    foreach (FileInfo f in directoryInfo.GetFiles())
    {
     if (rootDirectoryNode.cancelled)
     {
      rootDirectoryNode.OnPopulateComplete(false);
      Thread.CurrentThread.Abort();
     }
     rootDirectoryNode.OnFileScanned(f.FullName);
     FileNode fileNode = new FileNode(f);
     fileNode.Parent = this;
     this.files.Add(fileNode);
    }

    foreach (DirectoryInfo d in directoryInfo.GetDirectories())
    {
     DirectoryNode dirNode = new DirectoryNode(d.FullName);
     dirNode.Parent = this;
     dirs.Add(dirNode);
     dirNode.DoPopulate(rootDirectoryNode);
    }
   }
   catch (ThreadAbortException)
   {
    //throw e;
   }
   catch (Exception e)
   {
    Console.WriteLine("Exception: {0}", e);
   }
  }

  protected void OnDirectoryScanned(string name)
  {
   if ((DateTime.Now - this.lastUpdateTime) < scanUpdateMinimum)
    return;

   this.lastUpdateTime = DateTime.Now;
   if (DirectoryScanned != null)
   {
    DirectoryScanned(this, new ScannedItemEventArgs(name));
   }
  }

  protected void OnFileScanned(string name)
  {
   if ((DateTime.Now - this.lastUpdateTime) < scanUpdateMinimum)
    return;

   this.lastUpdateTime = DateTime.Now;
   if (FileScanned != null)
   {
    FileScanned(this, new ScannedItemEventArgs(name));
   }
  }

  protected void OnPopulateComplete(bool success)
  {

   if (PopulateComplete != null)
   {
    PopulateComplete(this, new ScanCompleteEventArgs(success));
   }
  }


  // Walk through the directories, and match files with
  // those in the baseline. Save the percentage difference
  // with the object.
  public void CompareTrees(DirectoryNode baseline)
  {
   this.baselineDirectory = baseline;

   //Console.WriteLine("Comparing {0}", this.root);
   // First, compare the files...
   foreach (FileNode fileNode in files)
   {
    fileNode.Ratio = 0;
    FileNode fileNodeBaseline = baseline.FindFile(fileNode);
    if ((fileNodeBaseline != null) &&
     (fileNodeBaseline.Size != 0))
    {
     fileNode.Ratio = fileNode.Size / fileNodeBaseline.Size;
    }
    //Console.WriteLine("Name, Size: {0} {1}", fileNode.NameSize, fileNode.Ratio);
   }

   // Now, compare all the directories...
   foreach (DirectoryNode directoryNode in dirs)
   {
    DirectoryNode directoryNodeBaseline = baseline.FindDirectory(directoryNode);
    if (directoryNodeBaseline != null)
    {
     directoryNode.CompareTrees(directoryNodeBaseline);
    }
    else
    {
     // A bit of a hack. 
     // If there's no baseline directory (ie this
     // directory is new), we create a new directory
     // that has no size to it. 
     directoryNode.baselineDirectory = new DirectoryNode(0);
    }
   }
  }

  FileNode FindFile(FileNode fileNodeSearch)
  {
   foreach (FileNode fileNode in this.files)
   {
    if (fileNode.Equals(fileNodeSearch))
     return (fileNode);
   }
   return (null);
  }

  DirectoryNode FindDirectory(DirectoryNode directoryNodeSearch)
  {
   foreach (DirectoryNode directoryNode in this.dirs)
   {
    if (directoryNode.Equals(directoryNodeSearch))
    {
     return (directoryNode);
    }
   }
   return (null);
  }

  public void Serialize(string filename)
  {
   Stream streamWrite = File.Create(filename);
   //IFormatter writer = new SoapFormatter();
   IFormatter writer = new BinaryFormatter();
   writer.Serialize(streamWrite, this);
   streamWrite.Close();
  }

  public static DirectoryNode Deserialize(string filename)
  {
   Stream streamRead = File.OpenRead(filename);
   //IFormatter reader = new SoapFormatter();
   IFormatter reader = new BinaryFormatter();
   DirectoryNode directoryNode =
     (DirectoryNode)reader.Deserialize(streamRead);
   streamRead.Close();
   return (directoryNode);
  }

  // Routines to handle serialization
  // The full name and size are the only items that need
  // to be serialized.
  DirectoryNode(SerializationInfo info, StreamingContext content)
  {
   root = info.GetString("R");
   directoryInfo = new DirectoryInfo(root);
   files = (List<FileNode>)info.GetValue("F", typeof(List<FileNode>));
   dirs = (List<DirectoryNode>)info.GetValue("D", typeof(List<DirectoryNode>));
   size = info.GetInt64("S");
   sizeTree = info.GetInt64("ST");
  }

  public void GetObjectData(SerializationInfo info, StreamingContext content)
  {
   info.AddValue("R", root);
   info.AddValue("F", files);
   info.AddValue("D", dirs);
   info.AddValue("S", size);
   info.AddValue("ST", sizeTree);
  }

 }
}
