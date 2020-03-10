using System;
using System.Diagnostics;
#if UNITY_STANDALONE
using PEDebug = UnityEngine.Debug;
#else
using PEDebug = System.Diagnostics.Debug;
#endif
namespace tainicom.Aether.Physics2D {
	[DebuggerNonUserCode]
     static class Debug {
		[Conditional("DEBUG")]
		public static void Assert(bool condition) {
			PEDebug.Assert(condition);
		}
		[Conditional("DEBUG")]
		[Obsolete("Assert(bool, string, params object[]) is obsolete. Use AssertFormat(bool, string, params object[]) (UnityUpgradable) -> AssertFormat(*)", true)]
		public static void Assert(bool condition, string message, string detailMessageFormat, params object[] args) {
			PEDebug.Assert(condition, message, detailMessageFormat, args);
		}
		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message) {
			PEDebug.Assert(condition, message);
		}
		[Conditional("DEBUG")]
		[Obsolete("Assert(bool, string, params object[]) is obsolete. Use AssertFormat(bool, string, params object[]) (UnityUpgradable) -> AssertFormat(*)", true)]
		public static void Assert(bool condition, string message, string detailMessage) {
			PEDebug.Assert(condition, message, detailMessage);
		}
		[Conditional("DEBUG")]
		public static void Fail(string message) {
#if UNITY_STANDALONE
			PEDebug.LogError(message);
#else
			PEDebug.Fail(message);
#endif
		}
		[Conditional("DEBUG")]
		public static void Fail(string message, string detailMessage) {
#if UNITY_STANDALONE
			PEDebug.LogError(string.Format("{0}\n{1}", message, detailMessage));
#else
			PEDebug.Fail(message, detailMessage);
#endif
		}
		[Conditional("DEBUG")]
		public static void WriteLine(string message) {
#if UNITY_STANDALONE
			PEDebug.Log(message);
#else
			PEDebug.Write(message);
#endif
		}
		[Conditional("DEBUG")]
		public static void WriteLine(string format, params object[] args) {
#if UNITY_STANDALONE
			PEDebug.LogFormat(format, args);
#else
			PEDebug.WriteLine(format, args);
#endif
		}
		[Conditional("DEBUG")]
		public static void WriteLine(string message, string category) {
#if UNITY_STANDALONE
			PEDebug.Log(message);
#else
			PEDebug.WriteLine(message, category);
#endif
		}
		[Conditional("DEBUG")]
		public static void WriteLine(object value) {
#if UNITY_STANDALONE
			PEDebug.Log(value);
#else
			PEDebug.WriteLine(value);
#endif
		}
		[Conditional("DEBUG")]
		public static void WriteLineIf(bool condition, object value) {
#if UNITY_STANDALONE
			if (condition) {
				PEDebug.Log(value);
			}
#else
			PEDebug.WriteLineIf(condition, value);
#endif
		}
		[Conditional("DEBUG")]
		public static void WriteLineIf(bool condition, object value, string category) {
#if UNITY_STANDALONE
			if (condition) {
				PEDebug.Log(value);
			}
#else
			PEDebug.WriteLineIf(condition, value, category);
#endif
		}
		[Conditional("DEBUG")]
		public static void WriteLineIf(bool condition, string message) {
#if UNITY_STANDALONE
			if (condition) {
				PEDebug.Log(message);
			}
#else
			PEDebug.WriteLineIf(condition, message);
#endif
		}
		[Conditional("DEBUG")]
		public static void WriteLineIf(bool condition, string message, string category) {
#if UNITY_STANDALONE
			if (condition) {
				PEDebug.Log(message);
			}
#else
			PEDebug.WriteLineIf(condition, message, category);
#endif
		}
	}
}