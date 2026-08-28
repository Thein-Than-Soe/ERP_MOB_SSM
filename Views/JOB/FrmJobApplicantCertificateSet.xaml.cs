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
    public partial class FrmJobApplicantCertificateSet : ContentPage
    {
        #region "Declaring"
        VmlJobProfile mVmlJobProfile;
        private DAT_APPLICANT_CERTIFICATE CurrentItem;

        #endregion
        #region "Constructor"
        public FrmJobApplicantCertificateSet()
        {
            InitializeComponent();
        }
        public FrmJobApplicantCertificateSet(DAT_APPLICANT_CERTIFICATE argSelectedCertificate, VmlJobProfile existingViewModel)
        {
            InitializeComponent();
            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobProfile = existingViewModel;
            CurrentItem = argSelectedCertificate ?? new DAT_APPLICANT_CERTIFICATE();
            LoadData();
        }
        #endregion


        #region "Private method"
        private void LoadData()
        {
            // Set values into UI fields using x:Name
            EntryCertificateName.Text = CurrentItem.CertificateName_0_255;
            EntryInstituteName.Text = CurrentItem.InstituteName_0_255;
            EntryReference.Text = CurrentItem.ReferenceNo;
            EntryRemark.Text = CurrentItem.Remark;

            PickerCertificateType.SelectedItem =
                mVmlJobProfile.CertificateTypeList.FirstOrDefault(x =>
                    x.CertificateTypeName_0_255 == CurrentItem.CertificateTypeName_0_255);


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
                CurrentItem.CertificateName_0_255 = EntryCertificateName.Text;
                CurrentItem.InstituteName_0_255 = EntryInstituteName.Text;
                CurrentItem.ReferenceNo = EntryReference.Text;
                CurrentItem.Remark = EntryRemark.Text;

                var l_SelectedCertificateType = PickerCertificateType.SelectedItem as DAT_CERTIFICATE_TYPE;
                if (l_SelectedCertificateType != null)
                {
                    CurrentItem.CertificateTypeAsk = l_SelectedCertificateType.Ask;
                    CurrentItem.CertificateTypeName_0_255 = l_SelectedCertificateType.CertificateTypeName_0_255;
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
                await mVmlJobProfile.saveApplicantCertificate(CurrentItem);
                await Application.Current.MainPage.DisplayAlert("Save Success", "Successfully save", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Save Error", ex.Message, "OK");
            }
        }

        private async void OnDeleteBtn_Clicked(object sender, EventArgs e)
        {
            CurrentItem.StatusAsk = "6"; //delete key
            await mVmlJobProfile.saveApplicantCertificate(CurrentItem);
            await Navigation.PopAsync();
        }


        #endregion

    }
}
