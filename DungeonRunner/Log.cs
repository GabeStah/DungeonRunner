using System.Runtime.CompilerServices;

namespace DungeonRunner
{
	internal static class Log
	{
		internal static void Debug(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
		{
			Hearthstone_Deck_Tracker.Utility.Logging.Log.Debug($"[DungeonRunner] {msg}", memberName, sourceFilePath);
		}

		internal static void Error(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
		{
			Hearthstone_Deck_Tracker.Utility.Logging.Log.Error($"!!!!! [DungeonRunner] {msg}", memberName, sourceFilePath);
		}

		internal static void Info(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
		{
			Hearthstone_Deck_Tracker.Utility.Logging.Log.Info($"[DungeonRunner] {msg}", memberName, sourceFilePath);
		}
	}
}
