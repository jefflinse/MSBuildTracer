using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using MBEV = Microsoft.Build.Evaluation;
using MBEX = Microsoft.Build.Execution;

namespace MSBuildTracer
{
    class ImportTracer
    {
        private MBEV.Project project;

        public ImportTracer(MBEV.Project project)
        {
            this.project = project;
        }

        public void Trace(MBEV.ResolvedImport import, int traceLevel = 0)
        {
            PrintImportInfo(import, traceLevel);

            foreach (var childImport in project.Imports.Where(
                i => string.Equals(i.ImportingElement.ContainingProject.FullPath,
                                   project.ResolveAllProperties(import.ImportedProject.Location.File),
                                   StringComparison.OrdinalIgnoreCase)))
            {
                Trace(childImport, traceLevel + $"{import.ImportingElement.Location.Line}: ".Length);
            }
        }

        private void PrintImportInfo(MBEV.ResolvedImport import, int indentLength)
        {
            var indent = indentLength > 0 ? new StringBuilder().Insert(0, " ", indentLength).ToString() : "";

            Utils.WriteColor(indent, ConsoleColor.White);
            Utils.WriteColor($"{import.ImportingElement.Location.Line}: ", ConsoleColor.Cyan);

            var importedProjectFile = project.ResolveAllProperties(import.ImportedProject.Location.File);

            Utils.WriteColor(Path.GetDirectoryName(importedProjectFile) + Path.DirectorySeparatorChar, ConsoleColor.DarkGreen);
            Utils.WriteLineColor(Path.GetFileName(importedProjectFile), ConsoleColor.Green);

            if (!string.IsNullOrWhiteSpace(import.ImportingElement.Condition))
            {
                Utils.WriteColor($"{indent}because ", ConsoleColor.DarkGray);
                Utils.WriteLineColor($"{import.ImportingElement.Condition}", ConsoleColor.DarkCyan);
            }

            Console.WriteLine();
        }
    }
}
