function disableCreate() {
    $("#suggest-submit").hide();
}

function enableCreate() {
    $("#suggest-submit").show();
}

$(function () {
    if ($("[id^=" + payola.suggestor.id + "]").length > 0) {

        disableCreate();

        function split(val) {
            return val.split(/,\s*/);
        }
        function extractLast(term) {
            return split(term).pop();
        }

        $("[id^=" + payola.suggestor.id + "]").each(function () {
            if (bindMode) {
                $(this).prev("label").text("Vyhledat:");
            }

            var complete = $(this);

            $(this)
            // don't navigate away from the field on tab when selecting an item
			    .bind("keydown", function (event) {

			        if (event.keyCode === $.ui.keyCode.TAB &&
						    $(this).data("autocomplete").menu.active) {
			            event.preventDefault();
			        }

			        if (event.keyCode == $.ui.keyCode.ENTER) {
			            event.preventDefault();

			            if ($("li", $(this).data("autocomplete").menu.element).length == 1) {
			                var item = $("li", $(this).data("autocomplete").menu.element).eq(0).data("item.autocomplete");

			                this.value = item.Identification;
			                disableCreate();

			                if (bindMode) {
			                    this.value = item.Id;
			                    $(this).hide();
			                    var btn = $(this).next('input[type=submit]');
			                    $(this).after("<span>Navazuji...</span>");

			                    var form = complete.parents("form").eq(0);
			                    form.attr('action', form.attr("action").replace("/CreateAndAddRelation", "/AddRelation"));
			                    
			                    btn.click();
			                    btn.hide();
			                } else {
			                    document.location.href += "/Detail/" + item.Id;
			                }
			            } else if (bindMode) {

			                if ($("#ui-active-menuitem", $(this).data("autocomplete").menu.element).length == 0) {
			                    $(this).hide();
			                    var btn = $(this).next('input[type=submit]');
			                    $(this).after("<span>Navazuji...</span>");

			                    var form = complete.parents("form").eq(0);
			                    form.attr('action', form.attr("action").replace("/AddRelation", "/CreateAndAddRelation"));
			                    btn.click();
			                    btn.hide();
			                }
			            }

			            return false;
			        }
			    })
			    .autocomplete({
			        source: function (request, response) {
			            $.post(payola.suggestor.dictionary(this),
                            payola.suggestor.params(this, request, extractLast),
                            function (data) {
                                enableCreate();

                                var result = eval(data);
                                var form = complete.parents("form").eq(0);
                                form.attr('action', form.attr("action").replace("/AddRelation", "/CreateAndAddRelation"));

                                response(result);
                            }, 'json');
			        },
			        search: function () {
			            // custom minLength
			            var term = extractLast(this.value);
			            if (term.length < 2) {
			                return false;
			            }
			        },
			        focus: function () {
			            // prevent value inserted on focus
			            return false;
			        },
			        select: function (event, ui) {
			            if (bindMode) {
			                this.value = ui.item.Id;
			                $(this).hide();
			                var btn = $(this).next('input[type=submit]');
			                $(this).after("<span>Navazuji...</span>");

			                var form = complete.parents("form").eq(0);
			                form.attr('action', form.attr("action").replace("/CreateAndAddRelation", "/AddRelation"));

			                btn.click();
			                btn.hide();
			            } else {
			                this.value = ui.item.Identification;
			                disableCreate();

			                document.location.href += "/Detail/" + ui.item.Id;
			            }

			            return false;
			        }
			    })
                .data("autocomplete")._renderItem = function (ul, item) {
                    return $("<li></li>")
		                .data("item.autocomplete", item)
		                .append("<a>" + item.Identification + "</a>")
		                .appendTo(ul);
                };
        });
    }
});