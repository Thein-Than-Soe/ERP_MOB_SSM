using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.RES;
using CS.ERP.PL.JOB.RES;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.Frame;
using CS.ERP_MOB.ViewsModel.JOB;
using Microsoft.Maui.Devices;
using RGPopup.Maui.Pages;
using RGPopup.Maui.Services;

namespace CS.ERP_MOB.Views.JOB
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmJobCompanyPop : PopupPage
    {
        #region "Declaring"
        RES_COMPANY mRES_COMPANY = new RES_COMPANY();
        VmlJobCompany mVmlJobCompany;
        private TaskCompletionSource<object> _taskCompletionSource;
        public Task<object> PopupClosedTask => _taskCompletionSource.Task;
        #endregion
        public FrmJobCompanyPop()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlJobCompany = new VmlJobCompany();


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
        public FrmJobCompanyPop(JSN_RES_LOAD_COMPANY argJSN_RES_LOAD_COMPANY)
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlJobCompany = new VmlJobCompany();
                mVmlJobCompany.bindCompanyType(argJSN_RES_LOAD_COMPANY.RES_COMPANY_TYPE);
                mVmlJobCompany.bindCompanyLocationCity(argJSN_RES_LOAD_COMPANY.RES_CITY);
                mVmlJobCompany.bindCompanyLocationCountry(argJSN_RES_LOAD_COMPANY.RES_COUNTRY);

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
                mRES_COMPANY = new RES_COMPANY();
                if (entCompanyName.Text != null)
                {
                    mRES_COMPANY.CompanyName_0_255 = entCompanyName.Text;
                }
                var l_SelectedCompanyType = pkrCompanyType.SelectedItem as RES_COMPANY_TYPE;
                if (l_SelectedCompanyType != null)
                {
                    mRES_COMPANY.CompanyTypeAsk = l_SelectedCompanyType.Ask;
                }
                var l_SelectedLocationCity = pkrLocationCity.SelectedItem as RES_CITY;
                if (l_SelectedLocationCity != null)
                {
                    mRES_COMPANY.CityAsk = l_SelectedLocationCity.Ask;
                }
                var l_SelectedLocationCountry = pkrLocationCountry.SelectedItem as RES_COUNTRY;
                if (l_SelectedLocationCountry != null)
                {
                    mRES_COMPANY.CountryAsk = l_SelectedLocationCountry.Ask;
                }



                if (mRES_COMPANY != null)
                {
                    _taskCompletionSource.SetResult(mRES_COMPANY);
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