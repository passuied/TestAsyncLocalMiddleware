using Newtonsoft.Json;
using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAsyncLocalMiddleware.Infrastructure.Logging.NLog
{
    [LayoutRenderer("my-layout")]
    public class MyLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Override Append method of renderer
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var msg = new CornerstoneLogMessage
            {
                LoggerName = logEvent.LoggerName,
                Time = logEvent.TimeStamp,
                LogLevel = logEvent.Level.ToString().ToUpperInvariant(),
                Message = logEvent.FormattedMessage,
                CorpId = GetContextValueAsString("corp-id"),
                UserId = GetContextValueAsString("user-id"),
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(msg, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            builder.Append(json);
        }

        /// <summary>
        /// Get Context Value with given key and convert to String. If not found return null
        /// </summary>
        /// <param name="variablesContext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetContextValueAsString(string key)
        {
            if (MappedDiagnosticsLogicalContext.Contains(key))
                return MappedDiagnosticsLogicalContext.Get(key);
            else
                return null;
        }


    }

    internal class CornerstoneLogMessage
    {
        public string LoggerName { get; set; }

        public DateTime Time { get; set; }
        public string LogLevel { get; set; }

        public string Message { get; set; }

        public string CorpId { get; set; }

        public string UserId { get; internal set; }

    }
}
