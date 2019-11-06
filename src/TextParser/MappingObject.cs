using System;

namespace Enjaxel.TextParser
{
    /// <summary>
    /// マッピング時に使用するクラス
    /// </summary>
    internal sealed class MappingObject
    {
        /// <summary> Headerと紐づく番号 </summary>
        internal int Number { get; set; }

        /// <summary> プロパティ名称と対になるKey名 </summary>
        internal string MappingKeyName { get; set; }

        /// <summary> プロパティ名称 </summary>
        internal string Name { get; set; }

        /// <summary> プロパティの型 </summary>
        internal Type ObjectType { get; set; }

        /// <summary> Nullable判定 </summary>
        internal bool IsNullable { get; set; }

        /// <summary>
        /// マッピング時に使用します
        /// </summary>
        internal MappingObject()
        {
            Number = -1;
            MappingKeyName = string.Empty;
            Name = string.Empty;
            ObjectType = null;
            IsNullable = false;
        }
    }
}
