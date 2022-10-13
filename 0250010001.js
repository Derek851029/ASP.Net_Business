
$(function () {
    List_Location();
    List_Team();
});

function List_Location() {
    $.ajax({
        url: '0250010001.aspx/List_Location',
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        dataType: "json",       //如果要回傳值，請設成 json
        success: function (doc) {
            var table = $('#data').DataTable({
                destroy: true,
                data: eval(doc.d), "oLanguage": {
                    "sLengthMenu": "顯示 _MENU_ 筆記錄",
                    "sZeroRecords": "無符合資料",
                    "sInfo": "顯示第 _START_ 至 _END_ 項結果，共 _TOTAL_ 項",
                    "sInfoFiltered": "(從 _MAX_ 項結果過濾)",
                    "sInfoPostFix": "",
                    "sSearch": "搜索:",
                    "sUrl": "",
                    "oPaginate": {
                        "sFirst": "首頁",
                        "sPrevious": "上頁",
                        "sNext": "下頁",
                        "sLast": "尾頁"
                    }
                },
                "columnDefs": [{
                    "targets": -1,
                    "data": null,
                    "searchable": false,
                    "paging": false,
                    "ordering": false,
                    "info": false
                }],
                columns: [
                   { data: "SYSID" },
                    { data: "mission_name" },
                    { data: "Work_Time" },
                    { data: "Create_Date" },
                    { data: "person_charge" },
                    {
                        data: "SYSID", render: function (data, type, row, meta) {
                            return "<button id='edit' type='button' class='btn btn-primary btn-lg' data-toggle='modal' data-target='#myModal'>" +
                                "<span class='glyphicon glyphicon-pencil'></span>&nbsp;修改</button>"
                        }
                    }
                ]
            });
            //==========================
            $('#data tbody').unbind('click').
                on('click', '#edit', function () {
                    var table = $('#data').DataTable();
                    var file = table.row($(this).parents('tr')).data().file;
                    var id = table.row($(this).parents('tr')).data().SYSID;
                    Get_info(file, id);
                });
        }
    });
};

function Add_New() {
    document.getElementById("New_Location").value = '';
    document.getElementById("New_Time").value = '';
    document.getElementById("New_Team").value = '';
    var $select_elem = $("#New_Agent");
    $select_elem.empty();
    $select_elem.append("<option value=''>" + "請選擇檢查人員…" + "</option>");
}

function New_info() {
    $("#MAP div[id='MAP']").remove();
    var a = "<img src='/Patrol_System/NULL.png' id='img_MAP' />";
    var num = "";

    for (i = 1; i < 11; i++) {
        num = String.fromCharCode(i + 64);
        a += "<div id='D_" + num + "' class='drag'><img src='/Patrol_System/" + num + ".png' id='img_" + num + "' /></div>";
    }

    document.getElementById("MAP").innerHTML = a;
    $("#MAP div[id^='D_']").draggable(
     {
         containment: "#MAP",
         drag: function (event, ui) {
             //console.log('A', ui.position.left);
             //console.log('A', ui.position.top);
             //$("#x").val(ui.position.left);
             //$("#y").val(ui.position.top);
         }
     });
    var ul = "<ul id='Tab' class='nav nav-tabs'><li class='active'>";
    var tab = "<div class='tab-content'><div class='tab-pane active' ";
    var i = 1;
    for (i = 1; i < 11; i++) {
        if (i != 1) { ul += "<li>" }
        if (i != 1) { tab += "<div class='tab-pane' " }
        b = String.fromCharCode(i + 64);
        ul += "<a href='#li_point_" + b + "' data-toggle='tab'>" + b + "</a></li>";
        tab += " id='li_point_" + b + "'>" +
            "<table class='display table table-striped' style='width: 99%'>" +
            "<thead>" +
            "<tr>" +
            "<th style='text-align: center' colspan='4'>" +
            "<div class='col-lg-4 col-lg-offset-4'>" +
            "<input id='title_" + b + "' class='form-control' value='" + b + "' maxlength='15' onkeyup='cs(this);'" +
            "style='width: 100%; Font-Size: 18px; background-color: #ffffbb; color: #333333;' />" +
            "</div>" +
            "</th>" +
            "</tr>" +
            "</thead>" +
            "<tbody>";
        //=====================================================================

        //======= 五個題目 =======
        var q = 1;
        for (q = 1; q < 6; q++) {
            tab += "<tr>" +
                "<th style='text-align: center; width: 25%; height: 55px;'><strong>檢查內容</strong></th>" +
                    "<th style='text-align: center; width: 75%'>" +
                    "<input id='txt_" + b + q + "' class='form-control' value='' maxlength='50' onkeyup='cs(this);' " +
                    " style='width: 100%; background-color: #ffffbb; Font-Size: 18px;' />" +
                    "</th>" +
                    "</tr>" +
                    //=====================================================================
                    "<tr>" +
                    "<th style='text-align: center; height: 55px;'>" +
                    "<strong>回答類型</strong>" +
                    "</th>" +
                    "<th style='text-align: center;'>" +
                    "<select id='select_" + b + q + "' class='form-control' onchange=Change_Type('" + b + q + "'); " +
                    " style='Font-Size: 18px; background-color: #ffffbb;' >" +
                    "<option value='0'>選擇</option>" +
                    "<option value='1'>填空</option>" +
                    "</select></th>" +
                    "</tr>" +
                    //====================================================================
                    "<tr>" +
                    "<th style='text-align: center;'>" +
                    "<strong>回答內容</strong></th>" +
                    "<th id='" + b + q + "_1'>" +
                    "<div id='" + b + q + "_2'> </div>" +
                    "</th>" +
                    "</tr>";
            Change_Type(b + q);
        }
        //====================================================================
        tab += "</tbody>" +
         "</table>" +
         "</div>";
    }
    document.getElementById("table_madal").innerHTML = ul + "</ul>" + tab + "</div>";  //寫入 table_madal
}

