#pragma warning disable IDE0005
using System.Reflection; // Required by cake attributes
#pragma warning restore IDE0005
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cake.GitHub.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

// Note: other attributes are set by Cake