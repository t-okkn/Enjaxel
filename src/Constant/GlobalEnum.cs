namespace Enjaxel.Constant
{
    /// <summary>
    /// コンフィグの種別
    /// </summary>
    public enum ConfigType
    {
        /// <summary> Json </summary>
        JSON = 0,

        /// <summary> XML </summary>
        XML = 1,

        /// <summary> YAML </summary>
        YAML = 2,

        /// <summary> TOML </summary>
        TOML = 3,

        /// <summary> HOCON(Typesafe Config) </summary>
        HOCON = 4,

        /// <summary> INI </summary>
        INI = 5,

        /// <summary> CSV </summary>
        CSV = 6,

        /// <summary> Text(Single Line Configure) </summary>
        TEXT = 7,

        /// <summary> その他形式 </summary>
        OTHER = 8
    }
}
