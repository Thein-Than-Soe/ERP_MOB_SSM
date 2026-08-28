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
    public partial class FrmJobAdvicePop : PopupPage
    {
        #region "Declaring"
        DAT_ADVICE mDAT_ADVICE = new DAT_ADVICE();
        VmlJobAdvice mVmlJobAdvice;
        private TaskCompletionSource<object> _taskCompletionSource;
        public Task<object> PopupClosedTask => _taskCompletionSource.Task;
        #endregion
        public FrmJobAdvicePop()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlJobAdvice = new VmlJobAdvice();


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
        public FrmJobAdvicePop(JSN_RES_LOAD_ADVICE argJSN_RES_LOAD_ADVICE)
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlJobAdvice = new VmlJobAdvice();
                mVmlJobAdvice.bindAdviceType(argJSN_RES_LOAD_ADVICE.DAT_ADVICE_TYPE);

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
                mDAT_ADVICE = new DAT_ADVICE();
                if (entAdviceName.Text != null)
                {
                    mDAT_ADVICE.AdviceName_0_255 = entAdviceName.Text;
                }
                var l_SelectedAdviceType = pkrAdviceType.SelectedItem as DAT_ADVICE_TYPE;
                if (l_SelectedAdviceType != null)
                {
                    mDAT_ADVICE.AdviceTypeAsk = l_SelectedAdviceType.Ask;
                }
                //var l_SelectedAdviceCode = pkrAdviceCode.SelectedItem as DAT_ADVICE_TYPE;
                //if (l_SelectedAdviceCode != null)
                //{
                //    mDAT_ADVICE.AdviceTypeAsk = l_SelectedAdviceCode.Ask;
                //}



                if (mDAT_ADVICE != null)
                {
                    _taskCompletionSource.SetResult(mDAT_ADVICE);
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