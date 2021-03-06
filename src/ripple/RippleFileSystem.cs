using System;
using System.IO;
using System.Reflection;
using FubuCore;

namespace ripple
{
    public static class RippleFileSystem
    {
        public static void CleanWithTracing(this IFileSystem system, string directory)
        {
            Console.WriteLine("Cleaning contents of directory " + directory);
            system.CleanDirectory(directory);
        }

        public static string CodeDirectory()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            if (location.Contains("buildsupport"))
            {
                return location.ParentDirectory().ParentDirectory().ParentDirectory();
            }

            while (location.Contains("ripple"))
            {
                location = location.ParentDirectory();
            }

            return location;
        }

        public static string ParentDirectory(this string path)
        {
            return Path.GetDirectoryName(path);
        }

        public static string RippleLogsDirectory()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            // TODO -- wourld really like an extension method in FubuCore for
            // get director name
            return Path.GetDirectoryName(location).AppendPath("logs");
        }

        public static string RippleDirectory()
        {
            var codeDirectory = CodeDirectory();
            var system = new FileSystem();
            if (system.DirectoryExists(codeDirectory.AppendPath("ripple")))
            {
                return codeDirectory.AppendPath("ripple");
            }

            return codeDirectory.AppendPath("buildsupport");
        }

        public static string ToLogPath(this string filename)
        {
            return RippleLogsDirectory().AppendPath(filename);
        }

        public static void OpenLogFile(this IFileSystem fileSystem, string filename)
        {
            if (!filename.EndsWith(".log"))
            {
                filename += ".log";
            }

            var path = filename.ToLogPath();

            if (fileSystem.FileExists(path))
            {
                fileSystem.LaunchEditor(path);
            }
            else
            {
                Console.WriteLine("File {0} does not exist", path);
            }
        }

        public static void WriteLogFile(this IFileSystem fileSystem, string filename, string contents)
        {
            var logsDirectory = RippleLogsDirectory();
            fileSystem.CreateDirectory(logsDirectory);
            fileSystem.WriteStringToFile(logsDirectory.AppendPath(filename), contents);
        }


        public static string RippleExeLocation()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        public static string RakeRunnerFile()
        {
            return LocationOfRunner("run-rake.cmd");
        }

        public static string LocalNugetDirectory()
        {
            return CodeDirectory().AppendPath("nugets");
        }

        public static string LocationOfRunner(string file)
        {
            var folder = RippleExeLocation().ParentDirectory();
            var system = new FileSystem();

            if (system.FileExists(folder.AppendPath(file)))
            {
                return folder.AppendPath(file).ToFullPath();
            }

            return CodeDirectory().AppendPath("ripple", file);
        }
    }
}