
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.RES;
using Microsoft.Maui.Controls;

namespace CS.ERP_MOB.Views.JOB
{
    public partial class FrmJobAdviceDtl : ContentPage
    {
        private DAT_ADVICE jobAdvice;
        public FrmJobAdviceDtl()
        {
            InitializeComponent();
            jobAdvice = new DAT_ADVICE();

            // Optionally bind to the UI
            BindingContext = jobAdvice;
        }
        public FrmJobAdviceDtl(DAT_ADVICE selectedjobAdvice)
        {
            InitializeComponent();
            jobAdvice = selectedjobAdvice;

            // Optionally bind to the UI
            BindingContext = jobAdvice;
        }
    }
}
