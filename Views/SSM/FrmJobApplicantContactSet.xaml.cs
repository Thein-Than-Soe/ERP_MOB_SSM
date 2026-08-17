using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.ViewsModel.SSM;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace CS.ERP_MOB.Views.SSM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmJobApplicantContactSet : ContentPage
    {
        #region "Declaring"
        VmlSsmProfile mVmlJobProfile;
        private DAT_APPLICANT_CONTACT CurrentItem;

        #endregion
        #region "Constructor"
        public FrmJobApplicantContactSet()
        {
            InitializeComponent();
        }
        public FrmJobApplicantContactSet(DAT_APPLICANT_CONTACT argSelectedContact, VmlSsmProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            CurrentItem = argSelectedContact ?? new DAT_APPLICANT_CONTACT();
            LoadData();
        }
        #endregion

        #region "Method"
        private void LoadData()
        {
            // Set values into UI fields using x:Name
            EntryEmail.Text = CurrentItem.Email;
            EntryAddress.Text = CurrentItem.Address;
            entPhoneNumber.Text = CurrentItem.Mobile;
            PickerContactType.SelectedItem =
                mVmlJobProfile.ContactTypeList.FirstOrDefault(x =>
                    x.ContactTypeName_0_255 == CurrentItem.ContactTypeName_0_255);
            PickerCountryCode.SelectedItem =
                mVmlJobProfile.CountryList.FirstOrDefault(x =>
                    x.CountryMobileCode_0_50 == CurrentItem.CountryMobileCode_0_50);


            // Country / State / City cascade:
            // 1) select country instance from CountryList
            var country = mVmlJobProfile.CountryList?
                .FirstOrDefault(c => c.CountryName_0_255 == CurrentItem.CountryName_0_255);

            // assign to VM (this will cause PickerState.ItemsSource to reflect SelectedCountry.RES_STATE_DTL)
            mVmlJobProfile.SelectedCountry = country;
            var state = country?.RES_STATE_DTL?
                .FirstOrDefault(s => s.StateName_0_255 == CurrentItem.StateName_0_255);

            mVmlJobProfile.SelectedState = state;
            var city = state?.RES_CITY?
                .FirstOrDefault(ci => ci.CityName_0_255 == CurrentItem.CityName_0_255);

            mVmlJobProfile.SelectedCity = city;

        }
        #endregion

        #region "Events"
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Assign values from UI back into the model
                CurrentItem.Email = EntryEmail.Text;
                CurrentItem.Address = EntryAddress.Text;
                CurrentItem.Mobile = entPhoneNumber.Text;
                var l_SelectedContact= PickerContactType.SelectedItem as DAT_CONTACT_TYPE;
                CurrentItem.ContactTypeAsk = l_SelectedContact.Ask;
                var l_SelectedMobileCode= PickerCountryCode.SelectedItem as RES_COUNTRY_DTL;
                CurrentItem.CountryMobileAsk = l_SelectedMobileCode.Ask;


                var l_SelectedCountry= PickerCountry.SelectedItem as RES_COUNTRY_DTL;
                CurrentItem.CountryAsk = l_SelectedCountry.Ask;
                CurrentItem.CountryName_0_255 = l_SelectedCountry.CountryName_0_255;

                var l_SelectedState= PickerState.SelectedItem as RES_STATE_DTL;
                CurrentItem.StateAsk = l_SelectedState.Ask;
                CurrentItem.StateName_0_255 = l_SelectedState.StateName_0_255;


                var l_SelectedCity= PickerCity.SelectedItem as RES_CITY;
                CurrentItem.CityAsk = l_SelectedCity.Ask;
                CurrentItem.CityName_0_255 = l_SelectedCity.CityName_0_255;


                // Save api
                CurrentItem.StatusAsk = "1"; // Save key
                await mVmlJobProfile.saveApplicantContact(CurrentItem);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Save Error", ex.Message, "OK");
            }
        }

        private async void OnDeleteBtn_Clicked(object sender, EventArgs e)
        {
            CurrentItem.StatusAsk = "6"; //delete key
            await mVmlJobProfile.saveApplicantContact(CurrentItem);
            await Navigation.PopAsync();
        }


        #endregion

        #region "Private method"
        #endregion

    }
}
