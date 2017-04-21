using System.Collections.Generic;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.Packages
{
    public static class PackagesAliases
    {
        [CakeMethodAlias]
        public static IEnumerable<NuSpecDependency> ReadPackages(this ICakeContext context, FilePath pathToPackagesConfig)
        {
            return PackagesConfigReader.GetDependencies(pathToPackagesConfig.MakeAbsolute(context.Environment).FullPath);
        }
    }
}