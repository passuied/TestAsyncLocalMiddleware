﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Common.Logging.Configuration
{
    /// <summary>
    /// Helper class for working with NameValueCollection
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class NameValueCollectionHelper
    {
        /// <summary>
        /// Convert a <see cref="System.Collections.Specialized.NameValueCollection"/> into the corresponding 
        /// common logging equivalent <see cref="Common.Logging.Configuration.NameValueCollection"/>
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        public static NameValueCollection ToCommonLoggingCollection(System.Collections.Specialized.NameValueCollection properties)
        {
            var result = new NameValueCollection();
            foreach (var key in properties.AllKeys)
                result.Add(key, properties.Get(key));
            return result;
        }
    }
}
