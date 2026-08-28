using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.RES;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.Frame;
using CS.ERP_MOB.ViewsModel.JOB;
using Microsoft.Maui.Devices;
using RGPopup.Maui.Pages;
using RGPopup.Maui.Services;

namespace CS.ERP_MOB.Views.JOB
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmJobCommunityPop : PopupPage
    {
        #region "Declaring"
        DAT_COMMUNITY mDAT_COMMUNITY = new DAT_COMMUNITY();
        VmlJobCommunity mVmlJobCommunity;
        private TaskCompletionSource<object> _taskCompletionSource;
        public Task<object> PopupClosedTask => _taskCompletionSource.Task;
        #endregion
        public FrmJobCommunityPop()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlJobCommunity = new VmlJobCommunity();


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
        public FrmJobCommunityPop(JSN_RES_LOAD_COMMUNITY argJSN_RES_LOAD_COMMUNITY)
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlJobCommunity = new VmlJobCommunity();
                mVmlJobCommunity.bindCommunityType(argJSN_RES_LOAD_COMMUNITY.DAT_COMMUNITY_TYPE);

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

                await PopupNavigation.Instance.PopAsync();
                mDAT_COMMUNITY = new DAT_COMMUNITY();
                if (entCommunityName.Text != null)
                {
                    mDAT_COMMUNITY.CommunityName_0_255 = entCommunityName.Text;
                }
                var l_SelectedCommunityType = pkrCommunityType.SelectedItem as DAT_COMMUNITY_TYPE;
                if (l_SelectedCommunityType != null)
                {
                    mDAT_COMMUNITY.CommunityTypeAsk = l_SelectedCommunityType.Ask;
                }



                if (mDAT_COMMUNITY != null)
                {
                    _taskCompletionSource.SetResult(mDAT_COMMUNITY);
                }
                else
                {
                    _taskCompletionSource.SetResult(null);
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
    }
}