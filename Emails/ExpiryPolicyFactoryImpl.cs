using Apache.Ignite.Core.Cache.Expiry;
using Apache.Ignite.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails
{
    public class ExpiryPolicyFactoryImpl : IFactory<IExpiryPolicy>
    {
        public IExpiryPolicy CreateInstance()
        {
            //Set cache TTL to 20 mins, same as SessionState TTL
            return new ExpiryPolicy(TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(20),
                TimeSpan.FromMinutes(20));
        }
    }
}
