/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Htc.Vita.External.SQLite
{
  using System;
  using System.Globalization;

#if TRACE_DETECTION || TRACE_SHARED || TRACE_PRELOAD || TRACE_HANDLE
  using System.Diagnostics;
#endif

  using System.Collections.Generic;
  using System.IO;
  using System.Reflection;

#if !PLATFORM_COMPACTFRAMEWORK
  using System.Security;
#endif

  using System.Runtime.InteropServices;

#if (NET_40 || NET_45 || NET_451 || NET_452 || NET_46 || NET_461 || NET_462 || NET_47 || NET_471 || NET_472 || NET_48 || NET_STANDARD_20 || NET_STANDARD_21) && !PLATFORM_COMPACTFRAMEWORK
  using System.Runtime.Versioning;
#endif

  using System.Text;

#if !PLATFORM_COMPACTFRAMEWORK || COUNT_HANDLE
  using System.Threading;
#endif

  using System.Xml;

  #region Debug Data Static Class
#if COUNT_HANDLE || DEBUG
  /// <summary>
  /// This class encapsulates some tracking data that is used for debugging
  /// and testing purposes.
  /// </summary>
  internal static class DebugData
  {
      #region Private Data
#if DEBUG
      /// <summary>
      /// This lock is used to protect several static fields.
      /// </summary>
      private static readonly object staticSyncRoot = new object();
#endif

      /////////////////////////////////////////////////////////////////////////

      #region Critical Handle Counts (Debug Build Only)
#if COUNT_HANDLE
      //
      // NOTE: These counts represent the total number of outstanding
      //       (non-disposed) CriticalHandle derived object instances
      //       created by this library and are primarily for use by
      //       the test suite.  These counts are incremented by the
      //       associated constructors and are decremented upon the
      //       successful completion of the associated ReleaseHandle
      //       methods.
      //
      internal static int connectionCount;
      internal static int statementCount;
      internal static int backupCount;
      internal static int blobCount;
#endif
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Settings Read Counts (Debug Build Only)
#if DEBUG
      /// <summary>
      /// This dictionary stores the read counts for the runtime configuration
      /// settings.  This information is only recorded when compiled in the
      /// "Debug" build configuration.
      /// </summary>
      private static Dictionary<string, int> settingReadCounts;

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// This dictionary stores the read counts for the runtime configuration
      /// settings via the XML configuration file.  This information is only
      /// recorded when compiled in the "Debug" build configuration.
      /// </summary>
      private static Dictionary<string, int> settingFileReadCounts;
#endif
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Other Counts (Debug Build Only)
#if DEBUG
      /// <summary>
      /// This dictionary stores miscellaneous counts used for debugging
      /// purposes.  This information is only recorded when compiled in the
      /// "Debug" build configuration.
      /// </summary>
      private static Dictionary<string, int> otherCounts;
#endif
      #endregion
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Public Methods
#if DEBUG
      /// <summary>
      /// Creates dictionaries used to store the read counts for each of
      /// the runtime configuration settings.  These numbers are used for
      /// debugging and testing purposes only.
      /// </summary>
      public static void Initialize()
      {
          lock (staticSyncRoot)
          {
              //
              // NOTE: Create the dictionaries of statistics that will
              //       contain the number of times each setting value
              //       has been read.
              //
              if (settingReadCounts == null)
                  settingReadCounts = new Dictionary<string, int>();

              if (settingFileReadCounts == null)
                  settingFileReadCounts = new Dictionary<string, int>();

              if (otherCounts == null)
                  otherCounts = new Dictionary<string, int>();
          }
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Queries the read counts for the runtime configuration settings.
      /// These numbers are used for debugging and testing purposes only.
      /// </summary>
      /// <param name="viaFile">
      /// Non-zero if the specified settings were read from the XML
      /// configuration file.
      /// </param>
      /// <returns>
      /// A copy of the statistics for the specified runtime configuration
      /// settings -OR- null if they are not available.
      /// </returns>
      public static object GetSettingReadCounts(
          bool viaFile
          )
      {
          lock (staticSyncRoot)
          {
              if (viaFile)
              {
                  if (settingFileReadCounts == null)
                      return null;

                  return new Dictionary<string, int>(settingFileReadCounts);
              }
              else
              {
                  if (settingReadCounts == null)
                      return null;

                  return new Dictionary<string, int>(settingReadCounts);
              }
          }
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Clears the read counts for the runtime configuration settings.
      /// These numbers are used for debugging and testing purposes only.
      /// </summary>
      /// <param name="viaFile">
      /// Non-zero if the specified settings were read from the XML
      /// configuration file.
      /// </param>
      public static void ClearSettingReadCounts(
          bool viaFile
          )
      {
          lock (staticSyncRoot)
          {
              if (viaFile)
              {
                  if (settingFileReadCounts != null)
                      settingFileReadCounts.Clear();
              }
              else
              {
                  if (settingReadCounts != null)
                      settingReadCounts.Clear();
              }
          }
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Increments the read count for the specified runtime configuration
      /// setting.  These numbers are used for debugging and testing purposes
      /// only.
      /// </summary>
      /// <param name="name">
      /// The name of the setting being read.
      /// </param>
      /// <param name="viaFile">
      /// Non-zero if the specified setting is being read from the XML
      /// configuration file.
      /// </param>
      public static void IncrementSettingReadCount(
          string name,
          bool viaFile
          )
      {
          lock (staticSyncRoot)
          {
              //
              // NOTE: Update statistics for this setting value.
              //
              if (viaFile)
              {
                  if (settingFileReadCounts != null)
                  {
                      int count;

                      if (settingFileReadCounts.TryGetValue(name, out count))
                          settingFileReadCounts[name] = count + 1;
                      else
                          settingFileReadCounts.Add(name, 1);
                  }
              }
              else
              {
                  if (settingReadCounts != null)
                  {
                      int count;

                      if (settingReadCounts.TryGetValue(name, out count))
                          settingReadCounts[name] = count + 1;
                      else
                          settingReadCounts.Add(name, 1);
                  }
              }
          }
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Queries the counters.  These numbers are used for debugging and
      /// testing purposes only.
      /// </summary>
      /// <returns>
      /// A copy of the counters -OR- null if they are not available.
      /// </returns>
      public static object GetOtherCounts()
      {
          lock (staticSyncRoot)
          {
              if (otherCounts == null)
                  return null;

              return new Dictionary<string, int>(otherCounts);
          }
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Clears the counters.  These numbers are used for debugging and
      /// testing purposes only.
      /// </summary>
      public static void ClearOtherCounts()
      {
          lock (staticSyncRoot)
          {
              if (otherCounts != null)
                  otherCounts.Clear();
          }
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Increments the specified counter.
      /// </summary>
      /// <param name="name">
      /// The name of the counter being incremented.
      /// </param>
      public static void IncrementOtherCount(
          string name
          )
      {
          lock (staticSyncRoot)
          {
              if (otherCounts != null)
              {
                  int count;

                  if (otherCounts.TryGetValue(name, out count))
                      otherCounts[name] = count + 1;
                  else
                      otherCounts.Add(name, 1);
              }
          }
      }
#endif
      #endregion
  }
#endif
  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Helper Methods Static Class
  /// <summary>
  /// This static class provides some methods that are shared between the
  /// native library pre-loader and other classes.
  /// </summary>
  internal static class HelperMethods
  {
      #region Private Constants
      private const string DisplayNullObject = "<nullObject>";
      private const string DisplayEmptyString = "<emptyString>";
      private const string DisplayStringFormat = "\"{0}\"";

      /////////////////////////////////////////////////////////////////////////

      private const string DisplayNullArray = "<nullArray>";
      private const string DisplayEmptyArray = "<emptyArray>";

      /////////////////////////////////////////////////////////////////////////

      private const char ArrayOpen = '[';
      private const string ElementSeparator = ", ";
      private const char ArrayClose = ']';

      /////////////////////////////////////////////////////////////////////////

      private static readonly char[] SpaceChars = {
          '\t', '\n', '\r', '\v', '\f', ' '
      };
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Private Data
      /// <summary>
      /// This lock is used to protect the static <see cref="isMono" /> and
      /// <see cref="isDotNetCore" /> fields.
      /// </summary>
      private static readonly object staticSyncRoot = new object();

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This type is only present when running on Mono.
      /// </summary>
      private static readonly string MonoRuntimeType = "Mono.Runtime";

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This type is only present when running on .NET Core.
      /// </summary>
      private static readonly string DotNetCoreLibType = "System.CoreLib";

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Keeps track of whether we are running on Mono.  Initially null, it is
      /// set by the <see cref="IsMono" /> method on its first call.  Later, it
      /// is returned verbatim by the <see cref="IsMono" /> method.
      /// </summary>
      private static bool? isMono = null;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Keeps track of whether we are running on .NET Core.  Initially null,
      /// it is set by the <see cref="IsDotNetCore" /> method on its first
      /// call.  Later, it is returned verbatim by the
      /// <see cref="IsDotNetCore" /> method.
      /// </summary>
      private static bool? isDotNetCore = null;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Keeps track of whether we successfully invoked the
      /// <see cref="Debugger.Break" /> method.  Initially null, it is set by
      /// the <see cref="MaybeBreakIntoDebugger" /> method on its first call.
      /// </summary>
      private static bool? debuggerBreak = null;
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Private Methods
      /// <summary>
      /// Determines the ID of the current process.  Only used for debugging.
      /// </summary>
      /// <returns>
      /// The ID of the current process -OR- zero if it cannot be determined.
      /// </returns>
      private static int GetProcessId()
      {
          Process process = Process.GetCurrentProcess();

          if (process == null)
              return 0;

          return process.Id;
      }

      ///////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Determines whether or not this assembly is running on Mono.
      /// </summary>
      /// <returns>
      /// Non-zero if this assembly is running on Mono.
      /// </returns>
      private static bool IsMono()
      {
          try
          {
              lock (staticSyncRoot)
              {
                  if (isMono == null)
                      isMono = (Type.GetType(MonoRuntimeType) != null);

                  return (bool)isMono;
              }
          }
          catch
          {
              // do nothing.
          }

          return false;
      }

      ///////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Determines whether or not this assembly is running on .NET Core.
      /// </summary>
      /// <returns>
      /// Non-zero if this assembly is running on .NET Core.
      /// </returns>
      public static bool IsDotNetCore()
      {
          try
          {
              lock (staticSyncRoot)
              {
                  if (isDotNetCore == null)
                  {
                      isDotNetCore = (Type.GetType(
                          DotNetCoreLibType) != null);
                  }

                  return (bool)isDotNetCore;
              }
          }
          catch
          {
              // do nothing.
          }

          return false;
      }
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Internal Methods
      /// <summary>
      /// Resets the cached value for the "PreLoadSQLite_BreakIntoDebugger"
      /// configuration setting.
      /// </summary>
      internal static void ResetBreakIntoDebugger()
      {
          lock (staticSyncRoot)
          {
              debuggerBreak = null;
          }
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// If the "PreLoadSQLite_BreakIntoDebugger" configuration setting is
      /// present (e.g. via the environment), give the interactive user an
      /// opportunity to attach a debugger to the current process; otherwise,
      /// do nothing.
      /// </summary>
      internal static void MaybeBreakIntoDebugger()
      {
          lock (staticSyncRoot)
          {
              if (debuggerBreak != null)
                  return;
          }

          if (UnsafeNativeMethods.GetSettingValue(
                "PreLoadSQLite_BreakIntoDebugger", null) != null)
          {
              //
              // NOTE: Attempt to use the Console in order to prompt the
              //       interactive user (if any).  This may fail for any
              //       number of reasons.  Even in those cases, we still
              //       want to issue the actual request to break into the
              //       debugger.
              //
              try
              {
                  Console.WriteLine(StringFormat(
                      CultureInfo.CurrentCulture,
                      "Attach a debugger to process {0} " +
                      "and press any key to continue.",
                      GetProcessId()));

#if PLATFORM_COMPACTFRAMEWORK
                  Console.ReadLine();
#else
                  Console.ReadKey();
#endif
              }
#if !NET_COMPACT_20 && TRACE_SHARED
              catch (Exception e)
#else
              catch (Exception)
#endif
              {
#if !NET_COMPACT_20 && TRACE_SHARED
                  try
                  {
                      Trace.WriteLine(HelperMethods.StringFormat(
                          CultureInfo.CurrentCulture,
                          "Failed to issue debugger prompt, " +
                          "{0} may be unusable: {1}",
                          typeof(Console), e)); /* throw */
                  }
                  catch
                  {
                      // do nothing.
                  }
#endif
              }

              try
              {
                  Debugger.Break();

                  lock (staticSyncRoot)
                  {
                      debuggerBreak = true;
                  }
              }
              catch
              {
                  lock (staticSyncRoot)
                  {
                      debuggerBreak = false;
                  }

                  throw;
              }
          }
          else
          {
              //
              // BUGFIX: There is (almost) no point in checking for the
              //         associated configuration setting repeatedly.
              //         Prevent that here by setting the cached value
              //         to false.
              //
              lock (staticSyncRoot)
              {
                  debuggerBreak = false;
              }
          }
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Determines the ID of the current thread.  Only used for debugging.
      /// </summary>
      /// <returns>
      /// The ID of the current thread -OR- zero if it cannot be determined.
      /// </returns>
      internal static int GetThreadId()
      {
#if !PLATFORM_COMPACTFRAMEWORK
          return AppDomain.GetCurrentThreadId();
#else
          return 0;
#endif
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Determines if the specified flags are present within the flags
      /// associated with the parent connection object.
      /// </summary>
      /// <param name="flags">
      /// The flags associated with the parent connection object.
      /// </param>
      /// <param name="hasFlags">
      /// The flags to check for.
      /// </param>
      /// <returns>
      /// Non-zero if the specified flag or flags were present; otherwise,
      /// zero.
      /// </returns>
      internal static bool HasFlags(
          SQLiteConnectionFlags flags,
          SQLiteConnectionFlags hasFlags
          )
      {
          return ((flags & hasFlags) == hasFlags);
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Determines if preparing a query should be logged.
      /// </summary>
      /// <param name="flags">
      /// The flags associated with the parent connection object.
      /// </param>
      /// <returns>
      /// Non-zero if the query preparation should be logged; otherwise, zero.
      /// </returns>
      internal static bool LogPrepare(
          SQLiteConnectionFlags flags
          )
      {
          return HasFlags(flags, SQLiteConnectionFlags.LogPrepare);
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Determines if pre-parameter binding should be logged.
      /// </summary>
      /// <param name="flags">
      /// The flags associated with the parent connection object.
      /// </param>
      /// <returns>
      /// Non-zero if the pre-parameter binding should be logged; otherwise,
      /// zero.
      /// </returns>
      internal static bool LogPreBind(
          SQLiteConnectionFlags flags
          )
      {
          return HasFlags(flags, SQLiteConnectionFlags.LogPreBind);
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Determines if parameter binding should be logged.
      /// </summary>
      /// <param name="flags">
      /// The flags associated with the parent connection object.
      /// </param>
      /// <returns>
      /// Non-zero if the parameter binding should be logged; otherwise, zero.
      /// </returns>
      internal static bool LogBind(
          SQLiteConnectionFlags flags
          )
      {
          return HasFlags(flags, SQLiteConnectionFlags.LogBind);
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Determines if an exception in a native callback should be logged.
      /// </summary>
      /// <param name="flags">
      /// The flags associated with the parent connection object.
      /// </param>
      /// <returns>
      /// Non-zero if the exception should be logged; otherwise, zero.
      /// </returns>
      internal static bool LogCallbackExceptions(
          SQLiteConnectionFlags flags
          )
      {
          return HasFlags(flags, SQLiteConnectionFlags.LogCallbackException);
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Determines if backup API errors should be logged.
      /// </summary>
      /// <param name="flags">
      /// The flags associated with the parent connection object.
      /// </param>
      /// <returns>
      /// Non-zero if the backup API error should be logged; otherwise, zero.
      /// </returns>
      internal static bool LogBackup(
          SQLiteConnectionFlags flags
          )
      {
          return HasFlags(flags, SQLiteConnectionFlags.LogBackup);
      }

#if INTEROP_VIRTUAL_TABLE
      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Determines if logging for the <see cref="SQLiteModule" /> class is
      /// disabled.
      /// </summary>
      /// <param name="flags">
      /// The flags associated with the parent connection object.
      /// </param>
      /// <returns>
      /// Non-zero if logging for the <see cref="SQLiteModule" /> class is
      /// disabled; otherwise, zero.
      /// </returns>
      internal static bool NoLogModule(
          SQLiteConnectionFlags flags
          )
      {
          return HasFlags(flags, SQLiteConnectionFlags.NoLogModule);
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Determines if <see cref="SQLiteModule" /> errors should be logged.
      /// </summary>
      /// <param name="flags">
      /// The flags associated with the parent connection object.
      /// </param>
      /// <returns>
      /// Non-zero if the <see cref="SQLiteModule" /> error should be logged;
      /// otherwise, zero.
      /// </returns>
      internal static bool LogModuleError(
          SQLiteConnectionFlags flags
          )
      {
          return HasFlags(flags, SQLiteConnectionFlags.LogModuleError);
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Determines if <see cref="SQLiteModule" /> exceptions should be
      /// logged.
      /// </summary>
      /// <param name="flags">
      /// The flags associated with the parent connection object.
      /// </param>
      /// <returns>
      /// Non-zero if the <see cref="SQLiteModule" /> exception should be
      /// logged; otherwise, zero.
      /// </returns>
      internal static bool LogModuleException(
          SQLiteConnectionFlags flags
          )
      {
          return HasFlags(flags, SQLiteConnectionFlags.LogModuleException);
      }
#endif

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Determines if the current process is running on one of the Windows
      /// [sub-]platforms.
      /// </summary>
      /// <returns>
      /// Non-zero when running on Windows; otherwise, zero.
      /// </returns>
      internal static bool IsWindows()
      {
          PlatformID platformId = Environment.OSVersion.Platform;

          if ((platformId == PlatformID.Win32S) ||
              (platformId == PlatformID.Win32Windows) ||
              (platformId == PlatformID.Win32NT) ||
              (platformId == PlatformID.WinCE))
          {
              return true;
          }

          return false;
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This is a wrapper around the
      /// <see cref="String.Format(IFormatProvider,String,Object[])" /> method.
      /// On Mono, it has to call the method overload without the
      /// <see cref="IFormatProvider" /> parameter, due to a bug in Mono.
      /// </summary>
      /// <param name="provider">
      /// This is used for culture-specific formatting.
      /// </param>
      /// <param name="format">
      /// The format string.
      /// </param>
      /// <param name="args">
      /// An array the objects to format.
      /// </param>
      /// <returns>
      /// The resulting string.
      /// </returns>
      internal static string StringFormat(
          IFormatProvider provider,
          string format,
          params object[] args
          )
      {
          if (IsMono())
              return String.Format(format, args);
          else
              return String.Format(provider, format, args);
      }
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Public Methods
      public static string ToDisplayString(
          object value
          )
      {
          if (value == null)
              return DisplayNullObject;

          string stringValue = value.ToString();

          if (stringValue.Length == 0)
              return DisplayEmptyString;

          if (stringValue.IndexOfAny(SpaceChars) < 0)
              return stringValue;

          return HelperMethods.StringFormat(
              CultureInfo.InvariantCulture, DisplayStringFormat,
              stringValue);
      }

      /////////////////////////////////////////////////////////////////////////

      public static string ToDisplayString(
          Array array
          )
      {
          if (array == null)
              return DisplayNullArray;

          if (array.Length == 0)
              return DisplayEmptyArray;

          StringBuilder result = new StringBuilder();

          foreach (object value in array)
          {
              if (result.Length > 0)
                  result.Append(ElementSeparator);

              result.Append(ToDisplayString(value));
          }

          if (result.Length > 0)
          {
#if PLATFORM_COMPACTFRAMEWORK
              result.Insert(0, ArrayOpen.ToString());
#else
              result.Insert(0, ArrayOpen);
#endif

              result.Append(ArrayClose);
          }

          return result.ToString();
      }
      #endregion
  }

  internal static class HelperMethodsMod
  {
      private const string NativeApiVersion = "0.9.0.10";

      private static readonly object FileCheckingLock = new object();
      private static readonly object FileExtractingLock = new object();
      private static readonly Assembly ModuleAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
      private static readonly Dictionary<string, string> Sha1ChecksumWithVersion = new Dictionary<string, string>();

      private static string _nativeLibraryVersion = "Unknown";

      static HelperMethodsMod()
      {
          InitKnownVersion();
      }

      private static void CheckFileProperties(FileInfo fileInfo)
      {
          System.Threading.Tasks.Task.Run(() =>
          {
                  DoCheckFileProperties(fileInfo);
          });
      }

      private static bool CheckPathWritable(string path)
      {
          if (!Directory.Exists(path))
          {
              try
              {
                  Directory.CreateDirectory(path);
              }
              catch (Exception e)
              {
                  Core.Log.Logger.GetInstance(typeof(HelperMethodsMod)).Error($"Can not create \"{path}\", {e.Message}");
                  return false;
              }
          }

          var now = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
          var testFilePath = Path.Combine(path, $"{Core.Crypto.Sha1.GetInstance().GenerateInHex(now)}.tmp");
          try
          {
              File.WriteAllText(testFilePath, now);
              return true;
          }
          catch (Exception e)
          {
              Core.Log.Logger.GetInstance(typeof(HelperMethodsMod)).Error($"Can not write file to \"{path}\", {e.Message}");
          }
          return false;
      }

      private static void DoCheckFileProperties(FileInfo fileInfo)
      {
          if (fileInfo == null || !fileInfo.Exists)
          {
              return;
          }

          lock (FileCheckingLock)
          {
              var checksum = Core.Crypto.Sha1.GetInstance().GenerateInHex(fileInfo);
              _nativeLibraryVersion = GetVersionFrom(checksum);
              Core.Log.Logger.GetInstance(typeof(HelperMethodsMod)).Info($"{fileInfo.FullName}, sha1: {checksum}, version: {_nativeLibraryVersion}");
          }
      }

      private static string GetApplicationDataPath()
      {
          var path = GetWritablePathFromEnvironmentVariable("TEMP")
                  ?? GetWritablePathFromEnvironmentVariable("TMP")
                  ?? GetWritablePathFromEnvironmentVariableLocalAppData()
                  ?? GetWritablePathFromEnvironmentVariableUserProfile();
          if (!string.IsNullOrWhiteSpace(path))
          {
              return Path.Combine(path, "Vita");
          }
          return string.Empty;
      }

      private static string GetBinaryFilePath(string platform, string fileName)
      {
          var path = GetApplicationDataPath();
          if (string.IsNullOrWhiteSpace(path))
          {
              return string.Empty;
          }
          var path2 = Core.Crypto.Sha1.GetInstance().GenerateInHex($"{ModuleAssembly.Location}_{NativeApiVersion}_{platform}_{fileName}");
          return Path.Combine(path, path2, fileName);
      }

      private static string GetVersionFrom(string checksum)
      {
          if (string.IsNullOrWhiteSpace(checksum) || !Sha1ChecksumWithVersion.ContainsKey(checksum))
          {
              return "Unknown";
          }

          return Sha1ChecksumWithVersion[checksum];
      }

      private static string GetWritablePathFromEnvironmentVariable(string key)
      {
          if (string.IsNullOrWhiteSpace(key))
          {
              return null;
          }
          var path = Environment.GetEnvironmentVariable(key);
          if (string.IsNullOrWhiteSpace(path))
          {
              return null;
          }

          return CheckPathWritable(path)
                  ? path
                  : null;
      }

      private static string GetWritablePathFromEnvironmentVariableLocalAppData()
      {
          var path = Environment.GetEnvironmentVariable("LOCALAPPDATA");
          if (string.IsNullOrWhiteSpace(path))
          {
              return null;
          }

          path = Path.Combine(path, "Temp");
          return CheckPathWritable(path)
                  ? path
                  : null;
      }

      private static string GetWritablePathFromEnvironmentVariableUserProfile()
      {
          var path = Environment.GetEnvironmentVariable("USERPROFILE");
          if (string.IsNullOrWhiteSpace(path))
          {
              return null;
          }

          path = Path.Combine(path, "AppData", "Local", "Temp");
          return CheckPathWritable(path)
                  ? path
                  : null;
      }

      private static void InitKnownVersion()
      {
          Sha1ChecksumWithVersion.Add("644862c27ebccdc1ba5998a2c71866894b5ebb9c", "0.9.0.10 ( win32 / x86 )");
          Sha1ChecksumWithVersion.Add("aad752249a9da4b8cb8d12c91c3beccbe0362e38", "0.9.0.10 ( win32 / x64 )");
      }

      private static string PrepareBinary(string resourceName, string platformName, string binaryName)
      {
          if (string.IsNullOrWhiteSpace(binaryName))
          {
              return null;
          }

          var binaryPath = GetBinaryFilePath(platformName, binaryName);
          if (string.IsNullOrWhiteSpace(binaryPath))
          {
              Core.Log.Logger.GetInstance(typeof(HelperMethodsMod)).Error("Can not find binary path to load");
              return null;
          }

          lock (FileExtractingLock)
          {
              if (File.Exists(binaryPath))
              {
                  CheckFileProperties(new FileInfo(binaryPath));
                  return binaryPath;
              }

              var tempBinaryPath = $"{binaryPath}.{Core.Util.Convert.ToTimestampInMilli(DateTime.UtcNow)}";

              Core.Util.Extract.FromAssemblyToFileByResourceName(
                      resourceName,
                      new FileInfo(tempBinaryPath),
                      Core.Util.Extract.CompressionType.Gzip
              );

              if (!File.Exists(binaryPath) && File.Exists(tempBinaryPath))
              {
                  try
                  {
                      File.Move(tempBinaryPath, binaryPath);
                  }
                  catch (Exception e)
                  {
                      Core.Log.Logger.GetInstance(typeof(HelperMethodsMod)).Error($"Can not move file from \"{tempBinaryPath}\". {e}");
                  }
              }

              if (File.Exists(binaryPath))
              {
                  CheckFileProperties(new FileInfo(binaryPath));
                  return binaryPath;
              }
          }

          return null;
      }

      internal static Core.Runtime.Platform.NativeLibInfo PrepareLibrary()
      {
          var is64 = IntPtr.Size == 8;
          if (is64)
          {
              return Core.Runtime.Platform.LoadNativeLib(
                      PrepareBinary(
                              $"{Library.PackagePrefix}.x64.{Library.VitaExternalSqliteApi64}.dll.gz",
                              "x64",
                              $"{Library.VitaExternalSqliteApi64}.dll"
                      ) ?? $"x64/{Library.VitaExternalSqliteApi64}.dll"
              );
          }
          return Core.Runtime.Platform.LoadNativeLib(
                  PrepareBinary(
                          $"{Library.PackagePrefix}.x86.{Library.VitaExternalSqliteApi32}.dll.gz",
                          "x86",
                          $"{Library.VitaExternalSqliteApi32}.dll"
                  ) ?? $"x86/{Library.VitaExternalSqliteApi32}.dll"
          );
      }

      internal static class Library
      {
          internal const string PackagePrefix = "Htc.Vita.External.SQLite";
          internal const string VitaExternalSqliteApi32 = "vita_external_sqlite_api";
          internal const string VitaExternalSqliteApi64 = "vita_external_sqlite_api64";
      }
  }
  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Native Library Helper Class
  /// <summary>
  /// This static class provides a thin wrapper around the native library
  /// loading features of the underlying platform.
  /// </summary>
  internal static class NativeLibraryHelper
  {
      #region Private Delegates
      /// <summary>
      /// This delegate is used to wrap the concept of loading a native
      /// library, based on a file name, and returning the loaded module
      /// handle.
      /// </summary>
      /// <param name="fileName">
      /// The file name of the native library to load.
      /// </param>
      /// <returns>
      /// The native module handle upon success -OR- IntPtr.Zero on failure.
      /// </returns>
      private delegate IntPtr LoadLibraryCallback(
          string fileName
      );

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// This delegate is used to wrap the concept of querying the machine
      /// name of the current process.
      /// </summary>
      /// <returns>
      /// The machine name for the current process -OR- null on failure.
      /// </returns>
      private delegate string GetMachineCallback();
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Private Methods
      /// <summary>
      /// Attempts to load the specified native library file using the Win32
      /// API.
      /// </summary>
      /// <param name="fileName">
      /// The file name of the native library to load.
      /// </param>
      /// <returns>
      /// The native module handle upon success -OR- IntPtr.Zero on failure.
      /// </returns>
      private static IntPtr LoadLibraryWin32(
          string fileName
          )
      {
          return UnsafeNativeMethodsWin32.LoadLibrary(fileName);
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Attempts to determine the machine name of the current process using
      /// the Win32 API.
      /// </summary>
      /// <returns>
      /// The machine name for the current process -OR- null on failure.
      /// </returns>
      private static string GetMachineWin32()
      {
          //
          // NOTE: When running on Windows, attempt to use the native Win32
          //       API function (via P/Invoke) that can provide us with the
          //       processor architecture.
          //
          try
          {
              UnsafeNativeMethodsWin32.SYSTEM_INFO systemInfo;

              //
              // NOTE: Query the system information via P/Invoke, thus
              //       filling the structure.
              //
              UnsafeNativeMethodsWin32.GetSystemInfo(out systemInfo);

              //
              // NOTE: Return the processor architecture value as a string.
              //
              return systemInfo.wProcessorArchitecture.ToString();
          }
          catch
          {
              // do nothing.
          }

          return null;
      }

      /////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
      /// <summary>
      /// Attempts to load the specified native library file using the POSIX
      /// API.
      /// </summary>
      /// <param name="fileName">
      /// The file name of the native library to load.
      /// </param>
      /// <returns>
      /// The native module handle upon success -OR- IntPtr.Zero on failure.
      /// </returns>
      private static IntPtr LoadLibraryPosix(
          string fileName
          )
      {
          return UnsafeNativeMethodsPosix.dlopen(
              fileName, UnsafeNativeMethodsPosix.RTLD_DEFAULT);
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Attempts to determine the machine name of the current process using
      /// the POSIX API.
      /// </summary>
      /// <returns>
      /// The machine name for the current process -OR- null on failure.
      /// </returns>
      private static string GetMachinePosix()
      {
          //
          // NOTE: When running on POSIX (non-Windows), attempt to query the
          //       machine from the operating system via uname().
          //
          try
          {
              UnsafeNativeMethodsPosix.utsname utsName = null;

              if (UnsafeNativeMethodsPosix.GetOsVersionInfo(ref utsName) &&
                  (utsName != null))
              {
                  return utsName.machine;
              }
          }
          catch
          {
              // do nothing.
          }

          return null;
      }
#endif
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Public Methods
      /// <summary>
      /// Attempts to load the specified native library file.
      /// </summary>
      /// <param name="fileName">
      /// The file name of the native library to load.
      /// </param>
      /// <returns>
      /// The native module handle upon success -OR- IntPtr.Zero on failure.
      /// </returns>
      public static IntPtr LoadLibrary(
          string fileName
          )
      {
          LoadLibraryCallback callback = LoadLibraryWin32;

#if !PLATFORM_COMPACTFRAMEWORK
          if (!HelperMethods.IsWindows())
              callback = LoadLibraryPosix;
#endif

          return callback(fileName);
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Attempts to determine the machine name of the current process.
      /// </summary>
      /// <returns>
      /// The machine name for the current process -OR- null on failure.
      /// </returns>
      public static string GetMachine()
      {
          GetMachineCallback callback = GetMachineWin32;

#if !PLATFORM_COMPACTFRAMEWORK
          if (!HelperMethods.IsWindows())
              callback = GetMachinePosix;
#endif

          return callback();
      }
      #endregion
  }
  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Unmanaged Interop Methods Static Class (POSIX)
#if !PLATFORM_COMPACTFRAMEWORK
  /// <summary>
  /// This class declares P/Invoke methods to call native POSIX APIs.
  /// </summary>
  [SuppressUnmanagedCodeSecurity]
  internal static class UnsafeNativeMethodsPosix
  {
      /// <summary>
      /// This structure is used when running on POSIX operating systems
      /// to store information about the current machine, including the
      /// human readable name of the operating system as well as that of
      /// the underlying hardware.
      /// </summary>
      internal sealed class utsname
      {
          public string sysname;  /* Name of this implementation of
                                   * the operating system. */
          public string nodename; /* Name of this node within the
                                   * communications network to which
                                   * this node is attached, if any. */
          public string release;  /* Current release level of this
                                   * implementation. */
          public string version;  /* Current version level of this
                                   * release. */
          public string machine;  /* Name of the hardware type on
                                   * which the system is running. */
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// This structure is passed directly to the P/Invoke method to
      /// obtain the information about the current machine, including
      /// the human readable name of the operating system as well as
      /// that of the underlying hardware.
      /// </summary>
      [StructLayout(LayoutKind.Sequential)]
      private struct utsname_interop
      {
          //
          // NOTE: The following string fields should be present in
          //       this buffer, all of which will be zero-terminated:
          //
          //                      sysname
          //                      nodename
          //                      release
          //                      version
          //                      machine
          //
          [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
          public byte[] buffer;
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// This is the P/Invoke method that wraps the native Unix uname
      /// function.  See the POSIX documentation for full details on what it
      /// does.
      /// </summary>
      /// <param name="name">
      /// Structure containing a preallocated byte buffer to fill with the
      /// requested information.
      /// </param>
      /// <returns>
      /// Zero for success and less than zero upon failure.
      /// </returns>
#if NET_STANDARD_20 || NET_STANDARD_21
      [DllImport("libc",
#else
      [DllImport("__Internal",
#endif
          CallingConvention = CallingConvention.Cdecl)]
      private static extern int uname(out utsname_interop name);

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This is the P/Invoke method that wraps the native Unix dlopen
      /// function.  See the POSIX documentation for full details on what it
      /// does.
      /// </summary>
      /// <param name="fileName">
      /// The name of the executable library.
      /// </param>
      /// <param name="mode">
      /// This must be a combination of the individual bit flags RTLD_LAZY,
      /// RTLD_NOW, RTLD_GLOBAL, and/or RTLD_LOCAL.
      /// </param>
      /// <returns>
      /// The native module handle upon success -OR- IntPtr.Zero on failure.
      /// </returns>
#if NET_STANDARD_20 || NET_STANDARD_21
      [DllImport("libdl",
#else
      [DllImport("__Internal",
#endif
          EntryPoint = "dlopen",
          CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
          BestFitMapping = false, ThrowOnUnmappableChar = true,
          SetLastError = true)]
      internal static extern IntPtr dlopen(string fileName, int mode);

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This is the P/Invoke method that wraps the native Unix dlclose
      /// function.  See the POSIX documentation for full details on what it
      /// does.
      /// </summary>
      /// <param name="module">
      /// The handle to the loaded native library.
      /// </param>
      /// <returns>
      /// Zero upon success -OR- non-zero on failure.
      /// </returns>
#if NET_STANDARD_20 || NET_STANDARD_21
      [DllImport("libdl",
#else
      [DllImport("__Internal",
#endif
          EntryPoint = "dlclose",
          CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
      internal static extern int dlclose(IntPtr module);

      /////////////////////////////////////////////////////////////////////////

      #region Private Constants
      /// <summary>
      /// For use with dlopen(), bind function calls lazily.
      /// </summary>
      internal const int RTLD_LAZY = 0x1;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// For use with dlopen(), bind function calls immediately.
      /// </summary>
      internal const int RTLD_NOW = 0x2;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// For use with dlopen(), make symbols globally available.
      /// </summary>
      internal const int RTLD_GLOBAL = 0x100;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// For use with dlopen(), opposite of RTLD_GLOBAL, and the default.
      /// </summary>
      internal const int RTLD_LOCAL = 0x000;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// For use with dlopen(), the defaults used by this class.
      /// </summary>
      internal const int RTLD_DEFAULT = RTLD_NOW | RTLD_GLOBAL;
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Private Data
      /// <summary>
      /// These are the characters used to separate the string fields within
      /// the raw buffer returned by the <see cref="uname" /> P/Invoke method.
      /// </summary>
      private static readonly char[] utsNameSeparators = { '\0' };
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Private Methods
      /// <summary>
      /// This method is a wrapper around the <see cref="uname" /> P/Invoke
      /// method that extracts and returns the human readable strings from
      /// the raw buffer.
      /// </summary>
      /// <param name="utsName">
      /// This structure, which contains strings, will be filled based on the
      /// data placed in the raw buffer returned by the <see cref="uname" />
      /// P/Invoke method.
      /// </param>
      /// <returns>
      /// Non-zero upon success; otherwise, zero.
      /// </returns>
      internal static bool GetOsVersionInfo(
          ref utsname utsName
          )
      {
          try
          {
              utsname_interop utfNameInterop;

              if (uname(out utfNameInterop) < 0)
                  return false;

              if (utfNameInterop.buffer == null)
                  return false;

              string bufferAsString = Encoding.UTF8.GetString(
                  utfNameInterop.buffer);

              if ((bufferAsString == null) || (utsNameSeparators == null))
                  return false;

              bufferAsString = bufferAsString.Trim(utsNameSeparators);

              string[] parts = bufferAsString.Split(
                  utsNameSeparators, StringSplitOptions.RemoveEmptyEntries);

              if (parts == null)
                  return false;

              utsname localUtsName = new utsname();

              if (parts.Length >= 1)
                  localUtsName.sysname = parts[0];

              if (parts.Length >= 2)
                  localUtsName.nodename = parts[1];

              if (parts.Length >= 3)
                  localUtsName.release = parts[2];

              if (parts.Length >= 4)
                  localUtsName.version = parts[3];

              if (parts.Length >= 5)
                  localUtsName.machine = parts[4];

              utsName = localUtsName;
              return true;
          }
          catch
          {
              // do nothing.
          }

          return false;
      }
      #endregion
  }
#endif
  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Unmanaged Interop Methods Static Class (Win32)
  /// <summary>
  /// This class declares P/Invoke methods to call native Win32 APIs.
  /// </summary>
#if !PLATFORM_COMPACTFRAMEWORK
  [SuppressUnmanagedCodeSecurity]
#endif
  internal static class UnsafeNativeMethodsWin32
  {
      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This is the P/Invoke method that wraps the native Win32 LoadLibrary
      /// function.  See the MSDN documentation for full details on what it
      /// does.
      /// </summary>
      /// <param name="fileName">
      /// The name of the executable library.
      /// </param>
      /// <returns>
      /// The native module handle upon success -OR- IntPtr.Zero on failure.
      /// </returns>
#if !PLATFORM_COMPACTFRAMEWORK
      [DllImport("kernel32",
#else
      [DllImport("coredll",
#endif
          CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto,
#if !PLATFORM_COMPACTFRAMEWORK
          BestFitMapping = false, ThrowOnUnmappableChar = true,
#endif
          SetLastError = true)]
      internal static extern IntPtr LoadLibrary(string fileName);

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// This is the P/Invoke method that wraps the native Win32 GetSystemInfo
      /// function.  See the MSDN documentation for full details on what it
      /// does.
      /// </summary>
      /// <param name="systemInfo">
      /// The system information structure to be filled in by the function.
      /// </param>
#if !PLATFORM_COMPACTFRAMEWORK
      [DllImport("kernel32",
#else
      [DllImport("coredll",
#endif
          CallingConvention = CallingConvention.Winapi)]
      internal static extern void GetSystemInfo(out SYSTEM_INFO systemInfo);

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This enumeration contains the possible values for the processor
      /// architecture field of the system information structure.
      /// </summary>
      internal enum ProcessorArchitecture : ushort /* COMPAT: Win32. */
      {
          Intel = 0,
          MIPS = 1,
          Alpha = 2,
          PowerPC = 3,
          SHx = 4,
          ARM = 5,
          IA64 = 6,
          Alpha64 = 7,
          MSIL = 8,
          AMD64 = 9,
          IA32_on_Win64 = 10,
          Neutral = 11,
          ARM64 = 12,
          Unknown = 0xFFFF
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This structure contains information about the current computer. This
      /// includes the processor type, page size, memory addresses, etc.
      /// </summary>
      [StructLayout(LayoutKind.Sequential)]
      internal struct SYSTEM_INFO
      {
          public ProcessorArchitecture wProcessorArchitecture;
          public ushort wReserved; /* NOT USED */
          public uint dwPageSize; /* NOT USED */
          public IntPtr lpMinimumApplicationAddress; /* NOT USED */
          public IntPtr lpMaximumApplicationAddress; /* NOT USED */
#if PLATFORM_COMPACTFRAMEWORK
          public uint dwActiveProcessorMask; /* NOT USED */
#else
          public IntPtr dwActiveProcessorMask; /* NOT USED */
#endif
          public uint dwNumberOfProcessors; /* NOT USED */
          public uint dwProcessorType; /* NOT USED */
          public uint dwAllocationGranularity; /* NOT USED */
          public ushort wProcessorLevel; /* NOT USED */
          public ushort wProcessorRevision; /* NOT USED */
      }
  }
  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Unmanaged Interop Methods Static Class (SQLite)
  /// <summary>
  /// This class declares P/Invoke methods to call native SQLite APIs.
  /// </summary>
#if !PLATFORM_COMPACTFRAMEWORK
  [SuppressUnmanagedCodeSecurity]
#endif
  internal static class UnsafeNativeMethods
  {
      public const string ExceptionMessageFormat =
          "Caught exception in \"{0}\" method: {1}";

      /////////////////////////////////////////////////////////////////////////

      #region Shared Native SQLite Library Pre-Loading Code
      #region Private Constants
      /// <summary>
      /// The file extension used for dynamic link libraries.
      /// </summary>
      private static readonly string DllFileExtension = ".dll";

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// The primary file extension used for the XML configuration file.
      /// </summary>
      private static readonly string ConfigFileExtension = ".config";

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// The secondary file extension used for the XML configuration file.
      /// </summary>
      private static readonly string AltConfigFileExtension = ".altconfig";

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This is the name of the primary XML configuration file specific
      /// to the System.Data.SQLite assembly.
      /// </summary>
      private static readonly string XmlConfigFileName =
          typeof(UnsafeNativeMethods).Namespace + DllFileExtension +
          ConfigFileExtension;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This is the name of the secondary XML configuration file specific
      /// to the System.Data.SQLite assembly.
      /// </summary>
      private static readonly string XmlAltConfigFileName =
          typeof(UnsafeNativeMethods).Namespace + DllFileExtension +
          AltConfigFileExtension;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This is the XML configuratrion file token that will be replaced with
      /// the qualified path to the directory containing the XML configuration
      /// file.
      /// </summary>
      private static readonly string XmlConfigDirectoryToken =
          "%PreLoadSQLite_XmlConfigDirectory%";
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Private Constants (Desktop Framework Only)
#if !PLATFORM_COMPACTFRAMEWORK
      /// <summary>
      /// This is the environment variable token that will be replaced with
      /// the qualified path to the directory containing this assembly.
      /// </summary>
      private static readonly string AssemblyDirectoryToken =
          "%PreLoadSQLite_AssemblyDirectory%";

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This is the environment variable token that will be replaced with an
      /// abbreviation of the target framework attribute value associated with
      /// this assembly.
      /// </summary>
      private static readonly string TargetFrameworkToken =
          "%PreLoadSQLite_TargetFramework%";
#endif
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Private Data
      /// <summary>
      /// This lock is used to protect the static _SQLiteNativeModuleFileName,
      /// _SQLiteNativeModuleHandle, and processorArchitecturePlatforms fields.
      /// </summary>
      private static readonly object staticSyncRoot = new object();

      /////////////////////////////////////////////////////////////////////////
#if !PLATFORM_COMPACTFRAMEWORK
      /// <summary>
      /// This dictionary stores the mappings between target framework names
      /// and their associated (NuGet) abbreviations.  These mappings are only
      /// used by the <see cref="AbbreviateTargetFramework" /> method.
      /// </summary>
      private static Dictionary<string, string> targetFrameworkAbbreviations;
#endif

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This dictionary stores the mappings between processor architecture
      /// names and platform names.  These mappings are now used for two
      /// purposes.  First, they are used to determine if the assembly code
      /// base should be used instead of the location, based upon whether one
      /// or more of the named sub-directories exist within the assembly code
      /// base.  Second, they are used to assist in loading the appropriate
      /// SQLite interop assembly into the current process.
      /// </summary>
      private static Dictionary<string, string> processorArchitecturePlatforms;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This is the cached return value from the
      /// <see cref="GetAssemblyDirectory" /> method -OR- null if that method
      /// has never returned a valid value.
      /// </summary>
      private static string cachedAssemblyDirectory;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// When this field is non-zero, it indicates the
      /// <see cref="GetAssemblyDirectory" /> method was not able to locate a
      /// suitable assembly directory.  The
      /// <see cref="GetCachedAssemblyDirectory" /> method will check this
      /// field and skips calls into the <see cref="GetAssemblyDirectory" />
      /// method whenever it is non-zero.
      /// </summary>
      private static bool noAssemblyDirectory;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// This is the cached return value from the
      /// <see cref="GetXmlConfigFileName" /> method -OR- null if that method
      /// has never returned a valid value.
      /// </summary>
      private static string cachedXmlConfigFileName;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// When this field is non-zero, it indicates the
      /// <see cref="GetXmlConfigFileName" /> method was not able to locate a
      /// suitable XML configuration file name.  The
      /// <see cref="GetCachedXmlConfigFileName" /> method will check this
      /// field and skips calls into the <see cref="GetXmlConfigFileName" />
      /// method whenever it is non-zero.
      /// </summary>
      private static bool noXmlConfigFileName;
      #endregion

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// For now, this method simply calls the Initialize method.
      /// </summary>
      static UnsafeNativeMethods()
      {
          Initialize();
          HelperMethodsMod.PrepareLibrary();
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Attempts to initialize this class by pre-loading the native SQLite
      /// library for the processor architecture of the current process.
      /// </summary>
      internal static void Initialize()
      {
#if SQLITE_STANDARD || USE_INTEROP_DLL || PLATFORM_COMPACTFRAMEWORK
#if PRELOAD_NATIVE_LIBRARY
          //
          // NOTE: If this method has already fully completed at least once
          //       (and pre-loaded a native library), there is no reason to
          //       continue.
          //
          lock (staticSyncRoot)
          {
              if (_SQLiteNativeModuleHandle != IntPtr.Zero)
                  return;
          }
#endif
#endif

          /////////////////////////////////////////////////////////////////////

          #region Debug Build Only
#if DEBUG
          //
          // NOTE: Create the lists of statistics that will contain
          //       various counts used in debugging, including the
          //       number of times each setting value has been read.
          //
          DebugData.Initialize();
#endif
          #endregion

          /////////////////////////////////////////////////////////////////////

          //
          // NOTE: Check if a debugger needs to be attached before doing any
          //       real work.
          //
          HelperMethods.MaybeBreakIntoDebugger();

#if SQLITE_STANDARD || USE_INTEROP_DLL || PLATFORM_COMPACTFRAMEWORK
#if PRELOAD_NATIVE_LIBRARY
          //
          // NOTE: If the "No_PreLoadSQLite" environment variable is set (to
          //       anything), skip all of our special code and simply return.
          //
          if (GetSettingValue("No_PreLoadSQLite", null) != null)
              return;
#endif
#endif

          /////////////////////////////////////////////////////////////////////

          lock (staticSyncRoot)
          {
#if !PLATFORM_COMPACTFRAMEWORK
              //
              // TODO: Make sure to keep these lists updated when the
              //       target framework names (or their abbreviations)
              //       -OR- the processor architecture names (or their
              //       platform names) change.
              //
              if (targetFrameworkAbbreviations == null)
              {
                  targetFrameworkAbbreviations =
                      new Dictionary<string, string>(
                          StringComparer.OrdinalIgnoreCase);

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v2.0", "net20");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v3.5", "net35");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.0", "net40");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.5", "net45");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.5.1", "net451");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.5.2", "net452");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.6", "net46");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.6.1", "net461");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.6.2", "net462");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.7", "net47");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.7.1", "net471");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.7.2", "net472");

                  targetFrameworkAbbreviations.Add(
                      ".NETFramework,Version=v4.8", "net48");

                  targetFrameworkAbbreviations.Add(
                      ".NETStandard,Version=v2.0", "netstandard2.0");

                  targetFrameworkAbbreviations.Add(
                      ".NETStandard,Version=v2.1", "netstandard2.1");
              }
#endif

              /////////////////////////////////////////////////////////////////

              if (processorArchitecturePlatforms == null)
              {
                  //
                  // NOTE: Create the map of processor architecture names
                  //       to platform names using a case-insensitive string
                  //       comparer.
                  //
                  processorArchitecturePlatforms =
                      new Dictionary<string, string>(
                          StringComparer.OrdinalIgnoreCase);

                  //
                  // NOTE: Setup the list of platform names associated with
                  //       the supported processor architectures.
                  //
                  processorArchitecturePlatforms.Add("x86", "Win32");
                  processorArchitecturePlatforms.Add("x86_64", "x64");
                  processorArchitecturePlatforms.Add("AMD64", "x64");
                  processorArchitecturePlatforms.Add("IA64", "Itanium");
                  processorArchitecturePlatforms.Add("ARM", "WinCE");
              }

              /////////////////////////////////////////////////////////////////

#if SQLITE_STANDARD || USE_INTEROP_DLL || PLATFORM_COMPACTFRAMEWORK
#if PRELOAD_NATIVE_LIBRARY
              //
              // BUGBUG: What about other application domains?
              //
              if (_SQLiteNativeModuleHandle == IntPtr.Zero)
              {
                  string baseDirectory = null;
                  string processorArchitecture = null;
                  bool allowBaseDirectoryOnly = false;

                  /* IGNORED */
                  SearchForDirectory(
                      ref baseDirectory, ref processorArchitecture,
                      ref allowBaseDirectoryOnly);

                  //
                  // NOTE: Attempt to pre-load the SQLite core library (or
                  //       interop assembly) and store both the file name
                  //       and native module handle for later usage.
                  //
                  /* IGNORED */
                  PreLoadSQLiteDll(baseDirectory,
                      processorArchitecture, allowBaseDirectoryOnly,
                      ref _SQLiteNativeModuleFileName,
                      ref _SQLiteNativeModuleHandle);
              }
#endif
#endif
          }
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Combines two path strings.
      /// </summary>
      /// <param name="path1">
      /// The first path -OR- null.
      /// </param>
      /// <param name="path2">
      /// The second path -OR- null.
      /// </param>
      /// <returns>
      /// The combined path string -OR- null if both of the original path
      /// strings are null.
      /// </returns>
      private static string MaybeCombinePath(
          string path1,
          string path2
          )
      {
          if (path1 != null)
          {
              if (path2 != null)
                  return Path.Combine(path1, path2);
              else
                  return path1;
          }
          else
          {
              if (path2 != null)
                  return path2;
              else
                  return null;
          }
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Resets the cached XML configuration file name value, thus forcing the
      /// next call to <see cref="GetCachedXmlConfigFileName" /> method to rely
      /// upon the <see cref="GetXmlConfigFileName" /> method to fetch the
      /// XML configuration file name.
      /// </summary>
      private static void ResetCachedXmlConfigFileName()
      {
          #region Debug Build Only
#if DEBUG
          DebugData.IncrementOtherCount("Method_ResetCachedXmlConfigFileName");
#endif
          #endregion

          lock (staticSyncRoot)
          {
              cachedXmlConfigFileName = null;
              noXmlConfigFileName = false;
          }
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Queries and returns the cached XML configuration file name for the
      /// assembly containing the managed System.Data.SQLite components, if
      /// available.  If the cached XML configuration file name value is not
      /// available, the <see cref="GetXmlConfigFileName" /> method will
      /// be used to obtain the XML configuration file name.
      /// </summary>
      /// <returns>
      /// The XML configuration file name -OR- null if it cannot be determined
      /// or does not exist.
      /// </returns>
      private static string GetCachedXmlConfigFileName()
      {
          #region Debug Build Only
#if DEBUG
          DebugData.IncrementOtherCount("Method_GetCachedXmlConfigFileName");
#endif
          #endregion

          lock (staticSyncRoot)
          {
              if (cachedXmlConfigFileName != null)
                  return cachedXmlConfigFileName;

              if (noXmlConfigFileName)
                  return null;
          }

          return GetXmlConfigFileName();
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Queries and returns the XML configuration file name for the assembly
      /// containing the managed System.Data.SQLite components.
      /// </summary>
      /// <returns>
      /// The XML configuration file name -OR- null if it cannot be determined
      /// or does not exist.
      /// </returns>
      private static string GetXmlConfigFileName()
      {
          #region Debug Build Only
#if DEBUG
          DebugData.IncrementOtherCount("Method_GetXmlConfigFileName");
#endif
          #endregion

          string directory;
          string fileName;

#if !PLATFORM_COMPACTFRAMEWORK
          directory = AppDomain.CurrentDomain.BaseDirectory;
          fileName = MaybeCombinePath(directory, XmlConfigFileName);

          if (File.Exists(fileName))
          {
              lock (staticSyncRoot)
              {
                  cachedXmlConfigFileName = fileName;
              }

              return fileName;
          }

          fileName = MaybeCombinePath(directory, XmlAltConfigFileName);

          if (File.Exists(fileName))
          {
              lock (staticSyncRoot)
              {
                  cachedXmlConfigFileName = fileName;
              }

              return fileName;
          }
#endif

          directory = GetCachedAssemblyDirectory();
          fileName = MaybeCombinePath(directory, XmlConfigFileName);

          if (File.Exists(fileName))
          {
              lock (staticSyncRoot)
              {
                  cachedXmlConfigFileName = fileName;
              }

              return fileName;
          }

          fileName = MaybeCombinePath(directory, XmlAltConfigFileName);

          if (File.Exists(fileName))
          {
              lock (staticSyncRoot)
              {
                  cachedXmlConfigFileName = fileName;
              }

              return fileName;
          }

          lock (staticSyncRoot)
          {
              noXmlConfigFileName = true;
          }

          return null;
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// If necessary, replaces all supported XML configuration file tokens
      /// with their associated values.
      /// </summary>
      /// <param name="fileName">
      /// The name of the XML configuration file being read.
      /// </param>
      /// <param name="value">
      /// A setting value read from the XML configuration file.
      /// </param>
      /// <returns>
      /// The value of the <paramref name="value" /> will all supported XML
      /// configuration file tokens replaced.  No return value is reserved
      /// to indicate an error.  This method cannot fail.
      /// </returns>
      private static string ReplaceXmlConfigFileTokens(
          string fileName,
          string value
          )
      {
          if (!String.IsNullOrEmpty(value))
          {
              if (!String.IsNullOrEmpty(fileName))
              {
                  if (value.IndexOf(XmlConfigDirectoryToken) != -1)
                  {
                      try
                      {
                          string directory = Path.GetDirectoryName(fileName);

                          if (!String.IsNullOrEmpty(directory))
                          {
                              value = value.Replace(
                                  XmlConfigDirectoryToken, directory);
                          }
                      }
#if !NET_COMPACT_20 && TRACE_SHARED
                      catch (Exception e)
#else
                      catch (Exception)
#endif
                      {
#if !NET_COMPACT_20 && TRACE_SHARED
                          try
                          {
                              Trace.WriteLine(HelperMethods.StringFormat(
                                  CultureInfo.CurrentCulture, "Native " +
                                  "library pre-loader failed to replace XML " +
                                  "configuration file \"{0}\" tokens: {1}",
                                  fileName, e)); /* throw */
                          }
                          catch
                          {
                              // do nothing.
                          }
#endif
                      }
                  }
              }
          }

          return value;
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Queries and returns the value of the specified setting, using the
      /// specified XML configuration file.
      /// </summary>
      /// <param name="fileName">
      /// The name of the XML configuration file to read.
      /// </param>
      /// <param name="name">
      /// The name of the setting.
      /// </param>
      /// <param name="default">
      /// The value to be returned if the setting has not been set explicitly
      /// or cannot be determined.
      /// </param>
      /// <param name="expand">
      /// Non-zero to expand any environment variable references contained in
      /// the setting value to be returned.  This has no effect on the .NET
      /// Compact Framework.
      /// </param>
      /// <param name="tokens">
      /// Non-zero to replace any special token references contained in the
      /// setting value to be returned.  This has no effect on the .NET Compact
      /// Framework.
      /// </param>
      /// <returns>
      /// The value of the setting -OR- the default value specified by
      /// <paramref name="default" /> if it has not been set explicitly or
      /// cannot be determined.
      /// </returns>
      private static string GetSettingValueViaXmlConfigFile(
          string fileName, /* in */
          string name,     /* in */
          string @default, /* in */
          bool expand,     /* in */
          bool tokens      /* in */
          )
      {
          try
          {
              if ((fileName == null) || (name == null))
                  return @default;

              XmlDocument document = new XmlDocument();

              document.Load(fileName); /* throw */

              XmlElement element = document.SelectSingleNode(
                  HelperMethods.StringFormat(CultureInfo.InvariantCulture,
                  "/configuration/appSettings/add[@key='{0}']", name)) as
                  XmlElement; /* throw */

              if (element != null)
              {
                  string value = null;

                  if (element.HasAttribute("value"))
                      value = element.GetAttribute("value");

                  if (!String.IsNullOrEmpty(value))
                  {
#if !PLATFORM_COMPACTFRAMEWORK
                      if (expand)
                          value = Environment.ExpandEnvironmentVariables(value);

                      if (tokens)
                          value = ReplaceEnvironmentVariableTokens(value);
#endif

                      if (tokens)
                          value = ReplaceXmlConfigFileTokens(fileName, value);
                  }

                  if (value != null)
                      return value;
              }
          }
#if !NET_COMPACT_20 && TRACE_SHARED
          catch (Exception e)
#else
          catch (Exception)
#endif
          {
#if !NET_COMPACT_20 && TRACE_SHARED
              try
              {
                  Trace.WriteLine(HelperMethods.StringFormat(
                      CultureInfo.CurrentCulture, "Native library " +
                      "pre-loader failed to get setting \"{0}\" value " +
                      "from XML configuration file \"{1}\": {2}", name,
                      fileName, e)); /* throw */
              }
              catch
              {
                  // do nothing.
              }
#endif
          }

          return @default;
      }

      /////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
      /// <summary>
      /// Attempts to determine the target framework attribute value that is
      /// associated with the specified managed assembly, if applicable.
      /// </summary>
      /// <param name="assembly">
      /// The managed assembly to read the target framework attribute value
      /// from.
      /// </param>
      /// <returns>
      /// The value of the target framework attribute value for the specified
      /// managed assembly -OR- null if it cannot be determined.  If this
      /// assembly was compiled with a version of the .NET Framework prior to
      /// version 4.0, the value returned MAY reflect that version of the .NET
      /// Framework instead of the one associated with the specified managed
      /// assembly.
      /// </returns>
      private static string GetAssemblyTargetFramework(
          Assembly assembly
          )
      {
          if (assembly != null)
          {
#if NET_40 || NET_45 || NET_451 || NET_452 || NET_46 || NET_461 || NET_462 || NET_47 || NET_471 || NET_472 || NET_48 || NET_STANDARD_20 || NET_STANDARD_21
              try
              {
                  if (assembly.IsDefined(
                          typeof(TargetFrameworkAttribute), false))
                  {
                      TargetFrameworkAttribute targetFramework =
                          (TargetFrameworkAttribute)
                          assembly.GetCustomAttributes(
                              typeof(TargetFrameworkAttribute), false)[0];

                      return targetFramework.FrameworkName;
                  }
              }
              catch
              {
                  // do nothing.
              }
#elif NET_35
              return ".NETFramework,Version=v3.5";
#elif NET_20
              return ".NETFramework,Version=v2.0";
#endif
          }

          return null;
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// Accepts a long target framework attribute value and makes it into a
      /// much shorter version, suitable for use with NuGet packages.
      /// </summary>
      /// <param name="targetFramework">
      /// The long target framework attribute value to convert.
      /// </param>
      /// <returns>
      /// The short target framework attribute value -OR- null if it cannot
      /// be determined or converted.
      /// </returns>
      private static string AbbreviateTargetFramework(
          string targetFramework
          )
      {
          if (!String.IsNullOrEmpty(targetFramework))
          {
              string abbreviation;

              lock (staticSyncRoot)
              {
                  if (targetFrameworkAbbreviations != null)
                  {
                      if (targetFrameworkAbbreviations.TryGetValue(
                              targetFramework, out abbreviation))
                      {
                          return abbreviation;
                      }
                  }
              }

              //
              // HACK: *LEGACY* Fallback to the old method of
              //       abbreviating target framework names.
              //
              int index = targetFramework.IndexOf(
                  ".NETFramework,Version=v");

              if (index != -1)
              {
                  abbreviation = targetFramework;

                  abbreviation = abbreviation.Replace(
                      ".NETFramework,Version=v", "net");

                  abbreviation = abbreviation.Replace(
                      ".", String.Empty);

                  index = abbreviation.IndexOf(',');

                  if (index != -1)
                      return abbreviation.Substring(0, index);
                  else
                      return abbreviation;
              }
          }

          return targetFramework;
      }

      /////////////////////////////////////////////////////////////////////////

      /// <summary>
      /// If necessary, replaces all supported environment variable tokens
      /// with their associated values.
      /// </summary>
      /// <param name="value">
      /// A setting value read from an environment variable.
      /// </param>
      /// <returns>
      /// The value of the <paramref name="value" /> will all supported
      /// environment variable tokens replaced.  No return value is reserved
      /// to indicate an error.  This method cannot fail.
      /// </returns>
      private static string ReplaceEnvironmentVariableTokens(
          string value
          )
      {
          if (!String.IsNullOrEmpty(value))
          {
              if (value.IndexOf(AssemblyDirectoryToken) != -1)
              {
                  string directory = GetCachedAssemblyDirectory();

                  if (!String.IsNullOrEmpty(directory))
                  {
                      try
                      {
                          value = value.Replace(
                              AssemblyDirectoryToken, directory);
                      }
#if !NET_COMPACT_20 && TRACE_SHARED
                      catch (Exception e)
#else
                      catch (Exception)
#endif
                      {
#if !NET_COMPACT_20 && TRACE_SHARED
                          try
                          {
                              Trace.WriteLine(HelperMethods.StringFormat(
                                  CultureInfo.CurrentCulture, "Native library " +
                                  "pre-loader failed to replace assembly " +
                                  "directory token: {0}", e)); /* throw */
                          }
                          catch
                          {
                              // do nothing.
                          }
#endif
                      }
                  }
              }

              if (value.IndexOf(TargetFrameworkToken) != -1)
              {
                  Assembly assembly = null;

                  try
                  {
                      assembly = Assembly.GetExecutingAssembly();
                  }
#if !NET_COMPACT_20 && TRACE_SHARED
                  catch (Exception e)
#else
                  catch (Exception)
#endif
                  {
#if !NET_COMPACT_20 && TRACE_SHARED
                      try
                      {
                          Trace.WriteLine(HelperMethods.StringFormat(
                              CultureInfo.CurrentCulture, "Native library " +
                              "pre-loader failed to obtain executing " +
                              "assembly: {0}", e)); /* throw */
                      }
                      catch
                      {
                          // do nothing.
                      }
#endif
                  }

                  string targetFramework = AbbreviateTargetFramework(
                      GetAssemblyTargetFramework(assembly));

                  if (!String.IsNullOrEmpty(targetFramework))
                  {
                      try
                      {
                          value = value.Replace(
                              TargetFrameworkToken, targetFramework);
                      }
#if !NET_COMPACT_20 && TRACE_SHARED
                      catch (Exception e)
#else
                      catch (Exception)
#endif
                      {
#if !NET_COMPACT_20 && TRACE_SHARED
                          try
                          {
                              Trace.WriteLine(HelperMethods.StringFormat(
                                  CultureInfo.CurrentCulture, "Native library " +
                                  "pre-loader failed to replace target " +
                                  "framework token: {0}", e)); /* throw */
                          }
                          catch
                          {
                              // do nothing.
                          }
#endif
                      }
                  }
              }
          }

          return value;
      }
#endif

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Queries and returns the value of the specified setting, using the XML
      /// configuration file and/or the environment variables for the current
      /// process and/or the current system, when available.
      /// </summary>
      /// <param name="name">
      /// The name of the setting.
      /// </param>
      /// <param name="default">
      /// The value to be returned if the setting has not been set explicitly
      /// or cannot be determined.
      /// </param>
      /// <returns>
      /// The value of the setting -OR- the default value specified by
      /// <paramref name="default" /> if it has not been set explicitly or
      /// cannot be determined.  By default, all references to existing
      /// environment variables will be expanded to their corresponding values
      /// within the value to be returned unless either the "No_Expand" or
      /// "No_Expand_<paramref name="name" />" environment variable is set [to
      /// anything].
      /// </returns>
      internal static string GetSettingValue(
          string name,    /* in */
          string @default /* in */
          )
      {
#if !PLATFORM_COMPACTFRAMEWORK
          //
          // NOTE: If the special "No_SQLiteGetSettingValue" environment
          //       variable is set [to anything], this method will always
          //       return the default value.
          //
          if (Environment.GetEnvironmentVariable(
                "No_SQLiteGetSettingValue") != null)
          {
              return @default;
          }
#endif

          /////////////////////////////////////////////////////////////////////

          if (name == null)
              return @default;

          /////////////////////////////////////////////////////////////////////

          #region Debug Build Only
#if DEBUG
          //
          // NOTE: We are about to read a setting value from the environment
          //       or possibly from the XML configuration file; create or
          //       increment the appropriate statistic now.
          //
          DebugData.IncrementSettingReadCount(name, false);
#endif
          #endregion

          /////////////////////////////////////////////////////////////////////

          bool expand = true; /* SHARED: Environment -AND- XML config file. */
          bool tokens = true; /* SHARED: Environment -AND- XML config file. */

          /////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
          string value = null;

          if (Environment.GetEnvironmentVariable("No_Expand") != null)
          {
              expand = false;
          }
          else if (Environment.GetEnvironmentVariable(
                  HelperMethods.StringFormat(CultureInfo.InvariantCulture,
                  "No_Expand_{0}", name)) != null)
          {
              expand = false;
          }

          if (Environment.GetEnvironmentVariable("No_Tokens") != null)
          {
              tokens = false;
          }
          else if (Environment.GetEnvironmentVariable(
                  HelperMethods.StringFormat(CultureInfo.InvariantCulture,
                  "No_Tokens_{0}", name)) != null)
          {
              tokens = false;
          }

          value = Environment.GetEnvironmentVariable(name);

          if (!String.IsNullOrEmpty(value))
          {
              if (expand)
                  value = Environment.ExpandEnvironmentVariables(value);

              if (tokens)
                  value = ReplaceEnvironmentVariableTokens(value);
          }

          if (value != null)
              return value;

          //
          // NOTE: If the "No_SQLiteXmlConfigFile" environment variable is
          //       set [to anything], this method will NEVER read from the
          //       XML configuration file.
          //
          if (Environment.GetEnvironmentVariable(
                "No_SQLiteXmlConfigFile") != null)
          {
              return @default;
          }
#endif

          /////////////////////////////////////////////////////////////////////

          #region Debug Build Only
#if DEBUG
          //
          // NOTE: We are about to read a setting value from the XML
          //       configuration file; create or increment the appropriate
          //       statistic now.
          //
          DebugData.IncrementSettingReadCount(name, true);
#endif
          #endregion

          /////////////////////////////////////////////////////////////////////

          return GetSettingValueViaXmlConfigFile(
              GetCachedXmlConfigFileName(), name, @default, expand, tokens);
      }

      /////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
      private static string ListToString(IList<string> list)
      {
          if (list == null)
              return null;

          StringBuilder result = new StringBuilder();

          foreach (string element in list)
          {
              if (element == null)
                  continue;

              if (result.Length > 0)
                  result.Append(' ');

              result.Append(element);
          }

          return result.ToString();
      }

      /////////////////////////////////////////////////////////////////////////

      private static int CheckForArchitecturesAndPlatforms(
          string directory,
          ref List<string> matches
          )
      {
          int result = 0;

          if (matches == null)
              matches = new List<string>();

          lock (staticSyncRoot)
          {
              if (!String.IsNullOrEmpty(directory) &&
                  (processorArchitecturePlatforms != null))
              {
                  foreach (KeyValuePair<string, string> pair
                            in processorArchitecturePlatforms)
                  {
                      if (Directory.Exists(MaybeCombinePath(directory, pair.Key)))
                      {
                          matches.Add(pair.Key);
                          result++;
                      }

                      string value = pair.Value;

                      if (value == null)
                          continue;

                      if (Directory.Exists(MaybeCombinePath(directory, value)))
                      {
                          matches.Add(value);
                          result++;
                      }
                  }
              }
          }

          return result;
      }

      /////////////////////////////////////////////////////////////////////////

      private static bool CheckAssemblyCodeBase(
          Assembly assembly,
          ref string fileName
          )
      {
          try
          {
              if (assembly == null)
                  return false;

              string codeBase = assembly.CodeBase;

              if (String.IsNullOrEmpty(codeBase))
                  return false;

              Uri uri = new Uri(codeBase);
              string localFileName = uri.LocalPath;

              if (!File.Exists(localFileName))
                  return false;

              string directory = Path.GetDirectoryName(
                  localFileName); /* throw */

              string xmlConfigFileName = MaybeCombinePath(
                  directory, XmlConfigFileName);

              if (File.Exists(xmlConfigFileName))
              {
#if !NET_COMPACT_20 && TRACE_DETECTION
                  try
                  {
                      Trace.WriteLine(HelperMethods.StringFormat(
                          CultureInfo.CurrentCulture,
                          "Native library pre-loader found primary XML " +
                          "configuration file via code base for currently " +
                          "executing assembly: \"{0}\"",
                          xmlConfigFileName)); /* throw */
                  }
                  catch
                  {
                      // do nothing.
                  }
#endif

                  fileName = localFileName;
                  return true;
              }

              string xmlAltConfigFileName = MaybeCombinePath(
                  directory, XmlAltConfigFileName);

              if (File.Exists(xmlAltConfigFileName))
              {
#if !NET_COMPACT_20 && TRACE_DETECTION
                  try
                  {
                      Trace.WriteLine(HelperMethods.StringFormat(
                          CultureInfo.CurrentCulture,
                          "Native library pre-loader found secondary XML " +
                          "configuration file via code base for currently " +
                          "executing assembly: \"{0}\"",
                          xmlAltConfigFileName)); /* throw */
                  }
                  catch
                  {
                      // do nothing.
                  }
#endif

                  fileName = localFileName;
                  return true;
              }

              List<string> matches = null;

              if (CheckForArchitecturesAndPlatforms(directory, ref matches) > 0)
              {
#if !NET_COMPACT_20 && TRACE_DETECTION
                  try
                  {
                      Trace.WriteLine(HelperMethods.StringFormat(
                          CultureInfo.CurrentCulture,
                          "Native library pre-loader found native sub-directories " +
                          "via code base for currently executing assembly: \"{0}\"",
                          ListToString(matches))); /* throw */
                  }
                  catch
                  {
                      // do nothing.
                  }
#endif

                  fileName = localFileName;
                  return true;
              }

              return false;
          }
#if !NET_COMPACT_20 && TRACE_SHARED
          catch (Exception e)
#else
          catch (Exception)
#endif
          {
#if !NET_COMPACT_20 && TRACE_SHARED
              try
              {
                  Trace.WriteLine(HelperMethods.StringFormat(
                      CultureInfo.CurrentCulture,
                      "Native library pre-loader failed to check code base " +
                      "for currently executing assembly: {0}", e)); /* throw */
              }
              catch
              {
                  // do nothing.
              }
#endif
          }

          return false;
      }
#endif

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Resets the cached assembly directory value, thus forcing the next
      /// call to <see cref="GetCachedAssemblyDirectory" /> method to rely
      /// upon the <see cref="GetAssemblyDirectory" /> method to fetch the
      /// assembly directory.
      /// </summary>
      private static void ResetCachedAssemblyDirectory()
      {
          #region Debug Build Only
#if DEBUG
          DebugData.IncrementOtherCount("Method_ResetCachedAssemblyDirectory");
#endif
          #endregion

          lock (staticSyncRoot)
          {
              cachedAssemblyDirectory = null;
              noAssemblyDirectory = false;
          }
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Queries and returns the cached directory for the assembly currently
      /// being executed, if available.  If the cached assembly directory value
      /// is not available, the <see cref="GetAssemblyDirectory" /> method will
      /// be used to obtain the assembly directory.
      /// </summary>
      /// <returns>
      /// The directory for the assembly currently being executed -OR- null if
      /// it cannot be determined.
      /// </returns>
      private static string GetCachedAssemblyDirectory()
      {
          #region Debug Build Only
#if DEBUG
          DebugData.IncrementOtherCount("Method_GetCachedAssemblyDirectory");
#endif
          #endregion

          lock (staticSyncRoot)
          {
              if (cachedAssemblyDirectory != null)
                  return cachedAssemblyDirectory;

              if (noAssemblyDirectory)
                  return null;
          }

          return GetAssemblyDirectory();
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Queries and returns the directory for the assembly currently being
      /// executed.
      /// </summary>
      /// <returns>
      /// The directory for the assembly currently being executed -OR- null if
      /// it cannot be determined.
      /// </returns>
      private static string GetAssemblyDirectory()
      {
          #region Debug Build Only
#if DEBUG
          DebugData.IncrementOtherCount("Method_GetAssemblyDirectory");
#endif
          #endregion

          try
          {
              Assembly assembly = Assembly.GetExecutingAssembly();

              if (assembly == null)
              {
                  lock (staticSyncRoot)
                  {
                      noAssemblyDirectory = true;
                  }

                  return null;
              }

              string fileName = null;

#if PLATFORM_COMPACTFRAMEWORK
              AssemblyName assemblyName = assembly.GetName();

              if (assemblyName == null)
              {
                  lock (staticSyncRoot)
                  {
                      noAssemblyDirectory = true;
                  }

                  return null;
              }

              fileName = assemblyName.CodeBase;
#else
              if (!CheckAssemblyCodeBase(assembly, ref fileName))
                  fileName = assembly.Location;
#endif

              if (String.IsNullOrEmpty(fileName))
              {
                  lock (staticSyncRoot)
                  {
                      noAssemblyDirectory = true;
                  }

                  return null;
              }

              string directory = Path.GetDirectoryName(fileName);

              if (String.IsNullOrEmpty(directory))
              {
                  lock (staticSyncRoot)
                  {
                      noAssemblyDirectory = true;
                  }

                  return null;
              }

              lock (staticSyncRoot)
              {
                  cachedAssemblyDirectory = directory;
              }

              return directory;
          }
#if !NET_COMPACT_20 && TRACE_SHARED
          catch (Exception e)
#else
          catch (Exception)
#endif
          {
#if !NET_COMPACT_20 && TRACE_SHARED
              try
              {
                  Trace.WriteLine(HelperMethods.StringFormat(
                      CultureInfo.CurrentCulture,
                      "Native library pre-loader failed to get directory " +
                      "for currently executing assembly: {0}", e)); /* throw */
              }
              catch
              {
                  // do nothing.
              }
#endif
          }

          lock (staticSyncRoot)
          {
              noAssemblyDirectory = true;
          }

          return null;
      }
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Native SQLite Library Helper Methods
      /// <summary>
      /// Determines the (possibly fully qualified) file name for the native
      /// SQLite library that was loaded by this class.
      /// </summary>
      /// <returns>
      /// The file name for the native SQLite library that was loaded by
      /// this class -OR- null if its value cannot be determined.
      /// </returns>
      internal static string GetNativeModuleFileName()
      {
#if SQLITE_STANDARD || USE_INTEROP_DLL || PLATFORM_COMPACTFRAMEWORK
#if PRELOAD_NATIVE_LIBRARY
          lock (staticSyncRoot)
          {
              if (_SQLiteNativeModuleFileName != null)
                  return _SQLiteNativeModuleFileName;
          }
#endif
#endif

#if USE_HTC_INTEROP_DLL
          if (IntPtr.Size == 8)
          {
              return SQLITE_DLL64;
          }
          return SQLITE_DLL32;
#else
          return SQLITE_DLL; /* COMPAT */
#endif
      }
      #endregion

      /////////////////////////////////////////////////////////////////////////

      #region Optional Native SQLite Library Pre-Loading Code
      //
      // NOTE: If we are looking for the standard SQLite DLL ("sqlite3.dll"),
      //       the interop DLL ("SQLite.Interop.dll"), or we are running on the
      //       .NET Compact Framework, we should include this code (only if the
      //       feature has actually been enabled).  This code would be totally
      //       redundant if this module has been bundled into the mixed-mode
      //       assembly.
      //
#if SQLITE_STANDARD || USE_INTEROP_DLL || PLATFORM_COMPACTFRAMEWORK

      //
      // NOTE: Only compile in the native library pre-load code if the feature
      //       has been enabled for this build.
      //
#if PRELOAD_NATIVE_LIBRARY
      /// <summary>
      /// The name of the environment variable containing the processor
      /// architecture of the current process.
      /// </summary>
      private static readonly string PROCESSOR_ARCHITECTURE =
          "PROCESSOR_ARCHITECTURE";

      /////////////////////////////////////////////////////////////////////////

      #region Private Data
      /// <summary>
      /// The native module file name for the native SQLite library or null.
      /// </summary>
      private static string _SQLiteNativeModuleFileName = null;

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// The native module handle for the native SQLite library or the value
      /// IntPtr.Zero.
      /// </summary>
      private static IntPtr _SQLiteNativeModuleHandle = IntPtr.Zero;
      #endregion

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Determines the base file name (without any directory information)
      /// for the native SQLite library to be pre-loaded by this class.
      /// </summary>
      /// <returns>
      /// The base file name for the native SQLite library to be pre-loaded by
      /// this class -OR- null if its value cannot be determined.
      /// </returns>
      internal static string GetNativeLibraryFileNameOnly()
      {
          string fileNameOnly = GetSettingValue(
              "PreLoadSQLite_LibraryFileNameOnly", null);

          if (fileNameOnly != null)
              return fileNameOnly;

#if USE_HTC_INTEROP_DLL
          if (IntPtr.Size == 8)
          {
              return SQLITE_DLL64;
          }
          return SQLITE_DLL32;
#else
          return SQLITE_DLL; /* COMPAT */
#endif
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Searches for the native SQLite library in the directory containing
      /// the assembly currently being executed as well as the base directory
      /// for the current application domain.
      /// </summary>
      /// <param name="baseDirectory">
      /// Upon success, this parameter will be modified to refer to the base
      /// directory containing the native SQLite library.
      /// </param>
      /// <param name="processorArchitecture">
      /// Upon success, this parameter will be modified to refer to the name
      /// of the immediate directory (i.e. the offset from the base directory)
      /// containing the native SQLite library.
      /// </param>
      /// <param name="allowBaseDirectoryOnly">
      /// Upon success, this parameter will be modified to non-zero only if
      /// the base directory itself should be allowed for loading the native
      /// library.
      /// </param>
      /// <returns>
      /// Non-zero (success) if the native SQLite library was found; otherwise,
      /// zero (failure).
      /// </returns>
      private static bool SearchForDirectory(
          ref string baseDirectory,         /* out */
          ref string processorArchitecture, /* out */
          ref bool allowBaseDirectoryOnly   /* out */
          )
      {
          if (GetSettingValue(
                "PreLoadSQLite_NoSearchForDirectory", null) != null)
          {
              return false; /* DISABLED */
          }

          //
          // NOTE: Determine the base file name for the native SQLite library.
          //       If this is not known by this class, we cannot continue.
          //
          string fileNameOnly = GetNativeLibraryFileNameOnly();

          if (fileNameOnly == null)
              return false;

          //
          // NOTE: Build the list of base directories and processor/platform
          //       names.  These lists will be used to help locate the native
          //       SQLite core library (or interop assembly) to pre-load into
          //       this process.
          //
          string[] directories = {
              GetAssemblyDirectory(),
#if !PLATFORM_COMPACTFRAMEWORK
              AppDomain.CurrentDomain.BaseDirectory,
#endif
          };

          string extraSubDirectory = null;

          if ((GetSettingValue(
                  "PreLoadSQLite_AllowBaseDirectoryOnly", null) != null) ||
              (HelperMethods.IsDotNetCore() && !HelperMethods.IsWindows()))
          {
              extraSubDirectory = String.Empty; /* .NET Core on POSIX */
          }

          string[] subDirectories = {
              GetProcessorArchitecture(), /* e.g. "x86" */
              GetPlatformName(null),      /* e.g. "Win32" */
              extraSubDirectory           /* base directory only? */
          };

          foreach (string directory in directories)
          {
              if (directory == null)
                  continue;

              foreach (string subDirectory in subDirectories)
              {
                  if (subDirectory == null)
                      continue;

                  string fileName = FixUpDllFileName(MaybeCombinePath(
                      MaybeCombinePath(directory, subDirectory),
                      fileNameOnly));

                  //
                  // NOTE: If the SQLite DLL file exists, return success.
                  //       Prior to returning, set the base directory and
                  //       processor architecture to reflect the location
                  //       where it was found.
                  //
                  if (File.Exists(fileName))
                  {
#if !NET_COMPACT_20 && TRACE_DETECTION
                      try
                      {
                          Trace.WriteLine(HelperMethods.StringFormat(
                              CultureInfo.CurrentCulture,
                              "Native library pre-loader found native file " +
                              "name \"{0}\", returning directory \"{1}\" and " +
                              "sub-directory \"{2}\"...", fileName, directory,
                              subDirectory)); /* throw */
                      }
                      catch
                      {
                          // do nothing.
                      }
#endif

                      baseDirectory = directory;
                      processorArchitecture = subDirectory;
                      allowBaseDirectoryOnly = (subDirectory.Length == 0);

                      return true; /* FOUND */
                  }
              }
          }

          return false; /* NOT FOUND */
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Queries and returns the base directory of the current application
      /// domain.
      /// </summary>
      /// <returns>
      /// The base directory for the current application domain -OR- null if it
      /// cannot be determined.
      /// </returns>
      private static string GetBaseDirectory()
      {
          //
          // NOTE: If the "PreLoadSQLite_BaseDirectory" environment variable
          //       is set, use it verbatim for the base directory.
          //
          string directory = GetSettingValue("PreLoadSQLite_BaseDirectory",
              null);

          if (directory != null)
              return directory;

#if !PLATFORM_COMPACTFRAMEWORK
          //
          // NOTE: If the "PreLoadSQLite_UseAssemblyDirectory" environment
          //       variable is set (to anything), then attempt to use the
          //       directory containing the currently executing assembly
          //       (i.e. System.Data.SQLite) intsead of the application
          //       domain base directory.
          //
          if (GetSettingValue(
                  "PreLoadSQLite_UseAssemblyDirectory", null) != null)
          {
              directory = GetAssemblyDirectory();

              if (directory != null)
                  return directory;
          }

          //
          // NOTE: Otherwise, fallback on using the base directory of the
          //       current application domain.
          //
          return AppDomain.CurrentDomain.BaseDirectory;
#else
          //
          // NOTE: Otherwise, fallback on using the directory containing
          //       the currently executing assembly.
          //
          return GetAssemblyDirectory();
#endif
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Determines if the dynamic link library file name requires a suffix
      /// and adds it if necessary.
      /// </summary>
      /// <param name="fileName">
      /// The original dynamic link library file name to inspect.
      /// </param>
      /// <returns>
      /// The dynamic link library file name, possibly modified to include an
      /// extension.
      /// </returns>
      private static string FixUpDllFileName(
          string fileName /* in */
          )
      {
          if (!String.IsNullOrEmpty(fileName))
          {
              if (HelperMethods.IsWindows())
              {
                  if (!fileName.EndsWith(DllFileExtension,
                          StringComparison.OrdinalIgnoreCase))
                  {
                      return fileName + DllFileExtension;
                  }
              }
          }

          return fileName;
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Queries and returns the processor architecture of the current
      /// process.
      /// </summary>
      /// <returns>
      /// The processor architecture of the current process -OR- null if it
      /// cannot be determined.
      /// </returns>
      private static string GetProcessorArchitecture()
      {
          //
          // NOTE: If the "PreLoadSQLite_ProcessorArchitecture" environment
          //       variable is set, use it verbatim for the current processor
          //       architecture.
          //
          string processorArchitecture = GetSettingValue(
              "PreLoadSQLite_ProcessorArchitecture", null);

          if (processorArchitecture != null)
              return processorArchitecture;

          //
          // BUGBUG: Will this always be reliable?
          //
          processorArchitecture = GetSettingValue(PROCESSOR_ARCHITECTURE, null);

          /////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
          //
          // HACK: Check for an "impossible" situation.  If the pointer size
          //       is 32-bits, the processor architecture cannot be "AMD64".
          //       In that case, we are almost certainly hitting a bug in the
          //       operating system and/or Visual Studio that causes the
          //       PROCESSOR_ARCHITECTURE environment variable to contain the
          //       wrong value in some circumstances.  Please refer to ticket
          //       [9ac9862611] for further information.
          //
          if ((IntPtr.Size == sizeof(int)) &&
              String.Equals(processorArchitecture, "AMD64",
                  StringComparison.OrdinalIgnoreCase))
          {
#if !NET_COMPACT_20 && TRACE_DETECTION
              //
              // NOTE: When tracing is enabled, save the originally detected
              //       processor architecture before changing it.
              //
              string savedProcessorArchitecture = processorArchitecture;
#endif

              //
              // NOTE: We know that operating systems that return "AMD64" as
              //       the processor architecture are actually a superset of
              //       the "x86" processor architecture; therefore, return
              //       "x86" when the pointer size is 32-bits.
              //
              processorArchitecture = "x86";

#if !NET_COMPACT_20 && TRACE_DETECTION
              try
              {
                  //
                  // NOTE: Show that we hit a fairly unusual situation (i.e.
                  //       the "wrong" processor architecture was detected).
                  //
                  Trace.WriteLine(HelperMethods.StringFormat(
                      CultureInfo.CurrentCulture,
                      "Native library pre-loader detected {0}-bit pointer " +
                      "size with processor architecture \"{1}\", using " +
                      "processor architecture \"{2}\" instead...",
                      IntPtr.Size * 8 /* bits */, savedProcessorArchitecture,
                      processorArchitecture)); /* throw */
              }
              catch
              {
                  // do nothing.
              }
#endif
          }
#endif

          /////////////////////////////////////////////////////////////////////

          if (processorArchitecture == null)
          {
              //
              // NOTE: Default to the processor architecture reported by the
              //       appropriate native operating system API, if any.
              //
              processorArchitecture = NativeLibraryHelper.GetMachine();

              //
              // NOTE: Upon failure, return empty string.  This will prevent
              //       the calling method from considering this method call
              //       a "failure".
              //
              if (processorArchitecture == null)
                  processorArchitecture = String.Empty;
          }

          /////////////////////////////////////////////////////////////////////

          return processorArchitecture;
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Given the processor architecture, returns the name of the platform.
      /// </summary>
      /// <param name="processorArchitecture">
      /// The processor architecture to be translated to a platform name.
      /// </param>
      /// <returns>
      /// The platform name for the specified processor architecture -OR- null
      /// if it cannot be determined.
      /// </returns>
      private static string GetPlatformName(
          string processorArchitecture /* in */
          )
      {
          if (processorArchitecture == null)
              processorArchitecture = GetProcessorArchitecture();

          if (String.IsNullOrEmpty(processorArchitecture))
              return null;

          lock (staticSyncRoot)
          {
              if (processorArchitecturePlatforms == null)
                  return null;

              string platformName;

              if (processorArchitecturePlatforms.TryGetValue(
                      processorArchitecture, out platformName))
              {
                  return platformName;
              }
          }

          return null;
      }

      /////////////////////////////////////////////////////////////////////////
      /// <summary>
      /// Attempts to load the native SQLite library based on the specified
      /// directory and processor architecture.
      /// </summary>
      /// <param name="baseDirectory">
      /// The base directory to use, null for default (the base directory of
      /// the current application domain).  This directory should contain the
      /// processor architecture specific sub-directories.
      /// </param>
      /// <param name="processorArchitecture">
      /// The requested processor architecture, null for default (the
      /// processor architecture of the current process).  This caller should
      /// almost always specify null for this parameter.
      /// </param>
      /// <param name="allowBaseDirectoryOnly">
      /// Non-zero indicates that the native SQLite library can be loaded
      /// from the base directory itself.
      /// </param>
      /// <param name="nativeModuleFileName">
      /// The candidate native module file name to load will be stored here,
      /// if necessary.
      /// </param>
      /// <param name="nativeModuleHandle">
      /// The native module handle as returned by LoadLibrary will be stored
      /// here, if necessary.  This value will be IntPtr.Zero if the call to
      /// LoadLibrary fails.
      /// </param>
      /// <returns>
      /// Non-zero if the native module was loaded successfully; otherwise,
      /// zero.
      /// </returns>
      private static bool PreLoadSQLiteDll(
          string baseDirectory,            /* in */
          string processorArchitecture,    /* in */
          bool allowBaseDirectoryOnly,     /* in */
          ref string nativeModuleFileName, /* out */
          ref IntPtr nativeModuleHandle    /* out */
          )
      {
          //
          // NOTE: If the specified base directory is null, use the default
          //       (i.e. attempt to automatically detect it).
          //
          if (baseDirectory == null)
              baseDirectory = GetBaseDirectory();

          //
          // NOTE: If we failed to query the base directory, stop now.
          //
          if (baseDirectory == null)
              return false;

          //
          // NOTE: Determine the base file name for the native SQLite library.
          //       If this is not known by this class, we cannot continue.
          //
          string fileNameOnly = GetNativeLibraryFileNameOnly();

          if (fileNameOnly == null)
              return false;

          //
          // NOTE: If the native SQLite library exists in the base directory
          //       itself, possibly stop now.
          //
          string fileName = FixUpDllFileName(MaybeCombinePath(baseDirectory,
              fileNameOnly));

          if (File.Exists(fileName))
          {
              //
              // NOTE: If the caller is allowing the base directory itself
              //       to be used, also make sure a processor architecture
              //       was not specified; if either condition is false just
              //       stop now and return failure.
              //
              if (allowBaseDirectoryOnly &&
                  String.IsNullOrEmpty(processorArchitecture))
              {
                  goto baseDirOnly;
              }
              else
              {
                  return false;
              }
          }

          //
          // NOTE: If the specified processor architecture is null, use the
          //       default.
          //
          if (processorArchitecture == null)
              processorArchitecture = GetProcessorArchitecture();

          //
          // NOTE: If we failed to query the processor architecture, stop now.
          //
          if (processorArchitecture == null)
              return false;

          //
          // NOTE: Build the full path and file name for the native SQLite
          //       library using the processor architecture name.
          //
          fileName = FixUpDllFileName(MaybeCombinePath(MaybeCombinePath(
              baseDirectory, processorArchitecture), fileNameOnly));

          //
          // NOTE: If the file name based on the processor architecture name
          // is not found, try using the associated platform name.
          //
          if (!File.Exists(fileName))
          {
              //
              // NOTE: Attempt to translate the processor architecture to a
              //       platform name.
              //
              string platformName = GetPlatformName(processorArchitecture);

              //
              // NOTE: If we failed to translate the platform name, stop now.
              //
              if (platformName == null)
                  return false;

              //
              // NOTE: Build the full path and file name for the native SQLite
              //       library using the platform name.
              //
              fileName = FixUpDllFileName(MaybeCombinePath(MaybeCombinePath(
                  baseDirectory, platformName), fileNameOnly));

              //
              // NOTE: If the file does not exist, skip trying to load it.
              //
              if (!File.Exists(fileName))
                  return false;
          }

      baseDirOnly:

          try
          {
#if !NET_COMPACT_20 && TRACE_PRELOAD
              try
              {
                  //
                  // NOTE: Show exactly where we are trying to load the native
                  //       SQLite library from.
                  //
                  Trace.WriteLine(HelperMethods.StringFormat(
                      CultureInfo.CurrentCulture,
                      "Native library pre-loader is trying to load native " +
                      "SQLite library \"{0}\"...", fileName)); /* throw */
              }
              catch
              {
                  // do nothing.
              }
#endif

              //
              // NOTE: Attempt to load the native library.  This will either
              //       return a valid native module handle, return IntPtr.Zero,
              //       or throw an exception.  This must use the appropriate
              //       P/Invoke method for the current operating system.
              //
              nativeModuleFileName = fileName;
              nativeModuleHandle = NativeLibraryHelper.LoadLibrary(fileName);

              return (nativeModuleHandle != IntPtr.Zero);
          }
#if !NET_COMPACT_20 && TRACE_PRELOAD
          catch (Exception e)
#else
          catch (Exception)
#endif
          {
#if !NET_COMPACT_20 && TRACE_PRELOAD
              try
              {
                  //
                  // NOTE: First, grab the last Win32 error number.
                  //
                  int lastError = Marshal.GetLastWin32Error(); /* throw */

                  //
                  // NOTE: Show where we failed to load the native SQLite
                  //       library from along with the Win32 error code and
                  //       exception information.
                  //
                  Trace.WriteLine(HelperMethods.StringFormat(
                      CultureInfo.CurrentCulture,
                      "Native library pre-loader failed to load native " +
                      "SQLite library \"{0}\" (getLastError = {1}): {2}",
                      fileName, lastError, e)); /* throw */
              }
              catch
              {
                  // do nothing.
              }
#endif
          }

          return false;
      }
#endif
#endif
      #endregion

      /////////////////////////////////////////////////////////////////////////

#if PLATFORM_COMPACTFRAMEWORK
    //
    // NOTE: On the .NET Compact Framework, the native interop assembly must
    //       be used because it provides several workarounds to .NET Compact
    //       Framework limitations important for proper operation of the core
    //       System.Data.SQLite functionality (e.g. being able to bind
    //       parameters and handle column values of types Int64 and Double).
    //
    internal const string SQLITE_DLL = "SQLite.Interop.116.dll";
#elif SQLITE_STANDARD
    //
    // NOTE: Otherwise, if the standard SQLite library is enabled, use it.
    //
    internal const string SQLITE_DLL = "sqlite3";
#elif USE_INTEROP_DLL
    //
    // NOTE: Otherwise, if the native SQLite interop assembly is enabled,
    //       use it.
    //
#if USE_HTC_INTEROP_DLL
    internal const string SQLITE_DLL32 = HelperMethodsMod.Library.VitaExternalSqliteApi32 + ".dll";
    internal const string SQLITE_DLL64 = HelperMethodsMod.Library.VitaExternalSqliteApi64 + ".dll";
#else
    internal const string SQLITE_DLL = "SQLite.Interop.dll";
#endif
#else
    //
    // NOTE: Finally, assume that the mixed-mode assembly is being used.
    //
    internal const string SQLITE_DLL = "System.Data.SQLite.dll";
#endif

    // This section uses interop calls that also fetch text length to optimize conversion.
    // When using the standard dll, we can replace these calls with normal sqlite calls and
    // do unoptimized conversions instead afterwards
    #region interop added textlength calls

#if !SQLITE_STANDARD

#if USE_HTC_INTEROP_DLL
        internal static IntPtr sqlite3_bind_parameter_name_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_parameter_name_interop_64(stmt, index, ref len);
      }
      return sqlite3_bind_parameter_name_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_parameter_name_interop")]
    internal static extern IntPtr sqlite3_bind_parameter_name_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_parameter_name_interop")]
    internal static extern IntPtr sqlite3_bind_parameter_name_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_bind_parameter_name_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_database_name_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_database_name_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_database_name_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_database_name_interop")]
    internal static extern IntPtr sqlite3_column_database_name_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_database_name_interop")]
    internal static extern IntPtr sqlite3_column_database_name_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_database_name_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_database_name16_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_database_name16_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_database_name16_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_database_name16_interop")]
    internal static extern IntPtr sqlite3_column_database_name16_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_database_name16_interop")]
    internal static extern IntPtr sqlite3_column_database_name16_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_database_name16_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_decltype_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_decltype_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_decltype_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_decltype_interop")]
    internal static extern IntPtr sqlite3_column_decltype_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_decltype_interop")]
    internal static extern IntPtr sqlite3_column_decltype_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_decltype_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_decltype16_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_decltype16_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_decltype16_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_decltype16_interop")]
    internal static extern IntPtr sqlite3_column_decltype16_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_decltype16_interop")]
    internal static extern IntPtr sqlite3_column_decltype16_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_decltype16_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_name_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_name_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_name_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_name_interop")]
    internal static extern IntPtr sqlite3_column_name_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_name_interop")]
    internal static extern IntPtr sqlite3_column_name_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_name_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_name16_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_name16_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_name16_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_name16_interop")]
    internal static extern IntPtr sqlite3_column_name16_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_name16_interop")]
    internal static extern IntPtr sqlite3_column_name16_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_name16_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_origin_name_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_origin_name_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_origin_name_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_origin_name_interop")]
    internal static extern IntPtr sqlite3_column_origin_name_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_origin_name_interop")]
    internal static extern IntPtr sqlite3_column_origin_name_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_origin_name_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_origin_name16_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_origin_name16_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_origin_name16_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_origin_name16_interop")]
    internal static extern IntPtr sqlite3_column_origin_name16_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_origin_name16_interop")]
    internal static extern IntPtr sqlite3_column_origin_name16_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_origin_name16_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_table_name_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_table_name_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_table_name_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_table_name_interop")]
    internal static extern IntPtr sqlite3_column_table_name_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_table_name_interop")]
    internal static extern IntPtr sqlite3_column_table_name_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_table_name_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_table_name16_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_table_name16_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_table_name16_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_table_name16_interop")]
    internal static extern IntPtr sqlite3_column_table_name16_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_table_name16_interop")]
    internal static extern IntPtr sqlite3_column_table_name16_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_table_name16_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_text_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_text_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_text_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_text_interop")]
    internal static extern IntPtr sqlite3_column_text_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_text_interop")]
    internal static extern IntPtr sqlite3_column_text_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_text_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_text16_interop(IntPtr stmt, int index, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_text16_interop_64(stmt, index, ref len);
      }
      return sqlite3_column_text16_interop_32(stmt, index, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_text16_interop")]
    internal static extern IntPtr sqlite3_column_text16_interop_32(IntPtr stmt, int index, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_text16_interop")]
    internal static extern IntPtr sqlite3_column_text16_interop_64(IntPtr stmt, int index, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_column_text16_interop(IntPtr stmt, int index, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_errmsg_interop(IntPtr db, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_errmsg_interop_64(db, ref len);
      }
      return sqlite3_errmsg_interop_32(db, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_errmsg_interop")]
    internal static extern IntPtr sqlite3_errmsg_interop_32(IntPtr db, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_errmsg_interop")]
    internal static extern IntPtr sqlite3_errmsg_interop_64(IntPtr db, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_errmsg_interop(IntPtr db, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_prepare_interop(IntPtr db, IntPtr pSql, int nBytes, ref IntPtr stmt, ref IntPtr ptrRemain, ref int nRemain)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_prepare_interop_64(db, pSql, nBytes, ref stmt, ref ptrRemain, ref nRemain);
      }
      return sqlite3_prepare_interop_32(db, pSql, nBytes, ref stmt, ref ptrRemain, ref nRemain);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_prepare_interop")]
    internal static extern SQLiteErrorCode sqlite3_prepare_interop_32(IntPtr db, IntPtr pSql, int nBytes, ref IntPtr stmt, ref IntPtr ptrRemain, ref int nRemain);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_prepare_interop")]
    internal static extern SQLiteErrorCode sqlite3_prepare_interop_64(IntPtr db, IntPtr pSql, int nBytes, ref IntPtr stmt, ref IntPtr ptrRemain, ref int nRemain);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_prepare_interop(IntPtr db, IntPtr pSql, int nBytes, ref IntPtr stmt, ref IntPtr ptrRemain, ref int nRemain);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_table_column_metadata_interop(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, ref IntPtr ptrDataType, ref IntPtr ptrCollSeq, ref int notNull, ref int primaryKey, ref int autoInc, ref int dtLen, ref int csLen)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_table_column_metadata_interop_64(db, dbName, tblName, colName, ref ptrDataType, ref ptrCollSeq, ref notNull, ref primaryKey, ref autoInc, ref dtLen, ref csLen);
      }
      return sqlite3_table_column_metadata_interop_32(db, dbName, tblName, colName, ref ptrDataType, ref ptrCollSeq, ref notNull, ref primaryKey, ref autoInc, ref dtLen, ref csLen);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_table_column_metadata_interop")]
    internal static extern SQLiteErrorCode sqlite3_table_column_metadata_interop_32(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, ref IntPtr ptrDataType, ref IntPtr ptrCollSeq, ref int notNull, ref int primaryKey, ref int autoInc, ref int dtLen, ref int csLen);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_table_column_metadata_interop")]
    internal static extern SQLiteErrorCode sqlite3_table_column_metadata_interop_64(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, ref IntPtr ptrDataType, ref IntPtr ptrCollSeq, ref int notNull, ref int primaryKey, ref int autoInc, ref int dtLen, ref int csLen);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_table_column_metadata_interop(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, ref IntPtr ptrDataType, ref IntPtr ptrCollSeq, ref int notNull, ref int primaryKey, ref int autoInc, ref int dtLen, ref int csLen);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_value_text_interop(IntPtr p, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_value_text_interop_64(p, ref len);
      }
      return sqlite3_value_text_interop_32(p, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_value_text_interop")]
    internal static extern IntPtr sqlite3_value_text_interop_32(IntPtr p, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_value_text_interop")]
    internal static extern IntPtr sqlite3_value_text_interop_64(IntPtr p, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_value_text_interop(IntPtr p, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_value_text16_interop(IntPtr p, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_value_text16_interop_64(p, ref len);
      }
      return sqlite3_value_text16_interop_32(p, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_value_text16_interop")]
    internal static extern IntPtr sqlite3_value_text16_interop_32(IntPtr p, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_value_text16_interop")]
    internal static extern IntPtr sqlite3_value_text16_interop_64(IntPtr p, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_value_text16_interop(IntPtr p, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_malloc_size_interop(IntPtr p)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_malloc_size_interop_64(p);
      }
      return sqlite3_malloc_size_interop_32(p);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_malloc_size_interop")]
    internal static extern int sqlite3_malloc_size_interop_32(IntPtr p);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_malloc_size_interop")]
    internal static extern int sqlite3_malloc_size_interop_64(IntPtr p);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern int sqlite3_malloc_size_interop(IntPtr p);
#endif

#if INTEROP_LOG
#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_config_log_interop()
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_config_log_interop_64();
      }
      return sqlite3_config_log_interop_32();
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_config_log_interop")]
    internal static extern SQLiteErrorCode sqlite3_config_log_interop_32();

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_config_log_interop")]
    internal static extern SQLiteErrorCode sqlite3_config_log_interop_64();
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_config_log_interop();
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_unconfig_log_interop()
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_unconfig_log_interop_64();
      }
      return sqlite3_unconfig_log_interop_32();
    }

    [DllImport(SQLITE_DLL32)]
    internal static extern SQLiteErrorCode sqlite3_unconfig_log_interop_32();

    [DllImport(SQLITE_DLL64)]
    internal static extern SQLiteErrorCode sqlite3_unconfig_log_interop_64();
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_unconfig_log_interop();
#endif
#endif
#endif
// !SQLITE_STANDARD

    #endregion

    // These functions add existing functionality on top of SQLite and require a little effort to
    // get working when using the standard SQLite library.
    #region interop added functionality

#if !SQLITE_STANDARD

#if USE_HTC_INTEROP_DLL
    internal static IntPtr interop_libversion()
    {
      if (IntPtr.Size == 8)
      {
        return interop_libversion_64();
      }
      return interop_libversion_32();
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "interop_libversion")]
    internal static extern IntPtr interop_libversion_32();

    [DllImport(SQLITE_DLL64, EntryPoint = "interop_libversion")]
    internal static extern IntPtr interop_libversion_64();
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr interop_libversion();
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr interop_sourceid()
    {
      if (IntPtr.Size == 8)
      {
        return interop_sourceid_64();
      }
      return interop_sourceid_32();
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "interop_sourceid")]
    internal static extern IntPtr interop_sourceid_32();

    [DllImport(SQLITE_DLL64, EntryPoint = "interop_sourceid")]
    internal static extern IntPtr interop_sourceid_64();
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr interop_sourceid();
#endif

#if USE_HTC_INTEROP_DLL
    internal static int interop_compileoption_used(IntPtr zOptName)
    {
      if (IntPtr.Size == 8)
      {
        return interop_compileoption_used_64(zOptName);
      }
      return interop_compileoption_used_32(zOptName);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "interop_compileoption_used")]
    internal static extern int interop_compileoption_used_32(IntPtr zOptName);

    [DllImport(SQLITE_DLL64, EntryPoint = "interop_compileoption_used")]
    internal static extern int interop_compileoption_used_64(IntPtr zOptName);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern int interop_compileoption_used(IntPtr zOptName);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr interop_compileoption_get(int N)
    {
      if (IntPtr.Size == 8)
      {
        return interop_compileoption_get_64(N);
      }
      return interop_compileoption_get_32(N);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "interop_compileoption_get")]
    internal static extern IntPtr interop_compileoption_get_32(int N);

    [DllImport(SQLITE_DLL64, EntryPoint = "interop_compileoption_get")]
    internal static extern IntPtr interop_compileoption_get_64(int N);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr interop_compileoption_get(int N);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_close_interop(IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_close_interop_64(db);
      }
      return sqlite3_close_interop_32(db);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_close_interop")]
    internal static extern SQLiteErrorCode sqlite3_close_interop_32(IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_close_interop")]
    internal static extern SQLiteErrorCode sqlite3_close_interop_64(IntPtr db);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_close_interop(IntPtr db);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_create_function_interop(IntPtr db, byte[] strName, int nArgs, int nType, IntPtr pvUser, SQLiteCallback func, SQLiteCallback fstep, SQLiteFinalCallback ffinal, int needCollSeq)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_create_function_interop_64(db, strName, nArgs, nType, pvUser, func, fstep, ffinal, needCollSeq);
      }
      return sqlite3_create_function_interop_32(db, strName, nArgs, nType, pvUser, func, fstep, ffinal, needCollSeq);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_create_function_interop")]
    internal static extern SQLiteErrorCode sqlite3_create_function_interop_32(IntPtr db, byte[] strName, int nArgs, int nType, IntPtr pvUser, SQLiteCallback func, SQLiteCallback fstep, SQLiteFinalCallback ffinal, int needCollSeq);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_create_function_interop")]
    internal static extern SQLiteErrorCode sqlite3_create_function_interop_64(IntPtr db, byte[] strName, int nArgs, int nType, IntPtr pvUser, SQLiteCallback func, SQLiteCallback fstep, SQLiteFinalCallback ffinal, int needCollSeq);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_create_function_interop(IntPtr db, byte[] strName, int nArgs, int nType, IntPtr pvUser, SQLiteCallback func, SQLiteCallback fstep, SQLiteFinalCallback ffinal, int needCollSeq);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_finalize_interop(IntPtr stmt)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_finalize_interop_64(stmt);
      }
      return sqlite3_finalize_interop_32(stmt);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_finalize_interop")]
    internal static extern SQLiteErrorCode sqlite3_finalize_interop_32(IntPtr stmt);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_finalize_interop")]
    internal static extern SQLiteErrorCode sqlite3_finalize_interop_64(IntPtr stmt);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_finalize_interop(IntPtr stmt);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_backup_finish_interop(IntPtr backup)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_backup_finish_interop_64(backup);
      }
      return sqlite3_backup_finish_interop_32(backup);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_backup_finish_interop")]
    internal static extern SQLiteErrorCode sqlite3_backup_finish_interop_32(IntPtr backup);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_backup_finish_interop")]
    internal static extern SQLiteErrorCode sqlite3_backup_finish_interop_64(IntPtr backup);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_backup_finish_interop(IntPtr backup);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_blob_close_interop(IntPtr blob)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_blob_close_interop_64(blob);
      }
      return sqlite3_blob_close_interop_32(blob);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_blob_close_interop")]
    internal static extern SQLiteErrorCode sqlite3_blob_close_interop_32(IntPtr blob);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_blob_close_interop")]
    internal static extern SQLiteErrorCode sqlite3_blob_close_interop_64(IntPtr blob);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_blob_close_interop(IntPtr blob);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_open_interop(byte[] utf8Filename, byte[] vfsName, SQLiteOpenFlagsEnum flags, int extFuncs, ref IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_open_interop_64(utf8Filename, vfsName, flags, extFuncs, ref db);
      }
      return sqlite3_open_interop_32(utf8Filename, vfsName, flags, extFuncs, ref db);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_open_interop")]
    internal static extern SQLiteErrorCode sqlite3_open_interop_32(byte[] utf8Filename, byte[] vfsName, SQLiteOpenFlagsEnum flags, int extFuncs, ref IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_open_interop")]
    internal static extern SQLiteErrorCode sqlite3_open_interop_64(byte[] utf8Filename, byte[] vfsName, SQLiteOpenFlagsEnum flags, int extFuncs, ref IntPtr db);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_open_interop(byte[] utf8Filename, byte[] vfsName, SQLiteOpenFlagsEnum flags, int extFuncs, ref IntPtr db);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_open16_interop(byte[] utf8Filename, byte[] vfsName, SQLiteOpenFlagsEnum flags, int extFuncs, ref IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_open16_interop_64(utf8Filename, vfsName, flags, extFuncs, ref db);
      }
      return sqlite3_open16_interop_32(utf8Filename, vfsName, flags, extFuncs, ref db);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_open16_interop")]
    internal static extern SQLiteErrorCode sqlite3_open16_interop_32(byte[] utf8Filename, byte[] vfsName, SQLiteOpenFlagsEnum flags, int extFuncs, ref IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_open16_interop")]
    internal static extern SQLiteErrorCode sqlite3_open16_interop_64(byte[] utf8Filename, byte[] vfsName, SQLiteOpenFlagsEnum flags, int extFuncs, ref IntPtr db);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_open16_interop(byte[] utf8Filename, byte[] vfsName, SQLiteOpenFlagsEnum flags, int extFuncs, ref IntPtr db);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_reset_interop(IntPtr stmt)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_reset_interop_64(stmt);
      }
      return sqlite3_reset_interop_32(stmt);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_reset_interop")]
    internal static extern SQLiteErrorCode sqlite3_reset_interop_32(IntPtr stmt);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_reset_interop")]
    internal static extern SQLiteErrorCode sqlite3_reset_interop_64(IntPtr stmt);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_reset_interop(IntPtr stmt);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_changes_interop(IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_changes_interop_64(db);
      }
      return sqlite3_changes_interop_32(db);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_changes_interop")]
    internal static extern int sqlite3_changes_interop_32(IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_changes_interop")]
    internal static extern int sqlite3_changes_interop_64(IntPtr db);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern int sqlite3_changes_interop(IntPtr db);
#endif
#endif
// !SQLITE_STANDARD

    #endregion

    // The standard api call equivalents of the above interop calls
    #region standard versions of interop functions

#if SQLITE_STANDARD

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_open_v2(byte[] utf8Filename, ref IntPtr db, SQLiteOpenFlagsEnum flags, byte[] vfsName);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
#else
    [DllImport(SQLITE_DLL, CharSet = CharSet.Unicode)]
#endif
    internal static extern SQLiteErrorCode sqlite3_open16(string fileName, ref IntPtr db);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_close(IntPtr db);

#if !INTEROP_LEGACY_CLOSE
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_close_v2(IntPtr db); /* 3.7.14+ */
#endif

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_create_function(IntPtr db, byte[] strName, int nArgs, int nType, IntPtr pvUser, SQLiteCallback func, SQLiteCallback fstep, SQLiteFinalCallback ffinal);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_finalize(IntPtr stmt);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_backup_finish(IntPtr backup);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_reset(IntPtr stmt);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_bind_parameter_name(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_database_name(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_database_name16(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_decltype(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_decltype16(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_name(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_name16(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_origin_name(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_origin_name16(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_table_name(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_table_name16(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_text(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_text16(IntPtr stmt, int index);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_errmsg(IntPtr db);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_prepare(IntPtr db, IntPtr pSql, int nBytes, ref IntPtr stmt, ref IntPtr ptrRemain);

#if USE_PREPARE_V2
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_prepare_v2(IntPtr db, IntPtr pSql, int nBytes, ref IntPtr stmt, ref IntPtr ptrRemain);
#endif

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_table_column_metadata(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, ref IntPtr ptrDataType, ref IntPtr ptrCollSeq, ref int notNull, ref int primaryKey, ref int autoInc);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_value_text(IntPtr p);

#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_value_text16(IntPtr p);

#endif
    // SQLITE_STANDARD

    #endregion

    // These functions are custom and have no equivalent standard library method.
    // All of them are "nice to haves" and not necessarily "need to haves".
    #region no equivalent standard method

#if !SQLITE_STANDARD

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_context_collseq_interop(IntPtr context, ref int type, ref int enc, ref int len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_context_collseq_interop_64(context, ref type, ref enc, ref len);
      }
      return sqlite3_context_collseq_interop_32(context, ref type, ref enc, ref len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_context_collseq_interop")]
    internal static extern IntPtr sqlite3_context_collseq_interop_32(IntPtr context, ref int type, ref int enc, ref int len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_context_collseq_interop")]
    internal static extern IntPtr sqlite3_context_collseq_interop_64(IntPtr context, ref int type, ref int enc, ref int len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_context_collseq_interop(IntPtr context, ref int type, ref int enc, ref int len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_context_collcompare_interop(IntPtr context, byte[] p1, int p1len, byte[] p2, int p2len)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_context_collcompare_interop_64(context, p1, p1len, p2, p2len);
      }
      return sqlite3_context_collcompare_interop_32(context, p1, p1len, p2, p2len);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_context_collcompare_interop")]
    internal static extern int sqlite3_context_collcompare_interop_32(IntPtr context, byte[] p1, int p1len, byte[] p2, int p2len);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_context_collcompare_interop")]
    internal static extern int sqlite3_context_collcompare_interop_64(IntPtr context, byte[] p1, int p1len, byte[] p2, int p2len);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern int sqlite3_context_collcompare_interop(IntPtr context, byte[] p1, int p1len, byte[] p2, int p2len);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_cursor_rowid_interop(IntPtr stmt, int cursor, ref long rowid)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_cursor_rowid_interop_64(stmt, cursor, ref rowid);
      }
      return sqlite3_cursor_rowid_interop_32(stmt, cursor, ref rowid);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_cursor_rowid_interop")]
    internal static extern SQLiteErrorCode sqlite3_cursor_rowid_interop_32(IntPtr stmt, int cursor, ref long rowid);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_cursor_rowid_interop")]
    internal static extern SQLiteErrorCode sqlite3_cursor_rowid_interop_64(IntPtr stmt, int cursor, ref long rowid);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_cursor_rowid_interop(IntPtr stmt, int cursor, ref long rowid);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_index_column_info_interop(IntPtr db, byte[] catalog, byte[] IndexName, byte[] ColumnName, ref int sortOrder, ref int onError, ref IntPtr Collation, ref int colllen)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_index_column_info_interop_64(db, catalog, IndexName, ColumnName, ref sortOrder, ref onError, ref Collation, ref colllen);
      }
      return sqlite3_index_column_info_interop_32(db, catalog, IndexName, ColumnName, ref sortOrder, ref onError, ref Collation, ref colllen);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_index_column_info_interop")]
    internal static extern SQLiteErrorCode sqlite3_index_column_info_interop_32(IntPtr db, byte[] catalog, byte[] IndexName, byte[] ColumnName, ref int sortOrder, ref int onError, ref IntPtr Collation, ref int colllen);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_index_column_info_interop")]
    internal static extern SQLiteErrorCode sqlite3_index_column_info_interop_64(IntPtr db, byte[] catalog, byte[] IndexName, byte[] ColumnName, ref int sortOrder, ref int onError, ref IntPtr Collation, ref int colllen);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_index_column_info_interop(IntPtr db, byte[] catalog, byte[] IndexName, byte[] ColumnName, ref int sortOrder, ref int onError, ref IntPtr Collation, ref int colllen);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_table_cursor_interop(IntPtr stmt, int db, int tableRootPage)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_table_cursor_interop_64(stmt, db, tableRootPage);
      }
      return sqlite3_table_cursor_interop_32(stmt, db, tableRootPage);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_table_cursor_interop")]
    internal static extern int sqlite3_table_cursor_interop_32(IntPtr stmt, int db, int tableRootPage);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_table_cursor_interop")]
    internal static extern int sqlite3_table_cursor_interop_64(IntPtr stmt, int db, int tableRootPage);
#else
    [DllImport(SQLITE_DLL)]
    internal static extern int sqlite3_table_cursor_interop(IntPtr stmt, int db, int tableRootPage);
#endif

#endif
// !SQLITE_STANDARD

    #endregion

    // Standard API calls global across versions.  There are a few instances of interop calls
    // scattered in here, but they are only active when PLATFORM_COMPACTFRAMEWORK is declared.
    #region standard sqlite api calls

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_libversion()
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_libversion_64();
      }
      return sqlite3_libversion_32();
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_libversion", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_libversion_32();

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_libversion", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_libversion_64();
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_libversion();
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_libversion_number()
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_libversion_number_64();
      }
      return sqlite3_libversion_number_32();
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_libversion_number", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_libversion_number_32();

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_libversion_number", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_libversion_number_64();
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_libversion_number();
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_sourceid()
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_sourceid_64();
      }
      return sqlite3_sourceid_32();
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_sourceid", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_sourceid_32();

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_sourceid", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_sourceid_64();
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_sourceid();
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_compileoption_used(IntPtr zOptName)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_compileoption_used_64(zOptName);
      }
      return sqlite3_compileoption_used_32(zOptName);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_compileoption_used", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_compileoption_used_32(IntPtr zOptName);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_compileoption_used", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_compileoption_used_64(IntPtr zOptName);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_compileoption_used(IntPtr zOptName);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_compileoption_get(int N)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_compileoption_get_64(N);
      }
      return sqlite3_compileoption_get_32(N);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_compileoption_get", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_compileoption_get_32(int N);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_compileoption_get", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_compileoption_get_64(int N);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_compileoption_get(int N);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_enable_shared_cache(int enable)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_enable_shared_cache_64(enable);
      }
      return sqlite3_enable_shared_cache_32(enable);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_enable_shared_cache", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_enable_shared_cache_32(int enable);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_enable_shared_cache", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_enable_shared_cache_64(int enable);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_enable_shared_cache(
        int enable);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_enable_load_extension(IntPtr db, int enable)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_enable_load_extension_64(db, enable);
      }
      return sqlite3_enable_load_extension_32(db, enable);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_enable_load_extension", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_enable_load_extension_32(IntPtr db, int enable);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_enable_load_extension", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_enable_load_extension_64(IntPtr db, int enable);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_enable_load_extension(
        IntPtr db, int enable);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_load_extension(IntPtr db, byte[] fileName, byte[] procName, ref IntPtr pError)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_load_extension_64(db, fileName, procName, ref pError);
      }
      return sqlite3_load_extension_32(db, fileName, procName, ref pError);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_load_extension", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_load_extension_32(IntPtr db, byte[] fileName, byte[] procName, ref IntPtr pError);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_load_extension", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_load_extension_64(IntPtr db, byte[] fileName, byte[] procName, ref IntPtr pError);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_load_extension(
        IntPtr db, byte[] fileName, byte[] procName, ref IntPtr pError);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_overload_function(IntPtr db, IntPtr zName, int nArgs)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_overload_function_64(db, zName, nArgs);
      }
      return sqlite3_overload_function_32(db, zName, nArgs);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_overload_function", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_overload_function_32(IntPtr db, IntPtr zName, int nArgs);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_overload_function", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_overload_function_64(IntPtr db, IntPtr zName, int nArgs);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_overload_function(IntPtr db, IntPtr zName, int nArgs);
#endif

#if WINDOWS
#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_win32_set_directory(uint type, string value)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_win32_set_directory_64(type, value);
      }
      return sqlite3_win32_set_directory_32(type, value);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_win32_set_directory", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern SQLiteErrorCode sqlite3_win32_set_directory_32(uint type, string value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_win32_set_directory", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern SQLiteErrorCode sqlite3_win32_set_directory_64(uint type, string value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
#else
    [DllImport(SQLITE_DLL, CharSet = CharSet.Unicode)]
#endif
    //
    // NOTE: The "sqlite3_win32_set_directory" SQLite core library function is
    //       only supported on Windows.
    //
    internal static extern SQLiteErrorCode sqlite3_win32_set_directory(uint type, string value);
#endif

#if !DEBUG // NOTE: Should be "WIN32HEAP && !MEMDEBUG"
#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_win32_reset_heap()
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_win32_reset_heap_64();
      }
      return sqlite3_win32_reset_heap_32();
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_win32_reset_heap", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_win32_reset_heap_32();

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_win32_reset_heap", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_win32_reset_heap_64();
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    //
    // NOTE: The "sqlite3_win32_reset_heap" SQLite core library function is
    //       only supported on Windows when the Win32 native allocator is in
    //       use (i.e. by default, in "Release" builds of System.Data.SQLite
    //       only).  By default, in "Debug" builds of System.Data.SQLite, the
    //       MEMDEBUG allocator is used.
    //
    internal static extern SQLiteErrorCode sqlite3_win32_reset_heap();
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_win32_compact_heap(ref uint largest)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_win32_compact_heap_64(ref largest);
      }
      return sqlite3_win32_compact_heap_32(ref largest);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_win32_compact_heap", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_win32_compact_heap_32(ref uint largest);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_win32_compact_heap", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_win32_compact_heap_64(ref uint largest);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    //
    // NOTE: The "sqlite3_win32_compact_heap" SQLite core library function is
    //       only supported on Windows when the Win32 native allocator is in
    //       use (i.e. by default, in "Release" builds of System.Data.SQLite
    //       only).  By default, in "Debug" builds of System.Data.SQLite, the
    //       MEMDEBUG allocator is used.
    //
    internal static extern SQLiteErrorCode sqlite3_win32_compact_heap(ref uint largest);
#endif
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_malloc(int n)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_malloc_64(n);
      }
      return sqlite3_malloc_32(n);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_malloc", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_malloc_32(int n);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_malloc", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_malloc_64(int n);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_malloc(int n);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_malloc64(ulong n)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_malloc64_64(n);
      }
      return sqlite3_malloc64_32(n);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_malloc64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_malloc64_32(ulong n);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_malloc64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_malloc64_64(ulong n);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_malloc64(ulong n);
#endif

#if USE_HTC_INTEROP_DLL
        internal static IntPtr sqlite3_realloc(IntPtr p, int n)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_realloc_64(p, n);
      }
      return sqlite3_realloc_32(p, n);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_realloc", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_realloc_32(IntPtr p, int n);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_realloc", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_realloc_64(IntPtr p, int n);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_realloc(IntPtr p, int n);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_realloc64(IntPtr p, ulong n)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_realloc64_64(p, n);
      }
      return sqlite3_realloc64_32(p, n);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_realloc64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_realloc64_32(IntPtr p, ulong n);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_realloc64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_realloc64_64(IntPtr p, ulong n);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_realloc64(IntPtr p, ulong n);
#endif

#if USE_HTC_INTEROP_DLL
    internal static ulong sqlite3_msize(IntPtr p)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_msize_64(p);
      }
      return sqlite3_msize_32(p);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_msize", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong sqlite3_msize_32(IntPtr p);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_msize", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong sqlite3_msize_64(IntPtr p);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern ulong sqlite3_msize(IntPtr p);
#endif

#if USE_HTC_INTEROP_DLL
        internal static void sqlite3_free(IntPtr p)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_free_64(p);
      }
      else
      {
        sqlite3_free_32(p);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_free", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_free_32(IntPtr p);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_free", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_free_64(IntPtr p);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_free(IntPtr p);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_interrupt(IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_interrupt_64(db);
      }
      else
      {
        sqlite3_interrupt_32(db);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_interrupt", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_interrupt_32(IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_interrupt", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_interrupt_64(IntPtr db);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_interrupt(IntPtr db);
#endif

#if USE_HTC_INTEROP_DLL
    internal static long sqlite3_last_insert_rowid(IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_last_insert_rowid_64(db);
      }
      return sqlite3_last_insert_rowid_32(db);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_last_insert_rowid", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_last_insert_rowid_32(IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_last_insert_rowid", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_last_insert_rowid_64(IntPtr db);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_last_insert_rowid(IntPtr db);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_changes(IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_changes_64(db);
      }
      return sqlite3_changes_32(db);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_changes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_changes_32(IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_changes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_changes_64(IntPtr db);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_changes(IntPtr db);
#endif

#if USE_HTC_INTEROP_DLL
    internal static long sqlite3_memory_used()
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_memory_used_64();
      }
      return sqlite3_memory_used_32();
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_memory_used", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_memory_used_32();

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_memory_used", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_memory_used_64();
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_memory_used();
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static long sqlite3_memory_highwater(int resetFlag)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_memory_highwater_64(resetFlag);
      }
      return sqlite3_memory_highwater_32(resetFlag);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_memory_highwater", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_memory_highwater_32(int resetFlag);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_memory_highwater", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_memory_highwater_64(int resetFlag);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_memory_highwater(int resetFlag);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_shutdown()
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_shutdown_64();
      }
      return sqlite3_shutdown_32();
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_shutdown", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_shutdown_32();

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_shutdown", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_shutdown_64();
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_shutdown();
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_busy_timeout(IntPtr db, int ms)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_busy_timeout_64(db, ms);
      }
      return sqlite3_busy_timeout_32(db, ms);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_busy_timeout", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_busy_timeout_32(IntPtr db, int ms);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_busy_timeout", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_busy_timeout_64(IntPtr db, int ms);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_busy_timeout(IntPtr db, int ms);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_clear_bindings(IntPtr stmt)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_clear_bindings_64(stmt);
      }
      return sqlite3_clear_bindings_32(stmt);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_clear_bindings", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_clear_bindings_32(IntPtr stmt);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_clear_bindings", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_clear_bindings_64(IntPtr stmt);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_clear_bindings(IntPtr stmt);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_bind_blob(IntPtr stmt, int index, Byte[] value, int nSize, IntPtr nTransient)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_blob_64(stmt, index, value, nSize, nTransient);
      }
      return sqlite3_bind_blob_32(stmt, index, value, nSize, nTransient);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_blob", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_blob_32(IntPtr stmt, int index, Byte[] value, int nSize, IntPtr nTransient);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_blob", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_blob_64(IntPtr stmt, int index, Byte[] value, int nSize, IntPtr nTransient);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_bind_blob(IntPtr stmt, int index, Byte[] value, int nSize, IntPtr nTransient);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_bind_double(IntPtr stmt, int index, double value)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_double_64(stmt, index, value);
      }
      return sqlite3_bind_double_32(stmt, index, value);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_double", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_double_32(IntPtr stmt, int index, double value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_double", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_double_64(IntPtr stmt, int index, double value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_double(IntPtr stmt, int index, double value);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_bind_int(IntPtr stmt, int index, int value)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_int_64(stmt, index, value);
      }
      return sqlite3_bind_int_32(stmt, index, value);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_int_32(IntPtr stmt, int index, int value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_int_64(IntPtr stmt, int index, int value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_bind_int(IntPtr stmt, int index, int value);
#endif

    //
    // NOTE: This really just calls "sqlite3_bind_int"; however, it has the
    //       correct type signature for an unsigned (32-bit) integer.
    //
#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_bind_uint(IntPtr stmt, int index, uint value)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_uint_64(stmt, index, value);
      }
      return sqlite3_bind_uint_32(stmt, index, value);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_uint_32(IntPtr stmt, int index, uint value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_uint_64(IntPtr stmt, int index, uint value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_bind_int")]
#endif
    internal static extern SQLiteErrorCode sqlite3_bind_uint(IntPtr stmt, int index, uint value);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_bind_int64(IntPtr stmt, int index, long value)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_int64_64(stmt, index, value);
      }
      return sqlite3_bind_int64_32(stmt, index, value);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_int64_32(IntPtr stmt, int index, long value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_int64_64(IntPtr stmt, int index, long value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_int64(IntPtr stmt, int index, long value);
#endif
#endif

    //
    // NOTE: This really just calls "sqlite3_bind_int64"; however, it has the
    //       correct type signature for an unsigned long (64-bit) integer.
    //
#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_bind_uint64(IntPtr stmt, int index, ulong value)
    {
      if (IntPtr.Size == 8)
      {
         return sqlite3_bind_uint64_64(stmt, index, value);
      }
      return sqlite3_bind_uint64_32(stmt, index, value);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_uint64_32(IntPtr stmt, int index, ulong value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_uint64_64(IntPtr stmt, int index, ulong value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_uint64(IntPtr stmt, int index, ulong value);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_bind_null(IntPtr stmt, int index)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_null_64(stmt, index);
      }
      return sqlite3_bind_null_32(stmt, index);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_null_32(IntPtr stmt, int index);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_null_64(IntPtr stmt, int index);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_bind_null(IntPtr stmt, int index);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_bind_text(IntPtr stmt, int index, byte[] value, int nlen, IntPtr pvReserved)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_text_64(stmt, index, value, nlen, pvReserved);
      }
      return sqlite3_bind_text_32(stmt, index, value, nlen, pvReserved);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_text", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_text_32(IntPtr stmt, int index, byte[] value, int nlen, IntPtr pvReserved);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_text", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_bind_text_64(IntPtr stmt, int index, byte[] value, int nlen, IntPtr pvReserved);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_bind_text(IntPtr stmt, int index, byte[] value, int nlen, IntPtr pvReserved);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_bind_parameter_count(IntPtr stmt)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_parameter_count_64(stmt);
      }
      return sqlite3_bind_parameter_count_32(stmt);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_parameter_count", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_bind_parameter_count_32(IntPtr stmt);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_parameter_count", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_bind_parameter_count_64(IntPtr stmt);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_bind_parameter_count(IntPtr stmt);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_bind_parameter_index(IntPtr stmt, byte[] strName)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_parameter_index_64(stmt, strName);
      }
      return sqlite3_bind_parameter_index_32(stmt, strName);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_parameter_index", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_bind_parameter_index_32(IntPtr stmt, byte[] strName);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_parameter_index", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_bind_parameter_index_64(IntPtr stmt, byte[] strName);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_bind_parameter_index(IntPtr stmt, byte[] strName);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_column_count(IntPtr stmt)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_count_64(stmt);
      }
      return sqlite3_column_count_32(stmt);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_count", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_column_count_32(IntPtr stmt);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_count", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_column_count_64(IntPtr stmt);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_column_count(IntPtr stmt);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_step(IntPtr stmt)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_step_64(stmt);
      }
      return sqlite3_step_32(stmt);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_step_32(IntPtr stmt);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_step_64(IntPtr stmt);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_step(IntPtr stmt);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_stmt_readonly(IntPtr stmt)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_stmt_readonly_64(stmt);
      }
      return sqlite3_stmt_readonly_32(stmt);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_stmt_readonly", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_stmt_readonly_32(IntPtr stmt); /* 3.7.4+ */

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_stmt_readonly", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_stmt_readonly_64(IntPtr stmt); /* 3.7.4+ */
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_stmt_readonly(IntPtr stmt); /* 3.7.4+ */
#endif

#if USE_HTC_INTEROP_DLL
    internal static double sqlite3_column_double(IntPtr stmt, int index)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_double_64(stmt, index);
      }
      return sqlite3_column_double_32(stmt, index);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_double", CallingConvention = CallingConvention.Cdecl)]
    internal static extern double sqlite3_column_double_32(IntPtr stmt, int index);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_double", CallingConvention = CallingConvention.Cdecl)]
    internal static extern double sqlite3_column_double_64(IntPtr stmt, int index);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double sqlite3_column_double(IntPtr stmt, int index);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_column_int(IntPtr stmt, int index)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_int_64(stmt, index);
      }
      return sqlite3_column_int_32(stmt, index);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_int", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_column_int_32(IntPtr stmt, int index);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_int", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_column_int_64(IntPtr stmt, int index);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_column_int(IntPtr stmt, int index);
#endif

#if USE_HTC_INTEROP_DLL
    internal static long sqlite3_column_int64(IntPtr stmt, int index)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_int64_64(stmt, index);
      }
      return sqlite3_column_int64_32(stmt, index);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_column_int64_32(IntPtr stmt, int index);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_column_int64_64(IntPtr stmt, int index);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_column_int64(IntPtr stmt, int index);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_column_blob(IntPtr stmt, int index)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_blob_64(stmt, index);
      }
      return sqlite3_column_blob_32(stmt, index);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_blob", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_column_blob_32(IntPtr stmt, int index);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_blob", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_column_blob_64(IntPtr stmt, int index);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_column_blob(IntPtr stmt, int index);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_column_bytes(IntPtr stmt, int index)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_bytes_64(stmt, index);
      }
      return sqlite3_column_bytes_32(stmt, index);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_bytes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_column_bytes_32(IntPtr stmt, int index);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_bytes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_column_bytes_64(IntPtr stmt, int index);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_column_bytes(IntPtr stmt, int index);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_column_bytes16(IntPtr stmt, int index)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_bytes16_64(stmt, index);
      }
      return sqlite3_column_bytes16_32(stmt, index);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_bytes16", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_column_bytes16_32(IntPtr stmt, int index);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_bytes16", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_column_bytes16_64(IntPtr stmt, int index);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_column_bytes16(IntPtr stmt, int index);
#endif

#if USE_HTC_INTEROP_DLL
    internal static TypeAffinity sqlite3_column_type(IntPtr stmt, int index)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_column_type_64(stmt, index);
      }
      return sqlite3_column_type_32(stmt, index);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_column_type", CallingConvention = CallingConvention.Cdecl)]
    internal static extern TypeAffinity sqlite3_column_type_32(IntPtr stmt, int index);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_column_type", CallingConvention = CallingConvention.Cdecl)]
    internal static extern TypeAffinity sqlite3_column_type_64(IntPtr stmt, int index);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern TypeAffinity sqlite3_column_type(IntPtr stmt, int index);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_create_collation(IntPtr db, byte[] strName, int nType, IntPtr pvUser, SQLiteCollation func)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_create_collation_64(db, strName, nType, pvUser, func);
      }
      return sqlite3_create_collation_32(db, strName, nType, pvUser, func);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_create_collation", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_create_collation_32(IntPtr db, byte[] strName, int nType, IntPtr pvUser, SQLiteCollation func);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_create_collation", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_create_collation_64(IntPtr db, byte[] strName, int nType, IntPtr pvUser, SQLiteCollation func);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_create_collation(IntPtr db, byte[] strName, int nType, IntPtr pvUser, SQLiteCollation func);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_aggregate_count(IntPtr context)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_aggregate_count_64(context);
      }
      return sqlite3_aggregate_count_32(context);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_aggregate_count", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_aggregate_count_32(IntPtr context);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_aggregate_count", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_aggregate_count_64(IntPtr context);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_aggregate_count(IntPtr context);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_value_blob(IntPtr p)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_value_blob_64(p);
      }
      return sqlite3_value_blob_32(p);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_value_blob", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_value_blob_32(IntPtr p);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_value_blob", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_value_blob_64(IntPtr p);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_value_blob(IntPtr p);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_value_bytes(IntPtr p)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_value_bytes_64(p);
      }
      return sqlite3_value_bytes_32(p);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_value_bytes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_value_bytes_32(IntPtr p);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_value_bytes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_value_bytes_64(IntPtr p);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_value_bytes(IntPtr p);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_value_bytes16(IntPtr p)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_value_bytes16_64(p);
      }
      return sqlite3_value_bytes16_32(p);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_value_bytes16", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_value_bytes16_32(IntPtr p);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_value_bytes16", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_value_bytes16_64(IntPtr p);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_value_bytes16(IntPtr p);
#endif

#if USE_HTC_INTEROP_DLL
    internal static double sqlite3_value_double(IntPtr p)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_value_double_64(p);
      }
      return sqlite3_value_double_32(p);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_value_double", CallingConvention = CallingConvention.Cdecl)]
    internal static extern double sqlite3_value_double_32(IntPtr p);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_value_double", CallingConvention = CallingConvention.Cdecl)]
    internal static extern double sqlite3_value_double_64(IntPtr p);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double sqlite3_value_double(IntPtr p);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_value_int(IntPtr p)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_value_int_64(p);
      }
      return sqlite3_value_int_32(p);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_value_int", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_value_int_32(IntPtr p);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_value_int", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_value_int_64(IntPtr p);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_value_int(IntPtr p);
#endif

#if USE_HTC_INTEROP_DLL
    internal static long sqlite3_value_int64(IntPtr p)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_value_int64_64(p);
      }
      return sqlite3_value_int64_32(p);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_value_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_value_int64_32(IntPtr p);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_value_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_value_int64_64(IntPtr p);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3_value_int64(IntPtr p);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static TypeAffinity sqlite3_value_type(IntPtr p)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_value_type_64(p);
      }
      return sqlite3_value_type_32(p);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_value_type", CallingConvention = CallingConvention.Cdecl)]
    internal static extern TypeAffinity sqlite3_value_type_32(IntPtr p);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_value_type", CallingConvention = CallingConvention.Cdecl)]
    internal static extern TypeAffinity sqlite3_value_type_64(IntPtr p);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern TypeAffinity sqlite3_value_type(IntPtr p);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_blob(IntPtr context, byte[] value, int nSize, IntPtr pvReserved)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_blob_64(context, value, nSize, pvReserved);
      }
      else
      {
        sqlite3_result_blob_32(context, value, nSize, pvReserved);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_blob", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_blob_32(IntPtr context, byte[] value, int nSize, IntPtr pvReserved);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_blob", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_blob_64(IntPtr context, byte[] value, int nSize, IntPtr pvReserved);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_result_blob(IntPtr context, byte[] value, int nSize, IntPtr pvReserved);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_double(IntPtr context, double value)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_double_64(context, value);
      }
      else
      {
        sqlite3_result_double_32(context, value);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_double", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_double_32(IntPtr context, double value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_double", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_double_64(IntPtr context, double value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_double(IntPtr context, double value);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_error(IntPtr context, byte[] strErr, int nLen)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_error_64(context, strErr, nLen);
      }
      else
      {
        sqlite3_result_error_32(context, strErr, nLen);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_error", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_error_32(IntPtr context, byte[] strErr, int nLen);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_error", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_error_64(IntPtr context, byte[] strErr, int nLen);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_result_error(IntPtr context, byte[] strErr, int nLen);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_error_code(IntPtr context, SQLiteErrorCode value)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_error_code_64(context, value);
      }
      else
      {
        sqlite3_result_error_code_64(context, value);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_error_code", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_error_code_32(IntPtr context, SQLiteErrorCode value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_error_code", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_error_code_64(IntPtr context, SQLiteErrorCode value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_result_error_code(IntPtr context, SQLiteErrorCode value);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_error_toobig(IntPtr context)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_error_toobig_64(context);
      }
      else
      {
        sqlite3_result_error_toobig_32(context);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_error_toobig", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_error_toobig_32(IntPtr context);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_error_toobig", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_error_toobig_64(IntPtr context);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_result_error_toobig(IntPtr context);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_error_nomem(IntPtr context)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_error_nomem_64(context);
      }
      else
      {
        sqlite3_result_error_nomem_32(context);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_error_nomem", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_error_nomem_32(IntPtr context);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_error_nomem", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_error_nomem_64(IntPtr context);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_result_error_nomem(IntPtr context);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_value(IntPtr context, IntPtr value)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_value_64(context, value);
      }
      else
      {
        sqlite3_result_value_32(context, value);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_value", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_value_32(IntPtr context, IntPtr value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_value", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_value_64(IntPtr context, IntPtr value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_result_value(IntPtr context, IntPtr value);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_zeroblob(IntPtr context, int nLen)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_zeroblob_64(context, nLen);
      }
      else
      {
        sqlite3_result_zeroblob_32(context, nLen);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_zeroblob", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_zeroblob_32(IntPtr context, int nLen);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_zeroblob", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_zeroblob_64(IntPtr context, int nLen);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_result_zeroblob(IntPtr context, int nLen);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_int(IntPtr context, int value)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_int_64(context, value);
      }
      else
      {
        sqlite3_result_int_32(context, value);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_int", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_int_32(IntPtr context, int value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_int", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_int_64(IntPtr context, int value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_result_int(IntPtr context, int value);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_int64(IntPtr context, long value)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_int64_64(context, value);
      }
      else
      {
        sqlite3_result_int64_32(context, value);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_int64_32(IntPtr context, long value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_int64", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_int64_64(IntPtr context, long value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_int64(IntPtr context, long value);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_null(IntPtr context)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_null_64(context);
      }
      else
      {
        sqlite3_result_null_32(context);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_null_32(IntPtr context);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_null_64(IntPtr context);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_result_null(IntPtr context);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_text(IntPtr context, byte[] value, int nLen, IntPtr pvReserved)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_text_64(context, value, nLen, pvReserved);
      }
      else
      {
        sqlite3_result_text_32(context, value, nLen, pvReserved);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_text", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_text_32(IntPtr context, byte[] value, int nLen, IntPtr pvReserved);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_text", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_result_text_64(IntPtr context, byte[] value, int nLen, IntPtr pvReserved);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_result_text(IntPtr context, byte[] value, int nLen, IntPtr pvReserved);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_aggregate_context(IntPtr context, int nBytes)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_aggregate_context_64(context, nBytes);
      }
      return sqlite3_aggregate_context_32(context, nBytes);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_aggregate_context", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_aggregate_context_32(IntPtr context, int nBytes);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_aggregate_context", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_aggregate_context_64(IntPtr context, int nBytes);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_aggregate_context(IntPtr context, int nBytes);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_bind_text16(IntPtr stmt, int index, string value, int nlen, IntPtr pvReserved)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_bind_text16_64(stmt, index, value, nlen, pvReserved);
      }
      return sqlite3_bind_text16_32(stmt, index, value, nlen, pvReserved);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_bind_text16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern SQLiteErrorCode sqlite3_bind_text16_32(IntPtr stmt, int index, string value, int nlen, IntPtr pvReserved);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_bind_text16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern SQLiteErrorCode sqlite3_bind_text16_64(IntPtr stmt, int index, string value, int nlen, IntPtr pvReserved);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
#else
    [DllImport(SQLITE_DLL, CharSet = CharSet.Unicode)]
#endif
    internal static extern SQLiteErrorCode sqlite3_bind_text16(IntPtr stmt, int index, string value, int nlen, IntPtr pvReserved);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_error16(IntPtr context, string strName, int nLen)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_error16_64(context, strName, nLen);
      }
      else
      {
        sqlite3_result_error16_32(context, strName, nLen);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_error16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern void sqlite3_result_error16_32(IntPtr context, string strName, int nLen);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_error16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern void sqlite3_result_error16_64(IntPtr context, string strName, int nLen);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
#else
    [DllImport(SQLITE_DLL, CharSet = CharSet.Unicode)]
#endif
    internal static extern void sqlite3_result_error16(IntPtr context, string strName, int nLen);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_result_text16(IntPtr context, string strName, int nLen, IntPtr pvReserved)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_result_text16_64(context, strName, nLen, pvReserved);
      }
      else
      {
        sqlite3_result_text16_32(context, strName, nLen, pvReserved);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_result_text16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern void sqlite3_result_text16_32(IntPtr context, string strName, int nLen, IntPtr pvReserved);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_result_text16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern void sqlite3_result_text16_64(IntPtr context, string strName, int nLen, IntPtr pvReserved);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
#else
    [DllImport(SQLITE_DLL, CharSet = CharSet.Unicode)]
#endif
    internal static extern void sqlite3_result_text16(IntPtr context, string strName, int nLen, IntPtr pvReserved);
#endif

#if INTEROP_CODEC || INTEROP_INCLUDE_SEE
#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_key(IntPtr db, byte[] key, int keylen)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_key_64(db, key, keylen);
      }
      return sqlite3_key_32(db, key, keylen);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_key", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_key_32(IntPtr db, byte[] key, int keylen);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_key", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_key_64(IntPtr db, byte[] key, int keylen);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_key(IntPtr db, byte[] key, int keylen);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_rekey(IntPtr db, byte[] key, int keylen)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_rekey_64(db, key, keylen);
      }
      return sqlite3_rekey_32(db, key, keylen);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_rekey", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_rekey_32(IntPtr db, byte[] key, int keylen);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_rekey", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_rekey_64(IntPtr db, byte[] key, int keylen);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_rekey(IntPtr db, byte[] key, int keylen);
#endif
#endif

#if INTEROP_INCLUDE_ZIPVFS
#if USE_HTC_INTEROP_DLL
    internal static void zipvfsInit_v2()
    {
      if (IntPtr.Size == 8)
      {
        zipvfsInit_v2_64();
      }
      else
      {
        zipvfsInit_v2_32();
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "zipvfsInit_v2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void zipvfsInit_v2_32();
 
    [DllImport(SQLITE_DLL64, EntryPoint = "zipvfsInit_v2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void zipvfsInit_v2_64();
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void zipvfsInit_v2();
#endif

#if USE_HTC_INTEROP_DLL
    internal static void zipvfsInit_v3(int regDflt)
    {
      if (IntPtr.Size == 8)
      {
        zipvfsInit_v3_64(regDflt);
      }
      else
      {
        zipvfsInit_v3_32(regDflt);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "zipvfsInit_v3", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void zipvfsInit_v3_32(int regDflt);

    [DllImport(SQLITE_DLL64, EntryPoint = "zipvfsInit_v3", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void zipvfsInit_v3_64(int regDflt);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void zipvfsInit_v3(int regDflt);
#endif
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_busy_handler(IntPtr db, SQLiteBusyCallback func, IntPtr pvUser)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_busy_handler_64(db, func, pvUser);
      }
      else
      {
        sqlite3_busy_handler_32(db, func, pvUser);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_busy_handler", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_busy_handler_32(IntPtr db, SQLiteBusyCallback func, IntPtr pvUser);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_busy_handler", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_busy_handler_64(IntPtr db, SQLiteBusyCallback func, IntPtr pvUser);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_busy_handler(IntPtr db, SQLiteBusyCallback func, IntPtr pvUser);
#endif

#if USE_HTC_INTEROP_DLL
        internal static void sqlite3_progress_handler(IntPtr db, int ops, SQLiteProgressCallback func, IntPtr pvUser)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_progress_handler_64(db, ops, func, pvUser);
      }
      else
      {
        sqlite3_progress_handler_32(db, ops, func, pvUser);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_progress_handler", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_progress_handler_32(IntPtr db, int ops, SQLiteProgressCallback func, IntPtr pvUser);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_progress_handler", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_progress_handler_64(IntPtr db, int ops, SQLiteProgressCallback func, IntPtr pvUser);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_progress_handler(IntPtr db, int ops, SQLiteProgressCallback func, IntPtr pvUser);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_set_authorizer(IntPtr db, SQLiteAuthorizerCallback func, IntPtr pvUser)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_set_authorizer_64(db, func, pvUser);
      }
      return sqlite3_set_authorizer_32(db, func, pvUser);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_set_authorizer", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_set_authorizer_32(IntPtr db, SQLiteAuthorizerCallback func, IntPtr pvUser);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_set_authorizer", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_set_authorizer_64(IntPtr db, SQLiteAuthorizerCallback func, IntPtr pvUser);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_set_authorizer(IntPtr db, SQLiteAuthorizerCallback func, IntPtr pvUser);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_update_hook(IntPtr db, SQLiteUpdateCallback func, IntPtr pvUser)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_update_hook_64(db, func, pvUser);
      }
      return sqlite3_update_hook_32(db, func, pvUser);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_update_hook", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_update_hook_32(IntPtr db, SQLiteUpdateCallback func, IntPtr pvUser);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_update_hook", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_update_hook_64(IntPtr db, SQLiteUpdateCallback func, IntPtr pvUser);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_update_hook(IntPtr db, SQLiteUpdateCallback func, IntPtr pvUser);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_commit_hook(IntPtr db, SQLiteCommitCallback func, IntPtr pvUser)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_commit_hook_64(db, func, pvUser);
      }
      return sqlite3_commit_hook_32(db, func, pvUser);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_commit_hook", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_commit_hook_32(IntPtr db, SQLiteCommitCallback func, IntPtr pvUser);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_commit_hook", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_commit_hook_64(IntPtr db, SQLiteCommitCallback func, IntPtr pvUser);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_commit_hook(IntPtr db, SQLiteCommitCallback func, IntPtr pvUser);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_trace(IntPtr db, SQLiteTraceCallback func, IntPtr pvUser)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_trace_64(db, func, pvUser);
      }
      return sqlite3_trace_32(db, func, pvUser);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_trace", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_trace_32(IntPtr db, SQLiteTraceCallback func, IntPtr pvUser);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_trace", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_trace_64(IntPtr db, SQLiteTraceCallback func, IntPtr pvUser);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_trace(IntPtr db, SQLiteTraceCallback func, IntPtr pvUser);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_trace_v2(IntPtr db, SQLiteTraceFlags mask, SQLiteTraceCallback2 func, IntPtr pvUser)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_trace_v2_64(db, mask, func, pvUser);
      }
      return sqlite3_trace_v2_32(db, mask, func, pvUser);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_trace_v2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_trace_v2_32(IntPtr db, SQLiteTraceFlags mask, SQLiteTraceCallback2 func, IntPtr pvUser);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_trace_v2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_trace_v2_64(IntPtr db, SQLiteTraceFlags mask, SQLiteTraceCallback2 func, IntPtr pvUser);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_trace_v2(IntPtr db, SQLiteTraceFlags mask, SQLiteTraceCallback2 func, IntPtr pvUser);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_limit(IntPtr db, SQLiteLimitOpsEnum op, int value)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_limit_64(db, op, value);
      }
      return sqlite3_limit_32(db, op, value);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_limit", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_limit_32(IntPtr db, SQLiteLimitOpsEnum op, int value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_limit", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_limit_64(IntPtr db, SQLiteLimitOpsEnum op, int value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_limit(IntPtr db, SQLiteLimitOpsEnum op, int value);
#endif

    // Since sqlite3_config() takes a variable argument list, we have to overload declarations
    // for all possible calls that we want to use.
#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_config_none(SQLiteConfigOpsEnum op)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_config_none_64(op);
      }
      return sqlite3_config_none_32(op);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_config_none_32(SQLiteConfigOpsEnum op);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_config_none_64(SQLiteConfigOpsEnum op);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_config")]
#endif
    internal static extern SQLiteErrorCode sqlite3_config_none(SQLiteConfigOpsEnum op);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_config_int(SQLiteConfigOpsEnum op, int value)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_config_int_64(op, value);
      }
      return sqlite3_config_int_32(op, value);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_config_int_32(SQLiteConfigOpsEnum op, int value);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_config_int_64(SQLiteConfigOpsEnum op, int value);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_config")]
#endif
    internal static extern SQLiteErrorCode sqlite3_config_int(SQLiteConfigOpsEnum op, int value);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_config_log(SQLiteConfigOpsEnum op, SQLiteLogCallback func, IntPtr pvUser)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_config_log_64(op, func, pvUser);
      }
      return sqlite3_config_log_32(op, func, pvUser);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_config_log_32(SQLiteConfigOpsEnum op, SQLiteLogCallback func, IntPtr pvUser);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_config_log_64(SQLiteConfigOpsEnum op, SQLiteLogCallback func, IntPtr pvUser);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_config")]
#endif
    internal static extern SQLiteErrorCode sqlite3_config_log(SQLiteConfigOpsEnum op, SQLiteLogCallback func, IntPtr pvUser);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_db_config_charptr(IntPtr db, SQLiteConfigDbOpsEnum op, IntPtr charPtr)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_db_config_charptr_64(db, op, charPtr);
      }
      return sqlite3_db_config_charptr_32(db, op, charPtr);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_db_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_db_config_charptr_32(IntPtr db, SQLiteConfigDbOpsEnum op, IntPtr charPtr);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_db_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_db_config_charptr_64(IntPtr db, SQLiteConfigDbOpsEnum op, IntPtr charPtr);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_db_config", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_db_config")]
#endif
    internal static extern SQLiteErrorCode sqlite3_db_config_charptr(IntPtr db, SQLiteConfigDbOpsEnum op, IntPtr charPtr);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_db_config_int_refint(IntPtr db, SQLiteConfigDbOpsEnum op, int value, ref int result)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_db_config_int_refint_64(db, op, value, ref result);
      }
      return sqlite3_db_config_int_refint_32(db, op, value, ref result);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_db_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_db_config_int_refint_32(IntPtr db, SQLiteConfigDbOpsEnum op, int value, ref int result);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_db_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_db_config_int_refint_64(IntPtr db, SQLiteConfigDbOpsEnum op, int value, ref int result);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_db_config", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_db_config")]
#endif
    internal static extern SQLiteErrorCode sqlite3_db_config_int_refint(IntPtr db, SQLiteConfigDbOpsEnum op, int value, ref int result);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_db_config_intptr_two_ints(IntPtr db, SQLiteConfigDbOpsEnum op, IntPtr ptr, int int0, int int1)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_db_config_intptr_two_ints_64(db, op, ptr, int0, int1);
      }
      return sqlite3_db_config_intptr_two_ints_32(db, op, ptr, int0, int1);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_db_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_db_config_intptr_two_ints_32(IntPtr db, SQLiteConfigDbOpsEnum op, IntPtr ptr, int int0, int int1);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_db_config", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_db_config_intptr_two_ints_64(IntPtr db, SQLiteConfigDbOpsEnum op, IntPtr ptr, int int0, int int1);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_db_config", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_db_config")]
#endif
    internal static extern SQLiteErrorCode sqlite3_db_config_intptr_two_ints(IntPtr db, SQLiteConfigDbOpsEnum op, IntPtr ptr, int int0, int int1);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_db_status(IntPtr db, SQLiteStatusOpsEnum op, ref int current, ref int highwater, int resetFlag)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_db_status_64(db, op, ref current, ref highwater, resetFlag);
      }
      return sqlite3_db_status_32(db, op, ref current, ref highwater, resetFlag);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_db_status", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_db_status_32(IntPtr db, SQLiteStatusOpsEnum op, ref int current, ref int highwater, int resetFlag);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_db_status", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_db_status_64(IntPtr db, SQLiteStatusOpsEnum op, ref int current, ref int highwater, int resetFlag);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_db_status(IntPtr db, SQLiteStatusOpsEnum op, ref int current, ref int highwater, int resetFlag);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_rollback_hook(IntPtr db, SQLiteRollbackCallback func, IntPtr pvUser)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_rollback_hook_64(db, func, pvUser);
      }
      return sqlite3_rollback_hook_32(db, func, pvUser);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_rollback_hook", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_rollback_hook_32(IntPtr db, SQLiteRollbackCallback func, IntPtr pvUser);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_rollback_hook", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_rollback_hook_64(IntPtr db, SQLiteRollbackCallback func, IntPtr pvUser);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_rollback_hook(IntPtr db, SQLiteRollbackCallback func, IntPtr pvUser);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_db_handle(IntPtr stmt)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_db_handle_64(stmt);
      }
      return sqlite3_db_handle_32(stmt);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_db_handle", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_db_handle_32(IntPtr stmt);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_db_handle", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_db_handle_64(IntPtr stmt);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_db_handle(IntPtr stmt);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_db_release_memory(IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_db_release_memory_64(db);
      }
      return sqlite3_db_release_memory_32(db);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_db_release_memory", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_db_release_memory_32(IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_db_release_memory", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_db_release_memory_64(IntPtr db);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_db_release_memory(IntPtr db);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_db_filename(IntPtr db, IntPtr dbName)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_db_filename_64(db, dbName);
      }
      return sqlite3_db_filename_32(db, dbName);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_db_filename", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_db_filename_32(IntPtr db, IntPtr dbName);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_db_filename", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_db_filename_64(IntPtr db, IntPtr dbName);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_db_filename(IntPtr db, IntPtr dbName);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_db_readonly(IntPtr db, IntPtr dbName)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_db_readonly_64(db, dbName);
      }
      return sqlite3_db_readonly_32(db, dbName);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_db_readonly", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_db_readonly_32(IntPtr db, IntPtr dbName);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_db_readonly", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_db_readonly_64(IntPtr db, IntPtr dbName);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_db_readonly(IntPtr db, IntPtr dbName);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_db_filename_bytes(IntPtr db, byte[] dbName)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_db_filename_bytes_64(db, dbName);
      }
      return sqlite3_db_filename_bytes_32(db, dbName);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_db_filename", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_db_filename_bytes_32(IntPtr db, byte[] dbName);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_db_filename", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_db_filename_bytes_64(IntPtr db, byte[] dbName);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_db_filename", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_db_filename")]
#endif
    internal static extern IntPtr sqlite3_db_filename_bytes(IntPtr db, byte[] dbName);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_next_stmt_64(db, stmt);
      }
      return sqlite3_next_stmt_32(db, stmt);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_next_stmt", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_next_stmt_32(IntPtr db, IntPtr stmt);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_next_stmt", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_next_stmt_64(IntPtr db, IntPtr stmt);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_exec(IntPtr db, byte[] strSql, IntPtr pvCallback, IntPtr pvParam, ref IntPtr errMsg)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_exec_64(db, strSql, pvCallback, pvParam, ref errMsg);
      }
      return sqlite3_exec_32(db, strSql, pvCallback, pvParam, ref errMsg);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_exec", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_exec_32(IntPtr db, byte[] strSql, IntPtr pvCallback, IntPtr pvParam, ref IntPtr errMsg);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_exec", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_exec_64(IntPtr db, byte[] strSql, IntPtr pvCallback, IntPtr pvParam, ref IntPtr errMsg);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_exec(IntPtr db, byte[] strSql, IntPtr pvCallback, IntPtr pvParam, ref IntPtr errMsg);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_release_memory(int nBytes)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_release_memory_64(nBytes);
      }
      return sqlite3_release_memory_32(nBytes);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_release_memory", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_release_memory_32(int nBytes);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_release_memory", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_release_memory_64(int nBytes);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_release_memory(int nBytes);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_get_autocommit(IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_get_autocommit_64(db);
      }
      return sqlite3_get_autocommit_32(db);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_get_autocommit", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_get_autocommit_32(IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_get_autocommit", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_get_autocommit_64(IntPtr db);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_get_autocommit(IntPtr db);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_extended_result_codes(IntPtr db, int onoff)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_extended_result_codes_64(db, onoff);
      }
      return sqlite3_extended_result_codes_32(db, onoff);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_extended_result_codes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_extended_result_codes_32(IntPtr db, int onoff);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_extended_result_codes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_extended_result_codes_64(IntPtr db, int onoff);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_extended_result_codes(IntPtr db, int onoff);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_errcode(IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_errcode_64(db);
      }
      return sqlite3_errcode_32(db);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_errcode", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_errcode_32(IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_errcode", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_errcode_64(IntPtr db);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_errcode(IntPtr db);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_extended_errcode(IntPtr db)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_extended_errcode_64(db);
      }
      return sqlite3_extended_errcode_32(db);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_extended_errcode", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_extended_errcode_32(IntPtr db);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_extended_errcode", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_extended_errcode_64(IntPtr db);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_extended_errcode(IntPtr db);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_errstr(SQLiteErrorCode rc)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_errstr_64(rc);
      }
      return sqlite3_errstr_32(rc);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_errstr", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_errstr_32(SQLiteErrorCode rc); /* 3.7.15+ */

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_errstr", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_errstr_64(SQLiteErrorCode rc); /* 3.7.15+ */
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_errstr(SQLiteErrorCode rc); /* 3.7.15+ */
#endif

    // Since sqlite3_log() takes a variable argument list, we have to overload declarations
    // for all possible calls.  For now, we are only exposing a single string, and
    // depend on the caller to format the string.
#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_log(SQLiteErrorCode iErrCode, byte[] zFormat)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_log_64(iErrCode, zFormat);
      }
      else
      {
        sqlite3_log_32(iErrCode, zFormat);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_log", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_log_32(SQLiteErrorCode iErrCode, byte[] zFormat);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_log", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_log_64(SQLiteErrorCode iErrCode, byte[] zFormat);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_log(SQLiteErrorCode iErrCode, byte[] zFormat);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_file_control(IntPtr db, byte[] zDbName, int op, IntPtr pArg)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_file_control_64(db, zDbName, op, pArg);
      }
      return sqlite3_file_control_32(db, zDbName, op, pArg);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_file_control", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_file_control_32(IntPtr db, byte[] zDbName, int op, IntPtr pArg);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_file_control", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_file_control_64(IntPtr db, byte[] zDbName, int op, IntPtr pArg);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_file_control(IntPtr db, byte[] zDbName, int op, IntPtr pArg);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_backup_init(IntPtr destDb, byte[] zDestName, IntPtr sourceDb, byte[] zSourceName)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_backup_init_64(destDb, zDestName, sourceDb, zSourceName);
      }
      return sqlite3_backup_init_32(destDb, zDestName, sourceDb, zSourceName);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_backup_init", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_backup_init_32(IntPtr destDb, byte[] zDestName, IntPtr sourceDb, byte[] zSourceName);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_backup_init", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_backup_init_64(IntPtr destDb, byte[] zDestName, IntPtr sourceDb, byte[] zSourceName);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_backup_init(IntPtr destDb, byte[] zDestName, IntPtr sourceDb, byte[] zSourceName);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_backup_step(IntPtr backup, int nPage)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_backup_step_64(backup, nPage);
      }
      return sqlite3_backup_step_32(backup, nPage);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_backup_step", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_backup_step_32(IntPtr backup, int nPage);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_backup_step", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_backup_step_64(IntPtr backup, int nPage);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_backup_step(IntPtr backup, int nPage);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_backup_remaining(IntPtr backup)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_backup_remaining_64(backup);
      }
      return sqlite3_backup_remaining_32(backup);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_backup_remaining", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_backup_remaining_32(IntPtr backup);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_backup_remaining", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_backup_remaining_64(IntPtr backup);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_backup_remaining(IntPtr backup);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_backup_pagecount(IntPtr backup)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_backup_pagecount_64(backup);
      }
      return sqlite3_backup_pagecount_32(backup);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_backup_pagecount", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_backup_pagecount_32(IntPtr backup);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_backup_pagecount", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_backup_pagecount_64(IntPtr backup);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_backup_pagecount(IntPtr backup);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_blob_close(IntPtr blob)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_blob_close_64(blob);
      }
      return sqlite3_blob_close_32(blob);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_blob_close", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_blob_close_32(IntPtr blob);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_blob_close", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_blob_close_64(IntPtr blob);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_blob_close(IntPtr blob);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3_blob_bytes(IntPtr blob)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_blob_bytes_64(blob);
      }
      return sqlite3_blob_bytes_32(blob);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_blob_bytes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_blob_bytes_32(IntPtr blob);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_blob_bytes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3_blob_bytes_64(IntPtr blob);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3_blob_bytes(IntPtr blob);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_blob_open(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, long rowId, int flags, ref IntPtr ptrBlob)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_blob_open_64(db, dbName, tblName, colName, rowId, flags, ref ptrBlob);
      }
      return sqlite3_blob_open_32(db, dbName, tblName, colName, rowId, flags, ref ptrBlob);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_blob_open", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_blob_open_32(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, long rowId, int flags, ref IntPtr ptrBlob);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_blob_open", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_blob_open_64(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, long rowId, int flags, ref IntPtr ptrBlob);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_blob_open(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, long rowId, int flags, ref IntPtr ptrBlob);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_blob_read(IntPtr blob, byte[] buffer, int count, int offset)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_blob_read_64(blob, buffer, count, offset);
      }
      return sqlite3_blob_read_32(blob, buffer, count, offset);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_blob_read", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_blob_read_32(IntPtr blob, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int count, int offset);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_blob_read", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_blob_read_64(IntPtr blob, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int count, int offset);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_blob_read(IntPtr blob, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int count, int offset);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_blob_reopen(IntPtr blob, long rowId)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_blob_reopen_64(blob, rowId);
      }
      return sqlite3_blob_reopen_32(blob, rowId);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_blob_reopen", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_blob_reopen_32(IntPtr blob, long rowId);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_blob_reopen", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_blob_reopen_64(IntPtr blob, long rowId);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_blob_reopen(IntPtr blob, long rowId);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_blob_write(IntPtr blob, byte[] buffer, int count, int offset)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_blob_write_64(blob, buffer, count, offset);
      }
      return sqlite3_blob_write_32(blob, buffer, count, offset);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_blob_write", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_blob_write_32(IntPtr blob, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int count, int offset);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_blob_write", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_blob_write_64(IntPtr blob, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int count, int offset);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_blob_write(IntPtr blob, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int count, int offset);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3_declare_vtab(IntPtr db, IntPtr zSQL)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_declare_vtab_64(db, zSQL);
      }
      return sqlite3_declare_vtab_32(db, zSQL);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_declare_vtab", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_declare_vtab_32(IntPtr db, IntPtr zSQL);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_declare_vtab", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3_declare_vtab_64(IntPtr db, IntPtr zSQL);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3_declare_vtab(IntPtr db, IntPtr zSQL);
