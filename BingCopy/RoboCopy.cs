using System;
using System.Configuration;
using System.Diagnostics;

namespace BingCopy
{
    internal class RoboCopy
    {
        private string _sourceDir;
        private string _destDir;

        private string _robocopyPath;
        private string _robocopyArgs;

        public RoboCopy()
        {
            LoadConfig();
        }
        
        public void Start()
        {
            try
            {
                using (var cmd = new Process())
                {
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.Arguments = String.Format("/C {0} {1} {2} {3}",
                                                            this._robocopyPath,
                                                            this._sourceDir,
                                                            this._destDir,
                                                            this._robocopyArgs);

                    cmd.Start();
                    cmd.WaitForExit();
                }
            }
            catch (Exception)
            {

            }
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

            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["RoboCopy.Path"]))
                this._robocopyPath = ConfigurationManager.AppSettings["RoboCopy.Path"];

            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["RoboCopy.Args"]))
                this._robocopyArgs = ConfigurationManager.AppSettings["RoboCopy.Args"];
        }
    }
}
