namespace Ex3.API.MiddleWares
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    // This class limits the number of requests [_limit] in a window time [_windowTime] per user [according to API key]
    public class RateLimiterMiddleWare
    {
        private static readonly ConcurrentDictionary<string, Queue<DateTime>> _usersToRequests = new ConcurrentDictionary<string, Queue<DateTime>>();
        private static readonly int _limit = 2;
        private static readonly TimeSpan _windowTime = TimeSpan.FromMinutes(1);
        private readonly RequestDelegate _next;

        public RateLimiterMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        private void RemoveOldRequests(Queue<DateTime> userQueue, DateTime now)
        {
            while (userQueue.Count > 0 && userQueue.Peek() < now - _windowTime)
            {
                userQueue.Dequeue();
            }
        }

        private void AddRequest(Queue<DateTime> userQueue, DateTime requestToAdd)
        {
            userQueue.Enqueue(requestToAdd);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string userId = GetUserIdentifier(context);
            DateTime now = DateTime.UtcNow;
            Queue<DateTime> userQueue = _usersToRequests.GetOrAdd(userId, new Queue<DateTime>());

            RemoveOldRequests(userQueue, now);

            // Case 1 - Adding the request (optional)
            if (userQueue.Count < _limit)
            {
                AddRequest(userQueue, now);
                await _next(context);
            }
            // Case 2 - Responding with Too Many Requests (optional)
            else
            {
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Too many requests, please try again later.");
            }
        }

        private string GetUserIdentifier(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}


