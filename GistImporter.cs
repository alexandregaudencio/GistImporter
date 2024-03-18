using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GistImporter
{

    [InitializeOnLoad]
    public static class GistImporter
    {
        static readonly Regex _getGistUrlInfoRegex = new Regex("https://gist.github.com/(?<owner>.+)/(?<gistId>[a-z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //static readonly Regex _getDescriptionRegex = new Regex(@"\<title\>(?<description>.+)\</title\>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex _getFileUrlRegex = new Regex("href=\"(?<url>.+/raw/[a-z0-9\\./\\-]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        [MenuItem("Tools/Import Gist")]
        public static void ImportGist(string url, string folder = "")
        {

            try
            {
                using (var client = new WebClient())
                {
                    var gistPageContent = client.DownloadString(url);
                    var filesMatches = _getFileUrlRegex.Matches(gistPageContent);

                    if (filesMatches.Count > 0)
                    {
                        var infoMatch = _getGistUrlInfoRegex.Match(url).Groups;
                        var gistOwner = infoMatch["owner"].Value;
                        var gistId = infoMatch["gistId"].Value;
                        var rawUrls = filesMatches
                                        .OfType<Match>()
                                        .Select(m => $"https://gist.github.com{m.Groups["url"].Value}")
                                        .OrderByDescending(u => u.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                                        .ToArray();

                        //Combine path description
                        var destinationFolder = Path.Combine(Application.dataPath, folder, $"{Path.GetFileNameWithoutExtension(rawUrls.First())}");

                        // Downloads and write the Gist files.
                        for (var i = 0; i < rawUrls.Length; i++)
                        {
                            var rawUrl = rawUrls[i];
                            var filename = Path.GetFileName(rawUrl);

                            EditorUtility.DisplayProgressBar("Importing Gist...", filename, i / (float)filesMatches.Count);
                            DownloadFile(client, rawUrl, destinationFolder, filename);
                        }

                        EditorUtility.ClearProgressBar();
                    }
                    else
                        Debug.LogWarning($"No files found for Gist '{url}'.");
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static bool ValidateGistUrl(string url)
        {
            return _getGistUrlInfoRegex.IsMatch(url);
        }

        static void DownloadFile(WebClient client, string url, string destinationFolder, string filename)
        {
            var fileContent = client.DownloadString(url);

            if (fileContent.IndexOf("using UnityEditor;", StringComparison.OrdinalIgnoreCase) > -1)
                destinationFolder = Path.Combine(destinationFolder, "Editor");

            Directory.CreateDirectory(destinationFolder);
            File.WriteAllText(Path.Combine(destinationFolder, filename), fileContent);
        }



    }


 


}