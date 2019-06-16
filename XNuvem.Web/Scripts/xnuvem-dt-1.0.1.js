/// <reference path="jquery-2.2.3.js" />
/// <reference path="xnuvem-app-1.0.0.js" />
/// <reference path="../content/plugins/datatables-1.10/datatables.js" />
/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

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

$(function () {
    var pathCombine = function (p1, p2) {
        if (typeof p1 === 'undefined' || typeof p2 == 'undefined')
            return "";
        return p1.endsWith("/") ? p1 + p2 : p1 + "/" + p2;
    }

    // TODO: Extend options to base dataTables and new functionalities
    var XNuvemTable = function (jdt, opts) {
        this.dt = $(jdt);
        this.linkButton = '<a class="btn btn-flat btn-default btn-xs" href="{0}" title="Editar"><i class="fa fa-arrow-right text-orange"></i></a>&nbsp;&nbsp;';
        this.toolbar = {
            panel: '<div class="xn_toolbar" />',
            btns: {
                add: '<a href="#" class="btn btn-app btn-success" title="Adicionar um novo item"><i class="fa fa-plus-square text-success"></i> Adicionar</a>',
                del: '<a href="#" class="btn btn-app btn-default" title="Excluir itens selecionados" data-xn-btn-delete><i class="fa fa-times-circle text-danger"></i> Excluir</a>'
            }
        };
        this.cl = this.dt.attr("data-xn-columns").split(","); // column list
        this.oc = this.dt.attr("data-xn-orderable").split(","); // orderable columns
        this.url = this.dt.attr("data-xn-url");
        var showSelAttr = this.dt.attr("data-xn-show-select");
        this.showSelect = "true" === showSelAttr;
        this.urlAdd =       this.dt.attr("data-xn-url-add");
        this.urlDelete =    this.dt.attr("data-xn-url-delete");
        this.keyColumn =    this.dt.attr("data-xn-key");
        this.linkColumns =  this.dt.attr("data-xn-link");
        this.spLinkUrls = [];
        this.spLinkCols = [];
        var that = this;
        if (that.linkColumns) {
            var sp = that.linkColumns.split(";");
            $.each(sp, function (i, o) {
                var colLink = o.split(":");
                that.spLinkCols.push(colLink[0]);
                that.spLinkUrls.push(colLink[1]);
            });
        }        
        this.searchTime = 0;
        this.searchDelay = 800;
        this._constructor();
    }

    $.extend(XNuvemTable.prototype, {
        _constructor: function () {
            var that = this;
            var opts = this._buildOptions();
            var odt = that.dt.dataTable(opts);
            var tb = $(that.toolbar.panel);
            if (that.urlAdd) {
                var addButton = $(that.toolbar.btns.add);
                addButton.attr("href", that.urlAdd);
                tb.append(addButton);
            }
            if (that.urlDelete) {
                var delButton = $(that.toolbar.btns.del);
                delButton.click(function (e) {
                    e.preventDefault();
                    var selection = odt.find('[aria-checked="true"] > input[type="checkbox"]');
                    if (selection.length) {
                        var keys = [];
                        selection.each(function (i, o) {
                            keys.push($(o).attr("data-xn-value"));
                        });
                        XNuvem.messageBox.showWarning("Atenção!", 'Esta operação não pode ser desfeita. Tem certeza que deseja excluir os itens selecionados?', function () {
                            that._postDelete(keys.join(";"));
                        });
                    } else {
                        XNuvem.messageBox.showInfo("Informação", "Nenhum item selecionado.");
                    }
                });
                tb.append(delButton);
            }
            if (tb.children().length > 0) {
                odt.parents(".dataTables_wrapper").prepend(tb);
            }
            odt.parents(".dataTables_wrapper")
                .find(".dataTables_filter input")
                .attr("placeholder", "Pesquisar por...")
		        .unbind() // Unbind previous default bindings
		        .bind("keydown", function (e) { // Bind our desired behavior
		            // Call when the keyCode equals to (enter)
		            if (e.keyCode == 13) {
		                that._searchStart(odt.api(), $(this).val());
		            }
		            return;
		        })
		        .bind("input", function (e) { // Bind our desired behavior
		            that._searchStart(odt.api(), $(this).val());
		            return;
		        });
            odt.on("draw.dt", function (xhre) {
                if ($.fn.iCheck) {
                    $('.dataTable input[type="checkbox"]').iCheck({
                        checkboxClass: 'icheckbox_square-blue',
                        radioClass: 'iradio_square-blue'
                    });
                }
            });
        },
        _buildOptions: function () {
            var opts = {};
            var ajaxOpt = this._getAjaxOpt();
            var colOpt = this._getColumnsOpt();
            var colDefOpt = this._getColumnsDefsOpt();
            var orderOpt = this._getInitialOrderOpt();
            $.extend(
                opts,
                XNuvemTable.defaults,
                {
                    ajax: ajaxOpt,
                    columns: colOpt,
                    columnDefs: colDefOpt
                },
                orderOpt);
            return opts;
        },
        _getAjaxOpt: function() {
            var a = {
                url: this.url,
                type: "POST",
                error: function (xhr, error, thrown) {
                    XNuvem.statusMessage.showError("Tabela de dados", xhr.responseText);
                }
            };
            return a;
        },
        _getColumnsOpt: function() {
            var cs = [];
            if (this.showSelect) {
                cs.push({ "data": null });
            };
            $(this.cl).each(function (i, c) {
                cs.push({ "data": c });
            });
            return cs;
        },
        _getColumnsDefsOpt: function () {
            var that = this;
            var cd = [];
            var initialIndex = 0;
            if (that.showSelect) {
                initialIndex = 1;
                cd.push({
                    "orderable": false,
                    "targets": [0],
                    "render": function (data, type, row) {
                        if (that.keyColumn) {
                            return '&nbsp;&nbsp;<input type="checkbox" data-xn-value="{0}" />'.replace("{0}", row[that.keyColumn]);
                        }
                        else {
                            return '&nbsp;&nbsp;<input type="checkbox" />';
                        }
                    }
                });
            }
            $(this.cl).each(function (i, c) {
                var orderable = that.oc.indexOf(c) > -1;
                // if link column use render the link
                var li = that.spLinkCols.indexOf(c)
                if (li > -1) {
                    var colLnk = that.spLinkUrls[li];
                    var colKey = that._getColumnFromLink(colLnk);                    
                    cd.push({
                        "orderable": orderable,
                        "targets": [i + initialIndex],
                        "render": function (data, type, row, meta) {
                            var u = colLnk.replace("{" + colKey + "}", row[colKey]);
                            return that.linkButton.replace("{0}", u) + data;
                        }
                    });
                } else {
                    cd.push({
                        "orderable": orderable,
                        "targets": [i + initialIndex]
                    });
                }
            });
            return cd;
        },
        _getInitialOrderOpt: function () {
            var that = this;
            var initialIndex = that.showSelect ? 1 : 0;
            if (that.oc && that.oc[0]) {
                var colIndex = that.cl.indexOf(that.oc[0]) + initialIndex;
                return {
                    "order": [[colIndex, 'asc']]
                };
            }
            return {};
        },
        _searchStart: function (d, v) {
            var that = this;
            if (that.searchTime) {
                clearTimeout(that.searchTime);
                that.searchTime = 0;
            }
            that.searchTime = setTimeout(function () {
                d.search(v).draw();
            }, that.searchDelay);
        },
        _getColumnFromLink: function (l) {
            var m = l.match(/{(.+)}/i);
            return m ? m[1] : "";
        },
        _doSearch: function () {
            var val = this.dt.parents(".dataTables_wrapper")
                .find(".dataTables_filter input").val();
            var d = this.dt.dataTable().api();
            d.search(val).draw();
        },
        _postDelete: function (ks) {
            var that = this;
            var postData = { keys: ks };
            $.ajax({
                method: 'POST',
                url: that.urlDelete,
                data: postData
            })
            .done(function (data) {
                if (data.IsError) {
                    XNuvem.statusMessage.showError("Erro", data.Messages.join(". "));
                } else {
                    XNuvem.statusMessage.showSuccess("Sucesso", data.Messages.join(". "));
                }
                that._doSearch();
            })
            .fail(function (jqXHR, responseText) {
                XNuvem.statusMessage.showError("Erro", jqXHR.responseText);
                that._doSearch();
            });
        }
    });

    XNuvemTable.defaults = {
        pageLength: 25,
        processing: true,
        serverSide: true,
        responsive: true,
        language: {
            "sEmptyTable": "Nenhum registro encontrado",
            "sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
            "sInfoFiltered": "(Filtrados de _MAX_ registros)",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLengthMenu": "_MENU_ resultados por página",
            "sLoadingRecords": '<i class="fa fa-refresh fa-spin"></i> Carregando...',
            "sProcessing": '<i class="fa fa-refresh fa-spin"></i> Processando...',
            "sZeroRecords": "Nenhum registro encontrado",
            "sSearch": "Pesquisar",
            "oPaginate": {
                "sNext": "Próximo",
                "sPrevious": "Anterior",
                "sFirst": "Primeiro",
                "sLast": "Último"
            },
            "oAria": {
                "sSortAscending": ": Ordenar colunas de forma ascendente",
                "sSortDescending": ": Ordenar colunas de forma descendente"
            }
        }
    }

    $.fn.xnTable = function() {
        return this.each(function () {
            new XNuvemTable(this);
        });
    }

    // Set default error mode for dataTable to XNuvem mode
    $.fn.dataTable.ext.errMode = function (settings, techNote, message) {
        XNuvem.statusMessage.showError("Tabela de dados", message);
        return false;
    }

    $(document).ready(function () {
        $("[data-xn-dt]").xnTable();
    });
});