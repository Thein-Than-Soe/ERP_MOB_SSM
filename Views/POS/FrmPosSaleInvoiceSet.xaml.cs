
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.RES;
using Microsoft.Maui.Controls;

namespace CS.ERP_MOB.Views.POS
{
    public partial class FrmPosSaleInvoiceSet : ContentPage
    {
        private RES_SALE_INVOICE invoice;
        public FrmPosSaleInvoiceSet()
        {
            InitializeComponent();
            invoice = new RES_SALE_INVOICE();

            // Optionally bind to the UI
            BindingContext = invoice;
        }
        public FrmPosSaleInvoiceSet(RES_SALE_INVOICE selectedInvoice)
        {
            InitializeComponent();
            invoice = selectedInvoice;

            // Optionally bind to the UI
            BindingContext = invoice;
        }
    }
}
