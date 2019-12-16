$("#add-vacancy").click(function() { addNewVacancy(); });
$(".progress").hide();

loadCategories();

function loadCategories() {

    const options = $("#category-id");
    options.html('');

    $.get('/api/categories',
        function(result) {
            $.each(result, function() {
                options.append($("<option />").val(this.id).text(this.name));
            });
        }
    );
}

function addNewVacancy() {
    const key = $("#security-key").val();
    const categoryId = $("#category-id").val();
    const comment = $("#vacancy-comment").val();

    const title = $("#vacancy-title").val();
    const description = $('#vacancy-description').val();
    const content = $('#vacancy-content').summernote('code');
    const contact = $("#vacancy-contact").val();

    const progress = $(".progress");

    const data = {
        key: key,
        categoryId: categoryId,
        comment: comment,
        title: title,
        description: description,
        content: content,
        contact: contact
    };

    progress.show();

    $.post('/api/vacancy/new', data)
        .done(function(response) {
            progress.hide();
            window.location.replace(response.shareUrl);
        }).fail(function(err) {
            if (!!err && err.status === 403)
                alert('Access denied');
            else
                console.error(err);
        });
}