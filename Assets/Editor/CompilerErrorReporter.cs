using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Luminia.Editor
{
    [InitializeOnLoad]
    public static class CompilerErrorReporter
    {
        private const string DiagnosticDirectory = "DiagnosticLogs";
        private const string ErrorFile = DiagnosticDirectory + "/UnityCompilerErrors.txt";

        static CompilerErrorReporter()
        {
            CompilationPipeline.compilationStarted -= OnCompilationStarted;
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.assemblyCompilationFinished -= OnAssemblyCompilationFinished;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
        }

        [MenuItem("Luminia/Open Compiler Error Log")]
        public static void OpenCompilerErrorLog()
        {
            Directory.CreateDirectory(DiagnosticDirectory);
            if (!File.Exists(ErrorFile))
            {
                File.WriteAllText(ErrorFile, "No compiler error has been recorded yet.", Encoding.UTF8);
            }

            EditorUtility.RevealInFinder(Path.GetFullPath(ErrorFile));
        }

        private static void OnCompilationStarted(object context)
        {
            Directory.CreateDirectory(DiagnosticDirectory);
            File.WriteAllText(ErrorFile,
                "LUMINIA UNITY COMPILER LOG" + Environment.NewLine +
                "Started: " + DateTime.Now.ToString("O") + Environment.NewLine +
                "Unity: " + Application.unityVersion + Environment.NewLine +
                new string('-', 72) + Environment.NewLine,
                Encoding.UTF8);
        }

        private static void OnAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            var output = new StringBuilder();
            foreach (var message in messages)
            {
                if (message.type != CompilerMessageType.Error)
                {
                    continue;
                }

                output.AppendLine("Assembly: " + assemblyPath);
                output.AppendLine(message.file + "(" + message.line + "," + message.column + ")");
                output.AppendLine(message.message);
                output.AppendLine();
            }

            if (output.Length > 0)
            {
                Directory.CreateDirectory(DiagnosticDirectory);
                File.AppendAllText(ErrorFile, output.ToString(), Encoding.UTF8);
            }
        }
    }
}
