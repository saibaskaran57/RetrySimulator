// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Default ASP.NET Core boilerplate code", Scope = "type", Target = "~T:Server.Program")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Default ASP.NET Core boilerplate code", Scope = "member", Target = "~M:Server.Startup.ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection)")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Default ASP.NET Core boilerplate code", Scope = "member", Target = "~M:Server.Startup.Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder,Microsoft.AspNetCore.Hosting.IWebHostEnvironment)")]
[assembly: SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Default ASP.NET Core boilerplate code", Scope = "type", Target = "~T:Server.Program")]