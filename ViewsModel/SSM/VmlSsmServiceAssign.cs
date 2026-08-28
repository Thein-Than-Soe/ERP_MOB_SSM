using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.POS;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SSM
{
    public class VmlSsmServiceAssign : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_SALE_LOAD mJSN_REQ_SALE_LOAD = new JSN_REQ_SALE_LOAD();
        public JSN_LOAD_SALE_BROWSE mJSN_LOAD_SALE_BROWSE = new JSN_LOAD_SALE_BROWSE();

        //loadSaleOrder
        public JSN_LOAD_SALE_ORDER mJSN_LOAD_SALE_ORDER = new JSN_LOAD_SALE_ORDER();

        //saveSaleOrderJunOva
        public JSN_REQ_SALE_ORDER_JUN mJSN_REQ_SALE_ORDER_JUN_save = new JSN_REQ_SALE_ORDER_JUN();
        public JSN_SALE_ORDER_JUN mJSN_SALE_ORDER_JUN_save   = new JSN_SALE_ORDER_JUN();

        //getSaleOrderJun
        public JSN_SALE_ORDER_JUN mJSN_SALE_ORDER_JUN_get = new JSN_SALE_ORDER_JUN();
        public JSN_REQ_SALE_ORDER_JUN mJSN_REQ_SALE_ORDER_JUN_get = new JSN_REQ_SALE_ORDER_JUN();   


        string mRequest = "";
        string mResponse = "";
        #endregion

        #region "Contructor"
        public VmlSsmServiceAssign()
        {
            SelectedStock = new RES_STOCK();
            loadSaleOrder();

            DateTime now = DateTime.Now;

            StartDate = now.Date;
            StartTime = now.TimeOfDay;

            EndDate = now.Date;
            EndTime = now.TimeOfDay;

            Quantity = 1;
        }
        #endregion

        #region "Display View"

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

                CalculateEndTime();
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
            }
        }
        public string SelectedPrice
        {
            get
            {
                return SelectedStockBarcode?.WholesalePrice?.ToString() ?? "0";
            }
        }

        public string SelectedUOM
        {
            get
            {
                return SelectedStockBarcode?.UOMName_0_255 ?? "";
            }
        }

        public string SelectedCurrency
        {
            get
            {
                return SelectedStockBarcode?.CurrencyDescription_0_500 ?? "";
            }
        }


        //For start time, end time
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

                CalculateEndTime();
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

                CalculateEndTime();
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
            }
        }
        #endregion

        #region "Amount calculate methods"
        private void CalculateEndTime()
        {
            if (SelectedStockBarcode == null)
                return;

            DateTime startDateTime = StartDate.Date + StartTime;

            string uom = SelectedStockBarcode.UOMName_0_255?.Trim().ToLower();

            DateTime endDateTime;

            switch (uom)
            {
                case "hour":
                case "hours":
                    endDateTime = startDateTime.AddHours((double)Quantity);
                    break;

                case "minute":
                case "minutes":
                    endDateTime = startDateTime.AddMinutes((double)Quantity);
                    break;

                case "day":
                case "days":
                    endDateTime = startDateTime.AddDays((double)Quantity);
                    break;

                default:
                    // If UOM isn't time-based,
                    // don't automatically change EndTime.
                    return;
            }

            EndDate = endDateTime.Date;
            EndTime = endDateTime.TimeOfDay;
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

            Subtotal = price * Quantity;
        }
        #endregion

        #region "Data Tab"
        public JSN_LOAD_SALE_ORDER JSN_LOAD_SALE_ORDER = new JSN_LOAD_SALE_ORDER();
        public JSN_LOAD_SALE_ORDER OrderLoad
        {
            get { return JSN_LOAD_SALE_ORDER; }
            set { JSN_LOAD_SALE_ORDER = value; NotifyPropertyChanged("OrderLoad"); }
        }



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
                NotifyPropertyChanged(nameof(SelectedPrice));
                NotifyPropertyChanged(nameof(SelectedUOM));
                NotifyPropertyChanged(nameof(SelectedCurrency));

                CalculateEndTime();
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
                UpdateSelectedStockBarcode();
            }
        }

        public List<RES_CUSTOMER_DTL> mCustomerList = new List<RES_CUSTOMER_DTL>();
        public List<RES_CUSTOMER_DTL> CustomerList
        {
            get { return mCustomerList; }
            set
            {
                mCustomerList = value;
                NotifyPropertyChanged("CustomerList");
                NotifyPropertyChanged("CustomerContactList");
            }
        }

        public RES_CUSTOMER_DTL mSelectedCustomer = new RES_CUSTOMER_DTL();
        public RES_CUSTOMER_DTL SelectedCustomer
        {
            get { return mSelectedCustomer; }
            set {
                mSelectedCustomer = value; 
                NotifyPropertyChanged("SelectedCustomer"); 
                NotifyPropertyChanged("CustomerContactList"); }
        }
        

        public List<RES_CUSTOMER_CONTACT> mCustomerContactList = new List<RES_CUSTOMER_CONTACT>();
        public List<RES_CUSTOMER_CONTACT> CustomerContactList
        {
            get { return mCustomerContactList; }
            set { mCustomerContactList = value; NotifyPropertyChanged("CustomerContactList"); }
        }

        public List<RES_USER_LST> mAvailableUserList = new List<RES_USER_LST>();
        public List<RES_USER_LST> AvailableUserList
        {
            get { return mAvailableUserList; }
            set {
                mAvailableUserList = value; 
                NotifyPropertyChanged("AvailableUserList"); 
            }
        }
        public RES_USER_LST mAssignedUser = new RES_USER_LST();
        public RES_USER_LST AssignedUser
        {
            get { return mAssignedUser; }
            set {
                mAssignedUser = value; 
                NotifyPropertyChanged("AssignedUser"); 
            }
        }
        #endregion

        #region "Commands"
        #endregion

        #region "Method"
        private void UpdateSelectedStockBarcode()
        {
            if (SelectedStock == null)
            {
                SelectedStockBarcode = null;
                return;
            }

            SelectedStockBarcode = StockBarcodeList
                .FirstOrDefault(x => x.StockAsk == SelectedStock.Ask);
        }


        #endregion

        #region "Web Service Api"
        public async Task loadSaleOrder()
        {
            try
            {
                REQ_AUTHORIZATION mREQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mRequest = JsonConvert.SerializeObject(mREQ_AUTHORIZATION);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsloadSaleOrder);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_LOAD_SALE_ORDER = JsonConvert.DeserializeObject<JSN_LOAD_SALE_ORDER>(mResponse);
                    if (mJSN_LOAD_SALE_ORDER.Message.Code == "7")
                    {
                        if (this.mJSN_LOAD_SALE_ORDER.RES_STOCK.Count > 0)
                        {
                            CustomerList = this.mJSN_LOAD_SALE_ORDER.RES_CUSTOMER_DTL;
                            StockList = this.mJSN_LOAD_SALE_ORDER.RES_STOCK;  
                            StockBarcodeList = this.mJSN_LOAD_SALE_ORDER.RES_STOCK_BARCODE;  
                            

                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.LoadSuccess);
                        }
                        else
                        {
                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.NoData);
                        }
                    }
                    else
                    {
                        MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, this.mJSN_LOAD_SALE_ORDER.Message.Message);
                    }
                }
                else
                {
                    MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.WebServiceErr);
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        #endregion



    }
}
