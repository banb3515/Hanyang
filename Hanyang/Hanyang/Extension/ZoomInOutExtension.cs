#region API 참조
using System;
#endregion

namespace Hanyang.Extension
{
    public static class ZoomInOutExtension
    {
        #region Clamp
        public static double Clamp(this double self, double min, double max)
        {
            return Math.Min(max, Math.Max(self, min));
        }
        #endregion
    }
}
