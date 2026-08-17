using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Views.SSM;
using CS.ERP_MOB.ViewsModel.SSM;
using Stripe;
using System.Globalization;
//using System.Windows.Forms;

namespace CS.ERP_MOB.Views.SSM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmSsmProfile : ContentView
    {
        #region "Declaring"
        VmlSsmProfile mVmlJobProfile;
        // At the top of your class
        private FileResult selectedImageFile;
        string filePath;

        #endregion
        #region "Constructor"
        public FrmSsmProfile()
        {
            try
            {
                InitializeComponent();
                mVmlJobProfile = new VmlSsmProfile();
                mVmlJobProfile.mJSN_REQ_APPLICANT_DTL.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;

                _ = LoadDataAsync();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        private async Task LoadDataAsync()
        {
            await mVmlJobProfile.loadApplicant();
            await mVmlJobProfile.getApplicantDetail();

            BindingContext = mVmlJobProfile;
            await Task.Delay(500);
        }
        #endregion

        #region "Private Method"
        private bool isContactDropdownVisible = false;
        private void OnContactDropdownToggleTapped(object sender, EventArgs e)
        {
            isContactDropdownVisible = !isContactDropdownVisible;
            Contact_DropdownContent.IsVisible = isContactDropdownVisible;
            // Optionally update icon to up/down
            Contact_DropdownToggleIcon.Text = isContactDropdownVisible ? "\uf077" : "\uf078"; // up / down chevron
        }
        private bool isEduDropdownVisible = false;
        private void OnEduDropdownToggleTapped(object sender, EventArgs e)
        {
            isEduDropdownVisible = !isEduDropdownVisible;
            Edu_DropdownContent.IsVisible = isEduDropdownVisible;

            // Optionally update icon to up/down
            Edu_DropdownToggleIcon.Text = isEduDropdownVisible ? "\uf077" : "\uf078"; // up / down chevron
        }

        private bool isExpDropdownVisible = false;
        private void OnExpDropdownToggleTapped(object sender, EventArgs e)
        {
            isExpDropdownVisible = !isExpDropdownVisible;
            Exp_DropdownContent.IsVisible = isExpDropdownVisible;
            // Optionally update icon to up/down
            Exp_DropdownToggleIcon.Text = isExpDropdownVisible ? "\uf077" : "\uf078"; // up / down chevron
        }
        private bool isSkillDropdownVisible = false;
        private void OnSkillDropdownToggleTapped(object sender, EventArgs e)
        {
            isSkillDropdownVisible = !isSkillDropdownVisible;
            Skill_DropdownContent.IsVisible = isSkillDropdownVisible;
            // Optionally update icon to up/down
            Skill_DropdownToggleIcon.Text = isSkillDropdownVisible ? "\uf077" : "\uf078"; // up / down chevron
        }
        private bool isLangDropdownVisible = false;
        private void OnLangDropdownToggleTapped(object sender, EventArgs e)
        {
            isLangDropdownVisible = !isLangDropdownVisible;
            Lang_DropdownContent.IsVisible = isLangDropdownVisible;
            // Optionally update icon to up/down
            Lang_DropdownToggleIcon.Text = isLangDropdownVisible ? "\uf077" : "\uf078"; // up / down chevron
        }
        private bool isCertDropdownVisible = false;
        private void OnCertDropdownToggleTapped(object sender, EventArgs e)
        {
            isCertDropdownVisible = !isCertDropdownVisible;
            Cert_DropdownContent.IsVisible = isCertDropdownVisible;
            // Optionally update icon to up/down
            Cert_DropdownToggleIcon.Text = isCertDropdownVisible ? "\uf077" : "\uf078"; // up / down chevron
        }
        private bool isSocialDropdownVisible = false;
        private void OnSocialDropdownToggleTapped(object sender, EventArgs e)
        {
            isSocialDropdownVisible = !isSocialDropdownVisible;
            Social_DropdownContent.IsVisible = isSocialDropdownVisible;
            // Optionally update icon to up/down
            Social_DropdownToggleIcon.Text = isSocialDropdownVisible ? "\uf077" : "\uf078"; // up / down chevron
        }


        #endregion

        #region "Load data"

        #endregion

        #region "Card routing of profile"
        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            // Navigate or perform action for Personal
            await Navigation.PushAsync(new FrmJobApplicantProfileSet(mVmlJobProfile));
        }

        private async void OnSummaryCardTapped(object sender, EventArgs e)
        {
            // Navigate or perform action for Personal
            await Navigation.PushAsync(new FrmApplicantSummarySet(mVmlJobProfile));
        }
        private async void OnAvailabilityCardTapped(object sender, EventArgs e)
        {
            if (mVmlJobProfile.AvailabilityList != null && mVmlJobProfile.AvailabilityList.Count != 0) 
            {
                //await Navigation.PushAsync(new FrmApplicantAvailabilitySet(mVmlJobProfile));
            }
        }
        private async void OnSalaryCardTapped(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new FrmApplicantSalarySet(mVmlJobProfile));
        }

        #endregion

        #region "Route to entry page"

        // Routing to entry forms
        private async void contact_lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                //if (!Utility.checkButtonAccess("Edit"))
                //{
                var selectedItem = e.SelectedItem as DAT_APPLICANT_CONTACT;
                if (selectedItem != null)
                {
                    await Navigation.PushAsync(new FrmJobApplicantContactSet(selectedItem, mVmlJobProfile));
                }
                else { return; }


                    ((ListView)sender).SelectedItem = null;
                //}
                //else
                //{
                //    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                //}
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private async void edu_lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                //if (!Utility.checkButtonAccess("Edit"))
                //{
                var selectedItem = e.SelectedItem as DAT_APPLICANT_EDUCATION;
                if (selectedItem != null)
                {
                    //await Navigation.PushAsync(new FrmJobApplicantEducationSet(selectedItem, mVmlJobProfile));
                }
                else { return; }
                    ((ListView)sender).SelectedItem = null;
                //}
                //else
                //{
                //    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                //}
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private async void exp_lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                //if (!Utility.checkButtonAccess("Edit"))
                //{
                var selectedItem = e.SelectedItem as DAT_APPLICANT_WORKING_EXPERIENCE;
                if (selectedItem != null)
                {
                    //await Navigation.PushAsync(new FrmJobApplicantExperienceSet(selectedItem, mVmlJobProfile));
                }
                else { return; }
                    ((ListView)sender).SelectedItem = null;
                //}
                //else
                //{
                //    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                //}
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private async void skill_lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                //if (!Utility.checkButtonAccess("Edit"))
                //{
                var selectedItem = e.SelectedItem as DAT_APPLICANT_SKILL;
                if (selectedItem != null)
                {

                    //await Navigation.PushAsync(new FrmJobApplicantSkillSet(selectedItem, mVmlJobProfile));
                }
                else { return; }
                    ((ListView)sender).SelectedItem = null;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private async void lang_lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                //if (!Utility.checkButtonAccess("Edit"))
                //{
                var selectedItem = e.SelectedItem as DAT_APPLICANT_LANGUAGE_SKILL;
                if (selectedItem != null)
                {
                    //await Navigation.PushAsync(new FrmJobApplicantLanguageSkillSet(selectedItem, mVmlJobProfile));
                }
                else { return; }
                    ((ListView)sender).SelectedItem = null;
                //}
                //else
                //{
                //    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                //}
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private async void cert_lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                //if (!Utility.checkButtonAccess("Edit"))
                //{
                var selectedItem = e.SelectedItem as DAT_APPLICANT_CERTIFICATE;
                if (selectedItem != null)
                {
                    //await Navigation.PushAsync(new FrmJobApplicantCertificateSet(selectedItem, mVmlJobProfile));
                }
                else { return; }
                    ((ListView)sender).SelectedItem = null;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private async void social_lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                //if (!Utility.checkButtonAccess("Edit"))
                //{
                var selectedItem = e.SelectedItem as DAT_APPLICANT_SOCIAL_AFFAIRS;
                if (selectedItem != null)
                {
                    //await Navigation.PushAsync(new FrmJobApplicantSocialSet(selectedItem, mVmlJobProfile));
                }
                else { return; }
                    ((ListView)sender).SelectedItem = null;
                //}
                //else
                //{
                //    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                //}
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }



        private async void AddEducation_Clicked(object sender, EventArgs e)
        {
            try
            {
                //await Navigation.PushAsync(new FrmJobApplicantEducationSet(new DAT_APPLICANT_EDUCATION(), mVmlJobProfile));
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error going to education add new page", ex.Message, "OK");
            }
        }

        private async void AddExperience_Clicked(object sender, EventArgs e)
        {
            try
            {
                //await Navigation.PushAsync(new FrmJobApplicantExperienceSet(new DAT_APPLICANT_WORKING_EXPERIENCE(), mVmlJobProfile));
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error going to experience add new page", ex.Message, "OK");
            }
        }

        private async void AddSkill_Clicked(object sender, EventArgs e)
        {
            try
            {
                //await Navigation.PushAsync(new FrmJobApplicantSkillSet(new DAT_APPLICANT_SKILL(), mVmlJobProfile));
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error going to skill add new page", ex.Message, "OK");
            }
        }

        private async void AddLanguage_Clicked(object sender, EventArgs e)
        {
            try
            {
                //await Navigation.PushAsync(new FrmJobApplicantLanguageSkillSet(new DAT_APPLICANT_LANGUAGE_SKILL(), mVmlJobProfile));
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error going to language add new page", ex.Message, "OK");
            }
        }

        private async void AddCertificate_Clicked(object sender, EventArgs e)
        {
            try
            {
                //await Navigation.PushAsync(new FrmJobApplicantCertificateSet(new DAT_APPLICANT_CERTIFICATE(), mVmlJobProfile));
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error going to certificate add new page", ex.Message, "OK");
            }
        }

        private async void AddSocial_Clicked(object sender, EventArgs e)
        {
            try
            {
                //await Navigation.PushAsync(new FrmJobApplicantSocialSet(new DAT_APPLICANT_SOCIAL_AFFAIRS(), mVmlJobProfile));
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error going to social add new page", ex.Message, "OK");
            }
        }

        private async void AddContact_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new FrmJobApplicantContactSet(new DAT_APPLICANT_CONTACT(), mVmlJobProfile));
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error going to contact add new page", ex.Message, "OK");
            }
        }

        #endregion

    }
}