using System;

namespace Enjaxel.TextParser.Ini
{
    /// <summary>
    /// INIファイルのセクション・パラメータとプロパティを紐付けるための属性クラス
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IniMappingAttribute : Attribute
    {
        /// <summary> セクション名 </summary>
        public string SectionName { get; set; }

        /// <summary> パラメータ名 </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// INIファイルのセクション・パラメータとプロパティを紐付けるための属性
        /// </summary>
        public IniMappingAttribute(string SectionName, string ParameterName)
        {
            this.SectionName = SectionName;
            this.ParameterName = ParameterName;
        }
    }
}
