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
    public partial class FrmJobCommunityLst : ContentView
    {
        #region "Declaring"
        VmlJobCommunity mVmlJobCommunity;
        #endregion
        #region "Constructor"
        public FrmJobCommunityLst()
        {
            try
            {
                InitializeComponent();

                BindingContext = mVmlJobCommunity = new VmlJobCommunity();
                mVmlJobCommunity.mJSN_REQ_COMMUNITY.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mVmlJobCommunity.mJSN_REQ_COMMUNITY.DAT_COMMUNITY = new List<DAT_COMMUNITY> { new DAT_COMMUNITY() };
                mVmlJobCommunity.getCommunity();
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

            if (mVmlJobCommunity != null)
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

        private void sortCommunityList(string sortBy)
        {
            if (mVmlJobCommunity.CommunityList == null || !mVmlJobCommunity.CommunityList.Any())
                return;

            IEnumerable<DAT_COMMUNITY> sorted;

            switch (sortBy)
            {
                case "CommunityName_0_255":
                    sorted = mVmlJobCommunity.IsAscending
                        ? mVmlJobCommunity.CommunityList.OrderBy(x => x.CommunityName_0_255)
                        : mVmlJobCommunity.CommunityList.OrderByDescending(x => x.CommunityName_0_255);
                    break;

                case "CommunityTypeName_0_255":
                    sorted = mVmlJobCommunity.IsAscending
                        ? mVmlJobCommunity.CommunityList.OrderBy(x => x.CommunityTypeName_0_255)
                        : mVmlJobCommunity.CommunityList.OrderByDescending(x => x.CommunityTypeName_0_255);
                    break;

                case "OpenedbyName_0_255":
                    sorted = mVmlJobCommunity.IsAscending
                        ? mVmlJobCommunity.CommunityList.OrderBy(x => x.OpenedbyName_0_255)
                        : mVmlJobCommunity.CommunityList.OrderByDescending(x => x.OpenedbyName_0_255);
                    break;

                default:
                    return;
            }

            mVmlJobCommunity.CommunityList = new List<DAT_COMMUNITY>(sorted);
            if (mVmlJobCommunity.IsCardView)
            {
                collectionView.ItemsSource = mVmlJobCommunity.CommunityList;
            }
            else if (mVmlJobCommunity.IsListView)
            {
                lstView.ItemsSource = mVmlJobCommunity.CommunityList;
            }
            else
            {
                MyGrid.ItemsSource = mVmlJobCommunity.CommunityList;
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
                    mVmlJobCommunity.searchData(e.NewTextValue);
                }
                else
                {
                    mVmlJobCommunity.searchData("");
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
                    var selectedItem = e.SelectedItem as DAT_COMMUNITY;
                    if (selectedItem != null)
                    {
                        //await Navigation.PushAsync(new FrmJobAdviceSet(selectedItem));
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
                    var selectedItem = e.CurrentSelection.FirstOrDefault() as DAT_COMMUNITY;
                    if (selectedItem != null)
                    {
                        //await Navigation.PushAsync(new FrmJobAdviceSet(selectedItem));
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
                var selectedItem = e.CurrentSelection.FirstOrDefault() as DAT_COMMUNITY;
                if (selectedItem == null) return;
                //await Navigation.PushAsync(new FrmJobAdviceSet(selectedItem));
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
                //await Navigation.PushAsync(new FrmJobAdviceSet(new DAT_COMMUNITY()));

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
                mVmlJobCommunity.getCommunity();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

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
                        mVmlJobCommunity.searchData(text);
                    }
                    else
                    {
                        mVmlJobCommunity.searchData("");
                    }
                }
                else
                {
                    mVmlJobCommunity.searchDataApi(text);
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
                foreach (var item in mVmlJobCommunity.sortingList)
                    item.ShowIcon = false;

                // Show only tapped item’s icon
                tappedItem.ShowIcon = true;
                sortCommunityList(tappedItem.value);
            }
        }
        private void Ascending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlJobCommunity.IsDescending = false;
            mVmlJobCommunity.IsAscending = true;
        }
        private void Descending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlJobCommunity.IsDescending = true;
            mVmlJobCommunity.IsAscending = false;
        }

        #endregion
    }
}