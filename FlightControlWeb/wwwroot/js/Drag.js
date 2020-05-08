function dragOverHandler(ev) {
    if (ev.stopPropagation) {
        ev.stopPropagation(); //Stops some browsers from redirecting.
    }
    document.getElementById('drop_zone').style.backgroundImage = "url(leaflet/images/drag.JPG)";
    // Prevent default behavior (Prevent file from being opened)
    ev.preventDefault();
}

function dropHandler(ev) {
    console.log('File(s) dropped');
    // Prevent default behavior (Prevent file from being opened)
    ev.preventDefault();

    if (ev.dataTransfer.items) {
        // Use DataTransferItemList interface to access the file(s)
        for (var i = 0; i < ev.dataTransfer.items.length; i++) {

            // If dropped items aren't files, reject them
            if (ev.dataTransfer.items[i].kind === 'file') {
                var file = ev.dataTransfer.items[i].getAsFile();
                var reader = new FileReader();
                    reader.onload = function (e) {
                        // get file content
                        var text = e.target.result;

                        $.ajax({
                            url: "../api/FlightPlans",
                            type: 'POST',
                            data: text,
                            dataType: 'json',
                            success: function () {
                                alert("yes");
                            }
                        });
                    }
                    reader.readAsText(file);
            }
        }
    } else {
        // Use DataTransfer interface to access the file(s)
        for (var i = 0; i < ev.dataTransfer.files.length; i++) {
            console.log('... file[' + i + '].name = ' + ev.dataTransfer.files[i].name);
        }
    }
    document.getElementById('drop_zone').style.background = "white";
    // Prevent default behavior (Prevent file from being opened)
    ev.preventDefault();
    // Pass event to removeDragData for cleanup
    removeDragData(ev)
}

function removeDragData(ev) {
    console.log('Removing drag data')

    if (ev.dataTransfer.items) {
        // Use DataTransferItemList interface to remove the drag data
        ev.dataTransfer.items.clear();
    } else {
        // Use DataTransfer interface to remove the drag data
        ev.dataTransfer.clearData();
    }
}