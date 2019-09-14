using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestAsyncLocalMiddleware.Infrastructure
{
    public class SecurityTokenAccessor : ISecurityTokenAccessor
    {
        private readonly AsyncLocal<IDictionary<string, object>> _asyncLocalDictionary = new AsyncLocal<IDictionary<string, object>>();

        public IDictionary<string, object> ContextItems
        {
            get
            {
                var result = GetThreadLocal();
                if (result == null)
                {
                    result = new Dictionary<string, object>();
                    SetThreadLocal(result);
                }
                return result;
            }
        }

        private IDictionary<string, object> GetThreadLocal()
        {
            return _asyncLocalDictionary.Value;
        }

        private void SetThreadLocal(IDictionary<string, object> contextItems)
        {
            _asyncLocalDictionary.Value = contextItems;
        }

    }
}
