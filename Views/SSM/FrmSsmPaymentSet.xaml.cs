namespace CS.ERP_MOB.Views.SSM;

using CS.ERP.PL.ECO.DAT;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.SSM;

public partial class FrmSsmPaymentSet : ContentPage
{
    private readonly VmlSsmBookNow vm;
    private bool _isLoaded;
    private RES_SALE_PAYMENT CurrentItem;

    public FrmSsmPaymentSet()
    {
        try
        {
            InitializeComponent();

            vm = new VmlSsmBookNow();
            BindingContext = vm;
            LoadData();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine( $"FrmSsmPaymentSet ERROR: {ex}");

            throw;
        }
    }
    public FrmSsmPaymentSet(RES_SALE_PAYMENT mRES_SALE_PAYMENT, VmlSsmBookNow vm)
    {
        try
        {
            InitializeComponent();
            this.vm = vm;
            BindingContext = vm;
            CurrentItem = mRES_SALE_PAYMENT ?? new RES_SALE_PAYMENT();
            LoadData();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FrmSsmPaymentSet ERROR: {ex}");
            throw;
        }
    }

    #region "Private method"
    private void LoadData()
    {
        // Set values into UI fields using x:Name
        vm.InitializePaymentAmount();

    }

    private bool ValidateData()
    {
        if (vm.SelectedPaymentType == null)
        {
            Application.Current.MainPage.DisplayAlert(
                "Required",
                "Please select a payment type.",
                "OK");

            return false;
        }

        switch (vm.SelectedPaymentType.Ask)
        {
            case "1": // CASH

                if (!decimal.TryParse(vm.Tender, out decimal tenderAmount))
                {
                    Application.Current.MainPage.DisplayAlert(
                        "Required",
                        "Please enter a valid tender amount.",
                        "OK");

                    return false;
                }

                if (tenderAmount < vm.GrandTotal)
                {
                    Application.Current.MainPage.DisplayAlert(
                        "Invalid Tender",
                        "Tender amount cannot be less than the Grand Total.",
                        "OK");

                    return false;
                }

                break;

            case "2": // CHEQUE

                if (string.IsNullOrWhiteSpace(vm.TransactionNo))
                {
                    Application.Current.MainPage.DisplayAlert(
                        "Required",
                        "Please enter the cheque number.",
                        "OK");

                    return false;
                }

                break;

            case "3": // CREDIT CARD
                if (vm.SelectedToBank == null)
                {
                    Application.Current.MainPage.DisplayAlert(
                        "Required",
                        "Please select the To Bank.",
                        "OK");

                    return false;
                }
                break;

            case "8": // DEBIT CARD

                if (vm.SelectedToBank == null)
                {
                    Application.Current.MainPage.DisplayAlert(
                        "Required",
                        "Please select the To Bank.",
                        "OK");

                    return false;
                }

                break;
        }
        return true;

    }

    private async void AddToList_Clicked(object sender, EventArgs e)
    {
        try
        {
            bool isValid = ValidateData();

            if (!isValid)
                return;

            // Create a new payment record
            RES_SALE_PAYMENT payment = new RES_SALE_PAYMENT();


            // =====================================================
            // PAYMENT - COMMON DATA
            // =====================================================

            payment.PaymentTypeAsk = vm.SelectedPaymentType.Ask;
            payment.PaymentTypeName_0_255 = vm.SelectedPaymentType.PaymentTypeName_0_255;

            payment.PaymentDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            payment.CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk;

            payment.CustomerAsk = vm.SelectedCustomer.Ask;

            payment.ContactAsk = vm.SelectedCustomerContact.Ask;

            payment.CurrencyAsk = vm.UserSelectedCurrency.Ask;

            payment.GSTAsk = vm.TaxInformation.Ask;

            payment.SalePersonAsk = Common.mCommon.User.UserAsk;



            // =====================================================
            // TRANSACTION DATE
            // =====================================================

            DateTime TranDate =
                vm.TransactionDate.Date +
                vm.TransactionTime;

            string TD = TranDate
                    .ToUniversalTime()
                    .ToString("yyyy-MM-ddTHH:mm:ss.fffZ");


            payment.TransactionDate = TD;


            // =====================================================
            // TARGETED DATE
            // Transaction Date + 1 Month
            // =====================================================

            DateTime TargetedDate =TranDate.AddMonths(1);

            string TargetedDateString =TargetedDate
                    .ToUniversalTime()
                    .ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            payment.TargetedDate = TargetedDateString;


            // =====================================================
            // PAYMENT AMOUNT
            // =====================================================

            payment.Subtotal = vm.GrandTotal.ToString();
            payment.GrandTotal = vm.GrandTotal.ToString();
            payment.DepositAmount = vm.DepositAmount;
            payment.OutstandingAmount = vm.RemainingAmount;


            // =====================================================
            // PAYMENT TYPE SPECIFIC DATA
            // =====================================================

            switch (vm.SelectedPaymentType.Ask)
            {
                case "1": // CASH

                    payment.Tender =
                        vm.Tender;

                    payment.Change =
                        vm.Change;

                    break;


                case "2": // CHEQUE

                    payment.ChequeDate =
                        TD;

                    payment.ChequeNo =
                        vm.TransactionNo;

                    break;


                // =================================================
                // CREDIT CARD
                // =================================================

                case "3":


                    payment.CreditCardNo = vm.TransactionNo;

                    payment.BankAsk =
                        vm.SelectedToBank.Ask;

                    break;


                // =================================================
                // DEBIT CARD
                // =================================================

                case "8":

                    payment.CreditCardNo = vm.TransactionNo;

                    payment.BankAsk = vm.SelectedToBank.Ask;

                    break;


                // =================================================
                // HIT PAY
                // =================================================

                case "15":

                    // HitPay API will be handled during Save

                    break;


                // =================================================
                // DEFAULT
                // =================================================

                default:

                    break;
            }


            // =====================================================
            // ADD PAYMENT TO VIEWMODEL LIST
            // =====================================================

            vm.PaymentList.Add(payment);

            // =====================================================
            // SUCCESS
            // =====================================================

            await Application.Current.MainPage.DisplayAlert(
                "Payment",
                "Successfully added the payment.",
                "OK");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                ex.ToString(),
                "OK");
        }
    }
    
    private async void Delete_Clicked(object sender, EventArgs e)
    {
        try
        {
            bool isValid = ValidateData();

            if (!isValid)
                return;

            // =====================================================
            // ADD PAYMENT TO VIEWMODEL LIST
            // =====================================================

            vm.PaymentList.Remove(CurrentItem);

            // =====================================================
            // SUCCESS
            // =====================================================

            await Application.Current.MainPage.DisplayAlert(
                "Payment",
                "Successfully remove the payment.",
                "OK");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                ex.ToString(),
                "OK");
        }
    }
    
    
    
    #endregion



}