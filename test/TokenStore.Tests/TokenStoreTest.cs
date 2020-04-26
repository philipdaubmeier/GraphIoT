using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using PhilipDaubmeier.TokenStore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.TokenStore.Tests
{
    public class TokenStoreTest
    {
        [Fact]
        public async void TestTokenStoreSave()
        {
            var db = GetMockDbContext();
            var tokenStore = new TokenStore<TokenStoreTest>(db, GetOptions());

            await tokenStore.UpdateToken("accessToken", new DateTime(2019, 1, 1, 12, 0, 0), "refreshToken");

            Mock.Get(db).Verify(d => d.SaveChangesAsync(default), Times.Once);

            var databaseRawData = db.AuthDataSet.ToList();
            var writtenAccessToken = databaseRawData.Where(x => x.AuthDataId == "unittest.access_token").Select(x => x.DataContent).SingleOrDefault();
            var writtenAccessTokenExpiry = databaseRawData.Where(x => x.AuthDataId == "unittest.access_token_expiry").Select(x => x.DataContent).SingleOrDefault();
            var writtenRefreshToken = databaseRawData.Where(x => x.AuthDataId == "unittest.refresh_token").Select(x => x.DataContent).SingleOrDefault();

            Assert.Equal("accessToken", writtenAccessToken);
            Assert.Equal("636819408000000000", writtenAccessTokenExpiry);
            Assert.Equal("refreshToken", writtenRefreshToken);
        }

        [Fact]
        public void TestTokenStoreLoad()
        {
            var db = GetMockDbContext(new List<AuthData>()
            {
                new AuthData() { AuthDataId = "unittest.access_token", DataContent = "accessToken" },
                new AuthData() { AuthDataId = "unittest.access_token_expiry", DataContent = "636819408000000000" },
                new AuthData() { AuthDataId = "unittest.refresh_token", DataContent = "refreshToken" }
            });
            var tokenStore = new TokenStore<TokenStoreTest>(db, GetOptions());

            Assert.Equal("accessToken", tokenStore.AccessToken);
            Assert.Equal(new DateTime(2019, 1, 1, 12, 0, 0), tokenStore.AccessTokenExpiry);
            Assert.Equal("refreshToken", tokenStore.RefreshToken);
        }

        [Fact]
        public async void TestTokenStoreSaveLoad()
        {
            var db = GetMockDbContext();
            var tokenStore = new TokenStore<TokenStoreTest>(db, GetOptions());

            Assert.Null(tokenStore.AccessToken);
            Assert.Equal(DateTime.MinValue, tokenStore.AccessTokenExpiry);
            Assert.Null(tokenStore.RefreshToken);

            await tokenStore.UpdateToken("accessToken", new DateTime(2019, 1, 1, 12, 0, 0), "refreshToken");

            Mock.Get(db).Verify(d => d.SaveChangesAsync(default), Times.Once);

            Assert.Equal("accessToken", tokenStore.AccessToken);
            Assert.Equal(new DateTime(2019, 1, 1, 12, 0, 0), tokenStore.AccessTokenExpiry);
            Assert.Equal("refreshToken", tokenStore.RefreshToken);
        }

        [Fact]
        public async void TestIsAccessTokenValid()
        {
            var db = GetMockDbContext();
            var tokenStore = new TokenStore<TokenStoreTest>(db, GetOptions());

            // should be false if no token is present yet
            Assert.False(tokenStore.IsAccessTokenValid());

            await tokenStore.UpdateToken("accessToken", DateTime.Now.AddMinutes(1), "refreshToken");

            // should be true if token expiry is in the future
            Assert.True(tokenStore.IsAccessTokenValid());

            await tokenStore.UpdateToken("accessToken", DateTime.Now, "refreshToken");

            // should be false if token expiry is less then or equal to now
            Assert.False(tokenStore.IsAccessTokenValid());
        }

        private IOptions<TokenStoreConfig> GetOptions()
        {
            return Options.Create(new TokenStoreConfig()
            {
                ClassNameMapping = new Dictionary<string, string>()
                {
                    { "TokenStoreTest", "unittest" }
                }
            });
        }

        private ITokenStoreDbContext GetMockDbContext(List<AuthData>? authList = null)
        {
            var db = Mock.Of<ITokenStoreDbContext>(d => d.SaveChangesAsync(default) == Task.FromResult(1));
            db.AuthDataSet = GetQueryableMockDbSet(authList ?? new List<AuthData>());
            return db;
        }

        private DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet.Object;
        }
    }
}