#endif

#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_mprintf(IntPtr format, __arglist)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_mprintf_64(format, __arglist(__arglist));
      }
      return sqlite3_mprintf_32(format, __arglist(__arglist));
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_mprintf", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_mprintf_32(IntPtr format, __arglist);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_mprintf", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_mprintf_64(IntPtr format, __arglist);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_mprintf(IntPtr format, __arglist);
#endif
    #endregion

    ///////////////////////////////////////////////////////////////////////////

    // SQLite API calls that are provided by "well-known" extensions that may be statically
    // linked with the SQLite core native library currently in use.
    #region extension sqlite api calls
    #region virtual table
#if INTEROP_VIRTUAL_TABLE
#if USE_HTC_INTEROP_DLL
    internal static IntPtr sqlite3_create_disposable_module(IntPtr db, IntPtr name, ref sqlite3_module module, IntPtr pClientData, xDestroyModule xDestroy)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3_create_disposable_module_64(db, name, ref module, pClientData, xDestroy);
      }
      return sqlite3_create_disposable_module_32(db, name, ref module, pClientData, xDestroy);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_create_disposable_module", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_create_disposable_module_32(IntPtr db, IntPtr name, ref sqlite3_module module, IntPtr pClientData, xDestroyModule xDestroy);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_create_disposable_module", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr sqlite3_create_disposable_module_64(IntPtr db, IntPtr name, ref sqlite3_module module, IntPtr pClientData, xDestroyModule xDestroy);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern IntPtr sqlite3_create_disposable_module(IntPtr db, IntPtr name, ref sqlite3_module module, IntPtr pClientData, xDestroyModule xDestroy);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3_dispose_module(IntPtr pModule)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3_dispose_module_64(pModule);
      }
      else
      {
        sqlite3_dispose_module_32(pModule);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3_dispose_module", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_dispose_module_32(IntPtr pModule);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3_dispose_module", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3_dispose_module_64(IntPtr pModule);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3_dispose_module(IntPtr pModule);
#endif
#endif
    #endregion

    ///////////////////////////////////////////////////////////////////////////

    #region session extension
#if INTEROP_SESSION_EXTENSION
#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    internal delegate int xSessionFilter(IntPtr context, IntPtr pTblName);

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    internal delegate SQLiteChangeSetConflictResult xSessionConflict(IntPtr context, SQLiteChangeSetConflictType type, IntPtr iterator);

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    internal delegate SQLiteErrorCode xSessionInput(IntPtr context, IntPtr pData, ref int nData);

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    internal delegate SQLiteErrorCode xSessionOutput(IntPtr context, IntPtr pData, int nData);

    ///////////////////////////////////////////////////////////////////////////

#if USE_HTC_INTEROP_DLL
    internal static long sqlite3session_memory_used(IntPtr session)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_memory_used_64(session);
      }
      return sqlite3session_memory_used_32(session);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_memory_used", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3session_memory_used_32(IntPtr session);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_memory_used", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long sqlite3session_memory_used_64(IntPtr session);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern long sqlite3session_memory_used(IntPtr session);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3session_create(IntPtr db, byte[] dbName, ref IntPtr session)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_create_64(db, dbName, ref session);
      }
      return sqlite3session_create_32(db, dbName, ref session);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_create", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_create_32(IntPtr db, byte[] dbName, ref IntPtr session);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_create", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_create_64(IntPtr db, byte[] dbName, ref IntPtr session);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3session_create(IntPtr db, byte[] dbName, ref IntPtr session);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3session_delete(IntPtr session)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3session_delete_64(session);
      }
      else
      {
        sqlite3session_delete_32(session);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_delete", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3session_delete_32(IntPtr session);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_delete", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3session_delete_64(IntPtr session);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3session_delete(IntPtr session);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3session_enable(IntPtr session, int enable)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_enable_64(session, enable);
      }
      return sqlite3session_enable_32(session, enable);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_enable", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3session_enable_32(IntPtr session, int enable);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_enable", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3session_enable_64(IntPtr session, int enable);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3session_enable(IntPtr session, int enable);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3session_indirect(IntPtr session, int indirect)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_indirect_64(session, indirect);
      }
      return sqlite3session_indirect_32(session, indirect);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_indirect", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3session_indirect_32(IntPtr session, int indirect);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_indirect", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3session_indirect_64(IntPtr session, int indirect);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3session_indirect(IntPtr session, int indirect);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3session_attach(IntPtr session, byte[] tblName)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_attach_64(session, tblName);
      }
      return sqlite3session_attach_32(session, tblName);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_attach", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_attach_32(IntPtr session, byte[] tblName);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_attach", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_attach_64(IntPtr session, byte[] tblName);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3session_attach(IntPtr session, byte[] tblName);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3session_table_filter(IntPtr session, xSessionFilter xFilter, IntPtr context)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3session_table_filter_64(session, xFilter, context);
      }
      else
      {
        sqlite3session_table_filter_32(session, xFilter, context);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_table_filter", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3session_table_filter_32(IntPtr session, xSessionFilter xFilter, IntPtr context);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_table_filter", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3session_table_filter_64(IntPtr session, xSessionFilter xFilter, IntPtr context);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3session_table_filter(IntPtr session, xSessionFilter xFilter, IntPtr context);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3session_changeset(IntPtr session, ref int nChangeSet, ref IntPtr pChangeSet)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_changeset_64(session, ref nChangeSet, ref pChangeSet);
      }
      return sqlite3session_changeset_32(session, ref nChangeSet, ref pChangeSet);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_changeset", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_changeset_32(IntPtr session, ref int nChangeSet, ref IntPtr pChangeSet);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_changeset", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_changeset_64(IntPtr session, ref int nChangeSet, ref IntPtr pChangeSet);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3session_changeset(IntPtr session, ref int nChangeSet, ref IntPtr pChangeSet);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3session_diff(IntPtr session, byte[] fromDbName, byte[] tblName, ref IntPtr errMsg)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_diff_64(session, fromDbName, tblName, ref errMsg);
      }
      return sqlite3session_diff_32(session, fromDbName, tblName, ref errMsg);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_diff", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_diff_32(IntPtr session, byte[] fromDbName, byte[] tblName, ref IntPtr errMsg);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_diff", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_diff_64(IntPtr session, byte[] fromDbName, byte[] tblName, ref IntPtr errMsg);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3session_diff(IntPtr session, byte[] fromDbName, byte[] tblName, ref IntPtr errMsg);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3session_patchset(IntPtr session, ref int nPatchSet, ref IntPtr pPatchSet)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_patchset_64(session, ref nPatchSet, ref pPatchSet);
      }
      return sqlite3session_patchset_32(session, ref nPatchSet, ref pPatchSet);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_patchset", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_patchset_32(IntPtr session, ref int nPatchSet, ref IntPtr pPatchSet);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_patchset", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_patchset_64(IntPtr session, ref int nPatchSet, ref IntPtr pPatchSet);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3session_patchset(IntPtr session, ref int nPatchSet, ref IntPtr pPatchSet);
