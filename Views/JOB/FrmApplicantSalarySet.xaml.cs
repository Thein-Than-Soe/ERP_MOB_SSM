
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
    public partial class FrmApplicantSalarySet : ContentPage
    {
        #region "Declaring"
        VmlJobProfile mVmlJobProfile;
        private DAT_APPLICANT CurrentApplicant;

        #endregion
        #region "Constructor"
        public FrmApplicantSalarySet()
        {
            InitializeComponent();
        }
        public FrmApplicantSalarySet(VmlJobProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            if (mVmlJobProfile != null && mVmlJobProfile.Applicant.Count > 0) {
                LoadData();
            }
        }
        #endregion

        #region "Method"
        private void LoadData()
        {

            CurrentApplicant = mVmlJobProfile.Applicant[0];
            // Set values into UI fields using x:Name
            EntrySalary.Text = CurrentApplicant.ExpectedSalary;

            PickerCurrency.SelectedItem =
                mVmlJobProfile.CurrencyList.FirstOrDefault(x =>
                    x.CurrencyCode_0_50 == CurrentApplicant.CurrencyCode_0_50);

        }

        #endregion

        #region "Events"
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Assign values from UI back into the model
                CurrentApplicant.ExpectedSalary = EntrySalary.Text;
                var l_Selected = PickerCurrency.SelectedItem as RES_CURRENCY;
                if (l_Selected != null)
                {
                    CurrentApplicant.CurrencyAsk = l_Selected.Ask;
                    CurrentApplicant.CurrencyCode_0_50 = l_Selected.CurrencyCode_0_50;
                }

                // Save api
                //CurrentApplicant.StatusAsk = "1";
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
