using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using IdentityServer4;

namespace Identity4Center.BaseConfig
{
    public class Config
    { 
       
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "apiUser",
                    Password = "apiUserPassword",
                    Claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim(ClaimTypes.Name, "王银行"),
                        new Claim("lol", "峡谷之巅最强压缩"),
                        new Claim("权限模块", "OAth2.0")
                    }
                }
                //,
                // new TestUser
                //{
                //    SubjectId = "2",
                //    Username = "apiGuest",
                //    Password = "apiGuestPassword",
                //    Claims = new List<Claim>
                //    {
                //        new Claim(ClaimTypes.Role, "user"),
                //         new Claim("权限模块", "OAth2.0")
                //    }
                //}
            };
        }

      

        public static IEnumerable<ApiResource> GetApiResources()
        {
            List<ApiResource> apiLists = new List<ApiResource>();

            //必须需要的权限
            apiLists.Add(
                new ApiResource(
                    "UserApi",
                "访问UserApi的权限",
                new List<string>(){ ClaimTypes.Role}
            ));
            return apiLists;
        }


        #region 基础配置
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            //可以返回的claims
            List<IdentityResource> IdentityResources = new List<IdentityResource>();
            List<string> familyInfo = new List<string>{ "lol", "权限模块"};
            IdentityResources.Add(
                new IdentityResource ( "FamilyInfo", "其他信息", familyInfo )
               
             );
            IdentityResources.Add(new IdentityResources.OpenId());
            IdentityResources.Add(new IdentityResources.Profile());
            
            return IdentityResources;
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            //string allowUrls = getConfig("RedirectUris");
            // JsonConvert.DeserializeObject(allowUrls);
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            var RedirectUri = configuration.GetSection("RedirectUris").GetChildren();
            List<string> RedirectUris = new List<string>();
            foreach (var item in RedirectUri)
            {
                RedirectUris.Add(item.Value);
            }
            

            List<Client> clients = new List<Client>();
            clients.Add(new Client
            {
                ClientId = "AutoSchedule",
                ClientName = "自动传输任务客户端名称",
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AllowOfflineAccess = true,
                RequireClientSecret = false,
                AlwaysIncludeUserClaimsInIdToken = true,
                // AllowedGrantTypes =GrantTypes.ClientCredentials,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                    {
                        new Secret("AutoScheduleSecret".Sha256())
                    },
                AllowedScopes = { "UserApi" }
            });

            clients.Add(new Client
            {
                ClientId = "AutoSchedulePassword",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets =
                    {
                        new Secret("AutoScheduleSecret".Sha256())
                    },
                AllowedScopes = { "UserApi" }
            });

           
            clients.Add(new Client
            {
                ClientId = "AutoScheduleImpl",
                ClientName="Oauth2.0隐藏模式客户端",
                AllowedGrantTypes = GrantTypes.Implicit,
                RedirectUris = RedirectUris,
                AllowedScopes = { "UserApi" },
                //允许将token通过浏览器传递
                AllowAccessTokensViaBrowser = true
            });
            

            //授权码模式
            clients.Add(new Client
            {
                ClientId = "AutoScheduleCode",
                ClientName = "Oauth2.0授权码客户端",
                ClientSecrets = { new Secret("AutoScheduleSecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = RedirectUris,
                AllowedScopes = { "UserApi" }
               
            });

            //混合模式 实现openid

            clients.Add(new Client
            {
                AlwaysIncludeUserClaimsInIdToken = true,
                ClientId = "AutoScheduleHybrid",
                ClientName = "Oauth2.0混合模式",
                ClientSecrets = { new Secret("AutoScheduleSecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Hybrid,
                RedirectUris = RedirectUris,
                AllowedScopes = { 
                    "UserApi",
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "FamilyInfo"
                },
                AllowOfflineAccess = true,
                AllowAccessTokensViaBrowser = true

            });

            return clients;
        } 
        #endregion

    }
}
