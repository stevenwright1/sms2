using System;
using System.Collections.Generic;
using System.IO;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;

using NUnit.Framework;

namespace AuthGateway.Shared.Unittest
{
	class DummyLogger : ILogger
	{
		private IList<LogEntry> entries = new List<LogEntry>();
		public void Write(LogEntry message)
		{
			entries.Add(message);
		}
		public IList<LogEntry> getEntries() { return entries; }
	}
	[TestFixture]
	public class TestLogger
	{
		private DummyLogger errorLogger;
		private DummyLogger infoLogger;
		private DummyLogger debugLogger;
		[SetUp]
		public void init()
		{
			errorLogger = new DummyLogger();
			infoLogger = new DummyLogger();
			debugLogger = new DummyLogger();
			Logger.Instance.AddLogger(errorLogger, LogLevel.Error);
			Logger.Instance.AddLogger(infoLogger, LogLevel.Info);
			Logger.Instance.AddLogger(debugLogger, LogLevel.Debug);
		}

		[TearDown]
		public void clean()
		{
			Logger.Instance.EmptyLoggers();
			Logger.Instance.Clear();
		}

		private void writeTestMessages()
		{
			Logger.Instance.WriteToLog("error", LogLevel.Error);
			Logger.Instance.WriteToLog("info", LogLevel.Info);
			Logger.Instance.WriteToLog("debug", LogLevel.Debug);
			Logger.Instance.Flush();
		}

		[Test]
		public void TestErrorLevel()
		{
			clean(); init();
			Logger.Instance.SetLogLevel(LogLevel.Error);
			writeTestMessages();
			Assert.AreEqual(1, errorLogger.getEntries().Count);
			Assert.AreEqual(0, infoLogger.getEntries().Count);
			Assert.AreEqual(0, debugLogger.getEntries().Count);
			Logger.Instance.Clear();
		}

		[Test]
		public void TestInfoLevel()
		{
			clean(); init();
			Logger.Instance.SetLogLevel(LogLevel.Info);
			writeTestMessages();
			Assert.AreEqual(1, errorLogger.getEntries().Count);
			Assert.AreEqual(2, infoLogger.getEntries().Count);
			Assert.AreEqual(0, debugLogger.getEntries().Count);
			Logger.Instance.Clear();
		}

		[Test]
		public void TestDebugLevel()
		{
			clean(); init();
			Logger.Instance.SetLogLevel(LogLevel.Debug);
			writeTestMessages();

			Assert.AreEqual(1, errorLogger.getEntries().Count);
			Assert.AreEqual(2, infoLogger.getEntries().Count);
			Assert.AreEqual(3, debugLogger.getEntries().Count);
			Logger.Instance.Clear();
		}

		[Test]
		public void TestLogByEachLoggerLevelAndNotGlobal()
		{
			clean(); init();
			Logger.Instance.SetLogLevel(LogLevel.All);
			writeTestMessages();
			// If we were logging by the global level, debug messages should've never arrived, this is intended
			Logger.Instance.SetLogLevel(LogLevel.Debug);
			Assert.AreEqual(1, errorLogger.getEntries().Count);
			Assert.AreEqual(2, infoLogger.getEntries().Count);
			Assert.AreEqual(3, debugLogger.getEntries().Count);
			Logger.Instance.Clear();
		}

		[Test]
		public void TestCommon()
		{
			clean();
			var logDir = Path.Combine(Environment.CurrentDirectory);
			var logFilePrefix = "testZeroZero";
			Logger.Instance.AddLogger(new FileLogger(logDir, logFilePrefix, 0, 0), LogLevel.Error);

			string[] files;

			Logger.Instance.WriteToLog("test11", "2012-01-01", LogLevel.Error);
			Logger.Instance.WriteToLog("test12", "2012-01-01", LogLevel.Error);
			files = Directory.GetFiles(logDir, logFilePrefix + "_*", SearchOption.TopDirectoryOnly);
			Assert.AreEqual(1, files.Length);
			Logger.Instance.WriteToLog("test13", "2012-01-01", LogLevel.Error);
			Logger.Instance.WriteToLog("test14", "2012-01-01", LogLevel.Error);
			files = Directory.GetFiles(logDir, logFilePrefix + "_*", SearchOption.TopDirectoryOnly);
			Assert.AreEqual(1, files.Length);
		}

		[Test]
		public void TestLogMaxFiles()
		{
			clean();
			var logDir = Path.Combine(Environment.CurrentDirectory);
			var logFilePrefix = "testMaxFile";
			cleardir(logDir, logFilePrefix);
			Logger.Instance.AddLogger(new FileLogger(logDir, logFilePrefix, 3), LogLevel.Error);

			string[] files;

			Logger.Instance.WriteToLog("test1", "2012-01-01", LogLevel.Error);
			Logger.Instance.WriteToLog("test1", "2012-02-01", LogLevel.Info);
			Logger.Instance.WriteToLog("test1", "2010-03-01", LogLevel.Error);
			files = Directory.GetFiles(logDir, logFilePrefix + "_*", SearchOption.TopDirectoryOnly);
			Assert.AreEqual(2, files.Length);
			Logger.Instance.WriteToLog("test1", "2013-04-01", LogLevel.Error);
			Logger.Instance.WriteToLog("test1", "2014-04-01", LogLevel.Error);
			files = Directory.GetFiles(logDir, logFilePrefix + "_*", SearchOption.TopDirectoryOnly);
			Assert.AreEqual(3, files.Length);
		}
		
		void cleardir(string logDir, string logFilePrefix)
		{
			var files = Directory.GetFiles(logDir, logFilePrefix + "_*", SearchOption.TopDirectoryOnly);
			foreach(var file in files)
				File.Delete(file);
		}

		[Test]
		public void TestLogMaxSize()
		{
			clean();
			var logDir = Path.Combine(Environment.CurrentDirectory);
			var logFilePrefix = "testMaxSize";
			cleardir(logDir, logFilePrefix);
			Logger.Instance.AddLogger(new FileLogger(logDir, logFilePrefix, 0, 4), LogLevel.Error);

			for (var i = 0; i < 10; i++)
			{
				Logger.Instance.WriteToLog(new String('t', 1048576), LogLevel.Error);
			}

			var files = Directory.GetFiles(logDir, logFilePrefix + "_*", SearchOption.TopDirectoryOnly);
			Assert.AreEqual(3, files.Length);
		}

		[Test]
		public void TestLogMaxFilesAndSize()
		{
			clean();
			var logDir = Path.Combine(Environment.CurrentDirectory);
			var logFilePrefix = "testMaxFilesAndSize";
			cleardir(logDir, logFilePrefix);
			Logger.Instance.AddLogger(new FileLogger(logDir, logFilePrefix, 3, 3), LogLevel.Error);

			for (var i = 0; i < 10; i++)
			{
				Logger.Instance.WriteToLog(new String('t', 1048576), LogLevel.Error);
			}

			var files = Directory.GetFiles(logDir, logFilePrefix + "_*", SearchOption.TopDirectoryOnly);
			Assert.AreEqual(3, files.Length);
		}
	}
}
