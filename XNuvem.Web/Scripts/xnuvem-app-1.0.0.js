/// <reference path="jquery-2.2.3.js" />
/// <reference path="jquery.validate.js" />
/// <reference path="../Content/plugins/select2/select2.js" />
/// <reference path="globalize.js" />
/// <reference path="globalize/number.js" />
/// <reference path="globalize/currency.js" />
/// <reference path="globalize/date.js" />
/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

(function ($) {
    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };
})(jQuery);

if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (searchString, position) {
        var subjectString = this.toString();
        if (typeof position !== 'number' || !isFinite(position) || Math.floor(position) !== position || position > subjectString.length) {
            position = subjectString.length;
        }
        position -= searchString.length;
        var lastIndex = subjectString.indexOf(searchString, position);
        return lastIndex !== -1 && lastIndex === position;
    };
}

if (!String.prototype.replaceAll) {
    String.prototype.replaceAll = function (search, replacement) {
        var target = this;
        return target.replace(new RegExp(search, 'g'), replacement);
    };
}



if (typeof jQuery === "undefined") {
    throw new Error("XNuvem requires jQuery");
}

var XNuvem = new XNuvemApp();

function XNuvemApp() {
    // XNuvem.statusMessage
    // Usado para mostrar mensagem de status no sistema como uma statusBar
    this.statusMessage = {
        panes: {
            error: $("<div />", {
                "class": "alert alert-error alert-dismissable",
                "style": "display: none"
            }).clone(),
            warning: $("<div />", {
                "class": "alert alert-warning alert-dismissable",
                "style": "display: none"
            }).clone(),
            info: $("<div />", {
                "class": "alert alert-info alert-dismissable",
                "style": "display: none"
            }).clone(),
            success: $("<div />", {
                "class": "alert alert-success alert-dismissable",
                "style": "display: none"
            }).clone()
        },
        // Show status message
        // title: Title of the display
        // message: Message of the display
        // displayTime: Time the display show in screen. 0 to infinite
        // pane: Inner pane of the message type of: XNuvem.statusMessage.panes (error, warning, info, success)
        // fa_icon: fa-icon css class
        show: function (title, message, displayTime, pane, fa_icon) {
            var timeOut = 8000;
            if (typeof(displayTime) !== 'undefined') {
                timeOut = displayTime;
            }
            var pn = XNuvem.statusMessage.panes.info.clone();
            var fa = "fa-info";
            if (typeof (pane) !== 'undefined') {
                pn = pane.clone();
            }
            if (typeof (fa_icon) !== 'undefined') {
                fa = fa_icon;
            }
            pn.append('<button class="close" aria-hidden="true" data-dismiss="alert" type="button">×</button>');
            pn.append('<h4><i class="icon fa ' + fa + '"></i>&nbsp' + title + '</h4>');
            pn.append('<div>' + message + '</div>');
            $("#app-messages").prepend(pn);
            pn.fadeIn('fast', function () {
                $(this).delay(timeOut).fadeOut('slow', function () {
                    pn.remove();
                });
            });
        },
        // Show status error
        // title: Title of the display
        // message: Message of the display
        // displayTime: Time the display show in screen. 0 to infinite
        showError: function (title, message, displayTime) {
            XNuvem.statusMessage.show(title, message, displayTime, XNuvem.statusMessage.panes.error, "fa-warning");
        },
        // Show status warning
        // title: Title of the display
        // message: Message of the display
        // displayTime: Time the display show in screen. 0 to infinite
        showWarning: function (title, message, displayTime) {
            XNuvem.statusMessage.show(title, message, displayTime, XNuvem.statusMessage.panes.warning, "fa-warning");
        },
        // Show status info
        // title: Title of the display
        // message: Message of the display
        // displayTime: Time the display show in screen. 0 to infinite
        showInfo: function (title, message, displayTime) {
            XNuvem.statusMessage.show(title, message, displayTime, XNuvem.statusMessage.panes.info, "fa-info");
        },
        // Show status sucess
        // title: Title of the display
        // message: Message of the display
        // displayTime: Time the display show in screen. 0 to infinite
        showSuccess: function (title, message, displayTime) {
            XNuvem.statusMessage.show(title, message, displayTime, XNuvem.statusMessage.panes.success, "fa-check");
        }
    }; // XNuvem.statusMessage

    // Message box
    var messageBoxPane =
            '<div class="modal" style="display: none;">' +
        '    <div class="modal-dialog">' +
        '    <div class="modal-content">' +
        '        <div class="modal-header">' +
        '        <button aria-label="Close" data-dismiss="modal" class="close" type="button"><span aria-hidden="true">×</span></button>' +
        '        <h4 class="modal-title"></h4>' +
        '        </div>' +
        '        <div class="modal-body">' +
        '        </div>' +
        '        <div class="modal-footer">' +
        '        <button data-dismiss="modal" class="btn btn-default pull-left" type="button">Fechar</button>' +
        '        <button data-bs-action="click" class="btn btn-black" type="button" style="display: none;">Continuar</button>' +
        '        </div>' +
        '    </div>' +
        '    </div>' +
        '</div>';
    this.messageBox = {
        mbType: {
            none: "modal-primary",
            info: "modal-info",
            warning: "modal-warning",
            success: "modal-success",
            danger: "modal-danger"
        },
        show: function (title, message, onContinue, modalType, fa_icon) {
            var mt = XNuvem.messageBox.mbType.none;
            var fa = "fa-info";
            if (typeof (modalType) !== 'undefined') {
                mt = modalType;
            }
            if (typeof (fa_icon) !== 'undefined') {
                fa = fa_icon;
            }
            var modal = $(messageBoxPane);
            modal.addClass(mt);
            modal.find(".modal-title").append('<i class="icon fa ' + fa + '"></i>&nbsp;').append(title);
            modal.find(".modal-body").append(message);
            modal.find('[data-dismiss="modal"]').click(function (e) {
                e.preventDefault();
                modal.remove();
            });
            if (typeof (onContinue) !== 'undefined') {
                modal.find('[data-bs-action="click"]').click(function (e) {
                    e.preventDefault();
                    modal.remove();
                    onContinue(e);
                }).show();
            };
            $("body").append(modal);
            modal.fadeIn('slow');
        }, // XNuvem.messageBox.show
        showDanger: function (title, message, onContinue) {
            XNuvem.messageBox.show(title, message, onContinue, XNuvem.messageBox.mbType.danger, "fa-warning");
        },
        showInfo: function (title, message, onContinue) {
            XNuvem.messageBox.show(title, message, onContinue, XNuvem.messageBox.mbType.info, "fa-info");
        },
        showWarning: function (title, message, onContinue) {
            XNuvem.messageBox.show(title, message, onContinue, XNuvem.messageBox.mbType.warning, "fa-warning");
        },
        showSuccess: function (title, message, onContinue) {
            XNuvem.messageBox.show(title, message, onContinue, XNuvem.messageBox.mbType.success, "fa-check");
        }
    }; // XNuvem.messageBox
    this.parseDate = function (value) {
        if (typeof value === 'string') {
            var d = /\/Date\((\d*)\)\//.exec(value);
            return (d) ? new Date(+d[1]) : value;
        }
        return value;
    }
    this.formatDate = function (value) {
        var v = this.parseDate(value);
        return Globalize('pt').formatDate(v);
    }
    this.formatNumber = function (n) {
        var value = (typeof n === 'string') ? parseFloat(n) : n;
        return Globalize('pt').formatNumber(value);
    }
    this.formatCurrency = function (n) {
        var value = (typeof n === 'string') ? parseFloat(n) : n;
        return Globalize('pt').formatCurrency(value, 'BRL');
    }
    this.pathCombine = function (p1, p2) {
        if (typeof p1 === 'undefined' || typeof p2 == 'undefined')
            return "";
        return p1.endsWith("/") ? p1 + p2 : p1 + "/" + p2;
    }
};

