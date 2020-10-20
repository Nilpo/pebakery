﻿using PEBakery.Core.ViewModels;
using PEBakery.Helper;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace PEBakery.Core
{
    public class HttpFileDownloader
    {
        #region Properties
        private readonly MainViewModel _m;
        private readonly int _timeOut;
        private readonly string _userAgent;
        private readonly string _referer;
        #endregion

        #region Constructor
        public HttpFileDownloader(MainViewModel m, int timeOut, string customUserAgent, string referer)
        {
            _m = m;
            _timeOut = timeOut;
            _userAgent = customUserAgent ?? Engine.DefaultUserAgent;
            _referer = referer;
        }
        #endregion

        #region Download
        public struct Report
        {
            /// <summary>
            /// Successfully finished receiving the reposne without exceptions
            /// </summary>
            public bool Result;
            /// <summary>
            /// HTTP status code came with respose. When the request could not be sent, it is set to 0.
            /// </summary>
            public int StatusCode;
            public string ErrorMsg;

            public Report(bool result, int statusCode, string errorMsg)
            {
                Result = result;
                StatusCode = statusCode;
                ErrorMsg = errorMsg;
            }
        }

        /// <summary>
        /// Download a file with HttpClient.
        /// </summary>
        /// <returns>true in case of success.</returns>
        public async Task<Report> Download(string url, string destPath, CancellationToken? cancelToken = null)
        {
            Uri uri = new Uri(url);

            bool result;
            HttpStatusCode statusCode;
            string errorMsg = null;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.AllowAutoRedirect = true;
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                using (HttpClient client = new HttpClient(handler))
                {
                    // Set Timeout
                    client.Timeout = TimeSpan.FromSeconds(_timeOut);

                    // User Agent
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);

                    // Referrer
                    if (_referer != null)
                    {
                        Uri refererUri = new Uri(_referer);
                        client.DefaultRequestHeaders.Referrer = refererUri;
                    }

                    // Progress Report
                    Progress<(long Position, long ContentLength, TimeSpan Elapsed)> progress = null;
                    if (_m != null)
                    {
                        progress = new Progress<(long, long, TimeSpan)>(x =>
                        {
                            (long position, long contentLength, TimeSpan t) = x;
                            string elapsedStr = $"Elapsed Time: {(int)t.TotalHours}h {t.Minutes}m {t.Seconds}s";

                            if (0 < contentLength)
                            { // Server returned proper content length.
                                Debug.Assert(position <= contentLength);
                                double percent = position * 100.0 / contentLength;
                                _m.BuildCommandProgressValue = percent;

                                string receivedStr = $"Received : {NumberHelper.ByteSizeToSIUnit(position, 1)} ({percent:0.0}%)";

                                int totalSec = (int)t.TotalSeconds;
                                string total = NumberHelper.ByteSizeToSIUnit(contentLength, 1);
                                if (totalSec == 0)
                                {
                                    _m.BuildCommandProgressText = $"{url}\r\nTotal : {total}\r\n{receivedStr}\r\n{elapsedStr}";
                                }
                                else
                                {
                                    long bytePerSec = position / totalSec; // Byte per sec
                                    string speedStr = NumberHelper.ByteSizeToSIUnit(bytePerSec, 1) + "/s"; // KB/s, MB/s, ...

                                    // ReSharper disable once PossibleLossOfFraction
                                    TimeSpan r = TimeSpan.FromSeconds((contentLength - position) / bytePerSec);
                                    string remainStr = $"Remaining Time : {(int)r.TotalHours}h {r.Minutes}m {r.Seconds}s";
                                    _m.BuildCommandProgressText = $"{url}\r\nTotal : {total}\r\n{receivedStr}\r\nSpeed : {speedStr}\r\n{elapsedStr}\r\n{remainStr}";
                                }
                            }
                            else
                            { // Ex) Response do not have content length info. Ex) Google Drive
                                if (!_m.BuildCommandProgressIndeterminate)
                                    _m.BuildCommandProgressIndeterminate = true;

                                string receivedStr = $"Received : {NumberHelper.ByteSizeToSIUnit(position, 1)}";

                                int totalSec = (int)t.TotalSeconds;
                                if (totalSec == 0)
                                {
                                    _m.BuildCommandProgressText = $"{url}\r\n{receivedStr}\r\n{elapsedStr}";
                                }
                                else
                                {
                                    long bytePerSec = position / totalSec; // Byte per sec
                                    string speedStr = NumberHelper.ByteSizeToSIUnit(bytePerSec, 1) + "/s"; // KB/s, MB/s, ...
                                    _m.BuildCommandProgressText = $"{url}\r\n{receivedStr}\r\nSpeed : {speedStr}\r\n{elapsedStr}";
                                }
                            }
                        });
                    }

                    // Download file from uri
                    using (FileStream fs = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                    {
                        TimeSpan reportInterval = TimeSpan.FromSeconds(1);
                        HttpClientDownloader downloader = new HttpClientDownloader(client, uri, fs, progress, reportInterval, cancelToken);
                        try
                        {
                            await downloader.DownloadAsync();

                            Debug.Assert(downloader.StatusCode != null, "Successful HTTP response must have status code.");
                            statusCode = (HttpStatusCode)downloader.StatusCode;

                            result = true;
                        }
                        catch (HttpRequestException e)
                        {
                            if (downloader.StatusCode == null)
                                statusCode = 0; // Unable to send a request. Ex) Network not available
                            else
                                statusCode = (HttpStatusCode)downloader.StatusCode;

                            result = false;
                            errorMsg = $"[{(int)statusCode}] {e.Message}";
                        }
                    }
                }

            }

            if (!result)
            { // Download failed, delete file
                if (File.Exists(destPath))
                    File.Delete(destPath);
            }

            Debug.Assert((result && statusCode != 0) || !result, $"Inconsistent Result {result} and StatusCode {(int)statusCode}");
            return new Report(result, (int)statusCode, errorMsg);
        }
        #endregion
    }
}
