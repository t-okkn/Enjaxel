using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Enjaxel.TextParser.Ini
{
    /// <summary>
    /// INIのセクション内容を保持するクラス
    /// </summary>
    public sealed class IniSection
    {
        /// <summary> セクション名称 </summary>
        public string Name { get; }

        /// <summary> セクションに属しているパラメータ </summary>
        public IReadOnlyDictionary<string, string> Parameters { get; }

        /// <summary>
        /// INIのセクション内容を保持します
        /// </summary>
        public IniSection(string Name, IDictionary<string, string> Parameters)
        {
            this.Name = Name;
            this.Parameters = new ReadOnlyDictionary<string, string>(Parameters);
        }
    }
}
