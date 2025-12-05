[![NuGet Gallery](https://img.shields.io/badge/NuGet%20Gallery-enbrea.apikey-blue.svg)](https://www.nuget.org/packages/Enbrea.ApiKey/)
![GitHub](https://img.shields.io/github/license/enbrea/enbrea.apikey)

# Enbrea.ApiKey

A .NET library for supporting API key–based access control in ASP.NET Core applications.

+ Supports .NET 10, .NET 9 and .NET 8
+ Integrates easily into ASP.NET Core using `services.AddApiKeyValidation(...)`.
+ Provides attribute-based protection using `[ApiKey(typeof(MyApiKeyPolicyProvider))]` to secure controllers or actions.
+ Offers configurable key extraction through Authorization headers (e.g. `ApiKey`), custom headers (e.g. `X-API-KEY`), and query parameters.
+ Supports customizable policies that define allowed API keys, IP ranges (CIDR), and local-only access.
+ Includes built-in error handling that returns proper HTTP status codes or [RFC 9457 Problem Details](https://datatracker.ietf.org/doc/html/rfc9457) responses.
+ Uses an extensible design so you can override or replace extractors, policies, and error factories through dependency injection.

## Installation

```
dotnet add package Enbrea.ApiKey
```

## Getting started

See [GitHub wiki](https://github.com/enbrea/enbrea.apikey/wiki).

## Can I help?

Yes, that would be much appreciated. The best way to help is to post a response via the Issue Tracker and/or submit a Pull Request.
