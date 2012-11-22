using System;
using System.Data.Entity;
using Payola.Model;

namespace Payola.Scrapers.Stb
{
    class Program
    {

        /// <summary>
        ///     Shared database connection.
        /// </summary>
        static private PayolaContext _dbCtx;


        /// <summary>
        ///     Exits using Environment.Exit(...), but saves the DB changes before it does so.
        /// </summary>
        /// <param name="statusCode">Exit status code.</param>
        static public void Exit (int exitCode)
        {
            if (_dbCtx != null)
            {
                _dbCtx.SaveChanges ();
            }
            Environment.Exit (exitCode);
        }

        /// <summary>
        ///     Initializes database connection.
        /// </summary>
        static private void InitializeDatabaseConnection ()
        {
            Database.SetInitializer<PayolaContext> (new PayolaContextInitializer ());
            _dbCtx = new PayolaContext ();
        }

        /// <summary>
        ///     Prints how to use this utility.
        /// </summary>
        static private void PrintHelp ()
        {
            Console.WriteLine ("Usage:   -h          Show this help.");
            Console.WriteLine ("         -l url      Start scraping from URL.");
            Console.WriteLine ("         -d dir      Start scraping from a local directory.");
        }

        static void Main (string[] args)
        {
            if (args.Length == 0)
            {
                // Automatically use the default address
                InitializeDatabaseConnection();
                LinkScraper linkScraper = new LinkScraper(_dbCtx, "http://Stbezo.info/");
                linkScraper.Scrape();
                Exit(0);
            }

            if (args.Length == 1 && args[0].Equals ("-h"))
            {
                // Show help and exit
                PrintHelp ();
                Exit (0);
            }

            if (args.Length > 2)
            {
                // Wrong arg length
                PrintHelp ();
                Exit (1);
            }

            string method = args[0];
            string path = args[1];

            ScraperBase scraper;
            InitializeDatabaseConnection();

            if (method.Equals ("-l"))
            {
                scraper = new LinkScraper(_dbCtx, path);
            }
            else if (method.Equals ("-d"))
            {
                scraper = new DirectoryScraper(_dbCtx, path);
            }
            else
            {
                PrintHelp ();
                Exit (1);
                return;
            }

            scraper.Scrape();
            Exit (0);

        }
    }
}
