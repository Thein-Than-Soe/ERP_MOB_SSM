
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.ViewsModel.SSM;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace CS.ERP_MOB.Views.SSM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmApplicantSummarySet : ContentPage
    {
        #region "Declaring"
        VmlSsmProfile mVmlJobProfile;
        private DAT_APPLICANT CurrentApplicant;

        #endregion
        #region "Constructor"
        public FrmApplicantSummarySet()
        {
            InitializeComponent();
        }
        public FrmApplicantSummarySet(VmlSsmProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            if (existingViewModel != null && existingViewModel.Applicant.Count > 0)
            {
                CurrentApplicant = existingViewModel.Applicant[0];
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
                CurrentApplicant.ProfessionalSummary = EntrySummary.Text;
                // Save api
                CurrentApplicant.StatusAsk = "1";
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
