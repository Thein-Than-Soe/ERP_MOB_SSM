namespace CS.ERP_MOB.Views.SSM;

using CS.ERP.PL.HMS.DAT;
using CS.ERP_MOB.ViewsModel.SSM;

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
        //assign data

        //save book now 
    }
}