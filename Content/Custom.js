$(document).ready(function () {
    //-------------------------dashboard--------------------------
    $(document).on('click', '.dashboardLink', function () {
        $.ajax({
            method: "GET",
            url: "/Home/Profile",
            contentType: false,
            success: function (response) {
                $('#content-body').html(response);
            },
            error: function (xhr, status, error) {
                console.log("error" + error);
            }
        });
    });
    //--------------------------stock-------------------------------
    //Show Stock List
    $(document).on('click', '.stocksLink', function () {
        $.ajax({
            method: "GET",
            url: "/Stock/Stocks",
            contentType: false,
            success: function (response) {
                $('#content-body').html(response);
            },
            error: function (xhr, status, error) {
                console.log("error" + error);
            }
        });
    });

    //Get data into modal for add or edit stock
    $(document).on('click', '#addOrEditStock', function () {
        var id = $(this).data('id');
        $('.addOrEdit-modal-body').html('');
        $.ajax({
            method: "GET",
            url: "/Stock/AddOrEditStock/" + id,
            contentType: false,

            success: function (response) {
                $('.addOrEdit-modal-body').html(response);
                $('#stockModal').modal('show');
                $.validator.unobtrusive.parse($("#StockForm"));
            }
        });
    });

    //save data into model after add or edit stock
    $(document).on('submit', '#StockForm', function (e) {
        e.preventDefault();
        $.ajax({
            method: "POST",
            url: "/Stock/SaveStock/",
            data: $(this).serialize(),

            success: function (response) {
                $('#stockModal').modal('hide');
                $('.stocksLink').click();
            },

            error: function (xhr, status, error) {
                console.log('error', error);
            }
        });
    });

    //send stock report email to admin 
    $(document).on('click', '#sendEmail', function () {
        $.ajax({
            method: "GET",
            url: "/Stock/SendStockReportEmail",
            contentType: "application/json",
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                } else {
                    alert("Failed to send email.");
                }
            },
            error: function (xhr, status, error) {
                console.log("Error: " + error);
            }
        });
    });


    //delete stock
    $(document).on('click', '#deleteStock', function () {
        var id = $(this).data('id');
        if (confirm("Are you sure you want to delete this item?")) {
            $.ajax({
                method: "GET",
                url: "/Stock/DeleteStock/" + id,

                success: function (response) {
                    if (response.success) {
                        console.log("Item deleted successfully!!");
                        $('.stocksLink').click();
                    }
                },

                error: function (xhr, status, error) {
                    console.log('error');
                    console.log('errorStatus', xhr);
                }
            });
        }
    });

    //--------------------------Discount---------------------------

    //show discount list
    $(document).on('click', '.discountLink', function () {
        $.ajax({
            method: "GET",
            url: "/Discount/discounts",
            contentType: false,
            success: function (response) {
                $('#content-body').html(response);
            },
            error: function (xhr, status, error) {
                console.log("error" + error);
            }
        });
    });

    //Get data into modal for add or edit discount
    $(document).on('click', '#addOrEditDiscount', function () {
        var id = $(this).data('id');
        $('.addOrEdit-modal-body').html('');

        $.ajax({
            method: "GET",
            url: "/Discount/AddOrEditDiscount/" + id,
            contentType: false,

            success: function (response) {
                $('.addOrEdit-modal-body').html(response);
                $('#discountModal').modal('show');
                $.validator.unobtrusive.parse($("#DiscountForm"));
            }
        });
    });

    $(document).on('submit', '#DiscountForm', function (e) {
        e.preventDefault();
        $.ajax({
            method: "POST",
            url: "/Discount/SaveDiscount/",
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    $('#discountModal').modal('hide');
                    $('.discountLink').click();
                } else {
                    console.log("Error");
                    $('.text-danger').empty();
                    response.errors.forEach(function (error) {
                        $('<span class="text-danger">' + error + '</span>').appendTo('.addOrEdit-modal-body');
                    });
                }
            },
            error: function (xhr, status, error) {
                console.log('error', error);
            }
        });
    });

    //delete discount
    $(document).on('click', '#deleteDiscount', function () {
        var id = $(this).data('id');
        if (confirm("Are you sure you want to delete this item?")) {
            $.ajax({
                method: "GET",
                url: "/Discount/DeleteDiscount/" + id,

                success: function (response) {
                    if (response.success) {
                        console.log("Item deleted successfully!!");
                        $('.discountLink').click();
                    }
                },

                error: function (xhr, status, error) {
                    console.log('error');
                    console.log('errorStatus', xhr);
                }
            });
        }
    });


    //--------------------------Stock Report -------------------------------
    $(document).on('click', '.stockReportLink', function () {
        $.ajax({
            method: "GET",
            url: "/Stock/StockReport",
            contentType: false,
            success: function (response) {
                $('#content-body').html(response);
            },
            error: function (xhr, status, error) {
                console.log("error" + error);
            }
        });
    })
});