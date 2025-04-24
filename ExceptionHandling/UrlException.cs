using System;
using System.Xml.Linq;

namespace PodcastAppG19.ExceptionHandling
{
    public class UrlException : Exception
    {
        private XDocument filen;

        public UrlException(XDocument filen)
        {
            this.filen = filen;
        }

        public UrlException(XDocument fil, string additionalMessage)
            : base($"Fel format på URL: {additionalMessage}, vänligen försök igen")
        {
            Fil = fil;
            AdditionalMessage = additionalMessage;
        }

        public XDocument Fil { get; set; }
        public string AdditionalMessage { get; set; }
    }
}
