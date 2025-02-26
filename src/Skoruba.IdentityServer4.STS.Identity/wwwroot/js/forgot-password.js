$(document).ready(function () {
    $('.resetPasswordBy').hide();

    if ($('.labsoft-forgotPassword-choosePolicy-group').is(':visible')) {
        var selectedLoginPolicy = $('[name=Policy]:checked').val();
        $('#resetPasswordBy' + selectedLoginPolicy).show();
    } else {
        $('#resetPasswordByUsername').show();
        $('#username-forgotpassword').prop('checked', true);
    }

    $('input[name="Policy"]').change(function () {
        var selectedLoginPolicy = $(this).val();
        $('.resetPasswordBy').hide();
        $('#resetPasswordBy' + selectedLoginPolicy).show();
    });
});