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
using System.Collections.ObjectModel;
using System.Diagnostics;
//using System.Windows.Forms;

namespace CS.ERP_MOB.Views.JOB
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmJobSearchLst : ContentView
    {
        #region "Declaring"
        private bool _ignoreItemSelected = false;
        VmlJobSearch mVmlJobSearch;
        #endregion
        #region "Constructor"
        public FrmJobSearchLst()
        {
            try
            {
                InitializeComponent();

                BindingContext = mVmlJobSearch = new VmlJobSearch();
                mVmlJobSearch.mJSN_REQ_JOB_SEARCH.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mVmlJobSearch.mJSN_REQ_JOB_SEARCH.DAT_JOB_SEARCH = new List<DAT_JOB_SEARCH> { new DAT_JOB_SEARCH() };
                //mVmlJobSearch.mJSN_REQ_JOB_SEARCH.DAT_JOB_SEARCH[0].SD = Utility.getTLFormLoadSD();
                //mVmlJobSearch.mJSN_REQ_JOB_SEARCH.DAT_JOB_SEARCH[0].ED = Utility.getTLFormLoadED();
                //mVmlJobSearch.mJSN_REQ_JOB_SEARCH.DAT_JOB_SEARCH_DETAIL.Add(new DAT_JOB_SEARCH_DETAIL());
                //mVmlJobSearch.mJSN_REQ_JOB_SEARCH.RES_SALE_BROWSE.Add(new RES_SALE_BROWSE());
                mVmlJobSearch.getJobVacancy();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        #endregion

        #region "Private Method"
        private async void OnSaveTapped(object sender, EventArgs e)
        {
            // Prevent selection from triggering
            if (collectionView.SelectedItem != null)
                collectionView.SelectedItem = null;
            _ignoreItemSelected = true;
            await Task.Delay(80);
        }

        #endregion

        #region "Event"

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (mVmlJobSearch != null)
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

        private void sortJobVacancyList(string sortBy)
        {
            if (mVmlJobSearch.JobVacancyList == null || !mVmlJobSearch.JobVacancyList.Any())
                return;

            IEnumerable<DAT_JOB_SEARCH> sorted;

            switch (sortBy)
            {
                case "DesignationName_0_255":
                    sorted = mVmlJobSearch.IsAscending
                        ? mVmlJobSearch.JobVacancyList.OrderBy(x => x.DesignationName_0_255)
                        : mVmlJobSearch.JobVacancyList.OrderByDescending(x => x.DesignationName_0_255);
                    break;

                case "CompanyName_0_255":
                    sorted = mVmlJobSearch.IsAscending
                        ? mVmlJobSearch.JobVacancyList.OrderBy(x => x.CompanyName_0_255)
                        : mVmlJobSearch.JobVacancyList.OrderByDescending(x => x.CompanyName_0_255);
                    break;

                case "VacancyAvailabilityName_0_255":
                    sorted = mVmlJobSearch.IsAscending
                        ? mVmlJobSearch.JobVacancyList.OrderBy(x => x.VacancyAvailabilityName_0_255)
                        : mVmlJobSearch.JobVacancyList.OrderByDescending(x => x.VacancyAvailabilityName_0_255);
                    break;

                case "CityName_0_255":
                    sorted = mVmlJobSearch.IsAscending
                        ? mVmlJobSearch.JobVacancyList.OrderBy(x => x.CityName_0_255)
                        : mVmlJobSearch.JobVacancyList.OrderByDescending(x => x.CityName_0_255);
                    break;

                case "SD":
                    sorted = mVmlJobSearch.IsAscending
                        ? mVmlJobSearch.JobVacancyList.OrderBy(x => x.SD)
                        : mVmlJobSearch.JobVacancyList.OrderByDescending(x => x.SD);
                    break;
                case "ED":
                    sorted = mVmlJobSearch.IsAscending
                        ? mVmlJobSearch.JobVacancyList.OrderBy(x => x.ED)
                        : mVmlJobSearch.JobVacancyList.OrderByDescending(x => x.ED);
                    break;

                default:
                    return;
            }

            mVmlJobSearch.JobVacancyList = new ObservableCollection<DAT_JOB_SEARCH>(sorted);
            if (mVmlJobSearch.IsCardView)
            {
                collectionView.ItemsSource = mVmlJobSearch.JobVacancyList;
            }
            else if (mVmlJobSearch.IsListView)
            {
                lstView.ItemsSource = mVmlJobSearch.JobVacancyList;
            }
            else
            {
                MyGrid.ItemsSource = mVmlJobSearch.JobVacancyList;
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
                    mVmlJobSearch.searchData(e.NewTextValue);
                }
                else
                {
                    mVmlJobSearch.searchData("");
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
                    var selectedItem = e.SelectedItem as DAT_JOB_SEARCH;
                    if (_ignoreItemSelected)
                    {
                        _ignoreItemSelected = false; // Reset the flag
                        ((ListView)sender).SelectedItem = null; // Deselect the item
                        return; // Skip the rest of the logic
                    }
                    if (selectedItem != null)
                    {
                        //await Navigation.PushAsync(new FrmJobSearchDtl(selectedItem,async updatedItem => 
                        //                                                                { updatedItem.IsSaved; }));
                        await Navigation.PushAsync(new FrmJobSearchDtl(selectedItem, mVmlJobSearch));
                        //await Navigation.PushAsync(new FrmJobSearchDtl(selectedItem));

                    }
                    else { return; }
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
                    var selectedItem = e.CurrentSelection.FirstOrDefault() as DAT_JOB_SEARCH;
                    if (_ignoreItemSelected)
                    {
                        _ignoreItemSelected = false; // Reset the flag
                        ((CollectionView)sender).SelectedItem = null; // Deselect the item
                        return; // Skip the rest of the logic
                    }
                    if (selectedItem != null)
                    {
                        //await Navigation.PushAsync(new FrmJobSearchDtl(selectedItem));
                        await Navigation.PushAsync(new FrmJobSearchDtl(selectedItem, mVmlJobSearch));
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
                var selectedItem = e.CurrentSelection.FirstOrDefault() as DAT_JOB_SEARCH;
                if (selectedItem == null) return;
                //await Navigation.PushAsync(new FrmJobVacancyDtl(selectedItem));
                await Navigation.PushAsync(new FrmJobSearchDtl(selectedItem, mVmlJobSearch));
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
                //await Navigation.PushAsync(new FrmJobVacancyDtl(new DAT_JOB_SEARCH()));

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
                mVmlJobSearch.getJobVacancy();
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
                        mVmlJobSearch.searchData(text);
                    }
                    else
                    {
                        mVmlJobSearch.searchData("");
                    }
                }
                else
                {
                    mVmlJobSearch.searchDataApi(text);
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
                foreach (var item in mVmlJobSearch.sortingList)
                    item.ShowIcon = false;

                // Show only tapped item’s icon
                tappedItem.ShowIcon = true;
                sortJobVacancyList(tappedItem.value);
            }
        }
        private void Ascending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlJobSearch.IsDescending = false;
            mVmlJobSearch.IsAscending = true;
        }
        private void Descending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlJobSearch.IsDescending = true;
            mVmlJobSearch.IsAscending = false;
        }
        #endregion

    }
}