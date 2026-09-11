namespace CS.ERP_MOB.Views.SSM;

using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Views.Frame;
using CS.ERP_MOB.ViewsModel.SSM;
using RGPopup.Maui.Extensions;
using System.Globalization;

public partial class FrmSsmBookNowLst : ContentView
{
    private readonly VmlSsmBookNow vm;
    private bool _isFromFrontDesk;
    private bool _isFromSchedulerBlankCell;
    private string FrontDeskAsk;
    private DateTime BlankCellDate;

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
    public FrmSsmBookNowLst(string arg)
    {
        try
        {
            InitializeComponent();

            vm = new VmlSsmBookNow();

            if (arg.StartsWith("DATE:", StringComparison.OrdinalIgnoreCase))
            {
                // Opened from blank scheduler cell
                _isFromSchedulerBlankCell = true;

                string dateString = arg.Substring("DATE:".Length);
                BlankCellDate = Utility.getDateTime(dateString);
            }
            else
            {
                // Opened from Front Desk
                _isFromFrontDesk = true;

                FrontDeskAsk = arg;
            }

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
                
                await vm.getBookNow(FrontDeskAsk);
            }
            else if(_isFromSchedulerBlankCell)
            {
                // Normal new booking with date from schedule
                await vm.loadBookNow();
                vm.OrderDate = BlankCellDate.Date;
                vm.OrderTime = BlankCellDate.TimeOfDay;
            }
            else
            {
                // Normal new booking
                await Task.Delay(300);
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





    //dis rate
    private void EntDisRate_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry)
            return;

        string text = e.NewTextValue ?? "";

        // Allow empty while user is editing
        if (string.IsNullOrEmpty(text))
            return;

        // Allow "." temporarily
        if (text == ".")
            return;

        // Only digits and decimal point
        if (!decimal.TryParse(
                text,
                System.Globalization.NumberStyles.AllowDecimalPoint,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal value))
        {
            RestorePreviousText(entry, e.OldTextValue);
            return;
        }

        // Maximum 2 decimal places
        int dotIndex = text.IndexOf('.');

        if (dotIndex >= 0)
        {
            int decimalPlaces = text.Length - dotIndex - 1;

            if (decimalPlaces > 2)
            {
                RestorePreviousText(entry, e.OldTextValue);
                return;
            }
        }

        // Maximum = 100
        if (value > 100)
        {
            RestorePreviousText(entry, e.OldTextValue);
            return;
        }

        // Do NOT update vm.DiscountRate here.
    }


    private void RestorePreviousText(Entry entry, string oldText)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (entry.Text != oldText)
                entry.Text = oldText;
        });
    }

    private void EntDisRate_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender is not Entry entry)
            return;

        string text = entry.Text ?? "";

        if (string.IsNullOrWhiteSpace(text) || text == ".")
        {
            vm.DiscountRate = 0;
            entry.Text = "";
            return;
        }

        if (decimal.TryParse(
            text,
            System.Globalization.NumberStyles.AllowDecimalPoint,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal value))
        {
            if (value > 100)
                value = 100;

            vm.DiscountRate = value;
        }
    }

    private async void btn_SaveBookNow_Tapped( object sender,EventArgs e)
    {
        // call API to assign users
        //assign UI data
        bool isValid = ValidateBookNow();

        if (!isValid)
            return;

        //save book now

        await vm.bindSaveBookNowData();
        await vm.saveBookNow();
        bool hasOtherPayment = false;
        if (vm.mJSN_RES_BOOK_NOW.RES_SALE_PAYMENT != null)
            {
                foreach (var payment in vm.mJSN_RES_BOOK_NOW.RES_SALE_PAYMENT)
                {
                    switch (payment.PaymentTypeAsk)
                    {
                        case "1": // Cash
                        case "2": // Cheque
                                  // Nothing to show
                            break;

                        case "15": // HitPay
                            vm.HitPayUrl = payment.HitPayURL ?? "";

                            if (!string.IsNullOrWhiteSpace(vm.HitPayUrl))
                            {
                                await Clipboard.Default.SetTextAsync(vm.HitPayUrl);

                                await Application.Current.MainPage.DisplayAlert(
                                    "HitPay URL",
                                    $"{vm.HitPayUrl}\n\nThe URL has been copied to your clipboard.",
                                    "OK");
                            }
                            break;

                        default:
                            // Card / other payment types
                            hasOtherPayment = true;
                            break;
                    }
                }
            }

            // Show subscription payment only once
            //if (hasOtherPayment)
            //{
            //    await Navigation.PushPopupAsync(
            //        new FrmSubscriptionPayment());
            //}

        }
    private async void btn_Delete_Tapped(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.FrontDeskAsk))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Delete Order",
                "This is a new order, can't delete.",
                "OK");

            return;
        }

        // Existing order → perform delete
        
        vm.mDAT_BOOK_NOW_HEADER.Ask = FrontDeskAsk;
        vm.mDAT_BOOK_NOW_HEADER.StatusAsk = "6";
        bool deleteSuccess = await vm.saveBookNow_delete();

        if (deleteSuccess) //show new form
        {
            if (!Common.bindMenu("ssm-book-now-set"))
            {
                Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "24", Text = "Book", MenuUrl = "ssm-book-now-set", logoImg = "" };
                MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", ApplicationMessage.Message.MenuAccessRight);
            }
            Common.routeMenu(Common.mCommon.SelectedMenu);
        
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


        return true;
    }


    private async void AddPayment_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new FrmSsmPaymentSet(vm));
        }
        catch (Exception ex)
        {
            await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error going to add new page", ex.Message, "OK");
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