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

                    new ImportTracer(project).TraceAll();
                    break;

                case Mode.Properties:

                    new PropertyTracer(project).TraceAll(options.Query);
                    break;

                case Mode.Targets:

                    new TargetTracer(project).TraceAll(options.Query);
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
                options.Valid = true;

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

                return options;
            }
        }
    }
}
