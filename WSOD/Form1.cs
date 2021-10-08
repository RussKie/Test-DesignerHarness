using Microsoft.VisualBasic.ApplicationServices;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Text;

namespace StuckWsod
{
    public partial class Form1 : BaseForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            label1.Text = Properties.Settings.Default.IsPortable.ToString();
        }
    }

    public class BaseForm : Form
    {
        public BaseForm()
        {
            if (ThemeModule.IsDarkTheme)
            {
                throw new Exception("BOO");
            }
        }
    }

    public static class ThemeModule
    {
        private static ThemeRepository Repository { get; } = new();
        public static bool IsDarkTheme { get; private set; }
    }

    public class ThemeRepository
    {
        public ThemeRepository()
        {
            string appDirectory = AppSettings.GetGitExtensionsDirectory() ?? throw new DirectoryNotFoundException("Application directory not found");
        }
    }

    public static class AppSettings
    {
        private static string _applicationExecutablePath = Application.ExecutablePath;

        static AppSettings()
        {
            ApplicationDataPath = new Lazy<string?>(() =>
            {
                if (IsPortable())
                {
                    return GetGitExtensionsDirectory();
                }

                // Make ApplicationDataPath version independent
                return Application.UserAppDataPath;
            });

            LocalApplicationDataPath = new Lazy<string?>(() =>
            {
                if (IsPortable())
                {
                    return GetGitExtensionsDirectory();
                }

                string path = "";
                return path;
            });

            ////var trace = new StackTrace();
            ////throw new Exception(trace.ToString());

            //bool newFile = CreateEmptySettingsFileIfMissing();

            //SettingsContainer = new RepoDistSettings(null, GitExtSettingsCache.FromCache(SettingsFilePath), SettingLevel.Unknown);

            //if (newFile || !File.Exists(SettingsFilePath))
            //{
            //    ImportFromRegistry();
            //}

            //MigrateAvatarSettings();
            //MigrateSshSettings();

            return;

            static bool CreateEmptySettingsFileIfMissing()
            {
                try
                {
                    string dir = Path.GetDirectoryName(SettingsFilePath);
                    if (!Directory.Exists(dir) || File.Exists(SettingsFilePath))
                    {
                        return false;
                    }
                }
                catch (ArgumentException)
                {
                    // Illegal characters in the filename
                    return false;
                }

                File.WriteAllText(SettingsFilePath, "<?xml version=\"1.0\" encoding=\"utf-8\"?><dictionary />", Encoding.UTF8);
                return true;
            }
        }

        public static bool IsPortable() => Properties.Settings.Default.IsPortable;

        public static Version AppVersion => Assembly.GetCallingAssembly().GetName().Version;
        public static string ProductVersion => Application.ProductVersion;
        public static readonly string ApplicationName = "Git Extensions";
        public static readonly string ApplicationId = ApplicationName.Replace(" ", "");
        public static readonly string SettingsFileName = ApplicationId + ".settings";
        public static readonly string UserPluginsDirectoryName = "UserPlugins";
        public static string SettingsFilePath => Path.Combine(ApplicationDataPath.Value, SettingsFileName);
        public static string UserPluginsPath => Path.Combine(LocalApplicationDataPath.Value, UserPluginsDirectoryName);
        public static string AvatarImageCachePath => Path.Combine(ApplicationDataPath.Value, "Images\\");
        public static Lazy<string?> ApplicationDataPath { get; private set; }
        public static readonly Lazy<string?> LocalApplicationDataPath;

        public static string? GetGitExtensionsDirectory()
        {
            return Path.GetDirectoryName(GetGitExtensionsFullPath());
        }
        public static string GetGitExtensionsFullPath()
        {
            return _applicationExecutablePath;
        }
    }
}
