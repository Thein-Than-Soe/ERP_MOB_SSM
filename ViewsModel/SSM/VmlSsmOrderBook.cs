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
    public class VmlSsmOrderBook : BaseViewModel
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
        public VmlSsmOrderBook()
        {
            SelectedStock = new RES_STOCK();
            loadSaleOrder();
        }
        #endregion

        #region "Display View"

        private int _quantity = 1;

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 1)
                    value = 1;

                _quantity = value;

                NotifyPropertyChanged(nameof(Quantity));
                NotifyPropertyChanged(nameof(TotalPrice));
            }
        }

        public string TotalPrice
        {
            get
            {
                decimal unitPrice = 0;

                
                    decimal.TryParse(
                        SelectedStock?.StockRetailPrice,
                        out unitPrice);
                

                decimal total = unitPrice * Quantity;

                return total.ToString();
            }
        }

        #endregion

        #region "Amount calculate methods"


        #endregion

        #region "Data Tab"
        public JSN_LOAD_SALE_ORDER JSN_LOAD_SALE_ORDER = new JSN_LOAD_SALE_ORDER();
        public JSN_LOAD_SALE_ORDER OrderLoad
        {
            get { return JSN_LOAD_SALE_ORDER; }
            set { JSN_LOAD_SALE_ORDER = value; NotifyPropertyChanged("OrderLoad"); }
        }


        public List<RES_STOCK> mStockList = new List<RES_STOCK>();
        public List<RES_STOCK> StockList
        {
            get { return mStockList; }
            set { mStockList = value; NotifyPropertyChanged("StockList"); }
        }

        public ObservableCollection<RES_STOCK> mSelectedStockList = new ObservableCollection<RES_STOCK>();
        public ObservableCollection<RES_STOCK> SelectedStockList
        {
            get { return mSelectedStockList; }
            set { mSelectedStockList = value; NotifyPropertyChanged("SelectedStockList"); }
        }

        public RES_STOCK mSelectedStock = new RES_STOCK();
        public RES_STOCK SelectedStock
        {
            get { return mSelectedStock; }
            set { mSelectedStock = value; NotifyPropertyChanged("SelectedStock"); }
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


        #endregion

        #region "Commands"
        #endregion

        #region "Method"
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
                            SelectedStock = this.mJSN_LOAD_SALE_ORDER.RES_STOCK[0]; //edit later
                            SelectedStockList = new ObservableCollection<RES_STOCK>
                                                {
                                                    this.mJSN_LOAD_SALE_ORDER.RES_STOCK[0],
                                                    this.mJSN_LOAD_SALE_ORDER.RES_STOCK[1]
                                                }; //edit later
                            SelectedCustomer = this.mJSN_LOAD_SALE_ORDER.RES_CUSTOMER_DTL[0]; //edit later

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
