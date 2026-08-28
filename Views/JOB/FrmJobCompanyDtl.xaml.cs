using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.CHT;
using CS.ERP_MOB.ViewsModel.JOB;
using Maui.DataGrid;
using Newtonsoft.Json.Linq;
using Stripe;
using System.Diagnostics;
//using System.Windows.Forms;

namespace CS.ERP_MOB.Views.JOB
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmJobCompanyDtl : ContentPage
    {
        #region "Declaring"
        public RES_COMPANY jobCompany;
        VmlJobCompany mVmlJobCompany;
        #endregion
        #region "Constructor"
        public FrmJobCompanyDtl()
        {
            try
            {
                InitializeComponent();

                this.BindingContext = mVmlJobCompany = new VmlJobCompany();

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        public FrmJobCompanyDtl(RES_COMPANY selectedjobCompany)
        {
            InitializeComponent();

            // Optionally bind to the UI
            this.BindingContext = mVmlJobCompany = new VmlJobCompany();

            // Ensure jobCompany is initialized before accessing its properties
            jobCompany = new RES_COMPANY();
            jobCompany.Ask = selectedjobCompany?.Ask;

            if (jobCompany.Ask != null)
            {
                _ = mVmlJobCompany.getJobCompanyDtl(jobCompany);
            }
        }
        public FrmJobCompanyDtl(RES_COMPANY selectedjobCompany, VmlJobCompany argExistingViewModel)
        {
            InitializeComponent();

            // Optionally bind to the UI
            this.BindingContext = mVmlJobCompany = argExistingViewModel;

            // Ensure jobCompany is initialized before accessing its properties
            jobCompany = new RES_COMPANY();
            jobCompany.Ask = selectedjobCompany?.Ask;

            if (jobCompany.Ask != null)
            {
                _ = mVmlJobCompany.getJobCompanyDtl(jobCompany);
            }
        }


        #endregion

        #region "Private Method"
        #endregion

        #region "Event"
        private async void lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                //if (!Utility.checkButtonAccess("Edit"))
                //{
                var selectedItem = e.SelectedItem as DAT_JOB_SEARCH;
                if (selectedItem != null)
                {
                    await Navigation.PushAsync(new FrmJobSearchDtl(selectedItem));

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


        private async void OnLabelTapped(object sender, EventArgs e)
        {
            await Launcher.Default.OpenAsync("https://cocobubbletea.com/");
        }

        #endregion
    }
}