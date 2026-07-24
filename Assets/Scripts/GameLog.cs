using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Luminia
{
    public sealed class GameLog : MonoBehaviour
    {
        private static readonly object FileLock = new object();
        private string logDirectory = string.Empty;
        private string currentLogPath = string.Empty;

        public static string CurrentLogPath { get; private set; } = string.Empty;

        private void Awake()
        {
            logDirectory = Path.Combine(Application.persistentDataPath, "Logs");
            Directory.CreateDirectory(logDirectory);
            currentLogPath = Path.Combine(logDirectory, $"Luminia_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            CurrentLogPath = currentLogPath;
            Application.logMessageReceivedThreaded += WriteLog;
            WriteHeader();
            Debug.Log($"Luminia log file: {currentLogPath}");
        }

        private void OnDestroy()
        {
            Application.logMessageReceivedThreaded -= WriteLog;
        }

        public static void OpenLogFolder()
        {
            var directory = Path.Combine(Application.persistentDataPath, "Logs");
            Directory.CreateDirectory(directory);
            Application.OpenURL("file:///" + directory.Replace('\\', '/'));
        }

        private void WriteHeader()
        {
            var header = new StringBuilder()
                .AppendLine("LUMINIA DIAGNOSTIC LOG")
                .AppendLine($"Started: {DateTime.Now:O}")
                .AppendLine($"Game version: {Application.version}")
                .AppendLine($"Unity version: {Application.unityVersion}")
                .AppendLine($"Platform: {Application.platform}")
                .AppendLine($"OS: {SystemInfo.operatingSystem}")
                .AppendLine($"CPU: {SystemInfo.processorType}")
                .AppendLine($"GPU: {SystemInfo.graphicsDeviceName}")
                .AppendLine($"Resolution: {Screen.width}x{Screen.height}")
                .AppendLine(new string('-', 72))
                .ToString();
            File.WriteAllText(currentLogPath, header, Encoding.UTF8);
        }

        private void WriteLog(string message, string stackTrace, LogType type)
        {
            try
            {
                var entry = $"[{DateTime.Now:HH:mm:ss.fff}] [{type}] {message}{Environment.NewLine}";
                if ((type == LogType.Error || type == LogType.Exception || type == LogType.Assert) &&
                    !string.IsNullOrWhiteSpace(stackTrace))
                {
                    entry += stackTrace + Environment.NewLine;
                }

                lock (FileLock)
                {
                    File.AppendAllText(currentLogPath, entry, Encoding.UTF8);
                }
            }
            catch
            {
                // Never throw from the logging callback. Unity's own Player.log remains available.
            }
        }
    }
}
