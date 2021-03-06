﻿using System;
using System.Threading;

namespace Microsoft.Azure.DataLake.Store.RetryPolicies
{
    /// <summary>
    /// Exponential retry policy.
    /// Does retries for following: 
    /// For 5xx http status codes except 501 and 505 
    /// For 401, 408 and 429 status codes
    /// Any other unhandled exception from web- Request timeout for client, etc
    /// </summary>
    public class ExponentialRetryPolicy : RetryPolicy
    {
        /// <summary>
        /// Tracks the current number of retries
        /// </summary>
        private int NumberOfRetries { get; set; }
        /// <summary>
        /// Maximum number of retries
        /// </summary>
        private int MaxRetries { get; }
        /// <summary>
        /// Factor by which we will increase the interval
        /// </summary>
        private int ExponentialFactor { get; }
        /// <summary>
        /// Wait time
        /// </summary>
        private int ExponentialInterval { get; set; }
        /// <summary>
        /// Default settings of Exponential retry policies
        /// </summary>
        public ExponentialRetryPolicy()
        {
            NumberOfRetries = 0;
            MaxRetries = 4;
            ExponentialFactor = 4;
            ExponentialInterval = DefaultRetryInterval;
        }
        /// <summary>
        /// Exponential retry policies with specified maximum retries and interval
        /// </summary>
        /// <param name="maxRetries">Maximum retries</param>
        /// <param name="interval">Exponential time interval</param>
        public ExponentialRetryPolicy(int maxRetries, int interval)
        {
            NumberOfRetries = 0;
            MaxRetries = maxRetries;
            ExponentialFactor = 4;
            ExponentialInterval = interval;
        }

        /// <summary>
        /// Determines whether to retry exponentially. 
        /// </summary>
        /// <param name="httpCode">Http status code</param>
        /// <param name="ex">Last exception that we saw during Httprequest</param>
        /// <returns>True if it should be retried else false</returns>
        public override bool ShouldRetry(int httpCode, Exception ex)
        {
            if (ShouldRetryBasedOnHttpOutput(httpCode,ex))
            {
                if (NumberOfRetries < MaxRetries)
                {
                    Thread.Sleep(ExponentialInterval);
                    ExponentialInterval = ExponentialFactor * ExponentialInterval;
                    NumberOfRetries++;
                    return true;
                }
            }
            return false;
        }
    }
}
