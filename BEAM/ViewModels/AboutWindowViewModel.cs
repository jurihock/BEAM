using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels;

/// <summary>
/// The view model controling the about window.
/// Used to gather license information using license file compiled within the application.
/// </summary>
public partial class AboutWindowViewModel : ViewModelBase
{
    /// <summary>
    /// A simple record for storing the name of a software and its license text.
    /// </summary>
    /// <param name="Name">The name of the used software</param>
    /// <param name="Licence">The license text</param>
    public record LicenseInformation(string Name, string Licence);

    /// <summary>
    /// The licenses BEAM uses.
    /// </summary>
    public ObservableCollection<LicenseInformation> Licenses { get; set; } = [];

    /// <summary>
    /// The current version of BEAM.
    /// Set in BEAM.csproj.
    /// </summary>
    [ObservableProperty] public partial string Version { get; set; }

    /// <summary>
    /// The copyright string of BEAM.
    /// Set in BEAM.csproj.
    /// </summary>
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

            // load text from license file
            var licenseStream = AssetLoader.Open(path);
            var reader = new StreamReader(licenseStream);
            var license = reader.ReadToEnd();

            // add license information
            Licenses.Add(new LicenseInformation(name, license));

            // cleanup for the current license file
            reader.Close();
            licenseStream.Close();
        }
    }
}