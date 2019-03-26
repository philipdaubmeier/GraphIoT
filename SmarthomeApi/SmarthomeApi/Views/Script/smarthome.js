$(document).ready(function () {
    var dateFormat = "dd.mm.yy";
    var getDate = function (element) {
        try {
            return $.datepicker.parseDate(dateFormat, element.val());
        } catch (error) {
            return null;
        }
    }

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
                valueFormatString: "DD.MM.YY HH:mm:ss",
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

        var clearLines = function () {
            lines.splice(0, lines.length);
        };

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
        
        $("#reload").on("click", function () {
            $.ajax({
                url: "../api/viessmann/solar/graph?count=800&begin=" + getDate($("#from")).getTime() + "&end=" + getDate($("#to")).getTime(),
                method: "GET"
            })
            .done(function (data) {
                clearLines();
                var begin = new Date(data.begin);
                var spacing = data.spacing_millis;
                for (var n = 0; n < data.lines.length; n++) {
                    var curve = [];
                    for (var i = 0; i < data.lines[n].points.length; i++) {
                        var time = new Date(begin.getTime() + i * spacing)
                        curve.push({ x: time, y: data.lines[n].points[i] });
                    }
                    addLine(data.lines[n].name, curve, data.lines[n].format);
                }
                getChart().render();
            });
        });
    };

    solarDhwChart($("#chart"));
    
    var from = $("#from")
        .datepicker({
            defaultDate: 0,
            changeMonth: true,
            numberOfMonths: 1,
            altField: "#from",
            altFormat: dateFormat
        });
    
    var to = $("#to")
        .datepicker({
            defaultDate: 0,
            changeMonth: true,
            numberOfMonths: 1,
            altField: "#to",
            altFormat: dateFormat
        });

    //from.on("change", function () {
    //    to.datepicker("option", "minDate", getDate(this));
    //});
    //to.on("change", function () {
    //    from.datepicker("option", "maxDate", getDate(this));
    //});
});