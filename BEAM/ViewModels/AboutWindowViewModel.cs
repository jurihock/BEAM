using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Platform;

namespace BEAM.ViewModels;

public class AboutWindowViewModel : ViewModelBase
{
    public record LicenseInformation(string Name, string Licence);

    public ObservableCollection<LicenseInformation> Licenses { get; set; } = [];

    public AboutWindowViewModel()
    {
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