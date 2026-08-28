using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.JOB.DAT;
using System.Globalization;
namespace CS.ERP_MOB.Views.Frame;
using CS.ERP_MOB.General;
using System.Windows.Input;

public partial class FrmSsmItemCard : ContentView
{
	public FrmSsmItemCard()
	{
		InitializeComponent();
        BindingContext = this;
    }

    // =========================
    // Item Name
    // =========================

    public static readonly BindableProperty ItemNameProperty =
        BindableProperty.Create(
            nameof(ItemName),
            typeof(string),
            typeof(FrmSsmItemCard),
            string.Empty);

    public string ItemName
    {
        get => (string)GetValue(ItemNameProperty);
        set => SetValue(ItemNameProperty, value);
    }


    // =========================
    // Item Number
    // =========================

    public static readonly BindableProperty ItemNumberProperty =
        BindableProperty.Create(
            nameof(ItemNumber),
            typeof(string),
            typeof(FrmSsmItemCard),
            string.Empty);

    public string ItemNumber
    {
        get => (string)GetValue(ItemNumberProperty);
        set => SetValue(ItemNumberProperty, value);
    }


    // =========================
    // Qty
    // =========================

    public static readonly BindableProperty QtyProperty =
        BindableProperty.Create(
            nameof(Qty),
            typeof(string),
            typeof(FrmSsmItemCard),
            string.Empty,
            propertyChanged: OnCalculationPropertyChanged);

    public string Qty
    {
        get => (string)GetValue(QtyProperty);
        set => SetValue(QtyProperty, value);
    }


    // =========================
    // UOM
    // =========================

    public static readonly BindableProperty UOMProperty =
        BindableProperty.Create(
            nameof(UOM),
            typeof(string),
            typeof(FrmSsmItemCard),
            string.Empty);

    public string UOM
    {
        get => (string)GetValue(UOMProperty);
        set => SetValue(UOMProperty, value);
    }


    // =========================
    // Price
    // =========================

    public static readonly BindableProperty PriceProperty =
        BindableProperty.Create(
            nameof(Price),
            typeof(string),
            typeof(FrmSsmItemCard),
            string.Empty,
            propertyChanged: OnCalculationPropertyChanged);

    public string Price
    {
        get => (string)GetValue(PriceProperty);
        set => SetValue(PriceProperty, value);
    }


    // =========================
    // Discount
    // =========================

    public static readonly BindableProperty DiscountProperty =
        BindableProperty.Create(
            nameof(Discount),
            typeof(string),
            typeof(FrmSsmItemCard),
            string.Empty,
            propertyChanged: OnDiscountChanged);


    public string Discount
    {
        get => (string)GetValue(DiscountProperty);
        set => SetValue(DiscountProperty, value);
    }


    // =========================
    // Calculated Amount
    // =========================

    public static readonly BindableProperty AmountProperty =
        BindableProperty.Create(
            nameof(Amount),
            typeof(string),
            typeof(FrmSsmItemCard),
            "0.00");

    public string Amount
    {
        get => (string)GetValue(AmountProperty);
        set => SetValue(AmountProperty, value);
    }


    // =========================
    // Discount Visibility
    // =========================

    public static readonly BindableProperty HasDiscountProperty =
        BindableProperty.Create(
            nameof(HasDiscount),
            typeof(bool),
            typeof(FrmSsmItemCard),
            false);

    public bool HasDiscount
    {
        get => (bool)GetValue(HasDiscountProperty);
        set => SetValue(HasDiscountProperty, value);
    }


    // =========================
    // Calculation
    // =========================

    private static void OnCalculationPropertyChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        var card = (FrmSsmItemCard)bindable;

        card.CalculateAmount();
    }


    private static void OnDiscountChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        var card = (FrmSsmItemCard)bindable;

        card.UpdateDiscountVisibility();
        card.CalculateAmount();
    }


    private void CalculateAmount()
    {
        decimal price = ParseDecimal(Price);
        decimal discount = ParseDecimal(Discount);
        decimal qty = ParseDecimal(Qty);

        decimal amount = (price - discount) * qty;

        Amount = amount.ToString("0.00", CultureInfo.InvariantCulture);
    }


    private void UpdateDiscountVisibility()
    {
        decimal discount = ParseDecimal(Discount);

        HasDiscount = discount != 0;
    }


    private static decimal ParseDecimal(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        if (decimal.TryParse(
            value,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out decimal result))
        {
            return result;
        }

        return 0;
    }

    public static readonly BindableProperty TapCommandProperty =
    BindableProperty.Create(
        nameof(TapCommand),
        typeof(ICommand),
        typeof(FrmSsmItemCard));

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }


    public static readonly BindableProperty DeleteCommandProperty =
        BindableProperty.Create(
            nameof(DeleteCommand),
            typeof(ICommand),
            typeof(FrmSsmItemCard));

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }


    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(FrmSsmItemCard));

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
   
}