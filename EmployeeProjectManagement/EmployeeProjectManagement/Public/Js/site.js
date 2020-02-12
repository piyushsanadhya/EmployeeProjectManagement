//Ajax helpers
$.ajaxGet = function (url, success) {
    return $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        success: success
    });
};

$.ajaxPost = function (url, data, success) {
    data.__RequestVerificationToken = $("[name='__RequestVerificationToken']").val();

    var ajax = {
        url: url,
        type: 'POST',
        dataType: 'json',
        data: data,
        headers: {
            'X-VerificationToken': $("[name='__RequestVerificationToken']").val()
        },
        success: success
    };

    return $.ajax(ajax);
};