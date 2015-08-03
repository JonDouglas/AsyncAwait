using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwait
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        /// <summary>
        /// Pausing for a period of time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        static async Task<T> DelayResult<T>(T result, TimeSpan delay)
        {
            //Useful for use in Unit Testing to fake an async operation.
            await Task.Delay(delay);
            return result;
        }
        /// <summary>
        /// Pausing for a period of time.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static async Task<string> DownloadStringWithRetries(string uri)
        {
            using (var client = new HttpClient())
            {
                //Retry after 1 second, 2 seconds, 4 seconds.
                var nextDelay = TimeSpan.FromSeconds(1);
                for (int i = 0; i != 3; ++i)
                {
                    try
                    {
                        return await client.GetStringAsync(uri);
                    }
                    catch
                    {
                    }

                    await Task.Delay(nextDelay);
                    nextDelay = nextDelay + nextDelay;
                }

                //Try one last time, allowing the error to propogate.
                return await client.GetStringAsync(uri);
            }
        }

        /// <summary>
        /// Pausing for a period of time.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static async Task<string> DownloadStringWithTimeout(string uri)
        {
            using (var client = new HttpClient())
            {
                var downloadTask = client.GetStringAsync(uri);
                var timeoutTask = Task.Delay(3000);

                var completedTask = await Task.WhenAny(downloadTask, timeoutTask);
                if (completedTask == timeoutTask)
                {
                    return null;
                }
                return await downloadTask;
            }
        }

        interface IMyAsyncInterface
        {
            Task<int> GetValueAsync();
        }

        class MySynchronousImplementation : IMyAsyncInterface
        {
            public Task<int> GetValueAsync()
            {
                return Task.FromResult(13);
            }
        }
    }
}
