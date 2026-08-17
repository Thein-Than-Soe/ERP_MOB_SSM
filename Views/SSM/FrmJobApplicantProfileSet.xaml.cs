using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.SSM;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Globalization;

namespace CS.ERP_MOB.Views.SSM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmJobApplicantProfileSet : ContentPage
    {
        #region "Declaring"
        VmlSsmProfile mVmlJobProfile;
        private DAT_APPLICANT mDAT_APPLICANT;

        #endregion
        #region "Constructor"
        public FrmJobApplicantProfileSet()
        {
            InitializeComponent();
        }
        public FrmJobApplicantProfileSet(VmlSsmProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            if (existingViewModel.Applicant.Count > 0)
            {
                mDAT_APPLICANT = existingViewModel.Applicant[0];
            }
            else
            {
                mDAT_APPLICANT = new DAT_APPLICANT();
            }
                
            LoadData();
        }
        #endregion

        #region "Method"
        private void LoadData()
        {
            if (mVmlJobProfile.Applicant.Count > 0)
            {
                // Set values into UI fields using x:Name
                PickerCountryCode.SelectedItem =
                mVmlJobProfile.CountryList.FirstOrDefault(x =>
                    x.CountryMobileCode_0_50 == mVmlJobProfile.Applicant[0].CountryMobileCode_0_50);

                PickerGender.SelectedItem =
                    mVmlJobProfile.GenderList.FirstOrDefault(x =>
                        x.GenderName_0_255 == mVmlJobProfile.Applicant[0].GenderName_0_255);

                PickerNationality.SelectedItem =
                    mVmlJobProfile.NationalityList.FirstOrDefault(x =>
                        x.NationalityName_0_255 == mVmlJobProfile.Applicant[0].NationalityName_0_255);

                // Get DateTime in local time from utility
                string FormattedStartDate = mVmlJobProfile.Applicant[0].DOB; // e.g., "31/07/2025 09:14 PM"
                DateTime LocalStartDate = getDateTimeOfFormattedDate(FormattedStartDate);
                PickerDOB.Date = LocalStartDate.Date;
            }

        }
        #endregion

        #region "Events"
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                //profile
                mDAT_APPLICANT.Profile = mVmlJobProfile.mUploadFilePath;

                mDAT_APPLICANT.ApplicantName_0_255 = entUserName.Text;
                mDAT_APPLICANT.Email = entEmail.Text;
                mDAT_APPLICANT.Phone = entPhoneNumber.Text;
                mDAT_APPLICANT.Website = entWebsite.Text;

                var l_SelectedCountry = PickerCountryCode.SelectedItem as RES_COUNTRY_DTL;
                if (l_SelectedCountry != null)
                {
                    mDAT_APPLICANT.CountryAsk = l_SelectedCountry.Ask;
                    mDAT_APPLICANT.CountryName_0_255 = l_SelectedCountry.CountryName_0_255;
                }
                var l_SelectedGender = PickerGender.SelectedItem as ERP.PL.SYS.DAT.RES_GENDER;
                if (l_SelectedGender != null)
                {
                    mDAT_APPLICANT.GenderAsk = l_SelectedGender.Ask;
                    mDAT_APPLICANT.GenderName_0_255 = l_SelectedGender.GenderName_0_255;
                }
                var l_SelectedNationality = PickerNationality.SelectedItem as RES_NATIONALITY;
                if (l_SelectedNationality != null)
                {
                    mDAT_APPLICANT.NationalityAsk = l_SelectedNationality.Ask;
                    mDAT_APPLICANT.NationalityName_0_255 = l_SelectedNationality.NationalityName_0_255;
                }

                // Combine picker values,  Assign to model or send
                DateTime selectedDOB = PickerDOB.Date.ToUniversalTime();
                string formattedDOB = selectedDOB.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                mDAT_APPLICANT.DOB = formattedDOB;

                // Save api // Save key
                await mVmlJobProfile.saveApplicantProfile(mDAT_APPLICANT);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Save Error", ex.Message, "OK");
            }
        }

        #endregion

        #region "Private method"
        public static DateTime getDateTimeOfFormattedDate(string argDate)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(argDate))
                {
                    // If your string is always in this format
                    return DateTime.ParseExact(
                        argDate,
                        Common.mCommon.UserSetting.DateTimeFormatName_0_255,
                        CultureInfo.InvariantCulture
                    );
                }
                else
                {
                    return DateTime.Now;
                }
            }
            catch
            {
                throw; // don't use ex.InnerException; it might be null
            }
        }
        #endregion


    }
}
