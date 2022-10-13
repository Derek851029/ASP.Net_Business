<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="test5.aspx.cs" Inherits="test5" %>

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
            //document.getElementById().addEventListener('click', function () {
            //    one;
            //    two;

            //});
        });

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
                                data: "APP_EMAIL",
                                render: function (data, type, row, meta) {
                                    return "<div class='checkbox'>" +
                                        "<label>" +
                                        "<input id='chack' type='checkbox' name='chack' style='width: 30px; height: 30px; value='" + data+"' /> "
                                        +"</label>"
                                        + "</div>";
                                }

                            }]
                    });
                    //==========================================================
                    var mail_address = [];
                    $('#data_company tbody').
                        unbind('click').
                        on('click', '#chack', function () {
                            var table = $('#data_company').DataTable();
                            var cno = table.row($(this).parents('tr')).data().APP_EMAIL;

                            if ($(this).prop("checked")) {
                                //alert('test');
                                mail_address.push(cno);
                            }
                            else {
                                for (var i = 0; i < mail_address.length; i++) {
                                    //alert('目前ARRAY裡面有' + mail_address.length + '個');
                                    if (mail_address[i] == cno) {
                                        mail_address.splice(i, 1);
                                    }
                                }
                            }
                            document.getElementById('send_address').value = "";
                            document.getElementById('send_address').value = mail_address;
                        });
                    //$(document).ready(function () {
                    //    $("#CheckAll").click(function () {
                    //        if ($("#CheckAll").prop("checked")) {//如果全選按鈕有被選擇的話（被選擇是true）
                    //            $("input[name='Chack']").prop("checked", true);//把所有的核取方框的property都變成勾選
                    //        } else {
                    //            $("input[name='Chack']").prop("checked", false);//把所有的核取方框的property都取消勾選
                    //        }
                    //    })
                    //})
                    //==========================================================
                    
                }
            });
        }

        function Send_mail() {
            var form = document.getElementById("send_form").value;
            var address = document.getElementById("send_address").value;
            var title = document.getElementById("send_title").value;
            var body = document.getElementById("send_msg").value;
            if (form == '') {
                alert('請輸入寄件人');
            }
            if (address == '') {
                alert('請輸入公司信箱');
            }
            if (title == '') {
                alert('請輸入標題');
            }
            if (body == '') {
                alert('請輸入內容');
            }

            $.ajax({
                url: 'test5.aspx/Send_mail',
                type: 'POST',
                data: JSON.stringify({ form: form, address: address, title: title, body: body }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var json = JSON.parse(doc.d);
                    alert(json.status)
                }
            })
        }

        //function CheckBox_All() {
        //    $(document).ready(function () {
        //        $("#CheckAll").click(function () {
        //            if ($("#CheckAll").prop("checked")) {//如果全選按鈕有被選擇的話（被選擇是true）
        //                $("input[name='Chack']").prop("checked", true);//把所有的核取方框的property都變成勾選
        //            } else {
        //                $("input[name='Chack']").prop("checked", false);//把所有的核取方框的property都取消勾選
        //            }
        //        })
        //    })
        //}

        function List_Can_Message() {
            $.ajax({
                url: 'test5.aspx/List_Can_Message',
                type: 'POST',
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#Can_message').DataTable({
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
                        "aLengthMenu": [[5, 10, 20], [5, 10, 100]],
                        "iDisplayLength": 10,
                        "columnDefs": [{
                            "targets": -1,
                            "data": null,
                            "searchable": false,
                            "paging": false,
                            "ordering": false,
                            "info": false
                        }],
                        columns: [
                            { data: "EMAIL_TITLE" },
                            { data: "EMAIL_CONTENT" },
                            {
                                data: "EMAIL_TITLE",
                                render: function (data, type, row, meta) {
                                    return "<div class='checkbox'><label>" +
                                        "<input type='checkbox' style='width: 30px; height: 30px; right: 40px;' id='chack2' />" +
                                        "</label></div>";
                                }

                            }]
                    });
                    //==========================================================
                    var title = [];
                    var content = [];
                    $('#Can_message tbody').
                        unbind('click').
                        on('click', '#chack2', function () {
                            var table = $('#Can_message').DataTable();
                            var cno = table.row($(this).parents('tr')).data().EMAIL_TITLE;
                            var ctt = table.row($(this).parents('tr')).data().EMAIL_CONTENT;
                            if ($(this).prop("checked")) {
                                title.push(cno);
                                content.push(ctt);
                            }
                            else {
                                for (var i = 0; i < title.length; i++) {
                                    if (title[i] == cno) {
                                        title.splice(i, 1);
                                    }
                                    if (content[i] == ctt) {
                                        content.splice(i,1);
                                    }
                                }
                            }
                            document.getElementById('send_title').value = "";
                            document.getElementById('send_title').value = title;
                            document.getElementById('send_msg').value = "";
                            document.getElementById('send_msg').value = content;
                            //on('click', '#Delete_message', function () {
                            //    if ($(this).prop(checked)) {
                            //        DeleteSelectTable();
                            //    }
                            //})
                        })   

                }
            });
        }
        
        function Create_Can_Message() {
            var add_title = document.getElementById("Add_title").value;
            var add_content = document.getElementById("Add_content").value;
            $.ajax({
                url: 'test5.aspx/Create_Can_Message',
                type: 'POST',
                data: JSON.stringify({ add_title: add_title, add_content: add_content}),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d);
                    if (json.status == '請輸入標題' || json.status == '請輸入內容') {
                        alert(json.status)
                    } else {
                        window.location.reload();
                    }
                }
            });
        }

        function DeleteSelectTable() {
            var delete_title = document.getElementById("send_title").value;
            var delete_content = document.getElementById("send_msg").value;
                $.ajax({
                    url: 'test5.aspx/DeleteSelectTable',
                    type: 'POST',
                    data: JSON.stringify({ delete_title: delete_title, delete_content: delete_content }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                        var json = JSON.parse(doc.d);
                        if (json.status == '請選擇刪除項目') {
                            alert(json.status)
                        } else {
                            alert(json.status)
                            window.location.reload();
                        }
                    }
                });
        }                 
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
    <!-- ====== Modal Div1====== -->
    <div class="modal fade" id="Div1" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="width: 600px;">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>選擇公司</label>
                    </strong></h2>
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


       <!-- ====== Modal Div2====== -->
    <div class="modal fade" id="Div2" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="width: 600px;">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>編輯內容</label>
                    </strong></h2>
                </div>
                <div class="modal-body">
                    <table class="table table-bordered table-striped" style="width: 99%">
                        <tbody>
                            <tr>
                                <td>
                                    <table id="Can_message" class="display table table-striped" style="width: 99%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">標題</th>
                                                <th style="text-align: center;">內容</th>
                                                <th style="text-align: center;">選擇</th>
                                            </tr>
                                        </thead>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-success btn-lg" data-toggle="modal" data-target="#New_message" style="Font-Size: 20px; float: left;"><span class='glyphicon glyphicon-pencil'></span>&nbsp;&nbsp;新增</button>
                    <button type="button" class="btn btn-danger btn-lg" data-toggle="modal" data-target="#Delete_message" id="delete" onclick="DeleteSelectTable()" style="Font-Size: 20px; float: left;"><span class='glyphicon glyphicon-remove'></span>&nbsp;&nbsp;刪除</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>
      <!-- ====== Modal Edit_content ====== -->
    <div class="modal fade" id="New_message" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="width: 1100px;">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <input id="txt_hid_id" name="txt_hid_id" type="hidden" />
                    <h2 class="modal-title"><strong>
                        <label id="txt_title">內容（新增）</label><label id="txt_Agent_ID"></label></strong></h2>
                </div>
                <div class="modal-body">

                    <!-- ========================================== -->
                    <table id="data2" class="table table-bordered table-striped" style="width: 99%">
                        <thead>
                            <tr>
                                <th style="text-align: center" colspan="4">
                                    <span style="font-size: 20px"><strong>編輯訊息</strong></span>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td style="width: 20%; color: #D50000;">標題</td>
                                <td style="width: 30%">
                                    <div>
                                        <input id="Add_title" class="form-control" placeholder="標題" maxlength="50"
                                            style="width: 100%; Font-Size: 18px; background-color: #ffffbb" data-toggle="tooltip" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 20%; color: #D50000;">內容</td>
                                <td style="width: 30%">
                                    <div data-toggle="tooltip">
                                        <textarea id="Add_content" name="Reply" class="form-control" cols="45" rows="3" placeholder="內容"
                                            style="font-size: 18px; resize: none; background-color: #ffffbb; width:100%;"></textarea>
                                    </div>
                                </td>
                            </tr>

                            <!-- ========================================== -->
                            <tr>
                                <th style="text-align: center; width: 50%; height: 55px;" colspan="4">
                                    <button id="btn_new" type="button" class="btn btn-primary btn-lg" onclick="Create_Can_Message()"><span class="glyphicon glyphicon-pencil"></span>&nbsp;&nbsp;新增</button>
                                    <%--<button id="btn_update" type="button" class="btn btn-primary btn-lg" onclick="Add_content()"><span class="glyphicon glyphicon-pencil"></span>&nbsp;&nbsp;修改</button>--%>
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
                <!-- =========== Modal content =========== -->
    <div class="table-responsive" style="width: 95%; margin: 10px 20px">
        <br />
        <div id="test" ></div>
        <h2><strong>發佈新公告</strong></h2>
        <table class="display table table-bordered" style="width: 99%">
            <thead>
                <tr>
                    <th style="text-align: center;" colspan="2">發佈新公告</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <th style="text-align: center; width: 12%">選擇公司：</th>
                    <th style="text-align: left; width: 88%">
                        <button type="button" class="btn btn-info btn-lg" data-toggle="modal" data-target="#Div1" style="Font-Size: 20px; float: left;"><span class='glyphicon glyphicon-search'></span>&nbsp;&nbsp;選擇公司</button>
                    </th>
                </tr>
                <tr>
                    <th style="text-align: center;">寄件人：</th>
                    <th style="text-align: center;">
                        <input type="email" id="send_form" class="form-control" value="" style="Font-Size: 20px; width: 100%; background-color: #ffffbb" />
                    </th>
                </tr>
                <tr>
                     <th style="text-align: center;">公司信箱：</th>
                     <th style="text-align: center;">
                        <input type="text" id="send_address" class="form-control" value="" style="Font-Size: 20px; width: 100%; background-color: #ffffbb" /> 
                     </th>
                </tr>
                <tr>
                    <th style="text-align: center;">標題：</th>
                    <th style="text-align: center;">
                        <input type="text" id="send_title" name="send_title" class="form-control" placeholder="標題"
                            maxlength="250" style="Font-Size: 20px; width: 100%; background-color: #ffffbb" />
                    </th>
                </tr>
                <tr>
                    <th style="text-align: center;">內容：</th>
                    <th style="text-align: center;">
<%--                        <select style="width: 80%";>
                            <option>選擇內容...</option>
                        </select>--%>
                        <button type="button" class="btn btn-info btn-lg" data-toggle="modal" data-target="#Div2" style="Font-Size: 20px; float: left;" onclick='List_Can_Message()'><span class='glyphicon glyphicon-search'></span>&nbsp;&nbsp;選擇內容</button>
                        <br />
                        <br />
                        <textarea rows="2" cols="20" id="send_msg" name="send_msg" class="form-control" placeholder="公告內容"
                          style="Font-Size: 20px; width: 100%; height: 250px; background-color: #ffffbb; resize: none;" ></textarea>
                    </th>
                </tr>
                <tr>
                    <th style="text-align: center;">發送信件：</th>
                    <th style="text-align: center;">
                        <button type="button" class="btn btn-success btn-lg" style="Font-Size: 20px; float: left;" onclick="Send_mail()">
                            <span class='glyphicon glyphicon-volume-up'></span>&nbsp;&nbsp;發送信件</button>
                    </th>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>