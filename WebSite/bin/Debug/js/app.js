// Comment at the top.
goog.provide('app'); //Providing your app namespace
goog.require('goog.dom');
 
app.init = function(){
  var newHeader = goog.dom.createDom('h1', {'style': 'background-color:#EEE'},
    'Hello world!');
  goog.dom.appendChild(document.body, newHeader);
};
 
goog.exportSymbol('app_init', app.init);