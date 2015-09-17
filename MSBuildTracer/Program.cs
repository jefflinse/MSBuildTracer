using System;
using System.Collections.Generic;
using System.Linq;

using MBEV = Microsoft.Build.Evaluation;

namespace MSBuildTracer
{
    enum Mode { Imports, Properties, Targets };

    class Program
    {
        static int Main(string[] args)
        {
            var options = Options.ProcessCommandLineArguments(args);
            if (!options.Valid)
            {
                Usage();
                return 1;
            }

            MBEV.Project project;

            try
            {
                project = new MBEV.Project(options.Filename);
            }
            catch (Microsoft.Build.Exceptions.InvalidProjectFileException e)
            {
                Console.WriteLine($"The project file '{options.Filename}' is invalid or doesn't exist.");
                Console.WriteLine();
                Console.WriteLine(e.Message);
                return 1;
            }

            switch (options.Mode)
            {
                case Mode.Imports:

                    var imports = project.Imports.Where(i => !i.IsImported);

                    if (!imports.Any())
                    {
                        Console.WriteLine("This project does not import any other files.");
                        return 0;
                    }

                    var importTracer = new ImportTracer(project);
                    foreach (var import in imports)
                    {
                        importTracer.Trace(import);
                        Console.WriteLine();
                    }

                    break;

                case Mode.Properties:

                    var properties = project.AllEvaluatedProperties.Where(
                        p => PropertyTracer.PropertyNameMatchesPattern(p.Name, options.Query) &&
                        !p.IsPredecessor(project));

                    if (!properties.Any())
                    {
                        Console.WriteLine($"No property matching '{options.Query}' is defined anywhere for this project.");
                        return 0;
                    }

                    foreach (var property in properties)
                    {
                        Utils.WriteLineColor($"[{property.Name}]", ConsoleColor.Cyan);
                        PropertyTracer.Trace(property);
                        Console.WriteLine();
                    }

                    break;

                case Mode.Targets:

                    var tracer = new TargetTracer(project);

                    var targets = project.Targets.Where(t => TargetTracer.TargetNameMatchesPattern(t.Key, options.Query)).Select(t => t.Value);

                    if (!targets.Any())
                    {
                        Console.WriteLine($"No target matching '{options.Query}' is defined anywhere for this project.");
                        return 0;
                    }

                    foreach (var target in targets)
                    {
                        tracer.Trace(target);
                        Console.WriteLine();
                    }

                    break;
            }

            return 0;
        }

        private static void Usage()
        {
            Console.WriteLine("usage:\n\tMSBuildTracer filename (-i|-p|-t) [query]");
        }

        private class Options
        {
            public bool Valid { get; private set; }

            public Mode Mode { get; private set; }

            public string Filename { get; private set; }

            public string Query { get; private set; }

            public static Options ProcessCommandLineArguments(string[] args)
            {
                var options = new Options();
                if (args.Length < 2)
                {
                    options.Valid = false;
                    return options;
                }

                options.Filename = args[0];

                if (args[1] == "-i")
                {
                    options.Mode = Mode.Imports;
                }
                else if (args[1] == "-p")
                {
                    options.Mode = Mode.Properties;
                }
                else if (args[1] == "-t")
                {
                    options.Mode = Mode.Targets;
                }
                else
                {
                    options.Valid = false;
                }

                options.Query = args.Length == 3 ? args[2] : "*";

                options.Valid = true;
                return options;
            }
        }
    }
}
