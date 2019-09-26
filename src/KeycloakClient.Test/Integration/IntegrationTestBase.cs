using System;

namespace KeycloakClient.Test
{
    public class IntegrationTestBase : IDisposable
    {

        protected IntegrationTestBase()
        {
            client = new KeycloakClient(new KeycloakAdminClientOptions
            {
                ClientId = "admin-cli",
                Username = "Admin",
                Password = "Admin",
                Realm = "master",
                Url = new Uri("http://localhost:8080/auth/")
            });
        }

        public void Dispose()
        {
            client = null;
        }

        protected IKeycloakClient client;
    }
}
