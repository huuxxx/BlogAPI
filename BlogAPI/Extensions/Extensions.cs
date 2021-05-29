using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Extensions
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            var stackTrack = new StackTrace();
            var stackFrame = stackTrack.GetFrame(1);

            return stackFrame.GetMethod().Name;
        }
    }
}
