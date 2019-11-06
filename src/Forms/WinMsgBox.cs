using System;
using System.Text;
using System.Windows.Forms;

namespace Enjaxel.Forms
{
    /// <summary>
    /// MessageBoxクラスのWrapper
    /// </summary>
    public static class WinMsgBox
    {
        #region Show
        /// <summary>
        /// 指定したテキストを表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text)
        {
            return MessageBox.Show(text);
        }

        /// <summary>
        /// 指定したオブジェクトの前に、指定したテキストを表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="owner"> モーダルダイアログボックスを所有するウインドウ </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text, IWin32Window owner)
        {
            return MessageBox.Show(owner, text);
        }

        /// <summary>
        /// 指定したテキストとキャプションを表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text, string caption)
        {
            return MessageBox.Show(text, caption);
        }

        /// <summary>
        /// 指定したオブジェクトの前に、指定したテキストとキャプションを表示する
        /// メッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="owner"> モーダルダイアログボックスを所有するウインドウ </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show
            (string text, string caption, IWin32Window owner)
        {
            return MessageBox.Show(owner, text, caption);
        }

        /// <summary>
        /// 指定したテキスト、キャプション、およびボタンを表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show
            (string text, string caption, MessageBoxButtons buttons)
        {
            return MessageBox.Show(text, caption, buttons);
        }

        /// <summary>
        /// 指定したオブジェクトの前に、指定したテキスト、キャプション、およびボタンを表示する
        /// メッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="owner"> モーダルダイアログボックスを所有するウインドウ </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show
            (string text, string caption, MessageBoxButtons buttons, IWin32Window owner)
        {
            return MessageBox.Show(owner, text, caption, buttons);
        }

        /// <summary>
        /// 指定したテキスト、キャプション、ボタン、およびボタンを表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show
            (string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// 指定したオブジェクトの前に、指定したテキスト、キャプション、ボタン、およびボタンを
        /// 表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="owner"> モーダルダイアログボックスを所有するウインドウ </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        IWin32Window owner)
        {
            return MessageBox.Show(owner, text, caption, buttons, icon);
        }

        /// <summary>
        /// 指定したテキスト、キャプション等を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton);
        }

        /// <summary>
        /// 指定したオブジェクトの前に、指定したテキスト、キャプション等を表示する
        /// メッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="owner"> モーダルダイアログボックスを所有するウインドウ </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        IWin32Window owner)
        {
            return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton);
        }

