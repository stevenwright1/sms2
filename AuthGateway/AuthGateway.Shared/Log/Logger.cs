using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;

namespace AuthGateway.Shared.Log
{
	[DefaultValue(Error)]
	public enum LogLevel
	{
		Off,
		Error,
		Info,
		Debug,
		DebugVerbose,
		All = DebugVerbose
	}

	public class LoggerInstance
	{
		private LogLevel level;
		private ILogger logger;

		public LoggerInstance(ILogger logger, LogLevel level)
		{
			this.level = level;
			this.logger = logger;
		}

		public LogLevel Level
		{
			get { return this.level; }
		}
		public ILogger Logger
		{
			get { return this.logger; }
		}
	}

	public class Logger : IDisposable
	{
		private static Logger instance = new Logger();
		private object lockobj = new object();
		private object lockLoggerObj = new object();
		private List<LoggerInstance> loggers;
		private Queue<LogEntry> queue;
		private LogLevel currentLoggerMaxLevel = LogLevel.Error;
		private LogLevel logLevel = LogLevel.Error;
		private int maxLogAge = 60 * 5;
		private int queueSize = 20;
		private DateTime LastFlushed = DateTime.Now;
		private bool flushOnWrite = false;

		private Logger()
		{
			loggers = new List<LoggerInstance>();
			queue = new Queue<LogEntry>();
		}

		/// <summary>
		/// Get the Logger instance (singleton)
		/// </summary>
		public static Logger Instance
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Get the Logger instance (singleton)
		/// </summary>
		public static Logger I
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Set the global debug level
		/// </summary>
		public Logger SetLogLevel(LogLevel level)
		{
			this.logLevel = level;
			return this;
		}

		/// <summary>
		/// Gets the global debug level
		/// </summary>
		public LogLevel GetLogLevel()
		{
			return this.logLevel;
		}

		/// <summary>
		/// Set if the logger should flush on every write
		/// </summary>
		public Logger SetFlushOnWrite(bool flush)
		{
			this.flushOnWrite = flush;
			return this;
		}

		/// <summary>
		/// Add a logger
		/// </summary>
		public Logger AddLogger(ILogger logger, LogLevel level = LogLevel.Error)
		{
			lock (lockLoggerObj)
			{
				this.loggers.Add(new LoggerInstance(logger, level));
			}
			if (level > currentLoggerMaxLevel)
				currentLoggerMaxLevel = level;
			return this;
		}

		public void RemoveLogger(ILogger logger)
		{
			lock (lockLoggerObj)
			{
				LoggerInstance toRemove = null;
				foreach (var li in this.loggers)
				{
					if (li.Logger == logger)
					{
						toRemove = li;
						break;
					}
				}
				if (toRemove != null)
					this.loggers.Remove(toRemove);
			}
		}

		/// <summary>
		/// Remove all registered loggers
		/// </summary>
		public Logger EmptyLoggers()
		{
			lock (lockLoggerObj)
			{
				this.loggers.Clear();
			}
			currentLoggerMaxLevel = LogLevel.Error;
			return this;
		}

		/// <summary>
		/// The single instance method that writes to the log file
		/// </summary>
		/// <param name="message">The message to write to the log</param>
		public Logger WriteToLog(string message, LogLevel level)
		{
			// No registered logger with this level
			if (level > currentLoggerMaxLevel) return this;

			lock (lockobj)
			{
				// Create the entry and push to the Queue
				LogEntry logEntry = new LogEntry(message, level);
				queue.Enqueue(logEntry);
				if (queue.Count >= queueSize || DoPeriodicFlush() || level == LogLevel.Error || flushOnWrite)
				{
					FlushLog();
				}
			}
			return this;
		}

		public Logger WriteToLog(Exception ex, IList<string> tags)
		{
			LogEntry logTags = new LogEntry(string.Join(", ", tags), LogLevel.All);
			LogEntry logEntry = new LogEntry("Error: " + ex.Message, LogLevel.All);
			LogEntry logStack = new LogEntry("Stack: " + ex.StackTrace, LogLevel.All);
			lock (lockobj)
			{
				queue.Enqueue(logTags);
				queue.Enqueue(logEntry);
				queue.Enqueue(logStack);
				FlushLog();
			}
			return this;
		}

		public bool ShouldLog(LogLevel level)
		{
			return (level <= currentLoggerMaxLevel);
		}

		/// <summary>
		/// The single instance method that writes to the log file
		/// </summary>
		/// <param name="message">The message to write to the log</param>
		/// <param name="logDate">The date of the log file</param>
		/// <param name="level">The level of the message</param>
		public Logger WriteToLog(string message, string logDate, LogLevel level)
		{
			// No registered logger with this level
			if (level > currentLoggerMaxLevel) return this;

			lock (lockobj)
			{
				// Create the entry and push to the Queue
				LogEntry logEntry = new LogEntry(message, logDate, level);
				queue.Enqueue(logEntry);
				if (queue.Count >= queueSize || DoPeriodicFlush() || level == LogLevel.Error || flushOnWrite)
				{
					FlushLog();
				}
			}
			return this;
		}

		private bool DoPeriodicFlush()
		{
			TimeSpan logAge = DateTime.Now - LastFlushed;
			if (logAge.TotalSeconds >= maxLogAge)
			{
				LastFlushed = DateTime.Now;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Flushes the Queue to the physical log file
		/// </summary>
		private void FlushLog()
		{
			lock (lockLoggerObj)
			{
				while (queue.Count > 0)
				{
					LogEntry entry = queue.Dequeue();
					foreach (LoggerInstance instance in this.loggers)
					{
						if (instance.Level > logLevel || entry.Level > instance.Level) continue;
						try
						{
							instance.Logger.Write(entry);
						}
						catch
						{

						}
					}
				}
			}
		}

		/// <summary>
		/// Clears the log queue
		/// </summary>
		public Logger Clear()
		{
			lock (lockobj)
			{
				queue.Clear();
			}
			return this;
		}

		/// <summary>
		/// Try to flush the queue inmediately
		/// </summary>
		public Logger Flush()
		{
			lock (lockobj)
			{
				FlushLog();
			}
			return this;
		}

		public void Dispose()
		{
			try
			{
				Flush();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				// Some logger may throw an error, we don't want that
			}
		}
	}

	/// <summary>
	/// A Log class to store the message and the Date and Time the log entry was created
	/// </summary>
	public class LogEntry
	{
		public string Message { get; set; }
		public string ThreadId { get; set; }
		public string LogTime { get; set; }
		public string LogDate { get; set; }
		public LogLevel Level { get; set; }

		public LogEntry(string message, LogLevel level = LogLevel.Debug)
			: this(message, DateTime.Now.ToString("yyyy-MM-dd"), level)
		{
		}

		public LogEntry(string message, string logDate, LogLevel level = LogLevel.Debug)
		{
			Message = message;
			ThreadId = Thread.CurrentThread.ManagedThreadId.ToString();
			LogDate = logDate;
			LogTime = DateTime.Now.ToString("HH:mm:ss.fff");
			Level = level;
		}
	}
}