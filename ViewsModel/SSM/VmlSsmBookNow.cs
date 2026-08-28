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

            Quantity = 1;


        }
        #endregion

        #region "Boolen declare"
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
        RES_SALE_PAYMENT mRES_SALE_PAYMENT = new RES_SALE_PAYMENT();
        DAT_BOOK_NOW_HEADER mDAT_BOOK_NOW_HEADER = new DAT_BOOK_NOW_HEADER();
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

                // Update payment fields according to selected type
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

                // Assign barcode data to display properties
                SelectedPrice = mSelectedStockBarcode?.WholesalePrice?.ToString() ?? "0";
                ItemUOM = mSelectedStockBarcode?.UOMName_0_255 ?? "";

                UserSelectedCurrency = CurrencyList.FirstOrDefault(x => x.Ask == mSelectedStockBarcode.CurrencyAsk);
                SelectedCurrency = UserSelectedCurrency?.CurrencyDescription_0_500 ?? "";

                NotifyPropertyChanged(nameof(SelectedCurrency));

                CalculateEndTimeFromQuantity();
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

        public List<RES_USER_LST> mAvailableUserList = new List<RES_USER_LST>();
        public List<RES_USER_LST> AvailableUserList
        {
            get { return mAvailableUserList; }
            set
            {
                mAvailableUserList = value;
                NotifyPropertyChanged("AvailableUserList");
            }
        }
        public RES_USER_LST mAssignedUser = new RES_USER_LST();
        public RES_USER_LST AssignedUser
        {
            get { return mAssignedUser; }
            set
            {
                mAssignedUser = value;
                NotifyPropertyChanged("AssignedUser");
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
            get
            {
                return mUserSelectedCurrency;
            }
            set
            {
                mUserSelectedCurrency = value;
                NotifyPropertyChanged("UserSelectedCurrency");
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
                CalculateSubtotal();
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
                CalculateDiscount();
            }
        }
        private decimal _discountAmount;
        public decimal DiscountAmount
        {
            get => _discountAmount;
            set
            {
                if (_discountAmount != value)
                {
                    _discountAmount = value;
                    NotifyPropertyChanged(nameof(DiscountAmount));
                    NotifyPropertyChanged("HasDiscount");
                    CalculateGrandTotal();
                }
            }
        }
        private decimal _taxAmount;
        public decimal TaxAmount
        {
            get => _taxAmount;
            set
            {
                if (_taxAmount != value)
                {
                    _taxAmount = value;
                    NotifyPropertyChanged(nameof(TaxAmount));
                }
            }
        }
        private decimal _grandTotal;
        public decimal GrandTotal
        {
            get => _grandTotal;
            set
            {
                if (_grandTotal != value)
                {
                    _grandTotal = value;
                    NotifyPropertyChanged(nameof(GrandTotal));
                }
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


        private string mConverterQuantity;

        public string ConverterQuantity
        {
            get => mConverterQuantity;
            set
            {
                if (mConverterQuantity == value)
                    return;

                mConverterQuantity = value;
                NotifyPropertyChanged(nameof(ConverterQuantity));
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


        #endregion

        #region "Amount calculate methods"
        private string GetUOMName(RES_UOM uom)
        {
            return uom?.UOMName_0_255?.Trim().ToLower() ?? "";
        }
        private string GetItemUOM()
        {
            return SelectedStockBarcode?.UOMName_0_255?
                .Trim()
                .ToLower() ?? "";
        }
        private decimal ConvertQuantityToItemUOM()
        {
            if (SelectedStockBarcode == null || UserSelectedUOM == null)
                return Quantity;

            string itemUOM = GetItemUOM();
            string selectedUOM = GetUOMName(UserSelectedUOM);

            if (string.IsNullOrEmpty(itemUOM) ||
                string.IsNullOrEmpty(selectedUOM))
                return Quantity;

            // Same UOM
            if (IsHour(itemUOM) && IsHour(selectedUOM))
                return Quantity;

            if (IsDay(itemUOM) && IsDay(selectedUOM))
                return Quantity;

            // User selects Day, item price is per Hour
            if (IsHour(itemUOM) && IsDay(selectedUOM))
                return Quantity * 24;

            // User selects Hour, item price is per Day
            if (IsDay(itemUOM) && IsHour(selectedUOM))
                return Quantity / 24m;

            // Same unit or unsupported conversion
            return Quantity;
        }
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

            string uom = GetUOMName(UserSelectedUOM);

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

            string uom = GetUOMName(UserSelectedUOM);

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
                SelectedStockBarcode.WholesalePrice?.ToString(),
                out decimal price))
            {
                price = 0;
            }

            decimal convertedQuantity = ConvertQuantityToItemUOM();
            ConverterQuantity = convertedQuantity.ToString("0.##");
            Subtotal = price * convertedQuantity;
            CalculateDiscount();

            CalculateTax();

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

        private void CalculateDiscount()
        {
            // Reset first
            DiscountAmount = 0;

            if (DiscountRules == null || DiscountRules.Count == 0)
                return;

            if (Subtotal <= 0)
                return;

            // FIND ALL RULES THAT MATCH CURRENT SUBTOTAL
            var matchedRules = DiscountRules
                .Where(rule => IsDiscountRulesMatched(rule, Subtotal))
                .OrderByDescending(rule =>
                    ParseDecimal(rule.DiscountCalculationAmount))
                .ToList();


            // No matching rule
            if (matchedRules.Count == 0)
                return;

            SelectedRule = matchedRules.First();

            decimal rate = ParseDecimal(SelectedRule.Rate);


            // DiscountTypeAsk:
            //
            // 1 = Percentage
            // 2 = Fixed amount
            // 3 = Coupon
            //
            // Based on your sample:
            //
            // R4 -> 1 -> %
            // R5 -> 2 -> $
            // R6 -> 3 -> Coupon
            //

            switch (SelectedRule.DiscountTypeAsk)
            {
                // Percentage
                case "1":
                    DiscountAmount = Subtotal * rate / 100m;
                    break;

                // Fixed amount
                case "2":
                    DiscountAmount = rate;
                    break;


                // Coupon
                case "3":
                    // Your API has "Cu" as the type.
                    // If this coupon is already a fixed amount:
                    DiscountAmount = rate;
                    break;


                default:

                    DiscountAmount = 0;

                    break;
            }

            // Never allow discount greater than subtotal
            if (DiscountAmount > Subtotal)
            {
                DiscountAmount = Subtotal;
            }

            NotifyPropertyChanged("DiscountAmount");
            NotifyPropertyChanged("HasDiscount");
        }
        //private bool IsDiscountRulesMatched(DAT_DISCOUNT_RULE rule, decimal subtotal)
        //{
        //    decimal conditionAmount = ParseDecimal(rule.DiscountCalculationAmount);


        //    switch (rule.DiscountConditionTypeName_0_255?.Trim())
        //    {
        //        case ">=":
        //            return subtotal >= conditionAmount;

        //        case ">":
        //            return subtotal > conditionAmount;

        //        case "=":
        //        case "==":
        //            return subtotal == conditionAmount;

        //        case "<=":
        //            return subtotal <= conditionAmount;

        //        case "<":
        //            return subtotal < conditionAmount;

        //        default:
        //            return false;
        //    }
        //}
        private bool IsDiscountRulesMatched( DAT_DISCOUNT_RULE rule,decimal subtotal)
        {
            // ---------------------------------------------------------
            // 1. Check subtotal condition
            // ---------------------------------------------------------

            decimal conditionAmount =
                ParseDecimal(rule.DiscountCalculationAmount);

            bool amountMatched;

            switch (rule.DiscountConditionTypeName_0_255?.Trim())
            {
                case ">=":
                    amountMatched = subtotal >= conditionAmount;
                    break;

                case ">":
                    amountMatched = subtotal > conditionAmount;
                    break;

                case "=":
                case "==":
                    amountMatched = subtotal == conditionAmount;
                    break;

                case "<=":
                    amountMatched = subtotal <= conditionAmount;
                    break;

                case "<":
                    amountMatched = subtotal < conditionAmount;
                    break;

                default:
                    amountMatched = false;
                    break;
            }

            // Amount does not match
            if (!amountMatched)
                return false;


            // ---------------------------------------------------------
            // 2. Check discount start/end date
            // ---------------------------------------------------------

            if (OrderDate == default)
                return false;


            // Parse SD
            if (!DateTime.TryParse(
                rule.SD,
                null,
                DateTimeStyles.RoundtripKind,
                out DateTime startDate))
            {
                return false;
            }


            // Parse ED
            if (!DateTime.TryParse(
                rule.ED,
                null,
                DateTimeStyles.RoundtripKind,
                out DateTime endDate))
            {
                return false;
            }


            // ---------------------------------------------------------
            // Convert everything to the same timezone
            // ---------------------------------------------------------

            DateTime orderDate = OrderDate;

            if (startDate.Kind == DateTimeKind.Utc)
                startDate = startDate.ToLocalTime();

            if (endDate.Kind == DateTimeKind.Utc)
                endDate = endDate.ToLocalTime();


            // Remove time if SD/ED are intended as whole dates
            startDate = startDate.Date;
            endDate = endDate.Date;
            orderDate = orderDate.Date;


            // ---------------------------------------------------------
            // OrderDate must be between SD and ED
            // ---------------------------------------------------------

            if (orderDate < startDate || orderDate > endDate)
                return false;


            return true;
        }
        private void CalculateTax()
        {
            TaxAmount = 0;

            if (TaxInformation == null)
            {
                return;
            }

            // Get tax rate
            var taxInfo = TaxInformation;

            if (taxInfo == null)
            {
                return;
            }

            decimal taxRate = ParseDecimal(taxInfo.GSTRate);

            if (taxRate <= 0)
            {
                return;
            }

            decimal taxableAmount = Subtotal - DiscountAmount;


            if (taxableAmount <= 0)
            {
                TaxAmount = 0;
                return;
            }

            TaxAmount = taxableAmount * taxRate / 100m;
            CalculateGrandTotal();
        }
        private void CalculateGrandTotal()
        {
            decimal total = Subtotal - DiscountAmount + TaxAmount;

            if (total < 0)
            {
                total = 0;
            }

            GrandTotal = total;
        }

        #endregion

        #region "Method"

        //Stock
        private void UpdateSelectedStockBarcode()
        {
            if (SelectedStock == null)
            {
                SelectedStockBarcode = null;
                return;
            }

            SelectedStockBarcode = StockBarcodeList.FirstOrDefault(x => x.StockAsk == SelectedStock.Ask);
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

                string transactionDate = mRES_SALE_PAYMENT.TransactionDate;
                DateTime Date = Utility.getDateTime(transactionDate);
                TransactionDate = Date.Date;
                TransactionTime = Date.TimeOfDay;

                //service
                SelectedStock = StockList .FirstOrDefault(x => x.Ask == mDAT_BOOK_NOW_DETAIL.StockAsk);
                UpdateSelectedStockBarcode();
                ConverterQuantity = mDAT_BOOK_NOW_DETAIL.QTY;
                ItemUOM = mDAT_BOOK_NOW_DETAIL.UOMName_0_255;

                UserSelectedUOM = UomList.FirstOrDefault(x => x.Ask == mDAT_BOOK_NOW_DETAIL.UOMAsk);
                SelectedCurrency = mDAT_BOOK_NOW_HEADER.CurrencyDescription_0_500;
                SelectedPrice = mDAT_BOOK_NOW_DETAIL.Price;

                Subtotal = Utility.getGrandTotalDecimal( mDAT_BOOK_NOW_DETAIL.TotalAmount);
                DiscountAmount = Utility.getGrandTotalDecimal( mDAT_BOOK_NOW_HEADER.DiscountAmount);
                TaxAmount = Utility.getGrandTotalDecimal( mDAT_BOOK_NOW_HEADER.GSTAmount);
                GrandTotal = Utility.getGrandTotalDecimal( mDAT_BOOK_NOW_HEADER.GrandTotal);


                SelectedPaymentType = PaymentTypeList.FirstOrDefault(x => x.Ask == mRES_SALE_PAYMENT.PaymentTypeAsk);
                SelectedFromBank = FromBankList.FirstOrDefault(x => x.Ask == mRES_SALE_PAYMENT.BankAsk);
                SelectedToBank = ToBankList.FirstOrDefault(x => x.Ask == mRES_SALE_PAYMENT.BankAsk);
                Tender = mRES_SALE_PAYMENT.Tender;
                Change = mRES_SALE_PAYMENT.Change;

                await getAvailableUser();
                //assign
                //AssignedUser = AvailableUserList.FirstOrDefault(x => x.Ask == mDAT_SERVICE_ASSIGN.PickupByAsk);
                AssignedUser = AvailableUserList.FirstOrDefault(x => x.Ask == mDAT_SERVICE_ASSIGN.PickupByAsk);

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
                mDAT_BOOK_NOW_HEADER.ContactAsk = SelectedCustomerContact.Ask;

                mDAT_BOOK_NOW_HEADER.DiscountTypeAsk = SelectedRule.Ask;
                mDAT_BOOK_NOW_HEADER.DiscountAmount = this.DiscountAmount.ToString();

                mDAT_BOOK_NOW_HEADER.GSTAsk = TaxInformation.Ask;
                mDAT_BOOK_NOW_HEADER.GSTAmount = this.TaxAmount.ToString();
                mDAT_BOOK_NOW_HEADER.OutstandingAmount = this.GrandTotal.ToString();
                mDAT_BOOK_NOW_HEADER.GrandTotal = this.GrandTotal.ToString();
                mDAT_BOOK_NOW_HEADER.Subtotal = this.Subtotal.ToString();
                mDAT_BOOK_NOW_HEADER.SalePersonAsk = SelectedRule.Ask;
                mDAT_BOOK_NOW_HEADER.CurrencyAsk = UserSelectedCurrency.Ask;



                //service detail
                mDAT_BOOK_NOW_DETAIL.StockAsk = this.SelectedStock.Ask;
                DateTime SD = StartDate.Date + StartTime;
                mDAT_BOOK_NOW_DETAIL.SD = SD.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                DateTime ED = EndDate.Date + EndTime;
                mDAT_BOOK_NOW_DETAIL.ED = ED.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                mDAT_BOOK_NOW_DETAIL.Price = this.SelectedPrice;
                mDAT_BOOK_NOW_DETAIL.QTY = this.ConverterQuantity;
                mDAT_BOOK_NOW_DETAIL.UOMAsk = this.UserSelectedUOM.Ask;
                

                mDAT_BOOK_NOW_DETAIL.TotalAmount = this.Subtotal.ToString();
                mDAT_BOOK_NOW_DETAIL.TotalCost = this.Subtotal.ToString();
                mDAT_BOOK_NOW_DETAIL.CurrencyAsk = UserSelectedCurrency.Ask;




                //assign
                mDAT_SERVICE_ASSIGN.PickupByAsk = this.AssignedUser.Ask;
                mDAT_SERVICE_ASSIGN.ServiceStatusAsk = this.StatusAsk;
                mDAT_SERVICE_ASSIGN.ServiceContactAsk = this.SelectedCustomerContact.Ask;
                mDAT_SERVICE_ASSIGN.ServiceSD = mDAT_BOOK_NOW_DETAIL.SD;
                mDAT_SERVICE_ASSIGN.ServiceED = mDAT_BOOK_NOW_DETAIL.ED;




                //payment
                mRES_SALE_PAYMENT.PaymentDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                mRES_SALE_PAYMENT.CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk;
                mRES_SALE_PAYMENT.CustomerAsk = _selectedCustomer.Ask;
                mRES_SALE_PAYMENT.ContactAsk = SelectedCustomerContact.Ask;
                mRES_SALE_PAYMENT.CurrencyAsk = UserSelectedCurrency.Ask;
                mRES_SALE_PAYMENT.GSTAsk = TaxInformation.Ask;
                mRES_SALE_PAYMENT.SalePersonAsk = AssignedUser.Ask;
                mRES_SALE_PAYMENT.StatusAsk = this.StatusAsk;

                //according to pay type
                DateTime TranDate = TransactionDate.Date + TransactionTime;
                String TD = TranDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                
                mRES_SALE_PAYMENT.Subtotal = this.GrandTotal.ToString();
                mRES_SALE_PAYMENT.GrandTotal = this.GrandTotal.ToString();
                mRES_SALE_PAYMENT.DepositAmount = this.GrandTotal.ToString();
                mRES_SALE_PAYMENT.OutstandingAmount = this.GrandTotal.ToString();

                

                switch (SelectedPaymentType.Ask)
                {
                    case "1":// CASH
                        mRES_SALE_PAYMENT.Tender = this.Tender;
                        mRES_SALE_PAYMENT.Change = this.Change;

                        break;

                    case "2":// CHEQUE

                        mRES_SALE_PAYMENT.ChequeDate = TD;
                        mRES_SALE_PAYMENT.ChequeNo = this.TransactionNo;

                        break;


                    // =========================
                    // CREDIT CARD
                    // =========================
                    case "3":

                        mRES_SALE_PAYMENT.ChequeNo = this.TransactionNo;
                        //show card fill box
                        mRES_SALE_PAYMENT.CreditCardNo = "";
                        mRES_SALE_PAYMENT.BankAsk = SelectedToBank.Ask;

                        break;


                    // =========================
                    // DEBIT CARD
                    // =========================
                    case "8":
                        //show card fill box

                        mRES_SALE_PAYMENT.CreditCardNo = "";
                        mRES_SALE_PAYMENT.BankAsk = SelectedToBank.Ask;
                        break;


                    // =========================
                    
                    // =========================
                    case "15":// HIT PAY

                        //call hitpay api and assign url show
                        mRES_SALE_PAYMENT.HitPayURL = "";
                        break;


                    // =========================
                    // DEFAULT
                    // =========================
                    default:

                        break;
                }

                

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

                            DiscountRules = mJSN_RES_LOAD_BOOK_NOW.DAT_DISCOUNT_RULE;
                            TaxInformation = mJSN_RES_LOAD_BOOK_NOW.RES_GST[0];

                            StockList = this.mJSN_RES_LOAD_BOOK_NOW.RES_STOCK;
                            StockBarcodeList = this.mJSN_RES_LOAD_BOOK_NOW.RES_STOCK_BARCODE;
                            //assign only time
                            UomList = this.mJSN_RES_LOAD_BOOK_NOW.RES_UOM.Where(x => x.UOMTypeAsk == "4").ToList();
                            CurrencyList = mJSN_RES_LOAD_BOOK_NOW.RES_CURRENCY;


                            //bindDataTab(this.mJSN_LOAD_SALE_PAYMENT.RES_SALE_PAYMENT);
                            Utility.closeLoader();
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_LOAD_BOOK_NOW.Message.Message);
                        }
                        else
                        {
                            Utility.closeLoader();
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_LOAD_BOOK_NOW.Message.Message);
                        }
                    }
                    else
                    {
                        Utility.closeLoader();
                        WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("DAT.ErrWebService"));
                    }
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
                        mRES_SALE_PAYMENT = mJSN_RES_BOOK_NOW_get.RES_SALE_PAYMENT[0];

                        //disable UI according to status
                        StatusAsk = mDAT_BOOK_NOW_HEADER.StatusAsk;

                        bindGetBookNowData();

                        Utility.closeLoader();
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_BOOK_NOW_get.Message.Message);
                    }
                    else
                    {
                        Utility.closeLoader();
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_BOOK_NOW_get.Message.Message);
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
                await bindSaveBookNowData();
                mJSN_REQ_BOOK_NOW.DAT_BOOK_NOW_HEADER = mDAT_BOOK_NOW_HEADER;
                mJSN_REQ_BOOK_NOW.DAT_BOOK_NOW_DETAIL = new List<DAT_BOOK_NOW_DETAIL> { mDAT_BOOK_NOW_DETAIL };
                mJSN_REQ_BOOK_NOW.DAT_SERVICE_ASSIGN = new List<DAT_SERVICE_ASSIGN> { mDAT_SERVICE_ASSIGN };
                mJSN_REQ_BOOK_NOW.RES_SALE_PAYMENT = new List<RES_SALE_PAYMENT> { mRES_SALE_PAYMENT };

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_BOOK_NOW);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wssaveBookNow);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_BOOK_NOW = JsonConvert.DeserializeObject<JSN_RES_BOOK_NOW>(mResponse);
                    if (mJSN_RES_BOOK_NOW.Message.Code == "7")
                    {
                        //mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK = new DAT_FRONT_DESK();
                        //await getFrontDeskUser();

                        Utility.closeLoader();
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_BOOK_NOW.Message.Message);
                    }
                    else
                    {
                        Utility.closeLoader();
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_BOOK_NOW.Message.Message);
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

        #endregion



    }
}
