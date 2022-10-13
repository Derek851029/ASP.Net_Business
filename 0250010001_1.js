$(function () { List_Location() });
function List_Location() {
    $.ajax({
        url: "0250010001.aspx/List_Location", type: "POST", data: JSON.stringify({}), contentType: "application/json; charset=UTF-8", dataType: "json", success: function (a) {
            $("#data").DataTable({
                destroy: !0, data: eval(a.d), oLanguage: {
                    sLengthMenu: "\u986f\u793a _MENU_ \u7b46\u8a18\u9304", sZeroRecords: "\u7121\u7b26\u5408\u8cc7\u6599", sInfo: "\u986f\u793a\u7b2c _START_ \u81f3 _END_ \u9805\u7d50\u679c\uff0c\u5171 _TOTAL_ \u9805", sInfoFiltered: "(\u5f9e _MAX_ \u9805\u7d50\u679c\u904e\u6ffe)", sInfoPostFix: "",
                    sSearch: "\u641c\u7d22:", sUrl: "", oPaginate: { sFirst: "\u9996\u9801", sPrevious: "\u4e0a\u9801", sNext: "\u4e0b\u9801", sLast: "\u5c3e\u9801" }
                }, columnDefs: [{ targets: -1, data: null, searchable: !1, paging: !1, ordering: !1, info: !1 }], columns: [{ data: "SYSID" }, { data: "mission_name" }, { data: "Work_Time" }, { data: "Create_Date" }, { data: "person_charge" }, { data: "SYSID", render: function (c, e, h, a) { return "<button id='edit' type='button' class='btn btn-primary btn-lg' data-toggle='modal' data-target='#myModal'><span class='glyphicon glyphicon-pencil'></span>&nbsp;\u4fee\u6539</button>" } }]
            });
            $("#data tbody").unbind("click").on("click", "#edit", function () { var c = $("#data").DataTable(), a = c.row($(this).parents("tr")).data().file, c = c.row($(this).parents("tr")).data().SYSID; Get_info(a, c) })
        }
    })
}
function New_info() { $("#MAP div[id='MAP']").remove(); document.getElementById("MAP").innerHTML = "<img src='/Patrol_System/004.png' id='img_MAP' /><div id='D_A' class='drag'><img src='/Patrol_System/A.png' id='img_A' /></div>"; var a = document.getElementById("D_A"); a.style.top = "400px"; a.style.left = "550px"; $("#MAP div[id^='D_']").draggable({ containment: "#MAP", drag: function (a, e) { } }) }
function Get_info(a, c) {
    alert("456"); $("#MAP div[id='MAP']").remove(); $("#Tab").remove(); var e = "<img src='/Patrol_System/" + a + "' id='img_MAP' />"; document.getElementById("MAP").innerHTML = e; $.ajax({
        url: "0250010001.aspx/Get_info", type: "POST", data: JSON.stringify({ ID: c }), contentType: "application/json; charset=UTF-8", dataType: "json", success: function (a) {
            a = JSON.parse('{"data":' + a.d + "}"); for (var c = a.data.length, d = 0, b = "", f = "<ul id='Tab' class='nav nav-tabs'><li class='active'>", g = "<div class='tab-content'><div class='tab-pane active' ",
            d = 0; d < c; d++) 0 != d && (f += "<li>"), 0 != d && (g += "<div class='tab-pane' "), b = a.data[d].bit, e = "<div id='D_" + b + "' class='drag'><img src='/Patrol_System/" + b + ".png' id='img_" + b + "' /></div>", f += "<a href='#li_point_" + b + "' data-toggle='tab'>\u5de1\u67e5\u9ede" + b + "</a></li>", g += " id='li_point_" + b + "'><table class='display table table-striped' style='width: 99%'><thead><tr><th style='text-align: center' colspan='4'><span style='font-size: 20px'><strong>" + a.data[d].name + "</strong></span></th></tr></thead><tbody></tbody></table></div>",
            document.getElementById("MAP").innerHTML += e, b = document.getElementById("D_" + b), b.style.top = a.data[d].x + "px", b.style.left = a.data[d].y + "px"; document.getElementById("table_madal").innerHTML = f + "</ul>" + g + "</div>"; $("#MAP div[id^='D_']").draggable({ containment: "#MAP", drag: function (a, b) { console.log("A", b.position.left); console.log("A", b.position.top) } })
        }
    })
};