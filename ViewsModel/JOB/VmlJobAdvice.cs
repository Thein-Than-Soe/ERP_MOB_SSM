using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.HCM.REQ;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.REQ;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.JOB;
using CS.ERP_MOB.Services.SYS;
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
    public class VmlJobAdvice : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_ADVICE mJSN_REQ_ADVICE = new JSN_REQ_ADVICE();
        public JSN_RES_ADVICE mJSN_RES_ADVICE = new JSN_RES_ADVICE();
        public JSN_RES_LOAD_ADVICE mJSN_RES_LOAD_ADVICE = new JSN_RES_LOAD_ADVICE();
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
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobAdvice.lbl.AdviceType"), value = "AdviceTypeName_0_255", ShowIcon = true },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobAdvice.lbl.AdviceName"), value = "AdviceName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobAdvice.lbl.OpenedBy"), value = "OpenedbyName_0_255", ShowIcon = false },
            ];
        #endregion

        #region "Contructor"
        public VmlJobAdvice()
        {
            this.switchDisplayView(DisplayView.Card);
            AdviceLoad = new JSN_RES_LOAD_ADVICE();
            AdviceList = new List<DAT_ADVICE>();
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
        public JSN_RES_LOAD_ADVICE JSN_RES_LOAD_ADVICE = new JSN_RES_LOAD_ADVICE();
        public JSN_RES_LOAD_ADVICE AdviceLoad
        {
            get { return JSN_RES_LOAD_ADVICE; }
            set { JSN_RES_LOAD_ADVICE = value; NotifyPropertyChanged("AdviceLoad"); }
        }

        public DAT_ADVICE mDAT_ADVICE = new DAT_ADVICE();
        public List<DAT_ADVICE> mDAT_ADVICE_LST = new List<DAT_ADVICE>();


        public DAT_ADVICE DAT_ADVICE
        {
            get { return mDAT_ADVICE; }
            set { mDAT_ADVICE = value; NotifyPropertyChanged("DAT_ADVICE"); }
        }

        public List<DAT_ADVICE> mAdviceList;
        public List<DAT_ADVICE> AdviceList
        {
            get { return mAdviceList; }
            set { mAdviceList = value; NotifyPropertyChanged("AdviceList"); }
        }
        // Pickers in Searchmore Popup
        public List<DAT_ADVICE_TYPE> mJobAdviceTypeList;
        public List<DAT_ADVICE_TYPE> JobAdviceTypeList
        {
            get { return mJobAdviceTypeList; }
            set { mJobAdviceTypeList = value; NotifyPropertyChanged("JobAdviceTypeList"); }
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
                        //    this.getAdvice();
                        //}
                        mJSN_REQ_ADVICE.DAT_ADVICE = new List<DAT_ADVICE> { new DAT_ADVICE() };
                        mJSN_REQ_ADVICE.DAT_ADVICE[0].Sequence = "0";
                        this.getAdvice();
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
                    //mRefreshCommand = new Command(() => this.getAdvice());
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
                    //mRefreshCommand = new Command(() => this.getAdvice());
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
                    //mRefreshCommand = new Command(() => this.getAdvice());
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
                    mSendItemCommand = new Command<DAT_ADVICE>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Send"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.AdviceName_0_255}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Send")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

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
        private void bindDataTab(List<DAT_ADVICE> argDAT_ADVICE_LST)
        {
            try
            {
                if (argDAT_ADVICE_LST != null && argDAT_ADVICE_LST.Count > 0)
                {
                    DAT_ADVICE = argDAT_ADVICE_LST[0];
                    AdviceList = argDAT_ADVICE_LST;
                }
                else
                {
                    AdviceList = new List<DAT_ADVICE>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private void combineImageUrl(List<DAT_ADVICE> argDAT_ADVICE_LST)
        {
            try
            {
                if (argDAT_ADVICE_LST != null && argDAT_ADVICE_LST.Count > 0)
                {
                    foreach (DAT_ADVICE l_RES_COMPANY in argDAT_ADVICE_LST)
                    {
                        l_RES_COMPANY.ReferenceDoc = Sys_Service.getUploadURL() + l_RES_COMPANY.ReferenceDoc;
                    }
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
                mJSN_REQ_ADVICE.DAT_ADVICE = new List<DAT_ADVICE>
                {
                    new DAT_ADVICE
                    {
                        Remark = argKeyword
                    }
                };
                getAdvice();
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
                List<DAT_ADVICE> l_DAT_ADVICE_Lst = new List<DAT_ADVICE>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (DAT_ADVICE l_DAT_ADVICE in mJSN_RES_ADVICE.DAT_ADVICE)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_DAT_ADVICE.AdviceName_0_255.ToLower().Contains(argKeyword)
                            || l_DAT_ADVICE.AdviceTypeName_0_255.ToLower().Contains(argKeyword)
                            || l_DAT_ADVICE.ResolvedbyName_0_255.ToLower().Contains(argKeyword))
                        {
                            l_DAT_ADVICE_Lst.Add(l_DAT_ADVICE);
                        }
                    }
                }
                else
                {
                    l_DAT_ADVICE_Lst = mJSN_RES_ADVICE.DAT_ADVICE;// OriginalInvoiceClosedList.GetRange(0, OriginalInvoiceClosedList.Count);
                }
                bindDataTab(l_DAT_ADVICE_Lst);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void formatUserSettingData(List<DAT_ADVICE> argDAT_ADVICE_LST)
        {
            try
            {
                if (argDAT_ADVICE_LST != null && argDAT_ADVICE_LST.Count > 0)
                {
                    foreach (DAT_ADVICE l_DAT_ADVICE in argDAT_ADVICE_LST)
                    {
                        l_DAT_ADVICE.StartDate = Utility.getDateTimeString(l_DAT_ADVICE.StartDate).ToString();
                        if (l_DAT_ADVICE.StatusAsk == "1")
                        {
                            l_DAT_ADVICE.StatusName_0_255 = "Inactive";
                        }
                        else
                        {
                            l_DAT_ADVICE.StatusName_0_255 = "Active";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        //private Task ExecuteActiveItem()
        //{
        //    saveInvoice();
        //    return Task.CompletedTask;
        //}
        private void selectMoreSearch()
        {
            try
            {
                loadAdvice();
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
                var popup = new FrmJobAdvicePop(this.AdviceLoad);
                await PopupNavigation.Instance.PushAsync(popup);

                var result = await popup.PopupClosedTask;
                if (result is DAT_ADVICE selectedData)
                {
                    mJSN_REQ_ADVICE.DAT_ADVICE = new List<DAT_ADVICE> { selectedData };
                    if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                    {
                        AdviceList = mDAT_ADVICE_LST.Where(data => (selectedData.AdviceTypeAsk == "0" || data.AdviceTypeAsk == selectedData.AdviceTypeAsk)
                                                                    && (string.IsNullOrEmpty(selectedData.AdviceName_0_255) 
                                                                    || data.AdviceName_0_255?.ToLower().Contains(selectedData.AdviceName_0_255.ToLower()) == true)
                                                                       ).ToList();
                    }
                    else
                    {
                        getAdvice();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        // Binding data for pickers in Searchmore Popup
        public void bindAdviceType(List<DAT_ADVICE_TYPE> argDAT_ADVICE_LST)
        {
            try
            {
                if (argDAT_ADVICE_LST != null && argDAT_ADVICE_LST.Count > 0)
                {
                    JobAdviceTypeList = argDAT_ADVICE_LST;
                }
                else
                {
                    JobAdviceTypeList = new List<DAT_ADVICE_TYPE>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion

        #region "Web Service Api"
        public async void getAdvice()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_ADVICE);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wsgetAdvice);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_ADVICE = JsonConvert.DeserializeObject<JSN_RES_ADVICE>(mResponse);
                    if (this.mJSN_RES_ADVICE.Message.Code == "7")
                    {
                        if (this.mJSN_RES_ADVICE.DAT_ADVICE.Count > 0)
                        {
                            formatUserSettingData(this.mJSN_RES_ADVICE.DAT_ADVICE);
                            mDAT_ADVICE_LST = this.mJSN_RES_ADVICE.DAT_ADVICE;
                            combineImageUrl(this.mJSN_RES_ADVICE.DAT_ADVICE);
                            bindDataTab(this.mJSN_RES_ADVICE.DAT_ADVICE);
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_ADVICE.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_ADVICE.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_ADVICE.Message.Message);
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
        public async void loadAdvice()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wsLoadAdvice);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_LOAD_ADVICE = JsonConvert.DeserializeObject<JSN_RES_LOAD_ADVICE>(mResponse);
                    if (mJSN_RES_LOAD_ADVICE.Message.Code == "7")
                    {
                        Utility.closeLoader();
                        this.AdviceLoad = mJSN_RES_LOAD_ADVICE;
                        callSearchMorePopup();
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_LOAD_ADVICE.Message.Message);
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

            getAdvice(); // Your data fetch


            IsLoadingMore = false;
        }
        #endregion
    }
}
