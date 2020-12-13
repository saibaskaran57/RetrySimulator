// <copyright file="GlobalSuppressions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "The program is written in console", Scope = "member", Target = "~M:Client.Program.CreateRetryOption~System.ValueTuple{System.String,System.Int32}")]
[assembly: SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Arguments is not being used and will remove suppression when exist", Scope = "member", Target = "~M:Client.Program.Main(System.String[])")]
[assembly: SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "The program is written in console", Scope = "type", Target = "~T:Client.Program")]
[assembly: SuppressMessage("Major Bug", "S3168:\"async\" methods should not return \"void\"", Justification = "Polly does not support async Task", Scope = "member", Target = "~M:Client.Program.HandleRetry(Polly.DelegateResult{System.Net.Http.HttpResponseMessage},System.TimeSpan,Polly.Context)")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "The program is written in console", Scope = "member", Target = "~M:Client.Program.Main(System.String[])")]
[assembly: SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Http client can be reused and should not be disposed", Scope = "type", Target = "~T:Client.ServiceClient")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "The program is written in console", Scope = "member", Target = "~M:Client.Program.CreateRequest(System.String,System.String)~System.Threading.Tasks.Task{System.Net.Http.HttpResponseMessage}")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "The program is written in console", Scope = "member", Target = "~M:Client.ServiceClient.SendAsync(Common.Models.RequestOption)~System.Threading.Tasks.Task{System.Net.Http.HttpResponseMessage}")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "The program is written in console", Scope = "member", Target = "~M:Client.ServiceClient.ExecuteAsync(Common.Models.RequestOption)~System.Threading.Tasks.Task{System.Net.Http.HttpResponseMessage}")]