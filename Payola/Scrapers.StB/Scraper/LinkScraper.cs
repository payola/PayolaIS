using System.IO;
using System.Net;
using Payola.Model;
using System;

namespace Payola.Scrapers.Stb
{
    public class LinkScraper : ScraperBase
    {
        public LinkScraper (PayolaContext ctx, string path)
            : base (ctx, path)
        {

        }

        protected override string GetDataForResourcePath (string path)
        {
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create (path);
            if (req == null)
            {
                return null;
            }

            HttpWebResponse response = (HttpWebResponse) req.GetResponse ();
            if (response == null)
            {
                return null;
            }

            // Open data stream:
            Stream stream = response.GetResponseStream ();
            if (stream == null)
            {
                response.Close ();
                return null;
            }

            StreamReader reader = new StreamReader (stream);
            if (reader == null)
            {
                stream.Close ();
                response.Close ();
                return null;
            }

            HttpStatusCode code = response.StatusCode;

            // Close streams
            reader.Close ();
            stream.Close ();
            response.Close ();

            string pageContent = reader.ReadToEnd ();

            if (!code.HasFlag (HttpStatusCode.OK))
            {
                // Error occurred
                return null;
            }

            return pageContent;
        }

        protected override string GetResourcePathForCharAndIndex (char c, int i)
        {
            return string.Format ("{0}/{1}{2}.htm", _path, c, i > 0 ? i.ToString () : String.Empty);
        }

    }
}
