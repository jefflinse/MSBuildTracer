using System;
using System.Collections.Generic;
using System.Linq;

using MBEV = Microsoft.Build.Evaluation;

namespace MSBuildTracer
{
    enum Mode { Properties, Targets };

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
            catch (Microsoft.Build.Exceptions.InvalidProjectFileException)
            {
                Console.WriteLine($"The project file '{args[1]}' is invalid or doesn't exist.");
                return 1;
            }

            switch (options.Mode)
            {
                case Mode.Properties:

                    var properties = project.AllEvaluatedProperties.Where(
                        p => PropertyTracer.PropertyNameMatchesPattern(p.Name, options.Query));

                    if (!properties.Any())
                    {
                        Console.WriteLine($"No property matching '{options.Query}' is defined anywhere for this project.");
                    }

                    foreach (var property in properties)
                    {
                        Console.WriteLine($"[{property.Name}]");
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
            Console.WriteLine("usage:\n\tMSBuildTracer filename (-p|-t) [query]");
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

                if (args[1] == "-p")
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
