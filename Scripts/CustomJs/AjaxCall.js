const AjaxCall = async (url, type, data = null, dataType = false, contentType = false, success = null, error = null) => {
    $.ajax({
        url,
        type,
        data,
        dataType,
        contentType,
        success,
        error
    });
}
