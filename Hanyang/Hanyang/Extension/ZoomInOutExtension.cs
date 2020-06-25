using System;
using System.Collections.Generic;
using System.Text;

namespace Hanyang.Extension
{
    public static class ZoomInOutExtension
    {
        public static double Clamp(this double self, double min, double max)
        {
            return Math.Min(max, Math.Max(self, min));
        }
    }
}
