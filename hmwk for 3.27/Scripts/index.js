$(() => {
    $("#new-simcha").on('click', function () {

        $("#addSimchaModal").modal();

    });


    $("#new-contributor").on('click', function () {
        var contributorId = $(this).data('id');
        $("#new-contributor-hidden").val(contributorId);
        $("#addContributorModal").modal();

    });

    $(".deposit").on('click', function () {
        var contributorId = $(this).data('id');
        $("#deposit-hidden").val(contributorId);
        console.log(contributorId);
        $("#addDepositModal").modal();
    });

    $(".editContributor").on('click', function () {

        const button = $(this);
        const firstName = button.data('firstname');
        const lastName = button.data('lastname');
        const id = button.data('id');
        const cell = button.data('cell');
        const date = button.data('date');
        const alwaysInclude = button.data('alwaysinclude');
        console.log(id);

        $("#firstName").val(firstName);
        $("#lastName").val(lastName);
        $("#contributorIdModal").val(id);
        $("#cell").val(cell);
        $("#date").val(date);
        if (alwaysInclude == "True") {
            $("#alwaysInclude").prop('checked', true);
        }
        else {
            $("#alwaysInclude").prop('checked', false);
        }


        $("#editContributorModal").modal('show');

    });

 
    $("#submit-deposit").on('click', function () {
        $("#hiddenid").data('id', contributordataid); //get data id of button

    });
});