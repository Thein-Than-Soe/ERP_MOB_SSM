using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.AMS.RES;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.HMS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.Frame;
using CS.ERP_MOB.ViewsModel.SYS;
using Microsoft.Maui.Devices;
using RGPopup.Maui.Pages;
using RGPopup.Maui.Services;
using System.Diagnostics;

namespace CS.ERP_MOB.Views.SYS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmSysMyPaymentPop : PopupPage
    {
        #region "Declaring"
        VmlMyPayment mVmlMyPayment;
        RES_SALE_PAYMENT selectedData = new RES_SALE_PAYMENT();
        private TaskCompletionSource<object> _taskCompletionSource;
        public Task<object> PopupClosedTask => _taskCompletionSource.Task;


        #endregion

        #region "Constructor"
        public FrmSysMyPaymentPop()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlMyPayment = new VmlMyPayment();


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
        public FrmSysMyPaymentPop(VmlMyPayment mVmlMyPayment)
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
                    await InitializePopupAsync(mVmlMyPayment);
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region "Method"
        private async Task InitializePopupAsync(VmlMyPayment mVmlMyPayment)
        {
            try
            {
                await mVmlMyPayment.loadSalePayHis();

                BindingContext = mVmlMyPayment;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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


    }
}