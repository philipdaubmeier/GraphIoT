$(document).ready(function () {
    var dataPoints = [];

    var options = {
        zoomEnabled: true,
        animationEnabled: false,
        theme: "light2",
        axisX: {
            valueFormatString: "HH mm ss",
        },
        axisY: {
            title: "Ws",
            titleFontSize: 12,
            includeZero: false
        },
        data: [{
            type: "line",
            yValueFormatString: "# Ws",
            xValueFormatString: "HH:mm:ss",
            dataPoints: dataPoints
        }]
    };

    $.ajax({
        url: "/api/digitalstrom/energy/curves/days/2019-03-22",
        method: "GET"
    })
        .done(function (data) {
            for (var i = 0; i < data.circuits[0].energy_curve.length; i++) {
                dataPoints.push({
                    x: new Date(1553258791 + i * 1000),
                    y: data.circuits[0].energy_curve[i]
                });
            }
            $("#chart").CanvasJSChart(options);
        });
});