using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AuthGateway.Shared.Log.Loggers
{
	public class FileLogger : ILogger
	{
		private string logDir = System.IO.Path.GetTempPath();
		private string logFile = "log";
		private int maxLogFiles = 0;
		private int maxLogSize = 0;
		private long maxLogSizeBytes = 0;
		private int currentFileNumber = 0;
			 
		public FileLogger()
		{
		}

		public FileLogger(string logDir, string logFile) : this(logDir, logFile, 0)
		{
		}

		public FileLogger(string logDir, string logFile, int maxLogFiles) : this(logDir, logFile, maxLogFiles, 0)
		{
		}
		public FileLogger(string logDir, string logFile, int maxLogFiles, int maxLogSize)
		{
			
			this.logDir = logDir;
			this.logDir = this.logDir.TrimEnd(Path.DirectorySeparatorChar);
			this.maxLogFiles = maxLogFiles;
			this.maxLogSize = maxLogSize;
			this.maxLogSizeBytes = (maxLogSize * 1048576);
			DirectoryInfo dir = new DirectoryInfo(logDir);

			if (!dir.Exists)
				dir.Create();
			this.logDir += Path.DirectorySeparatorChar;
			this.logFile = logFile;
		}

		public void Write(LogEntry entry)
		{
			string filePrefix = logFile + "_";

			string filePath = logDir + filePrefix + entry.LogDate;

			string fileFullPath = filePath;
			if (currentFileNumber > 0)
				fileFullPath += "_" + currentFileNumber.ToString();

			deleteMaxLogFiles(filePrefix, fileFullPath);

			var nextFile = true;

			while (nextFile)
			{
				nextFile = false;

				using (FileStream fs = File.Open(fileFullPath, FileMode.Append, FileAccess.Write))
				{
					if (this.maxLogSize > 0 && fs.Position > this.maxLogSizeBytes)
					{
						nextFile = true;
						currentFileNumber++;
						fileFullPath = filePath + "_" + currentFileNumber.ToString(); ;
						deleteMaxLogFiles(filePrefix, fileFullPath);
					}
					else 
					{
						using (StreamWriter log = new StreamWriter(fs, Encoding.UTF8))
						{
							log.WriteLine(string.Format("{0}-TID({1}): {2}", entry.LogTime, entry.ThreadId, entry.Message).Trim());
						}
					}
				}
			}
		}

		private void deleteMaxLogFiles(string filePrefix, string fileFullPath)
		{
			if (maxLogFiles > 0 && !File.Exists(fileFullPath))
			{
				var files = Directory.GetFiles(logDir, filePrefix + "*", SearchOption.TopDirectoryOnly);
				if (files.Length > 0)
				{
					Array.Sort<string>(files);
					var index = 0;
					while (files.Length - index >= maxLogFiles)
					{
						try
						{
							File.Delete(files[index]);
						}
						catch { }
						index++;
					}
				}
			}
		}
	}
}
