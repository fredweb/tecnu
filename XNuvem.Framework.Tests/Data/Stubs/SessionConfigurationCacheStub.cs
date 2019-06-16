using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNuvem.Data.Providers;

namespace XNuvem.Tests.Data.Stubs
{
    public class SessionConfigurationCacheStub : ISessionConfigurationCache
    {
        public SessionConfigurationCacheStub() {

        }

        public NHibernate.Cfg.Configuration GetConfiguration() {
            return null;
        }

        public void SetConfiguration(NHibernate.Cfg.Configuration config) {
            
        }


        public void InvalidateCache() {
            throw new NotImplementedException();
        }
    }
}
