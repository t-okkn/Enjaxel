using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Enjaxel.Linq
{
    /// <summary>
    /// System.Linq.Enumerableの拡張クラス
    /// </summary>
    public static class ExtensionEnumerable
    {
        #region Combination
        /// <summary>
        /// 入力された要素から組み合わせを導出します
        /// </summary>
        /// <typeparam name="T"> 組み合わせを求める型 </typeparam>
        /// <param name="items"> 抽出元の要素 </param>
        /// <param name="pickUp"> 組み合わせ要素数 </param>
        /// <param name="withRepetition"> 重複を許可するかどうかのフラグ </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T[]> Combination<T>
            (this IEnumerable<T> items, int pickUp, bool withRepetition)
            where T : IEquatable<T>
        {
            if (pickUp == 1)
            {
                foreach (T item in items)
                {
                    yield return new T[] { item };
                }
            }

            foreach (T item in items)
            {
                // itemよりも前のものを除く（順列と組み合わせの違い)
                // 重複を許さない場合、unusedから item そのものも取り除く
                IEnumerable<T> unused = withRepetition ?
                                        items :
                                        items.SkipWhile(x => !x.Equals(item)).Skip(1);

                var left = new T[1] { item };
                foreach (T[] right in Combination(unused, pickUp - 1, withRepetition))
                {
                    yield return left.Concat(right).ToArray();
                }
            }
        }

        /// <summary>
        /// 入力された要素から重複を許可せず、組み合わせを導出します
        /// </summary>
        /// <typeparam name="T"> 組み合わせを求める型 </typeparam>
        /// <param name="items"> 抽出元の要素 </param>
        /// <param name="pickUp"> 組み合わせ要素数 </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T[]> Combination<T>(this IEnumerable<T> items, int pickUp)
            where T : IEquatable<T>
        {
            return items.Combination(pickUp, false);
        }
        #endregion

        #region Distinct
        /// <summary>
        /// 指定した IEnumerable&lt;T&gt; を使用してシーケンスから一意の要素を返します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, TKey>
            (this IEnumerable<T> source, Func<T, TKey> selector)
        {
            return source.Distinct(new CompareSelector<T, TKey>(selector));
        }
        #endregion

        #region ForEachAsync（戻り値なし）
        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 同期処理内容 </param>
        /// <param name="actionAsync"> 非同期処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="cancellationToken"> 処理を取り消すために使用するToken </param>
        /// <param name="isExecuteAnotherThread"> 非同期処理を他のスレッドで実行させるか </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        private static async Task InnerForEachAsync<T>(IEnumerable<T> source,
                                                       Action<T> action,
                                                       Func<T, Task> actionAsync,
                                                       int parallelLimit,
                                                       bool isThrowError,
                                                       CancellationToken cancellationToken,
                                                       bool isExecuteAnotherThread)
        {
            // エラー処理
            if (source == null)
            {
                throw new ArgumentNullException("処理対象がNullです");
            }

            if (action == null && actionAsync == null)
            {
                throw new ArgumentNullException("処理内容が定義されていません");
            }

            if (parallelLimit <= 0)
            {
                throw new ArgumentOutOfRangeException("同時に実施する処理の上限値は" +
                                                      "1以上である必要があります");
            }

            using (var semaphore = new SemaphoreSlim(parallelLimit, parallelLimit))
            {
                int ex_count = 0;
                var tasks = new List<Task>(source.Count());

                foreach (T item in source)
                {
                    // エラーカウンターが回っていたらTaskの生成を終了
                    if (isThrowError && Interlocked.Add(ref ex_count, 0) > 0)
                    {
                        break;
                    }

                    // Cancelの受付
                    cancellationToken.ThrowIfCancellationRequested();

                    // 同時処理上限に達している場合、ここで待機
                    await semaphore.WaitAsync(cancellationToken)
                                   .ConfigureAwait(isExecuteAnotherThread);

                    // 同期か非同期のDelegateによってTaskの内容を変える
                    Task wrap_task = null;

                    if (action != null)
                    {
                        wrap_task = Task.Run(() => action(item));
                    }
                    else if (actionAsync != null)
                    {
                        wrap_task = actionAsync(item);
                    }

                    // 対象の要素に対して処理を実施し、Semaphoreの開放と
                    // エラーが発生していた場合の処理を行うTaskを生成
                    Task task = wrap_task.ContinueWith(beforeTask =>
                    {
                        semaphore.Release();

                        if (isThrowError && beforeTask.IsFaulted)
                        {
                            Interlocked.Increment(ref ex_count);
                            throw beforeTask.Exception;
                        }
                    });

                    // Taskの追加
                    tasks.Add(task);
                }

                // 実行
                if (tasks.Count != 0)
                {
                    await Task.WhenAll(tasks).ConfigureAwait(isExecuteAnotherThread);
                }
            }
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="cancellationToken"> 処理を取り消すために使用するToken </param>
        /// <param name="isExecuteAnotherThread"> 非同期処理を他のスレッドで実行させるか </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source,
                                                 Action<T> action,
                                                 int parallelLimit,
                                                 bool isThrowError,
                                                 CancellationToken cancellationToken,
                                                 bool isExecuteAnotherThread)
        {
            await InnerForEachAsync(source, action, null, parallelLimit, isThrowError,
                                    cancellationToken, isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="isExecuteAnotherThread"> 非同期処理を他のスレッドで実行させるか </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source,
                                                 Action<T> action,
                                                 int parallelLimit,
                                                 bool isThrowError,
                                                 bool isExecuteAnotherThread)
        {
            var c = new CancellationToken();
            await source.ForEachAsync
                (action, parallelLimit, isThrowError, c, isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="cancellationToken"> 処理を取り消すために使用するToken </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source,
                                                 Action<T> action,
                                                 int parallelLimit,
                                                 bool isThrowError,
                                                 CancellationToken cancellationToken)
        {
            await source.ForEachAsync
                (action, parallelLimit, isThrowError, cancellationToken, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source,
                                                 Action<T> action,
                                                 int parallelLimit,
                                                 bool isThrowError)
        {
            var c = new CancellationToken();
            await source.ForEachAsync(action, parallelLimit, isThrowError, c, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source,
                                                 Action<T> action,
                                                 int parallelLimit)
        {
            var c = new CancellationToken();
            await source.ForEachAsync(action, parallelLimit, true, c, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的（上限100件）に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>
            (this IEnumerable<T> source, Action<T> action)
        {
            var c = new CancellationToken();
            await source.ForEachAsync(action, 100, true, c, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="cancellationToken"> 処理を取り消すために使用するToken </param>
        /// <param name="isExecuteAnotherThread"> 非同期処理を他のスレッドで実行させるか </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source,
                                                 Func<T, Task> action,
                                                 int parallelLimit,
                                                 bool isThrowError,
                                                 CancellationToken cancellationToken,
                                                 bool isExecuteAnotherThread)
        {
            await InnerForEachAsync(source, null, action, parallelLimit, isThrowError,
                                    cancellationToken, isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="isExecuteAnotherThread"> 非同期処理を他のスレッドで実行させるか </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source,
                                                 Func<T, Task> action,
                                                 int parallelLimit,
                                                 bool isThrowError,
                                                 bool isExecuteAnotherThread)
        {
            var c = new CancellationToken();
            await source.ForEachAsync
                (action, parallelLimit, isThrowError, c, isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="cancellationToken"> 処理を取り消すために使用するToken </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source,
                                                 Func<T, Task> action,
                                                 int parallelLimit,
                                                 bool isThrowError,
                                                 CancellationToken cancellationToken)
        {
            await source.ForEachAsync
                (action, parallelLimit, isThrowError, cancellationToken, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source,
                                                 Func<T, Task> action,
                                                 int parallelLimit,
                                                 bool isThrowError)
        {
            var c = new CancellationToken();
            await source.ForEachAsync(action, parallelLimit, isThrowError, c, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source,
                                                 Func<T, Task> action,
                                                 int parallelLimit)
        {
            var c = new CancellationToken();
            await source.ForEachAsync(action, parallelLimit, true, c, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的（上限100件）に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task ForEachAsync<T>
            (this IEnumerable<T> source, Func<T, Task> action)
        {
            var c = new CancellationToken();
            await source.ForEachAsync(action, 100, true, c, false);
        }
        #endregion

        #region ForEachAsync(戻り値あり)
        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 同期処理内容 </param>
        /// <param name="actionAsync"> 非同期処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="cancellationToken"> 処理を取り消すために使用するToken </param>
        /// <param name="isExecuteAnotherThread"> 非同期処理を他のスレッドで実行させるか </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> InnerForEachAsync<T, TResult>
            (IEnumerable<T> source,
             Func<T, TResult> action,
             Func<T, Task<TResult>> actionAsync,
             int parallelLimit,
             bool isThrowError,
             CancellationToken cancellationToken,
             bool isExecuteAnotherThread)
        {
            // エラー処理
            if (source == null)
            {
                throw new ArgumentNullException("処理対象がNullです");
            }

            if (action == null && actionAsync == null)
            {
                throw new ArgumentNullException("処理内容が定義されていません");
            }

            if (parallelLimit <= 0)
            {
                throw new ArgumentOutOfRangeException("同時に実施する処理の上限値は" +
                                                      "1以上である必要があります");
            }

            using (var semaphore = new SemaphoreSlim(parallelLimit, parallelLimit))
            {
                int ex_count = 0;
                var tasks = new List<Task<TResult>>(source.Count());

                foreach (T item in source)
                {
                    // エラーカウンターが回っていたらTaskの生成を終了
                    if (isThrowError && Interlocked.Add(ref ex_count, 0) > 0)
                    {
                        break;
                    }

                    // Cancelの受付
                    cancellationToken.ThrowIfCancellationRequested();

                    // 同時処理上限に達している場合、ここで待機
                    await semaphore.WaitAsync(cancellationToken)
                                   .ConfigureAwait(isExecuteAnotherThread);

                    // 同期か非同期のDelegateによってTaskの内容を変える
                    Task<TResult> wrap_task = null;

                    if (action != null)
                    {
                        wrap_task = Task.Run(() => action(item));
                    }
                    else if (actionAsync != null)
                    {
                        wrap_task = actionAsync(item);
                    }

                    // 対象の要素に対して処理を実施し、Semaphoreの開放と
                    // エラーが発生していた場合の処理を行うTaskを生成
                    Task<TResult> task = wrap_task.ContinueWith(beforeTask =>
                    {
                        semaphore.Release();

                        if (isThrowError && beforeTask.IsFaulted)
                        {
                            Interlocked.Increment(ref ex_count);
                            throw beforeTask.Exception;
                        }

                        return beforeTask.Result;
                    });

                    // Taskの追加
                    tasks.Add(task);
                }

                // 実行
                if (tasks.Count != 0)
                {
                    return await Task.WhenAll(tasks)
                                     .ConfigureAwait(isExecuteAnotherThread);
                }
                else
                {
                    return new TResult[0] { };
                }
            }
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="cancellationToken"> 処理を取り消すために使用するToken </param>
        /// <param name="isExecuteAnotherThread"> 非同期処理を他のスレッドで実行させるか </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source,
             Func<T, TResult> action,
             int parallelLimit,
             bool isThrowError,
             CancellationToken cancellationToken,
             bool isExecuteAnotherThread)
        {
            return await InnerForEachAsync(source, action, null, parallelLimit,
                                           isThrowError, cancellationToken,
                                           isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="cancellationToken"> 処理を取り消すために使用するToken </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source,
             Func<T, TResult> action,
             int parallelLimit,
             bool isThrowError,
             CancellationToken cancellationToken)
        {
            return await source.ForEachAsync
                (action, parallelLimit, isThrowError, cancellationToken, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="isExecuteAnotherThread"> 非同期処理を他のスレッドで実行させるか </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source,
             Func<T, TResult> action,
             int parallelLimit,
             bool isThrowError,
             bool isExecuteAnotherThread)
        {
            var c = new CancellationToken();
            return await source.ForEachAsync
                (action, parallelLimit, isThrowError, c, isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source,
             Func<T, TResult> action,
             int parallelLimit,
             bool isThrowError)
        {
            var c = new CancellationToken();
            return await source.ForEachAsync
                (action, parallelLimit, isThrowError, c, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source,
             Func<T, TResult> action,
             int parallelLimit)
        {
            var c = new CancellationToken();
            return await source.ForEachAsync(action, parallelLimit, true, c, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的（上限100件）に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source, Func<T, TResult> action)
        {
            var c = new CancellationToken();
            return await source.ForEachAsync(action, 100, true, c, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="cancellationToken"> 処理を取り消すために使用するToken </param>
        /// <param name="isExecuteAnotherThread"> 非同期処理を他のスレッドで実行させるか </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source,
             Func<T, Task<TResult>> action,
             int parallelLimit,
             bool isThrowError,
             CancellationToken cancellationToken,
             bool isExecuteAnotherThread)
        {
            return await InnerForEachAsync(source, null, action, parallelLimit,
                                           isThrowError, cancellationToken,
                                           isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="cancellationToken"> 処理を取り消すために使用するToken </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source,
             Func<T, Task<TResult>> action,
             int parallelLimit,
             bool isThrowError,
             CancellationToken cancellationToken)
        {
            return await source.ForEachAsync
                (action, parallelLimit, isThrowError, cancellationToken, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <param name="isExecuteAnotherThread"> 非同期処理を他のスレッドで実行させるか </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source,
             Func<T, Task<TResult>> action,
             int parallelLimit,
             bool isThrowError,
             bool isExecuteAnotherThread)
        {
            var c = new CancellationToken();
            return await source.ForEachAsync
                (action, parallelLimit, isThrowError, c, isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各要素に対して処理中のエラーを発生させるか </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source,
             Func<T, Task<TResult>> action,
             int parallelLimit,
             bool isThrowError)
        {
            var c = new CancellationToken();
            return await source.ForEachAsync
                (action, parallelLimit, isThrowError, c, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source,
             Func<T, Task<TResult>> action,
             int parallelLimit)
        {
            var c = new CancellationToken();
            return await source.ForEachAsync(action, parallelLimit, true, c, false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的（上限100件）に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象の要素の型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public static async Task<TResult[]> ForEachAsync<T, TResult>
            (this IEnumerable<T> source, Func<T, Task<TResult>> action)
        {
            var c = new CancellationToken();
            return await source.ForEachAsync(action, 100, true, c, false);
        }
        #endregion
    }
}
