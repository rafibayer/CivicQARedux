// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function submitFormAjax(formId, method = 'POST') {
    let form = $(formId)[0];
    $.ajax({
        url: form.action,
        type: method,
        data: form.data,
        error: function (err) {
            console.error(err);
        }
    });
}