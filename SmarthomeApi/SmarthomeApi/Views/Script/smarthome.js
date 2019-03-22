$(document).ready(function () {
    var energyConsumtionChart = function (chartElement) {
        var getChart = function () {
            return chartElement.CanvasJSChart();
        };

        var toggleDataSeries = function (e) {
            if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible)
                e.dataSeries.visible = false;
            else
                e.dataSeries.visible = true;
            getChart().render();
        };

        var lines = [];
        var options = {
            zoomEnabled: true,
            animationEnabled: false,
            theme: "light2",
            axisX: {
                valueFormatString: "HH:mm:ss",
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
        $("#chart").CanvasJSChart(options);

        var addLine = function (name, curve) {
            lines.push({
                type: "line",
                name: name,
                showInLegend: true,
                yValueFormatString: "# Ws",
                xValueFormatString: "HH:mm:ss",
                connectNullData: true,
                lineThickness: 1,
                dataPoints: curve
            });
        };

        $.ajax({
            url: "../api/digitalstrom/energy/curves/days/2019-03-22",
            method: "GET"
        })
            .done(function (data) {
                for (var n = 0; n < data.circuits.length; n++) {
                    var curve = [];
                    for (var i = 0; i < data.circuits[n].energy_curve.length; i++) {
                        var val = data.circuits[n].energy_curve[i];
                        curve.push({
                            x: new Date((data.circuits[n].begin + i) * 1000),
                            y: val < 0 ? null : val
                        });
                    }
                    addLine(data.circuits[n].name, curve);
                }
                getChart().render();
            });
    };

    var solarDhwChart = function (chartElement) {
        var getChart = function () {
            return chartElement.CanvasJSChart();
        };

        var toggleDataSeries = function (e) {
            if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible)
                e.dataSeries.visible = false;
            else
                e.dataSeries.visible = true;
            getChart().render();
        };

        var lines = [];
        var options = {
            zoomEnabled: true,
            animationEnabled: false,
            theme: "light2",
            axisX: {
                valueFormatString: "HH:mm:ss",
            },
            axisY: {
                title: "Wh",
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
        $("#chart").CanvasJSChart(options);

        var addLine = function (name, curve, format) {
            lines.push({
                type: "line",
                name: name,
                showInLegend: true,
                yValueFormatString: format,
                xValueFormatString: "HH:mm:ss",
                connectNullData: true,
                lineThickness: 1,
                dataPoints: curve
            });
        };

        $.ajax({
            url: "../api/viessmann/solar/curves/days/2019-03-22",
            method: "GET"
        })
            .done(function (data) {
                var begin = Date.parse(data.begin);
                var curve_wh = [], curve_ct = [], curve_dt = [];
                for (var i = 0; i < data.solardata.length; i++) {
                    var time = new Date(begin + i * 5 * 60 * 1000)
                    curve_wh.push({ x: time, y: data.solardata[i].wh });
                    curve_ct.push({ x: time, y: data.solardata[i].collector_temp });
                    curve_dt.push({ x: time, y: data.solardata[i].dhw_temp });
                }
                addLine("Produktion Wh", curve_wh, "# Wh");
                addLine("Kollektortemperatur", curve_ct, "#.# °C");
                addLine("Warmwassertemperatur", curve_dt, "#.# °C");
                getChart().render();
            });
    };

    solarDhwChart($("#chart"));
});