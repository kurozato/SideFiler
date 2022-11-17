using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Drawing;
using BlackSugar.Extension;

namespace BlackSugar.Service.Model
{
    public enum UITheme
    {
        Light = 1,
        Dark = 2,
    } 

    public class UISettingsModel
    {
        [JsonPropertyName("theme")]
        public string? ThemeName { get; set; }

        [JsonPropertyName("accentColor")]
        public string? AccentColor { get; set; }

        [JsonPropertyName("tabColor")]
        public string? TabColor { get; set; }

        [JsonPropertyName("baseColor")]
        public string? BaseColor { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonPropertyName("width")]
        public int? Width { get; set; }


        [JsonIgnore]
        public UITheme Theme => ThemeName.TryParse<UITheme>();//ConvertTheme.TryGetTheme(ThemeName);

        [JsonIgnore]
        public static UISettingsModel Default
            => new UISettingsModel
            {
                ThemeName = "light",
                TabColor = "lightBlue",
                BaseColor = "steel",
                AccentColor = "grey"
            };

        public static UISettingsModel TryDark()
             => new UISettingsModel
             {
                 ThemeName = "dark",
                 TabColor = "lightBlue",
                 BaseColor = "steel",
                 AccentColor = "grey"
             };
    }
}
