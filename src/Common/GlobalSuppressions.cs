// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "Documentation with urls does not require period.", Scope = "type", Target = "~T:Common.PollyRetryHandler")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "NewtonJson requires to deserialize member", Scope = "member", Target = "~P:Common.Models.RequestOption.Headers")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "NewtonJson requires to deserialize member", Scope = "member", Target = "~P:Common.Models.RequestOption.ContentHeaders")]
[assembly: SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "ISerializable not supported not cross platforms", Scope = "type", Target = "~T:Common.TooManyRequestException")]