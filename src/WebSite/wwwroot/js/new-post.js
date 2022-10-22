$("#add-post").click(function () { addNewPost(); });
$(".progress").hide();

loadCategories();

function loadCategories() {

    const options = $("#category-id");
    
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

    const link = $("#link").val();
    const key = $("#security-key").val();
    const categoryId = $("#category-id").val();
    const title = $("#post-title").val();
    const comment = $("#post-comment").val();
    const titleUa = $("#post-title-ua").val();
    const commentUa = $("#post-comment-ua").val();

    const progress = $(".progress");

    const data = {
        link: link,
        key: key,
        categoryId: categoryId,
        title: title,
        comment: comment,
        titleUa: titleUa,
        commentUa: commentUa,
    };

    progress.show();

    $.post('/api/publications', data)
        .done(function (response) {
            progress.hide();
            window.location.replace(response.shareUrl);
        })
        .fail(function (err) {
            if (!!err && err.status === 403) {
                alert('Access denied');
            }
            else {
                console.error(err);
            }
        });
}