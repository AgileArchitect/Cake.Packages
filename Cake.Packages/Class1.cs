using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.Packages
{
    public static class PackagesAliases
    {
        [CakeMethodAlias]
        public static IEnumerable ReadPackagesConfig(this ICakeContext context, ICakeEnvironment environment, FilePath pathToPackagesConfig)
        {
            return PackagesConfigReader.GetDependencies(pathToPackagesConfig.MakeAbsolute(environment).FullPath);
        }
    }

    public static class PackagesConfigReader
    {
        public static IEnumerable<NuSpecDependency> GetDependencies(string packagesFile)
        {
            if (!File.Exists(packagesFile))
                throw new FileNotFoundException();

            System.Xml.XmlDocument packages = new System.Xml.XmlDocument();
            var lines = File.ReadAllLines(packagesFile);
            var content = string.Join(Environment.NewLine, lines);
            content = content.Replace("utf-8", "utf-16");
            packages.LoadXml(content);
            return packages.DocumentElement.SelectNodes("/packages/package").Cast<System.Xml.XmlNode>().Select(x => new NuSpecDependency { Id = x.Attributes["id"].Value, Version = x.Attributes["version"].Value }).ToList();
        }
    }


}