#endif

#if USE_HTC_INTEROP_DLL
    internal static int sqlite3session_isempty(IntPtr session)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_isempty_64(session);
      }
      return sqlite3session_isempty_32(session);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_isempty", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3session_isempty_32(IntPtr session);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_isempty", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sqlite3session_isempty_64(IntPtr session);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern int sqlite3session_isempty(IntPtr session);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_start(ref IntPtr iterator, int nChangeSet, IntPtr pChangeSet)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_start_64(ref iterator, nChangeSet, pChangeSet);
      }
      return sqlite3changeset_start_32(ref iterator, nChangeSet, pChangeSet);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_start", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_start_32(ref IntPtr iterator, int nChangeSet, IntPtr pChangeSet);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_start", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_start_64(ref IntPtr iterator, int nChangeSet, IntPtr pChangeSet);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_start(ref IntPtr iterator, int nChangeSet, IntPtr pChangeSet);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_start_v2(ref IntPtr iterator, int nChangeSet, IntPtr pChangeSet, SQLiteChangeSetStartFlags flags)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_start_v2_64(ref iterator, nChangeSet, pChangeSet, flags);
      }
      return sqlite3changeset_start_v2_32(ref iterator, nChangeSet, pChangeSet, flags);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_start_v2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_start_v2_32(ref IntPtr iterator, int nChangeSet, IntPtr pChangeSet, SQLiteChangeSetStartFlags flags);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_start_v2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_start_v2_64(ref IntPtr iterator, int nChangeSet, IntPtr pChangeSet, SQLiteChangeSetStartFlags flags);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_start_v2(ref IntPtr iterator, int nChangeSet, IntPtr pChangeSet, SQLiteChangeSetStartFlags flags);
