#region API 참조
using System;
#endregion

namespace Hanyang.Interface
{
    public interface INotification
    {
        void CreateNotification(string title, string message);
    }
}
