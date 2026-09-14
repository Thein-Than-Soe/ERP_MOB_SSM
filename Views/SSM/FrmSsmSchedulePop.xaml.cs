using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.AMS.RES;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.HMS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.Frame;
using CS.ERP_MOB.ViewsModel.SSM;
using Microsoft.Maui.Devices;
using RGPopup.Maui.Pages;
using RGPopup.Maui.Services;
using System.Diagnostics;

namespace CS.ERP_MOB.Views.SSM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmSsmSchedulePop : PopupPage
    {
        #region "Declaring"
        DAT_FRONT_DESK mDAT_FRONT_DESK = new DAT_FRONT_DESK();
        VmlSchedule mVmlSchedule;
        private TaskCompletionSource<object> _taskCompletionSource;
        public Task<object> PopupClosedTask => _taskCompletionSource.Task;
        #endregion
        public FrmSsmSchedulePop()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlSchedule = new VmlSchedule();


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
        public FrmSsmSchedulePop(VmlSchedule mVmlSchedule)
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlSchedule;
                
                _taskCompletionSource = new TaskCompletionSource<object>();

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

        public async void SearchTappedAsync(object sender, EventArgs e)
        {
            //await PopupNavigation.Instance.PopAsync();

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

                var l_SelectedCustomer = pkrCustomer.SelectedItem as RES_USER_LST;

                DAT_FRONT_DESK selectedFrontDesk = new DAT_FRONT_DESK();

                if (l_SelectedCustomer != null &&
                    !string.IsNullOrWhiteSpace(l_SelectedCustomer.Ask))
                {
                    selectedFrontDesk.UserAsk = l_SelectedCustomer.Ask;
                }

                _taskCompletionSource?.TrySetResult(selectedFrontDesk);

                await PopupNavigation.Instance.PopAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                _taskCompletionSource?.TrySetException(ex);
            }
        }      
    }
}