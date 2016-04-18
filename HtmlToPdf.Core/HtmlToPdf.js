var system = require('system');
var webpage = require('webpage');

var page = webpage.create();

//page.viewportSize = { width: 600, height: 600 };
//page.paperSize = { format: 'A4', orientation: 'portrait', margin: '1cm' };

var dpi = 72.0, dpcm = dpi / 2.54;
var widthCm = 21.0, heightCm = 29.7; // A4
page.viewportSize = { width: Math.round(widthCm * dpcm), height: Math.round(heightCm * dpcm) };
page.paperSize = { width: page.viewportSize.width + 'px', height: page.viewportSize.height + 'px', orientation: 'portrait', margin: '0px' };//, margin: '1cm'
page.settings.dpi = dpi;
page.zoomFactor = 1.0;

var line, params;
function input() {
    line = system.stdin.readLine();

    switch (line) {
        case 'exit': {
            console.log('exit');
            phantom.exit();
            break;
        }
        case '': {
            console.log('empty');
            input();
            break;
        }
        default:
            {
            try {
                params = JSON.parse(line);
                page.content = params.content;

                page.render(params.name + '.pdf');
                //page.render('/dev/stdout', { format: 'pdf' });
                //page.render('\\\\.\\CON', { format: 'pdf' });
                //page.renderPdf('/dev/stdout');

                console.log('converted');
            } catch (ex) {
                console.log(ex);
            } 

            input();
            break;
        }
    }
}

input();