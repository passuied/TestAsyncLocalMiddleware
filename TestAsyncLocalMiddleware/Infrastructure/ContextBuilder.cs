using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAsyncLocalMiddleware.Infrastructure
{
    internal class ContextBuilder
    {
        public const string CorpIdHeader = "x-corp-id";
        public const string UserIdHeader = "x-user-id";

        /// <summary>
        /// Build context based on ServiceContextAccessor, security token and header dictionary.
        /// </summary>
        /// <typeparam name="T">Context type</typeparam>
        /// <param name="securityTokenAccessor">Security token accessor</param>
        /// <param name="headers">Header dictionary</param>
        /// <param name="serviceContextAccessor"> Service Context Accessor instance </param>
        /// <returns>Context</returns>
        public static DefaultServiceContext BuildContext(ISecurityTokenAccessor securityTokenAccessor,
            IDictionary<string, IEnumerable<string>> headers
            )
        {

            var contextCollection = new Dictionary<string, IEnumerable<string>>();
            if (securityTokenAccessor.ContextItems != null)
            {
                foreach (var kv in securityTokenAccessor.ContextItems)
                {
                    contextCollection.Add(kv.Key, new[] { kv.Value.ToString() });
                }
            }

            if (headers != null)
            {
                foreach (var h in headers)
                {
                    if (!contextCollection.ContainsKey(h.Key))
                        contextCollection.Add(h.Key, h.Value);
                }
            }

            return contextCollection.Keys.Count > 0
                ? Deserialize(contextCollection)
                : new DefaultServiceContext();
        }

        private static DefaultServiceContext Deserialize(Dictionary<string, IEnumerable<string>> contextCollection)
        {
            var context = new DefaultServiceContext();

            if (contextCollection.TryGetValue(CorpIdHeader, out var corpIdHeaders))
            {
                context.CorpId = corpIdHeaders.First();
            }

            if (contextCollection.TryGetValue(UserIdHeader, out var userIdHeaders))
            {
                context.UserId = Convert.ToInt32(userIdHeaders.First());
            }

            return context;
        }
    }

}
