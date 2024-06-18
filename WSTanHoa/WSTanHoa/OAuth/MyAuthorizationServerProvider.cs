using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WSTanHoa.Providers;

namespace WSTanHoa.OAuth
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            CConnection _cDAL_DHN = new CConnection(CGlobalVariable.DHN);
            DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("select * from TRUNGTAMKHACHHANG.dbo.UserService where username='" + context.UserName + "' and password='" + context.Password + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Role, dt.Rows[0]["role"].ToString()));
                identity.AddClaim(new Claim("password", context.Password));
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "Provided username and password are incorrect");
                return;
            }
        }
    }
}