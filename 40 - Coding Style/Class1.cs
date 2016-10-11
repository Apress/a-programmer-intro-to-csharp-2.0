using System;
using System.Data.SqlClient;

class SomeRandomClass
{
 private static readonly int MaxValueP1 = 10;

 public void SomeRandomMethod(int p1, string p2)
 {
  if (p1 < 0 || p1 > SomeRandomClass.MaxValueP1) 
  {
   throw new ArgumentOutOfRangeException("p1", p1, "p1 must be a positive number less than " + MaxValueP1.ToString());
  }

  if (p2 == null)
  {
   throw new ArgumentNullException("p2");
  }

  if (p2.Length > p1)
  {
   throw new ArgumentOutOfRangeException("The length of p2 cannot be greater than p1");
  }

  //real method body
 }

 /// <summary>
 /// The main entry point for the application.
 /// </summary>
 [STAThread]
 static void Main(string[] args)
 {
  using (SqlConnection conn1 = new SqlConnection(), conn2 = new SqlConnection())
  {
   //some more code here
   using (SqlCommand cmd = new SqlCommand())
   {
    ;
   }
  }
 }

 }