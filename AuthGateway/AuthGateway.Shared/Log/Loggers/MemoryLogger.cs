using System;
using System.IO;

namespace AuthGateway.Shared.Log.Loggers
{
	public class MemoryLogger : ILogger, IDisposable
	{
		private MemoryStream memoryStream;
		private StreamWriter writer;
		private StreamReader reader;

		public MemoryLogger()
		{
			memoryStream = new MemoryStream();
			writer = new StreamWriter(memoryStream);
			writer.AutoFlush = true;
			reader = new StreamReader(memoryStream);
		}

		public void Write(LogEntry entry)
		{
				writer.WriteLine(string.Format("{0}\t{1}", entry.LogTime, entry.Message).Trim());
		}

		public string GetText()
		{
			memoryStream.Position = 0;
			return reader.ReadToEnd();
		}

		public void Dispose()
		{
			if (writer != null)
				writer.Dispose();
			if (memoryStream != null)
				memoryStream.Dispose();
		}
	}
}
