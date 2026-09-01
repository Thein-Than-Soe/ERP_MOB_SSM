namespace CS.ERP_MOB.Views.SSM;

using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP_MOB.Views.Frame;
using CS.ERP_MOB.ViewsModel.SSM;
using RGPopup.Maui.Extensions;

public partial class FrmSsmBookNowLst : ContentView
{
    private readonly VmlSsmBookNow vm;
    private bool _isFromFrontDesk;
    private string FrontDeskAsk;

    private bool _isLoaded;

    public FrmSsmBookNowLst()
    {
        try
        {
            InitializeComponent();
            _isFromFrontDesk = false;

            vm = new VmlSsmBookNow();

            BindingContext = vm;

            Loaded += FrmSsmBookNowLst_Loaded;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"FrmSsmBookNowLst ERROR: {ex}");

            throw;
        }
    }

    // Constructor when coming from Front Desk
    public FrmSsmBookNowLst(String argDAT_FRONT_DESK_Ask)
    {
        try
        {
            InitializeComponent();

            _isFromFrontDesk = true;

            vm = new VmlSsmBookNow();

            FrontDeskAsk = argDAT_FRONT_DESK_Ask;

            BindingContext = vm;

            Loaded += FrmSsmBookNowLst_Loaded;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"FrmSsmBookNowLst ERROR: {ex}");

            throw;
        }
    }

    // =========================================================
    // Loaded
    // =========================================================
    private async void FrmSsmBookNowLst_Loaded(object sender, EventArgs e)
    {
        if (_isLoaded)
            return;

        _isLoaded = true;

        try
        {
            if (_isFromFrontDesk)
            {
                // Existing booking from Front Desk
                await vm.loadBookNow();
                await vm.getBookNow(FrontDeskAsk);
            }
            else
            {
                // Normal new booking
                await vm.loadBookNow();
            }

            //ServiceButton.IsVisible = !vm.IsServiceAdded;
            //EmptyItemLabel.IsVisible = !vm.IsServiceAdded;
            //ServiceCard.IsVisible = vm.IsServiceAdded;
            //InvoiceSection.IsVisible = vm.IsServiceAdded;
            //ServiceSection.IsVisible = vm.IsCustomerSelected;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"========== BOOK NOW LOAD ERROR ==========\n{ex}");
        }
    }
    private async void btn_addService(object sender, EventArgs e)
    {
        if (!vm.IsServiceEditable)
            return;

        await Navigation.PushAsync(
            new FrmSsmServiceSet(vm));

        vm.IsServiceAdded = true;

        ServiceButton.IsVisible = !vm.IsServiceAdded;
        EmptyItemLabel.IsVisible = !vm.IsServiceAdded;
        ServiceCard.IsVisible = vm.IsServiceAdded;
        InvoiceSection.IsVisible = vm.IsServiceAdded;
        ServiceSection.IsVisible = vm.IsCustomerSelected;
    }

    private async void ServiceCard_Tapped(
        object sender,
        EventArgs e)
    {
        if (!vm.IsServiceEditable)
            return;

        await Navigation.PushAsync(
            new FrmSsmServiceSet(vm));

        vm.IsServiceAdded = true;

        ServiceButton.IsVisible = !vm.IsServiceAdded;
        EmptyItemLabel.IsVisible = !vm.IsServiceAdded;
        ServiceCard.IsVisible = vm.IsServiceAdded;
        InvoiceSection.IsVisible = vm.IsServiceAdded;
        ServiceSection.IsVisible = vm.IsCustomerSelected;
    }

    private async void btn_SaveBookNow_Tapped( object sender,EventArgs e)
    {
        // call API to assign users
        //assign UI data
        bool isValid = ValidateBookNow();

        if (!isValid)
            return;

        //save book now
        await vm.saveBookNow();
        if (vm.SelectedPaymentType.Ask == "15")
            {
                await Clipboard.Default.SetTextAsync(vm.HitPayUrl);

                await Application.Current.MainPage.DisplayAlert(
                    "HitPay URL",
                    $"{vm.HitPayUrl}\n\nThe URL has been copied to your clipboard.",
                    "OK");
            }
            else
            {
                await Navigation.PushPopupAsync(new FrmSubscriptionPayment());
            }
        
    }

    private bool ValidateBookNow()
    {
        if (vm.SelectedCustomer == null)
        {
            Application.Current.MainPage.DisplayAlert(
                "Required",
                "Please select a customer.",
                "OK");

            return false;
        }

        if (vm.SelectedCustomerContact == null)
        {
            Application.Current.MainPage.DisplayAlert(
                "Required",
                "Please select a customer contact.",
                "OK");

            return false;
        }

        if (vm.SelectedStock == null)
        {
            Application.Current.MainPage.DisplayAlert(
                "Required",
                "Please select a service.",
                "OK");

            return false;
        }

        if (vm.UserSelectedUOM == null)
        {
            Application.Current.MainPage.DisplayAlert(
                "Required",
                "Please select a UOM in service.",
                "OK");

            return false;
        }

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


    private async void AddPayment_Clicked(object sender, EventArgs e)
    {
        try
        {
            vm.InitializePaymentAmount();

            await Navigation.PushAsync(new FrmSsmPaymentSet(new RES_SALE_PAYMENT(), vm));
        }
        catch (Exception ex)
        {
            await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error going to experience add new page", ex.Message, "OK");
        }
    }
    private async void payment_lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            //if (!Utility.checkButtonAccess("Edit"))
            //{

            var selectedItem = e.SelectedItem as RES_SALE_PAYMENT;
            if (selectedItem != null)
            {
                vm.InitializePaymentAmount();
                await Navigation.PushAsync(new FrmSsmPaymentSet(selectedItem, vm));
            }
            else { return; }
                ((ListView)sender).SelectedItem = null;
            //}
            //else
            //{
            //    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
            //}
        }
        catch (Exception ex)
        {
            throw ex.InnerException;
        }
    }

    private bool _isPaymentDropdownOpen = false;

    private void OnPaymentDropdownToggleTapped(object sender, EventArgs e)
    {
        _isPaymentDropdownOpen = !_isPaymentDropdownOpen;

        Payment_DropdownContent.IsVisible = _isPaymentDropdownOpen;

        Payment_DropdownToggleIcon.Text =
            _isPaymentDropdownOpen
                ? "\uf077"   // chevron-up
                : "\uf078";  // chevron-down
    }

}