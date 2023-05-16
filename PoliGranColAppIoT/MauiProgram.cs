using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using PoliGranColAppIoT.Services;
using PoliGranColAppIoT.Services.IServices;
using PoliGranColAppIoT.Views;

namespace PoliGranColAppIoT;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
            .UseMauiApp<App>()
			.UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<NewUpdatePage>();
        builder.Services.AddSingleton<LoginPage>();

        ConfigureServices(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

    private static void ConfigureServices(IServiceCollection services)
    {
        // Agrega tus servicios al contenedor
        services.AddSingleton<IAccountService,AccountService>();
        services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
    }
}
