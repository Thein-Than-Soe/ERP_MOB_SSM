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
    public partial class FrmJobApplicantEducationSet : ContentPage
    {
        #region "Declaring"
        VmlJobProfile mVmlJobProfile;
        private DAT_APPLICANT_EDUCATION CurrentItem;

        #endregion
        #region "Constructor"
        public FrmJobApplicantEducationSet()
        {
            InitializeComponent();
        }
        public FrmJobApplicantEducationSet(DAT_APPLICANT_EDUCATION argSelectedEducation, VmlJobProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            CurrentItem = argSelectedEducation ?? new DAT_APPLICANT_EDUCATION();
            LoadData();
        }
        #endregion


        #region "Private method"
        private void LoadData()
        {
            // Set values into UI fields using x:Name
            EntryMajor.Text = CurrentItem.Major;
            EntrySchool.Text = CurrentItem.ReferenceDetail;
            EntryGPA.Text = CurrentItem.GPA;
            EntryReference.Text = CurrentItem.ReferenceNo;
            EntryRemark.Text = CurrentItem.Remark;

            PickerEducationLevel.SelectedItem =
                mVmlJobProfile.EducationLevelList.FirstOrDefault(x =>
                    x.EducationLevelName_0_255 == CurrentItem.EducationLevelName_0_255);
            PickerCountry.SelectedItem =
                mVmlJobProfile.CountryList.FirstOrDefault(x =>
                    x.CountryName_0_255 == CurrentItem.CountryName_0_255);

            

            // Get DateTime in local time from utility
            string FormattedStartDate = CurrentItem.SD; // e.g., "31/07/2025 09:14 PM"
            DateTime LocalStartDate = getDateTimeOfFormattedDate(FormattedStartDate);
            StartDatePicker.Date = LocalStartDate.Date;

            string FormattedEndDate = CurrentItem.ED; // e.g., "31/07/2025 09:14 PM"
            DateTime localEndDate = getDateTimeOfFormattedDate(FormattedEndDate);
            EndDatePicker.Date = localEndDate.Date;

        }

        #endregion


        #region "Method"

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

        #region "Events"
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Assign values from UI back into the model
                CurrentItem.Major = EntryMajor.Text;
                CurrentItem.ReferenceDetail = EntrySchool.Text;
                CurrentItem.GPA = EntryGPA.Text; 
                CurrentItem.ReferenceNo = EntryReference.Text;
                CurrentItem.Remark = EntryRemark.Text;

                var l_SelectedEducationLevel = PickerEducationLevel.SelectedItem as DAT_EDUCATION_LEVEL;
                if (l_SelectedEducationLevel != null)
                {
                    CurrentItem.EducationLevelAsk = l_SelectedEducationLevel.Ask;
                    CurrentItem.EducationLevelName_0_255 = l_SelectedEducationLevel.EducationLevelName_0_255;
                }
                var l_SelectedCountry = PickerCountry.SelectedItem as DAT_COUNTRY;
                if (l_SelectedCountry != null)
                {
                    CurrentItem.CountryAsk = l_SelectedCountry.Ask;
                    CurrentItem.CountryName_0_255 = l_SelectedCountry.CountryName_0_255;
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
                await mVmlJobProfile.saveApplicantEducation(CurrentItem);

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Save Error", ex.Message, "OK");
            }
        }

        private async void OnDeleteBtn_Clicked(object sender, EventArgs e)
        {
            CurrentItem.StatusAsk = "6"; //delete key
            await mVmlJobProfile.saveApplicantEducation(CurrentItem);
            await Navigation.PopAsync();
        }


        #endregion

    }
}