function Get_info(file, id) {
    $("#MAP div[id='MAP']").remove();
    $("#Tab").remove();
    document.getElementById("table_madal").innerHTML = "";
    $.ajax({
        url: '0250010001.aspx/Get_info',
        type: 'POST',
        data: JSON.stringify({ ID: id }),
        contentType: 'application/json; charset=UTF-8',
        dataType: "json",
        success: function (doc) {
            var obj = JSON.parse('{"data":' + doc.d + '}');
            if (obj.data[0].status == '') {
                var a = "<img src='/Patrol_System/" + file + "' id='img_MAP' />";
                document.getElementById("MAP").innerHTML = a;
                var total = obj.data.length;
                var b = "";
                var ul = "<ul id='Tab' class='nav nav-tabs'><li class='active'>";
                var tab = "<div class='tab-content'><div class='tab-pane active' ";
                var i = 0;
                for (i = 0; i < total; i++) {
                    if (i != 0) { ul += "<li>" }
                    if (i != 0) { tab += "<div class='tab-pane' " }
                    b = obj.data[i].bit;
                    a = "<div id='D_" + b + "' class='drag'><img src='/Patrol_System/" + b + ".png' id='img_" + b + "' /></div>"
                    ul += "<a href='#li_point_" + b + "' data-toggle='tab'>" + b + "</a></li>";
                    tab += " id='li_point_" + b + "'>" +
                        "<table class='display table table-striped' style='width: 99%'>" +
                        "<thead>" +
                        "<tr>" +
                        "<th style='text-align: center' colspan='4'>" +
                        "<div class='col-lg-4 col-lg-offset-4'>" +
                        "<input id='title_" + b + "' class='form-control' value='" + obj.data[i].name + "' maxlength='15' onkeyup='cs(this);'" +
                        "style='width: 100%; Font-Size: 18px; background-color: #ffffbb; color: #333333;' />" +
                        "</div>" +
                        "</th>" +
                        "</tr>" +
                        "</thead>" +
                        "<tbody>";
                    //=====================================================================

                    //======= 五個題目 =======
                    var q = 1;
                    var value = "";
                    for (q = 1; q < 6; q++) {
                        switch (q) {
                            case 1: value = obj.data[i].q_1; break;
                            case 2: value = obj.data[i].q_2; break;
                            case 3: value = obj.data[i].q_3; break;
                            case 4: value = obj.data[i].q_4; break;
                            default: value = obj.data[i].q_5;
                        }
                        if (value != "") {
                            tab += "<tr>" +
                                "<th style='text-align: center; width: 25%; height: 55px;'><strong>檢查內容</strong></th>" +
                                "<th style='text-align: center; width: 75%'>" +
                                "<input id='txt_" + b + q + "' class='form-control' value='" + value + "' maxlength='50' onkeyup='cs(this);' " +
                                " style='width: 100%; background-color: #ffffbb; Font-Size: 18px;' />" +
                                "</th>" +
                                "</tr>" +
                                //=====================================================================
                                "<tr>" +
                                "<th style='text-align: center; height: 55px;'>" +
                                "<strong>回答類型</strong>" +
                                "</th>" +
                                "<th style='text-align: center;'>" +
                                "<select id='select_" + b + q + "' class='form-control' onchange=Change_Type('" + b + q + "'); " +
                                " style='Font-Size: 18px; background-color: #ffffbb;' >" +
                                "<option value='0'>選擇</option>" +
                                "<option value='1'>填空</option>" +
                                "</select></th>" +
                                "</tr>" +
                                //====================================================================
                                "<tr>" +
                                "<th style='text-align: center;'>" +
                                "<strong>回答內容</strong></th>" +
                                "<th id='" + b + q + "_1'>" +
                                "<div id='" + b + q + "_2'> </div>" +
                                "</th>" +
                                "</tr>";
                            Change_Type(b + q);
                        }
                    }
                    //====================================================================
                    tab += "</tbody>" +
                     "</table>" +
                     "</div>";
                    document.getElementById("MAP").innerHTML += a;  // 寫入藍色 icon
                    var Div = document.getElementById("D_" + b);
                    Div.style.top = obj.data[i].x + "px";
                    Div.style.left = obj.data[i].y + "px";
                }
                document.getElementById("table_madal").innerHTML = ul + "</ul>" + tab + "</div>";  //寫入 table_madal
                $("#MAP div[id^='D_']").draggable(
                    {
                        containment: "#MAP",
                        drag: function (event, ui) {
                            console.log('A', ui.position.left);
                            console.log('A', ui.position.top);
                            //$("#x").val(ui.position.left);
                            //$("#y").val(ui.position.top);
                        }
                    });
            } else { alert(obj.data[0].status); };
        }
        //=========================
    });
}

