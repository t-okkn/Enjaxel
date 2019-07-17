namespace Enjaxel.Communication
{
    /// <summary>
    /// 名前付きパイプIPC通信向けラッパーインターフェース
    /// </summary>
    public interface INamedPipeIpc
    {
        /// <summary>
        /// パイプ名
        /// </summary>
        string PipeName { get; }
    }
}
