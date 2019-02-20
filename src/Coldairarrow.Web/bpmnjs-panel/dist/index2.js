'use strict';

var _jquery = require('jquery');

var _jquery2 = _interopRequireDefault(_jquery);

var _Modeler = require('bpmn-js/lib/Modeler');

var _Modeler2 = _interopRequireDefault(_Modeler);

var _bpmnJsPropertiesPanel = require('bpmn-js-properties-panel');

var _bpmnJsPropertiesPanel2 = _interopRequireDefault(_bpmnJsPropertiesPanel);

var _camunda = require('bpmn-js-properties-panel/lib/provider/camunda');

var _camunda2 = _interopRequireDefault(_camunda);

var _camunda3 = require('camunda-bpmn-moddle/resources/camunda');

var _camunda4 = _interopRequireDefault(_camunda3);

var _minDash = require('min-dash');

var _newDiagram = require('../resources/newDiagram.bpmn');

var _newDiagram2 = _interopRequireDefault(_newDiagram);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

var container = (0, _jquery2.default)('#js-drop-zone');

var canvas = (0, _jquery2.default)('#js-canvas');

var bpmnModeler = new _Modeler2.default({
  container: canvas,
  propertiesPanel: {
    parent: '#js-properties-panel'
  },
  additionalModules: [_bpmnJsPropertiesPanel2.default, _camunda2.default],
  moddleExtensions: {
    camunda: _camunda4.default
  }
});
container.removeClass('with-diagram');

function createNewDiagram() {
  openDiagram(_newDiagram2.default);
}

function openDiagram(xml) {

  bpmnModeler.importXML(xml, function (err) {

    if (err) {
      container.removeClass('with-diagram').addClass('with-error');

      container.find('.error pre').text(err.message);

      console.error(err);
    } else {
      container.removeClass('with-error').addClass('with-diagram');
    }
  });
}

function saveSVG(done) {
  bpmnModeler.saveSVG(done);
}

function saveDiagram(done) {

  bpmnModeler.saveXML({ format: true }, function (err, xml) {
    done(err, xml);
  });
}

function registerFileDrop(container, callback) {

  function handleFileSelect(e) {
    e.stopPropagation();
    e.preventDefault();

    var files = e.dataTransfer.files;

    var file = files[0];

    var reader = new FileReader();

    reader.onload = function (e) {

      var xml = e.target.result;

      callback(xml);
    };

    reader.readAsText(file);
  }

  function handleDragOver(e) {
    e.stopPropagation();
    e.preventDefault();

    e.dataTransfer.dropEffect = 'copy'; // Explicitly show this is a copy.
  }

  container.get(0).addEventListener('dragover', handleDragOver, false);
  container.get(0).addEventListener('drop', handleFileSelect, false);
}

////// file drag / drop ///////////////////////

// check file api availability
if (!window.FileList || !window.FileReader) {
  window.alert('Looks like you use an older browser that does not support drag and drop. ' + 'Try using Chrome, Firefox or the Internet Explorer > 10.');
} else {
  registerFileDrop(container, openDiagram);
}

// bootstrap diagram functions

(0, _jquery2.default)(function () {

  (0, _jquery2.default)('#js-create-diagram').click(function (e) {
    e.stopPropagation();
    e.preventDefault();

    createNewDiagram();
  });

  var downloadLink = (0, _jquery2.default)('#js-download-diagram');
  var downloadSvgLink = (0, _jquery2.default)('#js-download-svg');

  (0, _jquery2.default)('.buttons a').click(function (e) {
    if (!(0, _jquery2.default)(this).is('.active')) {
      e.preventDefault();
      e.stopPropagation();
    }
  });

  function setEncoded(link, name, data) {
    var encodedData = encodeURIComponent(data);

    if (data) {
      link.addClass('active').attr({
        'href': 'data:application/bpmn20-xml;charset=UTF-8,' + encodedData,
        'download': name
      });
    } else {
      link.removeClass('active');
    }
  }

  var exportArtifacts = (0, _minDash.debounce)(function () {

    saveSVG(function (err, svg) {
      setEncoded(downloadSvgLink, 'diagram.svg', err ? null : svg);
    });

    saveDiagram(function (err, xml) {
      setEncoded(downloadLink, 'diagram.bpmn', err ? null : xml);
    });
  }, 500);

  bpmnModeler.on('commandStack.changed', exportArtifacts);
});