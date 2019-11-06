using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Enjaxel.TextParser.Ini;

namespace Enjaxel.TextParser.Config
{
    /// <summary>
    /// INIの内容をListで保持するクラス
    /// </summary>
    public class IniContents : IConfig
    {
        /// <summary> コンフィグの種別 </summary>
        public ConfigType Type { get; }

        /// <summary> セクションの一覧 </summary>
        public IReadOnlyList<string> Sections { get; }

        /// <summary> コンテンツ </summary>
        public IReadOnlyList<IniSection> Contents { get; }

        /// <summary>
        /// INIの内容をListで保持します
        /// </summary>
        public IniContents()
        {
            Type = ConfigType.INI;
        }

        /// <summary>
        /// INIの内容をListで保持します
        /// </summary>
        internal IniContents(IEnumerable<string> Sections,
                             IEnumerable<IniSection> Contents)
        {
            // コピーを作成する
            var s = new List<string>(Sections);
            var c = new List<IniSection>(Contents);

            Type = ConfigType.INI;
            this.Sections = new ReadOnlyCollection<string>(s);
            this.Contents = new ReadOnlyCollection<IniSection>(c);
        }

        /// <summary>
        /// コンフィグを読み込みます
        /// </summary>
        /// <param name="configPath"> コンフィグファイルのパス </param>
        /// <returns></returns>
        public IConfig ReadConfig(string configPath)
        {
            // 実装途中
            return null;
        }

        /// <summary>
        /// IniContentsの情報を文字列として表します
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // 実装途中
            var sb = new StringBuilder();

            return sb.ToString();
        }

        /// <summary>
        /// IniContents内の任意のセクション情報を文字列として表します
        /// </summary>
        /// <param name="section"> セクション名称 </param>
        /// <returns></returns>
        public string ToString(string section)
        {
            // 実装途中
            return "";
        }
    }
}
