$("#add-post").click(function () { addNewPost(); });
$(".progress").hide();

loadCategories();

function loadCategories() {

    var options = $("#category-id");
    options.html('');

    $.get('/api/categories',
        function (result) {
            $.each(result, function () {
                options.append($("<option />").val(this.id).text(this.name));
            });
        }
    );
}

function addNewPost() {

    var link = $("#link").val();
    var key = $("#security-key").val();
    var categoryId = $("#category-id").val();
    var progress = $(".progress");
    var url = '/api/publications/new?url=' + link + '&key=' + key & '&categoryId=' + categoryId;

    progress.show();

    $.get(url, function (data) {
        progress.hide();
        window.location.replace(data.shareUrl);
    }).done(function () {
    }).fail(function (err) {
        if (!!err && err.status == 403)
            alert('Неверный ключ');
        else
            console.error(err);
    });
}