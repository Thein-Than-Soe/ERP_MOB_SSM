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

    private async void AddToList_Clicked(object sender, EventArgs e)
    {
        try
        {
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

            payment.SalePersonAsk = vm.AssignedUser.Ask;

            payment.StatusAsk = vm.StatusAsk;


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

                    payment.ChequeNo =
                        vm.TransactionNo;

                    payment.CreditCardNo =
                        "";

                    payment.BankAsk =
                        vm.SelectedToBank.Ask;

                    break;


                // =================================================
                // DEBIT CARD
                // =================================================

                case "8":

                    payment.CreditCardNo =
                        "";

                    payment.BankAsk =
                        vm.SelectedToBank.Ask;

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
    #endregion



}