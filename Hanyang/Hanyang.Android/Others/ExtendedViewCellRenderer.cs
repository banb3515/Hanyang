#region API 참조
using Hanyang.Others;

using System.ComponentModel;

using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Hanyang.Droid.Others;
#endregion

[assembly: ExportRenderer(typeof(ExtendedViewCell), typeof(ExtendedViewCellRenderer))]
namespace Hanyang.Droid.Others
{
    class ExtendedViewCellRenderer : ViewCellRenderer
    {
        #region 변수
        private Android.Views.View cellCoreView;
        private Drawable unSelectedBackground;
        private bool isSelected;
        #endregion

        #region GetCellCore
        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            this.cellCoreView = base.GetCellCore(item, convertView, parent, context);

            this.isSelected = false;
            this.unSelectedBackground = cellCoreView.Background;

            return cellCoreView;
        }
        #endregion

        #region OnCellPropertyChanged
        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnCellPropertyChanged(sender, args);

            if (args.PropertyName == "IsSelected")
            {
                this.isSelected = !this.isSelected;

                if (this.isSelected)
                {
                    var extendedViewCell = sender as ExtendedViewCell;
                    this.cellCoreView.SetBackgroundColor(extendedViewCell.SelectedBackgroundColor.ToAndroid());
                }
                else
                    this.cellCoreView.SetBackground(this.unSelectedBackground);
            }
        }
        #endregion
    }
}