using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.CHT;
using CS.ERP_MOB.ViewsModel.JOB;
using Maui.DataGrid;
using Newtonsoft.Json.Linq;
using Stripe;
using System.Diagnostics;
using System.Collections.ObjectModel;
//using System.Windows.Forms;

namespace CS.ERP_MOB.Views.JOB
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmJobAdviceLst : ContentView
    {
        #region "Declaring"
        VmlJobAdvice mVmlJobAdvice;
        #endregion
        #region "Constructor"
        public FrmJobAdviceLst()
        {
            try
            {
                InitializeComponent();

                BindingContext = mVmlJobAdvice = new VmlJobAdvice();
                mVmlJobAdvice.mJSN_REQ_ADVICE.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mVmlJobAdvice.mJSN_REQ_ADVICE.DAT_ADVICE = new List<DAT_ADVICE> { new DAT_ADVICE() };
                mVmlJobAdvice.getAdvice();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        #endregion

        #region "Private Method"


        #endregion

        #region "Event"


        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (mVmlJobAdvice != null)
            {
                int newColumns = width switch
                {
                    < 400 => 2,
                    < 600 => 2,
                    < 800 => 4,
                    < 1000 => 5,
                    < 1200 => 6,
                    < 1400 => 7,
                    < 1600 => 8,
                    < 1800 => 9,
                    _ => 10
                };

                if (collectionView.ItemsLayout is not GridItemsLayout currentLayout ||
                    currentLayout.Span != newColumns)
                {
                    var layout = new GridItemsLayout(newColumns, ItemsLayoutOrientation.Vertical)
                    {
                        VerticalItemSpacing = 10,
                        HorizontalItemSpacing = 10
                    };
                    collectionView.ItemsLayout = layout;
                }
            }
        }

        private void sortAdviceList(string sortBy)
        {
            if (mVmlJobAdvice.AdviceList == null || !mVmlJobAdvice.AdviceList.Any())
                return;

            IEnumerable<DAT_ADVICE> sorted;

            switch (sortBy)
            {
                case "AdviceTypeName_0_255":
                    sorted = mVmlJobAdvice.IsAscending
                        ? mVmlJobAdvice.AdviceList.OrderBy(x => x.AdviceTypeName_0_255)
                        : mVmlJobAdvice.AdviceList.OrderByDescending(x => x.AdviceTypeName_0_255);
                    break;

                case "AdviceName_0_255":
                    sorted = mVmlJobAdvice.IsAscending
                        ? mVmlJobAdvice.AdviceList.OrderBy(x => x.AdviceName_0_255)
                        : mVmlJobAdvice.AdviceList.OrderByDescending(x => x.AdviceName_0_255);
                    break;

                case "OpenedbyName_0_255":
                    sorted = mVmlJobAdvice.IsAscending
                        ? mVmlJobAdvice.AdviceList.OrderBy(x => x.OpenedbyName_0_255)
                        : mVmlJobAdvice.AdviceList.OrderByDescending(x => x.OpenedbyName_0_255);
                    break;


                default:
                    return;
            }

            mVmlJobAdvice.AdviceList = new List<DAT_ADVICE>(sorted);
            if (mVmlJobAdvice.IsCardView)
            {
                collectionView.ItemsSource = mVmlJobAdvice.AdviceList;
            }
            else if (mVmlJobAdvice.IsListView)
            {
                lstView.ItemsSource = mVmlJobAdvice.AdviceList;
            }
            else
            {
                MyGrid.ItemsSource = mVmlJobAdvice.AdviceList;
            }
        }
        private async void TgrNew_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new FrmJobCompanyDtl());
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
                    mVmlJobAdvice.searchData(e.NewTextValue);
                }
                else
                {
                    mVmlJobAdvice.searchData("");
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
                    var selectedItem = e.SelectedItem as DAT_ADVICE;
                    if (selectedItem != null)
                    {
                        await Navigation.PushAsync(new FrmJobAdviceDtl(selectedItem));
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
                    var selectedItem = e.CurrentSelection.FirstOrDefault() as DAT_ADVICE;
                    if (selectedItem != null)
                    {
                        await Navigation.PushAsync(new FrmJobAdviceDtl(selectedItem));
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
                var selectedItem = e.CurrentSelection.FirstOrDefault() as DAT_ADVICE;
                if (selectedItem == null) return;
                await Navigation.PushAsync(new FrmJobAdviceDtl(selectedItem));
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
                //await Navigation.PushAsync(new FrmJobAdviceSet(new DAT_ADVICE()));

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
                mVmlJobAdvice.getAdvice();
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
                        mVmlJobAdvice.searchData(text);
                    }
                    else
                    {
                        mVmlJobAdvice.searchData("");
                    }
                }
                else
                {
                    mVmlJobAdvice.searchDataApi(text);
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void OnMenuTapped(object sender, TappedEventArgs e)
        {
            Overlay.IsVisible = true;
            MenuBox.IsVisible = true;
        }
        private void OnOverlayTapped(object sender, EventArgs e)
        {
            MenuBox.IsVisible = false;
            Overlay.IsVisible = false;
        }

        private void Sorting_Tapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is SortingItem tappedItem)
            {
                // Hide all icons
                foreach (var item in mVmlJobAdvice.sortingList)
                    item.ShowIcon = false;

                // Show only tapped item’s icon
                tappedItem.ShowIcon = true;
                sortAdviceList(tappedItem.value);
            }
        }
        private void Ascending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlJobAdvice.IsDescending = false;
            mVmlJobAdvice.IsAscending = true;
        }
        private void Descending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlJobAdvice.IsDescending = true;
            mVmlJobAdvice.IsAscending = false;
        }

        #endregion
    }
}