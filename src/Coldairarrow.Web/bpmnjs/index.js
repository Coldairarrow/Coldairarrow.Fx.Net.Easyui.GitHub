var BpmnJS = window.BpmnJS;
var bpmnViewer = new BpmnJS({
	container : '#canvas'
});
function success(){
	$('body').removeClass('fail').addClass('success');
}
function fail(err){
	$('body').addClass('fail');
	console.error('something went wrong!');
	console.error(err);
}

$.get('pizza-collaboration.bpmn', function(){
	var diagramXML = '<?xml version="1.0" encoding="UTF-8"?>\n' +
        '<definitions xmlns="http://www.omg.org/spec/BPMN/20100524/MODEL" xmlns:bpmndi="http://www.omg.org/spec/BPMN/20100524/DI" xmlns:omgdc="http://www.omg.org/spec/DD/20100524/DC" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" targetNamespace="" xsi:schemaLocation="http://www.omg.org/spec/BPMN/20100524/MODEL http://www.omg.org/spec/BPMN/2.0/20100501/BPMN20.xsd">\n' +
        '  <process id="Process_14jyjr3">\n' +
        '    <startEvent id="StartEvent_0lfiff0" />\n' +
        '  </process>\n' +
        '  <bpmndi:BPMNDiagram id="sid-74620812-92c4-44e5-949c-aa47393d3830">\n' +
        '    <bpmndi:BPMNPlane id="sid-cdcae759-2af7-4a6d-bd02-53f3352a731d" bpmnElement="Process_14jyjr3">\n' +
        '      <bpmndi:BPMNShape id="StartEvent_0lfiff0_di" bpmnElement="StartEvent_0lfiff0">\n' +
        '        <omgdc:Bounds x="219" y="120" width="36" height="36" />\n' +
        '      </bpmndi:BPMNShape>\n' +
        '    </bpmndi:BPMNPlane>\n' +
        '    <bpmndi:BPMNLabelStyle id="sid-e0502d32-f8d1-41cf-9c4a-cbb49fecf581">\n' +
        '      <omgdc:Font name="Arial" size="11" isBold="false" isItalic="false" isUnderline="false" isStrikeThrough="false" />\n' +
        '    </bpmndi:BPMNLabelStyle>\n' +
        '    <bpmndi:BPMNLabelStyle id="sid-84cb49fd-2f7c-44fb-8950-83c3fa153d3b">\n' +
        '      <omgdc:Font name="Arial" size="12" isBold="false" isItalic="false" isUnderline="false" isStrikeThrough="false" />\n' +
        '    </bpmndi:BPMNLabelStyle>\n' +
        '  </bpmndi:BPMNDiagram>\n' +
        '</definitions>';
    bpmnViewer.importXML(diagramXML, function(err){
		if (err) { return fail(err); }
		try {
			var canvas = bpmnViewer.get('canvas');
			canvas.zoom('fit-viewport');
			var eventBus = bpmnViewer.get("eventBus");
			eventBus.on('element.click', function(evt) {
				//点击元素，弹出页面可以在这里写
				// alert(0);
			});
			return success();
		} catch (e) {
			return fail(e);
		}
	});
    $("#save-button").on("click",function () {
        bpmnViewer.saveXML({
            format: true
        }, function(err, xml) {
            if (err) {
                console.error('diagram save failed', err);
            } else {
                var a = document.createElement('a');
                a.setAttribute('href',"data:application/bpmn20-xml;charset=UTF-8," + xml);
                a.setAttribute('download','流程.bpmn');
                a.click();
            }
        });
    });
}, 'text');
