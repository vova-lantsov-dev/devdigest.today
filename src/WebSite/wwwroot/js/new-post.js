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
    var comment = $("#post-comment").val();

    var progress = $(".progress");
    
    var data = {
        link: link,
        key: key,
        categoryId: categoryId,
        comment: comment
    };

    progress.show();

    $.post('/api/publications/new', data)
        .done(function () {
            progress.hide();
            window.location.replace(data.shareUrl);
        }).fail(function (err) {
            if (!!err && err.status == 403)
                alert('Неверный ключ');
            else
                console.error(err);
        });
}