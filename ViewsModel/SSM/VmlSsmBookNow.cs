using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.HMS.REQ;
using CS.ERP.PL.HMS.RES;
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
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SSM
{
    public class VmlSsmBookNow : BaseViewModel
    {
        #region "Declaring"

        //loadBookNow
        public JSN_RES_LOAD_BOOK_NOW mJSN_RES_LOAD_BOOK_NOW = new JSN_RES_LOAD_BOOK_NOW();
        //saveBookNow
        public JSN_REQ_BOOK_NOW mJSN_REQ_BOOK_NOW = new JSN_REQ_BOOK_NOW();
        public JSN_RES_BOOK_NOW mJSN_RES_BOOK_NOW = new JSN_RES_BOOK_NOW();
        //getAvailableUser
        public JSN_REQ_AVAILABLE_USER mJSN_REQ_AVAILABLE_USER = new JSN_REQ_AVAILABLE_USER();
        public JSN_RES_AVAILABLE_USER mJSN_RES_AVAILABLE_USER = new JSN_RES_AVAILABLE_USER();
        //getBookNow
        public JSN_REQ_BOOK_NOW mJSN_REQ_BOOK_NOW_get = new JSN_REQ_BOOK_NOW();
        public JSN_RES_BOOK_NOW mJSN_RES_BOOK_NOW_get = new JSN_RES_BOOK_NOW();
        public DAT_FRONT_DESK mDAT_FRONT_DESK = new DAT_FRONT_DESK();
        //data list


        string mRequest = "";
        string mResponse = "";
        #endregion

        #region "Contructor"
        public VmlSsmBookNow()
        {
            DateTime now = DateTime.Now;

            StartDate = now.Date;
            StartTime = now.TimeOfDay;

            StartDate = now.Date;
            StartTime = now.TimeOfDay;

            EndDate = now.Date;
            EndTime = now.TimeOfDay;

            RecurringDate = now.Date;
            RecurringTime = now.TimeOfDay;


            Quantity = 1;
            SelectedGSTMethod = GSTMethodList.FirstOrDefault(x => x.Ask == "E");

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

        #region "Boolen declare"

        private bool mIsPaymentListEmpty = true;
        public bool IsPaymentListEmpty
        {
            get => mIsPaymentListEmpty;
            set
            {
                if (mIsPaymentListEmpty != value)
                {
                    NotifyPropertyChanged(nameof(IsPaymentListEmpty));
                }
            }
        }

        public bool HasDiscount => DiscountAmount > 0;
        public bool HasGST =>TaxInformation != null &&!string.IsNullOrWhiteSpace(TaxInformation.GSTRate) && TaxInformation.GSTRate != "0";


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
        private string mStatusAsk = "1";

        public string StatusAsk
        {
            get => mStatusAsk;
            set
            {
                if (mStatusAsk == value)
                    return;

                mStatusAsk = value;

                NotifyPropertyChanged(nameof(StatusAsk));
                NotifyPropertyChanged(nameof(IsServiceEditable));
                NotifyPropertyChanged(nameof(IsPaymentEditable));
            }
        }


        // Open only
        public bool IsServiceEditable
        {
            get
            {
                return StatusAsk == "1";
            }
        }


        // Assign -> Check Out
        public bool IsPaymentEditable
        {
            get
            {
                return StatusAsk == "1"
                    || StatusAsk == "2"
                    || StatusAsk == "3"
                    || StatusAsk == "4"
                    || StatusAsk == "5"
                    || StatusAsk == "6";
            }
        }

        #endregion

        #region "Data Tab"
        //data tab for get and save data
        DAT_BOOK_NOW_DETAIL mDAT_BOOK_NOW_DETAIL = new DAT_BOOK_NOW_DETAIL();
        List<RES_SALE_PAYMENT> mRES_SALE_PAYMENT = new List<RES_SALE_PAYMENT>();
        public DAT_BOOK_NOW_HEADER mDAT_BOOK_NOW_HEADER = new DAT_BOOK_NOW_HEADER();
        DAT_SERVICE_ASSIGN mDAT_SERVICE_ASSIGN = new DAT_SERVICE_ASSIGN();



        //Payment
        public List<RES_BANK> mToBankList = new List<RES_BANK>();
        public List<RES_BANK> ToBankList
        {
            get { return mToBankList; }
            set { mToBankList = value; NotifyPropertyChanged("ToBankList"); }
        }

        public List<RES_COMPANY_BANK> mFromBankList = new List<RES_COMPANY_BANK>();
        public List<RES_COMPANY_BANK> FromBankList
        {
            get { return mFromBankList; }
            set { mFromBankList = value; NotifyPropertyChanged("FromBankList"); }
        }
        private RES_COMPANY_BANK mSelectedFromBank;

        public RES_COMPANY_BANK SelectedFromBank
        {
            get => mSelectedFromBank;
            set
            {
                if (mSelectedFromBank == value)
                    return;

                mSelectedFromBank = value;
                NotifyPropertyChanged(nameof(SelectedFromBank));
            }
        }


        private RES_BANK mSelectedToBank;

        public RES_BANK SelectedToBank
        {
            get => mSelectedToBank;
            set
            {
                if (mSelectedToBank == value)
                    return;

                mSelectedToBank = value;
                NotifyPropertyChanged(nameof(SelectedToBank));
            }
        }



        public List<RES_PAYMENT_TYPE> mPaymentTypeList = new List<RES_PAYMENT_TYPE>();
        public List<RES_PAYMENT_TYPE> PaymentTypeList      
        {
            get { return mPaymentTypeList; }
            set { mPaymentTypeList = value; NotifyPropertyChanged("PaymentTypeList"); }
        }

        //user added list
        private ObservableCollection<RES_SALE_PAYMENT> mPaymentList =
    new ObservableCollection<RES_SALE_PAYMENT>();

        public ObservableCollection<RES_SALE_PAYMENT> PaymentList
        {
            get => mPaymentList;
            set
            {
                if (mPaymentList == value)
                    return;

                mPaymentList = value ?? new ObservableCollection<RES_SALE_PAYMENT>();

                NotifyPropertyChanged(nameof(PaymentList));

                IsPaymentListEmpty = mPaymentList.Count == 0;
                NotifyPropertyChanged(nameof(IsPaymentListEmpty));

                CalculateRemainingAmount();
            }
        }

        private RES_PAYMENT_TYPE mSelectedPaymentType;

        public RES_PAYMENT_TYPE SelectedPaymentType
        {
            get => mSelectedPaymentType;
            set
            {
                if (mSelectedPaymentType == value)
                    return;

                mSelectedPaymentType = value;

                NotifyPropertyChanged(nameof(SelectedPaymentType));

                UpdatePaymentFields();
            }
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

                IsServiceAdded = mSelectedStock != null;
                IsServiceNotAdded = mSelectedStock == null;

                NotifyPropertyChanged(nameof(IsServiceAdded));
                NotifyPropertyChanged(nameof(IsServiceNotAdded));
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

                UserSelectedUOM = UomList?.FirstOrDefault( x => x.StockAttAsk == mSelectedStock.UOMAsk);

                UserSelectedCurrency = CurrencyList?.FirstOrDefault( x => x.Ask == mSelectedStock.CurrencyAsk);

                UpdateSelectedStockBarcode();
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

        public List<RES_USER_LST> mAvailableUserList;
        public List<RES_USER_LST> AvailableUserList
        {
            get { return mAvailableUserList; }
            set
            {
                mAvailableUserList = value;
                NotifyPropertyChanged("AvailableUserList");
            }
        }
        public RES_USER_LST mAssignedUser;
        public RES_USER_LST AssignedUser
        {
            get { return mAssignedUser; }
            set
            {
                mAssignedUser = value;
                NotifyPropertyChanged("AssignedUser");
                HasAssignedUser = AssignedUser != null;
                NotifyPropertyChanged(nameof(HasAssignedUser));
            }
        }

        //Discount
        private List<RES_DISCOUNT_TYPE> mDiscountTypeList;
        public List<RES_DISCOUNT_TYPE> DiscountTypeList
        {
            get
            {
                return mDiscountTypeList;
            }
            set
            {
                mDiscountTypeList = value;
                NotifyPropertyChanged("DiscountTypeList");
            }
        }
        private RES_DISCOUNT_TYPE mSelectedDiscountType;
        public RES_DISCOUNT_TYPE SelectedDiscountType
        {
            get
            {
                return mSelectedDiscountType;
            }
            set
            {
                mSelectedDiscountType = value;
                NotifyPropertyChanged("SelectedDiscountType");
                if (!_isLoadingDefaultDiscountRule)
                    CalculateDiscountAmount();
            }
        }
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
        private DAT_DISCOUNT_RULE mSelectedDiscountRule;
        public DAT_DISCOUNT_RULE SelectedDiscountRule
        {
            get
            {
                return mSelectedDiscountRule;
            }
            set
            {
                mSelectedDiscountRule = value;
                NotifyPropertyChanged("SelectedDiscountRule");
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
                SelectedCurrency = value?.CurrencyDescription_0_500 ?? string.Empty;

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

        #region "Commands"
        #endregion

        #region "Display View"

        //Payment method display
        private string _hitPayUrl;

        public string HitPayUrl
        {
            get => _hitPayUrl;
            set
            {
                if (_hitPayUrl != value)
                {
                    _hitPayUrl = value;
                    NotifyPropertyChanged(nameof(HitPayUrl));
                }
            }
        }


        private bool mHasAssignedUser = false;

        public bool HasAssignedUser
        {
            get => mHasAssignedUser;
            set
            {
                if (mHasAssignedUser == value)
                    return;

                mHasAssignedUser = value;
                NotifyPropertyChanged(nameof(HasAssignedUser));
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
                CalculateDiscountAmount(true);
                CalculateGrandTotal();
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
        
        private decimal mDiscountRate;

        public decimal DiscountRate
        {
            get => mDiscountRate;
            set
            {
                value = Math.Round(value, 2);
                if (mDiscountRate == value)
                    return;

                mDiscountRate = value;

                NotifyPropertyChanged(nameof(DiscountRate));

                // User changed rate.
                // Only recalculate using current Type + Rate.

                if (!_isLoadingDefaultDiscountRule)
                    CalculateDiscountAmount();
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
                CalculateRemainingAmount();
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

                CalculateRemainingAmount();
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
        private bool _isLoadingDefaultDiscountRule;

        private void CalculateDiscountAmount(bool findDefaultRule = false)
        {
            try
            {
                // Initial load: find the matching discount rule
                if (findDefaultRule)
                {
                    if (DiscountTypeList == null ||
                        DiscountTypeList.Count == 0 ||
                        UserSelectedCurrency == null)
                    {
                        DiscountAmount = 0;
                        return;
                    }

                    DAT_DISCOUNT_RULE matchedRule =
                        Utility.FindMatchingDiscountRule(
                            DiscountRules,
                            "4",
                            Subtotal,
                            OrderDate,
                            UserSelectedCurrency.Ask);

                    if (matchedRule != null)
                    {
                        _isLoadingDefaultDiscountRule = true;

                        SelectedDiscountType = DiscountTypeList.FirstOrDefault(x =>
                            x.Ask == matchedRule.DiscountTypeAsk);

                        DiscountRate = decimal.TryParse(
                            matchedRule.Rate,
                            out decimal rate)
                            ? rate
                            : 0;

                        SelectedDiscountRule = matchedRule;

                        _isLoadingDefaultDiscountRule = false;
                    }
                }

                // Calculate using the CURRENT selected values
                if (SelectedDiscountType == null)
                {
                    DiscountAmount = 0;
                    return;
                }

                SelectedDiscountRule = new DAT_DISCOUNT_RULE
                {
                    DiscountTypeAsk = SelectedDiscountType.Ask,
                    DiscountTypeName_0_255 =
                        SelectedDiscountType.DiscountTypeName_0_255,
                    Rate = DiscountRate.ToString()
                };

                decimal discount = Utility.CalculateDiscount(
                    SelectedDiscountRule,
                    calculationValue: Subtotal);

                DiscountAmount = discount;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"CalculateDiscountAmount ERROR: {ex}");

                DiscountAmount = 0;
            }
        }

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
            // ROUND OFF process
            // ==========================================

            //if (IsRoundOffEnabled)
            //{
            //    decimal floorAmt = Math.Floor(totalBeforeRoundOff);
            //    decimal pointValue = Math.Round(totalBeforeRoundOff - floorAmt, 2);

            //    decimal baseCent = 0.05m;

            //    decimal remainder = pointValue % baseCent;

            //    if (remainder == 0)
            //    {
            //        RoundOffAmount = 0;
            //        GrandTotal = totalBeforeRoundOff;
            //    }
            //    else
            //    {
            //        if (pointValue < baseCent)
            //        {
            //            RoundOffAmount = pointValue;
            //        }
            //        else if (pointValue > baseCent && pointValue < 0.1m)
            //        {
            //            RoundOffAmount = pointValue - baseCent;
            //        }
            //        else if (pointValue > 0.1m)
            //        {
            //            int lastDigitOfPoint =
            //                (int)(pointValue * 100) % 10;

            //            if (lastDigitOfPoint > 5)
            //            {
            //                RoundOffAmount =
            //                    (lastDigitOfPoint - 5) / 100m;
            //            }
            //            else
            //            {
            //                RoundOffAmount =
            //                    lastDigitOfPoint / 100m;
            //            }
            //        }

            //        GrandTotal = totalBeforeRoundOff + RoundOffAmount;
            //    }
            //}
            //else
            //{
            //    RoundOffAmount = 0;
            //    GrandTotal = totalBeforeRoundOff;
            //}
            // ==========================================
            // ROUND OFF PROCESS
            // ==========================================

            if (IsRoundOffEnabled)
            {
                const decimal roundingUnit = 0.05m;

                // Round UP to the next 0.05
                decimal roundedTotal =
                    Math.Ceiling(totalBeforeRoundOff / roundingUnit) * roundingUnit;

                RoundOffAmount = roundedTotal - totalBeforeRoundOff;

                GrandTotal = roundedTotal;
            }
            else
            {
                RoundOffAmount = 0;
                GrandTotal = totalBeforeRoundOff;
            }
        }
        private void CalculateRemainingAmount()
        {
            //Remaining =GrandTotal- Previous Payments - Current Deposit
            decimal grandTotal = 0;
            decimal currentDeposit = 0;
            decimal previousPayments = 0;

            decimal.TryParse(GrandTotal.ToString(), out grandTotal);
            decimal.TryParse(DepositAmount, out currentDeposit);

            if (PaymentList != null)
            {
                foreach (var payment in PaymentList)
                {
                    if (decimal.TryParse(payment.DepositAmount, out decimal amount))
                    {
                        previousPayments += amount;
                    }
                }
            }

            decimal remainingAmount =
                grandTotal - previousPayments - currentDeposit;

            if (remainingAmount < 0)
                remainingAmount = 0;

            RemainingAmount = remainingAmount.ToString("0.##");
        }
        public void InitializePaymentAmount()
        {
            decimal grandTotal = 0;
            decimal previousPayments = 0;

            decimal.TryParse(GrandTotal.ToString(), out grandTotal);

            if (PaymentList != null)
            {
                foreach (var payment in PaymentList)
                {
                    if (decimal.TryParse(payment.DepositAmount, out decimal amount))
                    {
                        previousPayments += amount;
                    }
                }
            }

            decimal remainingAmount = grandTotal - previousPayments;

            if (remainingAmount < 0)
                remainingAmount = 0;

            DepositAmount = remainingAmount.ToString("0.##");

            CalculateRemainingAmount();
        }
        #endregion

        #region "Method"

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

            // First get all barcodes matching Stock + UOM + Currency
            var matchingBarcodes = StockBarcodeList
                .Where(x =>
                    x.StockAsk == SelectedStock.Ask &&
                    x.UOMAsk == UserSelectedUOM.StockAttAsk &&
                    x.CurrencyAsk == UserSelectedCurrency.Ask)
                .ToList();

            // Then find the barcode whose date range contains OrderDate
            SelectedStockBarcode = matchingBarcodes.FirstOrDefault(x =>
            {
                DateTime startDate = Utility.getDateTime(x.SD);
                DateTime endDate = Utility.getDateTime(x.ED);

                return OrderDate >= startDate.Date &&
                       OrderDate <= endDate.Date;
            });

            // add for date range limit
            if (SelectedStockBarcode == null)
            {
                SelectedPrice = "0";
                Application.Current?.MainPage?.DisplayAlert("Barcode Not Found", "There is no barcode available for the selected Stock, UOM, and Currency.", "OK");
            }
            else
            {
                SelectedPrice = SelectedStockBarcode.RetailPrice?.ToString() ?? "0";
            }

            NotifyPropertyChanged(nameof(SelectedPrice));
        }
        private void UpdatePaymentFields()
        {
            if (SelectedPaymentType == null)
            {
                TransactionNoLabel = "Transaction No";

                IsTransactionDateEditable = false;
                IsTransactionNoVisible = false;
                IsBankVisible = false;
                IsCashVisible = false;

                return;
            }


            // Default values
            IsTransactionDateEditable = false;
            IsTransactionNoVisible = false;
            IsBankVisible = false;
            IsCashVisible = false;

            TransactionNoLabel = "Transaction No";


            switch (SelectedPaymentType.Ask)
            {
                // =========================
                // CASH
                // =========================
                case "1":

                    IsCashVisible = true;

                    break;


                // =========================
                // CHEQUE
                // =========================
                case "2":

                    IsTransactionNoVisible = true;
                    IsTransactionDateEditable = true;
                    IsBankVisible = true;

                    TransactionNoLabel = "Cheque No";

                    break;


                // =========================
                // CREDIT CARD
                // =========================
                case "3":

                    IsTransactionNoVisible = true;
                    IsBankVisible = true;

                    TransactionNoLabel = "Credit Card No";

                    break;


                // =========================
                // DEBIT CARD
                // =========================
                case "8":

                    // According to your requirement:
                    // Transaction No is hidden for Debit Card
                    IsBankVisible = true;
                    break;


                // =========================
                // HIT PAY
                // =========================
                case "15":

                    // Transaction No hidden
                    // Bank hidden

                    break;


                // =========================
                // DEFAULT
                // =========================
                default:

                    IsTransactionNoVisible = true;

                    TransactionNoLabel = "Transaction No";

                    break;
            }

            NotifyPropertyChanged(nameof(IsTransactionDateEditable));
            NotifyPropertyChanged(nameof(IsTransactionNoVisible));
            NotifyPropertyChanged(nameof(IsBankVisible));
            NotifyPropertyChanged(nameof(IsCashVisible));
            NotifyPropertyChanged(nameof(TransactionNoLabel));
        }


        #endregion



        #region "Bind data"
        public async Task bindGetBookNowData()
        {
            try
            {
                //header
                SelectedCustomer = CustomerList.FirstOrDefault(x => x.Ask == mDAT_BOOK_NOW_HEADER.CustomerAsk);
                SelectedCustomerContact = CustomerContactList.FirstOrDefault(x => x.Ask == mDAT_BOOK_NOW_HEADER.ContactAsk);

                string bookNowDate = mDAT_BOOK_NOW_HEADER.BookNowDate;
                DateTime bookNowDateTime = Utility.getDateTime(bookNowDate);
                OrderDate = bookNowDateTime.Date;
                OrderTime = bookNowDateTime.TimeOfDay;

                DateTime serviceStart = Utility.getDateTime( mDAT_BOOK_NOW_DETAIL.SD);

                StartDate = serviceStart.Date;
                StartTime = serviceStart.TimeOfDay;


                // Service End Date / Time
                DateTime serviceEnd = Utility.getDateTime( mDAT_BOOK_NOW_DETAIL.ED);

                EndDate = serviceEnd.Date;
                EndTime = serviceEnd.TimeOfDay;

                //service
                SelectedStock = StockList .FirstOrDefault(x => x.Ask == mDAT_BOOK_NOW_DETAIL.StockAsk);
                Quantity = ParseDecimal(mDAT_BOOK_NOW_DETAIL.QTY);
                UserSelectedCurrency = CurrencyList.FirstOrDefault(x =>   x.Ask == mDAT_BOOK_NOW_DETAIL.CurrencyAsk);
                UserSelectedUOM = UomList.FirstOrDefault(x => x.StockAttAsk == mDAT_BOOK_NOW_DETAIL.UOMAsk);

                UpdateSelectedStockBarcode();

                ItemUOM = UserSelectedUOM.UOMName_0_255;

                Subtotal = Utility.getGrandTotalDecimal( mDAT_BOOK_NOW_DETAIL.TotalAmount);
                DiscountRate = Utility.getGrandTotalDecimal( mDAT_BOOK_NOW_HEADER.DiscountRate);
                DiscountAmount = Utility.getGrandTotalDecimal( mDAT_BOOK_NOW_HEADER.DiscountAmount);

                //gst
                TaxAmount = Utility.getGrandTotalDecimal( mDAT_BOOK_NOW_HEADER.GSTAmount);
                SelectedGSTMethod = GSTMethodList.FirstOrDefault(x => x.Ask == mDAT_BOOK_NOW_HEADER.GSTType);
                IsGSTEnabled = !string.IsNullOrWhiteSpace(mDAT_BOOK_NOW_HEADER.GSTType);

                GrandTotal = Utility.getGrandTotalDecimal( mDAT_BOOK_NOW_HEADER.GrandTotal);
                RoundOffAmount =Utility.getGrandTotalDecimal(mDAT_BOOK_NOW_HEADER.RoundOffAmount);

                IsRoundOffEnabled = decimal.TryParse(mDAT_BOOK_NOW_HEADER.RoundOffAmount, out decimal roundOffAmount) && roundOffAmount != 0;
                

                await getAvailableUser();
                //assign
                //AssignedUser = AvailableUserList.FirstOrDefault(x => x.Ask == mDAT_SERVICE_ASSIGN.PickupByAsk);
                AssignedUser = AvailableUserList.FirstOrDefault(x => x.Ask == mDAT_SERVICE_ASSIGN.PickupByAsk);
                SelectedBeatType =  BeatTypeList.FirstOrDefault(x => x.Ask == mDAT_BOOK_NOW_HEADER.BeatTypeAsk);


                PaymentList = new ObservableCollection<RES_SALE_PAYMENT>(mRES_SALE_PAYMENT);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task bindSaveBookNowData()
        {
            try
            {
                //header
                mDAT_BOOK_NOW_HEADER.CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk;
                DateTime orderDateTime = OrderDate.Date + OrderTime; 
                mDAT_BOOK_NOW_HEADER.BookNowDate = orderDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                mDAT_BOOK_NOW_HEADER.StatusAsk = this.StatusAsk;
               
                mDAT_BOOK_NOW_HEADER.CustomerAsk = _selectedCustomer.Ask;
                mDAT_BOOK_NOW_HEADER.CustomerName_0_255 = _selectedCustomer.CustomerName_0_255;
                mDAT_BOOK_NOW_HEADER.ContactAsk = SelectedCustomerContact.Ask;

                mDAT_BOOK_NOW_HEADER.DiscountTypeAsk = SelectedDiscountType.Ask;
                mDAT_BOOK_NOW_HEADER.DiscountTypeName_0_255 = SelectedDiscountType.DiscountTypeName_0_255;
                mDAT_BOOK_NOW_HEADER.DiscountRate = this.DiscountRate.ToString();
                mDAT_BOOK_NOW_HEADER.DiscountAmount = this.DiscountAmount.ToString();

                mDAT_BOOK_NOW_HEADER.GSTAsk = TaxInformation.Ask;
                mDAT_BOOK_NOW_HEADER.GSTRate = TaxInformation.GSTRate;
                mDAT_BOOK_NOW_HEADER.GSTAmount = this.TaxAmount.ToString();
                mDAT_BOOK_NOW_HEADER.GSTType = this.SelectedGSTMethod.Ask;
                mDAT_BOOK_NOW_HEADER.GSTTypeName_0_255 = this.SelectedGSTMethod.GSTMethodName;

                mDAT_BOOK_NOW_HEADER.OutstandingAmount = this.GrandTotal.ToString();
                mDAT_BOOK_NOW_HEADER.GrandTotal = this.GrandTotal.ToString();
                mDAT_BOOK_NOW_HEADER.Subtotal = this.Subtotal.ToString();
                mDAT_BOOK_NOW_HEADER.SalePersonAsk = Common.mCommon.User.UserAsk;
                mDAT_BOOK_NOW_HEADER.CurrencyAsk = UserSelectedCurrency.Ask;
                mDAT_BOOK_NOW_HEADER.CurrencyCode_0_50 = UserSelectedCurrency.CurrencyCode_0_50;

                mDAT_BOOK_NOW_HEADER.BeatTypeAsk = SelectedBeatType?.Ask ?? "";
                mDAT_BOOK_NOW_HEADER.BeatTypeName_0_255 = SelectedBeatType?.BeatTypeName_0_255 ?? "";

                DateTime RecurringDateTime = RecurringDate.Date + RecurringTime;
                mDAT_BOOK_NOW_HEADER.ReferenceDate = RecurringDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                


                DateTime SD = StartDate.Date + StartTime;
                mDAT_BOOK_NOW_HEADER.SD = SD.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                DateTime ED = EndDate.Date + EndTime;
                mDAT_BOOK_NOW_HEADER.ED = ED.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                mDAT_BOOK_NOW_HEADER.RoundOffAmount = this.RoundOffAmount.ToString();
                mDAT_BOOK_NOW_HEADER.RevenueAmount = this.RevenueAmount.ToString();



                //service detail
                mDAT_BOOK_NOW_DETAIL.StockAsk = this.SelectedStock.Ask;
                mDAT_BOOK_NOW_DETAIL.StockCode_0_50 = this.SelectedStock.StockCode_0_50;
                mDAT_BOOK_NOW_DETAIL.StockName_0_255 = this.SelectedStock.StockName_0_255;
                
                mDAT_BOOK_NOW_DETAIL.SD = SD.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                
                mDAT_BOOK_NOW_DETAIL.ED = ED.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                mDAT_BOOK_NOW_DETAIL.Price = this.Subtotal.ToString();
                mDAT_BOOK_NOW_DETAIL.QTY = this.Quantity.ToString();
                mDAT_BOOK_NOW_DETAIL.UOMAsk = this.UserSelectedUOM.StockAttAsk;
                mDAT_BOOK_NOW_DETAIL.UOMCode_0_50 = this.UserSelectedUOM.UOMCode_0_50;
                mDAT_BOOK_NOW_DETAIL.UOMName_0_255 = this.UserSelectedUOM.UOMName_0_255;
                

                mDAT_BOOK_NOW_DETAIL.TotalAmount = this.Subtotal.ToString();
                mDAT_BOOK_NOW_DETAIL.TotalCost = this.Subtotal.ToString();
                mDAT_BOOK_NOW_DETAIL.Cost = this.SelectedStockBarcode.RetailPrice;
                mDAT_BOOK_NOW_DETAIL.CurrencyAsk = UserSelectedCurrency.Ask;



                //assign
                mDAT_SERVICE_ASSIGN.CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk;
                mDAT_SERVICE_ASSIGN.CustomerAsk = _selectedCustomer.Ask;
                mDAT_SERVICE_ASSIGN.CustomerName_0_255 = _selectedCustomer.CustomerName_0_255;

                mDAT_SERVICE_ASSIGN.PickupByAsk = this.AssignedUser?.Ask ?? "";
                mDAT_SERVICE_ASSIGN.ServiceStatusAsk = this.StatusAsk;
                mDAT_SERVICE_ASSIGN.ServiceContactAsk = this.SelectedCustomerContact.Ask;
                mDAT_SERVICE_ASSIGN.ServiceSD = mDAT_BOOK_NOW_DETAIL.SD;
                mDAT_SERVICE_ASSIGN.ServiceED = mDAT_BOOK_NOW_DETAIL.ED;
                mDAT_SERVICE_ASSIGN.ServiceDate = mDAT_BOOK_NOW_HEADER.BookNowDate;

                mDAT_SERVICE_ASSIGN.GSTAmount = mDAT_BOOK_NOW_HEADER.GSTAmount;
                mDAT_SERVICE_ASSIGN.GSTAsk = mDAT_BOOK_NOW_HEADER.GSTAsk;

                mDAT_SERVICE_ASSIGN.RoundOffAmount = mDAT_BOOK_NOW_HEADER.RoundOffAmount;

                //payment
                mRES_SALE_PAYMENT = PaymentList.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region "Web Service Api"
        public async Task loadBookNow()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsloadBookNow);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_RES_LOAD_BOOK_NOW = JsonConvert.DeserializeObject<JSN_RES_LOAD_BOOK_NOW>(mResponse);
                    if (mJSN_RES_LOAD_BOOK_NOW.Message.Code == "7")
                    {
                        if (this.mJSN_RES_LOAD_BOOK_NOW.RES_BANK.Count > 0)
                        {
                            this.StatusAsk = "1";
                            ToBankList = mJSN_RES_LOAD_BOOK_NOW.RES_BANK;
                            var currentCompany = mJSN_RES_LOAD_BOOK_NOW.RES_COMPANY
                                                 .FirstOrDefault(x => x.Ask == Common.mCommon.CompanyUserData.CompanyAsk);

                            FromBankList = currentCompany?.RES_COMPANY_BANK?.ToList()
                                           ?? new List<RES_COMPANY_BANK>();

                            PaymentTypeList = mJSN_RES_LOAD_BOOK_NOW.RES_PAYMENT_TYPE;
                            CustomerList = mJSN_RES_LOAD_BOOK_NOW.RES_CUSTOMER_DTL;
                            BeatTypeList = mJSN_RES_LOAD_BOOK_NOW.DAT_BEAT_TYPE;

                            TaxInformation = mJSN_RES_LOAD_BOOK_NOW.RES_GST[0];
                            DiscountRules = mJSN_RES_LOAD_BOOK_NOW.DAT_DISCOUNT_RULE;
                            DiscountTypeList = mJSN_RES_LOAD_BOOK_NOW.RES_DISCOUNT_TYPE;

                            StockList = this.mJSN_RES_LOAD_BOOK_NOW.RES_STOCK;
                            StockBarcodeList = this.mJSN_RES_LOAD_BOOK_NOW.RES_STOCK_BARCODE;
                            //assign only time
                            UomList = this.mJSN_RES_LOAD_BOOK_NOW.RES_STOCK_UOM.Where(x => x.UOMTypeAsk == "4").ToList();
                            CurrencyList = mJSN_RES_LOAD_BOOK_NOW.RES_CURRENCY;


                            //bindDataTab(this.mJSN_LOAD_SALE_PAYMENT.RES_SALE_PAYMENT);
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_LOAD_BOOK_NOW.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_LOAD_BOOK_NOW.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("DAT.ErrWebService"));
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Utility.closeLoader();
            }
        }
        public async Task getBookNow(string argAsk)
        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_BOOK_NOW_get.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_BOOK_NOW_get.DAT_BOOK_NOW_HEADER.Ask = argAsk;

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_BOOK_NOW_get);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetBookNow);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_BOOK_NOW_get = JsonConvert.DeserializeObject<JSN_RES_BOOK_NOW>(mResponse);
                    if (mJSN_RES_BOOK_NOW_get.Message.Code == "7")
                    {
                        

                        mDAT_SERVICE_ASSIGN = mJSN_RES_BOOK_NOW_get.DAT_SERVICE_ASSIGN[0];
                        mDAT_BOOK_NOW_HEADER = mJSN_RES_BOOK_NOW_get.DAT_BOOK_NOW_HEADER[0];
                        mDAT_BOOK_NOW_DETAIL = mJSN_RES_BOOK_NOW_get.DAT_BOOK_NOW_DETAIL[0];
                        mRES_SALE_PAYMENT = mJSN_RES_BOOK_NOW_get.RES_SALE_PAYMENT;

                        

                        bindGetBookNowData();

                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_BOOK_NOW_get.Message.Message);
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_BOOK_NOW_get.Message.Message);
                    }
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("DAT.ErrWebService"));
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
        public async Task getAvailableUser()
        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_AVAILABLE_USER.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                DateTime orderDateTime = OrderDate.Date + OrderTime;

                mJSN_REQ_AVAILABLE_USER.RES_USER_LST = new RES_USER_LST
                {
                    CustomerAsk = SelectedCustomer.Ask,
                    SD = orderDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_AVAILABLE_USER);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetAvailableUser);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_AVAILABLE_USER = JsonConvert.DeserializeObject<JSN_RES_AVAILABLE_USER>(mResponse);
                    if (mJSN_RES_AVAILABLE_USER.Message.Code == "7")
                    {
                        AvailableUserList = mJSN_RES_AVAILABLE_USER.RES_USER_LST;

                        Utility.closeLoader();
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_AVAILABLE_USER.Message.Message);
                    }
                    else
                    {
                        Utility.closeLoader();
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_AVAILABLE_USER.Message.Message);
                    }
                }
                else
                {
                    Utility.closeLoader();
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("DAT.ErrWebService"));
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

        public async Task saveBookNow()
        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_BOOK_NOW.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                
                mJSN_REQ_BOOK_NOW.DAT_BOOK_NOW_HEADER = mDAT_BOOK_NOW_HEADER;
                mJSN_REQ_BOOK_NOW.DAT_BOOK_NOW_DETAIL = new List<DAT_BOOK_NOW_DETAIL> { mDAT_BOOK_NOW_DETAIL };
                mJSN_REQ_BOOK_NOW.DAT_SERVICE_ASSIGN = new List<DAT_SERVICE_ASSIGN> { mDAT_SERVICE_ASSIGN };
                mJSN_REQ_BOOK_NOW.RES_SALE_PAYMENT =  mRES_SALE_PAYMENT;

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_BOOK_NOW);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wssaveBookNow);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_BOOK_NOW = JsonConvert.DeserializeObject<JSN_RES_BOOK_NOW>(mResponse);
                    if (mJSN_RES_BOOK_NOW.Message.Code == "7")
                    {
                        //mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK = new DAT_FRONT_DESK();
                        //await getFrontDeskUser();
                        HitPayUrl = mJSN_RES_BOOK_NOW.RES_SALE_PAYMENT != null &&
                                     mJSN_RES_BOOK_NOW.RES_SALE_PAYMENT.Count > 0
                                ? mJSN_RES_BOOK_NOW.RES_SALE_PAYMENT[0].HitPayURL ?? ""
                                : "";

                        string orderNumber = mJSN_RES_BOOK_NOW.DAT_BOOK_NOW_HEADER[0]?.BookNowCode_0_50 ?? "";

                        await Application.Current.MainPage.DisplayAlert(
                            "Booking Successful",
                            $"Your booking was successful.\n\nOrder Number: {orderNumber}",
                            "OK");

                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_BOOK_NOW.Message.Message);
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_BOOK_NOW.Message.Message);
                    }
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("DAT.ErrWebService"));
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
