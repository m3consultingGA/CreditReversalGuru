

$(function () {

	var d1, d2, d3, data, chartOptions;

	d1 = [
        [new  Date(2019,1, 1), 12], [new Date(2019, 2, 1), 7], [new Date(2019, 3, 1), 10], [new Date(2019, 4, 1), 6],
        [new Date(2019, 5, 1), 3]
    ];
 
    d2 = [
        [1325376000000, 10], [1328054400000, 6], [1330560000000, 3], [1333238400000, 4],
        [1335830400000, 3]
    ];
 
    d3 = [
        [1325376000000, 6], [1328054400000, 4], [1330560000000, 1], [1333238400000, 2],
        [1335830400000, 1]
    ];

    data = [{
    	label: 'First Round',
    	data: d1,
        bars: {
          order: 0
        }
    }, {
    	label: 'Second Round',
    	data: d2,
        bars: {
          order: 1
        }
    }, {
    	label: 'Third Round',
    	data: d3,
        bars: {
          order: 2
        }
    }];

    chartOptions = {
        xaxis: {
             
            min: (new Date(2019, 1, 1)).getTime(),
            max: (new Date(2019, 12, 31)).getTime(),
            mode: "time",
            tickSize: [1, "month"],
            monthNames: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
            tickLength: 0,
            
        },
        grid: {
            hoverable: true,
            clickable: false,
            borderWidth: 0
        },
        bars: {
	    	show: true,
	    	barWidth: 12*24*60*60*300,
            fill: true,
            lineWidth: 1,
            lineWidth: 0,
            fillColor: { colors: [ { opacity: 1 }, { opacity: 1 } ] }
	    },
        
        tooltip: true,
        tooltipOpts: {
            content: '%s: %y'
        },
        colors: mvpready_core.layoutColors
    }


    var holder = $('#vertical-chart');

    if (holder.length) {
        $.plot(holder, data, chartOptions );
    }

});