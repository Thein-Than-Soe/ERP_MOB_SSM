using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.REQ;
using CS.ERP.PL.JOB.RES;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.JOB;
using CS.ERP_MOB.Views.JOB;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.JOB
{
    public class VmlJobCommunity : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_COMMUNITY mJSN_REQ_COMMUNITY = new JSN_REQ_COMMUNITY();
        public JSN_RES_COMMUNITY mJSN_RES_COMMUNITY = new JSN_RES_COMMUNITY();
        public JSN_RES_LOAD_COMMUNITY mJSN_RES_LOAD_COMMUNITY = new JSN_RES_LOAD_COMMUNITY();
        string mRequest = "";
        string mResponse = "";

        private bool mIsAscending;
        public bool IsAscending
        {
            get
            {
                return mIsAscending;
            }
            set
            {
                mIsAscending = value;
                NotifyPropertyChanged("IsAscending");
            }
        }
        private bool mIsDescending;
        public bool IsDescending
        {
            get
            {
                return mIsDescending;
            }
            set
            {
                mIsDescending = value;
                NotifyPropertyChanged("IsDescending");
            }
        }
        public ICommand LoadMoreCommand { get; }
        private bool isLoadingMore = false;
        public bool IsLoadingMore
        {
            get => isLoadingMore;
            set
            {
                isLoadingMore = value;
                NotifyPropertyChanged(nameof(IsLoadingMore));
            }
        }
        public ObservableCollection<SortingItem> sortingList { get; set; }
        SortingItem[] labelTexts = [
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobCommunity.lbl.CommunityName"), value = "CommunityName_0_255", ShowIcon = true },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobCommunity.lbl.CommunityType"), value = "CommunityTypeName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobCommunity.lbl.OpenedBy"), value = "OpenedbyName_0_255", ShowIcon = false },
            ];

        #endregion

        #region "Contructor"
        public VmlJobCommunity()
        {
            this.switchDisplayView(DisplayView.Card);
            CommunityLoad = new JSN_RES_LOAD_COMMUNITY();
            CommunityList = new List<DAT_COMMUNITY>();
            LoadMoreCommand = new Command(async () => await LoadMoreItems());
            sortingList = new ObservableCollection<SortingItem>(labelTexts);
            IsAscending = true;
            IsDescending = false;
        }
        #endregion

        #region "Display View"
        private bool mIsCardView;
        public bool IsCardView
        {
            get
            {
                return mIsCardView;
            }
            set
            {
                mIsCardView = value;
                NotifyPropertyChanged("IsCardView");
            }
        }

        private bool mIsListView;
        public bool IsListView
        {
            get
            {
                return mIsListView;
            }
            set
            {
                mIsListView = value;
                NotifyPropertyChanged("IsListView");
            }
        }

        private bool mIsGridView;
        public bool IsGridView
        {
            get
            {
                return mIsGridView;
            }
            set
            {
                mIsGridView = value;
                NotifyPropertyChanged("IsGridView");
            }
        }

        private bool mIsRefreshing;
        public bool IsRefreshing
        {
            get
            {
                return mIsRefreshing;
            }
            set
            {
                mIsRefreshing = value;
                NotifyPropertyChanged("IsRefreshing");
                if (value) // Only when refreshing starts
                {
                    IsRefreshing = false;

                }
            }
        }
        #endregion

        #region "Data Tab"
        public JSN_RES_LOAD_COMMUNITY JSN_RES_LOAD_COMMUNITY = new JSN_RES_LOAD_COMMUNITY();
        public JSN_RES_LOAD_COMMUNITY CommunityLoad
        {
            get { return JSN_RES_LOAD_COMMUNITY; }
            set { JSN_RES_LOAD_COMMUNITY = value; NotifyPropertyChanged("CommunityLoad"); }
        }

        public DAT_COMMUNITY mDAT_COMMUNITY = new DAT_COMMUNITY();
        public List<DAT_COMMUNITY> mDAT_COMMUNITY_LST = new List<DAT_COMMUNITY>();

        public DAT_COMMUNITY DAT_COMMUNITY
        {
            get { return mDAT_COMMUNITY; }
            set { mDAT_COMMUNITY = value; NotifyPropertyChanged("DAT_COMMUNITY"); }
        }

        public List<DAT_COMMUNITY> mCommunityList;
        public List<DAT_COMMUNITY> CommunityList
        {
            get { return mCommunityList; }
            set { mCommunityList = value; NotifyPropertyChanged("CommunityList"); }
        }


        // Pickers in Searchmore Popup

        public List<DAT_COMMUNITY_TYPE> mJobCommunityTypeList;
        public List<DAT_COMMUNITY_TYPE> JobCommunityTypeList
        {
            get { return mJobCommunityTypeList; }
            set { mJobCommunityTypeList = value; NotifyPropertyChanged("JobCommunityTypeList"); }
        }

        // End of Pickers in Searchmore Popup

        #endregion

        #region "Commands"
        private ICommand mCardViewCommand;
        public ICommand CardViewCommand
        {
            get
            {
                if (mCardViewCommand == null)
                {
                    mCardViewCommand = new Command(() => this.switchDisplayView(DisplayView.Card));
                }
                return mCardViewCommand;
            }
        }

        private ICommand mListViewCommand;
        public ICommand ListViewCommand
        {
            get
            {
                if (mListViewCommand == null)
                {
                    mListViewCommand = new Command(() => this.switchDisplayView(DisplayView.List));
                }
                return mListViewCommand;
            }
        }

        private ICommand mGridViewCommand;
        public ICommand GridViewCommand
        {
            get
            {
                if (mGridViewCommand == null)
                {
                    mGridViewCommand = new Command(() => this.switchDisplayView(DisplayView.Grid));
                }
                return mGridViewCommand;
            }
        }

        private ICommand mRefreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (mRefreshCommand == null)
                {
                    mRefreshCommand = new Command(() => {
                        //if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                        //{

                        //}
                        //else
                        //{
                        //    this.getCommunity();
                        //}
                        mJSN_REQ_COMMUNITY.DAT_COMMUNITY = new List<DAT_COMMUNITY> { new DAT_COMMUNITY() };
                        mJSN_REQ_COMMUNITY.DAT_COMMUNITY[0].Sequence = "0";
                        this.getCommunity();
                    });
                }
                return mRefreshCommand;
            }
        }
        private ICommand mEditItemCommand;
        public ICommand EditItemCommand
        {
            get
            {
                if (mEditItemCommand == null)
                {
                    mEditItemCommand = new Command(() => this.switchDisplayView(DisplayView.Grid));
                    //mRefreshCommand = new Command(() => this.getCommunity());
                }
                return mEditItemCommand;
            }
        }
        private ICommand mDeleteItemCommand;
        public ICommand DeleteItemCommand
        {
            get
            {
                if (mDeleteItemCommand == null)
                {
                    //mRefreshCommand = new Command(() => this.getCommunity());
                }
                return mDeleteItemCommand;
            }
        }
        private ICommand mSelectItemCommand;
        public ICommand SelectItemCommand
        {
            get
            {
                if (mSelectItemCommand == null)
                {
                    //mRefreshCommand = new Command(() => this.getCommunity());
                }
                return mSelectItemCommand;
            }
        }
        private ICommand mSendItemCommand;
        public ICommand SendItemCommand
        {
            get
            {
                if (mSendItemCommand == null)
                {
                    mSendItemCommand = new Command<DAT_COMMUNITY>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Send"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.CommunityName_0_255}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.confirm.Send")}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.btnName.No")}");

                            if (answer)
                            {
                            }
                        }
                    });
                }
                return mSendItemCommand;
            }
        }
