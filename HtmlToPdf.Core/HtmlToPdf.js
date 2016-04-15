var page = require('webpage').create(), system = require('system');

//page.viewportSize = { width: 600, height: 600 };
//page.paperSize = { format: 'A4', orientation: 'portrait' };//, margin: '1cm'
var dpi = 72.0, dpcm = dpi/2.54;
var widthCm = 21.0, heightCm = 29.7; // A4
page.viewportSize = { width: Math.round(widthCm * dpcm), height: Math.round(heightCm * dpcm) };
page.paperSize = { width: page.viewportSize.width + 'px', height: page.viewportSize.height + 'px', orientation: 'portrait', margin: '0px' };//1cm
page.settings.dpi = dpi;
page.zoomFactor = 1.0;

page.content = system.args[1];

//http://stackoverflow.hex1.ru/questions/19512983/phantomjs-pdf-to-stdout/21185279
//page.render('/dev/stdout', { format: 'pdf' });
page.render('stdout.pdf');

phantom.exit();