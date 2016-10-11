using System;
using System.Collections;
using System.IO;
using System.Reflection;

public interface ILogger
{
	void Log(string message);
}

public class LogDriver
{
	ArrayList loggers = new ArrayList();

	public LogDriver()
	{
		ScanDirectoryForLoggers();
	}

	void ScanDirectoryForLoggers()
	{
		DirectoryInfo dir = new DirectoryInfo(@".");
		foreach (FileInfo f in dir.GetFiles(@"LogAddIn*.dll"))
		{
			ScanAssemblyForLoggers(f.FullName);
		}
	}

	void ScanAssemblyForLoggers(string filename)
	{
		Console.WriteLine("Loading: {0}", filename);

		Assembly a = Assembly.LoadFrom(filename);

		foreach (Type t in a.GetTypes())
		{
			Console.WriteLine("Type: {0}", t);
			if (t.GetInterface("ILogger") != null)
			{
				Console.WriteLine("ILogger: {0}", t);
				ILogger iLogger = (ILogger) Activator.CreateInstance(t);
				loggers.Add(iLogger);
			}
		}
	}


	public void AddLogger(ILogger logger)
	{
		loggers.Add(logger);
	}

	public void Log(string message) 
	{
		foreach (ILogger logger in loggers)
		{
			logger.Log(message);
		}
	}
}

public class LogConsole: ILogger
{
	public void Log(string message)
	{
		Console.WriteLine(message);
	}
}

