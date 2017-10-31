using System;
using System.Configuration;

namespace FJW.Repository.Expression2Sql
{
    public class ClientProviderFactory
    {
        public virtual IClientProvider CreateProvider(string key)
        {
            var connSetting = ConfigurationManager.ConnectionStrings[key];
            
            if (connSetting.ProviderName.Equals("System.Data.SqlClient", StringComparison.CurrentCultureIgnoreCase))
            {
                return new ClientProvider() {  ConnectionString = connSetting.ConnectionString};
            }
            throw new NotSupportedException("can not supported ProviderName:" + connSetting.ProviderName);
        }
    }
}
