using System;
using System.Collections.Generic;
using System.Text;

namespace XIDE
{
    class Helper
    {
        public static bool IsUnix()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;

            if (pid == PlatformID.Unix)
                return true;
            else
                return false;
        }
    }
}
