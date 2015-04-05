using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

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

        /// <summary>
        /// Loads configuration from settings file.
        /// </summary>
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

        /// <summary>
        /// Starts the Bing copy process.
        /// </summary>
        public void Start()
        {
            if (!ValidateConfig())
                return;

            List<string> sourceFiles = GetSourceFiles();

            if (sourceFiles == null || sourceFiles.Count == 0)
                return;
        
            sourceFiles.ForEach(RenameFileIfExistsInDest);
        }

        /// <summary>
        /// Validates if the configuration is valid.
        /// </summary>
        /// <returns>true if the configuration is valid, false otherwise.</returns>
        private bool ValidateConfig()
        {
            return Directory.Exists(this._sourceDir) &&
                   Directory.Exists(this._destDir);
        }

        /// <summary>
        /// Gets files from the source directory.
        /// </summary>
        /// <returns>A List of files in the source directory.</returns>
        private List<string> GetSourceFiles()
        {
            return Directory.GetFiles(this._sourceDir, "*.jpg", SearchOption.TopDirectoryOnly)
                            .ToList();
        }

        /// <summary>
        /// Renames the file in destination directory, if it already exists.
        /// </summary>
        /// <param name="sourceFile">The path of the file in source directory.</param>
        private void RenameFileIfExistsInDest(string sourceFile)
        {
            string sourceFileName = Path.GetFileName(sourceFile);
            string destFile = String.Format("{0}{1}", this._destDir, sourceFileName);

            if (File.Exists(destFile) &&
                FileHasChanged(sourceFile, destFile))
            {
                string newDestFile = String.Format("{0}{1}", this._destDir, GetNewFileName(sourceFile));
                File.Move(destFile, newDestFile);
            }
        }

        /// <summary>
        /// Gets a new name for the file being renamed.
        /// </summary>
        /// <param name="sourceFile">The path of the file in source directory.</param>
        /// <returns>A new filename with timestamp suffix.</returns>
        private static string GetNewFileName(string sourceFile)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFile);
            string extension = sourceFile.Split('.').Last();

            return String.Format("{0}_{1}.{2}",
                                 fileName,
                                 DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                                 extension);
        }

        /// <summary>
        /// Determines if the file in the source directory and destination directory are same.
        /// </summary>
        /// <param name="sourceFile">Path of the file in the source directory.</param>
        /// <param name="destFile">Path of the file in the destination directory.</param>
        /// <returns>true if the file has changed. File is considered changed if it's last write time or size is different.</returns>
        private static bool FileHasChanged(string sourceFile, string destFile)
        {
            var sourceFileInfo = new FileInfo(sourceFile);
            var destFileInfo = new FileInfo(destFile);

            //var hasChanged = sourceFileInfo.LastWriteTimeUtc != destFileInfo.LastWriteTimeUtc ||
            //                 sourceFileInfo.Length != destFileInfo.Length;

            var hasChanged = sourceFileInfo.Length != destFileInfo.Length ||
                             ComputeHash(sourceFile) != ComputeHash(destFile);

            return hasChanged;
        }

        /// <summary>
        /// Computes the hash of a given file using MD5 hash algorithm.
        /// </summary>
        /// <param name="filePath">The file whose needs to be hashed.</param>
        /// <returns>A string representation of the hash.</returns>
        private static string ComputeHash(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
        }
    }
}