#endif

#if USE_HTC_INTEROP_DLL
        internal static SQLiteErrorCode sqlite3changeset_next(IntPtr iterator)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_next_64(iterator);
      }
      return sqlite3changeset_next_32(iterator);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_next", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_next_32(IntPtr iterator);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_next", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_next_64(IntPtr iterator);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_next(IntPtr iterator);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_op(IntPtr iterator, ref IntPtr pTblName, ref int nColumns, ref SQLiteAuthorizerActionCode op, ref int bIndirect)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_op_64(iterator, ref pTblName, ref nColumns, ref op, ref bIndirect);
      }
      return sqlite3changeset_op_32(iterator, ref pTblName, ref nColumns, ref op, ref bIndirect);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_op", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_op_32(IntPtr iterator, ref IntPtr pTblName, ref int nColumns, ref SQLiteAuthorizerActionCode op, ref int bIndirect);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_op", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_op_64(IntPtr iterator, ref IntPtr pTblName, ref int nColumns, ref SQLiteAuthorizerActionCode op, ref int bIndirect);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_op(IntPtr iterator, ref IntPtr pTblName, ref int nColumns, ref SQLiteAuthorizerActionCode op, ref int bIndirect);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_pk(IntPtr iterator, ref IntPtr pPrimaryKeys, ref int nColumns)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_pk_64(iterator, ref pPrimaryKeys, ref nColumns);
      }
      return sqlite3changeset_pk_32(iterator, ref pPrimaryKeys, ref nColumns);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_pk", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_pk_32(IntPtr iterator, ref IntPtr pPrimaryKeys, ref int nColumns);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_pk", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_pk_64(IntPtr iterator, ref IntPtr pPrimaryKeys, ref int nColumns);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_pk(IntPtr iterator, ref IntPtr pPrimaryKeys, ref int nColumns);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_old(IntPtr iterator, int columnIndex, ref IntPtr pValue)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_old_64(iterator, columnIndex, ref pValue);
      }
      return sqlite3changeset_old_32(iterator, columnIndex, ref pValue);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_old", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_old_32(IntPtr iterator, int columnIndex, ref IntPtr pValue);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_old", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_old_64(IntPtr iterator, int columnIndex, ref IntPtr pValue);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_old(IntPtr iterator, int columnIndex, ref IntPtr pValue);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_new(IntPtr iterator, int columnIndex, ref IntPtr pValue)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_new_64(iterator, columnIndex, ref pValue);
      }
      return sqlite3changeset_new_32(iterator, columnIndex, ref pValue);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_new", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_new_32(IntPtr iterator, int columnIndex, ref IntPtr pValue);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_new", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_new_64(IntPtr iterator, int columnIndex, ref IntPtr pValue);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_new(IntPtr iterator, int columnIndex, ref IntPtr pValue);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_conflict(IntPtr iterator, int columnIndex, ref IntPtr pValue)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_conflict_64(iterator, columnIndex, ref pValue);
      }
      return sqlite3changeset_conflict_32(iterator, columnIndex, ref pValue);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_conflict", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_conflict_32(IntPtr iterator, int columnIndex, ref IntPtr pValue);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_conflict", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_conflict_64(IntPtr iterator, int columnIndex, ref IntPtr pValue);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_conflict(IntPtr iterator, int columnIndex, ref IntPtr pValue);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_fk_conflicts(IntPtr iterator, ref int conflicts)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_fk_conflicts_64(iterator, ref conflicts);
      }
      return sqlite3changeset_fk_conflicts_32(iterator, ref conflicts);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_fk_conflicts", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_fk_conflicts_32(IntPtr iterator, ref int conflicts);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_fk_conflicts", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_fk_conflicts_64(IntPtr iterator, ref int conflicts);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_fk_conflicts(IntPtr iterator, ref int conflicts);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_finalize(IntPtr iterator)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_finalize_64(iterator);
      }
      return sqlite3changeset_finalize_32(iterator);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_finalize", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_finalize_32(IntPtr iterator);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_finalize", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_finalize_64(IntPtr iterator);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_finalize(IntPtr iterator);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_invert(int nIn, IntPtr pIn, ref int nOut, ref IntPtr pOut)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_invert_64(nIn, pIn, ref nOut, ref pOut);
      }
      return sqlite3changeset_invert_32(nIn, pIn, ref nOut, ref pOut);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_invert", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_invert_32(int nIn, IntPtr pIn, ref int nOut, ref IntPtr pOut);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_invert", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_invert_64(int nIn, IntPtr pIn, ref int nOut, ref IntPtr pOut);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_invert(int nIn, IntPtr pIn, ref int nOut, ref IntPtr pOut);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_concat(int nA, IntPtr pA, int nB, IntPtr pB, ref int nOut, ref IntPtr pOut)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_concat_64(nA, pA, nB, pB, ref nOut, ref pOut);
      }
      return sqlite3changeset_concat_32(nA, pA, nB, pB, ref nOut, ref pOut);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_concat", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_concat_32(int nA, IntPtr pA, int nB, IntPtr pB, ref int nOut, ref IntPtr pOut);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_concat", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_concat_64(int nA, IntPtr pA, int nB, IntPtr pB, ref int nOut, ref IntPtr pOut);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_concat(int nA, IntPtr pA, int nB, IntPtr pB, ref int nOut, ref IntPtr pOut);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changegroup_new(ref IntPtr changeGroup)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changegroup_new_64(ref changeGroup);
      }
      return sqlite3changegroup_new_32(ref changeGroup);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changegroup_new", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changegroup_new_32(ref IntPtr changeGroup);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changegroup_new", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changegroup_new_64(ref IntPtr changeGroup);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changegroup_new(ref IntPtr changeGroup);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changegroup_add(IntPtr changeGroup, int nData, IntPtr pData)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changegroup_add_64(changeGroup, nData, pData);
      }
      return sqlite3changegroup_add_32(changeGroup, nData, pData);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changegroup_add", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changegroup_add_32(IntPtr changeGroup, int nData, IntPtr pData);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changegroup_add", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changegroup_add_64(IntPtr changeGroup, int nData, IntPtr pData);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changegroup_add(IntPtr changeGroup, int nData, IntPtr pData);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changegroup_output(IntPtr changeGroup, ref int nData, ref IntPtr pData)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changegroup_output_64(changeGroup, ref nData, ref pData);
      }
      return sqlite3changegroup_output_32(changeGroup, ref nData, ref pData);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changegroup_output", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changegroup_output_32(IntPtr changeGroup, ref int nData, ref IntPtr pData);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changegroup_output", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changegroup_output_64(IntPtr changeGroup, ref int nData, ref IntPtr pData);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changegroup_output(IntPtr changeGroup, ref int nData, ref IntPtr pData);
