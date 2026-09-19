using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.AMS.RES;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.HMS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.Frame;
using CS.ERP_MOB.ViewsModel.POS;
using Microsoft.Maui.Devices;
using RGPopup.Maui.Pages;
using RGPopup.Maui.Services;
using System.Diagnostics;

namespace CS.ERP_MOB.Views.POS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmPosSaleInvoicePop : PopupPage
    {
        #region "Declaring"
        VmlSalesInvoice mVmlSalesInvoice;
        DAT_FRONT_DESK selectedData = new DAT_FRONT_DESK();
        private TaskCompletionSource<object> _taskCompletionSource;
        public Task<object> PopupClosedTask => _taskCompletionSource.Task;


        #endregion

        #region "Constructor"
        public FrmPosSaleInvoicePop()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlSalesInvoice = new VmlSalesInvoice();


                var display = DeviceDisplay.MainDisplayInfo;
                double height = display.Height / display.Density;
                double width = display.Width / display.Density;

                // Set 2/3 height and full width
                PopupFrame.HeightRequest = height * 2 / 3;
                PopupFrame.WidthRequest = width;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public FrmPosSaleInvoicePop(VmlSalesInvoice mVmlSalesInvoice)
        {
            try
            {
                InitializeComponent();
                _taskCompletionSource = new TaskCompletionSource<object>();

                
                Loaded += async (s, e) =>
                {
                    var display = DeviceDisplay.MainDisplayInfo;
                    double height = display.Height / display.Density;
                    double width = display.Width / display.Density;

                    // Set 2/3 height and full width
                    PopupFrame.HeightRequest = height * 2 / 3;
                    PopupFrame.WidthRequest = width;
                    await InitializePopupAsync(mVmlSalesInvoice);
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region "Method"
        private async Task InitializePopupAsync(VmlSalesInvoice mVmlSalesInvoice)
        {
            try
            {
                await mVmlSalesInvoice.loadBookNow();

                BindingContext = mVmlSalesInvoice;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        private void ComboBox_ValueChanged(object sender, Syncfusion.Maui.Inputs.ComboBoxValueChangedEventArgs e)
        {
            if (sender is Syncfusion.Maui.Inputs.SfComboBox comboBox)
            {
                // Allow filtering while typing,
                // but don't let the suggestion list cover the Entry.
                comboBox.IsDropDownOpen = false;
            }
        }
        public async void SearchTappedAsync(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();

        }

        private async void OnBackgroundTapped(object sender, TappedEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void OkButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                //if typing perameter exist, add here and bind into selectedData
                _taskCompletionSource?.TrySetResult(selectedData);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                _taskCompletionSource?.TrySetException(ex);
            }
        }

        #endregion

        #region "System nav bar top, bottom padding"

        protected override void OnAppearing()
        {
            base.OnAppearing();

#if ANDROID
            ApplySystemInsets();

            System.Diagnostics.Debug.WriteLine(
                $"SYSTEM PADDING: " +
                $"L={SystemPadding.Left}, " +
                $"T={SystemPadding.Top}, " +
                $"R={SystemPadding.Right}, " +
                $"B={SystemPadding.Bottom}");

            System.Diagnostics.Debug.WriteLine(
                $"PAGE PADDING: " +
                $"L={Padding.Left}, " +
                $"T={Padding.Top}, " +
                $"R={Padding.Right}, " +
                $"B={Padding.Bottom}");
#endif
        }

#if ANDROID
        private void ApplySystemInsets()
        {
            var window = Platform.CurrentActivity?.Window;

            if (window == null)
                return;

            AndroidX.Core.View.WindowInsetsCompat? insets =
                AndroidX.Core.View.ViewCompat.GetRootWindowInsets(
                    window.DecorView);

            if (insets == null)
                return;

            var bars = insets.GetInsets(
                AndroidX.Core.View.WindowInsetsCompat.Type.SystemBars());

            var density = DeviceDisplay.MainDisplayInfo.Density;

            SearchGrid.Padding = new Thickness(
                bars.Left / density,
                bars.Top / density,
                bars.Right / density,
                bars.Bottom / density);

            System.Diagnostics.Debug.WriteLine(
                $"POPUP APPLIED INSETS: " +
                $"L={bars.Left}, " +
                $"T={bars.Top}, " +
                $"R={bars.Right}, " +
                $"B={bars.Bottom}");
        }
#endif
        #endregion
    }
}