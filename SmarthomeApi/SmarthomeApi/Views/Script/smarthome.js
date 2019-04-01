$(document).ready(function () {
    var dateFormat = "dd.mm.yy";
    var getDate = function (element) {
        try {
            return $.datepicker.parseDate(dateFormat, element.val());
        } catch (error) {
            return null;
        }
    };

    var fillChart = function (chartElement) {
        var getChart = function () {
            return chartElement.CanvasJSChart();
        };

        var toggleDataSeries = function (e) {
            if (typeof e.dataSeries.visible === "undefined" || e.dataSeries.visible)
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
                valueFormatString: "DD.MM.YY HH:mm:ss"
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

        var addLine = function (curve, name, format) {
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
                url: "../api/graph/" + $("#charttype").val() + "?count=800&begin=" + getDate($("#from")).getTime() + "&end=" + getDate($("#to")).getTime(),
                method: "GET"
            })
            .done(function (data) {
                clearLines();
                for (var n = 0; n < data.lines.length; n++) {
                    var begin = data.lines[n].begin;
                    var spacing = data.lines[n].spacing;
                    var curve = [];
                    for (var i = 0; i < data.lines[n].points.length; i++) {
                        var time = new Date(begin + i * spacing);
                        curve.push({ x: time, y: data.lines[n].points[i] });
                    }
                    addLine(curve, data.lines[n].name, data.lines[n].format);
                }
                getChart().render();
            });
        });
    };

    fillChart($("#chart"));
    
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
    
    from.datepicker("setDate", new Date(Date.now() - (24 * 60 * 60 * 1000) * 14));
    to.datepicker("setDate", Date.now());

    //from.on("change", function () {
    //    to.datepicker("option", "minDate", getDate($("#from")));
    //});
    //to.on("change", function () {
    //    from.datepicker("option", "maxDate", getDate($("#to")));
    //});
});