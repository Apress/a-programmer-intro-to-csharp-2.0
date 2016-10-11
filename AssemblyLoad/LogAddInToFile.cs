// file: LogAddInToFile.cs
// compiler with csc /r:logdriver.dll /target:dll logaddintofile.cs
using System;
using System.Collections;
using System.IO;

public class LogAddInToFile: ILogger
{
	StreamWriter streamWriter;

	public LogAddInToFile()
	{
		streamWriter = File.CreateText(@"logger.log");
		streamWriter.AutoFlush = true;
	}

	public void Log(string message)
	{
		streamWriter.WriteLine(message);
	}
}