#endif

#if USE_HTC_INTEROP_DLL
    internal static void sqlite3changegroup_delete(IntPtr changeGroup)
    {
      if (IntPtr.Size == 8)
      {
        sqlite3changegroup_delete_64(changeGroup);
      }
      else
      {
        sqlite3changegroup_delete_32(changeGroup);
      }
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changegroup_delete", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3changegroup_delete_32(IntPtr changeGroup);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changegroup_delete", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sqlite3changegroup_delete_64(IntPtr changeGroup);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern void sqlite3changegroup_delete(IntPtr changeGroup);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_apply(IntPtr db, int nChangeSet, IntPtr pChangeSet, xSessionFilter xFilter, xSessionConflict xConflict, IntPtr context)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_apply_64(db, nChangeSet, pChangeSet, xFilter, xConflict, context);
      }
      return sqlite3changeset_apply_32(db, nChangeSet, pChangeSet, xFilter, xConflict, context);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_apply", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_apply_32(IntPtr db, int nChangeSet, IntPtr pChangeSet, xSessionFilter xFilter, xSessionConflict xConflict, IntPtr context);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_apply", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_apply_64(IntPtr db, int nChangeSet, IntPtr pChangeSet, xSessionFilter xFilter, xSessionConflict xConflict, IntPtr context);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_apply(IntPtr db, int nChangeSet, IntPtr pChangeSet, xSessionFilter xFilter, xSessionConflict xConflict, IntPtr context);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_apply_strm(IntPtr db, xSessionInput xInput, IntPtr pIn, xSessionFilter xFilter, xSessionConflict xConflict, IntPtr context)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_apply_strm_64(db, xInput, pIn, xFilter, xConflict, context);
      }
      return sqlite3changeset_apply_strm_32(db, xInput, pIn, xFilter, xConflict, context);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_apply_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_apply_strm_32(IntPtr db, xSessionInput xInput, IntPtr pIn, xSessionFilter xFilter, xSessionConflict xConflict, IntPtr context);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_apply_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_apply_strm_64(IntPtr db, xSessionInput xInput, IntPtr pIn, xSessionFilter xFilter, xSessionConflict xConflict, IntPtr context);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_apply_strm(IntPtr db, xSessionInput xInput, IntPtr pIn, xSessionFilter xFilter, xSessionConflict xConflict, IntPtr context);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_concat_strm(xSessionInput xInputA, IntPtr pInA, xSessionInput xInputB, IntPtr pInB, xSessionOutput xOutput, IntPtr pOut)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_concat_strm_64(xInputA, pInA, xInputB, pInB, xOutput, pOut);
      }
      return sqlite3changeset_concat_strm_32(xInputA, pInA, xInputB, pInB, xOutput, pOut);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_concat_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_concat_strm_32(xSessionInput xInputA, IntPtr pInA, xSessionInput xInputB, IntPtr pInB, xSessionOutput xOutput, IntPtr pOut);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_concat_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_concat_strm_64(xSessionInput xInputA, IntPtr pInA, xSessionInput xInputB, IntPtr pInB, xSessionOutput xOutput, IntPtr pOut);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_concat_strm(xSessionInput xInputA, IntPtr pInA, xSessionInput xInputB, IntPtr pInB, xSessionOutput xOutput, IntPtr pOut);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_invert_strm(xSessionInput xInput, IntPtr pIn, xSessionOutput xOutput, IntPtr pOut)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_invert_strm_64(xInput, pIn, xOutput, pOut);
      }
      return sqlite3changeset_invert_strm_32(xInput, pIn, xOutput, pOut);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_invert_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_invert_strm_32(xSessionInput xInput, IntPtr pIn, xSessionOutput xOutput, IntPtr pOut);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_invert_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_invert_strm_64(xSessionInput xInput, IntPtr pIn, xSessionOutput xOutput, IntPtr pOut);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_invert_strm(xSessionInput xInput, IntPtr pIn, xSessionOutput xOutput, IntPtr pOut);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_start_strm(ref IntPtr iterator, xSessionInput xInput, IntPtr pIn)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_start_strm_64(ref iterator, xInput, pIn);
      }
      return sqlite3changeset_start_strm_32(ref iterator, xInput, pIn);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_start_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_start_strm_32(ref IntPtr iterator, xSessionInput xInput, IntPtr pIn);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_start_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_start_strm_64(ref IntPtr iterator, xSessionInput xInput, IntPtr pIn);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_start_strm(ref IntPtr iterator, xSessionInput xInput, IntPtr pIn);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changeset_start_v2_strm(ref IntPtr iterator, xSessionInput xInput, IntPtr pIn, SQLiteChangeSetStartFlags flags)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changeset_start_v2_strm_64(ref iterator, xInput, pIn, flags);
      }
      return sqlite3changeset_start_v2_strm_32(ref iterator, xInput, pIn, flags);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changeset_start_v2_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_start_v2_strm_32(ref IntPtr iterator, xSessionInput xInput, IntPtr pIn, SQLiteChangeSetStartFlags flags);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changeset_start_v2_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changeset_start_v2_strm_64(ref IntPtr iterator, xSessionInput xInput, IntPtr pIn, SQLiteChangeSetStartFlags flags);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changeset_start_v2_strm(ref IntPtr iterator, xSessionInput xInput, IntPtr pIn, SQLiteChangeSetStartFlags flags);
