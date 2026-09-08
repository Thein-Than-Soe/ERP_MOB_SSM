using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.POS;
using CS.ERP_MOB.Views.SSM;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SSM
{
    public class VmlSsmOrderBook : BaseViewModel
    {
        #region "Declaring"
        //loadSaleOrder
        public JSN_LOAD_SALE_ORDER mJSN_LOAD_SALE_ORDER = new JSN_LOAD_SALE_ORDER();

        //saveSaleOrderJunOva
        public JSN_REQ_SALE_ORDER_JUN mJSN_REQ_SALE_ORDER_JUN_save = new JSN_REQ_SALE_ORDER_JUN();
        public JSN_SALE_ORDER_JUN mJSN_SALE_ORDER_JUN_save = new JSN_SALE_ORDER_JUN();

        //getSaleOrderJun
        public JSN_SALE_ORDER_JUN mJSN_SALE_ORDER_JUN_get = new JSN_SALE_ORDER_JUN();
        public JSN_REQ_SALE_ORDER_JUN mJSN_REQ_SALE_ORDER_JUN_get = new JSN_REQ_SALE_ORDER_JUN();

        public ObservableCollection<SortingItem> sortingList { get; set; }
        SortingItem[] labelTexts = [
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.Setup.lbl.Code"), value = "OrderCode_0_50", ShowIcon = true },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.Setup.lbl.Date"), value = "OrderDate", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.Setup.lbl.Status"), value = "StatusName_0_255", ShowIcon = false }
            ];
        string mRequest = "";
        string mResponse = "";
        #endregion

        #region "Contructor"
        public VmlSsmOrderBook()
        {
            this.switchDisplayView(DisplayView.Card);
            SaleOrderLst = new List<RES_SALE_ORDER>();

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
            }
        }
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

        #region "Data Tab"
        public List<RES_SALE_ORDER> mRES_SALE_ORDER_LST;
        public List<RES_SALE_ORDER> RES_SALE_ORDER_LST
        {
            get { return mRES_SALE_ORDER_LST; }
            set { mRES_SALE_ORDER_LST = value; NotifyPropertyChanged("RES_SALE_ORDER_LST"); }
        }
        public List<RES_SALE_ORDER> mSaleOrderLst;
        public List<RES_SALE_ORDER> SaleOrderLst
        {
            get { return mSaleOrderLst; }
            set { mSaleOrderLst = value; NotifyPropertyChanged("SaleOrderLst"); }
        }

        public List<RES_SALE_ORDER_DETAIL> mSaleOrderDetailLst;
        public List<RES_SALE_ORDER_DETAIL> SaleOrderDetailLst
        {
            get { return mSaleOrderDetailLst; }
            set { mSaleOrderDetailLst = value; NotifyPropertyChanged("SaleOrderDetailLst"); }
        }

        //Barcode UOM,price,currency
        public List<RES_STOCK_BARCODE> mStockBarcodeList = new List<RES_STOCK_BARCODE>();
        public List<RES_STOCK_BARCODE> StockBarcodeList
        {
            get { return mStockBarcodeList; }
            set { mStockBarcodeList = value; NotifyPropertyChanged("StockBarcodeList"); }
        }

        private RES_STOCK_BARCODE mSelectedStockBarcode;

        public RES_STOCK_BARCODE SelectedStockBarcode
        {
            get => mSelectedStockBarcode;
            set
            {
                if (mSelectedStockBarcode == value)
                    return;

                mSelectedStockBarcode = value;

                NotifyPropertyChanged(nameof(SelectedStockBarcode));

                if (mSelectedStockBarcode == null)
                {
                    SelectedPrice = "0";
                    ItemUOM = "";

                    NotifyPropertyChanged(nameof(SelectedPrice));
                    NotifyPropertyChanged(nameof(ItemUOM));

                    CalculateSubtotal();
                    return;
                }

                SelectedPrice = mSelectedStockBarcode.RetailPrice?.ToString() ?? "0";

                ItemUOM = mSelectedStockBarcode.UOMName_0_255 ?? "";

                NotifyPropertyChanged(nameof(SelectedPrice));
                NotifyPropertyChanged(nameof(ItemUOM));

                CalculateSubtotal();
            }
        }

        public List<RES_STOCK> mStockList = new List<RES_STOCK>();
        public List<RES_STOCK> StockList
        {
            get { return mStockList; }
            set { mStockList = value; NotifyPropertyChanged("StockList"); }
        }

        private RES_STOCK mSelectedStock;

        public RES_STOCK SelectedStock
        {
            get => mSelectedStock;
            set
            {
                if (mSelectedStock == value)
                    return;

                mSelectedStock = value;

                NotifyPropertyChanged(nameof(SelectedStock));

                if (mSelectedStock == null)
                {
                    UserSelectedUOM = null;
                    UserSelectedCurrency = null;
                    SelectedStockBarcode = null;

                    SelectedPrice = "0";
                    NotifyPropertyChanged(nameof(SelectedPrice));

                    return;
                }

                UserSelectedUOM = UomList?.FirstOrDefault(
                    x => x.Ask == mSelectedStock.UOMAsk);

                UserSelectedCurrency = CurrencyList?.FirstOrDefault(
                    x => x.Ask == mSelectedStock.CurrencyAsk);

                //UpdateSelectedStockBarcode();
            }
        }

        public List<RES_CUSTOMER_DTL> mCustomerList = new List<RES_CUSTOMER_DTL>();

        public List<RES_CUSTOMER_DTL> CustomerList
        {
            get => mCustomerList;
            set
            {
                mCustomerList = value;
                NotifyPropertyChanged(nameof(CustomerList));
            }
        }
        public List<DAT_BEAT_TYPE> mBeatTypeList;

        public List<DAT_BEAT_TYPE> BeatTypeList
        {
            get => mBeatTypeList;
            set
            {
                mBeatTypeList = value;
                NotifyPropertyChanged(nameof(BeatTypeList));
            }
        }
        public DAT_BEAT_TYPE mSelectedBeatType;

        public DAT_BEAT_TYPE SelectedBeatType
        {
            get => mSelectedBeatType;
            set
            {
                mSelectedBeatType = value;
                NotifyPropertyChanged(nameof(SelectedBeatType));
                HasSelectedBeatType = SelectedBeatType != null;
                NotifyPropertyChanged(nameof(HasSelectedBeatType));
            }
        }


        // Customer
        private RES_CUSTOMER_DTL _selectedCustomer;

        public RES_CUSTOMER_DTL SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (_selectedCustomer == value)
                    return;

                _selectedCustomer = value;

                NotifyPropertyChanged(nameof(SelectedCustomer));

                // Update customer selected state
                IsCustomerSelected = _selectedCustomer != null;

                // Update contact list according to selected customer
                CustomerContactList =
                    _selectedCustomer?.RES_CUSTOMER_CONTACT?.ToList()
                    ?? new List<RES_CUSTOMER_CONTACT>();

                // Select first contact automatically
                SelectedCustomerContact =
                    CustomerContactList.FirstOrDefault();

                NotifyPropertyChanged(nameof(SelectedCustomerContact));
            }
        }


        // Customer contact list
        private List<RES_CUSTOMER_CONTACT> mCustomerContactList =
            new List<RES_CUSTOMER_CONTACT>();

        public List<RES_CUSTOMER_CONTACT> CustomerContactList
        {
            get => mCustomerContactList;
            set
            {
                mCustomerContactList = value;

                NotifyPropertyChanged(nameof(CustomerContactList));
            }
        }


        // Selected customer contact
        private RES_CUSTOMER_CONTACT _selectedCustomerContact;

        public RES_CUSTOMER_CONTACT SelectedCustomerContact
        {
            get => _selectedCustomerContact;
            set
            {
                if (_selectedCustomerContact == value)
                    return;

                _selectedCustomerContact = value;

                NotifyPropertyChanged(nameof(SelectedCustomerContact));
            }
        }
        //Discount
        private List<DAT_DISCOUNT_RULE> mDiscountRules;
        public List<DAT_DISCOUNT_RULE> DiscountRules
        {
            get
            {
                return mDiscountRules;
            }
            set
            {
                mDiscountRules = value;
                NotifyPropertyChanged("DiscountRules");
            }
        }
        private DAT_DISCOUNT_RULE mSelectedRule;
        public DAT_DISCOUNT_RULE SelectedRule
        {
            get
            {
                return mSelectedRule;
            }
            set
            {
                mSelectedRule = value;
                NotifyPropertyChanged("SelectedRule");
                NotifyPropertyChanged("DiscountRate");
                NotifyPropertyChanged("DiscountAmount");
            }
        }

        //Tax Information
        private List<RES_CURRENCY> mCurrencyList;
        public List<RES_CURRENCY> CurrencyList
        {
            get
            {
                return mCurrencyList;
            }
            set
            {
                mCurrencyList = value;
                NotifyPropertyChanged("CurrencyList");
            }
        }
        private RES_CURRENCY mUserSelectedCurrency;
        public RES_CURRENCY UserSelectedCurrency
        {
            get => mUserSelectedCurrency;
            set
            {
                if (mUserSelectedCurrency == value)
                    return;

                mUserSelectedCurrency = value;

                NotifyPropertyChanged(nameof(UserSelectedCurrency));

                UpdateSelectedStockBarcode();
            }
        }

        private List<RES_UOM> mUomList;
        public List<RES_UOM> UomList
        {
            get
            {
                return mUomList;
            }
            set
            {
                mUomList = value;
                NotifyPropertyChanged("UomList");
            }
        }
        private RES_UOM mUserSelectedUOM;

        public RES_UOM UserSelectedUOM
        {
            get => mUserSelectedUOM;
            set
            {
                if (mUserSelectedUOM == value)
                    return;

                mUserSelectedUOM = value;

                NotifyPropertyChanged(nameof(UserSelectedUOM));

                CalculateEndTimeFromQuantity();
                UpdateSelectedStockBarcode();
            }
        }

        private RES_GST mTaxInformation;
        public RES_GST TaxInformation
        {
            get
            {
                return mTaxInformation;
            }
            set
            {
                mTaxInformation = value;
                NotifyPropertyChanged("TaxInformation");
            }
        }
        #endregion


        #region "Task"
        private async Task LoadMoreItems()
        {
            if (IsLoadingMore) return;
            IsLoadingMore = true;
            await loadSaleOrder();
            await getSaleOrderJun();
            IsLoadingMore = false;
        }
        //private Task ExecuteActiveItem()
        //{
        //    saveMyOrder();
        //    return Task.CompletedTask;
        //}

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
                    mRefreshCommand = new Command(() => this.loadSaleOrder());
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
                    mEditItemCommand = new Command<RES_SALE_ORDER>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Edit"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.OrderCode_0_50}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Send")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                                //route to detail page
                            }
                        }
                    });
                    //mEditItemCommand = new Command(() => this.switchDisplayView(DisplayView.Grid));
                    //mRefreshCommand = new Command(() => this.getInvoice());
                }
                return mEditItemCommand;
            }
        }
        private ICommand mSelectItemCommand;
        public ICommand SelectItemCommand
        {
            get
            {
                if (mSelectItemCommand == null)
                {
                    //route to detail page
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
                    mSendItemCommand = new Command<RES_SALE_ORDER>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Send"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.OrderCode_0_50}",
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
                    mCardItemTappedCommand = new Command<RES_SALE_ORDER>(async (item) =>
                    {
                        bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.OrderCode_0_50}?",
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
        //private void bindDataTab(List<RES_STOCK> argRES_STOCK_LST)
        //{
        //    try
        //    {
        //        if (argRES_STOCK_LST != null && argRES_STOCK_LST.Count > 0)
        //        {
        //            SaleOrderLst = argRES_STOCK_LST;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }
        //}

        public void searchDataApi(string argKeyword)
        {
            try
            {
                //mJSN_REQ_SALE_LOAD.RES = new RES_SALE_ORDER();
                //mJSN_REQ_SALE_LOAD.RES_SALE_ORDER.Remark = argKeyword;
                loadSaleOrder();
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
                List<RES_STOCK> l_RES_STOCK_lst = new List<RES_STOCK>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (RES_STOCK l_RES_STOCK in mJSN_LOAD_SALE_ORDER.RES_STOCK)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_RES_STOCK.StockCode_0_50.ToLower().Contains(argKeyword)
                            || l_RES_STOCK.SD.ToLower().Contains(argKeyword)
                            || l_RES_STOCK.ED.ToLower().Contains(argKeyword)
                            )
                        {
                            l_RES_STOCK_lst.Add(l_RES_STOCK);
                        }
                    }
                }
                else
                {
                    l_RES_STOCK_lst = mJSN_LOAD_SALE_ORDER.RES_STOCK;// OriginalMyOrderClosedList.GetRange(0, OriginalMyOrderClosedList.Count);
                }
                //bindDataTab(l_RES_STOCK_lst);
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
                loadSaleOrder();
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
                var popup = new FrmSsmOrderBookPop(this.mJSN_SALE_ORDER_JUN_get);
                await PopupNavigation.Instance.PushAsync(popup);

                var result = await popup.PopupClosedTask;
                if (result is RES_SALE_ORDER selectedData)
                {
                    //mJSN_SALE_ORDER_JUN_get.RES_SALE_ORDER = selectedData;
                    if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                    {
                        SaleOrderLst = new List<RES_SALE_ORDER>(mRES_SALE_ORDER_LST.Where(data => (data.CustomerAsk == selectedData.CustomerAsk)
                                                                              || (data.OrderCode_0_50 == selectedData.OrderCode_0_50)).ToList());
                    }
                    else
                    {
                        await getSaleOrderJun();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        //Stock
        private void UpdateSelectedStockBarcode()
        {
            if (SelectedStock == null ||
                UserSelectedUOM == null ||
                UserSelectedCurrency == null ||
                StockBarcodeList == null)
            {
                SelectedStockBarcode = null;

                SelectedPrice = "0";
                NotifyPropertyChanged(nameof(SelectedPrice));

                return;
            }

            SelectedStockBarcode = StockBarcodeList.FirstOrDefault(x =>
                x.StockAsk == SelectedStock.Ask &&
                x.UOMAsk == UserSelectedUOM.Ask &&
                x.CurrencyAsk == UserSelectedCurrency.Ask);

            if (SelectedStockBarcode == null)
            {
                SelectedPrice = "0";
                Application.Current?.MainPage?.DisplayAlert(
        "Barcode Not Found",
        "There is no barcode available for the selected Stock, UOM, and Currency.",
        "OK");
            }
            else
            {
                SelectedPrice = SelectedStockBarcode.RetailPrice?.ToString() ?? "0";
            }

            NotifyPropertyChanged(nameof(SelectedPrice));
        }

        #endregion

        #region "GST method"

        public class GSTMethodItem
        {
            public string Ask { get; set; }
            public string GSTMethodName { get; set; }
        }
        public List<GSTMethodItem> GSTMethodList { get; set; } = new()
            {
                new GSTMethodItem
                {
                    Ask = "I",
                    GSTMethodName = "Inclusive"
                },
                new GSTMethodItem
                {
                    Ask = "E",
                    GSTMethodName = "Exclusive"
                }
            };

        private GSTMethodItem mSelectedGSTMethod;

        public GSTMethodItem SelectedGSTMethod
        {
            get => mSelectedGSTMethod;
            set
            {
                if (mSelectedGSTMethod == value)
                    return;

                mSelectedGSTMethod = value;

                NotifyPropertyChanged(nameof(SelectedGSTMethod));

                CalculateGrandTotal();
            }
        }
        private bool mIsGSTEnabled;

        public bool IsGSTEnabled
        {
            get => mIsGSTEnabled;
            set
            {
                if (mIsGSTEnabled == value)
                    return;

                mIsGSTEnabled = value;

                NotifyPropertyChanged(nameof(IsGSTEnabled));

                CalculateGrandTotal();
            }
        }

        //round off
        private bool mIsRoundOffEnabled;

        public bool IsRoundOffEnabled
        {
            get => mIsRoundOffEnabled;
            set
            {
                if (mIsRoundOffEnabled == value)
                    return;

                mIsRoundOffEnabled = value;

                NotifyPropertyChanged(nameof(IsRoundOffEnabled));

                CalculateGrandTotal();
            }
        }
        #endregion

        #region "Display View property"

        public bool HasDiscount => DiscountAmount > 0;
        private bool mIsSyncingTimeAndQuantity;
        private bool mIsServiceAdded = false;

        public bool IsServiceAdded
        {
            get => mIsServiceAdded;
            set
            {
                if (mIsServiceAdded != value)
                {
                    mIsServiceAdded = value;
                    NotifyPropertyChanged(nameof(IsServiceAdded));
                }
            }
        }
        private bool mIsServiceNotAdded = true;
        public bool IsServiceNotAdded
        {
            get => mIsServiceNotAdded;
            set
            {
                if (mIsServiceNotAdded != value)
                {
                    mIsServiceNotAdded = value;
                    NotifyPropertyChanged(nameof(IsServiceNotAdded));
                }
            }
        }
        private bool mIsCustomerSelected;

        public bool IsCustomerSelected
        {
            get => mIsCustomerSelected;
            set
            {
                if (mIsCustomerSelected == value)
                    return;

                mIsCustomerSelected = value;
                NotifyPropertyChanged(nameof(IsCustomerSelected));
            }
        }


        private bool mHasSelectedBeatType = false;

        public bool HasSelectedBeatType
        {
            get => mHasSelectedBeatType;
            set
            {
                if (mHasSelectedBeatType == value)
                    return;

                mHasSelectedBeatType = value;
                NotifyPropertyChanged(nameof(HasSelectedBeatType));
            }
        }
        private bool mIsBankVisible;

        public bool IsBankVisible
        {
            get => mIsBankVisible;
            set
            {
                if (mIsBankVisible == value)
                    return;

                mIsBankVisible = value;
                NotifyPropertyChanged(nameof(IsBankVisible));
            }
        }

        private string mTender;

        public string Tender
        {
            get => mTender;
            set
            {
                if (mTender == value)
                    return;

                mTender = value;
                NotifyPropertyChanged(nameof(Tender));

                CalculateChange();
            }
        }

        private string mChange;

        public string Change
        {
            get => mChange;
            set
            {
                if (mChange == value)
                    return;

                mChange = value;
                NotifyPropertyChanged(nameof(Change));
            }
        }

        private void CalculateChange()
        {
            if (decimal.TryParse(Tender, out decimal tenderAmount))
            {
                decimal changeAmount = tenderAmount - GrandTotal;

                Change = changeAmount >= 0
                    ? changeAmount.ToString("0.00")
                    : "0.00";
            }
            else
            {
                Change = "0.00";
            }
        }


        private bool mIsCashVisible;

        public bool IsCashVisible
        {
            get => mIsCashVisible;
            set
            {
                if (mIsCashVisible == value)
                    return;

                mIsCashVisible = value;
                NotifyPropertyChanged(nameof(IsCashVisible));
            }
        }

        private DateTime mTransactionDate = DateTime.Now;

        public DateTime TransactionDate
        {
            get => mTransactionDate;
            set
            {
                if (mTransactionDate == value)
                    return;

                mTransactionDate = value;
                NotifyPropertyChanged(nameof(TransactionDate));
            }
        }

        private TimeSpan mTransactionTime = DateTime.Now.TimeOfDay;

        public TimeSpan TransactionTime
        {
            get => mTransactionTime;
            set
            {
                if (mTransactionTime == value)
                    return;

                mTransactionTime = value;
                NotifyPropertyChanged(nameof(TransactionTime));
            }
        }


        private string mTransactionNo;

        public string TransactionNo
        {
            get => mTransactionNo;
            set
            {
                if (mTransactionNo == value)
                    return;

                mTransactionNo = value;
                NotifyPropertyChanged(nameof(TransactionNo));
            }
        }


        private bool mIsTransactionDateEditable;

        public bool IsTransactionDateEditable
        {
            get => mIsTransactionDateEditable;
            set
            {
                if (mIsTransactionDateEditable == value)
                    return;

                mIsTransactionDateEditable = value;
                NotifyPropertyChanged(nameof(IsTransactionDateEditable));
            }
        }


        private bool mIsTransactionNoVisible;

        public bool IsTransactionNoVisible
        {
            get => mIsTransactionNoVisible;
            set
            {
                if (mIsTransactionNoVisible == value)
                    return;

                mIsTransactionNoVisible = value;
                NotifyPropertyChanged(nameof(IsTransactionNoVisible));
            }
        }


        private string mTransactionNoLabel = "Transaction No";

        public string TransactionNoLabel
        {
            get => mTransactionNoLabel;
            set
            {
                if (mTransactionNoLabel == value)
                    return;

                mTransactionNoLabel = value;
                NotifyPropertyChanged(nameof(TransactionNoLabel));
            }
        }



        //Amount calculate
        private decimal mQuantity = 1;

        public decimal Quantity
        {
            get => mQuantity;
            set
            {
                if (mQuantity == value)
                    return;

                mQuantity = value;

                NotifyPropertyChanged(nameof(Quantity));

                CalculateEndTimeFromQuantity();
                CalculateSubtotal();
            }
        }

        private decimal mSubtotal;
        public decimal Subtotal
        {
            get => mSubtotal;
            set
            {
                if (mSubtotal == value)
                    return;

                mSubtotal = value;

                NotifyPropertyChanged(nameof(Subtotal));
                // Recalculate discount whenever subtotal changes
                CalculateSubtotalDiscount();
            }
        }
        private string mRevenueAmount;
        public string RevenueAmount
        {
            get => mRevenueAmount;
            set
            {
                if (mRevenueAmount == value)
                    return;

                mRevenueAmount = value;

                NotifyPropertyChanged(nameof(RevenueAmount));
            }
        }

        private decimal _discountAmount;

        public decimal DiscountAmount
        {
            get => _discountAmount;
            set
            {
                if (_discountAmount == value)
                    return;

                _discountAmount = value;

                NotifyPropertyChanged(nameof(DiscountAmount));
                NotifyPropertyChanged(nameof(HasDiscount));
                CalculateGrandTotal();
            }
        }
        private string mSelectedDiscountTypeAsk = "0";

        public string SelectedDiscountTypeAsk
        {
            get => mSelectedDiscountTypeAsk;
            set
            {
                if (mSelectedDiscountTypeAsk == value)
                    return;

                mSelectedDiscountTypeAsk = value;

                NotifyPropertyChanged(nameof(SelectedDiscountTypeAsk));

                // User changed discount type.
                // Recalculate amount using current Type + Rate.
                RecalculateDiscountAmount();
            }
        }
        private decimal mDiscountRate;

        public decimal DiscountRate
        {
            get => mDiscountRate;
            set
            {
                if (mDiscountRate == value)
                    return;

                mDiscountRate = value;

                NotifyPropertyChanged(nameof(DiscountRate));

                // User changed rate.
                // Only recalculate using current Type + Rate.
                RecalculateDiscountAmount();
            }
        }
        private decimal _taxAmount;

        public decimal TaxAmount
        {
            get => _taxAmount;
            set
            {
                if (_taxAmount == value)
                    return;

                _taxAmount = value;

                NotifyPropertyChanged(nameof(TaxAmount));
            }
        }
        private decimal _grandTotal;

        public decimal GrandTotal
        {
            get => _grandTotal;
            set
            {
                if (_grandTotal == value)
                    return;

                _grandTotal = value;

                NotifyPropertyChanged(nameof(GrandTotal));

                CalculateChange();
                //CalculateRemainingAmount();
            }
        }

        private decimal mRoundOffAmount;
        public decimal RoundOffAmount
        {
            get => mRoundOffAmount;
            set
            {
                if (mRoundOffAmount != value)
                {
                    mRoundOffAmount = value;
                    NotifyPropertyChanged(nameof(RoundOffAmount));
                }
            }
        }


        private string mDepositAmount;
        public string DepositAmount
        {
            get => mDepositAmount;
            set
            {
                if (mDepositAmount == value)
                    return;

                mDepositAmount = value;

                NotifyPropertyChanged(nameof(DepositAmount));

                //CalculateRemainingAmount();
            }
        }


        private string mRemainingAmount;

        public string RemainingAmount
        {
            get => mRemainingAmount;
            set
            {
                if (mRemainingAmount == value)
                    return;

                mRemainingAmount = value;

                NotifyPropertyChanged(nameof(RemainingAmount));
            }
        }

        private string mSelectedPrice;

        public string SelectedPrice
        {
            get => mSelectedPrice;
            set
            {
                if (mSelectedPrice == value)
                    return;

                mSelectedPrice = value;
                NotifyPropertyChanged(nameof(SelectedPrice));
            }
        }


        private string mItemUOM;

        public string ItemUOM
        {
            get => mItemUOM;
            set
            {
                if (mItemUOM == value)
                    return;

                mItemUOM = value;
                NotifyPropertyChanged(nameof(ItemUOM));
            }
        }


        private string mSelectedCurrency;

        public string SelectedCurrency
        {
            get => mSelectedCurrency;
            set
            {
                if (mSelectedCurrency == value)
                    return;

                mSelectedCurrency = value;
                NotifyPropertyChanged(nameof(SelectedCurrency));
            }
        }

        //For order date, start time, end time
        private DateTime mOrderDate = DateTime.Now.Date;

        public DateTime OrderDate
        {
            get => mOrderDate;
            set
            {
                if (mOrderDate == value)
                    return;

                mOrderDate = value;
                NotifyPropertyChanged(nameof(OrderDate));
            }
        }

        private TimeSpan mOrderTime = DateTime.Now.TimeOfDay;

        public TimeSpan OrderTime
        {
            get => mOrderTime;
            set
            {
                if (mOrderTime == value)
                    return;

                mOrderTime = value;
                NotifyPropertyChanged(nameof(OrderTime));
            }
        }

        private DateTime mStartDate = DateTime.Now.Date;

        public DateTime StartDate
        {
            get => mStartDate;
            set
            {
                if (mStartDate == value)
                    return;

                mStartDate = value;

                NotifyPropertyChanged(nameof(StartDate));

                CalculateEndTimeFromQuantity();
            }
        }
        private TimeSpan mStartTime = DateTime.Now.TimeOfDay;

        public TimeSpan StartTime
        {
            get => mStartTime;
            set
            {
                if (mStartTime == value)
                    return;

                mStartTime = value;

                NotifyPropertyChanged(nameof(StartTime));

                CalculateEndTimeFromQuantity();
            }
        }
        private DateTime mEndDate;

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
        private TimeSpan mEndTime;

        public TimeSpan EndTime
        {
            get => mEndTime;
            set
            {
                if (mEndTime == value)
                    return;

                mEndTime = value;
                NotifyPropertyChanged(nameof(EndTime));
                if (!mIsSyncingTimeAndQuantity)
                {
                    CalculateQuantityFromEndTime();
                }
            }
        }

        private DateTime mRecurringDate;
        public DateTime RecurringDate
        {
            get => mRecurringDate;
            set
            {
                if (mRecurringDate == value)
                    return;

                mRecurringDate = value;
                NotifyPropertyChanged(nameof(RecurringDate));
            }
        }
        private TimeSpan mRecurringTime;

        public TimeSpan RecurringTime
        {
            get => mRecurringTime;
            set
            {
                if (mRecurringTime == value)
                    return;

                mRecurringTime = value;
                NotifyPropertyChanged(nameof(RecurringTime));
            }
        }
        #endregion

        #region "Amount calculate methods"

        private bool IsHour(string uom)
        {
            return uom == "hour" ||
                   uom == "hours" ||
                   uom == "hr" ||
                   uom == "hrs";
        }

        private bool IsDay(string uom)
        {
            return uom == "day" ||
                   uom == "days";
        }
        private void CalculateEndTimeFromQuantity()
        {
            if (mIsSyncingTimeAndQuantity)
                return;

            if (UserSelectedUOM == null)
                return;

            DateTime startDateTime = StartDate.Date + StartTime;

            string uom = UserSelectedUOM.UOMName_0_255?
                .Trim()
                .ToLower() ?? "";

            DateTime endDateTime;

            if (IsHour(uom))
            {
                endDateTime = startDateTime.AddHours((double)Quantity);
            }
            else if (IsDay(uom))
            {
                endDateTime = startDateTime.AddDays((double)Quantity);
            }
            else
            {
                return;
            }

            try
            {
                mIsSyncingTimeAndQuantity = true;

                EndDate = endDateTime.Date;
                EndTime = endDateTime.TimeOfDay;

                NotifyPropertyChanged(nameof(EndDate));
                NotifyPropertyChanged(nameof(EndTime));
            }
            finally
            {
                mIsSyncingTimeAndQuantity = false;
            }
        }

        private void CalculateQuantityFromEndTime()
        {
            if (mIsSyncingTimeAndQuantity)
                return;

            if (UserSelectedUOM == null)
                return;

            DateTime startDateTime = StartDate.Date + StartTime;

            DateTime endDateTime = StartDate.Date + EndTime;

            // End time is earlier than or equal to start time,
            // so treat it as the next day.
            if (EndTime <= StartTime)
            {
                endDateTime = endDateTime.AddDays(1);
            }

            TimeSpan duration = endDateTime - startDateTime;

            string uom = UserSelectedUOM.UOMName_0_255?
                .Trim()
                .ToLower() ?? "";

            decimal newQuantity;

            if (IsHour(uom))
            {
                newQuantity = (decimal)duration.TotalHours;
            }
            else if (IsDay(uom))
            {
                newQuantity = (decimal)duration.TotalDays;
            }
            else
            {
                return;
            }

            try
            {
                mIsSyncingTimeAndQuantity = true;

                EndDate = endDateTime.Date;

                NotifyPropertyChanged(nameof(EndDate));

                Quantity = newQuantity;

                CalculateSubtotal();
            }
            finally
            {
                mIsSyncingTimeAndQuantity = false;
            }
        }
        private void CalculateSubtotal()
        {
            if (SelectedStockBarcode == null)
            {
                Subtotal = 0;
                return;
            }

            if (!decimal.TryParse(
                SelectedStockBarcode.RetailPrice?.ToString(),
                out decimal price))
            {
                price = 0;
            }

            Subtotal = price * Quantity;
            CalculateSubtotalDiscount();
            CalculateGrandTotal();
        }
        private decimal ParseDecimal(string? value)
        {
            if (decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal result))
            {
                return result;
            }

            return 0;
        }
        private void CalculateSubtotalDiscount()
        {
            var matchedRule = FindMatchingDiscountRule(
                discountCalculationFigureAsk: "4",
                calculationValue: Subtotal,
                discountDate: OrderDate,
                currencyAsk: UserSelectedCurrency?.Ask ?? "");

            SelectedRule = matchedRule;

            if (matchedRule == null)
            {
                // No matching rule
                mSelectedDiscountTypeAsk = "0";
                mDiscountRate = 0;

                NotifyPropertyChanged(nameof(SelectedDiscountTypeAsk));
                NotifyPropertyChanged(nameof(DiscountRate));

                DiscountAmount = 0;

                return;
            }

            // Get default values from matched rule
            mSelectedDiscountTypeAsk = matchedRule.DiscountTypeAsk;
            mDiscountRate = ParseDecimal(matchedRule.Rate);

            NotifyPropertyChanged(nameof(SelectedDiscountTypeAsk));
            NotifyPropertyChanged(nameof(DiscountRate));

            // Calculate amount using matched rule's Type + Rate
            RecalculateDiscountAmount();
        }
        private void RecalculateDiscountAmount()
        {
            decimal discount = CalculateDiscount(
                discountCalculationFigureAsk: "4",
                calculationValue: Subtotal,
                discountDate: OrderDate,
                currencyAsk: UserSelectedCurrency?.Ask ?? "");

            DiscountAmount = discount;
        }
        private decimal CalculateDiscount(
    string discountCalculationFigureAsk,
    decimal calculationValue,
    DateTime discountDate,
    string currencyAsk)
        {
            if (calculationValue <= 0)
                return 0;

            // At this point:
            // SelectedDiscountTypeAsk can come from the matched rule
            // OR from user selection.
            //
            // DiscountRate can come from the matched rule
            // OR from user input.

            string discountTypeAsk = SelectedDiscountTypeAsk;
            decimal rate = DiscountRate;

            decimal discount = 0;

            switch (discountTypeAsk)
            {
                case "1": // %
                    discount = calculationValue * rate / 100m;
                    break;

                case "2": // $
                    discount = rate;
                    break;

                case "3": // Coupon
                    discount = CalculateCouponDiscount(
                        SelectedRule,
                        rate,
                        calculationValue);
                    break;

                case "4": // By X get X
                case "5": // By X get Y
                          // Handle later when item/tier logic is added
                    discount = 0;
                    break;

                default:
                    discount = 0;
                    break;
            }

            if (discount < 0)
                discount = 0;

            if (discount > calculationValue)
                discount = calculationValue;

            return discount;
        }
        private decimal CalculateCouponDiscount(DAT_DISCOUNT_RULE rule, decimal rate, decimal calculationValue)
        {
            if (rule == null || rate <= 0)
                return 0;

            switch (rule.ValueTypeAsk)
            {
                case "1": // Fixed
                    return rate;

                case "2": // Percentage
                    return calculationValue * rate / 100m;

                default:
                    return 0;
            }
        }
        private DAT_DISCOUNT_RULE FindMatchingDiscountRule(
    string discountCalculationFigureAsk,
    decimal calculationValue,
    DateTime discountDate,
    string currencyAsk)
        {
            if (DiscountRules == null || DiscountRules.Count == 0)
                return null;

            var matchedRules = DiscountRules
                .Where(rule =>
                    rule.DiscountCalculationFigureAsk == discountCalculationFigureAsk &&
                    IsDiscountDateMatched(
                        discountDate,
                        DateTime.Parse(rule.SD),
                        DateTime.Parse(rule.ED)) &&
                    IsDiscountCurrencyMatched(rule, currencyAsk) &&
                    IsDiscountConditionMatched(rule, calculationValue))
                .OrderByDescending(rule =>
                    ParseDecimal(rule.DiscountCalculationAmount))
                .ToList();

            return matchedRules.FirstOrDefault();
        }
        private bool IsDiscountDateMatched(DateTime discountDate, DateTime startDate, DateTime endDate)
        {
            if (startDate.Kind == DateTimeKind.Utc)
                startDate = startDate.ToLocalTime();

            if (endDate.Kind == DateTimeKind.Utc)
                endDate = endDate.ToLocalTime();

            if (discountDate.Kind == DateTimeKind.Utc)
                discountDate = discountDate.ToLocalTime();

            discountDate = discountDate.Date;
            startDate = startDate.Date;
            endDate = endDate.Date;

            return discountDate >= startDate &&
                   discountDate <= endDate;
        }
        private bool IsDiscountCurrencyMatched(DAT_DISCOUNT_RULE rule, string currencyAsk)
        {
            if (rule == null)
                return false;

            // Only fixed amount needs currency matching.
            // Percentage does not need it.
            if (rule.DiscountTypeAsk != "2")
                return true;

            return rule.CurrencyAsk == currencyAsk;
        }
        private bool IsDiscountConditionMatched(DAT_DISCOUNT_RULE rule, decimal calculationValue)
        {
            decimal conditionAmount =
                ParseDecimal(rule.DiscountCalculationAmount);

            switch (rule.DiscountConditionTypeAsk)
            {
                // >=
                case "1":
                    return calculationValue >= conditionAmount;
                // <=
                case "2":
                    return calculationValue <= conditionAmount;
                // >
                case "3":
                    return calculationValue > conditionAmount;
                // Between
                case "4":

                    // Between requires a minimum and maximum value.
                    // DAT_DISCOUNT_RULE currently only shows
                    // DiscountCalculationAmount.
                    //
                    // So this needs the actual upper-bound field
                    // from your API/model before implementing.
                    return false;


                default:

                    return false;
            }
        }

        //private void CalculateDiscount()
        //{
        //    // Reset first
        //    DiscountAmount = 0;

        //    if (DiscountRules == null || DiscountRules.Count == 0)
        //        return;

        //    if (Subtotal <= 0)
        //        return;

        //    // FIND ALL RULES THAT MATCH CURRENT SUBTOTAL
        //    var matchedRules = DiscountRules
        //        .Where(rule => IsDiscountRulesMatched(rule, Subtotal))
        //        .OrderByDescending(rule =>
        //            ParseDecimal(rule.DiscountCalculationAmount))
        //        .ToList();

        //    // No matching rule
        //    if (matchedRules.Count == 0)
        //        return;

        //    // Select the matching rule
        //    SelectedRule = matchedRules.First();

        //    // =========================================================
        //    // Set default DiscountRate from SelectedRule
        //    // =========================================================
        //    decimal defaultRate = ParseDecimal(SelectedRule.Rate);

        //    if (mDiscountRate == 0)
        //    {
        //        mDiscountRate = defaultRate;
        //        NotifyPropertyChanged(nameof(DiscountRate));
        //    }

        //    // Use the ViewModel DiscountRate
        //    // User can change this value from the Entry
        //    decimal rate = DiscountRate;


        //    // =========================================================
        //    // DiscountTypeAsk:
        //    //
        //    // 1 = Percentage
        //    // 2 = Fixed amount
        //    // 3 = Coupon
        //    // =========================================================

        //    switch (SelectedRule.DiscountTypeAsk)
        //    {
        //        // Percentage
        //        case "1":

        //            DiscountAmount = Subtotal * rate / 100m;

        //            break;


        //        // Fixed amount
        //        case "2":

        //            DiscountAmount =
        //                ParseDecimal(SelectedRule.DiscountCalculationAmount);

        //            break;


        //        // Coupon
        //        case "3":

        //            DiscountAmount = Subtotal * rate / 100m;

        //            break;


        //        default:

        //            DiscountAmount = 0;

        //            break;
        //    }


        //    // =========================================================
        //    // Never allow discount greater than subtotal
        //    // =========================================================

        //    if (DiscountAmount > Subtotal)
        //    {
        //        DiscountAmount = Subtotal;
        //    }


        //    NotifyPropertyChanged(nameof(DiscountAmount));
        //    NotifyPropertyChanged(nameof(HasDiscount));
        //}
        ////private bool IsDiscountRulesMatched(DAT_DISCOUNT_RULE rule, decimal subtotal)
        ////{
        ////    decimal conditionAmount = ParseDecimal(rule.DiscountCalculationAmount);


        ////    switch (rule.DiscountConditionTypeName_0_255?.Trim())
        ////    {
        ////        case ">=":
        ////            return subtotal >= conditionAmount;

        ////        case ">":
        ////            return subtotal > conditionAmount;

        ////        case "=":
        ////        case "==":
        ////            return subtotal == conditionAmount;

        ////        case "<=":
        ////            return subtotal <= conditionAmount;

        ////        case "<":
        ////            return subtotal < conditionAmount;

        ////        default:
        ////            return false;
        ////    }
        ////}
        //private bool IsDiscountRulesMatched( DAT_DISCOUNT_RULE rule,decimal subtotal)
        //{
        //    // ---------------------------------------------------------
        //    // 1. Check subtotal condition
        //    // ---------------------------------------------------------

        //    decimal conditionAmount =
        //        ParseDecimal(rule.DiscountCalculationAmount);

        //    bool amountMatched;

        //    switch (rule.DiscountConditionTypeName_0_255?.Trim())
        //    {
        //        case ">=":
        //            amountMatched = subtotal >= conditionAmount;
        //            break;

        //        case ">":
        //            amountMatched = subtotal > conditionAmount;
        //            break;

        //        case "=":
        //        case "==":
        //            amountMatched = subtotal == conditionAmount;
        //            break;

        //        case "<=":
        //            amountMatched = subtotal <= conditionAmount;
        //            break;

        //        case "<":
        //            amountMatched = subtotal < conditionAmount;
        //            break;

        //        default:
        //            amountMatched = false;
        //            break;
        //    }

        //    // Amount does not match
        //    if (!amountMatched)
        //        return false;


        //    // ---------------------------------------------------------
        //    // 2. Check discount start/end date
        //    // ---------------------------------------------------------

        //    if (OrderDate == default)
        //        return false;


        //    // Parse SD
        //    if (!DateTime.TryParse(
        //        rule.SD,
        //        null,
        //        DateTimeStyles.RoundtripKind,
        //        out DateTime startDate))
        //    {
        //        return false;
        //    }


        //    // Parse ED
        //    if (!DateTime.TryParse(
        //        rule.ED,
        //        null,
        //        DateTimeStyles.RoundtripKind,
        //        out DateTime endDate))
        //    {
        //        return false;
        //    }


        //    // ---------------------------------------------------------
        //    // Convert everything to the same timezone
        //    // ---------------------------------------------------------

        //    DateTime orderDate = OrderDate;

        //    if (startDate.Kind == DateTimeKind.Utc)
        //        startDate = startDate.ToLocalTime();

        //    if (endDate.Kind == DateTimeKind.Utc)
        //        endDate = endDate.ToLocalTime();


        //    // Remove time if SD/ED are intended as whole dates
        //    startDate = startDate.Date;
        //    endDate = endDate.Date;
        //    orderDate = orderDate.Date;


        //    // ---------------------------------------------------------
        //    // OrderDate must be between SD and ED
        //    // ---------------------------------------------------------

        //    if (orderDate < startDate || orderDate > endDate)
        //        return false;


        //    return true;
        //}

        private void CalculateGrandTotal()
        {
            // ==========================================
            // TAXABLE AMOUNT
            // ==========================================

            decimal taxableAmount = Subtotal - DiscountAmount;

            if (taxableAmount < 0)
                taxableAmount = 0;

            this.RevenueAmount = taxableAmount.ToString();

            // ==========================================
            // GST
            // ==========================================

            TaxAmount = 0;

            if (IsGSTEnabled &&
                TaxInformation != null)
            {
                decimal taxRate =
                    ParseDecimal(TaxInformation.GSTRate);

                if (taxRate > 0)
                {
                    if (SelectedGSTMethod?.Ask == "I")
                    {
                        // ==========================================
                        // INCLUSIVE GST
                        // ==========================================
                        // GST is already included in the amount.
                        // Calculate TaxAmount for display/storage,
                        // but DO NOT add it to GrandTotal.

                        decimal amountBasedOnGST =
                            (100m + taxRate) / 100m;

                        decimal gstRateInAmount =
                            taxRate / 100m;

                        TaxAmount =
                            (taxableAmount / amountBasedOnGST)
                            * gstRateInAmount;
                    }
                    else
                    {
                        // ==========================================
                        // EXCLUSIVE GST
                        // ==========================================
                        // GST is added on top of taxable amount.

                        TaxAmount =
                            (taxableAmount / 100m)
                            * taxRate;
                    }
                }
            }


            // ==========================================
            // TOTAL BEFORE ROUND OFF
            // ==========================================

            decimal totalBeforeRoundOff;

            if (SelectedGSTMethod?.Ask == "I")
            {
                // Inclusive:
                // GST is already included in taxableAmount.
                totalBeforeRoundOff = taxableAmount;
            }
            else
            {
                // Exclusive:
                // Add GST to taxable amount.
                totalBeforeRoundOff =
                    taxableAmount + TaxAmount;
            }

            if (totalBeforeRoundOff < 0)
                totalBeforeRoundOff = 0;


            // ==========================================
            // ROUND OFF
            // ==========================================

            if (IsRoundOffEnabled)
            {
                decimal roundedTotal =
                    Math.Ceiling(totalBeforeRoundOff);

                RoundOffAmount =
                    roundedTotal - totalBeforeRoundOff;

                GrandTotal = roundedTotal;
            }
            else
            {
                RoundOffAmount = 0;

                GrandTotal = totalBeforeRoundOff;
            }
        }
        #endregion

        #region "Web Service Api"
        public async Task loadSaleOrder()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsloadSaleOrder);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_LOAD_SALE_ORDER = JsonConvert.DeserializeObject<JSN_LOAD_SALE_ORDER>(mResponse);
                    if (mJSN_LOAD_SALE_ORDER.Message.Code == "7")
                    {
                        if (this.mJSN_LOAD_SALE_ORDER.RES_STOCK.Count > 0)
                        {
                            //load all picker list here
                            //StockList = this.mJSN_LOAD_SALE_ORDER.RES_STOCK;
                            WeakReferenceMessenger.Default.Send(this.mJSN_LOAD_SALE_ORDER.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_LOAD_SALE_ORDER.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_LOAD_SALE_ORDER.Message.Message);
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
            finally
            {
                Utility.closeLoader();
            }
        }
        public async Task getSaleOrderJun()
        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_SALE_ORDER_JUN_get.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_SALE_ORDER_JUN_get.RES_SALE_ORDER.CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk;
                mJSN_REQ_SALE_ORDER_JUN_get.RES_SALE_ORDER.SD = Utility.getTLFormLoadSD();
                mJSN_REQ_SALE_ORDER_JUN_get.RES_SALE_ORDER.ED = Utility.getTLFormLoadED();

                mJSN_REQ_SALE_ORDER_JUN_get.RES_SALE_ORDER_DETAIL = new List<RES_SALE_ORDER_DETAIL> { };
                mJSN_REQ_SALE_ORDER_JUN_get.RES_SALE_BROWSE = new List<RES_SALE_BROWSE> { };

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_ORDER_JUN_get);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetSaleOrderJun);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_SALE_ORDER_JUN_get = JsonConvert.DeserializeObject<JSN_SALE_ORDER_JUN>(mResponse);
                    if (mJSN_SALE_ORDER_JUN_get.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_ORDER_JUN_get.RES_SALE_ORDER.Count > 0)
                        {
                            //load all picker list here
                            mRES_SALE_ORDER_LST = this.mJSN_SALE_ORDER_JUN_get.RES_SALE_ORDER;
                            SaleOrderLst = this.mJSN_SALE_ORDER_JUN_get.RES_SALE_ORDER;
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ORDER_JUN_get.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ORDER_JUN_get.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ORDER_JUN_get.Message.Message);
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
            finally
            {
                Utility.closeLoader();
            }
        }
        public async Task saveSaleOrderJunOva()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_ORDER_JUN_save);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsloadSaleOrder);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_SALE_ORDER_JUN_save = JsonConvert.DeserializeObject<JSN_SALE_ORDER_JUN>(mResponse);
                    if (mJSN_SALE_ORDER_JUN_save.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_ORDER_JUN_save.RES_SALE_ORDER.Count > 0)
                        {
                            //save result
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ORDER_JUN_save.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ORDER_JUN_save.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ORDER_JUN_save.Message.Message);
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
            finally
            {
                Utility.closeLoader();
            }
        }

        #endregion



    }
}
