
// SCALING OPTIONS
// scaling can have values as follows with full being the default
// "fit"    sets canvas and stage to dimensions and scales to fit inside window size
// "outside"    sets canvas and stage to dimensions and scales to fit outside window size
// "full"    sets stage to window size with no scaling
// "tagID"    add canvas to HTML tag of ID - set to dimensions if provided - no scaling

var scaling = "fit"; // this will resize to fit inside the screen dimensions
var width = 1024;
var height = 768;
var color = light;
var outerColor = darker;

var frame = new Frame({ scaling, width, height, color, outerColor, retina: true });
frame.on("ready", function () {

    var stage = frame.stage;
    var stageW = frame.width;
    var stageH = frame.height;

    frame.outerColor = '#444';
    frame.color = '#ddd';

    var bases = [];
    var shadows = [];

    var holderTile = new Container();
    holderTile
        .sca(2, 1)
        .center();

    var label = new Label("Hello").addTo(holderTile);

    var label = new Label({
        text: "CLICK",
        size: 10,
        font: "courier",
        color: "white",
        rollColor: "red",
        fontOptions: "italic bold"
    }).addTo(holderTile).ske(0, -45);

    var win = new Window({
        height: 300,
        interactive: true,
        padding: 0,
        slideDamp: .2,
        scrollBarDrag: true
    }).pos(50, 20);

    stage.update();

});