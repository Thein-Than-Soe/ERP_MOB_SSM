using CommunityToolkit.Maui;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Licensing;
//using ERP_MOB_SSM;
using FreshMvvm.Maui.Extensions;
using Maui.TouchEffect.Hosting;
using Microsoft.Extensions.Logging;
using RGPopup.Maui.Extensions;
using UraniumUI;
namespace CS.ERP_MOB
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //Syncfusion
            SyncfusionLicenseProvider.RegisterLicense(
            "Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdlWXdccnVXQmJfVk1xV0RWYEk=");

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .UseMauiTouchEffect()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FontAwesome5Solid.ttf", "FontAwesomeSolid");
                    fonts.AddFont("FontAwesome5Regular.ttf", "FontAwesomeRegular");
                    fonts.AddFont("FontAwesome5Brands.ttf", "FontAwesomeBrands");
                })
                .UseMauiRGPopup(config =>
                {
                    config.BackPressHandler = null;
                    config.FixKeyboardOverlap = true;
                });

            builder.Services.Add(ServiceDescriptor.Transient<MainPage, MainPage>());
            builder.Services.Add(ServiceDescriptor.Transient<MainPageModel, MainPageModel>());

#if DEBUG
            builder.Logging.AddDebug();
#endif

            //return builder.Build();
            MauiApp mauiApp = builder.Build();

            mauiApp.UseFreshMvvm();
            return mauiApp;
        }
    }
}
