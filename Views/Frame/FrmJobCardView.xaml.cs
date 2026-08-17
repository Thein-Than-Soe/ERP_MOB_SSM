using System.Windows.Input;

namespace CS.ERP_MOB.Views.Frame
{
    public partial class FrmJobCardView : ContentView
    {
	    public FrmJobCardView()
	    {
		    InitializeComponent();
        }

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(FrmJobCardView));

        public static readonly BindableProperty BodyLabel1Property =
            BindableProperty.Create(nameof(BodyLabel1), typeof(string), typeof(FrmJobCardView));

        public static readonly BindableProperty BodyLabel2Property =
            BindableProperty.Create(nameof(BodyLabel2), typeof(string), typeof(FrmJobCardView));

        public static readonly BindableProperty BodyLabel3Property =
            BindableProperty.Create(nameof(BodyLabel3), typeof(string), typeof(FrmJobCardView));

        public static readonly BindableProperty BodyLabel4Property =
            BindableProperty.Create(nameof(BodyLabel4), typeof(string), typeof(FrmJobCardView));

        public static readonly BindableProperty BodyLabel5Property =
            BindableProperty.Create(nameof(BodyLabel5), typeof(string), typeof(FrmJobCardView));


        public static readonly BindableProperty StatusProperty =
            BindableProperty.Create(nameof(Status), typeof(string), typeof(FrmJobCardView));

        public static readonly BindableProperty IsSavedProperty =
            BindableProperty.Create(nameof(IsSaved), typeof(Color), typeof(FrmJobCardView));

        public static readonly BindableProperty SaveCommandProperty =
            BindableProperty.Create(nameof(SaveCommand), typeof(ICommand), typeof(FrmJobCardView));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string BodyLabel1
        {
            get => (string)GetValue(BodyLabel1Property);
            set => SetValue(BodyLabel1Property, value);
        }

        public string BodyLabel2
        {
            get => (string)GetValue(BodyLabel2Property);
            set => SetValue(BodyLabel2Property, value);
        }

        public string BodyLabel3
        {
            get => (string)GetValue(BodyLabel3Property);
            set => SetValue(BodyLabel3Property, value);
        }

        public string BodyLabel4
        {
            get => (string)GetValue(BodyLabel4Property);
            set => SetValue(BodyLabel4Property, value);
        }
        public string BodyLabel5
        {
            get => (string)GetValue(BodyLabel5Property);
            set => SetValue(BodyLabel5Property, value);
        }
        public string Status
        {
            get => (string)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public Color IsSaved
        {
            get => (Color)GetValue(IsSavedProperty);
            set => SetValue(IsSavedProperty, value);
        }

        public ICommand SaveCommand
        {
            get => (ICommand)GetValue(SaveCommandProperty);
            set => SetValue(SaveCommandProperty, value);
        }
        //public static readonly BindableProperty SaveCommandProperty =
        //    BindableProperty.Create(
        //        nameof(SaveCommand),
        //        typeof(ICommand),
        //        typeof(FrmFrmJobCardView));

        //public ICommand SaveCommand
        //{
        //    get => (ICommand)GetValue(SaveCommandProperty);
        //    set => SetValue(SaveCommandProperty, value);
        //}
    }
}