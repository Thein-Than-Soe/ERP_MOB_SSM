using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.HMS.RES;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.Frame;
using CS.ERP_MOB.ViewsModel.SSM;
using Microsoft.Maui.Devices;
using RGPopup.Maui.Pages;
using RGPopup.Maui.Services;

namespace CS.ERP_MOB.Views.SSM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmSsmOrderBookPop : PopupPage
    {
        #region "Declaring"
        RES_SALE_ORDER mRES_SALE_ORDER = new RES_SALE_ORDER();
        VmlSsmOrderBook mVmlSsmOrderBook;
        private TaskCompletionSource<object> _taskCompletionSource;
        public Task<object> PopupClosedTask => _taskCompletionSource.Task;
        #endregion
        public FrmSsmOrderBookPop()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlSsmOrderBook = new VmlSsmOrderBook();


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
        public FrmSsmOrderBookPop(JSN_SALE_ORDER_JUN argJSN_SALE_ORDER_JUN)
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlSsmOrderBook = new VmlSsmOrderBook();

                //Bind load data for picker here  mmn
                //mVmlSsmOrderBook.bindCustomer(argJSN_SALE_ORDER_JUN.RES_USER_LST);

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
                mRES_SALE_ORDER = new RES_SALE_ORDER();
                var l_SelectedCustomer = pkrCustomer.SelectedItem as RES_SALE_ORDER;
                if (l_SelectedCustomer != null)
                {
//mmn check list here
                    //mRES_SALE_ORDER.CustomerAsk = l_SelectedCustomer.Ask;
                }
                if (entCode.Text != null)
                {
                    mRES_SALE_ORDER.OrderCode_0_50 = entCode.Text;
                }

                if (mRES_SALE_ORDER != null)
                {
                    _taskCompletionSource.SetResult(mRES_SALE_ORDER);
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