#endif

#if USE_HTC_INTEROP_DLL
        internal static SQLiteErrorCode sqlite3session_changeset_strm(IntPtr session, xSessionOutput xOutput, IntPtr pOut)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_changeset_strm_64(session, xOutput, pOut);
      }
      return sqlite3session_changeset_strm_32(session, xOutput, pOut);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_changeset_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_changeset_strm_32(IntPtr session, xSessionOutput xOutput, IntPtr pOut);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_changeset_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_changeset_strm_64(IntPtr session, xSessionOutput xOutput, IntPtr pOut);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3session_changeset_strm(IntPtr session, xSessionOutput xOutput, IntPtr pOut);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3session_patchset_strm(IntPtr session, xSessionOutput xOutput, IntPtr pOut)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3session_patchset_strm_64(session, xOutput, pOut);
      }
      return sqlite3session_patchset_strm_32(session, xOutput, pOut);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3session_patchset_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_patchset_strm_32(IntPtr session, xSessionOutput xOutput, IntPtr pOut);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3session_patchset_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3session_patchset_strm_64(IntPtr session, xSessionOutput xOutput, IntPtr pOut);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3session_patchset_strm(IntPtr session, xSessionOutput xOutput, IntPtr pOut);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changegroup_add_strm(IntPtr changeGroup, xSessionInput xInput, IntPtr pIn)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changegroup_add_strm_64(changeGroup, xInput, pIn);
      }
      return sqlite3changegroup_add_strm_32(changeGroup, xInput, pIn);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changegroup_add_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changegroup_add_strm_32(IntPtr changeGroup, xSessionInput xInput, IntPtr pIn);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changegroup_add_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changegroup_add_strm_64(IntPtr changeGroup, xSessionInput xInput, IntPtr pIn);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changegroup_add_strm(IntPtr changeGroup, xSessionInput xInput, IntPtr pIn);
