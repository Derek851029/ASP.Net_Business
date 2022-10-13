<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="TEST.aspx.cs" Inherits="TEST" %>

<asp:Content ID="Content" ContentPlaceHolderID="head2" Runat="Server">
    <link href="../DataTables/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="../bootstrap-chosen-master/bootstrap-chosen.css" rel="stylesheet" />
    <script src="../DataTables/jquery.dataTables.min.js"></script>
    <script src="../chosen/chosen.jquery.js"></script>
    <script src="../js/jquery.validate.min.js"></script>
    <script type="text/javascript">
        var id;
        var array_mno = [];
        $(function () {
            List_Team();
            //bindTable();
        });

        //=============================================
        function bindTable() {
            $.ajax({
                url: 'test5.aspx/List_Message',
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
                            { data: "Create_Time" },
                            { data: "Tag_Team" },
                            { data: "Create_Team" },
                            { data: "Create_Name" },
                            { data: "Title" },
                            {
                                data: "", render: function (data, type, row, meta) {
                                    return "<button type='button' class='btn btn-info btn-lg' id='click' " +
                                        "data-toggle='modal' data-target='#myModal'>" +
                                        "<h4>" +
                                        "<span class='glyphicon glyphicon-share-alt'></span>" +
                                        "&nbsp; 回覆訊息</h4></button>";
                                }
                            },
                        ]
                    });
                    //=========================================================
                    $('#data tbody').unbind('click').
                        on('click', '#click', function () {
                            var table = $('#data').DataTable();
                            var title = table.row($(this).parents('tr')).data().Title;
                            var msg = table.row($(this).parents('tr')).data().Message;
                            id = table.row($(this).parents('tr')).data().SYSID;
                            Message_Click(title, msg, id);
                        });
                    //=========================================================
                }
            });
        }

        function Message_Click(title, msg, id) {
            document.getElementById("txt_title").innerHTML = title;
            document.getElementById("txt_msg").innerHTML = msg;
            List_Response(id);
        }

        function List_Response(id) {
            document.getElementById("txt_Response").value = "";
            $.ajax({
                url: 'test5.aspx/List_Response',
                type: 'POST',
                data: JSON.stringify({ ID: id }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#data_2').DataTable({
                        destroy: true,
                        data: eval(doc.d), "oLanguage": {
                            "sLengthMenu": "顯示 _MENU_ 筆記錄",
                            "sZeroRecords": "尚無回覆訊息",
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
                            { data: "Response_Time" },
                            { data: "Agent_Team" },
                            { data: "Agent_Name" },
                            { data: "Response" }
                        ]
                    });
                }
            });
        }

        //=============================================
        function List_Team() {
            $.ajax({
                url: 'test5.aspx/List_Team',
                type: 'POST',
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#data_company').DataTable({
                        searching: false,
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
                        "aLengthMenu": [[50, 100], [50, 100]],
                        "iDisplayLength": 50,
                        "columnDefs": [{
                            "targets": -1,
                            "data": null,
                            "searchable": false,
                            "paging": false,
                            "ordering": false,
                            "info": false
                        }],
                        columns: [
                            { data: "BUSINESSNAME" },
                            { data: "APP_EMAIL" },
                            {
                                data: "APP_EMAIL", render: function (data, type, row, meta) {
                                    return "<div class='checkbox'><label>" +
                                        "<input type='checkbox' style='width: 30px; height: 30px;' id='chack'/>" +
                                        "</label></div>";
                                }
                            }]
                    });
                    //==========================================================
                    $('#data_company tbody').
                        unbind('click').
                        on('click', '#chack', function () {
                            var table = $('#data_company').DataTable();
                            var cno = table.row($(this).parents('tr')).data().APP_EMAIL;
                            //document.getElementById("send_address").innerHTML = cno;
                            $('#test').append(cno);
                        });
                    //==========================================================
                }
            });
        }

        function New_Msg() {
            var Msg = document.getElementById("txt_Response").value;
            if (Msg.trim() != "") {
                $.ajax({
                    url: 'test5.aspx/New_Msg',
                    type: 'POST',
                    data: JSON.stringify({ Msg: Msg, ID: id }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",       //如果要回傳值，請設成 json
                    success: function (doc) {
                        var json = JSON.parse(doc.d.toString());
                        if (json.status == "0") {
                            alert(json.txt);
                            List_Response(id);
                        } else {
                            alert(json.txt);
                        }
                    }
                });
            }
        }

        function SendMag() {
            var title = document.getElementById("send_title").value;
            var msg = document.getElementById("send_msg").value;
            $.ajax({
                url: 'test5.aspx/SendMag',
                type: 'POST',
                data: JSON.stringify({ str_Array: array_mno, Title: title, Message: msg }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    bindTable();
                    alert(doc.d);
                }
            })
        };

    </script>
    <style>
        body {
            font-family: "Microsoft JhengHei",Helvetica,Arial,Verdana,sans-serif;
            font-size: 20px;
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

        #data_2 td:nth-child(4) {
            text-align: left;
        }

        #data_company td:nth-child(3), #data_company td:nth-child(2), #data_company td:nth-child(1),
        #data_2 td:nth-child(3), #data_2 td:nth-child(2), #data_2 td:nth-child(1), #data td:nth-child(6),
        #data td:nth-child(5), #data td:nth-child(4), #data td:nth-child(3), #data td:nth-child(2), #data td:nth-child(1) {
            text-align: center;
        }
    </style>
    <!-- ====== Modal ====== -->
    <div class="modal fade" id="myModal" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="width: 80%;">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h1 class="modal-title"><strong>
                        <label>回覆訊息</label>
                    </strong></h1>
                </div>
                <div class="modal-body">
                    <table id="Table_Message" class="display table table-bordered" style="width: 99%">
                        <thead>
                            <tr>
                                <th style="text-align: center;" colspan="2">公告內容</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <th style="text-align: center; width: 20%">公告標題：</th>
                                <th id="txt_title" style="text-align: left; width: 80%"></th>
                            </tr>
                            <tr>
                                <th style="text-align: center; width: 20%">公告內容：</th>
                                <th id="txt_msg" style="text-align: left; width: 80%"></th>
                            </tr>
                        </tbody>
                    </table>
                    <!-- ========================================== -->
                    <br />
                    <table id="data_2" class="display table table-striped" style="width: 99%">
                        <thead>
                            <tr>
                                <th style="text-align: center; width: 15%">回覆時間</th>
                                <th style="text-align: center; width: 15%">回覆部門</th>
                                <th style="text-align: center; width: 15%">回覆人員</th>
                                <th style="text-align: center; width: 55%">回覆訊息</th>
                            </tr>
                        </thead>
                    </table>
                    <br />
                    <table class="display table table-striped" style="width: 99%">
                        <thead>
                            <tr>
                                <th style="text-align: center">
                                    <span style="font-size: 20px"><strong>回覆訊息</strong></span>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <th style="text-align: center; width: 100%">
                                    <input type="text" id="txt_Response" name="txt_Response" class="form-control" placeholder="回覆訊息" onkeyup="cs(this);"
                                        maxlength="250" style="Font-Size: 20px; width: 100%; background-color: #ffffbb" />
                                </th>
                            </tr>
                            <!-- ========================================== -->
                            <tr>
                                <th style="text-align: center; width: 100%; height: 55px;" colspan="2">
                                    <button id="btn_new" type="button" class="btn btn-success btn-lg" onclick="New_Msg()">
                                        <h4><span class="glyphicon glyphicon-ok"></span>&nbsp;&nbsp;發送訊息</h4>
                                    </button>
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
            <!-- Modal content-->
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
                        <label>選擇公司</label>
                    </strong></h2>
                    <h5 style="color: red">（最多只能勾選五筆）</h5>
                </div>
                <div class="modal-body">
                    <table class="table table-bordered table-striped" style="width: 99%">
                        <tbody>
                            <tr>
                                <td>
                                    <table id="data_company" class="display table table-striped" style="width: 99%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">公司名稱</th>
                                                <th style="text-align: center;">信箱</th>
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
        <h2><strong>全體公告（瀏覽）</strong></h2>
        <table id="data" class="display table table-striped" style="width: 99%">
            <thead>
                <tr>
                    <th style="text-align: center; width: 15%">公告時間</th>
                    <th style="text-align: center; width: 10%">接收部門</th>
                    <th style="text-align: center; width: 10%">公告部門</th>
                    <th style="text-align: center; width: 10%">公告人員</th>
                    <th style="text-align: center; width: 40%">公告標題</th>
                    <th style="text-align: center; width: 15%">回覆訊息</th>
                </tr>
            </thead>
        </table>
        <br />
        <div id="test" ></div>
        <table class="display table table-bordered" style="width: 99%">
            <thead>
                <tr>
                    <th style="text-align: center;" colspan="2">發佈新公告</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <th style="text-align: center; width: 12%">受文者：</th>
                    <th style="text-align: left; width: 88%">
                        <button type="button" class="btn btn-info btn-lg" data-toggle="modal" data-target="#Div1" style="Font-Size: 20px; float: left;"><span class='glyphicon glyphicon-search'></span>&nbsp;&nbsp;選擇公司</button>
                    </th>
                </tr>
                <tr>
                     <th style="text-align: center;">信箱地址：</th>
                     <th style="text-align: center;">
                        <input type="email" id="send_address" class="form-control" onkeyup="cs(this);"
                            maxlength="250" style="Font-Size: 20px; width: 100%; background-color: #ffffbb" />
                     </th>
                </tr>
                <tr>
                    <th style="text-align: center;">標題：</th>
                    <th style="text-align: center;">
                        <input type="text" id="send_title" name="send_title" class="form-control" placeholder="標題" onkeyup="cs(this);"
                            maxlength="250" style="Font-Size: 20px; width: 100%; background-color: #ffffbb" />
                    </th>
                </tr>
                <tr>
                    <th style="text-align: center;">內容：</th>
                    <th style="text-align: center;">
                        <input type="text" id="send_msg" name="send_msg" class="form-control" placeholder="公告內容" onkeyup="cs(this);"
                            maxlength="250" style="Font-Size: 20px; width: 100%; background-color: #ffffbb" />
                    </th>
                </tr>
                <tr>
                    <th style="text-align: center;">發送信件：</th>
                    <th style="text-align: center;">
                        <button type="button" class="btn btn-success btn-lg" style="Font-Size: 20px; float: left;" onclick="SendMag()">
                            <span class='glyphicon glyphicon-volume-up'></span>&nbsp;&nbsp;發送公告</button>
                    </th>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>