using System.Collections.ObjectModel;

namespace CS.ERP_MOB.Views.SSM;

public partial class FrmSsmOrderBookSet : ContentPage
{
    public FrmSsmOrderBookSet()
    {
        InitializeComponent();
        LoadDummyData();

        BindingContext = this;
    }

    public ObservableCollection<DummyCustomer> Customers { get; set; }
    public ObservableCollection<DummyContact> CustomerContacts { get; set; }
    public ObservableCollection<DummyStock> Stocks { get; set; }
    public ObservableCollection<DummyItem> InquiryItems { get; set; }

    public DummyCustomer SelectedCustomer { get; set; }
    public DummyContact SelectedCustomerContact { get; set; }
    public DummyStock SelectedStock { get; set; }



    private void LoadDummyData()
    {
        // ==============================
        // Customers
        // ==============================

        Customers = new ObservableCollection<DummyCustomer>
        {
            new DummyCustomer
            {
                CustomerName = "ABC Trading Co., Ltd."
            },

            new DummyCustomer
            {
                CustomerName = "Myanmar Star Company"
            },

            new DummyCustomer
            {
                CustomerName = "Golden Business Group"
            }
        };


        // ==============================
        // Customer Contacts
        // ==============================

        CustomerContacts = new ObservableCollection<DummyContact>
        {
            new DummyContact
            {
                ContactName = "Mg Mg - 09 123456789"
            },

            new DummyContact
            {
                ContactName = "Aung Aung - 09 987654321"
            },

            new DummyContact
            {
                ContactName = "Su Su - 09 555555555"
            }
        };


        // ==============================
        // Stock List
        // ==============================

        Stocks = new ObservableCollection<DummyStock>
        {
            new DummyStock
            {
                StockName = "Samsung Galaxy A55"
            },

            new DummyStock
            {
                StockName = "iPhone 15"
            },

            new DummyStock
            {
                StockName = "USB-C Cable"
            },

            new DummyStock
            {
                StockName = "Wireless Mouse"
            }
        };


        // ==============================
        // Already selected items
        // ==============================

        InquiryItems = new ObservableCollection<DummyItem>
        {
            new DummyItem
            {
                StockName_0_255 = "Samsung Galaxy A55",
                StockNumber_0_50 = "SAM-A55-256",
                QTY = "2",
                UOMName_0_255 = "PCS",
                Price = "650000",
                Discount = "10000",
                TotalAmount = "1290000"
            },

            new DummyItem
            {
                StockName_0_255 = "iPhone 15",
                StockNumber_0_50 = "APP-IP15-128",
                QTY = "1",
                UOMName_0_255 = "PCS",
                Price = "1800000",
                Discount = "50000",
                TotalAmount = "1750000"
            },

            new DummyItem
            {
                StockName_0_255 = "USB-C Cable",
                StockNumber_0_50 = "CAB-USBC-01",
                QTY = "5",
                UOMName_0_255 = "PCS",
                Price = "10000",
                Discount = "0",
                TotalAmount = "50000"
            }
        };
    }

    public class DummyCustomer
    {
        public string CustomerName { get; set; }
    }


    public class DummyContact
    {
        public string ContactName { get; set; }
    }


    public class DummyStock
    {
        public string StockName { get; set; }
    }


    public class DummyItem
    {
        public string StockName_0_255 { get; set; }
        public string StockNumber_0_50 { get; set; }
        public string QTY { get; set; }
        public string UOMName_0_255 { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public string TotalAmount { get; set; }
    }
}