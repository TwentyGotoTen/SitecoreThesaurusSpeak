require(["jasmineEnv"], function (jasmineEnv) {
  var setupTests = function () {
    "use strict";

    describe("Given a SynonymList model", function () {
      var component = new Sitecore.Definitions.Models.SynonymList();

      describe("when I create a SynonymList model", function () {
        it("it should have a 'isVisible' property that determines if the SynonymList component is visible or not", function () {
          expect(component.get("isVisible")).toBeDefined();
        });

        it("it should set 'isVisible' to true by default", function () {
          expect(SynonymList.get("isVisible")).toBe(true);
        });

        it("it should have a 'toggle' function that either shows or hides the SynonymList component depending on the 'isVisible' property", function () {
          expect(component.toggle).toBeDefined();
        });
      });
    });
  };

  runTests(jasmineEnv, setupTests);
});