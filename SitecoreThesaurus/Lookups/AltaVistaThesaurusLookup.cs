using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SitecoreThesaurus.Interfaces;
using SitecoreThesaurus.Entities;
using System.Net;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace SitecoreThesaurus.Lookups
{
    public class AltaVistaThesaurusLookup : ISynonymLookup
    {
        public String ConfigItemID { get; set; }

        public SynonymLookupResult GetSynonyms(string str)
        {
            if (String.IsNullOrEmpty(ConfigItemID))
                return GetFailureResult("Configuration item has not been set.");
            
            var db = Sitecore.Configuration.Factory.GetDatabase("master");
            var configItem = db.GetItem(ConfigItemID);

            if (configItem == null)
                return GetFailureResult("Configuration item does not exist");

            if (configItem.TemplateID.ToString() != "{F3ADABA0-E6B9-490B-8515-9ED471B5F717}")
                return GetFailureResult("Configuration item does not have the correct template.");

            int timeout = 0;
            Int32.TryParse(configItem["Lookup Timeout Milliseconds"], out timeout);

            if (timeout <= 0)
                return GetFailureResult("Lookup timeout value is invalid.");

            if (String.IsNullOrEmpty(configItem["Altavista Thesaurus API Key"]))
                return GetFailureResult("Lookup API key is not set");

            HttpWebResponse resp;

            try
            {
                var url = @"http://thesaurus.altervista.org/thesaurus/v1?word=" + HttpUtility.UrlEncode(str) + "&language=en_US&key=" + configItem["Altavista Thesaurus API Key"];
                WebRequest req = WebRequest.Create(url);
                req.Method = "GET";
                req.Timeout = timeout;
                resp = req.GetResponse() as HttpWebResponse;

                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    return GetFailureResult("Server response: " + resp.StatusCode.ToString());
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    Stream stream = ex.Response.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    String content = "";
                    using (StreamReader reader = new StreamReader(stream, encode))
                    {
                        content = reader.ReadToEnd();
                    }

                    if (content.Contains("no result found"))
                        return GetFailureResult("No results found for '" + str + "'");

                    return GetFailureResult("Web Service Error: " + content, ex);
                }

                if (ex.Status == WebExceptionStatus.Timeout)
                    return GetFailureResult("The lookup service failed to respond in time.", ex);
                else
                    return GetFailureResult("Error while looking up word", ex);
            }
            catch (Exception ex)
            {
                return GetFailureResult("An error occured while looking up the word", ex);
            }

            SynonymLookupResult result = new SynonymLookupResult();
            result.SynonymResults = GetSynonymsFromServiceResponse(resp);
            result.LookupSuccess = true;
            return result;
        }

        private List<SynonymResult> GetSynonymsFromServiceResponse(HttpWebResponse resp)
        {
            using (Stream respStream = resp.GetResponseStream())
            {
                var synonymResults = new List<SynonymResult>();
                XDocument doc = XDocument.Load(respStream);
                var synonymGroups = doc.Descendants("synonyms");

                foreach (XElement element in synonymGroups)
                {
                    var splitWords = element.Value.Split('|');
                    foreach (String rawResult in splitWords)
                    {
                        var correctedValue = Regex.Replace(rawResult, "\\(.*?\\)", "").Trim();

                        var synonymResult = new SynonymResult()
                                                {
                                                    Description = rawResult,
                                                    Value = correctedValue
                                                };

                        synonymResults.Add(synonymResult);
                    }
                }

                return synonymResults;
            }
        }

        private SynonymLookupResult GetFailureResult(String message)
        {
            SynonymLookupResult result = new SynonymLookupResult();
            result.LookupSuccess = false;
            result.FailureMessage = message;
            return result;
        }

        private SynonymLookupResult GetFailureResult(String message, Exception ex)
        {
            SynonymLookupResult result = GetFailureResult(message);
            result.Exception = ex;
            return result;
        }

    }
}