/*
 * USER GUIDE:
 * 
 * toggle the static bool to true or false depending on your data usage status. 
 * run program.
 * comment out your LFS gitattributes if your bool is true, vice versa if your bool is false.
 * enjoy a simpler approach to using LFS for free.
 * 
 * nb:
 * the end result is a sporadic upload of LFS files on a monthly cycle. however this is better than the hardlock without this solution.
 * if you need more routine LFS pushes, consider paying for a datapack.
 * 
 * */




using System;
using System.IO;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace GitLFSBandwidthChecker
{
    class Program
    {
        // Manually set this field to true or false
        static bool isBandwidthExceeded = true;

        static void Main(string[] args)
        {
            string githubToken = "token";
            string repoOwner = "repoOwner";
            string repoName = "repoName";
            string gitattributesPath = Path.Combine("..", "..", "..", "..", $"{ repoName}", ".gitattributes");
            string gitignorePath = Path.Combine("..", "..", "..", "..", $"{repoName}", ".gitignore");

            if (isBandwidthExceeded)
            {
                if (File.Exists(gitattributesPath))
                {
                    string[] gitattributesLines = File.ReadAllLines(gitattributesPath);
                    string[] gitignoreLines = File.Exists(gitignorePath) ? File.ReadAllLines(gitignorePath) : new string[0];

                    using (StreamWriter sw = File.AppendText(gitignorePath))
                    {
                        sw.WriteLine("\n# Ignore LFS files");

                        foreach (string line in gitattributesLines)
                        {
                            if (line.Contains("filter=lfs"))
                            {
                                string pattern = Regex.Match(line, @"^\*\.\w+").Value;
                                if (!string.IsNullOrEmpty(pattern) && !Array.Exists(gitignoreLines, element => element == pattern))
                                {
                                    sw.WriteLine(pattern);
                                }
                            }
                        }
                    }

                    Console.WriteLine("Appended LFS patterns to .gitignore.");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine(".gitattributes file not found.");
                    Console.ReadKey();

                }
            }
            else
            {
                if (File.Exists(gitignorePath))
                {
                    string[] gitignoreLines = File.ReadAllLines(gitignorePath);
                    using (StreamWriter sw = new StreamWriter(gitignorePath))
                    {
                        foreach (string line in gitignoreLines)
                        {
                            if (!Regex.IsMatch(line, @"^\*\.\w+"))
                            {
                                sw.WriteLine(line);
                            }
                        }
                    }

                    Console.WriteLine("Removed LFS patterns from .gitignore.");
                    Console.ReadKey();

                }
                else
                {
                    Console.WriteLine(".gitignore file not found.");
                    Console.ReadKey();

                }

                Console.WriteLine("LFS bandwidth usage is within limits.");
                Console.ReadKey();

            }
        }
    }
}