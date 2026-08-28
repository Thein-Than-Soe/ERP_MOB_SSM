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
    public partial class FrmJobCompanyLst : ContentView
    {
        private bool _ignoreItemSelected = false;

        #region "Declaring"
        VmlJobCompany mVmlJobCompany;
        #endregion
        #region "Constructor"
        public FrmJobCompanyLst()
        {
            try
            {
                InitializeComponent();

                BindingContext = mVmlJobCompany = new VmlJobCompany();
                mVmlJobCompany.mJSN_REQ_COMPANY.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mVmlJobCompany.mJSN_REQ_COMPANY.RES_COMPANY = new List<RES_COMPANY> { new RES_COMPANY() };
                //mVmlJobCompany.mJSN_REQ_COMPANY.RES_COMPANY[0].SD = Utility.getTLFormLoadSD();
                //mVmlJobCompany.mJSN_REQ_COMPANY.RES_COMPANY[0].ED = Utility.getTLFormLoadED();
                //mVmlJobCompany.mJSN_REQ_COMPANY.RES_COMPANY_DETAIL.Add(new RES_COMPANY_DETAIL());
                //mVmlJobCompany.mJSN_REQ_COMPANY.RES_SALE_BROWSE.Add(new RES_SALE_BROWSE());
                mVmlJobCompany.getJobCompany();
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

            if (mVmlJobCompany != null)
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

        private void sortJobCompanyList(string sortBy)
        {
            if (mVmlJobCompany.JobCompanyList == null || !mVmlJobCompany.JobCompanyList.Any())
                return;

            IEnumerable<RES_COMPANY> sorted;

            switch (sortBy)
            {
                case "CompanyName_0_255":
                    sorted = mVmlJobCompany.IsAscending
                        ? mVmlJobCompany.JobCompanyList.OrderBy(x => x.CompanyName_0_255)
                        : mVmlJobCompany.JobCompanyList.OrderByDescending(x => x.CompanyName_0_255);
                    break;

                case "CompanyRegSDate":
                    sorted = mVmlJobCompany.IsAscending
                        ? mVmlJobCompany.JobCompanyList.OrderBy(x => x.CompanyRegSDate)
                        : mVmlJobCompany.JobCompanyList.OrderByDescending(x => x.CompanyRegSDate);
                    break;

                case "CompanyTypeName_0_255":
                    sorted = mVmlJobCompany.IsAscending
                        ? mVmlJobCompany.JobCompanyList.OrderBy(x => x.CompanyTypeName_0_255)
                        : mVmlJobCompany.JobCompanyList.OrderByDescending(x => x.CompanyTypeName_0_255);
                    break;

                case "CityName_0_255":
                    sorted = mVmlJobCompany.IsAscending
                        ? mVmlJobCompany.JobCompanyList.OrderBy(x => x.CityName_0_255)
                        : mVmlJobCompany.JobCompanyList.OrderByDescending(x => x.CityName_0_255);
                    break;

                case "CountryName_0_255":
                    sorted = mVmlJobCompany.IsAscending
                        ? mVmlJobCompany.JobCompanyList.OrderBy(x => x.CountryName_0_255)
                        : mVmlJobCompany.JobCompanyList.OrderByDescending(x => x.CountryName_0_255);
                    break;

                default:
                    return;
            }

            mVmlJobCompany.JobCompanyList = new ObservableCollection<RES_COMPANY>(sorted);
            if (mVmlJobCompany.IsCardView)
            {
                collectionView.ItemsSource = mVmlJobCompany.JobCompanyList;
            }
            else if (mVmlJobCompany.IsListView)
            {
                lstView.ItemsSource = mVmlJobCompany.JobCompanyList;
            }
            else
            {
                MyGrid.ItemsSource = mVmlJobCompany.JobCompanyList;
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
                    mVmlJobCompany.searchData(e.NewTextValue);
                }
                else
                {
                    mVmlJobCompany.searchData("");
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
                if (_ignoreItemSelected)
                {
                    _ignoreItemSelected = false; // Reset the flag
                    ((ListView)sender).SelectedItem = null; // Deselect the item
                    return; // Skip the rest of the logic
                }

                //if (!Utility.checkButtonAccess("Edit"))
                //{
                var selectedItem = e.SelectedItem as RES_COMPANY;
                if (selectedItem != null)
                {
                    //await Navigation.PushAsync(new FrmJobCompanyDtl(selectedItem));
                    await Navigation.PushAsync(new FrmJobCompanyDtl(selectedItem, mVmlJobCompany));
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
        private async void cardView_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_ignoreItemSelected)
                {
                    _ignoreItemSelected = false; // Reset the flag
                    ((CollectionView)sender).SelectedItem = null; // Deselect the item
                    return; // Skip the rest of the logic
                }
                if (e.CurrentSelection.Count == 0)
                    return;
                if (Utility.checkButtonAccess("Edit"))
                {
                    var selectedItem = e.CurrentSelection.FirstOrDefault() as RES_COMPANY;
                    
                    if (selectedItem != null)
                    {
                        //await Navigation.PushAsync(new FrmJobCompanyDtl(selectedItem));
                        await Navigation.PushAsync(new FrmJobCompanyDtl(selectedItem, mVmlJobCompany));
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
                var selectedItem = e.CurrentSelection.FirstOrDefault() as RES_COMPANY;
                if (selectedItem == null) return;
                //await Navigation.PushAsync(new FrmJobCompanyDtl(selectedItem));
                await Navigation.PushAsync(new FrmJobCompanyDtl(selectedItem, mVmlJobCompany));
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
                await Navigation.PushAsync(new FrmJobCompanyDtl(new RES_COMPANY()));

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
                mVmlJobCompany.getJobCompany();
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
                        mVmlJobCompany.searchData(text);
                    }
                    else
                    {
                        mVmlJobCompany.searchData("");
                    }
                }
                else
                {
                    mVmlJobCompany.searchDataApi(text);
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
                foreach (var item in mVmlJobCompany.sortingList)
                    item.ShowIcon = false;

                // Show only tapped item’s icon
                tappedItem.ShowIcon = true;
                sortJobCompanyList(tappedItem.value);
            }
        }
        private void Ascending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlJobCompany.IsDescending = false;
            mVmlJobCompany.IsAscending = true;
        }
        private void Descending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlJobCompany.IsDescending = true;
            mVmlJobCompany.IsAscending = false;
        }
        #endregion
    }
}