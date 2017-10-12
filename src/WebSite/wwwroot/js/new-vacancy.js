$("#add-vacancy").click(function () { addNewVacancy(); });
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

function addNewVacancy() {


    var key = $("#security-key").val();
    var categoryId = $("#category-id").val();
    var comment = $("#vacancy-comment").val();

    var title = $("#vacancy-title").val();
    var description = $("#vacancy-description").val();
    var contact = $("#vacancy-contact").val();

    var progress = $(".progress");

    var data = {
        key: key,
        categoryId: categoryId,
        comment: comment,
        title: title,
        description: description,
        contact: contact
    };

    progress.show();

    $.post('/api/vacancies/new', data)
        .done(function (response) {
            progress.hide();
            window.location.replace(response.shareUrl);
        }).fail(function (err) {
            if (!!err && err.status == 403)
                alert('Access denied');
            else
                console.error(err);
        });
}