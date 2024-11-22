using Godot;
using System;
using System.Collections.Generic;

namespace GoDogKit
{
    /// <summary>
    /// Encapsulation of comma separated value (CSV) files reading and processing methods.
    /// </summary>
    public static class CsvUtility
    {
        public static List<string[]> Read(string path, bool ignoreFirstLine = true)
        {
            var list = new List<string[]>();

            var csv = FileAccess.Open(path, FileAccess.ModeFlags.Read);

            string[] line;

            if (ignoreFirstLine) csv.GetCsvLine();

            while ((line = csv.GetCsvLine())[0] is not "")
            {
                list.Add(line);
            }

            return list;
        }

        public static List<string[]> ReadStreaming(string path, bool ignoreFirstLine = true)
        => Read(Archive.Streaming + "\\" + path, ignoreFirstLine);

        public static bool Process(List<string[]> data, Action<string[]> callback)
        {
            if (data == null || data.Count == 0)
            {
                GD.PushError("CSV file is empty or not found.");
                return false;
            }

            foreach (var line in data) callback(line);

            return true;
        }

        public static bool ReadAndProcess(string path, Action<string[]> callback)
        => Process(Read(path), callback);

        public static bool ReadStreamingAndProcess(string path, Action<string[]> callback)
        => Process(ReadStreaming(path), callback);
    }

}
