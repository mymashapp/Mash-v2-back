global using Aimo.Core;
global using Aimo.Domain;

#region Fody.NullGuard
using NullGuard;
[assembly: NullGuard(ValidationFlags.All)]
#endregion