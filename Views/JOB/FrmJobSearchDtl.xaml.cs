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
    public partial class FrmJobSearchDtl : ContentPage
    {
        #region "Declaring"
        public DAT_JOB_SEARCH jobVacancy;
        VmlJobSearch mVmlJobSearch;
        #endregion
        #region "Constructor"
        public FrmJobSearchDtl()
        {
            try
            {
                InitializeComponent();

                this.BindingContext = mVmlJobSearch = new VmlJobSearch();

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        public FrmJobSearchDtl(DAT_JOB_SEARCH selectedJobVacancy)
        {
            InitializeComponent();

            // Optionally bind to the UI
            this.BindingContext = mVmlJobSearch = new VmlJobSearch();

            // Ensure jobVacancy is initialized before accessing its properties
            jobVacancy = new DAT_JOB_SEARCH();
            jobVacancy.Ask = selectedJobVacancy?.Ask;

            if (jobVacancy.Ask != null) 
            {
                _ = mVmlJobSearch.getJobVacancyDtl(jobVacancy);
            }
        }
        public FrmJobSearchDtl(DAT_JOB_SEARCH selectedJobVacancy, VmlJobSearch existingViewModel)
        {
            InitializeComponent();

            // Use the existing ViewModel from ContentView
            this.BindingContext = mVmlJobSearch = existingViewModel;

            jobVacancy = new DAT_JOB_SEARCH { Ask = selectedJobVacancy?.Ask };

            if (jobVacancy.Ask != null)
            {
                _ = mVmlJobSearch.getJobVacancyDtl(jobVacancy);
            }
        }


        #endregion

        #region "Private Method"
        #endregion

        #region "Event"

        #endregion
    }
}