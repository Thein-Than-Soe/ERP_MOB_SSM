namespace CS.ERP_MOB.Views.SSM;

using CS.ERP.PL.ECO.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP_MOB.ViewsModel.SSM;

public partial class FrmSsmServiceSet : ContentPage
{
    private readonly VmlSsmBookNow vm;
    private bool _isLoaded;

    public FrmSsmServiceSet()
    {
        try
        {
            InitializeComponent();

            vm = new VmlSsmBookNow();
            BindingContext = vm;

            Loaded += FrmSsmServiceSet_Loaded;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"FrmSsmServiceSet ERROR: {ex}");

            throw;
        }
    }
    public FrmSsmServiceSet(VmlSsmBookNow vm)
    {
        try
        {
            InitializeComponent();
            this.vm = vm;
            BindingContext = vm;

            Loaded += FrmSsmServiceSet_Loaded;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine( $"FrmSsmServiceSet ERROR: {ex}");
            throw;
        }
    }
    private async void FrmSsmServiceSet_Loaded(object sender, EventArgs e)
    {
        if (_isLoaded)
            return;

        _isLoaded = true;

        try
        {
            await vm.getAvailableUser();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"========== SERVICE SET LOAD ERROR ==========\n{ex}");
        }
    }


    private void Plus_Clicked(object sender, EventArgs e)
    {
        if (vm == null)
            return;

        vm.Quantity++;

        QtyEntry.Text = vm.Quantity.ToString();
    }
    private void Minus_Clicked(object sender, EventArgs e)
    {
        if (vm == null)
            return;

        if (vm.Quantity > 1)
            vm.Quantity--;

        QtyEntry.Text = vm.Quantity.ToString();
    }

    private void QtyEntry_TextChanged(object sender, TextChangedEventArgs e)
    {

        if (vm == null)
            return;

        if (int.TryParse(e.NewTextValue, out int quantity))
        {
            if (quantity < 1)
                quantity = 1;

            if (vm.Quantity != quantity)
                vm.Quantity = quantity;
        }
    }

    private async void AddToList_Clicked(object sender, EventArgs e)
    {
       
        try
        {
            //recurring date time add

            await Application.Current.MainPage.DisplayAlert(
                "Service",
                "Successfully added the service",
                "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
        }


    }


}