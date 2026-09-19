using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.POS;
using CS.ERP_MOB.Views.SSM;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SSM
{
    public class VmlSalesInvoice : BaseViewModel
    {
        #region "Declaring"
        string mRequest = "";
        string mResponse = "";

        public JSN_REQ_SALE_INVOICE_JUN mJSN_REQ_SALE_INVOICE_JUN = new JSN_REQ_SALE_INVOICE_JUN();
        public JSN_SALE_INVOICE_JUN mJSN_SALE_INVOICE_JUN = new JSN_SALE_INVOICE_JUN();
        public JSN_LOAD_SALE_INVOICE mJSN_LOAD_SALE_INVOICE = new JSN_LOAD_SALE_INVOICE();
        public List<RES_SALE_INVOICE> mRES_SALE_INVOICE_LST = new List<RES_SALE_INVOICE>();
        public ObservableCollection<RES_SALE_INVOICE> SalesInvoiceList { get; set; }
        public ObservableCollection<SortingItem> sortingList { get; set; }
        SortingItem[] labelTexts = [
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.SalesInvoiceJunOva.lbl.InvoiceDate"), value = "InvoiceDate", ShowIcon = true },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.SalesInvoiceJunOva.lbl.InvoiceNo"), value = "InvoiceCode_0_50", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.SalesInvoiceJunOva.lbl.Customer"), value = "CustomerName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.SalesInvoiceJunOva.lbl.Status"), value = "StatusName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.SalesInvoiceJunOva.lbl.Price"), value = "GrandTotal", ShowIcon = false} 
            ];

        #endregion

        #region "Contructor"
        public VmlSalesInvoice()
        {
            this.switchDisplayView(DisplayView.Card);
            SalesInvoiceLoad = new JSN_LOAD_SALE_INVOICE();
            SalesInvoiceList = new ObservableCollection<RES_SALE_INVOICE>();
            LoadMoreCommand = new Command(async () => await LoadMoreItems());
            sortingList = new ObservableCollection<SortingItem>(labelTexts);
            IsAscending = true;
            IsDescending = false;
        }
        #endregion

        #region "Boolean Declaring"
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
        private bool mIsAscending;
        public bool IsAscending{
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
        #endregion

        #region "Get Set"
        public JSN_LOAD_SALE_INVOICE JSN_LOAD_SALE_INVOICE = new JSN_LOAD_SALE_INVOICE();
        public JSN_LOAD_SALE_INVOICE SalesInvoiceLoad
        {
            get { return JSN_LOAD_SALE_INVOICE; }
            set { JSN_LOAD_SALE_INVOICE = value; NotifyPropertyChanged("SalesInvoiceLoad"); }
        }


        //public RES_SALE_BROWSE mRES_SALE_BROWSE = new RES_SALE_BROWSE();
        //public RES_SALE_BROWSE RES_SALE_BROWSE
        //{
        //    get { return mRES_SALE_BROWSE; }
        //    set { mRES_SALE_BROWSE = value; NotifyPropertyChanged("RES_SALE_BROWSE"); }
        //}

        //public RES_SALE_INVOICE mRES_SALE_INVOICE = new RES_SALE_INVOICE();
        //public RES_SALE_INVOICE RES_SALE_INVOICE
        //{
        //    get { return mRES_SALE_INVOICE; }
        //    set { mRES_SALE_INVOICE = value; NotifyPropertyChanged("RES_SALE_INVOICE"); }
        //}

        //public RES_SALE_INVOICE_DETAIL mRES_SALE_INVOICE_DETAIL = new RES_SALE_INVOICE_DETAIL();
        //public RES_SALE_INVOICE_DETAIL RES_SALE_INVOICE_DETAIL
        //{
        //    get { return mRES_SALE_INVOICE_DETAIL; }
        //    set { mRES_SALE_INVOICE_DETAIL = value; NotifyPropertyChanged("RES_SALE_INVOICE_DETAIL"); }
        //}

        //public RES_COMPANY mRES_COMPANY = new RES_COMPANY();
        //public RES_COMPANY RES_COMPANY
        //{
        //    get { return mRES_COMPANY; }
        //    set { mRES_COMPANY = value; NotifyPropertyChanged("RES_COMPANY"); }
        //}

        public List<RES_CUSTOMER_DTL> mCustomerDtlList;
        public List<RES_CUSTOMER_DTL> CustomerDtlList
        {
            get { return mCustomerDtlList; }
            set { mCustomerDtlList = value; NotifyPropertyChanged("CustomerDtlList"); }
        }

        #endregion


        #region "Load api date range display"
        //initial with user setting range 
        private static readonly DateRange mInitialScheduleRange = Utility.GetInitialScheduleDateRange();

        private DateTime mStartDate = mInitialScheduleRange.StartDate.Date;
        private TimeSpan mStartTime = mInitialScheduleRange.StartDate.TimeOfDay;
        private DateTime mEndDate = mInitialScheduleRange.EndDate.Date;
        private TimeSpan mEndTime = mInitialScheduleRange.EndDate.TimeOfDay;

        public DateTime StartDate
        {
            get => mStartDate;
            set
            {
                if (mStartDate == value)
                    return;

                mStartDate = value;

                NotifyPropertyChanged(nameof(StartDate));

            }
        }
        public TimeSpan StartTime
        {
            get => mStartTime;
            set
            {
                if (mStartTime == value)
                    return;

                mStartTime = value;

                NotifyPropertyChanged(nameof(StartTime));

            }
        }
        public DateTime EndDate
        {
            get => mEndDate;
            set
            {
                if (mEndDate == value)
                    return;

                mEndDate = value;
                NotifyPropertyChanged(nameof(EndDate));
            }
        }
        public TimeSpan EndTime
        {
            get => mEndTime;
            set
            {
                if (mEndTime == value)
                    return;

                mEndTime = value;
                NotifyPropertyChanged(nameof(EndTime));
            }
        }
        #endregion
        #region "Load api data tab"
        public List<RES_CUSTOMER_DTL> mLoadCustomerLst;
        public List<RES_CUSTOMER_DTL> LoadCustomerLst
        {
            get { return mLoadCustomerLst; }
            set { mLoadCustomerLst = value; NotifyPropertyChanged("LoadCustomerLst"); }
        }
        public RES_CUSTOMER_DTL mLoadSelectedCustomer;
        public RES_CUSTOMER_DTL LoadSelectedCustomer
        {
            get { return mLoadSelectedCustomer; }
            set { mLoadSelectedCustomer = value; NotifyPropertyChanged("LoadSelectedCustomer"); }
        }

        public List<DAT_FILTER_RANGE> mLoadFilterRangeLst;
        public List<DAT_FILTER_RANGE> LoadFilterRangeLst
        {
            get { return mLoadFilterRangeLst; }
            set { mLoadFilterRangeLst = value; NotifyPropertyChanged("LoadFilterRangeLst"); }
        }
        public DAT_FILTER_RANGE mLoadSelectedFilterRange;
        public DAT_FILTER_RANGE LoadSelectedFilterRange
        {
            get => mLoadSelectedFilterRange;
            set
            {

                mLoadSelectedFilterRange = value;
                NotifyPropertyChanged(nameof(LoadSelectedFilterRange));

                if (value == null)
                    return;

                DateRange range = Utility.OnFilterRangeChanged(value);

                if (range == null)
                    return;

                StartDate = range.StartDate.Date;
                StartTime = range.StartDate.TimeOfDay;

                EndDate = range.EndDate.Date;
                EndTime = range.EndDate.TimeOfDay;

            }
        }

        public List<RES_STOCK> mLoadStockLst;
        public List<RES_STOCK> LoadStockLst
        {
            get { return mLoadStockLst; }
            set { mLoadStockLst = value; NotifyPropertyChanged("LoadStockLst"); }
        }
        public RES_STOCK mLoadSelectedStock;
        public RES_STOCK LoadSelectedStock
        {
            get { return mLoadSelectedStock; }
            set { mLoadSelectedStock = value; NotifyPropertyChanged("LoadSelectedStock"); }
        }



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
                        //    this.getInvoice();
                        //}
                        mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = new RES_SALE_INVOICE();
                        mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE.Sequence = "0";
                        this.getInvoice();
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
                    mEditItemCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Edit"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.InvoiceCode_0_50}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Send")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                            } 
                        }
                    });
                        //mEditItemCommand = new Command(() => this.switchDisplayView(DisplayView.Grid));
                    //mRefreshCommand = new Command(() => this.getInvoice());
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
                    mDeleteItemCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Delete") && item.PostingStatusAsk != "1" && item.StatusAsk != "9")
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.InvoiceCode_0_50}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Delete")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = item;
                                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE.StatusAsk = "6";
                                saveInvoice();
                            }
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgDelete"));
                        }
                    });
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
                    //mRefreshCommand = new Command(() => this.getInvoice());
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
                    mSendItemCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Send"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.InvoiceCode_0_50}",
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
        private ICommand mActiveItemCommand;
        public ICommand ActiveItemCommand
        {
            get
            {               
                if (mActiveItemCommand == null)
                {
                    mActiveItemCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                    {
                        if (item.StatusAsk == "8" && Utility.checkButtonAccess("Active"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.InvoiceCode_0_50}?",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Active")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                                item.StatusAsk = "1";//1 for active
                                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = item;
                                await ExecuteActiveItem();
                            }
                        }
                        else if (item.StatusAsk != "8" && Utility.checkButtonAccess("Inactive"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.InvoiceCode_0_50}?",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Inactive")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                               item.StatusAsk = "8";//8 for inactive
                               mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = item;
                               await ExecuteActiveItem();
                            }
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                        }
                    });
                }
                return mActiveItemCommand;
            }
        }
        public ICommand LongPressItemCommand { get; }

        private ICommand mCardItemTappedCommand;
        public  ICommand CardItemTappedCommand
        {
            get
            {
                if (mCardItemTappedCommand == null)
                {
                    mCardItemTappedCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                    {
                        bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.InvoiceCode_0_50}?",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Active")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                        if (answer)
                        {
                            //await Navigation.PushAsync(new FrmPosSaleInvoiceSet(item));

                        }
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
        public ICommand LoadMoreCommand { get; }
        #endregion

        #region "Task"
        private async Task LoadMoreItems()
        {
            if (IsLoadingMore) return;
            IsLoadingMore = true;
            getInvoice();
            IsLoadingMore = false;
        }
        private Task ExecuteActiveItem()
        {
            saveInvoice();
            return Task.CompletedTask;
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
    
                var tmp = SalesInvoiceList;
                SalesInvoiceList = null;
                NotifyPropertyChanged(nameof(SalesInvoiceList));

                SalesInvoiceList = tmp;
                NotifyPropertyChanged(nameof(SalesInvoiceList));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void bindDataTab(List<RES_SALE_INVOICE> argRES_SALE_INVOICE_LST)
        {
            SalesInvoiceList ??= new ObservableCollection<RES_SALE_INVOICE>();
            SalesInvoiceList.Clear();
            if (argRES_SALE_INVOICE_LST == null)
                return;
            foreach (RES_SALE_INVOICE l_RES_SALE_INVOICE in argRES_SALE_INVOICE_LST)
            {
                SalesInvoiceList.Add(l_RES_SALE_INVOICE);
            }
        }
        public void searchDataApi(string argKeyword)
        {
            try
            {
                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = new RES_SALE_INVOICE();
                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE.Remark = argKeyword;
                getInvoice();
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
                List<RES_SALE_INVOICE> l_RES_SALE_INVOICE_Lst = new List<RES_SALE_INVOICE>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (RES_SALE_INVOICE l_RES_SALE_INVOICE in mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_RES_SALE_INVOICE.InvoiceCode_0_50.ToLower().Contains(argKeyword)
                            || l_RES_SALE_INVOICE.InvoiceDate.ToLower().Contains(argKeyword)
                            || l_RES_SALE_INVOICE.OutstandingAmount.ToLower().Contains(argKeyword)
                            || l_RES_SALE_INVOICE.GrandTotal.ToLower().Contains(argKeyword))
                        {
                            l_RES_SALE_INVOICE_Lst.Add(l_RES_SALE_INVOICE);
                        }
                    }
                }
                else
                {
                    l_RES_SALE_INVOICE_Lst = new List<RES_SALE_INVOICE>(mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE);// OriginalInvoiceClosedList.GetRange(0, OriginalInvoiceClosedList.Count);
                }
                bindDataTab(l_RES_SALE_INVOICE_Lst);
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
                //loadInvoice(); // if needed call load api for pickers
                callSearchMorePopup();
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
                var popup = new FrmSsmSaleInvoicePop(this);
                await PopupNavigation.Instance.PushAsync(popup);

                //DAT_FRONT_DESK requestedData = new DAT_FRONT_DESK(); 

                var result = await popup.PopupClosedTask;
                if (result is RES_SALE_INVOICE requestedData)
                {
                    //update req data model according to selected data
                    requestedData.CustomerAsk = LoadSelectedCustomer?.Ask ?? "0";
                    requestedData.CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk;
                    DateTime SD = StartDate.Date + StartTime;
                    requestedData.SD = SD.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    DateTime ED = EndDate.Date + EndTime;
                    requestedData.ED = ED.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                    //call api
                    await getInvoice_load(requestedData);

                    // Close THIS popup
                    await PopupNavigation.Instance.RemovePageAsync(popup);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"callSearchMorePopup ERROR: {ex}");
            }
        }


        public void bindCustomer(List<RES_CUSTOMER_DTL> argRES_CUSTOMER_DTL_LST)
        {
            try
            {
                if (argRES_CUSTOMER_DTL_LST != null && argRES_CUSTOMER_DTL_LST.Count > 0)
                {
                    CustomerDtlList = argRES_CUSTOMER_DTL_LST;
                }
                else
                {
                    CustomerDtlList = new List<RES_CUSTOMER_DTL>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion

        #region "Web Service Api"
        public async void getInvoice()
        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE_DETAIL = new List<RES_SALE_INVOICE_DETAIL> { new RES_SALE_INVOICE_DETAIL() };
                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_BROWSE = new List<RES_SALE_BROWSE> { new RES_SALE_BROWSE() };
                mJSN_REQ_SALE_INVOICE_JUN.DAT_RECURRING_TRANSACTION = new List<DAT_RECURRING_TRANSACTION> { new DAT_RECURRING_TRANSACTION() };

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_INVOICE_JUN);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetSaleInvoiceJun);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_SALE_INVOICE_JUN = JsonConvert.DeserializeObject<JSN_SALE_INVOICE_JUN>(mResponse);
                    if (this.mJSN_SALE_INVOICE_JUN.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE.Count > 0)
                        {
                            mRES_SALE_INVOICE_LST = this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE;
                            bindDataTab(this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE);
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
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

        public async void saveInvoice()
        {
            try
            {
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_INVOICE_JUN);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wssaveSaleInvoiceJunOva);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_SALE_INVOICE_JUN = JsonConvert.DeserializeObject<JSN_SALE_INVOICE_JUN>(mResponse);
                    if (mJSN_SALE_INVOICE_JUN.Message.Code == "7")
                    {
                        mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = new RES_SALE_INVOICE();
                        getInvoice();
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                    }
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("ErrWebService"));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public async Task loadInvoice()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsLoadSaleInvoice);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_LOAD_SALE_INVOICE = JsonConvert.DeserializeObject<JSN_LOAD_SALE_INVOICE>(mResponse);
                    if (mJSN_LOAD_SALE_INVOICE.Message.Code == "7")
                    {
                        
                        this.SalesInvoiceLoad = mJSN_LOAD_SALE_INVOICE;
                        LoadFilterRangeLst = mJSN_LOAD_SALE_INVOICE.DAT_FILTER_RANGE;
                        if (mLoadSelectedFilterRange == null)
                        {
                            LoadSelectedFilterRange = Utility.GetUserSettingFilterRange(LoadFilterRangeLst);
                            NotifyPropertyChanged(nameof(LoadSelectedFilterRange));
                        }
                        else
                        {
                            LoadSelectedFilterRange = mLoadSelectedFilterRange;
                            NotifyPropertyChanged(nameof(LoadSelectedFilterRange));
                        }

                        LoadStockLst = mJSN_LOAD_SALE_INVOICE.RES_STOCK;
                        LoadCustomerLst = mJSN_LOAD_SALE_INVOICE.RES_CUSTOMER_DTL;
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_LOAD_SALE_INVOICE.Message.Message);
                    }
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("ErrWebService"));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                Utility.closeLoader();
            }
        }

        public async Task getInvoice_load(RES_SALE_INVOICE argRES_SALE_INVOICE)
        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_SALE_INVOICE_JUN.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = argRES_SALE_INVOICE;
                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE_DETAIL = new List<RES_SALE_INVOICE_DETAIL> { new RES_SALE_INVOICE_DETAIL() };
                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_BROWSE = new List<RES_SALE_BROWSE> { new RES_SALE_BROWSE() };
                mJSN_REQ_SALE_INVOICE_JUN.DAT_RECURRING_TRANSACTION = new List<DAT_RECURRING_TRANSACTION> { new DAT_RECURRING_TRANSACTION() };

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_INVOICE_JUN);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetSaleInvoiceJun);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_SALE_INVOICE_JUN = JsonConvert.DeserializeObject<JSN_SALE_INVOICE_JUN>(mResponse);
                    if (this.mJSN_SALE_INVOICE_JUN.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE.Count > 0)
                        {
                            mRES_SALE_INVOICE_LST = this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE;
                            bindDataTab(this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE);
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
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

        #endregion
    }

}