public ICommand LongPressItemCommand { get; }

        private ICommand mCardItemTappedCommand;

        public ICommand CardItemTappedCommand
        {
            get
            {
                if (mCardItemTappedCommand == null)
                {
                    mCardItemTappedCommand = new Command(() =>
                    {

                    });
                }
                return mCardItemTappedCommand;
            }
        }
        private ICommand mMoreSearchCommand;
        public ICommand MoreSearchCommand
        {
            get
            {
                if (mMoreSearchCommand == null)
                {
                    mMoreSearchCommand = new Command(() => this.selectMoreSearch());
                }
                return mMoreSearchCommand;
            }
        }
        #endregion

        #region "Method"

        private void switchDisplayView(DisplayView argDisplayView)
        {
            try
            {
                IsCardView = argDisplayView == DisplayView.Card;
                IsListView = argDisplayView == DisplayView.List;
                IsGridView = argDisplayView == DisplayView.Grid;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void bindDataTab(List<DAT_COMMUNITY> argDAT_COMMUNITY_LST)
        {
            try
            {
                if (argDAT_COMMUNITY_LST != null && argDAT_COMMUNITY_LST.Count > 0)
                {
                    DAT_COMMUNITY = argDAT_COMMUNITY_LST[0];
                    CommunityList = argDAT_COMMUNITY_LST;
                }
                else
                {
                    CommunityList = new List<DAT_COMMUNITY>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public void searchDataApi(string argKeyword)
        {
            try
            {
                mJSN_REQ_COMMUNITY.DAT_COMMUNITY = new List<DAT_COMMUNITY>
                {
                    new DAT_COMMUNITY
                    {
                        Remark = argKeyword
                    }
                };
                getCommunity();
            }
            catch (Exception ex)
            {

                throw ex.InnerException;
            }
        }
        public void searchData(string argKeyword)
        {
            try
            {
                List<DAT_COMMUNITY> l_DAT_COMMUNITY_Lst = new List<DAT_COMMUNITY>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (DAT_COMMUNITY l_DAT_COMMUNITY in mJSN_RES_COMMUNITY.DAT_COMMUNITY)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_DAT_COMMUNITY.CommunityTypeName_0_255.ToLower().Contains(argKeyword)
                            || l_DAT_COMMUNITY.CommunityName_0_255.ToLower().Contains(argKeyword)
                            || l_DAT_COMMUNITY.ResolvedbyName_0_255.ToLower().Contains(argKeyword))
                        {
                            l_DAT_COMMUNITY_Lst.Add(l_DAT_COMMUNITY);
                        }
                    }
                }
                else
                {
                    l_DAT_COMMUNITY_Lst = mJSN_RES_COMMUNITY.DAT_COMMUNITY;
                }
                bindDataTab(l_DAT_COMMUNITY_Lst);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void formatUserSettingData(List<DAT_COMMUNITY> argDAT_COMMUNITY_LST)
        {
            try
            {
                if (argDAT_COMMUNITY_LST != null && argDAT_COMMUNITY_LST.Count > 0)
                {
                    foreach (DAT_COMMUNITY l_DAT_COMMUNITY in argDAT_COMMUNITY_LST)
                    {
                        l_DAT_COMMUNITY.StartDate = Utility.getDateTimeString(l_DAT_COMMUNITY.StartDate).ToString();
                        if (l_DAT_COMMUNITY.StatusAsk == "1")
                        {
                            l_DAT_COMMUNITY.StatusName_0_255 = "Inactive";
                        }
                        else
                        {
                            l_DAT_COMMUNITY.StatusName_0_255 = "Active";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private void selectMoreSearch()
        {
            try
            {
                loadCommunity();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private async void callSearchMorePopup()
        {
            try
            {
                var popup = new FrmJobCommunityPop(this.CommunityLoad);
                await PopupNavigation.Instance.PushAsync(popup);

                var result = await popup.PopupClosedTask;
                if (result is DAT_COMMUNITY selectedData)
                {
                    mJSN_REQ_COMMUNITY.DAT_COMMUNITY = new List<DAT_COMMUNITY> { selectedData };
                    if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                    {
                        CommunityList = mDAT_COMMUNITY_LST.Where(data => (selectedData.CommunityTypeAsk == "0" || data.CommunityTypeAsk == selectedData.CommunityTypeAsk)
                                                                        && (string.IsNullOrEmpty(selectedData.CommunityName_0_255)
                                                                        || data.CommunityName_0_255?.ToLower().Contains(selectedData.CommunityName_0_255.ToLower()) == true)
                                                                           ).ToList();
                    }
                    else
                    {
                        getCommunity();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        // Binding data for pickers in Searchmore Popup
        public void bindCommunityType(List<DAT_COMMUNITY_TYPE> argDAT_COMMUNITY_LST)
        {
            try
            {
                if (argDAT_COMMUNITY_LST != null && argDAT_COMMUNITY_LST.Count > 0)
                {
                    JobCommunityTypeList = argDAT_COMMUNITY_LST;
                }
                else
                {
                    JobCommunityTypeList = new List<DAT_COMMUNITY_TYPE>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion

        #region "Web Service Api"
        public async void getCommunity()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_COMMUNITY);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wsgetCommunity);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_COMMUNITY = JsonConvert.DeserializeObject<JSN_RES_COMMUNITY>(mResponse);
                    if (this.mJSN_RES_COMMUNITY.Message.Code == "7")
                    {
                        if (this.mJSN_RES_COMMUNITY.DAT_COMMUNITY.Count > 0)
                        {
                            formatUserSettingData(this.mJSN_RES_COMMUNITY.DAT_COMMUNITY);
                            mDAT_COMMUNITY_LST = this.mJSN_RES_COMMUNITY.DAT_COMMUNITY;
                            bindDataTab(this.mJSN_RES_COMMUNITY.DAT_COMMUNITY);
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_COMMUNITY.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_COMMUNITY.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_COMMUNITY.Message.Message);
                    }

                    Utility.closeLoader();
                }
                else
                {
                    Utility.closeLoader();
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("ErrWebService"));
                }
            }
            catch (Exception ex)
            {
                Utility.closeLoader();
                throw ex.InnerException;
            }
        }

        public async void loadCommunity()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wsLoadCommunity);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_LOAD_COMMUNITY = JsonConvert.DeserializeObject<JSN_RES_LOAD_COMMUNITY>(mResponse);
                    if (mJSN_RES_LOAD_COMMUNITY.Message.Code == "7")
                    {
                        Utility.closeLoader();
                        this.CommunityLoad = mJSN_RES_LOAD_COMMUNITY;
                        callSearchMorePopup();
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_LOAD_COMMUNITY.Message.Message);
                    }
                }
                else
                {
                    Utility.closeLoader();
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("ErrWebService"));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private async Task LoadMoreItems()
        {
            if (IsLoadingMore) return;
            IsLoadingMore = true;

            getCommunity(); // Your data fetch


            IsLoadingMore = false;
        }
        #endregion
    }
}
