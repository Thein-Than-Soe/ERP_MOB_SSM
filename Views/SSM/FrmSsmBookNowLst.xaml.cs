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
    private async void btn_Delete_Tapped( object sender,EventArgs e)
    {
        // call API to assign users
        //assign UI data
        vm.mDAT_BOOK_NOW_HEADER.StatusAsk = "6";
        await vm.saveBookNow();
        
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

    private async void Payment_Delete_Invoked(object sender, EventArgs e)
    {
        if (sender is not SwipeItem swipeItem)
            return;

        if (swipeItem.BindingContext is not RES_SALE_PAYMENT payment)
            return;

        vm.PaymentList.Remove(payment);
    }
    private async void Payment_Edit_Invoked(object sender, EventArgs e)
    {
        if (sender is not SwipeItem swipeItem)
            return;

        if (swipeItem.BindingContext is not RES_SALE_PAYMENT payment)
            return;

        // Edit selected payment
        // Put your edit logic here
        vm.InitializePaymentAmount();
        await Navigation.PushAsync(new FrmSsmPaymentSet(payment, vm));

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