

if (typeof jQuery === "undefined") {
    throw new Error("BusinessSuite requires jQuery");
}

var continueButton = null;
var preventContinue = true;
var confirmFunction = {};

var alertError = function (message) {
    var htmlPane =
                  '<div class="alert alert-danger alert-dismissable">'
                +       '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>'
                +       '<h4><i class="icon fa fa-ban"></i> Erro!</h4>'
                +       '<p>' + message + '</p>'
                + '</div>'
    $('[data-bs-id="alert-erro"]').html(htmlPane);
};

$(function () {
    var activateMenu = $("#menuController").val();
    if (activateMenu != undefined && activateMenu != "") {
        var accessor = '[data-bs-area="' + activateMenu + '"]';
        $(accessor).addClass("active");
    }

    $("#saveButton").click(function (e) {
        if ($(this).parents("form").valid()) {
            $(this).prop("disable", true);
            $("#waitSave").show();
        }
    });

    $('[data-dismiss="modal"]').click(function (e) {
        e.preventDefault();
        $(this).parents(".modal").hide();
    });

    $('[data-accept="modal"]').click(function (e) {
        e.preventDefault();
        $(this).parents(".modal").hide();
        preventContinue = false;
        confirmFunction();
    });

    $('[data-modal="error"]').click(function (e) {
        if (preventContinue) {
            e.preventDefault();

            var message = $(this).attr("data-modal-text");
            continueButton = $(this);
            confirmFunction = function () {
                continueButton.click();
            };

            $("#errorWindow .modal-body").html("<p>" + message + "</p>");
            $("#errorWindow").fadeIn();
        }
    });

    $('[data-modal="error-ajax"]').click(function (e) {
        if (preventContinue) {
            var url = $(this).attr("data-modal-url");
            var msType = $(this).attr("data-modal-type");

            var entryCode = "";

            if(msType == "E"){
                entryCode = $("#PalletCode").val();
            } else if(msType == "A") {
                entryCode = $(this).val();
            } else if (msType == "AP") {
                entryCode = $("#PalletCode").val();
            }

            continueButton = $(this);
            confirmFunction = function () {
                continueButton.click();
            };

            $.ajax({
                url: url,
                data: {entry: entryCode, typeCode: msType},
                type: 'GET',
                async: false,
                cache: false,
                timeout: 10000,
                error: function () {
                    return true;
                },
                success: function (data) {
                    if (data.displayMessage) {
                        e.preventDefault();
                        $("#errorWindow .modal-body").html("<p>" + data.message + "</p>");
                        $("#errorWindow").fadeIn();
                        return true;
                    }
                }
            });
        }
    });

    $('[data-bs-id="bt-load-qtd"]').click(function (e) {
        e.preventDefault();
        var urlAction = $(this).attr("data-bs-url");
        var sItemCode = $(this).parent("td").children('[data-bs-id="ItemCode"]').val();
        var oQuantity = $(this).parent().parent().find('td > input[type="text"]');
        $.ajax({
            url: urlAction,
            data: { itemCode:  sItemCode },
            type: 'GET',
            async: true,
            cache: false,
            timeout: 10000,
            error: function () {
                return true;
            },
            success: function (data) {
                oQuantity.val(data.quantity);
                return true;
            }
        }); //$.ajax()
    }); //$('[data-bs-id="bt-load"]').click()

    $('input[type="text"].form-control').focus(function () {
        var othis = $(this);
        setTimeout(function() {
            othis.select();
        }, 100);
    });

    $('[data-bs-id="bt-cancel-ap"]').click(function () {
        var athis = $(this);
        var docEntry = athis.parent().find('input[type="hidden"]').val();
        var urlAction = $(this).attr("data-bs-url");
        confirmFunction = function () {
            $.ajax({
                url: urlAction,
                data: { apontamentoEntry: docEntry },
                type: 'GET',
                async: true,
                cache: false,
                timeout: 10000,
                error: function () {
                    alertError("Houve um erro ao excluir o pallet.");
                },
                success: function (data) {
                    if (data.success) {
                        athis.parents('[data-bs-id="box-ap"]').fadeOut();
                    } else {
                        alertError(data.message);
                    }
                }
            }); //$.ajax()
        }

        $("#confirmWindow").find(".modal-body").html("<p>Tem certeza que deseja cancelar este pallet?</p>");
        $("#confirmWindow").fadeIn();
    });
});


//VALIDATION METHODS

$.validator.methods.number = function (value, element) {
    if (!isNaN(parseFloat(value))) {
        return true;
    }
    return false;
}