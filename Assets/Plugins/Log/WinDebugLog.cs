using System.Runtime.InteropServices;

namespace Plugins
{
    public class WinDebugLog
    {
#if UNITY_STANDALONE_WIN
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);
#endif
    }
}