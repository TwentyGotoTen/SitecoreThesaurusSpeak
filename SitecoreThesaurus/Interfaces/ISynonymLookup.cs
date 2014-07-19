using SitecoreThesaurus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitecoreThesaurus.Interfaces
{
    public interface ISynonymLookup
    {
        SynonymLookupResult GetSynonyms(String term);
    }
}