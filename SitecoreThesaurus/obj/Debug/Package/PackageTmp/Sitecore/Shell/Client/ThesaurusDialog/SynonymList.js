define(["sitecore"], function (Sitecore) {
  var model = Sitecore.Definitions.Models.ControlModel.extend({
    initialize: function (options) {
        this._super();
        this.set("synonyms", null);
        this.set("rawTerm", "");
        this.set("cleanTerm", "");
    }
  });

  var view = Sitecore.Definitions.Views.ControlView.extend({
    initialize: function (options) {
        this._super();

    },
    applySelectedValue: function (selectedVal) {
        var rawTerm = this.model.get("rawTerm");
        var cleanTerm = this.model.get("cleanTerm");
        var returnVal = rawTerm.replace(cleanTerm, selectedVal);

        var radWindow;

        if (window.radWindow)
            radWindow = window.radWindow;
        else if (window.frameElement && window.frameElement.radWindow)
            radWindow = window.frameElement.radWindow;
        else
            window.close();

        radWindow.Close(returnVal);
    }
  });

  Sitecore.Factories.createComponent("SynonymList", model, view, ".sc-SynonymList");
});
