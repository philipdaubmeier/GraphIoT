[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.TokenStore.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.TokenStore/)

# TokenStore

TokenStore is a .NET Core class library providing a means to store and retrieve OAuth access and refresh tokens to/from a configurable entity framework database.

## NuGet

```powershell
PM> Install-Package PhilipDaubmeier.TokenStore
```

## Usage

If you already have a database context, include the interface `ITokenStoreDbContext`, or create a new `DbContext` class that derives from it:

```csharp
public class MyDbContext : DbContext, ITokenStoreDbContext
{
    public DbSet<AuthData> AuthDataSet { get; set; }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    { }
}
```

Add a config section in your `appsettings.json` and define how the key in the database should be named for your class (can be any string you like):

```json
"TokenStoreConfig": {
  "ClassNameMapping": {
    "MyClassUsingTheTokenStore": "my_class_db_key"
  }
}
```

Set up the TokenStore for dependency injection in your `Startup.cs`, configuring the entity framework database context:

```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public IServiceProvider ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.ConfigureTokenStore(configuration.GetSection("TokenStoreConfig"));
    services.AddTokenStoreDbContext<MyDbContext>(opt => opt.UseSqlServer("<my_connection_str>"));

    services.AddTokenStore<MyClassUsingTheTokenStore>();
}
```

You can then use the TokenStore by getting it dependency injected into a class where you need it:

```csharp
public class MyClassUsingTheTokenStore
{
    private readonly TokenStore<MyClassUsingTheTokenStore> tokenStore;

    public MyClassUsingTheTokenStore(TokenStore<MyClassUsingTheTokenStore> tokenStore)
    {
        this.tokenStore = tokenStore;
    }

    public async Task DemoStoreSomeToken()
    {
        await tokenStore.UpdateToken("demo access token", DateTime.Now, "demo refresh token");
    }

    public void DemoLoadTheToken()
    {
        if (tokenStore.IsAccessTokenValid())
            Console.WriteLine($"Access Token is valid: ${tokenStore.AccessToken}");

        Console.WriteLine($"AT invalid, refresh token: ${tokenStore.RefreshToken}");
    }
}
```

You can also have a look at classes where TokenStore is used in GraphIoT, like [`PersistingDigitalstromAuth`](../GraphIoT.Digitalstrom/Config/PersistingDigitalstromAuth.cs).

## Platform Support

CompactTimeSeries is compiled for .NET Core 2.2.

## License

The MIT License (MIT)

Copyright (c) 2019 Philip Daubmeier

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
