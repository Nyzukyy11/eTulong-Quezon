using Avalonia.Controls;using Avalonia.Platform.Storage;
namespace ETulongDesktop;
public partial class MainWindow:Window {
 public MainWindow(){InitializeComponent();Opened+=(_,_)=>{if(DataContext is MainVm vm)vm.PickDocumentAsync=async()=>{var files=await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions{Title="Choose a fictional document image",AllowMultiple=false,FileTypeFilter=[new FilePickerFileType("PNG or JPEG image"){Patterns=["*.png","*.jpg","*.jpeg"]}]});if(files.Count==0)return null;await using var stream=await files[0].OpenReadAsync();if(stream.CanSeek&&stream.Length>2*1024*1024)throw new Exception("Choose an image no larger than 2 MB.");using var bytes=new MemoryStream();var buffer=new byte[8192];int read;while((read=await stream.ReadAsync(buffer))>0){if(bytes.Length+read>2*1024*1024)throw new Exception("Choose an image no larger than 2 MB.");await bytes.WriteAsync(buffer.AsMemory(0,read));}return(files[0].Name,bytes.ToArray());};};}
}