        /// <summary>
        /// 指定したテキスト、キャプション等を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options);
        }

        /// <summary>
        /// 指定したオブジェクトの前に、指定したテキスト、キャプション等を表示する
        /// メッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <param name="owner"> モーダルダイアログボックスを所有するウインドウ </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options,
                                        IWin32Window owner)
        {
            return MessageBox.Show(owner, text, caption, buttons, icon,
                                   defaultButton, options);
        }

        /// <summary>
        /// 指定したテキスト、キャプション等を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <param name="displayHelpButton"> ヘルプボタンの表示可否 </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options,
                                        bool displayHelpButton)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton,
                                   options, displayHelpButton);
        }

        /// <summary>
        /// 指定したテキスト、キャプション等を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <param name="helpFilePath"> ヘルプファイルのPath </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options,
                                        string helpFilePath)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton,
                                   options, helpFilePath);
        }

        /// <summary>
        /// 指定したオブジェクトの前に、指定したテキスト、キャプション等を表示する
        /// メッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <param name="helpFilePath"> ヘルプファイルのPath </param>
        /// <param name="owner"> モーダルダイアログボックスを所有するウインドウ </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options,
                                        string helpFilePath,
                                        IWin32Window owner)
        {
            return MessageBox.Show(owner, text, caption, buttons, icon,
                                   defaultButton, options, helpFilePath);
        }

        /// <summary>
        /// 指定したテキスト、キャプション等を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <param name="helpFilePath"> ヘルプファイルのPath </param>
        /// <param name="navigator"> ヘルプの種別 </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options,
                                        string helpFilePath,
                                        HelpNavigator navigator)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton,
                                   options, helpFilePath, navigator);
        }

        /// <summary>
        /// 指定したオブジェクトの前に、指定したテキスト、キャプション等を表示する
        /// メッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <param name="helpFilePath"> ヘルプファイルのPath </param>
        /// <param name="navigator"> ヘルプの種別 </param>
        /// <param name="owner"> モーダルダイアログボックスを所有するウインドウ </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options,
                                        string helpFilePath,
                                        HelpNavigator navigator,
                                        IWin32Window owner)
        {
            return MessageBox.Show(owner, text, caption, buttons, icon,
                                   defaultButton, options, helpFilePath, navigator);
        }

        /// <summary>
        /// 指定したテキスト、キャプション等を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <param name="helpFilePath"> ヘルプファイルのPath </param>
        /// <param name="keyword"> ヘルプのキーワード </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options,
                                        string helpFilePath,
                                        string keyword)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton,
                                   options, helpFilePath, keyword);
        }

        /// <summary>
        /// 指定したオブジェクトの前に、指定したテキスト、キャプション等を表示する
        /// メッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <param name="helpFilePath"> ヘルプファイルのPath </param>
        /// <param name="keyword"> ヘルプのキーワード </param>
        /// <param name="owner"> モーダルダイアログボックスを所有するウインドウ </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options,
                                        string helpFilePath,
                                        string keyword,
                                        IWin32Window owner)
        {
            return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton,
                                   options, helpFilePath, keyword);
        }

        /// <summary>
        /// 指定したテキスト、キャプション等を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <param name="helpFilePath"> ヘルプファイルのPath </param>
        /// <param name="navigator"> ヘルプの種別 </param>
        /// <param name="param"> ヘルプトピックのID </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options,
                                        string helpFilePath,
                                        HelpNavigator navigator,
                                        object param)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton,
                                   options, helpFilePath, navigator, param);
        }

        /// <summary>
        /// 指定したオブジェクトの前に、指定したテキスト、キャプション等を表示する
        /// メッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="icon"> メッセージボックスに表示するアイコン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <param name="helpFilePath"> ヘルプファイルのPath </param>
        /// <param name="navigator"> ヘルプの種別 </param>
        /// <param name="param"> ヘルプトピックのID </param>
        /// <param name="owner"> モーダルダイアログボックスを所有するウインドウ </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options,
                                        string helpFilePath,
                                        HelpNavigator navigator,
                                        object param,
                                        IWin32Window owner)
        {
            return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton,
                                   options, helpFilePath, navigator, param);
        }
        #endregion

        #region Warn
        /// <summary>
        /// 警告情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Warn(string text)
        {
            var caption = "警告";
            var icon = MessageBoxIcon.Warning;
            var buttons = MessageBoxButtons.OK;

            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// 警告情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Warn(string text, string caption)
        {
            var icon = MessageBoxIcon.Warning;
            var buttons = MessageBoxButtons.OK;

            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// 警告情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Warn
            (string text, string caption, MessageBoxButtons buttons)
        {
            var icon = MessageBoxIcon.Warning;
            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// 警告情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Warn(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxDefaultButton defaultButton)
        {
            var icon = MessageBoxIcon.Warning;
            return MessageBox.Show(text, caption, buttons, icon, defaultButton);
        }

        /// <summary>
        /// 警告情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Warn(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options)
        {
            var icon = MessageBoxIcon.Warning;
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options);
        }
        #endregion

        #region Error
        /// <summary>
        /// エラー情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Error(string text)
        {
            var caption = "エラー";
            var icon = MessageBoxIcon.Error;
            var buttons = MessageBoxButtons.OK;

            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// エラー情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Error(string text, string caption)
        {
            var icon = MessageBoxIcon.Error;
            var buttons = MessageBoxButtons.OK;

            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// エラー情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Error
            (string text, string caption, MessageBoxButtons buttons)
        {
            var icon = MessageBoxIcon.Error;
            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// エラー情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Error(string text,
                                         string caption,
                                         MessageBoxButtons buttons,
                                         MessageBoxDefaultButton defaultButton)
        {
            var icon = MessageBoxIcon.Error;
            return MessageBox.Show(text, caption, buttons, icon, defaultButton);
        }

        /// <summary>
        /// エラー情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Error(string text,
                                         string caption,
                                         MessageBoxButtons buttons,
                                         MessageBoxDefaultButton defaultButton,
                                         MessageBoxOptions options)
        {
            var icon = MessageBoxIcon.Error;
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options);
        }

        /// <summary>
        /// Exceptionの情報を表示するエラーメッセージボックスを表示します
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="isShowTrace"></param>
        /// <param name="additional"></param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Error(Exception ex, bool isShowTrace, string additional)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(additional))
            {
                sb.AppendLine(additional);
            }

            if (isShowTrace)
            {
                sb.AppendLine("【エラーメッセージとTrace内容】");
                sb.AppendLine(ex.ToString());
            }
            else
            {
                sb.AppendLine("【エラーメッセージ】");
                sb.AppendLine(ex.Message);
            }

            sb.AppendLine();
            sb.AppendLine("【Exceptionの型】");
            sb.AppendLine(ex.GetType().FullName);

            // InnerExceptionについても処理
            if (ex.InnerException != null)
            {
                sb.AppendLine();

                if (isShowTrace)
                {
                    sb.AppendLine("【内部エラーメッセージとTrace内容】");
                    sb.AppendLine(ex.InnerException.ToString());
                }
                else
                {
                    sb.AppendLine("【内部エラーメッセージ】");
                    sb.AppendLine(ex.InnerException.Message);
                }

                sb.AppendLine();
                sb.AppendLine("【内部のExceptionの型】");
                sb.AppendLine(ex.InnerException.GetType().FullName);
            }

            string caption = "エラー";
            var icon = MessageBoxIcon.Error;
            var buttons = MessageBoxButtons.OK;

            return MessageBox.Show(sb.ToString(), caption, buttons, icon);
        }

        /// <summary>
        /// Exceptionの情報を表示するエラーメッセージボックスを表示します
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="isShowTrace"></param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Error(Exception ex, bool isShowTrace)
        {
            return Error(ex, isShowTrace, "■エラー詳細\r\n");
        }
        #endregion

        #region Info
        /// <summary>
        /// 一般情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Info(string text)
        {
            var caption = "情報";
            var icon = MessageBoxIcon.Information;
            var buttons = MessageBoxButtons.OK;

            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// 一般情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Info(string text, string caption)
        {
            var icon = MessageBoxIcon.Information;
            var buttons = MessageBoxButtons.OK;

            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// 一般情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Info
            (string text, string caption, MessageBoxButtons buttons)
        {
            var icon = MessageBoxIcon.Information;
            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// 一般情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Info(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxDefaultButton defaultButton)
        {
            var icon = MessageBoxIcon.Information;
            return MessageBox.Show(text, caption, buttons, icon, defaultButton);
        }

        /// <summary>
        /// 一般情報を表示するメッセージボックスを表示します
        /// </summary>
        /// <param name="text"> メッセージボックスに表示するテキスト </param>
        /// <param name="caption"> メッセージボックスのタイトルバーに表示するテキスト </param>
        /// <param name="buttons"> メッセージボックスに表示するボタン </param>
        /// <param name="defaultButton"> メッセージボックスの規定のボタン </param>
        /// <param name="options"> メッセージボックスで使用する表示オプション </param>
        /// <returns> ダイアログボックスの戻り値を示す識別子 </returns>
        public static DialogResult Info(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxDefaultButton defaultButton,
                                        MessageBoxOptions options)
        {
            var icon = MessageBoxIcon.Information;
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options);
        }
        #endregion
    }
}
