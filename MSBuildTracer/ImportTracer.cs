using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void Trace()
        {
            IDictionary<string, int> indents = new Dictionary<string, int>();
            int maxDepth = 0;
            foreach (var import in project.Imports)
            {
                string file = import.ImportingElement.ContainingProject.Location.File;
                if (!indents.ContainsKey(file))
                {
                    indents[file] = maxDepth++;
                }

                int indent = indents[file];
                PrintImportInfo(import.ImportedProject.Location.LocationString, indent);
            }
        }

        public int TraceTree(int start, string prev, int indent)
        {
            int i = start;
            while(i++ < project.Imports.Count) {
                var import = project.Imports[i];
                if (import.ImportingElement.ContainingProject.Location.File == prev)
                {
                    PrintImportInfo(import.ImportedProject.Location.LocationString, indent);
                }
                else {
                    i = TraceTree(i, import.ImportingElement.ContainingProject.Location.File, indent + 1);
                    break;
                }
            }

            return i;
        }

        private void PrintImportInfo(string importProject, int indentCount)
        {
            var indent = indentCount > 1 ? new StringBuilder().Insert(0, "|   ", indentCount - 1).ToString() : "";
            var tree = indentCount > 0 ? "|   " : "";

            Console.WriteLine($"{indent}{tree}{importProject}");
        }
    }
}
