using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace User.Identity
{
    public class IdentityConfig
    {
        public static IEnumerable<Client> GetClient()
        {
            return new List<Client>()
            {

                 new Client(){
                    ClientId="Android",
                    ClientSecrets={new Secret("secret".Sha256()) },
                    RefreshTokenExpiration=TokenExpiration.Sliding,
                    AllowOfflineAccess=true,
                    RequireClientSecret=false,
                    AllowedGrantTypes=new List<string> {"sms_auth_code"},
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AllowedScopes=new List<string>{
                        "userapi",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                    }
                }
            };
        }

        public static IEnumerable<ApiResource> GetResources()
        {
            return new List<ApiResource>()
            {
                new ApiResource("userapi","user service")
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
    }
}
