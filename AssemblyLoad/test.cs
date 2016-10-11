using System;

class Test
{
	public static void Main()
	{
		LogDriver logDriver = new LogDriver();

		logDriver.AddLogger(new LogConsole());

		logDriver.Log("Log start: " + DateTime.Now.ToString());

		for (int i = 0; i < 5; i++)
		{
			logDriver.Log("Operation: " + i.ToString());
		}

		logDriver.Log("Log end: " + DateTime.Now.ToString());
	}
}