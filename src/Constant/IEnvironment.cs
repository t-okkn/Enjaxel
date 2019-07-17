using System.Net;

namespace Enjaxel.Constant
{
    /// <summary>
    /// 環境変数定義インターフェース
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// ConfigファイルのPath
        /// </summary>
        string ConfigPath { get; set; }

        /// <summary>
        /// 実行しているアプリのPath
        /// </summary>
        string AppPath { get; set; }

        /// <summary>
        /// 実行しているアプリのDirectory
        /// </summary>
        string AppDir { get; set; }

        /// <summary>
        /// 実行しているアプリの名称
        /// </summary>
        string MyName { get; set; }

        /// <summary>
        /// 実行端末のIPアドレス群
        /// </summary>
        IPAddress[] MyIPAddress { get; set; }
    }
}