(function ($) {
    // function to clear all input in the forms
    $.fn.clearForm = function () {
        return this.each(function () {
            var type = this.type, tag = this.tagName.toLowerCase();
            if (tag == 'form')
                return $(':input', this).clearForm();
            if (type == 'text' || type == 'password' || tag == 'textarea')
                this.value = '';
            else if (type == 'checkbox' || type == 'radio')
                this.checked = false;
            else if (tag == 'select')
                this.selectedIndex = -1;
        });
    };
    // Hack: clear plugin select2
    $.fn.resetSelect2 = function () {
        var f = $(this);
        var s = f.find("[data-xn-select2]");
        if (s.length) {
            s.empty().trigger('change');
        }
    }

    $.fn.select2.defaults.set("width", "100%");

    $.fn.clearFormEx = function () {
        var f = $(this);
        var clear = false;
        var ckf = f.find("[data-xn-clear]");
        if (ckf.length) {
            clear = true;
            var ckfId = ckf.attr("data-xn-clear");
            clear = !($("#" + ckfId).val());
        }
        if (clear) {
            f.clearForm();
            f.resetSelect2(); // Hack: reset select2 after post
        }
    }
}(jQuery));


$(function () {
    // Set menu active
    var fullUrl = window.location.href;

    $(".sidebar-menu").find("a").each(function (i, s) {
        var that = $(this);
        if (fullUrl.indexOf(that.attr("href")) > 0) {
            that.parents(".treeview-menu").addClass("menu-open").css("display", "block");
            that.parents("li").addClass("active");
        }
    });

    if (typeof $.fn.select2 !== 'undefined') {
        // Initialize ComboBox plugin
        $("[data-xn-select2]").select2({
            language: "pt-BR",
            placeholder: "Selecione..."
        });
    }

    // Auto submit forms from xnuvem
    $("form[data-xn-form]").submit(function (e) {
        e.preventDefault();
        var form = $(this);
        form.validate();
        if (!form.valid()) return;
        var sb = form.find("button[data-xn-save]");
        sb.prop("disabled", true);
        var wtn = $('<span><i class="fa fa-refresh fa-spin"></i>&nbsp;&nbsp;</span>');
        sb.prepend(wtn); // Wainting...
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: form.serialize()
        }).done(function (data) {
            wtn.remove();
            sb.prop("disabled", false);
            if (data.IsError) {
                XNuvem.statusMessage.showError("Erro !", "Erro: " + data.Messages.join(","));
            } else {
                form.clearFormEx();
                if (data.Messages) {
                    XNuvem.statusMessage.showSuccess("Sucesso", data.Messages.join(","));
                } else { // Else sucess default message
                    XNuvem.statusMessage.showSuccess("Sucesso", "Operação completada com êxito.")
                }
            }
        }).fail(function (xhr) {
            wtn.remove();
            sb.prop("disabled", false);
            if (xhr.responseText) {
                XNuvem.statusMessage.showError("Erro interno", xhr.responseText);
            } else {
                XNuvem.statusMessage.showError("Erro", "O servidor não retornou nenhum valor ou não está acessível.");
            }
        });
    }); // $("form[data-xn-form]").submit()

    if (typeof $.fn.datepicker !== 'undefined') {
        // Initialize datepicker
        $('[data-xn-date]').datepicker({
            format: 'dd/mm/yyyy',
            language: 'pt-BR'
        }).on('focus', function (e) {
            $(this).inputmask({
                alias: "dd/mm/yyyy",
                onKeyUp: function (e, buffer, opts) {
                    var $input = $(this);
                    var inp = String.fromCharCode(e.keyCode);
                    if (/[a-zA-Z]/.test(inp)) {
                        var today = new Date();
                        $input.val(today.getDate().toString() + (today.getMonth() + 1).toString() + today.getFullYear().toString());
                        $input.datepicker("setDate", $input.val());
                    }
                }
            });
        });
    }

    if (typeof $.fn.iCheck !== 'undefined') {
        $('input[type="checkbox"].icheck').iCheck({
            checkboxClass: 'icheckbox_square-blue',
            radioClass: 'iradio_square-blue'
        });

        $('input[type="checkbox"].icheck').on('ifChanged', function (e) {
            $(this).trigger("change", e);
        });
    }
});