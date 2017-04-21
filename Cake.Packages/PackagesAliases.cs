using System.Collections.Generic;
using System.Linq;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.Packages
{
    public static class PackagesAliases
    {
        [CakeMethodAlias]
        public static ICollection<NuSpecDependency> ReadPackages(this ICakeContext context, FilePath pathToPackagesConfig)
        {
            return PackagesConfigReader.GetDependencies(pathToPackagesConfig.MakeAbsolute(context.Environment).FullPath).ToList();
        }
    }
}