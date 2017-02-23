// Comment at the top.
goog.provide('app.views.HeaderBar');
goog.require('goog.events.EventHandler');
goog.require('goog.ui.Component');
goog.require('goog.Menu');
 
/**
 * The header bar to use for cool stuff.
 *
 * @param {goog.dom.DomHelper=} opt_domHelper DOM helper to use.
 *
 * @extends {goog.ui.Component}
 * @constructor
 */
app.views.HeaderBar = function(opt_domHelper) {
  goog.ui.Component.call(this, opt_domHelper);

    /**
   * Event handler for this object.
   * @type {goog.events.EventHandler}
   * @private
   */
  this.eh_ = new goog.events.EventHandler(this);

  this.configButton_ = new goog.ui.Button();
};
goog.inherits(app.views.HeaderBar, goog.ui.Component);

/**
 * The ids for the various elements in the system.
 * @enum {string}
 */
apps.views.HeaderBar.Ids = {
  CONFIG: 'config',
  STATUS: 'status'
};

/**
 * Creates an initial DOM representation for the component.
 */
app.views.HeaderBar.prototype.createDom = function() {
  var ids = this.makeIds(apps.views.HeaderBar.Ids);
  var dom = app.views.HeaderBarSoy({ ids: ids });
  this.decorateInternal(dom);
};


/**
 * Decorates an existing HTML DIV element as a SampleComponent.
 *
 * @param {Element} element The DIV element to decorate. The element's
 *    text, if any will be used as the component's label.
 */
goog.demos.SampleComponent.prototype.decorateInternal = function(element) {
  app.views.HeaderBar.superClass_.decorateInternal.call(this, element);

  // Render the two buttons.
  var statusElement = this.getElementById(apps.views.HeaderBar.Ids.STATUS);
  this.statusButton_.decorate(statusElement);

  var configElement = this.getElementById(apps.views.HeaderBar.Ids.CONFIG);
  this.configButton_.decorate(configElement);
};


/** @override */
app.views.HeaderBar.prototype.disposeInternal = function() {
  app.views.HeaderBar.superClass_.disposeInternal.call(this);
  this.eh_.dispose();
};


/**
 * Called when component's element is known to be in the document.
 */
app.views.HeaderBar.prototype.enterDocument = function() {
  app.views.HeaderBar.superClass_.enterDocument.call(this);
};


/**
 * Called when component's element is known to have been removed from the
 * document.
 */
app.views.HeaderBar.prototype.exitDocument = function() {
  app.views.HeaderBar.superClass_.exitDocument.call(this);
};

