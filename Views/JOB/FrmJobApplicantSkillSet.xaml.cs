
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
    public partial class FrmJobApplicantSkillSet : ContentPage
    {
        #region "Declaring"
        VmlJobProfile mVmlJobProfile;
        private DAT_APPLICANT_SKILL CurrentItem;

        #endregion
        #region "Constructor"
        public FrmJobApplicantSkillSet()
        {
            InitializeComponent();
        }
        public FrmJobApplicantSkillSet(DAT_APPLICANT_SKILL argSelectedSkill, VmlJobProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            CurrentItem = argSelectedSkill ?? new DAT_APPLICANT_SKILL();
            LoadData();
        }
        #endregion

        #region "Method"
        private void LoadData()
        {
            // Set values into UI fields using x:Name
            EntrySkill.Text = CurrentItem.SkillSet;
            EntryReference.Text = CurrentItem.ReferenceNo;
            EntryRemark.Text = CurrentItem.Remark;

            PickerSkillLevel.SelectedItem =
                mVmlJobProfile.SkillLevelList.FirstOrDefault(x =>
                    x.SkillLevelName_0_255 == CurrentItem.SkillLevelName_0_255);

        }
        #endregion

        #region "Events"
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Assign values from UI back into the model
                CurrentItem.SkillSet = EntrySkill.Text;
                CurrentItem.ReferenceNo = EntryReference.Text;
                CurrentItem.Remark = EntryRemark.Text;

                var l_SelectedSkillLevel = PickerSkillLevel.SelectedItem as DAT_SKILL_LEVEL;
                if (l_SelectedSkillLevel != null)
                {
                    CurrentItem.SkillLevelAsk = l_SelectedSkillLevel.Ask;
                    CurrentItem.SkillLevelName_0_255 = l_SelectedSkillLevel.SkillLevelName_0_255;
                }
                
                // Save api
                CurrentItem.StatusAsk = "1"; // Save key
                await mVmlJobProfile.saveApplicantSkill(CurrentItem);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Save Error", ex.Message, "OK");
            }
        }

        private async void OnDeleteBtn_Clicked(object sender, EventArgs e)
        {
            CurrentItem.StatusAsk = "6"; //delete key
            await mVmlJobProfile.saveApplicantSkill(CurrentItem);
            await Navigation.PopAsync();
        }


        #endregion

        #region "Private method"

        #endregion

    }
}
