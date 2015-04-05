using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            var bing = new BingWallpaperCopy();
            bing.Start();

            var robocopy = new RoboCopy();
            robocopy.Start();
        }
    }
}
