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
    public partial class FrmJobApplicantSocialSet : ContentPage
    {
        #region "Declaring"
        VmlJobProfile mVmlJobProfile;
        private DAT_APPLICANT_SOCIAL_AFFAIRS CurrentItem;

        #endregion
        #region "Constructor"
        public FrmJobApplicantSocialSet()
        {
            InitializeComponent();
        }
        public FrmJobApplicantSocialSet(DAT_APPLICANT_SOCIAL_AFFAIRS argSelectedEducation, VmlJobProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            CurrentItem = argSelectedEducation ?? new DAT_APPLICANT_SOCIAL_AFFAIRS();
            LoadData();
        }
        #endregion

        #region "Method"
        private void LoadData()
        {
            // Set values into UI fields using x:Name
            EntryEventName.Text = CurrentItem.NameofEvent;
            EntryRole.Text = CurrentItem.Role;
            EntryRemark.Text = CurrentItem.Remark;

            PickerCountry.SelectedItem =
                mVmlJobProfile.CountryList.FirstOrDefault(x =>
                    x.CountryName_0_255 == CurrentItem.CountryName_0_255);

        }
        #endregion

        #region "Events"
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Assign values from UI back into the model
                CurrentItem.NameofEvent = EntryEventName.Text;
                CurrentItem.Role = EntryRole.Text;
                CurrentItem.Remark = EntryRemark.Text;

                var l_SelectedSkillLevel = PickerCountry.SelectedItem as DAT_COUNTRY;
                if (l_SelectedSkillLevel != null)
                {
                    CurrentItem.CountryAsk = l_SelectedSkillLevel.Ask;
                    CurrentItem.CountryName_0_255 = l_SelectedSkillLevel.CountryName_0_255;
                }

                // Save api
                CurrentItem.StatusAsk = "1"; // Save key
                await mVmlJobProfile.saveApplicantSocialAffairs(CurrentItem);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Save Error", ex.Message, "OK");
            }
        }

        private async void OnDeleteBtn_Clicked(object sender, EventArgs e)
        {
            CurrentItem.StatusAsk = "6"; //delete key
            await mVmlJobProfile.saveApplicantSocialAffairs(CurrentItem);
            await Navigation.PopAsync();
        }


        #endregion

        #region "Private method"

        #endregion

    }
}
