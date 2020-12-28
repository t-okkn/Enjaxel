using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Enjaxel.Threading
{
    /// <summary>
    /// 非同期版ForEach
    /// </summary>
    public class ForEachAsync
    {
        #region 戻り値なし
        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 同期処理内容 </param>
        /// <param name="actionAsync"> 非同期処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        private async static Task InnerParallel<T>(
            IEnumerable<T> source,
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
                throw new ArgumentNullException("処理対象がnullです");
            }

            if (action == null && actionAsync == null)
            {
                throw new ArgumentNullException("処理内容が定義されていません");
            }

            if (parallelLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "同時に実施する処理の上限値は1以上である必要があります");
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

                    // 同期か非同期のDelegateで処理内容を変える
                    Task wrap_task = null;

                    if (action != null)
                    {
                        wrap_task = Task.Run(() => action(item));
                    }   
                    else if (actionAsync != null)
                    {
                        wrap_task = actionAsync(item);
                    }

                    // 対象のアイテムに対して処理を実施し、Semaphoreの開放と
                    // エラーが発生していた場合の処理を行うTaskを生成
                    Task stask = wrap_task.ContinueWith(beforeTask =>
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            semaphore.Release();
                        }

                        if (isThrowError && beforeTask.IsFaulted)
                        {
                            Interlocked.Increment(ref ex_count);
                            throw beforeTask.Exception;
                        }
                    });

                    // Taskの追加
                    tasks.Add(stask);
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
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task Parallel<T>(
            IEnumerable<T> source,
            Action<T> action,
            int parallelLimit,
            bool isThrowError,
            CancellationToken cancellationToken,
            bool isExecuteAnotherThread)
        {
            await InnerParallel(
                source,
                action,
                null,
                parallelLimit,
                isThrowError,
                cancellationToken,
                isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task Parallel<T>(
            IEnumerable<T> source,
            Action<T> action,
            int parallelLimit,
            bool isThrowError,
            bool isExecuteAnotherThread)
        {
            await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                new CancellationToken(),
                isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task Parallel<T>(
            IEnumerable<T> source,
            Action<T> action,
            int parallelLimit,
            bool isThrowError,
            CancellationToken cancellationToken)
        {
            await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                cancellationToken,
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public async static Task Parallel<T>(
            IEnumerable<T> source,
            Action<T> action,
            int parallelLimit,
            bool isThrowError)
        {
            await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                new CancellationToken(),
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
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
        public async static Task Parallel<T>(
            IEnumerable<T> source,
            Action<T> action,
            int parallelLimit)
        {
            await Parallel(
                source,
                action,
                parallelLimit,
                true,
                new CancellationToken(),
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的（上限100件）に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
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
        public async static Task Parallel<T>(IEnumerable<T> source, Action<T> action)
        {
            await Parallel(
                source,
                action,
                100,
                true,
                new CancellationToken(),
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task Parallel<T>(
            IEnumerable<T> source,
            Func<T, Task> action,
            int parallelLimit,
            bool isThrowError,
            CancellationToken cancellationToken,
            bool isExecuteAnotherThread)
        {
            await InnerParallel(
                source,
                null,
                action,
                parallelLimit,
                isThrowError,
                cancellationToken,
                isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task Parallel<T>(
            IEnumerable<T> source,
            Func<T, Task> action,
            int parallelLimit,
            bool isThrowError,
            bool isExecuteAnotherThread)
        {
            await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                new CancellationToken(),
                isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task Parallel<T>(
            IEnumerable<T> source,
            Func<T, Task> action,
            int parallelLimit,
            bool isThrowError,
            CancellationToken cancellationToken)
        {
            await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                cancellationToken,
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限値 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
        /// <returns> Task </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public async static Task Parallel<T>(
            IEnumerable<T> source,
            Func<T, Task> action,
            int parallelLimit,
            bool isThrowError)
        {
            await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                new CancellationToken(),
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
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
        public async static Task Parallel<T>(
            IEnumerable<T> source,
            Func<T, Task> action,
            int parallelLimit)
        {
            await Parallel(
                source,
                action,
                parallelLimit,
                true,
                new CancellationToken(),
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的（上限100件）に順次処理を行います（戻り値なし）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
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
        public async static Task Parallel<T>(IEnumerable<T> source, Func<T, Task> action)
        {
            await Parallel(
                source,
                action,
                100,
                true,
                new CancellationToken(),
                false);
        }
        #endregion

        #region 戻り値あり
        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 同期処理内容 </param>
        /// <param name="actionAsync"> 非同期処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        private async static Task<TResult[]> InnerParallel<T, TResult>(
            IEnumerable<T> source,
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
                throw new ArgumentNullException("処理対象がnullです");
            }

            if (action == null && actionAsync == null)
            {
                throw new ArgumentNullException("処理内容が定義されていません");
            }

            if (parallelLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "同時に実施する処理の上限値は1以上である必要があります");
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

                    // 同期か非同期のDelegateで処理内容を変える
                    Task<TResult> wrap_task = null;

                    if (action != null)
                    {
                        wrap_task = Task.Run(() => action(item));
                    }
                    else if (actionAsync != null)
                    {
                        wrap_task = actionAsync(item);
                    }

                    // 対象のアイテムに対して処理を実施し、Semaphoreの開放と
                    // エラーが発生していた場合の処理を行うTaskを生成
                    Task<TResult> ftask = wrap_task.ContinueWith(beforeTask =>
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            semaphore.Release();
                        }

                        if (isThrowError && beforeTask.IsFaulted)
                        {
                            Interlocked.Increment(ref ex_count);
                            throw beforeTask.Exception;
                        }

                        return beforeTask.Result;
                    });

                    // Taskの追加
                    tasks.Add(ftask);
                }

                // 実行
                if (tasks.Count != 0)
                {
                    return await Task.WhenAll(tasks).ConfigureAwait(isExecuteAnotherThread);
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
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, TResult> action,
            int parallelLimit,
            bool isThrowError,
            CancellationToken cancellationToken,
            bool isExecuteAnotherThread)
        {
            return await InnerParallel(
                source,
                action,
                null,
                parallelLimit,
                isThrowError,
                cancellationToken,
                isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, TResult> action,
            int parallelLimit,
            bool isThrowError,
            CancellationToken cancellationToken)
        {
            return await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                cancellationToken,
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, TResult> action,
            int parallelLimit,
            bool isThrowError,
            bool isExecuteAnotherThread)
        {
            return await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                new CancellationToken(),
                isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, TResult> action,
            int parallelLimit,
            bool isThrowError)
        {
            return await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                new CancellationToken(),
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
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
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, TResult> action,
            int parallelLimit)
        {
            return await Parallel(
                source,
                action,
                parallelLimit,
                true,
                new CancellationToken(),
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的（上限100件）に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
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
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, TResult> action)
        {
            return await Parallel(
                source,
                action,
                100,
                true,
                new CancellationToken(),
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, Task<TResult>> action,
            int parallelLimit,
            bool isThrowError,
            CancellationToken cancellationToken,
            bool isExecuteAnotherThread)
        {
            return await InnerParallel(
                source,
                null,
                action,
                parallelLimit,
                isThrowError,
                cancellationToken,
                isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, Task<TResult>> action,
            int parallelLimit,
            bool isThrowError,
            CancellationToken cancellationToken)
        {
            return await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                cancellationToken,
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
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
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, Task<TResult>> action,
            int parallelLimit,
            bool isThrowError,
            bool isExecuteAnotherThread)
        {
            return await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                new CancellationToken(),
                isExecuteAnotherThread);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
        /// <typeparam name="TResult"> 処理後の戻り値の型 </typeparam>
        /// <param name="source"> 処理対象 </param>
        /// <param name="action"> 処理内容 </param>
        /// <param name="parallelLimit"> 同時に実施する処理の上限 </param>
        /// <param name="isThrowError"> 各アイテムに対して処理中のエラーを発生させるか </param>
        /// <returns> TResultの配列を結果として返すTask </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="SemaphoreFullException"></exception>
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, Task<TResult>> action,
            int parallelLimit,
            bool isThrowError)
        {
            return await Parallel(
                source,
                action,
                parallelLimit,
                isThrowError,
                new CancellationToken(),
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
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
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, Task<TResult>> action,
            int parallelLimit)
        {
            return await Parallel(
                source,
                action,
                parallelLimit,
                true,
                new CancellationToken(),
                false);
        }

        /// <summary>
        /// 非同期にて処理対象に対して並列的（上限100件）に順次処理を行います（戻り値あり）
        /// </summary>
        /// <typeparam name="T"> 処理対象のアイテムの型 </typeparam>
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
        public async static Task<TResult[]> Parallel<T, TResult>(
            IEnumerable<T> source,
            Func<T, Task<TResult>> action)
        {
            return await Parallel(
                source,
                action,
                100,
                true,
                new CancellationToken(),
                false);
        }
        #endregion
    }
}
