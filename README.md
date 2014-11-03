# CacheTempData

`CacheTempData` is an ObjectCache-based TempData provider for ASP.NET MVC. The default `ITempDataProvider` implementation included in ASP.NET MVC uses `SessionState`. One of the problems with `SessionState` is that [it blocks concurrent ajax requests](http://johnculviner.com/asp-net-concurrent-ajax-requests-and-session-state-blocking/). `CacheTempData` solves this problem by storing `TempData` in `MemoryCache` (or any other `ObjectCache` implementation).

[![Build status](http://img.shields.io/appveyor/ci/mwijnands/cachetempdata.svg?style=flat)](https://ci.appveyor.com/project/mwijnands/cachetempdata) [![NuGet version](http://img.shields.io/nuget/v/XperiCode.CacheTempData.svg?style=flat)](https://www.nuget.org/packages/XperiCode.CacheTempData)

## Installation

The `CacheTempData` package is available at [NuGet](https://www.nuget.org/packages/XperiCode.CacheTempData). To install `CacheTempData`, run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console):

> ### Install-Package XperiCode.CacheTempData

## Documentation

There are multiple ways to use an alternative `ITempDataProvider` implementation in your ASP.NET MVC application. Jonathan George wrote a [nice article on the subject](http://consultingblogs.emc.com/jonathangeorge/archive/2009/10/14/using-an-alternative-itempdataprovider-implementation-in-asp-net-mvc.aspx). You can also checkout the [Sample application on GitHub](https://github.com/mwijnands/CacheTempData/tree/master/CacheTempData.Sample).

## Release notes

#### v0.9.1

- Removed constructor dependency on `HttpContextBase`
- Changed Microsoft.AspNet.Mvc dependencies to 5.0.0 (5.2.2 was unnecessary)
- `TempData` is now removed from cache when no values are saved

#### v0.9.0

- Initial release

## Collaboration

Please report issues if you find any. Pull requests are welcome for documentation and code.
