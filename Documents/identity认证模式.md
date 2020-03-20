## 客户端模式ClientCredentials

![1583743874263](C:\Users\王银行\AppData\Roaming\Typora\typora-user-images\1583743874263.png)

AllowedGrantTypes =GrantTypes.ClientCredentials,

bod入参：

client_id:AutoSchedule
client_secret:AutoScheduleSecret
grant_type:client_credentials
scope:UserApi



## 密码模式 ResourceOwnerPassword

入参:client_id:AutoSchedulePassword
client_secret:AutoScheduleSecret
grant_type:password
scope:UserApi
username:apiUser
password:apiUserPassword

返回：

{

​    "access_token": "",

​    "expires_in": 3600,

​    "token_type": "Bearer",

​    "scope": "UserApi"

}

## 隐藏模式 即用户在服务端登录 返回信息给xx

根据服务端地址拼url:

http://localhost:20000/connect/authorize?client_id=AutoScheduleImpl&redirect_uri=http://localhost:20000/Auth/Index&response_type=token&scope=UserApi

后端配置：



 IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            var RedirectUri = configuration.GetSection("RedirectUris").GetChildren();
            List<string> RedirectUris = new List<string>();
            foreach (var item in RedirectUri)
            {
                RedirectUris.Add(item.Value);
            }
            

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

隐藏模式解决了客户端模式用户身份验证和授权的问题，也解决了密码模式面临的用户密码暴露的问题，适应于全前端没有后端的第三方应用。但由于token携带在url中，安全性方面不能保证。

## 授权码模式

  授权码模式隐藏码模式最大不同是授权码模式不直接返回token，而是先返回一个授权码，然后再根据这个授权码去请求token。这比隐藏模式更为安全。从应用场景上来区分的话，隐藏模式适应于全前端的应用，授权码模式适用于有后端的应用，因为客户端根据授权码去请求token时是需要把客户端密码转进来的，为了避免客户端密码被暴露，所以请求token这个过程需要放在后台。**

拼url地址:

http://localhost:20000/connect/authorize?client_id=AutoScheduleCode&redirect_uri=http://localhost:20000/Auth/Index&response_type=code&scope=UserApi

 clients.Add(new Client
            {
                ClientId = "AutoScheduleCode",
                ClientName = "Oauth2.0授权码客户端",
                ClientSecrets = { new Secret("AutoScheduleSecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = RedirectUris,
                AllowedScopes = { "UserApi" }
                 });

新建一个支持授权码模式的客户端，请求token时需要客户端密码，需要设置clientSecret

![1584688079391](C:\Users\王银行\AppData\Roaming\Typora\typora-user-images\1584688079391.png)



## 混合模式

http://localhost:20000/connect/authorize?client_id=AutoScheduleHybrid&redirect_uri=http://localhost:20000/Auth/Index&response_type=code token id_token&scope=UserApi openid FamilyInfo&nonce=123&response_mode=fragment




               

     public static IEnumerable<IdentityResource> GetIdentityResources()
            {
                //可以返回的claims
                List<IdentityResource> IdentityResources = new List<IdentityResource>();
                List<string> familyInfo = new List<string>{ "lol", "权限模块"};
                IdentityResources.Add(
                    new IdentityResource ( "FamilyInfo", "其他信息", familyInfo ));
            IdentityResources.Add(new IdentityResources.OpenId());
            IdentityResources.Add(new IdentityResources.Profile());
            
            return IdentityResources;
        }
        new TestUser
                    {
                        SubjectId = "1",
                        Username = "apiUser",
                        Password = "apiUserPassword",
                        Claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Role, "admin"),
                            new Claim(ClaimTypes.Name, "王振"),
                             new Claim("lol", "峡谷之巅最强压缩"),
                            new Claim("权限模块", "OAth2.0")
                        }
                    }
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
                    "FamilyInfo"
                },
                AllowOfflineAccess = true,
                AllowAccessTokensViaBrowser = true

});