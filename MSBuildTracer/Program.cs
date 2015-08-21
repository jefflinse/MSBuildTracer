using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            MBE.Project project;

            try
            {
                project = new MBE.Project(projectFile);
            }
            catch (Microsoft.Build.Exceptions.InvalidProjectFileException)
            {
                Console.WriteLine($"The project file '{args[0]}' is invalid or doesn't exist.");
                return;
            }

            IEnumerable<MBE.ProjectProperty> properties;

            if (args.Length == 2)
            {
                properties = project.AllEvaluatedProperties.Where(p => PropertyNameMatchesPattern(p.Name, args[1]));
            }
            else
            {
                properties = project.AllEvaluatedProperties.OrderBy(p => p.Name);
            }

            if (!properties.Any())
            {
                string searchVerb = args.Length == 2 && args[1].Contains("*") ? "matching" : "named";
                Console.WriteLine($"No property {searchVerb} '{args[1]}' is defined anywhere for this project.");
                return;
            }

            foreach (var property in properties)
            {
                Console.WriteLine($"[{property.Name}]");
                TraceProperty(property);
                Console.WriteLine();
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
            var indent = new string('\t', indentCount);
            string location;

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
                location = $"{property.Xml.Location.File}:{property.Xml.Location.Line}";
            }

            Console.WriteLine($"{indent}Location:  {location}");

            if (property.UnevaluatedValue == property.EvaluatedValue)
            {
                Console.WriteLine($"{indent}Value:     {property.EvaluatedValue}");
            }
            else
            {
                Console.WriteLine($"{indent}U-Value:   {property.UnevaluatedValue}");
                Console.WriteLine($"{indent}E-Value:   {property.EvaluatedValue}");
            }
        }

        private static bool PropertyNameMatchesPattern(string propertyName, string pattern)
        {
            return new Regex($"^{pattern.Replace("*", ".*")}$", RegexOptions.IgnoreCase).Match(propertyName).Success;
        }

        private static void Usage()
        {
            Console.WriteLine("usage:\n\tMSBuildTracer filename [propertyname]");
        }
    }
}
