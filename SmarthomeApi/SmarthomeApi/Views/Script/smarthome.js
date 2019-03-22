$(document).ready(function () {
    var lines = [];

    var toggleDataSeries = function (e) {
        if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible)
            e.dataSeries.visible = false;
        else
            e.dataSeries.visible = true;
        chart.render();
    };

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
            includeZero: true
        },
        toolTip: {
            shared: true
        },
        legend: {
            cursor: "pointer",
            fontSize: 8,
            itemclick: toggleDataSeries
        },
        data: lines
    };

    var chart = new CanvasJS.Chart("chart", options);

    var addLine = function (name, curve) {
        lines.push({
            type: "line",
            name: name,
            showInLegend: true,
            yValueFormatString: "# Ws",
            xValueFormatString: "HH:mm:ss",
            connectNullData: true,
            lineThickness : 1,
            dataPoints: curve
        });
    };

    $.ajax({
        url: "/api/digitalstrom/energy/curves/days/2019-03-22",
        method: "GET"
    })
        .done(function (data) {
            for (var n = 0; n < data.circuits.length; n++) {
                var curve = [];
                for (var i = 0; i < data.circuits[n].energy_curve.length; i++) {
                    var val = data.circuits[n].energy_curve[i];
                    curve.push({
                        x: new Date(1553258791 + i * 1000),
                        y: val < 0 ? null : val
                    });
                }
                addLine(data.circuits[n].name, curve);
            }
            chart.render();
        });
});