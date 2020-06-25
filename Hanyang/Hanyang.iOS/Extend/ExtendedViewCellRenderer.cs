#region API 참조
using Hanyang.Extend;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
#endregion

namespace Hanyang.iOS.Extend
{
    public class ExtendedViewCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            var view = item as ExtendedViewCell;
            cell.SelectedBackgroundView = new UIView{BackgroundColor = view.SelectedBackgroundColor.ToUIColor()};

            return cell;
        }
    }
}