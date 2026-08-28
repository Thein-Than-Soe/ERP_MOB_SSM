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
    public partial class FrmJobApplicantLanguageSkillSet : ContentPage
    {
        #region "Declaring"
        VmlJobProfile mVmlJobProfile;
        private DAT_APPLICANT_LANGUAGE_SKILL CurrentItem;

        #endregion
        #region "Constructor"
        public FrmJobApplicantLanguageSkillSet()
        {
            InitializeComponent();
        }
        public FrmJobApplicantLanguageSkillSet(DAT_APPLICANT_LANGUAGE_SKILL argSelectedLanguage, VmlJobProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            CurrentItem = argSelectedLanguage?? new DAT_APPLICANT_LANGUAGE_SKILL();
            LoadData();
        }
        #endregion

        #region "Method"
        private void LoadData()
        {
            // Set values into UI fields using x:Name
            EntryLanguage.Text = CurrentItem.Language;
            EntryReference.Text = CurrentItem.ReferenceNo;
            EntryRemark.Text = CurrentItem.Remark;

            PickerLanguageLevel.SelectedItem =
                mVmlJobProfile.LanguageLevelList.FirstOrDefault(x =>
                    x.LanguageLevelName_0_255 == CurrentItem.LanguageLevelName_0_255);

        }
        #endregion

        #region "Events"
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Assign values from UI back into the model
                CurrentItem.Language = EntryLanguage.Text;
                CurrentItem.ReferenceNo = EntryReference.Text;
                CurrentItem.Remark = EntryRemark.Text;

                var l_SelectedLevel = PickerLanguageLevel.SelectedItem as DAT_LANGUAGE_LEVEL;
                if (l_SelectedLevel != null)
                {
                    CurrentItem.LanguageLevelAsk = l_SelectedLevel.Ask;
                    CurrentItem.LanguageLevelName_0_255 = l_SelectedLevel.LanguageLevelName_0_255;
                }
                // Save api
                CurrentItem.StatusAsk = "1"; // Save key
                await mVmlJobProfile.saveApplicantLanguage(CurrentItem);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Save Error", ex.Message, "OK");
            }
        }

        private async void OnDeleteBtn_Clicked(object sender, EventArgs e)
        {
            CurrentItem.StatusAsk = "6"; //delete key
            await mVmlJobProfile.saveApplicantLanguage(CurrentItem);
            await Navigation.PopAsync();
        }


        #endregion

        #region "Private method"

        #endregion

    }
}
