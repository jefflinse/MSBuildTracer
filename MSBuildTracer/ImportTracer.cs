using System;
using System.Collections.Generic;
using System.Linq;
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
                                   project.ResolveAllProperties(import.ImportingElement.Project),
                                   StringComparison.OrdinalIgnoreCase)))
            {
                Trace(childImport, traceLevel + 1);
            }
        }

        private void PrintImportInfo(MBEV.ResolvedImport import, int indentCount)
        {
            var indent = indentCount > 0 ? new StringBuilder().Insert(0, "    ", indentCount).ToString() : "";

            Console.WriteLine($"{indent}{import.ImportingElement.Location.Line}: {project.ResolveAllProperties(import.ImportedProject.Location.File)}");
        }
    }
}
