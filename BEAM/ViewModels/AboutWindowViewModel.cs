using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels;

public partial class AboutWindowViewModel : ViewModelBase
{
    public record LicenseInformation(string Name, string Licence);

    public ObservableCollection<LicenseInformation> Licenses { get; set; } = [];

    [ObservableProperty] public partial string Version { get; set; }
    [ObservableProperty] public partial string Copyright { get; set; }

    public AboutWindowViewModel()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var info = FileVersionInfo.GetVersionInfo(assembly.Location);

        Version = info.ProductVersion!;
        Copyright = info.LegalCopyright!;

        var d = AssetLoader.GetAssets(new Uri("avares://BEAM/Assets/Licenses/"), null);

        foreach (var path in d)
        {
            var name = path.ToString().Split('/').Last();
            var licenseStream = AssetLoader.Open(path);
            var reader = new StreamReader(licenseStream);
            var license = reader.ReadToEnd();

            Licenses.Add(new LicenseInformation(name, license));

            reader.Close();
            licenseStream.Close();
        }
    }
}