using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.HCM.RES;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.RES;
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
    public partial class FrmJobSearchPop : PopupPage
    {
        #region "Declaring"
        DAT_JOB_SEARCH mDAT_JOB_SEARCH = new DAT_JOB_SEARCH();
        VmlJobSearch mVmlJobSearch;
        private TaskCompletionSource<object> _taskCompletionSource;
        public Task<object> PopupClosedTask => _taskCompletionSource.Task;
        #endregion
        public FrmJobSearchPop()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlJobSearch = new VmlJobSearch();


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
        public FrmJobSearchPop(JSN_LOAD_JOB_VACANCY argJSN_LOAD_JOB_VACANCY)
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlJobSearch = new VmlJobSearch();
                mVmlJobSearch.bindJobClassification(argJSN_LOAD_JOB_VACANCY.DAT_JOB_CLASSIFICATION);
                mVmlJobSearch.bindJobDesignation(argJSN_LOAD_JOB_VACANCY.DAT_DESIGNATION);
                mVmlJobSearch.bindJobVacancyAvailability(argJSN_LOAD_JOB_VACANCY.DAT_VACANCY_AVAILABILITY);
                mVmlJobSearch.bindJobLocationCountry(argJSN_LOAD_JOB_VACANCY.RES_COUNTRY_DTL);

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
                mDAT_JOB_SEARCH = new DAT_JOB_SEARCH();
                int l_InputCount = 0;
                if (entVacancyName.Text != null)
                {
                    mDAT_JOB_SEARCH.DesignationName_0_255 = entVacancyName.Text;
                    l_InputCount += 1;
                }
                var l_SelectedJobClassification = pkrJobClassification.SelectedItem as DAT_JOB_CLASSIFICATION;
                if (l_SelectedJobClassification != null)
                {
                    mDAT_JOB_SEARCH.JobClassificationAsk = l_SelectedJobClassification.Ask;
                    l_InputCount += 1;
                }
                var l_SelectedDesignation = pkrJobDesignation.SelectedItem as DAT_DESIGNATION;
                if (l_SelectedDesignation != null)
                {
                    mDAT_JOB_SEARCH.DesignationAsk = l_SelectedDesignation.Ask;
                    l_InputCount += 1;
                }
                var l_SelectedVacancyAvailability = pkrVacancyAvailability.SelectedItem as DAT_VACANCY_AVAILABILITY;
                if (l_SelectedVacancyAvailability != null)
                {
                    mDAT_JOB_SEARCH.VacancyAvailabilityAsk = l_SelectedVacancyAvailability.Ask;
                    l_InputCount += 1;
                }
                
                var l_SelectedJobLocationCountry = pkrVacancyLocation.SelectedItem as RES_COUNTRY_DTL;
                if (l_SelectedJobLocationCountry != null)
                {
                    mDAT_JOB_SEARCH.CountryAsk = l_SelectedJobLocationCountry.Ask;
                    l_InputCount += 1;
                }


                if (l_InputCount != 0)
                {
                    _taskCompletionSource.SetResult(mDAT_JOB_SEARCH);
                    l_InputCount = 0;
                }
                else
                {
                    _taskCompletionSource.SetResult(null);
                }
                //if (mDAT_JOB_SEARCH != null)
                //{
                //    _taskCompletionSource.SetResult(mDAT_JOB_SEARCH);
                //}
                //else
                //{
                //    _taskCompletionSource.SetResult(null);
                //}
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
    }
}