#endif

#if USE_HTC_INTEROP_DLL
    internal static SQLiteErrorCode sqlite3changegroup_output_strm(IntPtr changeGroup, xSessionOutput xOutput, IntPtr pOut)
    {
      if (IntPtr.Size == 8)
      {
        return sqlite3changegroup_output_strm_64(changeGroup, xOutput, pOut);
      }
      return sqlite3changegroup_output_strm_32(changeGroup, xOutput, pOut);
    }

    [DllImport(SQLITE_DLL32, EntryPoint = "sqlite3changegroup_output_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changegroup_output_strm_32(IntPtr changeGroup, xSessionOutput xOutput, IntPtr pOut);

    [DllImport(SQLITE_DLL64, EntryPoint = "sqlite3changegroup_output_strm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SQLiteErrorCode sqlite3changegroup_output_strm_64(IntPtr changeGroup, xSessionOutput xOutput, IntPtr pOut);
#else
#if !PLATFORM_COMPACTFRAMEWORK
    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport(SQLITE_DLL)]
#endif
    internal static extern SQLiteErrorCode sqlite3changegroup_output_strm(IntPtr changeGroup, xSessionOutput xOutput, IntPtr pOut);
#endif
#endif
    #endregion
    #endregion

    ///////////////////////////////////////////////////////////////////////////

    #region sqlite interop api calls (.NET Compact Framework only)
#if PLATFORM_COMPACTFRAMEWORK && !SQLITE_STANDARD
    [DllImport(SQLITE_DLL)]
    internal static extern void sqlite3_last_insert_rowid_interop(IntPtr db, ref long rowId);

    [DllImport(SQLITE_DLL)]
    internal static extern void sqlite3_memory_used_interop(ref long bytes);

    [DllImport(SQLITE_DLL)]
    internal static extern void sqlite3_memory_highwater_interop(int resetFlag, ref long bytes);

    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_bind_double_interop(IntPtr stmt, int index, ref double value);

    [DllImport(SQLITE_DLL)]
    internal static extern SQLiteErrorCode sqlite3_bind_int64_interop(IntPtr stmt, int index, ref long value);

    [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_bind_int64_interop")]
    internal static extern SQLiteErrorCode sqlite3_bind_uint64_interop(IntPtr stmt, int index, ref ulong value);

    [DllImport(SQLITE_DLL)]
    internal static extern void sqlite3_column_double_interop(IntPtr stmt, int index, ref double value);

    [DllImport(SQLITE_DLL)]
    internal static extern void sqlite3_column_int64_interop(IntPtr stmt, int index, ref long value);

    [DllImport(SQLITE_DLL)]
    internal static extern void sqlite3_value_double_interop(IntPtr p, ref double value);

    [DllImport(SQLITE_DLL)]
    internal static extern void sqlite3_value_int64_interop(IntPtr p, ref Int64 value);

    [DllImport(SQLITE_DLL)]
    internal static extern void sqlite3_result_double_interop(IntPtr context, ref double value);

    [DllImport(SQLITE_DLL)]
    internal static extern void sqlite3_result_int64_interop(IntPtr context, ref Int64 value);

    [DllImport(SQLITE_DLL)]
    internal static extern void sqlite3_msize_interop(IntPtr p, ref ulong size);

    [DllImport(SQLITE_DLL)]
    internal static extern IntPtr sqlite3_create_disposable_module_interop(
        IntPtr db, IntPtr name, IntPtr pModule, int iVersion, xCreate xCreate,
        xConnect xConnect, xBestIndex xBestIndex, xDisconnect xDisconnect,
        xDestroy xDestroy, xOpen xOpen, xClose xClose, xFilter xFilter,
        xNext xNext, xEof xEof, xColumn xColumn, xRowId xRowId, xUpdate xUpdate,
        xBegin xBegin, xSync xSync, xCommit xCommit, xRollback xRollback,
        xFindFunction xFindFunction, xRename xRename, xSavepoint xSavepoint,
        xRelease xRelease, xRollbackTo xRollbackTo, IntPtr pClientData,
        xDestroyModule xDestroyModule);
#endif
    // PLATFORM_COMPACTFRAMEWORK && !SQLITE_STANDARD
    #endregion

    ///////////////////////////////////////////////////////////////////////////

    #region Native Delegates
#if INTEROP_VIRTUAL_TABLE
#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xCreate(
        IntPtr pDb,
        IntPtr pAux,
        int argc,
        IntPtr argv,
        ref IntPtr pVtab,
        ref IntPtr pError
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xConnect(
        IntPtr pDb,
        IntPtr pAux,
        int argc,
        IntPtr argv,
        ref IntPtr pVtab,
        ref IntPtr pError
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xBestIndex(
        IntPtr pVtab,
        IntPtr pIndex
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xDisconnect(
        IntPtr pVtab
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xDestroy(
        IntPtr pVtab
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xOpen(
        IntPtr pVtab,
        ref IntPtr pCursor
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xClose(
        IntPtr pCursor
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xFilter(
        IntPtr pCursor,
        int idxNum,
        IntPtr idxStr,
        int argc,
        IntPtr argv
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xNext(
        IntPtr pCursor
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate int xEof(
        IntPtr pCursor
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xColumn(
        IntPtr pCursor,
        IntPtr pContext,
        int index
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xRowId(
        IntPtr pCursor,
        ref long rowId
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xUpdate(
        IntPtr pVtab,
        int argc,
        IntPtr argv,
        ref long rowId
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xBegin(
        IntPtr pVtab
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xSync(
        IntPtr pVtab
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xCommit(
        IntPtr pVtab
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xRollback(
        IntPtr pVtab
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate int xFindFunction(
        IntPtr pVtab,
        int nArg,
        IntPtr zName,
        ref SQLiteCallback callback,
        ref IntPtr pUserData
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xRename(
        IntPtr pVtab,
        IntPtr zNew
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xSavepoint(
        IntPtr pVtab,
        int iSavepoint
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xRelease(
        IntPtr pVtab,
        int iSavepoint
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate SQLiteErrorCode xRollbackTo(
        IntPtr pVtab,
        int iSavepoint
    );

    ///////////////////////////////////////////////////////////////////////////

#if !PLATFORM_COMPACTFRAMEWORK
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
    public delegate void xDestroyModule(IntPtr pClientData);
#endif
    #endregion

    ///////////////////////////////////////////////////////////////////////////

    #region Native Structures
#if INTEROP_VIRTUAL_TABLE
    [StructLayout(LayoutKind.Sequential)]
    internal struct sqlite3_module
    {
        /*   0 */ public int iVersion;
        /*   8 */ public xCreate xCreate;
        /*  16 */ public xConnect xConnect;
        /*  24 */ public xBestIndex xBestIndex;
        /*  32 */ public xDisconnect xDisconnect;
        /*  40 */ public xDestroy xDestroy;
        /*  48 */ public xOpen xOpen;
        /*  56 */ public xClose xClose;
        /*  64 */ public xFilter xFilter;
        /*  72 */ public xNext xNext;
        /*  80 */ public xEof xEof;
        /*  88 */ public xColumn xColumn;
        /*  96 */ public xRowId xRowId;
        /* 104 */ public xUpdate xUpdate;
        /* 112 */ public xBegin xBegin;
        /* 120 */ public xSync xSync;
        /* 128 */ public xCommit xCommit;
        /* 136 */ public xRollback xRollback;
        /* 144 */ public xFindFunction xFindFunction;
        /* 152 */ public xRename xRename;
        /* The methods above are in version 1 of the sqlite3_module
         * object.  Those below are for version 2 and greater. */
        /* 160 */ public xSavepoint xSavepoint;
        /* 168 */ public xRelease xRelease;
        /* 176 */ public xRollbackTo xRollbackTo;
    }

    ///////////////////////////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential)]
    internal struct sqlite3_vtab
    {
        /*  0 */ public IntPtr pModule;
        /*  8 */ public int nRef; /* NO LONGER USED */
        /* 16 */ public IntPtr zErrMsg;
    }

    ///////////////////////////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential)]
    internal struct sqlite3_vtab_cursor
    {
        /* 0 */ public IntPtr pVTab;
    }

    ///////////////////////////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential)]
    internal struct sqlite3_index_constraint
    {
        public sqlite3_index_constraint(
            SQLiteIndexConstraint constraint
            )
            : this()
        {
            if (constraint != null)
            {
                iColumn = constraint.iColumn;
                op = constraint.op;
                usable = constraint.usable;
                iTermOffset = constraint.iTermOffset;
            }
        }

        ///////////////////////////////////////////////////////////////////////

        /* 0 */ public int iColumn;
        /* 4 */ public SQLiteIndexConstraintOp op;
        /* 5 */ public byte usable;
        /* 8 */ public int iTermOffset;
    }

    ///////////////////////////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential)]
    internal struct sqlite3_index_orderby
    {
        public sqlite3_index_orderby(
            SQLiteIndexOrderBy orderBy
            )
            : this()
        {
            if (orderBy != null)
            {
                iColumn = orderBy.iColumn;
                desc = orderBy.desc;
            }
        }

        ///////////////////////////////////////////////////////////////////////

        /* 0 */ public int iColumn; /* Column number */
        /* 4 */ public byte desc;   /* True for DESC.  False for ASC. */
    }

    ///////////////////////////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential)]
    internal struct sqlite3_index_constraint_usage
    {
        public sqlite3_index_constraint_usage(
            SQLiteIndexConstraintUsage constraintUsage
            )
            : this()
        {
            if (constraintUsage != null)
            {
                argvIndex = constraintUsage.argvIndex;
                omit = constraintUsage.omit;
            }
        }

        ///////////////////////////////////////////////////////////////////////

        public int argvIndex; /* if >0, constraint is part of argv to xFilter */
        public byte omit;     /* Do not code a test for this constraint */
    }

    ///////////////////////////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential)]
    internal struct sqlite3_index_info
    {
        /* Inputs */
        /*  0 */ public int nConstraint; /* Number of entries in aConstraint */
        /*  8 */ public IntPtr aConstraint;
        /* 16 */ public int nOrderBy;    /* Number of entries in aOrderBy */
        /* 24 */ public IntPtr aOrderBy;
        /* Outputs */
        /* 32 */ public IntPtr aConstraintUsage;
        /* 40 */ public int idxNum;           /* Number used to identify the index */
        /* 48 */ public string idxStr;        /* String, possibly obtained from sqlite3_malloc */
        /* 56 */ public int needToFreeIdxStr; /* Free idxStr using sqlite3_free() if true */
        /* 60 */ public int orderByConsumed;  /* True if output is already ordered */
        /* 64 */ public double estimatedCost; /* Estimated cost of using this index */
        /* 72 */ public long estimatedRows;   /* Estimated number of rows returned */
        /* 80 */ public SQLiteIndexFlags idxFlags; /* Mask of SQLITE_INDEX_SCAN_* flags */
        /* 88 */ public long colUsed;         /* Input: Mask of columns used by statement */
    }
#endif
    #endregion
  }
  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region .NET Compact Framework (only) CriticalHandle Class
#if PLATFORM_COMPACTFRAMEWORK
  internal abstract class CriticalHandle : IDisposable
  {
    private bool _isClosed;
    protected IntPtr handle;

    protected CriticalHandle(IntPtr invalidHandleValue)
    {
      handle = invalidHandleValue;
      _isClosed = false;
    }

    ~CriticalHandle()
    {
      Dispose(false);
    }

    private void Cleanup()
    {
      if (!IsClosed)
      {
        this._isClosed = true;
        if (!IsInvalid)
        {
          ReleaseHandle();
          GC.SuppressFinalize(this);
        }
      }
    }

    public void Close()
    {
      Dispose(true);
    }

    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      Cleanup();
    }

    protected abstract bool ReleaseHandle();

    protected void SetHandle(IntPtr value)
    {
      handle = value;
    }

    public void SetHandleAsInvalid()
    {
      _isClosed = true;
      GC.SuppressFinalize(this);
    }

    public bool IsClosed
    {
      get { return _isClosed; }
    }

    public abstract bool IsInvalid
    {
      get;
    }

  }
#endif
  #endregion

    ///////////////////////////////////////////////////////////////////////////

    #region SQLiteConnectionHandle Class
    // Handles the unmanaged database pointer, and provides finalization
    // support for it.
    internal sealed class SQLiteConnectionHandle : CriticalHandle
    {
#if SQLITE_STANDARD && !PLATFORM_COMPACTFRAMEWORK
        internal delegate void CloseConnectionCallback(
            SQLiteConnectionHandle hdl, IntPtr db);

        internal static CloseConnectionCallback closeConnection =
            SQLiteBase.CloseConnection;
#endif

        ///////////////////////////////////////////////////////////////////////

#if PLATFORM_COMPACTFRAMEWORK
        internal readonly object syncRoot = new object();
#endif

        ///////////////////////////////////////////////////////////////////////

        private bool ownHandle;

        ///////////////////////////////////////////////////////////////////////

#if INTEROP_LEGACY_CLOSE
        internal int version;
#endif

        ///////////////////////////////////////////////////////////////////////

        public static implicit operator IntPtr(SQLiteConnectionHandle db)
        {
            if (db != null)
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (db.syncRoot)
#endif
                {
                    return db.handle;
                }
            }
            return IntPtr.Zero;
        }

        ///////////////////////////////////////////////////////////////////////

        internal SQLiteConnectionHandle(IntPtr db, bool ownHandle)
            : this(ownHandle)
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                this.ownHandle = ownHandle;
                SetHandle(db);

#if INTEROP_LEGACY_CLOSE
                BumpVersion();
#endif
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private SQLiteConnectionHandle(bool ownHandle)
            : base(IntPtr.Zero)
        {
#if COUNT_HANDLE
            if (ownHandle)
                Interlocked.Increment(ref DebugData.connectionCount);
#endif
        }

        ///////////////////////////////////////////////////////////////////////

#if INTEROP_LEGACY_CLOSE
        private void BumpVersion()
        {
            Interlocked.Increment(ref version);
        }
#endif

        ///////////////////////////////////////////////////////////////////////

        protected override bool ReleaseHandle()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                if (!ownHandle) return true;
            }

            try
            {
#if INTEROP_LEGACY_CLOSE
                BumpVersion();
#endif

#if !PLATFORM_COMPACTFRAMEWORK
                IntPtr localHandle = Interlocked.Exchange(
                    ref handle, IntPtr.Zero);

#if SQLITE_STANDARD
                if (localHandle != IntPtr.Zero)
                    closeConnection(this, localHandle);
#else
                if (localHandle != IntPtr.Zero)
                    SQLiteBase.CloseConnection(this, localHandle);
#endif

#if !NET_COMPACT_20 && TRACE_HANDLE
                try
                {
                    Trace.WriteLine(HelperMethods.StringFormat(
                        CultureInfo.CurrentCulture,
                        "CloseConnection: {0}", localHandle)); /* throw */
                }
                catch
                {
                }
#endif
#else
                lock (syncRoot)
                {
                    if (handle != IntPtr.Zero)
                    {
                        SQLiteBase.CloseConnection(this, handle);
                        SetHandle(IntPtr.Zero);
                    }
                }
#endif
#if COUNT_HANDLE
                Interlocked.Decrement(ref DebugData.connectionCount);
#endif
#if DEBUG
                return true;
#endif
            }
#if !NET_COMPACT_20 && TRACE_HANDLE
            catch (SQLiteException e)
#else
            catch (SQLiteException)
#endif
            {
#if !NET_COMPACT_20 && TRACE_HANDLE
                try
                {
                    Trace.WriteLine(HelperMethods.StringFormat(
                        CultureInfo.CurrentCulture,
                        "CloseConnection: {0}, exception: {1}",
                        handle, e)); /* throw */
                }
                catch
                {
                }
#endif
            }
            finally
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (syncRoot)
#endif
                {
                    SetHandleAsInvalid();
                }
            }
#if DEBUG
            return false;
#else
            return true;
#endif
        }

        ///////////////////////////////////////////////////////////////////////

#if COUNT_HANDLE
        public int WasReleasedOk()
        {
            return Interlocked.Decrement(ref DebugData.connectionCount);
        }
#endif

        ///////////////////////////////////////////////////////////////////////

        public bool OwnHandle
        {
            get
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (syncRoot)
#endif
                {
                    return ownHandle;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////

        public override bool IsInvalid
        {
            get
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (syncRoot)
#endif
                {
                    return (handle == IntPtr.Zero);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////

#if DEBUG
        public override string ToString()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                return handle.ToString();
            }
        }
#endif
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////

    #region SQLiteStatementHandle Class
    // Provides finalization support for unmanaged SQLite statements.
    internal sealed class SQLiteStatementHandle : CriticalHandle
    {
#if PLATFORM_COMPACTFRAMEWORK
        internal readonly object syncRoot = new object();
#endif

        ///////////////////////////////////////////////////////////////////////

        private SQLiteConnectionHandle cnn;

        ///////////////////////////////////////////////////////////////////////

#if INTEROP_LEGACY_CLOSE
        private int version;
#endif

        ///////////////////////////////////////////////////////////////////////

        public static implicit operator IntPtr(SQLiteStatementHandle stmt)
        {
            if (stmt != null)
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (stmt.syncRoot)
#endif
                {
                    return stmt.handle;
                }
            }
            return IntPtr.Zero;
        }

        ///////////////////////////////////////////////////////////////////////

        internal SQLiteStatementHandle(SQLiteConnectionHandle cnn, IntPtr stmt)
            : this()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                this.cnn = cnn;
                SetHandle(stmt);

#if INTEROP_LEGACY_CLOSE
                SetVersion();
#endif
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private SQLiteStatementHandle()
            : base(IntPtr.Zero)
        {
#if COUNT_HANDLE
            Interlocked.Increment(ref DebugData.statementCount);
#endif
        }

        ///////////////////////////////////////////////////////////////////////

#if INTEROP_LEGACY_CLOSE
        private bool MatchVersion()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                return (cnn != null) ? (version == cnn.version) : false;
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private bool SetVersion()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                if (cnn != null)
                {
                    version = cnn.version;
                    return true;
                }

                return false;
            }
        }
#endif

        ///////////////////////////////////////////////////////////////////////

        protected override bool ReleaseHandle()
        {
            try
            {
#if !PLATFORM_COMPACTFRAMEWORK
                IntPtr localHandle = Interlocked.Exchange(
                    ref handle, IntPtr.Zero);

#if INTEROP_LEGACY_CLOSE
                if (!MatchVersion())
                {
#if !NET_COMPACT_20 && TRACE_HANDLE
                    try
                    {
                        Trace.WriteLine(HelperMethods.StringFormat(
                            CultureInfo.CurrentCulture,
                            "MatchVersion: {0} (statement handle)",
                            localHandle)); /* throw */
                    }
                    catch
                    {
                    }
#endif
#if COUNT_HANDLE
                    Interlocked.Decrement(ref DebugData.statementCount);
#endif
                    return false;
                }
#endif

                if (localHandle != IntPtr.Zero)
                    SQLiteBase.FinalizeStatement(cnn, localHandle);

#if !NET_COMPACT_20 && TRACE_HANDLE
                try
                {
                    Trace.WriteLine(HelperMethods.StringFormat(
                        CultureInfo.CurrentCulture,
                        "FinalizeStatement: {0}", localHandle)); /* throw */
                }
                catch
                {
                }
#endif
#else
                lock (syncRoot)
                {
                    if (handle != IntPtr.Zero)
                    {
                        SQLiteBase.FinalizeStatement(cnn, handle);
                        SetHandle(IntPtr.Zero);
                    }
                }
#endif
#if COUNT_HANDLE
                Interlocked.Decrement(ref DebugData.statementCount);
#endif
#if DEBUG
                return true;
#endif
            }
#if !NET_COMPACT_20 && TRACE_HANDLE
            catch (SQLiteException e)
#else
            catch (SQLiteException)
#endif
            {
#if !NET_COMPACT_20 && TRACE_HANDLE
                try
                {
                    Trace.WriteLine(HelperMethods.StringFormat(
                        CultureInfo.CurrentCulture,
                        "FinalizeStatement: {0}, exception: {1}",
                        handle, e)); /* throw */
                }
                catch
                {
                }
#endif
            }
            finally
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (syncRoot)
#endif
                {
                    SetHandleAsInvalid();
                }
            }
#if DEBUG
            return false;
#else
            return true;
#endif
        }

        ///////////////////////////////////////////////////////////////////////

#if COUNT_HANDLE
        public int WasReleasedOk()
        {
            return Interlocked.Decrement(ref DebugData.statementCount);
        }
#endif

        ///////////////////////////////////////////////////////////////////////

        public override bool IsInvalid
        {
            get
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (syncRoot)
#endif
                {
                    return (handle == IntPtr.Zero);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////

#if DEBUG
        public override string ToString()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                return handle.ToString();
            }
        }
#endif
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////

    #region SQLiteBackupHandle Class
    // Provides finalization support for unmanaged SQLite backup objects.
    internal sealed class SQLiteBackupHandle : CriticalHandle
    {
#if PLATFORM_COMPACTFRAMEWORK
        internal readonly object syncRoot = new object();
#endif

        ///////////////////////////////////////////////////////////////////////

        private SQLiteConnectionHandle cnn;

        ///////////////////////////////////////////////////////////////////////

#if INTEROP_LEGACY_CLOSE
        private int version;
#endif

        ///////////////////////////////////////////////////////////////////////

        public static implicit operator IntPtr(SQLiteBackupHandle backup)
        {
            if (backup != null)
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (backup.syncRoot)
#endif
                {
                    return backup.handle;
                }
            }
            return IntPtr.Zero;
        }

        ///////////////////////////////////////////////////////////////////////

        internal SQLiteBackupHandle(SQLiteConnectionHandle cnn, IntPtr backup)
            : this()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                this.cnn = cnn;
                SetHandle(backup);

#if INTEROP_LEGACY_CLOSE
                SetVersion();
#endif
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private SQLiteBackupHandle()
            : base(IntPtr.Zero)
        {
#if COUNT_HANDLE
            Interlocked.Increment(ref DebugData.backupCount);
#endif
        }

        ///////////////////////////////////////////////////////////////////////

#if INTEROP_LEGACY_CLOSE
        private bool MatchVersion()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                return (cnn != null) ? (version == cnn.version) : false;
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private bool SetVersion()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                if (cnn != null)
                {
                    version = cnn.version;
                    return true;
                }

                return false;
            }
        }
#endif

        ///////////////////////////////////////////////////////////////////////

        protected override bool ReleaseHandle()
        {
            try
            {
#if !PLATFORM_COMPACTFRAMEWORK
                IntPtr localHandle = Interlocked.Exchange(
                    ref handle, IntPtr.Zero);

#if INTEROP_LEGACY_CLOSE
                if (!MatchVersion())
                {
#if !NET_COMPACT_20 && TRACE_HANDLE
                    try
                    {
                        Trace.WriteLine(HelperMethods.StringFormat(
                            CultureInfo.CurrentCulture,
                            "MatchVersion: {0} (backup handle)",
                            localHandle)); /* throw */
                    }
                    catch
                    {
                    }
#endif
#if COUNT_HANDLE
                    Interlocked.Decrement(ref DebugData.backupCount);
#endif
                    return false;
                }
#endif

                if (localHandle != IntPtr.Zero)
                    SQLiteBase.FinishBackup(cnn, localHandle);

#if !NET_COMPACT_20 && TRACE_HANDLE
                try
                {
                    Trace.WriteLine(HelperMethods.StringFormat(
                        CultureInfo.CurrentCulture,
                        "FinishBackup: {0}", localHandle)); /* throw */
                }
                catch
                {
                }
#endif
#else
                lock (syncRoot)
                {
                    if (handle != IntPtr.Zero)
                    {
                        SQLiteBase.FinishBackup(cnn, handle);
                        SetHandle(IntPtr.Zero);
                    }
                }
#endif
#if COUNT_HANDLE
                Interlocked.Decrement(ref DebugData.backupCount);
#endif
#if DEBUG
                return true;
#endif
            }
#if !NET_COMPACT_20 && TRACE_HANDLE
            catch (SQLiteException e)
#else
            catch (SQLiteException)
#endif
            {
#if !NET_COMPACT_20 && TRACE_HANDLE
                try
                {
                    Trace.WriteLine(HelperMethods.StringFormat(
                        CultureInfo.CurrentCulture,
                        "FinishBackup: {0}, exception: {1}",
                        handle, e)); /* throw */
                }
                catch
                {
                }
#endif
            }
            finally
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (syncRoot)
#endif
                {
                    SetHandleAsInvalid();
                }
            }
#if DEBUG
            return false;
#else
            return true;
#endif
        }

        ///////////////////////////////////////////////////////////////////////

#if COUNT_HANDLE
        public int WasReleasedOk()
        {
            return Interlocked.Decrement(ref DebugData.backupCount);
        }
#endif

        ///////////////////////////////////////////////////////////////////////

        public override bool IsInvalid
        {
            get
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (syncRoot)
#endif
                {
                    return (handle == IntPtr.Zero);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////

#if DEBUG
        public override string ToString()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                return handle.ToString();
            }
        }
#endif
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////

    #region SQLiteBlobHandle Class
    // Provides finalization support for unmanaged SQLite blob objects.
    internal sealed class SQLiteBlobHandle : CriticalHandle
    {
#if PLATFORM_COMPACTFRAMEWORK
        internal readonly object syncRoot = new object();
#endif

        ///////////////////////////////////////////////////////////////////////

        private SQLiteConnectionHandle cnn;

        ///////////////////////////////////////////////////////////////////////

#if INTEROP_LEGACY_CLOSE
        private int version;
#endif

        ///////////////////////////////////////////////////////////////////////

        public static implicit operator IntPtr(SQLiteBlobHandle blob)
        {
            if (blob != null)
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (blob.syncRoot)
#endif
                {
                    return blob.handle;
                }
            }
            return IntPtr.Zero;
        }

        ///////////////////////////////////////////////////////////////////////

        internal SQLiteBlobHandle(SQLiteConnectionHandle cnn, IntPtr blob)
            : this()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                this.cnn = cnn;
                SetHandle(blob);

#if INTEROP_LEGACY_CLOSE
                SetVersion();
#endif
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private SQLiteBlobHandle()
            : base(IntPtr.Zero)
        {
#if COUNT_HANDLE
            Interlocked.Increment(ref DebugData.blobCount);
#endif
        }

        ///////////////////////////////////////////////////////////////////////

#if INTEROP_LEGACY_CLOSE
        private bool MatchVersion()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                return (cnn != null) ? (version == cnn.version) : false;
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private bool SetVersion()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                if (cnn != null)
                {
                    version = cnn.version;
                    return true;
                }

                return false;
            }
        }
#endif

        ///////////////////////////////////////////////////////////////////////

        protected override bool ReleaseHandle()
        {
            try
            {
#if !PLATFORM_COMPACTFRAMEWORK
                IntPtr localHandle = Interlocked.Exchange(
                    ref handle, IntPtr.Zero);

#if INTEROP_LEGACY_CLOSE
                if (!MatchVersion())
                {
#if !NET_COMPACT_20 && TRACE_HANDLE
                    try
                    {
                        Trace.WriteLine(HelperMethods.StringFormat(
                            CultureInfo.CurrentCulture,
                            "MatchVersion: {0} (blob handle)",
                            localHandle)); /* throw */
                    }
                    catch
                    {
                    }
#endif
#if COUNT_HANDLE
                    Interlocked.Decrement(ref DebugData.blobCount);
#endif
                    return false;
                }
#endif

                if (localHandle != IntPtr.Zero)
                    SQLiteBase.CloseBlob(cnn, localHandle);

#if !NET_COMPACT_20 && TRACE_HANDLE
                try
                {
                    Trace.WriteLine(HelperMethods.StringFormat(
                        CultureInfo.CurrentCulture,
                        "CloseBlob: {0}", localHandle)); /* throw */
                }
                catch
                {
                }
#endif
#else
                lock (syncRoot)
                {
                    if (handle != IntPtr.Zero)
                    {
                        SQLiteBase.CloseBlob(cnn, handle);
                        SetHandle(IntPtr.Zero);
                    }
                }
#endif
#if COUNT_HANDLE
                Interlocked.Decrement(ref DebugData.blobCount);
#endif
#if DEBUG
                return true;
#endif
            }
#if !NET_COMPACT_20 && TRACE_HANDLE
            catch (SQLiteException e)
#else
            catch (SQLiteException)
#endif
            {
#if !NET_COMPACT_20 && TRACE_HANDLE
                try
                {
                    Trace.WriteLine(HelperMethods.StringFormat(
                        CultureInfo.CurrentCulture,
                        "CloseBlob: {0}, exception: {1}",
                        handle, e)); /* throw */
                }
                catch
                {
                }
#endif
            }
            finally
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (syncRoot)
#endif
                {
                    SetHandleAsInvalid();
                }
            }
#if DEBUG
            return false;
#else
            return true;
#endif
        }

        ///////////////////////////////////////////////////////////////////////

#if COUNT_HANDLE
        public int WasReleasedOk()
        {
            return Interlocked.Decrement(ref DebugData.blobCount);
        }
#endif

        ///////////////////////////////////////////////////////////////////////

        public override bool IsInvalid
        {
            get
            {
#if PLATFORM_COMPACTFRAMEWORK
                lock (syncRoot)
#endif
                {
                    return (handle == IntPtr.Zero);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////

#if DEBUG
        public override string ToString()
        {
#if PLATFORM_COMPACTFRAMEWORK
            lock (syncRoot)
#endif
            {
                return handle.ToString();
            }
        }
#endif
    }
    #endregion
}
