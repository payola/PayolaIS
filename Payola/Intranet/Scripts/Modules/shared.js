var bindMode = document.location.href.match("Relation");

var payola = {
    suggestor: {
        id: (bindMode ? "RelatedEntityId" : "Suggest"),
        dictionary: function(elem) {

            var url = "";

            if (bindMode) {
                parts = document.location.pathname.split( /\// );
                url += "/" + parts[1] + "/SearchForRelatableEntities";
            } else {
                url = document.location.href + "/Search";
            }

            return url;
        },
        params: function(elem, request, extractLast) {
            var out = {
                needle: extractLast(request.term)
            };

            if (bindMode) {
                out.entityId = $("#EntityId").val(),
                out.relationTypeId = $("#RelationTypeId").val()
            }

            return out;
        }
    }
};


$(function () {
    //$(".tabs > ul").tabs();
    $("textarea").elastic();
    $(".datepicker").datepicker();
    $(".datetimepicker").datetimepicker();
    $("select").chosen();
});