using System.IO;
using Payola.Model;
using System;

namespace Payola.Scrapers.Stb
{
    public class DirectoryScraper : ScraperBase
    {
        public DirectoryScraper (PayolaContext ctx, string path)
            : base (ctx, path)
        {

        }

        protected override string GetDataForResourcePath (string path)
        {
            if (!File.Exists (path))
            {
                return null;
            }

            return File.ReadAllText (path);
        }

        protected override string GetResourcePathForCharAndIndex (char c, int i)
        {
            return string.Format ("{0}\\{1}{2}.htm", _path, c, i > 0 ? i.ToString () : String.Empty);
        }

    }
}
