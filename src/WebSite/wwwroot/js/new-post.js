$("#add-post").click(function () {
    createPost();
});

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

function createPost() {

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


    $.ajax({
        type: 'POST',
        url: '/api/publications',
        data: data,
        success: function (data, textStatus, request) {
            progress.hide();

            const location = request.getResponseHeader('location');

            window.location.replace(location);
        },
        error: function (request, textStatus, error) {
            progress.hide();

            const message = JSON.stringify({
                message: request.responseText,
                error
            }, null, 4);

            alert(message);

            console.error(error);
        }
    });
}