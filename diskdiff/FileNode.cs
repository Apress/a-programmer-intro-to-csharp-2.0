namespace DiskDiff
{
 using System;
 using System.IO;
 using System.Runtime.Serialization;
 using System.Windows.Forms;

 /// <summary>
 ///    Summary description for FileInfo.
 /// </summary>
 [Serializable]
 public class FileNode : BaseNode, IComparable<FileNode>, ISerializable
 {
  FileInfo fileInfo;
  long size = 0;
  long sizeUsed = 0;
  [NonSerialized]
  float ratio = -1.0F;		// ratio of new size to old size

  public FileNode(FileInfo fileInfo)
  {
   this.fileInfo = fileInfo;
   try
   {
    this.sizeUsed = fileInfo.Length;
    long clusterSize = ClusterSize.GetClusterSize(fileInfo.FullName);
    this.size = ((sizeUsed + clusterSize - 1) / clusterSize) * clusterSize;
   }
   catch (Exception)
   {
    //Console.WriteLine("Exception: {0}", e);
   }
  }

  public long Size
  {
   get
   {
    return (size);
   }
  }

  public long SizeUsed
  {
   get
   {
    return (sizeUsed);
   }
  }

  public override float FractionUsed
  {
   get
   {
    return ((float)Size / Parent.SizeTree);
   }
  }


  public override string Name
  {
   get
   {
    return (fileInfo.Name);
   }
  }
  public override string FullName
  {
   get
   {
    return (fileInfo.FullName);
   }
  }
  public override string NameSize
  {
   get
   {
    long sizeTemp = Size;
    if (sizeTemp % 1024 != 0)
    {
     sizeTemp += 1024;
    }
    int ratioInt = (int)ratio * 100;
    string ratioString = "";

    if (ratioInt == 0)
    {
     ratioString = "new";
    }
    else if ((ratioInt != -100) && (ratioInt != 100))
    {
     ratioString = String.Format("{0}%", ratioInt);
    }

    return (String.Format("{0} {1}K {2} {3}",
       fileInfo.Name,
       (long)sizeTemp / 1024,
       (long)sizeUsed / 1024,
       ratioString));
   }
  }

  // property that is true if the file is new or it is 
  // has a ratio > 1.0
  public override bool FlagRed
  {
   get
   {
    return ((Ratio == 0.0f) || (Ratio > 1.0f));
   }
  }

  public float Ratio
  {
   get
   {
    return (ratio);
   }
   set
   {
    ratio = value;
   }
  }

  public override bool EnableDeleteContents
  {
   get
   {
    return (false);
   }
  }

  public override bool EnableViewInNotepad
  {
   get
   {
    return (true);
   }
  }

  public override bool Delete(bool query)
  {
   if (query)
   {
    DialogResult result =
     MessageBox.Show(
      "Delete " + this.fileInfo.FullName + "?",
      "File Delete",
      MessageBoxButtons.OKCancel);

    if (result != DialogResult.OK)
     return false;
   }

   fileInfo.Delete();
   Parent.DeleteNode(this);

   return (true);
  }

  public int CompareTo(FileNode node2)
  {
   if (this.Size < node2.Size)
    return (1);
   else if (this.Size > node2.Size)
    return (-1);
   else
    return (0);
  }

  public bool Equals(FileNode node2)
  {
   if (fileInfo.Name == node2.fileInfo.Name)
   {
    return (true);
   }
   else
   {
    return (false);
   }
  }

  public override bool Equals(object object2)
  {
   return Equals((FileNode)object2);
  }

  public override int GetHashCode()
  {
   return (fileInfo.GetHashCode());
  }

  // Routines to handle serialization
  // The full name and size are the only items that need
  // to be serialized.
  FileNode(SerializationInfo info, StreamingContext content)
  {
   fileInfo = new FileInfo(info.GetString("N"));
   size = info.GetInt64("S");
  }

  public void GetObjectData(SerializationInfo info, StreamingContext content)
  {
   info.AddValue("N", fileInfo.FullName);
   info.AddValue("S", this.size);
  }

 }
}