function Change_Type(ID) {
    $.ajax({
        url: '0250010001.aspx/Check',
        type: 'POST',
        data: JSON.stringify({ ID: ID }),
        contentType: 'application/json; charset=UTF-8',
        dataType: "json",
        success: function (doc) {
            var a = doc.d;
            var b = document.getElementById("select_" + a).value;
            $("#" + a + "_2").remove();
            document.getElementById(a + "_1").innerHTML = From_Add(a, b);
        }
    });
}

function From_Add(ID, type) {
    var a = "<div id='" + ID + "_2'>"
    if (type == 0) {
        a += "<div class='form-check'>" +
            "<label class='form-check-label'>" +
            "<input type='radio' class='form-check-input' name='" + ID + "' disabled/>" +
            "有</label></div>" +
            "<div class='form-check'>" +
            "<label class='form-check-label'>" +
            "<input type='radio' class='form-check-input' name='" + ID + "' disabled/>" +
            "沒有</label></div>";
    }
    else {
        a += "<div>單位：<input id='Q_" + ID + "' class='form-control' maxlength='2' " +
            "style='width: 10%; background-color: #ffffbb; Font-Size: 18px;' /></div>";
    }
    return a + "</div>";
}

function List_Team() {
    var $select_elem = $("#New_Team");
    $.ajax({
        url: '0250010001.aspx/List_Team',
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        dataType: "json",
        success: function (doc) {
            var json = JSON.parse(doc.d);
            $select_elem.empty();
            $select_elem.append("<option value=''>" + "請選擇部門…" + "</option>");
            $.each(json, function (idx, obj) {
                $select_elem.append("<option value='" + obj.Agent_Team + "'>" + obj.Agent_Team + "</option>");
            });
        }
    });
}

function List_Agent(Team) {
    var $select_elem = $("#New_Agent");
    $.ajax({
        url: '0250010001.aspx/List_Agent',
        type: 'POST',
        data: JSON.stringify({ Team: Team }),
        contentType: 'application/json; charset=UTF-8',
        dataType: "json",
        success: function (doc) {
            var json = JSON.parse(doc.d);
            $select_elem.empty();
            $select_elem.append("<option value=''>" + "請選擇檢查人員…" + "</option>");
            $.each(json, function (idx, obj) {
                $select_elem.append("<option value='" + obj.Agent_ID + "'>" + obj.Agent_Name + "</option>");
            });
        }
    });
}

function Change_Team() {
    var s = document.getElementById("New_Team");
    var str_value = s.options[s.selectedIndex].value;
    List_Agent(str_value);
}