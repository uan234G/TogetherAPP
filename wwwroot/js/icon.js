window.onload = function () {
    const iconUpdate = document.getElementById('iconUpdate');
    const iconvalue = document.getElementById('iconval');
    const selectBtn = document.getElementById('select-btn');
    const avatarList = document.getElementById('avatarList');

    selectBtn.addEventListener('click', list);

    function list(){
        console.log('clicked');
        avatarList.classList.toggle('expand');
    }

    $('#divCircle img').click(function () {
        var theSrc = $(this).attr('src');
        $('.mainImg img').attr('src', theSrc);
        $('#iconval').val(theSrc);
        iconUpdate.submit()
    });
    $('#avatarList img').click(function() {
        var theSrc = $(this).attr('src');
        $('#main-image img').attr('src', theSrc);
        $('#iconval').val(theSrc);
        iconUpdate.submit()
    });
}
