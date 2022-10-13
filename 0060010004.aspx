<%@ Page Language="C#" EnableEventValidation="true" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="0060010004.aspx.cs" Inherits="_0060010004" %>

<asp:Content ID="Content" ContentPlaceHolderID="head2" runat="Server">
    <link href="../DataTables/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="../bootstrap-chosen-master/bootstrap-chosen.css" rel="stylesheet" />
    <script src="../DataTables/jquery.dataTables.min.js"></script>
    <script src="../chosen/chosen.jquery.js"></script>
    <script src="../js/jquery.validate.min.js"></script>
    <script type="text/javascript">
        var Role_ID = '<%= Session["RoleID"] %>';
        var Agent_ID = '<%= Session["UserID"] %>';
        var array_mno = [];
        $(function () {
            $('[data-toggle="tooltip"]').tooltip();
            style("Agent_LV", "10");
            style("Role_ID", "100");
            //Agent_Company_List();
            //Agent_Team_List();
            ROLELIST_List();
            bindTable();
            Company_List();
        });

        //=============================================
        function bindTable() {
            $.ajax({
                url: '0060010004.aspx/GetPartnerList',
                type: 'POST',
                data: JSON.stringify({ Array: array_mno }),
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
                        "aLengthMenu": [[25, 50, 100], [25, 50, 100]],
                        "iDisplayLength": 100,
                        "columnDefs": [{
                            "targets": -1,
                            "data": null,
                            "searchable": false,
                            "paging": false,
                            "ordering": false,
                            "info": false
                        }],
                        columns: [
                                //{ data: "Agent_ID" }, //一般使用者不需要顯示員工編號, 顯示SYSID就好
                                { data: "SYSID" },
                                { data: "Agent_Name" },
                                { data: "Agent_Company" },
                                { data: "Agent_Team" },
                                {
                                    data: "SYSID", render: function (data, type, row, meta) {
                                        return "<button type='button' class='btn btn-primary btn-lg' id='button' " +
                                            "data-toggle='modal' data-target='#myModal' >" +
                                            "<span class='glyphicon glyphicon-pencil'>" +
                                            "</span>&nbsp;修改</button>";
                                    }
                                },
                                {
                                    data: "SYSID", render: function (data, type, row, meta) {
                                        /*if (row.Agent_LV == '30') {
                                            return "";
                                        } else {
                                            return "<button type='button' class='btn btn-danger btn-lg' id='delete'>" +
                                                "<span class='glyphicon glyphicon-remove'>" +
                                                "</span>&nbsp;刪除</button>";
                                        }//*/
                                        return "<button type='button' class='btn btn-danger btn-lg' id='delete'>" +
                                                "<span class='glyphicon glyphicon-remove'>" +
                                                "</span>&nbsp;刪除</button>";
                                    }
                                },
                        ]
                    });
                    //=========================================================
                    $('#data tbody').unbind('click').
                        on('click', '#button', function () {
                            var table = $('#data').DataTable();
                            var cno = table.row($(this).parents('tr')).data().Agent_Name;
                            Button(cno);
                        })
                        .on('click', '#delete', function () {
                            var table = $('#data').DataTable();
                            var cno = table.row($(this).parents('tr')).data().Agent_Name;
                            Delete(cno);
                        });
                    //=========================================================
                }
            });
        }

        //=============================================
        function Company_List() {
            $.ajax({
                url: '0060010004.aspx/Agent_Company_List',
                type: 'POST',
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#data_company').DataTable({
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
                        "aLengthMenu": [[25, 50, 100], [25, 50, 100]],
                        "iDisplayLength": 25,
                        "columnDefs": [{
                            "targets": -1,
                            "data": null,
                            "searchable": false,
                            "paging": false,
                            "ordering": false,
                            "info": false
                        }],
                        columns: [
                                { data: "Agent_Company" },
                                {
                                    data: "Agent_Company", render: function (data, type, row, meta) {
                                        return "<div class='checkbox'><label>" +
                                           "<input type='checkbox' style='width: 30px; height: 30px;' id='chack' />" +
                                           "</label></div>";
                                    }
                                }]
                    });
                    //==========================================================
                    $('#data_company tbody').on('click', '#chack', function () {
                        var table = $('#data_company').DataTable();
                        var cno = table.row($(this).parents('tr')).data().Agent_Company;
                        var a = this.checked;
                        if (a == true) {
                            if (array_mno.length > 4) {
                                table.row($(this).prop('checked', false));
                                alert('最多只能勾選五筆');
                            } else {
                                array_mno.push(cno);
                            }
                        }
                        else {
                            array_mno.splice($.inArray(cno, array_mno), 1);
                        }
                    });
                    //==========================================================
                }
            });
        }

        //================ 帶入【所屬公司】資訊 ===============
        function Agent_Company_List() {
            $.ajax({
                url: '0060010004.aspx/Agent_Company_List',
                type: 'POST',
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d);
                    var $select_elem = $("#select_Agent_Company");
                    $select_elem.chosen("destroy")
                    $.each(json, function (idx, obj) {
                        $select_elem.append("<option value='" + obj.Agent_Company + "'>" + obj.Agent_Company + "</option>");
                    });
                    $select_elem.chosen({
                        width: "100%",
                        search_contains: true
                    });
                }
            });
        };

        function Agent_Company() {
            var company = document.getElementById("select_Agent_Company").value;
            document.getElementById("txt_Agent_Company").value = company.trim();
        };

        //================ 帶入【所屬部門】資訊 ===============
        function Agent_Team_List() {
            $.ajax({
                url: '0060010004.aspx/Agent_Team_List',
                type: 'POST',
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d);
                    var $select_elem = $("#select_Agent_Team");
                    $select_elem.chosen("destroy")
                    $.each(json, function (idx, obj) {
                        $select_elem.append("<option value='" + obj.Agent_Team + "'>" + obj.Agent_Team + "</option>");
                    });
                    $select_elem.chosen({
                        width: "100%",
                        search_contains: true
                    });
                }
            });
        };

        function Agent_Team() {
            var team = document.getElementById("select_Agent_Team").value;
            document.getElementById("txt_Agent_Team").value = team.trim();
        };

        //================ 帶入【系統選單權限】資訊 ===============
        function ROLELIST_List() {
            $.ajax({
                url: '0060010004.aspx/ROLELIST_List',
                type: 'POST',
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d);
                    var $select_elem = $("#Role_ID");
                    $select_elem.chosen("destroy")
                    $.each(json, function (idx, obj) {
                        $select_elem.append("<option value='" + obj.ROLE_ID + "'>" + obj.ROLE_NAME + "</option>");
                    });
                    $select_elem.chosen({
                        width: "100%",
                        search_contains: true
                    });
                }
            });
        };

        //================ 帶入【員工】資訊 ===============
        function Xin_De() {
            Button("0");
        }

        function Button(Agent_Name) {
            //style("select_Agent_Company", "");
            //style("select_Agent_Team", "");
            style("Agent_LV", "10");
            style("Role_ID", "100");
            document.getElementById("txt_Agent_ID").value = "";
            document.getElementById("txt_Agent_Name").value = "";
            document.getElementById("txt_Agent_Company").value = "";
            document.getElementById("txt_Agent_Team").value = "";
            document.getElementById("txt_Agent_Mail").value = "";
            document.getElementById("txt_Agent_Co_TEL").value = "";
            document.getElementById("txt_Agent_Phone_2").value = "";
            //document.getElementById("txt_Agent_Phone_3").value = "";
            //document.getElementById("txt_code").value = "";
            //document.getElementById("txt_mvpn").value = "";
            document.getElementById("txt_UserID").value = "";
            document.getElementById("txt_Password").value = "";
            document.getElementById("chk_Password").value = "";
            document.getElementById("txt_Agent_ID").disabled = false;
            document.getElementById("txt_Agent_ID").style.display = "none";
            document.getElementById("div_Agent_ID").style.display = "none";
            document.getElementById("btn_new").style.display = "none";
            document.getElementById("btn_update").style.display = "none";
            $("#txt_hid_id").val(Agent_ID);
            if (Agent_Name == '0') {
                document.getElementById("btn_new").style.display = "";
                document.getElementById("txt_Agent_ID").style.display = "";
                document.getElementById("txt_title").innerHTML = "員工資料（新增）";
            } else {
                document.getElementById("txt_Agent_ID").disabled = true;
                document.getElementById("div_Agent_ID").style.display = "";
                document.getElementById("btn_update").style.display = "";
                document.getElementById("txt_title").innerHTML = "員工資料（修改）";
                //===========================================
                $.ajax({
                    url: '0060010004.aspx/DispatchSystem',
                    type: 'POST',
                    data: JSON.stringify({
                        Agent_Name: Agent_Name,
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                        var obj = JSON.parse('{"data":' + doc.d + '}');
                        document.getElementById("txt_Agent_ID").value = obj.data[0].Agent_ID;
                        document.getElementById("lab_Agent_ID").innerHTML = obj.data[0].Agent_ID;
                        document.getElementById("txt_Agent_Name").value = obj.data[0].Agent_Name;
                        document.getElementById("txt_Agent_Company").value = obj.data[0].Agent_Company;
                        //style("select_Agent_Company", obj.data[0].Agent_Company);
                        document.getElementById("txt_Agent_Team").value = obj.data[0].Agent_Team;
                        //style("select_Agent_Team", obj.data[0].Agent_Team);
                        document.getElementById("txt_Agent_Mail").value = obj.data[0].Agent_Mail;
                        document.getElementById("txt_Agent_Co_TEL").value = obj.data[0].Agent_Co_TEL;
                        document.getElementById("txt_Agent_Phone_2").value = obj.data[0].Agent_Phone_2;
                        //document.getElementById("txt_Agent_Phone_3").value = obj.data[0].Agent_Phone_3;
                        //document.getElementById("txt_code").value = obj.data[0].Agent_Code;
                        //document.getElementById("txt_mvpn").value = obj.data[0].Agent_MVPN;
                        document.getElementById("txt_UserID").value = obj.data[0].UserID;
                        style("Agent_LV", obj.data[0].Agent_LV);
                        style("Role_ID", obj.data[0].Role_ID);
                    }
                });
                //===========================================
            }
        };

        function Delete(Agent_Name) {
            if (confirm("確定要刪除嗎？")) {
                $.ajax({
                    url: '0060010004.aspx/Delete',
                    ache: false,
                    type: 'POST',
                    //async: false,
                    data: JSON.stringify({ Agent_Name: Agent_Name }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",       //如果要回傳值，請設成 json
                    success: function (data) {
                        var json = JSON.parse(data.d.toString());
                        if (json.status == "success") {
                            bindTable();
                        }
                    }
                });
            }
        };

        function style(Name, value) {
            var $select_elem = $("#" + Name);
            $select_elem.chosen("destroy")
            document.getElementById(Name).value = value;
            $select_elem.chosen({
                width: "100%",
                search_contains: true
            });
        }

        function New_Agent() {
            document.getElementById("btn_update").disabled = true;
            document.getElementById("btn_new").disabled = true;
            var Flag;
            var Agent_ID = document.getElementById("txt_Agent_ID").value;
            var Agent_Name = document.getElementById("txt_Agent_Name").value;
            var Agent_Company = document.getElementById("txt_Agent_Company").value;
            var Agent_Team = document.getElementById("txt_Agent_Team").value;
            var Agent_Mail = document.getElementById("txt_Agent_Mail").value;
            var Agent_Co_TEL = document.getElementById("txt_Agent_Co_TEL").value;
            var Agent_Phone_2 = document.getElementById("txt_Agent_Phone_2").value;
            //var Agent_Phone_3 = document.getElementById("txt_Agent_Phone_3").value;
            //var Agent_Code = document.getElementById("txt_code").value;
            //var Agent_MVPN = document.getElementById("txt_mvpn").value;
            var UserID = document.getElementById("txt_UserID").value;
            var Password = document.getElementById("txt_Password").value;
            var chk_pass = document.getElementById("chk_Password").value;
            var Agent_LV = document.getElementById("Agent_LV").value;
            var Role_ID = document.getElementById("Role_ID").value;
            if ($("#txt_hid_id").val() == '0') {
                Flag = '0';
            } else {
                Flag = '1';
            };

            if (Password != '') {
                if (Password != chk_pass) {
                    alert("【登入密碼】與【確認密碼】不一樣，請重新輸入。");
                    document.getElementById("btn_update").disabled = false;
                    document.getElementById("btn_new").disabled = false;
                    return;
                }
            };

            $.ajax({
                url: '0060010004.aspx/New_Agent',
                type: 'POST',
                data: JSON.stringify({
                    Agent_ID: Agent_ID,
                    Agent_Name: Agent_Name,
                    Agent_Company: Agent_Company,
                    Agent_Team: Agent_Team,
                    Agent_Mail: Agent_Mail,
                    Agent_Co_TEL: Agent_Co_TEL,
                    Agent_Phone_2: Agent_Phone_2,
                    //Agent_Phone_3: Agent_Phone_3,
                    //Agent_Code: Agent_Code,
                    //Agent_MVPN: Agent_MVPN,
                    UserID: UserID,
                    Password: Password,
                    Role_ID: Role_ID,
                    Agent_LV: Agent_LV,
                    Flag: Flag
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    if (json.status == "new") {
                        alert("新增完成！")
                        bindTable();
                        //Agent_Company_List();
                        //Agent_Team_List();
                    }
                    else if (json.status == "update") {
                        alert("修改完成！");
                        bindTable();
                        //Agent_Company_List();
                        //Agent_Team_List();

                    }
                    else {
                        alert(json.status);
                    }
                    document.getElementById("btn_update").disabled = false;
                    document.getElementById("btn_new").disabled = false;
                },
                error: function () {
                    document.getElementById("btn_update").disabled = false;
                    document.getElementById("btn_new").disabled = false;
                }
            });

        }

    </script>
    <style>
        body {
            font-family: "Microsoft JhengHei",Helvetica,Arial,Verdana,sans-serif;
            font-size: 18px;
        }

        thead th {
            background-color: #666666;
            color: white;
        }

        tr td:first-child,
        tr th:first-child {
            border-top-left-radius: 8px;
            border-bottom-left-radius: 8px;
        }

        tr td:last-child,
        tr th:last-child {
            border-top-right-radius: 8px;
            border-bottom-right-radius: 8px;
        }

        #data_company td:nth-child(2), #data_company td:nth-child(1),
        #data2 td:nth-child(3), #data2 td:nth-child(1),
        #data td:nth-child(6), #data td:nth-child(5), #data td:nth-child(4),
        #data td:nth-child(3), #data td:nth-child(2), #data td:nth-child(1) {
            text-align: center;
        }
    </style>
    <!-- ====== Modal ====== -->
    <div class="modal fade" id="myModal" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="width: 1100px;">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <input id="txt_hid_id" name="txt_hid_id" type="hidden" />
                    <h2 class="modal-title"><strong>
                        <label id="txt_title">員工資料（新增）</label></strong></h2>
                </div>
                <div class="modal-body">

                    <!-- ========================================== -->
                    <table id="data2" class="table table-bordered table-striped" style="width: 99%">
                        <thead>
                            <tr>
                                <th style="text-align: center" colspan="4">
                                    <span style="font-size: 20px"><strong>員工資料</strong></span>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td style="width: 20%; color: #D50000;">員工編號</td>
                                <td style="width: 30%">
                                    <div>
                                        <div id="div_Agent_ID" data-toggle="tooltip" title="員工編號不允許修改">
                                            <label id="lab_Agent_ID"></label>
                                        </div>
                                        <input id="txt_Agent_ID" class="form-control" placeholder="員工編號" maxlength="10" onkeyup="value=value.replace(/[^\d]/g,'') "
                                            style="width: 100%; Font-Size: 18px; background-color: #ffffbb" data-toggle="tooltip" title="必填，不能超過１０個字元，並且只能填數字" />
                                    </div>
                                </td>
                                <td style="width: 20%; color: #D50000;">員工姓名</td>
                                <td style="width: 30%">
                                    <div>
                                        <input id="txt_Agent_Name" class="form-control" placeholder="員工姓名" maxlength="10"
                                            style="width: 100%; Font-Size: 18px; background-color: #ffffbb" data-toggle="tooltip" title="必填，不能超過１０個字元" />
                                    </div>
                                </td>
                            </tr>
                            <!-- ==========================================
                            <tr>
                                <td style="width: 20%">選擇所屬公司</td>
                                <td style="width: 30%">
                                    <div>
                                        <select id="select_Agent_Company" name="select_Agent_Company" class="chosen-select" style="width: 100%" onchange="Agent_Company()">
                                            <option value="">請選擇所屬公司…</option>
                                        </select>
                                    </div>
                                </td>
                                <td style="width: 20%">選擇所屬部門</td>
                                <td style="width: 30%">
                                    <div>
                                        <select id="select_Agent_Team" name="select_Agent_Team" class="chosen-select" style="width: 100%" onchange="Agent_Team()">
                                            <option value="">請選擇所屬部門…</option>
                                        </select>
                                    </div>
                                </td>
                            </tr>-->
                            <!-- ========================================== --> 
                            <tr>
                                <td style="width: 20%; color: #D50000;">所屬部門</td>
                                <td style="width: 30%">
                                    <div>
                                        <input id="txt_Agent_Company" class="form-control" placeholder="所屬部門" maxlength="20"
                                            style="width: 100%; Font-Size: 18px; background-color: #ffffbb" data-toggle="tooltip" title="必填，不能超過２０個字元" />
                                    </div>
                                </td>
                                <td style="width: 20%; color: #D50000;">員工類別</td>
                                <td style="width: 30%">
                                    <div>
                                        <input id="txt_Agent_Team" class="form-control" placeholder="ex:客服,業務,工程師" maxlength="20"
                                            style="width: 100%; Font-Size: 18px; background-color: #ffffbb" data-toggle="tooltip" title="必填，不能超過２０個字元" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 20%; color: #D50000;">電子信箱</td>
                                <td style="width: 30%">
                                    <div data-toggle="tooltip" title="不能超過５０個字元">
                                        <input id="txt_Agent_Mail" class="form-control" placeholder="電子信箱" maxlength="50"
                                            style="width: 100%; Font-Size: 18px; " />
                                    </div>
                                </td>
                                <td style="width: 20%">手機號碼</td>
                                <td style="width: 30%">
                                    <div data-toggle="tooltip" title="不能超過１2個字元">
                                        <input id="txt_Agent_Phone_2" class="form-control" placeholder="手機號碼" style="width: 100%; 
                                            Font-Size: 18px" maxlength="12"/>
                                        <!--    onkeyup="value=value.replace(/[^\d]/g,'') "       -->
                                    </div>
                                </td>                                
                            </tr>
  <!--                          <tr>
                                <td style="width: 20%">電話號碼</td>
                                <td style="width: 30%">
                                    <div data-toggle="tooltip" title="不能超過２０個字元，格式為【12-12345678#123】">
                                        <input id="txt_Agent_Co_TEL" class="form-control" placeholder="12-12345678#123"
                                            maxlength="20" style="width: 100%; Font-Size: 18px" />
                                    </div>
                                </td>
                                <td style="width: 20%">手機簡碼</td>
                                <td style="width: 30%">
                                    <div data-toggle="tooltip" title="不能超過１０個字元，並且只能填數字">
                                        <input id="txt_Agent_Phone_3" class="form-control" placeholder="手機簡碼" style="width: 100%; Font-Size: 18px" maxlength="10" onkeyup="value=value.replace(/[^\d]/g,'') " />
                                    </div>
                                </td>
                            </tr>       -->
   <!--                         <tr>
                                <td style="width: 20%">院區簡碼</td>
                                <td style="width: 30%">
                                    <div data-toggle="tooltip" title="不能超過６個字元，並且只能填數字">
                                        <input id="txt_code" class="form-control" placeholder="院區簡碼" style="width: 100%; Font-Size: 18px" maxlength="6" onkeyup="value=value.replace(/[^\d]/g,'') " />
                                    </div>
                                </td>
                                <td style="width: 20%">ＭＶＰＮ</td>
                                <td style="width: 30%">
                                    <div data-toggle="tooltip" title="不能超過８個字元，並且只能填數字">
                                        <input id="txt_mvpn" class="form-control" placeholder="ＭＶＰＮ" style="width: 100%; Font-Size: 18px" maxlength="8" onkeyup="value=value.replace(/[^\d]/g,'') " />
                                    </div>
                                </td>
                            </tr>   -->
                            <tr>
                                <td style="width: 20%; color: #D50000;">登入帳號</td>
                                <td style="width: 30%">
                                    <div data-toggle="tooltip" title="必填，不能超過１０個字元，並且只能填英文或數字">
                                        <input id="txt_UserID" class="form-control" placeholder="登入帳號" maxlength="10"
                                            style="width: 100%; Font-Size: 18px; background-color: #ffffbb" />
                                    </div>
                                </td>
                                <td style="width: 20%;">電話號碼</td>
                                <td style="width: 30%">
                                <div data-toggle="tooltip" title="不能超過２０個字元">
                                        <input id="txt_Agent_Co_TEL" class="form-control" placeholder=""
                                            maxlength="20" style="width: 100%; Font-Size: 18px" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 20%; color: #D50000;">登入密碼</td>
                                <td style="width: 30%">
                                    <div>
                                        <input id="txt_Password" class="form-control" placeholder="登入密碼" type="password"
                                            style="width: 100%; Font-Size: 18px; background-color: #ffffbb" maxlength="20" onkeyup="value=value.replace(/[\W]/g,'')"
                                            data-toggle="tooltip" title="必填，不能超過２０個字元，並且只能填英文或數字" />
                                    </div>
                                </td>
                                <td style="width: 20%; color: #D50000;">確認密碼</td>
                                <td style="width: 30%">
                                    <div>
                                        <input id="chk_Password" class="form-control" placeholder="請跟登入密碼一樣" type="password"
                                            style="width: 100%; Font-Size: 18px; background-color: #ffffbb" maxlength="20" onkeyup="value=value.replace(/[\W]/g,'')"
                                            data-toggle="tooltip" title="請跟登入密碼一樣" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 20%">操作權限</td>
                                <td style="width: 30%">
                                    <div data-toggle="tooltip" title="預設為【一般員工】">
                                        <select id="Agent_LV" style="width: 100%; Font-Size: 18px">
                                            <option value="10">一般員工</option>
                                            <option value="20">部門主管</option>
                                            <option value="30">管理員工</option>
                                        </select>
                                    </div>
                                </td>
                                <td style="width: 20%">選單瀏覽權限</td>
                                <td style="width: 30%">
                                    <div data-toggle="tooltip" title="預設為【一般員工】">
                                        <select id="Role_ID" style="width: 100%; Font-Size: 18px">
                                        </select>
                                    </div>
                                </td>
                            </tr>
                            <!-- ========================================== -->
                            <tr>
                                <th style="text-align: center; width: 50%; height: 55px;" colspan="4">
                                    <button id="btn_new" type="button" class="btn btn-success btn-lg" onclick="New_Agent()"><span class="glyphicon glyphicon-ok"></span>&nbsp;&nbsp;新增</button>
                                    <button id="btn_update" type="button" class="btn btn-primary btn-lg" onclick="New_Agent()"><span class="glyphicon glyphicon-pencil"></span>&nbsp;&nbsp;修改</button>
                                </th>
                            </tr>
                        </tbody>
                    </table>
                    <!-- ========================================== -->

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                </div>
            </div>
            <!-- =========== Modal content =========== -->
        </div>
    </div>
    <!--===================================================-->

    <!-- ====== Modal ====== -->
    <div class="modal fade" id="Div1" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="width: 600px;">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>搜索條件設定</label>
                    </strong></h2>
                    <h5 style="color: red">（最多只能勾選五筆）
                        <button type="button" class="btn btn-info btn-lg" style="Font-Size: 20px; float: right" onclick="bindTable()"><span class='glyphicon glyphicon-search'></span>&nbsp;&nbsp;開始搜索</button>
                    </h5>
                </div>
                <div class="modal-body">
                    <table class="table table-bordered table-striped" style="width: 99%">
                        <tbody>
                            <tr>
                                <td>
                                    <table id="data_company" class="display table table-striped" style="width: 99%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">所屬公司</th>
                                                <th style="text-align: center;">選擇</th>
                                            </tr>
                                        </thead>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <!-- ========================================== -->
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                </div>
            </div>
            <!-- =========== Modal content =========== -->
        </div>
    </div>
    <!--===================================================-->

    <div class="table-responsive" style="width: 95%; margin: 10px 20px">
        <h2><strong>員工資料管理（瀏覽）&nbsp; &nbsp;
            <button type="button" class="btn btn-success btn-lg" data-toggle="modal" data-target="#myModal" style="Font-Size: 20px;" onclick="Xin_De()"><span class='glyphicon glyphicon-plus'></span>&nbsp;&nbsp;新增員工</button>
            <button type="button" class="btn btn-warning btn-lg" data-toggle="modal" data-target="#Div1" style="Font-Size: 20px; float: right;"><span class='glyphicon glyphicon-cog'></span>&nbsp;&nbsp;搜索條件設定</button>
        </strong></h2>
        <table id="data" class="display table table-striped" style="width: 99%">
            <thead>
                <tr>
                    <th style="text-align: center;">員工編號</th>
                    <th style="text-align: center;">員工姓名</th>
                    <th style="text-align: center;">所屬部門</th>
                    <th style="text-align: center;">員工類別</th>
                    <th style="text-align: center;">修改</th>
                    <th style="text-align: center;">刪除</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
