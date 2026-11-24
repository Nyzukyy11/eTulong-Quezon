using Avalonia;using Avalonia.Controls.ApplicationLifetimes;using Avalonia.Markup.Xaml;using Microsoft.Extensions.DependencyInjection;using System.Text.Json;
namespace ETulongDesktop;
public partial class App:Application {
 ServiceProvider? services;
 public override void Initialize()=>AvaloniaXamlLoader.Load(this);
 public override void OnFrameworkInitializationCompleted(){
  if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop){var s=new ServiceCollection();string? configError=null;var address=Environment.GetEnvironmentVariable("ETULONG_API");if(string.IsNullOrWhiteSpace(address)){try{var config=JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"appsettings.json")));address=config?.ApiBaseAddress;}catch(Exception ex)when(ex is IOException or JsonException){configError="Could not read appsettings.json. Restore the configuration and restart.";}}
  if(!Uri.TryCreate(address,UriKind.Absolute,out var uri)||uri.Scheme is not("http" or "https")){configError="The API address is invalid. Set ApiBaseAddress in appsettings.json and restart.";uri=new Uri("http://127.0.0.1:5291/");}
  s.AddSingleton(new HttpClient{BaseAddress=uri,Timeout=TimeSpan.FromSeconds(15)});s.AddSingleton<DeskClient>();s.AddSingleton<MainVm>();services=s.BuildServiceProvider();var vm=services.GetRequiredService<MainVm>();desktop.MainWindow=new MainWindow{DataContext=vm};if(configError is not null)vm.Message=configError;else desktop.MainWindow.Opened+=async(_,_)=>await vm.RefreshCommand.ExecuteAsync(null);desktop.Exit+=(_,_)=>services.Dispose();}
  base.OnFrameworkInitializationCompleted();}
 record Settings(string ApiBaseAddress);
}
