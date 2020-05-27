﻿using Flow.Launcher.Infrastructure;
using Flow.Launcher.Plugin.SharedCommands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Flow.Launcher.Plugin.Explorer.Search.DirectoryInfo
{
    public class DirectoryInfoSearch
    {
        private Settings _settings;

        public DirectoryInfoSearch(Settings settings)
        {
            _settings = settings;
        }

        internal List<Result> TopLevelDirectorySearch(Query query, string search)
        {
            var criteria = ConstructSearchCriteria(search);

            if (search.IndexOfAny(Constants.SpecialSearchChars) >= 0)
                return DirectorySearch(SearchOption.AllDirectories, query, search, criteria);
            
            return DirectorySearch(SearchOption.TopDirectoryOnly, query, search, criteria);
        }

        public string ConstructSearchCriteria(string search)
        {
            string incompleteName = "";

            if (!search.EndsWith("\\"))
            {
                // not full path, get previous level directory string
                var indexOfSeparator = search.LastIndexOf('\\');

                incompleteName = search.Substring(indexOfSeparator + 1).ToLower();

                if (incompleteName.StartsWith(">"))
                    incompleteName = "*" + incompleteName.Substring(1);
            }

            incompleteName += "*";

            return incompleteName;
        }

        private List<Result> DirectorySearch(SearchOption searchOption, Query query, string search, string searchCriteria)
        {
            var results = new List<Result>();

            var path = search;

            if (!search.EndsWith("\\"))
            {
                // not full path, get previous level directory string
                var indexOfSeparator = search.LastIndexOf('\\');

                path = search.Substring(0, indexOfSeparator + 1);
            }

            var folderList = new List<Result>();
            var fileList = new List<Result>();

            try
            {
                var directoryInfo = new System.IO.DirectoryInfo(path);
                var fileSystemInfos = directoryInfo.GetFileSystemInfos(searchCriteria, searchOption);

                foreach (var fileSystemInfo in fileSystemInfos)
                {
                    if ((fileSystemInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;

                    if (fileSystemInfo is System.IO.DirectoryInfo)
                    {
                        folderList.Add(ResultManager.CreateFolderResult(fileSystemInfo.Name, fileSystemInfo.FullName, fileSystemInfo.FullName, query, true, false));
                    }
                    else
                    {
                        fileList.Add(ResultManager.CreateFileResult(fileSystemInfo.FullName, query, true, false));
                    }
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedAccessException || e is ArgumentException)
                {
                    results.Add(new Result { Title = e.Message, Score = 501 });

                    return results;
                }

                throw;
            }

            // Intial ordering, this order can be updated later by UpdateResultView.MainViewModel based on history of user selection.
            return results.Concat(folderList.OrderBy(x => x.Title)).Concat(fileList.OrderBy(x => x.Title)).ToList(); //<============= MOVE OUT
        }
    }
}
