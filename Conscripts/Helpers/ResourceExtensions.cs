using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.ApplicationModel.Resources;

namespace Conscripts.Helpers
{
    public static class ResourceExtensions
    {
        private static readonly ResourceLoader _resourceLoader = new();

        public static string GetLocalized(this string resourceKey) => _resourceLoader.GetString(resourceKey);
    }
}
