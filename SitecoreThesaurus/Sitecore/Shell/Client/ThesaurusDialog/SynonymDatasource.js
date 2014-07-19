define(["sitecore"], function (Sitecore) {
  var model = Sitecore.Definitions.Models.ControlModel.extend({
    initialize: function (options) {
        this._super();

        this.set("cleanTerm", "");
        this.set("rawTerm", "");
        this.set("synonyms", null);
        this.set("synonymCount", 0);
        this.set("synonymCountMessage", "");
        this.set("hasSynonyms", false);
        this.set("hasNoSynonyms", true);
        this.set("lookupFailed", false);
        this.set("failureMessage", "");
        this.set("lookupInProgress", true);

        this.on("change:synonyms", this.updateSynonymsMetaData, this);
     
        // getting the term from the url.
        var href = window.location.href;
        var hashes = href.slice(href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');

            if (hash[0].toLowerCase() == "term") {
                this.set("rawTerm", unescape(hash[1]));
            }
        }

        // cleaning up the term so we can perform the lookup
        var cleanedTerm = this.get("rawTerm").split(" ")[0];
        var trailingCrapPattern = /^(.*)(\W|_|[0-9])$/;
        while (trailingCrapPattern.test(cleanedTerm) && cleanedTerm.length > 0) {
            cleanedTerm = cleanedTerm.substring(0, cleanedTerm.length - 1);
        }
        var leadingCrapPattern = /^(\W|_|[0-9])(.*)$/;
        while (leadingCrapPattern.test(cleanedTerm) && cleanedTerm.length > 0) {
            cleanedTerm = cleanedTerm.substring(1, cleanedTerm.length);
        }
        this.set("cleanTerm", cleanedTerm);

        // Do the lookup
        if (cleanedTerm && cleanedTerm.length > 0) {
            $.ajax({
                url: "/api/sitecore/Thesaurus/Lookup",
                type: "POST",
                data: { term: cleanedTerm },
                context: this,
                success: function (data) {
                    if ( data && data.LookupSuccess) {
                        this.set("synonyms", data.SynonymResults);
                    }
                    else
                        this.setFailureState(data.FailureMessage);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    this.setFailureState(errorThrown);
                }
            });
        }

        this.set("lookupInProgress", false);
    },
    updateSynonymsMetaData: function ()
    {

        var synonyms = this.get("synonyms");
        var count = 0;
        if (synonyms)
        {
            count = this.get("synonyms").length;
        }
  
        this.set("synonymCount", count);
        this.set("hasSynonyms", (count > 0));
        this.set("hasNoSynonyms", (count <= 0));
        var message;

        var resultsWord = (count == 1) ? "result" : "results";
        message = count + " " + resultsWord + " for '" + this.get("cleanTerm") + "'";

        this.set("synonymCountMessage", message);

    },
    setFailureState: function (message)
    {
        this.set("lookupFailed", true);
        this.set("failureMessage", message);
        this.set("hasSynonyms", false);
        this.set("hasNoSynonyms", true);
    }
  });

  var view = Sitecore.Definitions.Views.ControlView.extend({
    initialize: function (options) {
      this._super();
    }
  });

  Sitecore.Factories.createComponent("SynonymDatasource", model, view, ".sc-SynonymDatasource");
});
