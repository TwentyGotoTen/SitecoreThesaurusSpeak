using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitecoreThesaurus.Entities
{
    public class SynonymLookupResult
    {     
        public IEnumerable<SynonymResult> SynonymResults { get; set; }
        public bool LookupSuccess { get; set; }
        public String FailureMessage { get; set; }
        public Exception Exception { get; set; }
    }
}