using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace BingCopy
{
    internal class BingWallpaperCopy
    {
        private string _sourceDir;
        private string _destDir;

        public BingWallpaperCopy()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Bing.SourceDir"]))
            {
                this._sourceDir = ConfigurationManager.AppSettings["Bing.SourceDir"];
                if (!this._sourceDir.EndsWith(@"\"))
                    this._sourceDir += @"\";
            }

            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Bing.DestDir"]))
            {
                this._destDir = ConfigurationManager.AppSettings["Bing.DestDir"];
                if (!this._destDir.EndsWith(@"\"))
                    this._destDir += @"\";
            }
        }

        public void Start()
        {
            if (!ValidateConfig())
                return;

            List<string> sourceFiles = GetSourceFiles();

            if (sourceFiles == null || sourceFiles.Count == 0)
                return;
        
            sourceFiles.ForEach(RenameFileIfExistsInDest);
        }

        private bool ValidateConfig()
        {
            return Directory.Exists(this._sourceDir) &&
                   Directory.Exists(this._destDir);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<string> GetSourceFiles()
        {
            return Directory.GetFiles(this._sourceDir, "*.jpg", SearchOption.TopDirectoryOnly)
                            .ToList();

            //if (files.Length == 0)
            //    return null;
        
            //var fileNames = (from f in files
            //               select Path.GetFileName(f)).ToList();

            //return fileNames;
        }

        private void RenameFileIfExistsInDest(string sourceFile)
        {
            string sourceFileName = Path.GetFileName(sourceFile);
            string destFile = String.Format("{0}{1}", this._destDir, sourceFileName);

            if (FileHasChanged(sourceFile, destFile))
            {
                string newDestFile = String.Format("{0}{1}", this._destDir, GetNewFileName(sourceFile));
                File.Move(destFile, newDestFile);
            }
        }

        private static string GetNewFileName(string sourceFile)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFile);
            string extension = sourceFile.Split('.').Last();

            return String.Format("{0}_{1}.{2}",
                                 fileName,
                                 DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                                 extension);
        }

        private static bool FileHasChanged(string sourceFile, string destFile)
        {
            var sourceFileInfo = new FileInfo(sourceFile);
            var destFileInfo = new FileInfo(destFile);

            return sourceFileInfo.LastWriteTimeUtc != destFileInfo.LastWriteTimeUtc ||
                   sourceFileInfo.Length != destFileInfo.Length;
        }
    }
}
