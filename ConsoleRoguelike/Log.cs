#region
using System.Collections.Generic;

#endregion

namespace DRODRoguelike
{
    public static class Log
    {
        public static List<string> Entries { get; set; }

        public static void Initialize()
        {
            Entries = new List<string>
                          {
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " ",
                              " "
                          };
        }

        public static void AddEntry(string entry)
        {
            Entries.Add(entry);
        }
    }
}