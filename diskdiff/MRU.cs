namespace DiskDiff
{
 using System;
 using System.Collections;
 using Microsoft.Win32;

 /// <summary>
 ///    Summary description for MRU.
 /// </summary>
 public class MRU
 {
  ArrayList entries = new ArrayList();
  string keyRoot = @"Software\Sample\DiskDiff";

  public MRU()
  {
   RegistryKey ourKey;
   ourKey = Registry.CurrentUser.CreateSubKey(keyRoot);

   for (int index = 0; index < 4; index++)
   {
    string keyName = "MRU_" + index;
    string value = (string)ourKey.GetValue(keyName);
    if (value != null)
    {
     entries.Insert(index, value);
    }
   }
   ourKey.Close();
  }

  public string this[int index]
  {
   get
   {
    if (index >= entries.Count)
     return ("");
    else
     return ((string)entries[index]);
   }
  }

  public void AddEntry(string entry)
  {
   entries.Insert(0, entry);
   if (entries.Count > 4)
   {
    entries.RemoveAt(4);
   }

   Save();
  }

  void Save()
  {
   RegistryKey ourKey;
   ourKey = Registry.CurrentUser.CreateSubKey(keyRoot);

   for (int index = 0; index < entries.Count; index++)
   {
    string keyName = "MRU_" + index;
    ourKey.SetValue(keyName, entries[index]);
   }
   ourKey.Close();
  }
 }
}
