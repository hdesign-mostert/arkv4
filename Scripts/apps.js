// Varying modal content - multiple buttons to trigger the same modal

$('#exampleModal3').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget) // Button that triggered the modal
    var recipient = button.data('whatever') // Extract info from data-* attributes
    // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
    // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
    var modal = $(this)
    modal.find('.modal-title').text('New message to ' + recipient)
    modal.find('.modal-body input').val(recipient)
})

//Tooltips

$(document).ready(function(){
    $('[data-toggle="tooltip"]').tooltip();
});


//Shuffle Effect at Features / Sliders

var Shuffle = window.Shuffle;

var myShuffle = new Shuffle(document.querySelector('.my-shuffle'), {
  itemSelector: '.image-item',
  sizer: '.my-sizer-element',
  buffer: 1,
});

window.jQuery('input[name="shuffle-filter"]').on('change', function (evt) {
  var input = evt.currentTarget;
  if (input.checked) {
    myShuffle.filter(input.value);
  }
});

// Success Message for Code Copy





