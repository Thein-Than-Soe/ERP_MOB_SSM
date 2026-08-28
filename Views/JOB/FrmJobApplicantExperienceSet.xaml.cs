using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.JOB;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Globalization;

namespace CS.ERP_MOB.Views.JOB
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmJobApplicantExperienceSet : ContentPage
    {
        #region "Declaring"
        VmlJobProfile mVmlJobProfile;
        private DAT_APPLICANT_WORKING_EXPERIENCE CurrentItem;

        #endregion
        #region "Constructor"
        public FrmJobApplicantExperienceSet()
        {
            InitializeComponent();
        }
        public FrmJobApplicantExperienceSet(DAT_APPLICANT_WORKING_EXPERIENCE argSelectedExperience, VmlJobProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            CurrentItem = argSelectedExperience ?? new DAT_APPLICANT_WORKING_EXPERIENCE();
            LoadData();
        }
        #endregion

        #region "Method"
        private void LoadData()
        {
            // Set values into UI fields using x:Name
            EntryCompany.Text = CurrentItem.Company;
            EntryResponsibility.Text = CurrentItem.Responsibilities;
            EntryReasontoLeave.Text = CurrentItem.ReasontoLeave;
            EntryDesignation.Text = CurrentItem.DesignationName_0_255;

            PickerBusinessType.SelectedItem =
                mVmlJobProfile.BusinessTypeList.FirstOrDefault(x =>
                    x.BusinessTypeName_0_255 == CurrentItem.BusinessTypeName_0_255);
            PickerEmploymentType.SelectedItem =
                mVmlJobProfile.EmploymentTypeList.FirstOrDefault(x =>
                    x.EmploymentTypeName_0_255 == CurrentItem.EmploymentTypeName_0_255);

            // Get DateTime in local time from utility
            string FormattedStartDate = CurrentItem.SD; // e.g., "31/07/2025 09:14 PM"
            DateTime LocalStartDate = getDateTimeOfFormattedDate(FormattedStartDate);
            StartDatePicker.Date = LocalStartDate.Date;

            string FormattedEndDate = CurrentItem.ED; // e.g., "31/07/2025 09:14 PM"
            DateTime localEndDate = getDateTimeOfFormattedDate(FormattedEndDate);
            EndDatePicker.Date = localEndDate.Date;
        }
        #endregion

        #region "Events"
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Assign values from UI back into the model
                CurrentItem.Company = EntryCompany.Text;
                CurrentItem.Responsibilities = EntryResponsibility.Text;
                CurrentItem.ReasontoLeave = EntryReasontoLeave.Text;
                CurrentItem.DesignationName_0_255 = EntryDesignation.Text;

                var l_SelectedBusinessType = PickerBusinessType.SelectedItem as DAT_BUSINESS_TYPE;
                if (l_SelectedBusinessType != null)
                {
                    CurrentItem.BusinessTypeAsk = l_SelectedBusinessType.Ask;
                    CurrentItem.BusinessTypeName_0_255 = l_SelectedBusinessType.BusinessTypeName_0_255;
                }
                var l_SelectedEmploymentType = PickerEmploymentType.SelectedItem as DAT_EMPLOYMENT_TYPE;
                if (l_SelectedEmploymentType != null)
                {
                    CurrentItem.EmploymentTypeAsk = l_SelectedEmploymentType.Ask;
                    CurrentItem.EmploymentTypeName_0_255 = l_SelectedEmploymentType.EmploymentTypeName_0_255;
                }

                // Combine picker values,  Assign to model or send
                DateTime selectedSD = StartDatePicker.Date.ToUniversalTime();
                string formattedSD = selectedSD.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                CurrentItem.SD = formattedSD;

                DateTime selectedED = EndDatePicker.Date.ToUniversalTime();
                string formattedED = selectedED.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                CurrentItem.ED = formattedED;

                // Save api
                CurrentItem.StatusAsk = "1"; // Save key
                await mVmlJobProfile.saveApplicantExperience(CurrentItem);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Save Error", ex.Message, "OK");
            }
        }

        private async void OnDeleteBtn_Clicked(object sender, EventArgs e)
        {
            CurrentItem.StatusAsk = "6"; //delete key
            await mVmlJobProfile.saveApplicantExperience(CurrentItem);
            await Navigation.PopAsync();
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
