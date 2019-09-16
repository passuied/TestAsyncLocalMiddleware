using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestAsyncLocalMiddleware.Infrastructure
{
    public class SecurityTokenAccessor : ISecurityTokenAccessor
    {
        private readonly AsyncLocal<ContextHolder> _asyncLocalDictionary = new AsyncLocal<ContextHolder>();

        public IDictionary<string, object> ContextItems
        {
            get
            {
                return _asyncLocalDictionary.Value?.Context;
            }
            set
            {
                var holder = _asyncLocalDictionary.Value;
                if (holder != null)
                {
                    // Clear current HttpContext trapped in the AsyncLocals, as its done.
                    holder.Context = null;
                }

                if (value != null)
                {
                    // Use an object indirection to hold the dictionary in the AsyncLocal,
                    // so it can be cleared in all ExecutionContexts when its cleared.
                    _asyncLocalDictionary.Value = new ContextHolder { Context = value };
                }
            }
        }

        

    }

    public class ContextHolder
    {
        public IDictionary<string, Object> Context;
    }
}
