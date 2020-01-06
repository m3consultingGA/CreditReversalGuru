$(function () {

	var data, chartOptions

	data = [
		{ label: "John", data: Math.floor (Math.random() * 100 + 250) }, 
		{ label: "Martin", data: Math.floor (Math.random() * 100 + 350) }, 
		{ label: "Ricky", data: Math.floor (Math.random() * 100 + 650) }, 
		{ label: "David", data: Math.floor (Math.random() * 100 + 50) }
	]

	chartOptions = {		
		series: {
			pie: {
				show: true,  
				innerRadius: 0, 
				stroke: {
					width: 4
				}
			}
		}, 
		legend: {
			show: false,
			position: 'ne'
		}, 
		tooltip: true,
		tooltipOpts: {
			content: '%s: %y'
		},
		grid: {
			hoverable: true
		},
		colors: mvpready_core.layoutColors
	}

	var holder = $('#pie-chart')

	if (holder.length) {
		$.plot(holder, data, chartOptions )
	}


})