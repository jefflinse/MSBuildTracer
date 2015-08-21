using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MBE = Microsoft.Build.Evaluation;

namespace MSBuildTracer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return;
            }

            var projectFile = args[0];
            var project = new MBE.Project(projectFile);

            if (args.Length == 2)
            {
                var property = project.AllEvaluatedProperties.FirstOrDefault(
                    p => string.Equals(p.Name, args[1], StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    TraceProperty(property);
                }
                else
                {
                    Console.WriteLine("'{0}' is not defined anywhere for this project.", args[1]);
                }
            }
            else
            {
                var properties = project.AllEvaluatedProperties.OrderBy(p => p.Name);
                foreach (var property in properties)
                {
                    TraceProperty(property);
                }
            }
        }

        private static void TraceProperty(MBE.ProjectProperty property, int traceLevel = 0)
        {
            if (property == null || property.IsGlobalProperty || property.IsReservedProperty)
            {
                return;
            }

            PrintPropertyInfo(property, traceLevel);

            if (property.IsImported)
            {
                TraceProperty(property.Predecessor, traceLevel + 1);
            }
        }

        private static void PrintPropertyInfo(MBE.ProjectProperty property, int indentCount)
        {
            var spaces = new string('\t', indentCount);
            string location;

            Console.WriteLine("[" + property.Name + "]");

            if (property.IsEnvironmentProperty)
            {
                location = "(environment)";
            }
            else if (property.IsReservedProperty)
            {
                location = "(reserved)";
            }
            else
            {
                location = property.Xml.Location.File + ":" + property.Xml.Location.Line;
            }

            Console.WriteLine(spaces + "Location:  " + location);

            if (property.UnevaluatedValue == property.EvaluatedValue)
            {
                Console.WriteLine(spaces + "Value:     " + property.EvaluatedValue);
            }
            else
            {
                Console.WriteLine(spaces + "U-Value:   " + property.UnevaluatedValue);
                Console.WriteLine(spaces + "E-Value:   " + property.EvaluatedValue);
            }

            Console.WriteLine();
        }

        private static void Usage()
        {
            Console.WriteLine("usage:\n\tMSBuildTracer filename [propertyname]");
        }
    }
}
