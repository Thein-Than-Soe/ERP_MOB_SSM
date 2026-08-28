using CS.ERP_MOB.ViewsModel.SSM;

namespace CS.ERP_MOB.Views.SSM;

public partial class FrmSsmOrderSet : ContentPage
{
    private readonly VmlSsmServiceAssign vm;
    private bool _isLoaded;
    public FrmSsmOrderSet()
    {
        try
        {
            InitializeComponent();
            vm = new VmlSsmServiceAssign();
            BindingContext = vm;
            Loaded += FrmSsmOrderSet_Loaded;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"FrmSsmStockDtl ERROR: {ex}");

            throw;
        }
    }

    private async void FrmSsmOrderSet_Loaded(object sender, EventArgs e)
    {
        if (_isLoaded)
            return;

        _isLoaded = true;

        try
        {
            await vm.loadSaleOrder();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"========== PRODUCT DETAIL LOAD ERROR ==========\n{ex}");
        }
    }


    private async void btn_addService(
        object sender,
        EventArgs e)
    {
        //await Navigation.PushAsync(new FrmSsmServiceSet(vm));
    }
    private async void btn_GSTchanged(
        object sender,
        EventArgs e)
    {
        //await Shell.Current.GoToAsync(nameof(FrmSsmStockDtl));
    }
    private async void btn_RoundOffChanged(
        object sender,
        EventArgs e)
    {
        //await Shell.Current.GoToAsync(nameof(FrmSsmStockDtl));
    }

}