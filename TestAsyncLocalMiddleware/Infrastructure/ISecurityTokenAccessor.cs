using System.Collections.Generic;

namespace TestAsyncLocalMiddleware.Infrastructure
{
    public interface ISecurityTokenAccessor
    {
        IDictionary<string, object> ContextItems
        {
            get;
        }
    }
}