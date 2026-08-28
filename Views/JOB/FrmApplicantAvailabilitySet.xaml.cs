
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.ViewsModel.JOB;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace CS.ERP_MOB.Views.JOB
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmApplicantAvailabilitySet : ContentPage
    {
        #region "Declaring"
        VmlJobProfile mVmlJobProfile;
        private DAT_APPLICANT CurrentApplicant;

        #endregion
        #region "Constructor"
        public FrmApplicantAvailabilitySet()
        {
            InitializeComponent();
        }
        public FrmApplicantAvailabilitySet(VmlJobProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            if (existingViewModel != null && existingViewModel.Applicant.Count > 0)
            {
                CurrentApplicant = existingViewModel.Applicant[0];

                PickerAvailability.SelectedItem =
                    mVmlJobProfile.AvailabilityList.FirstOrDefault(x =>
                        x.ApplicantAvailabilityName_0_255 == mVmlJobProfile.Applicant[0].ApplicantAvailabilityName_0_255);
            }

        }
        #endregion

        #region "Method"
        #endregion

        #region "Events"
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Assign values from UI back into the model
                var l_Selected = PickerAvailability.SelectedItem as DAT_APPLICANT_AVAILABILITY;
                if (l_Selected != null)
                {
                    CurrentApplicant.ApplicantAvailabilityAsk = l_Selected.Ask;
                    CurrentApplicant.ApplicantAvailabilityName_0_255 = l_Selected.ApplicantAvailabilityName_0_255;
                }
                // Save api
                await mVmlJobProfile.saveApplicantProfile(CurrentApplicant);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Save Error", ex.Message, "OK");
            }
        }

        #endregion

        #region "Private method"

        #endregion

    }
}
