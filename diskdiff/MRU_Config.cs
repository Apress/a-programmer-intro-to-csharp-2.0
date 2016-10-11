using System;
using System.Collections.Generic;
using System.Configuration;
using DiskDiff.Properties;

namespace DiskDiff
{
 class MRU_Config
 {
  List<String> entries = new List<string>();
  Settings settings = new Settings();

  public MRU_Config() 
  {
   entries.AddRange(settings.MRU.Split('|'));
  }
 
  public string this[int index]
  {
   get 
   {
    if (index >= entries.Count)
     return ("");
    else
     return entries[index];
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
   foreach(string s in entries){
    settings.MRU += (s + "|");
   }
   settings.Save();
  }
 }

}
