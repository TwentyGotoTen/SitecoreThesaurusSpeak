using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Web;
using Sitecore.Configuration;
using SitecoreThesaurus.Interfaces;

namespace SitecoreThesaurus.Shell.Client.ThesaurusDialog
{
    public class ThesaurusController : Controller
    {
        public JsonResult Lookup()
        {
            var term = WebUtil.GetFormValue("term");
            var lookup = Factory.CreateObject("Thesaurus", false) as ISynonymLookup;

            if (lookup == null)
            {
                var msg = "Failed to create Lookup object";
                Sitecore.Diagnostics.Log.Error(msg, this);
                return Json(msg);           
            }

            var lookupResult = lookup.GetSynonyms(term);

            return Json(lookupResult);
        }
    }
}