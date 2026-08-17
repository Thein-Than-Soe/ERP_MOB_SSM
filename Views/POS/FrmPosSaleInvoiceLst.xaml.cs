using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.CHT;
using CS.ERP_MOB.ViewsModel.POS;
using Maui.DataGrid;
using Newtonsoft.Json.Linq;
using Stripe;
using System.Diagnostics;
//using System.Windows.Forms;

namespace CS.ERP_MOB.Views.POS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmPosSaleInvoiceLst : ContentView
    {
        #region "Declaring"
        VmlSalesInvoice mVmlSalesInvoice;
        #endregion
        #region "Constructor"
        public FrmPosSaleInvoiceLst()
        {
            try
            {
                InitializeComponent();

                BindingContext = mVmlSalesInvoice = new VmlSalesInvoice();
                mVmlSalesInvoice.mJSN_REQ_SALE_INVOICE_JUN.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mVmlSalesInvoice.mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = new RES_SALE_INVOICE();
                mVmlSalesInvoice.mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE.SD = Utility.getTLFormLoadSD();
                mVmlSalesInvoice.mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE.ED = Utility.getTLFormLoadED();
                mVmlSalesInvoice.mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE_DETAIL.Add(new RES_SALE_INVOICE_DETAIL());
                mVmlSalesInvoice.mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_BROWSE.Add(new RES_SALE_BROWSE());
                mVmlSalesInvoice.getInvoice();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        #endregion

        #region "Private Mehtod"

        #endregion

        #region "Event"
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (mVmlSalesInvoice != null)
            {
                // Example logic: 1 column for narrow devices, 2 for medium, 3+ for wider
                int newColumns = width switch
                {
                    < 400 => 1,
                    < 600 => 2,
                    < 800 => 3,
                    < 1000 => 5,
                    < 1200 => 6,
                    < 1400 => 7,
                    < 1600 => 8,
                    < 1800 => 9,
                    _ => 10
                };

                if ((collectionView.ItemsLayout as GridItemsLayout)?.Span != newColumns)
                {
                    collectionView.ItemsLayout = new GridItemsLayout(newColumns, ItemsLayoutOrientation.Vertical);
                }
            }
        }
        private async void TgrNew_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new FrmPosSaleInvoiceSet());
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void entSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (entSearch.Text != null && entSearch.Text != "")
                {
                    mVmlSalesInvoice.searchData(e.NewTextValue);
                }
                else
                {
                    mVmlSalesInvoice.searchData("");
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        private async void lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (Utility.checkButtonAccess("Edit"))
                {
                    var selectedItem = e.SelectedItem as RES_SALE_INVOICE;
                    if (selectedItem != null)
                    {
                        await Navigation.PushAsync(new FrmPosSaleInvoiceSet(selectedItem));
                    }
                    ((ListView)sender).SelectedItem = null;
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private async void cardView_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.CurrentSelection.Count == 0)
                    return;
                if (Utility.checkButtonAccess("Edit"))
                {
                    var selectedItem = e.CurrentSelection.FirstOrDefault() as RES_SALE_INVOICE;
                    if (selectedItem != null)
                    {
                        await Navigation.PushAsync(new FrmPosSaleInvoiceSet(selectedItem));
                    }
                    ((CollectionView)sender).SelectedItem = null;
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private async void grdView_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0)
                return;
            if (Utility.checkButtonAccess("Edit"))
            {
                var selectedItem = e.CurrentSelection.FirstOrDefault() as RES_SALE_INVOICE;
                if (selectedItem == null) return;
                await Navigation.PushAsync(new FrmPosSaleInvoiceSet(selectedItem));
                ((DataGrid)sender).SelectedItem = null;
            }
            else
            {
                WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
            }
        }

        private async void TgrCardView_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (!Common.bindMenu("access-set"))
                {
                    Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "1", Text = "FrmAdmin", MenuUrl = "FrmAdmin", logoImg = "" };
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("Msg401"));
                }
                await Navigation.PushAsync(new FrmPosSaleInvoiceSet(new RES_SALE_INVOICE()));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void TgrRefresh_Tapped(object sender, EventArgs e)
        {
            try
            {
                entSearch.Text = "";
                mVmlSalesInvoice.getInvoice();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
      
        private void OnFabClicked(object sender, EventArgs e)
        {
            // Handle FAB click event
            Debug.WriteLine("FAB Clicked", "You clicked the floating action button!", "OK");
        }

        private void OnEntryCompleted(object sender, EventArgs e)
        {
            try
            {
                string text = ((Entry)sender).Text;
                if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                {
                   
                    if (text != null && text != "")
                    {
                        mVmlSalesInvoice.searchData(text);
                    }
                    else
                    {
                        mVmlSalesInvoice.searchData("");
                    }
                }
                else
                {
                    mVmlSalesInvoice.searchDataApi(text);
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion
    }
}