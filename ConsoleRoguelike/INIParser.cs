#region
using System;
using System.Collections;
using System.IO;
using System.Linq;

#endregion

namespace DRODRoguelike
{
    public class INIParser
    {
        private readonly String _iniFilePath;
        private readonly Hashtable _keyPairs = new Hashtable ();

        /// <summary>
        ///   Opens the INI file at the given path and enumerates the values in the IniParser.
        /// </summary>
        /// <param name = "iniPath">Full path to INI file.</param>
        public INIParser(String iniPath)
        {
            TextReader iniFile = null;
            String strLine;
            String currentRoot = null;
            String[] keyPair;

            _iniFilePath = iniPath;

            if (!File.Exists(iniPath))
                throw new FileNotFoundException("Unable to locate " + iniPath);
            try
            {
                iniFile = new StreamReader(iniPath);

                strLine = iniFile.ReadLine ();

                while (strLine != null)
                {
                    strLine = strLine.Trim ().ToUpper ();

                    if (strLine != "")
                    {
                        if (strLine.StartsWith("[") && strLine.EndsWith("]"))
                        {
                            currentRoot = strLine.Substring(1, strLine.Length - 2);
                        }
                        else
                        {
                            keyPair = strLine.Split(new[] {'='}, 2);

                            SectionPair sectionPair;
                            String value = null;

                            if (currentRoot == null)
                                currentRoot = "ROOT";

                            sectionPair.Section = currentRoot;
                            sectionPair.Key = keyPair[0];

                            if (keyPair.Length > 1)
                                value = keyPair[1];

                            _keyPairs.Add(sectionPair, value);
                        }
                    }

                    strLine = iniFile.ReadLine ();
                }
            }
            finally
            {
                if (iniFile != null)
                    iniFile.Close ();
            }
        }

        /// <summary>
        ///   Returns the value for the given section, key pair.
        /// </summary>
        /// <param name = "sectionName">Section name.</param>
        /// <param name = "settingName">Key name.</param>
        public String GetSetting(String sectionName, String settingName)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper ();
            sectionPair.Key = settingName.ToUpper ();

            return (String) _keyPairs[sectionPair];
        }

        /// <summary>
        ///   Enumerates all lines for given section.
        /// </summary>
        /// <param name = "sectionName">Section to enum.</param>
        public String[] EnumSection(String sectionName)
        {
            ArrayList tmpArray = new ArrayList ();

            foreach (SectionPair pair in
                _keyPairs.Keys.Cast<SectionPair> ().Where(pair => pair.Section == sectionName.ToUpper ()))
            {
                tmpArray.Add(pair.Key);
            }

            return (String[]) tmpArray.ToArray(typeof (String));
        }

        /// <summary>
        ///   Adds or replaces a setting to the table to be saved.
        /// </summary>
        /// <param name = "sectionName">Section to add under.</param>
        /// <param name = "settingName">Key name to add.</param>
        /// <param name = "settingValue">Value of key.</param>
        public void AddSetting(String sectionName, String settingName, String settingValue)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper ();
            sectionPair.Key = settingName.ToUpper ();

            if (_keyPairs.ContainsKey(sectionPair))
                _keyPairs.Remove(sectionPair);

            _keyPairs.Add(sectionPair, settingValue);
        }

        /// <summary>
        ///   Adds or replaces a setting to the table to be saved with a null value.
        /// </summary>
        /// <param name = "sectionName">Section to add under.</param>
        /// <param name = "settingName">Key name to add.</param>
        public void AddSetting(String sectionName, String settingName)
        {
            AddSetting(sectionName, settingName, null);
        }

        /// <summary>
        ///   Remove a setting.
        /// </summary>
        /// <param name = "sectionName">Section to add under.</param>
        /// <param name = "settingName">Key name to add.</param>
        public void DeleteSetting(String sectionName, String settingName)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper ();
            sectionPair.Key = settingName.ToUpper ();

            if (_keyPairs.ContainsKey(sectionPair))
                _keyPairs.Remove(sectionPair);
        }

        /// <summary>
        ///   Save settings to new file.
        /// </summary>
        /// <param name = "newFilePath">New file path.</param>
        public void SaveSettings(String newFilePath)
        {
            ArrayList sections = new ArrayList ();
            String strToSave = "";

            foreach (SectionPair sectionPair in
                _keyPairs.Keys.Cast<SectionPair> ().Where(sectionPair => !sections.Contains(sectionPair.Section)))
            {
                sections.Add(sectionPair.Section);
            }

            foreach (String section in sections)
            {
                strToSave += ("[" + section + "]\r\n");

                foreach (SectionPair sectionPair in _keyPairs.Keys)
                {
                    if (sectionPair.Section != section) continue;
                    String tmpValue = (String) _keyPairs[sectionPair];

                    if (tmpValue != null)
                        tmpValue = "=" + tmpValue;

                    strToSave += (sectionPair.Key + tmpValue + "\r\n");
                }

                strToSave += "\r\n";
            }

            TextWriter tw = new StreamWriter(newFilePath);
            tw.Write(strToSave);
            tw.Close ();
        }

        /// <summary>
        ///   Save settings back to ini file.
        /// </summary>
        public void SaveSettings()
        {
            SaveSettings(_iniFilePath);
        }
        #region Nested type: SectionPair
        private struct SectionPair
        {
            public String Key;
            public String Section;
        }
        #endregion
    }
}