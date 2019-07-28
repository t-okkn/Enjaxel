using Enjaxel.Constant;
using System.Collections;
using System.ComponentModel;

namespace Enjaxel.TextParser.Config
{
    /// <summary>
    /// コンフィグの内容を格納するインターフェース
    /// </summary>
    public interface IConfig
    {
        /// <summary> コンフィグの種別 </summary>
        ConfigType Type { get; }

        /// <summary>
        /// コンフィグをファイルから読み取り、コンフィグの内容を返します
        /// </summary>
        /// <returns> コンフィグの内容 </returns>
        IConfig ReadConfig(string configPath);
    }
}
