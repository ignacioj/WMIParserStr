using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WMIParserStr
{
    class Program
    {
        public static Arguments CommandLine;
        public static string entrada;
        public static List<Details> LConsumers = new List<Details>();
        public static List<Details> LEventFilters = new List<Details>();
        public static List<Details> LBindings = new List<Details>();

        static void Main(string[] args)
        {
            //----------------------------------------------------------
            CommandLine = new Arguments(args);
            if ((!string.IsNullOrEmpty(CommandLine["i"])) && (File.Exists(CommandLine["i"]))) entrada = CommandLine["i"];
            else Ayuda();

            string data = File.ReadAllText(entrada);

            Regex getNames = new Regex(@"(.*).Name=""(.*)""", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex strings = new Regex(@"[ -~]{3,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var matchesStrings = strings.Matches(data);

            int longObjects = matchesStrings.Count;

            for (int i = 0; i < longObjects; i++)
            {
                Match m = matchesStrings[i];
                if (m.Value.Contains(@"EventConsumer.Name="""))
                {
                    var nameConsumer = getNames.Matches(m.Value);
                    i++;
                    m = matchesStrings[i];
                    var nameEventFilter = getNames.Matches(m.Value);
                    LBindings.Add(new Details("Binding", nameConsumer[0].Groups[1].Value, nameConsumer[0].Groups[2].Value, nameEventFilter[0].Groups[2].Value, false));
                }
            }
            for (int i = 0; i < longObjects; i++)
            {
                Match m = matchesStrings[i];
                string type = "";
                string name = "";
                string content = "";
                string other = "";
                if (m.Value == "ActiveScriptEventConsumer")
                {
                    bool valid = false;
                    type = "ActiveScriptEventConsumer";
                    i++;
                    name = matchesStrings[i].Value;
                    i++;
                    if ((matchesStrings[i].Value.ToLower() == "vbscript") || (matchesStrings[i].Value.ToLower() == "jscript"))
                    {
                        valid = true;
                        other = matchesStrings[i].Value;
                        i++;
                        content = matchesStrings[i].Value;
                    }
                    else if ((matchesStrings[i + 1].Value.ToLower() == "vbscript") || (matchesStrings[i + 1].Value.ToLower() == "jscript"))
                    {
                        valid = true;
                        content = matchesStrings[i].Value;
                        i++;
                        other = matchesStrings[i].Value;
                    }
                    if (valid)
                    {
                        //check if it is a consumer without binding
                        var match = LBindings.FirstOrDefault(item => item.Content.Equals(name));
                        if (match != null) LConsumers.Add(new Details(type, name, content, other, false));
                        else LConsumers.Add(new Details(type, name, content, other, true));
                    }
                    else
                    {
                        i -= 2;
                    }
                }
                else if (m.Value == "CommandLineEventConsumer")
                {
                    type = "CommandLineEventConsumer";
                    i++;
                    content = matchesStrings[i].Value;
                    i++;
                    string temp = matchesStrings[i].Value;
                    var match = LBindings.FirstOrDefault(item => item.Content.Equals(temp));
                    if (match != null)
                    {
                        name = temp;
                        LConsumers.Add(new Details(type, name, content, "", false));
                    }
                    else if (temp.ToLower().EndsWith(".exe"))
                    {
                        other = temp;
                        i++;
                        name = matchesStrings[i].Value;
                        match = LBindings.FirstOrDefault(item => item.Content.Equals(name));
                        if (match != null) LConsumers.Add(new Details(type, name, content, "", false));
                        else LConsumers.Add(new Details(type, name, content, "", true));
                    }
                    else
                    {
                        name = temp;
                        LConsumers.Add(new Details(type, name, content, "", true));
                    }
                }
                else if (m.Value == "__EventFilter")
                {
                    type = "__EventFilter";
                    i++;
                    other = matchesStrings[i].Value;
                    i++;
                    name = matchesStrings[i].Value;
                    i++;
                    content = matchesStrings[i].Value;
                    for (int n = 0; n < 7; n++)
                    {
                        i++;
                        string temp = matchesStrings[i].Value;
                        if ((temp.ToLower() == "wql") || (n==6))
                        {
                            var match = LBindings.FirstOrDefault(item => item.Other.Equals(name));
                            if (match != null) LEventFilters.Add(new Details(type, name, content, other, false));
                            else LEventFilters.Add(new Details(type, name, content, other, true));
                            break;
                        }
                        else
                        {
                            content += temp;
                        }
                    }
                }
            }

            OutputToConsole();

            if ((!string.IsNullOrEmpty(CommandLine["s"])) && (Directory.Exists(CommandLine["s"])))
            {
                WriteStrings(matchesStrings);
            }

            if ((!string.IsNullOrEmpty(CommandLine["o"])) && (Directory.Exists(CommandLine["o"])))
            {
                OutputToFile();
            }
        }

        private static void WriteStrings(MatchCollection matchesStrings)
        {
            using (FileStream fileStream = new FileStream(Path.Combine(CommandLine["s"], "WMIParserStr-OBJECTS_strings.txt"), FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                foreach (var line in matchesStrings)
                {
                    writer.WriteLine(line.ToString());
                }
            }
        }
        private static void OutputToConsole()
        {
            Console.WriteLine("Total Bindings: {0}", LBindings.Count);
            foreach (var b in LBindings)
            {
                Console.WriteLine("[{0}]-[{1}]-[{2}]-[{3}]-[{4}]", b.Type, b.Name, b.Content, b.Other, b.Orphan);
            }
            Console.WriteLine("Total Consumers: {0}", LConsumers.Count);
            foreach (var b in LConsumers)
            {
                Console.WriteLine("[{0}]-[{1}]-[{2}]-[{3}]-[{4}]", b.Type, b.Name, b.Content, b.Other, b.Orphan);
            }
            Console.WriteLine("Total EventFilters: {0}", LEventFilters.Count);
            foreach (var b in LEventFilters)
            {
                Console.WriteLine("[{0}]-[{1}]-[{2}]-[{3}]-[{4}]", b.Type, b.Name, b.Content, b.Other, b.Orphan);
            }
        }

        private static void OutputToFile()
        {
            using (FileStream fileStream = new FileStream(Path.Combine(CommandLine["o"], "WMIParserStr-output.tsv"), FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", "Type", "Name", "Content", "Other", "Orphan");
                foreach (var item in LBindings)
                {
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", item.Type.ToString(), item.Name.ToString(), item.Content.ToString(), item.Other.ToString(), item.Orphan.ToString());
                }
                foreach (var item in LConsumers)
                {
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", item.Type.ToString(), item.Name.ToString(), item.Content.ToString(), item.Other.ToString(), item.Orphan.ToString());
                }
                foreach (var item in LEventFilters)
                {
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", item.Type.ToString(), item.Name.ToString(), item.Content.ToString(), item.Other.ToString(), item.Orphan.ToString());
                }
            }
        }

        private static void Ayuda()
        {
            var ayuda = $"Author: Ignacio J. Pérez Jiménez\r\ngithub.com/ignacioj\r\n\r\n-i Input file (OBJECTS.DATA)\r\n-o Output directory for analysis results\r\n-s Ouput directory to save the strings (not Unicode) of OBJECTS.DATA";
            Console.WriteLine(ayuda);
            System.Environment.Exit(0);
        }
    }

    /*
                    Type               Name                 Content                 Other                               Orphan
     Bindings:      Binding            Type of Consumer     Consumer name           EventFilter name                    FALSE
     Consumers:     Type of consumer   Consumer name        CommandLineTemplate     [ExecutablePath][VBScript/JSCript]  False/True 
     EventFilter:   __EventFilter      EventFilter name     Condition               [root\cimv2][...]
     */
    public class Details
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Other { get; set; }
        public bool Orphan { get; set; }

        public Details(string type, string name, string content, string other, bool orphan)
        {
            Type = type;
            Name = name;
            Content = content;
            Other = other;
            Orphan = orphan;
        }
    }
}
