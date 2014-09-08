using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XperiCode.CacheTempData.Tests
{
    [TestClass]
    public class CacheTempDataProviderTests
    {

        [TestMethod]
        public void Should_Save_TempData_In_Cache_With_New_SessionId_CacheKey()
        {
            var cacheMock = new Mock<ObjectCache>();
            var httpRequestCookieCollection = new HttpCookieCollection();
            var httpRequestMock = new Mock<HttpRequestBase>();
            httpRequestMock.SetupGet(r => r.Cookies).Returns(httpRequestCookieCollection);
            var httpResponseCookieCollection = new HttpCookieCollection();
            var httpResponseMock = new Mock<HttpResponseBase>();
            httpResponseMock.SetupGet(r => r.Cookies).Returns(httpResponseCookieCollection);
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.SetupGet(c => c.Request).Returns(httpRequestMock.Object);
            httpContextMock.SetupGet(c => c.Response).Returns(httpResponseMock.Object);
            var provider = new CacheTempDataProvider(httpContextMock.Object, cacheMock.Object);
            var controllerContextMock = new Mock<ControllerContext>();
            var values = new Dictionary<string, object> { { "name", "John Doe" } };

            provider.SaveTempData(controllerContextMock.Object, values);

            cacheMock.Verify(c => c.Set(
                It.Is<string>(s => s.StartsWith("[XperiCode.CacheTempData.SessionId].[")), 
                It.Is<CacheTempDataItem>(i => Convert.ToString(i.Data["name"]) == "John Doe"), 
                null, 
                null), Times.Once);
        }

        [TestMethod]
        public void Should_Save_TempData_In_Cache_With_Existing_SessionId_CacheKey()
        {
            var cacheMock = new Mock<ObjectCache>();
            var httpRequestCookieCollection = new HttpCookieCollection();
            Guid sessionId = Guid.NewGuid();
            httpRequestCookieCollection.Add(new HttpCookie("XperiCode.CacheTempData.SessionId", sessionId.ToString()));
            var httpRequestMock = new Mock<HttpRequestBase>();
            httpRequestMock.SetupGet(r => r.Cookies).Returns(httpRequestCookieCollection);
            var httpResponseCookieCollection = new HttpCookieCollection();
            var httpResponseMock = new Mock<HttpResponseBase>();
            httpResponseMock.SetupGet(r => r.Cookies).Returns(httpResponseCookieCollection);
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.SetupGet(c => c.Request).Returns(httpRequestMock.Object);
            httpContextMock.SetupGet(c => c.Response).Returns(httpResponseMock.Object);
            var provider = new CacheTempDataProvider(httpContextMock.Object, cacheMock.Object);
            var controllerContextMock = new Mock<ControllerContext>();
            var values = new Dictionary<string, object> { { "name", "John Doe" } };

            provider.SaveTempData(controllerContextMock.Object, values);

            cacheMock.Verify(c => c.Set(
                It.Is<string>(s => s == string.Concat("[XperiCode.CacheTempData.SessionId].[", sessionId, "]")), 
                It.Is<CacheTempDataItem>(i => Convert.ToString(i.Data["name"]) == "John Doe"), 
                null, 
                null), Times.Once);
        }

        [TestMethod]
        public void Should_Load_And_Remove_TempData_From_Cache()
        {

            var httpRequestCookieCollection = new HttpCookieCollection();
            Guid sessionId = Guid.NewGuid();
            httpRequestCookieCollection.Add(new HttpCookie("XperiCode.CacheTempData.SessionId", sessionId.ToString()));

            string cacheKey = string.Concat("[XperiCode.CacheTempData.SessionId].[", sessionId, "]");
            var cacheMock = new Mock<ObjectCache>();
            cacheMock.Setup(c => c.Get(cacheKey, null)).Returns(
                new CacheTempDataItem 
                { 
                    Data = new Dictionary<string, object> { { "name", "John Doe" } } 
                });
            
            var httpRequestMock = new Mock<HttpRequestBase>();
            httpRequestMock.SetupGet(r => r.Cookies).Returns(httpRequestCookieCollection);
            var httpResponseCookieCollection = new HttpCookieCollection();
            var httpResponseMock = new Mock<HttpResponseBase>();
            httpResponseMock.SetupGet(r => r.Cookies).Returns(httpResponseCookieCollection);
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.SetupGet(c => c.Request).Returns(httpRequestMock.Object);
            httpContextMock.SetupGet(c => c.Response).Returns(httpResponseMock.Object);
            var provider = new CacheTempDataProvider(httpContextMock.Object, cacheMock.Object);
            var controllerContextMock = new Mock<ControllerContext>();

            var values = provider.LoadTempData(controllerContextMock.Object);

            Assert.IsNotNull(values);
            Assert.IsTrue(Convert.ToString(values["name"]) == "John Doe");

            cacheMock.Verify(c => c.Remove(cacheKey, null), Times.Once);

        }
    }
}
