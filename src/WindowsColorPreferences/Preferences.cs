using Microsoft.Win32;
using System.Drawing;

namespace BlackSugar.WindowsColor
{
    public class Preferences
    {

        public string GetTheme()
        {
            var theme = "Light";

            const string keyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string valueName = "AppsUseLightTheme";

            using (var rKey = Registry.CurrentUser.OpenSubKey(keyName))
            {
                var appsUseLightTheme = rKey?.GetValue(valueName);
                if (appsUseLightTheme?.Equals(0) == true)
                    theme = "Dark";

                rKey?.Close();
            }

            return theme;
        }

        public Color GetAccentColor()
        {
            var color = Color.Empty;
            const string keyName = @"SOFTWARE\Microsoft\Windows\DWM";
            const string valueName = "ColorizationColor";

            using (var rKey = Registry.CurrentUser.OpenSubKey(keyName))
            {
                var colorizationColor = rKey?.GetValue(valueName) as int?;
                if (colorizationColor != null)
                {
                    var hex = colorizationColor?.ToString("X");

                    var uColor = (uint)colorizationColor.Value;
                    color = Color.FromArgb((byte)(uColor >> 24), (byte)(uColor >> 16), (byte)(uColor >> 8), (byte)(uColor));
                }
                rKey?.Close();
            }

            return color;
        }

    }
}