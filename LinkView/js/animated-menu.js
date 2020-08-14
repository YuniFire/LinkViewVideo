( function( $ ) {
$( document ).ready(function() {
$('.nav').prepend('<div id="menu-button">Menu</div>');
	$('.nav #menu-button').on('click', function(){
		var menu = $(this).next('ul');
		if (menu.hasClass('open')) {
			menu.removeClass('open');
		}
		else {
			menu.addClass('open');
		}
	});
});
} )( jQuery );