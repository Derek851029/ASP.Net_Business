<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="0010010003.aspx.cs" Inherits="_0010010003" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head2" Runat="Server">
    <link href="../css/jquery.datetimepicker.min.css" rel="stylesheet" />
    <link href="../DataTables/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="../bootstrap-chosen-master/bootstrap-chosen.css" rel="stylesheet" />
    <script src="../js/jquery.datetimepicker.full.min.js"></script>
    <script src="../chosen/chosen.jquery.js"></script>
    <script src="../js/jquery.validate.min.js"></script>
    <script src="../DataTables/jquery.dataTables.min.js"></script>
    <!----------新增進度------------------------------------------進度行事曆-------------------------------->
    <link href="../fullcalendar-2.8.0/fullcalendar.css" rel="stylesheet" />
    <link href="../fullcalendar-2.8.0/fullcalendar.print.css" rel="stylesheet" media="print" />
    <link href="../fullcalendar-2.8.0/fullcalendar.min.css" rel="stylesheet" />
    <script src="../fullcalendar-2.8.0/lib/moment.min.js"></script>
    <script src="../fullcalendar-2.8.0/fullcalendar.min.js"></script>
    <script src="../fullcalendar-2.8.0/lang-all.js"></script>
     <!------------------------------------------------------------------------------------>
    
    <script type="text/javascript">
        var seqno = '<%= seqno %>';
        var new_mno = '<%= new_mno %>';
        var new_mno2 = '<%= mno3 %>';
        var Agent_Mail = '<%= Session["Agent_Mail"] %>';
        var str_time = '<%= str_time %>';
        //取得建立者
        var menu = document.getElementById('menu_number').innerHTML;
        var menu_permission = document.getElementById('menu_permission').innerHTML;
        var Owner = menu.slice(11); //擷取從前面數第11個字之後的字串
        var Permission = menu_permission.slice(11, -12);

        $(function () {
            Client_Code_Search();       //選客戶
            renderCalendar();

            //判斷下拉式選單
            $("#Text5").change(function () {
                if ($('#Text5').val() == '取消') { //取得Text5中的value
                    document.getElementById('cause').style.display = "";
                    document.getElementById('upload_hide').style.display = "none";
                } else if ($('#Text5').val() == '已完成') {
                    document.getElementById('upload_hide').style.display = ""
                    document.getElementById('cause').style.display = "none";
                }
                else {
                    document.getElementById('cause').style.display = "none";
                    document.getElementById('upload_hide').style.display = "none";
                }
            });
        });

        function fileUpload() {
            let PID = document.getElementById('PID_hide').value;
            let files = document.getElementById('fileBox').files;
            let fileData = new FormData();
            fileData.append(files[0].name, files[0]);
            fileData.append('Owner', Owner);
            fileData.append('PID', PID);
            $.ajax({
                url: 'upFileHandler.ashx',
                type: "post",
                data: fileData,
                contentType: false,  //網頁要送到Server的資料型態
                processData: false,  //提交的时候不会序列化 data
                async: false,
                dataType: "json",
                success: function (doc) {
                    alert(doc.status);
                    document.getElementById('fileBox').value = "";
                    ViewPDF();
                }
            });
        }

        function ViewPDF() {
            let PID = document.getElementById('PID_hide').value;
            $.ajax({
                url: '0010010003.aspx/ViewPDF',
                type: 'POST',
                data: JSON.stringify({
                    PID: PID
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#ViewPDF').DataTable({
                        "bPaginate": false,
                        "binfo": false,
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
                        "columnDefs": [{
                            "targets": -1,
                            "data": null,
                            "searchable": false,
                            "paging": false,
                            "ordering": false,
                            "info": false,
                        }],
                        columns: [
                            {
                                data: "FileName", render: function (data, type, row, meta) {
                                    return "<a href='PDF/"+Owner+"/"+data+"' target='_blank'>"+data+"</a>"
                                }
                            },
                            {
                                data: "SYSID", render: function (data, type, row, meta) {
                                    return '<button type="button" class="btn btn-danger" id="delete" onclick="DeleteFile('+data+')">刪除</button>'
                                }
                            },
                        ]
                    });
                    $('#ViewPDF tbody').unbind('click').
                        on('click', '#delete', function () {
                        });
                }
            });
        }

        function DeleteFile(SYSID) {
            $.ajax({
                url: '0010010003.aspx/DeleteFile',
                type: 'POST',
                data: JSON.stringify({
                    SYSID: SYSID,
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    alert(json.status);
                    ViewPDF();
                }
            });
        }

        function Client_Code_Search() {     //選客戶
            $.ajax({
                url: '0010010003.aspx/Client_Code_Search',
                type: 'POST',
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                data: JSON.stringify({
                    Owner: Owner
            }),
                success: function (doc) {
                    var json = JSON.parse(doc.d);
                    var $select_elem = $("#DropClientCode");
                    $select_elem.chosen("destroy")
                    $.each(json, function (idx, obj) {
                    $select_elem.append("<option value='" + obj.A + "'>【" + obj.B + "】" + obj.A + "</option>");
                    });
                    $select_elem.chosen(
                        {
                            width: "100%",
                            search_contains: true
                        });
                    $('.chosen-single').css({ 'background-color': '#ffffbb' });
                }
            });
        }

        function Data_Save() {
            var BusinessName = document.getElementById('DropClientCode').value;
            var RunTime = document.getElementById("datetimepicker01").value;
            var OpinionProblem = document.getElementById("OpinionProblem").value;
            var OpinionContent = document.getElementById("OpinionContent").value;
            var OpinionRemarks = document.getElementById("OpinionRemarks").value;
            var Work_Name = document.getElementById("Work_Name").value;
            var ScheduleStatus = document.getElementById("Schedule_Status").value;
            var flag = '';
            if (ScheduleStatus == '執行中') {
                 flag = '1';
            } else if (ScheduleStatus == '維護中') {
                 flag = '2';
            } else if (ScheduleStatus == '未完成') {
                flag = '3';
            } else if (ScheduleStatus == '已完成') {
                flag = '4';
            }
            $.ajax({
                url: '0010010003.aspx/Data_Save',
                type: 'POST',
                data: JSON.stringify({
                    Owner: Owner,
                    BusinessName: BusinessName,
                    RunTime: RunTime,
                    OpinionProblem: OpinionProblem,
                    OpinionContent: OpinionContent,
                    OpinionRemarks: OpinionRemarks,
                    Work_Name: Work_Name,
                    ScheduleStatus: ScheduleStatus,
                    flag: flag
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    alert(json.status);
                    if (json.status == "新增完成！") {
                        window.location.reload();
                    }
                },
                error: function () { alert("新增每日進度發生錯誤"); }
            });
        }

         //================行事曆 ================
        //產生行事曆
        function renderCalendar() {
            $('#Calendar').fullCalendar({
                header: {
                    left: 'prev,next today',
                    center: 'title',
                    right: '' //'month,agendaWeek,agendaDay'
                },
                editable: false,
                defaultDate: new Date(),
                lang: 'zh-tw',
                eventClick: function (calEvent, jsEvent, view) {
                    GetClassScheduleList(calEvent.PID);
                },
                eventAfterRender: function (calEvent, jsEvent) {
                    //  類型  1: 執行中 2: 維護中 3：未完成  4：已完成  5：取消 
                    if (calEvent.value == '1') {
                        jsEvent.css('background-color', '#00ff00'); //綠色
                    } else if (calEvent.value == '2') {
                        jsEvent.css('background-color', '#0000ff'); //藍色
                    } else if (calEvent.value == '3') {
                        jsEvent.css('background-color', '#ff8c00'); //橘色
                    } else if (calEvent.value == '4') {
                        jsEvent.css('background-color', '#696969'); //灰色
                    } else if (calEvent.value == '5') {
                        jsEvent.css('background-color', '#ff0000'); //紅色
                    }
                },
                events: function (start, end, str_time,callback) {
                    $.ajax({
                        url: '0010010003.aspx/GetClassGroup',
                        type: 'POST',
                        data: JSON.stringify({
                            Owner: Owner,
                            Permission: Permission,
                            start: start.format("YYYY/MM/DD"),
                            end: end.format("YYYY/MM/DD"),
                            time: str_time
                        }),
                        contentType: 'application/json; charset=UTF-8',
                        dataType: "json",       //如果要回傳值，請設成 json
                        success: function (doc) {
                            var events = [];
                            $(eval(doc.d)).each(function () {
                                events.push({
                                    PID: this.PID,
                                    title: this.title,
                                    start: this.start,
                                    end: this.end,
                                    value: this.value,
                                });
                            });
                            callback(events);
                        }
                    });
                }
            });
        }

        function GetClassScheduleList(PID) {            //案件列表程式
            $.ajax({
                url: '0010010003.aspx/GetClassScheduleList',
                type: 'POST',
                data: JSON.stringify({
                    PID: PID
                }),
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
                            "info": false,
                            "defaultContent": "<button type='button' id='edit' class='btn btn-info btn-lg'>" +
                                "<span class='glyphicon glyphicon-search'></span>&nbsp;明細</button>"
                        }],
                        columns: [
                            //{ data: "SetupTime" },
                            //{ data: "UploadTime" },
                            { data: "BusinessName" },
                            { data: "Work_Name" },
                            { data: "RunTime" },
                            { data: "ScheduleStatus" },
                            {
                                data: "Case_ID", render: function (data, type, row, meta) {
                                    return "<button type='button' id='edit' class='btn btn-primary btn-lg' data-toggle='modal' data-target='#EditDaily'>" +
                                        "<span class='glyphicon glyphicon-pencil'></span></button>";
                                }
                            }
                        ]
                    });
                    $('#data tbody').unbind('click').
                        on('click', '#edit', function () {
                            LoadDaily(PID);
                        });
                }
            });
        }

        function GetALLSchedule() {            //案件列表程式
            $.ajax({
                url: '0010010003.aspx/GetALLSchedule',
                type: 'POST',
                data: JSON.stringify({
                    Owner: Owner,
                    Permission: Permission
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#AllScedule').DataTable({
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
                            "info": false,
                        }],
                        columns: [
                            //{ data: "SetupTime" },
                            //{ data: "UploadTime" },
                            { data: "BusinessName" },
                            { data: "Work_Name" },
                            { data: "RunTime" },
                            { data: "ScheduleStatus" },
                            {
                                data: "PID", render: function (data, type, row, meta) {
                                    return "<button type='button' id='edit' onclick='LoadDaily("+data+")' class='btn btn-primary btn-lg' data-toggle='modal' data-target='#EditDaily'>" +
                                        "<span class='glyphicon glyphicon-pencil'></span></button>";
                                }
                            }
                        ]
                    });
                    $('#Div1 tbody').unbind('click').
                        on('click', '#edit', function () {
                        });
                }
            });
        }

        function LoadDaily(PID) {
            $.ajax({
                url: '0010010003.aspx/LoadDaily',
                type: 'POST',
                data: JSON.stringify({ PID: PID }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var text = '{"data":' + doc.d + '}';
                    var obj = JSON.parse(text);
                    document.getElementById('PID_hide').value = obj.data[0].PID;
                    document.getElementById('Name').disabled = true;
                    document.getElementById('Day').disabled = true;
                    document.getElementById('Text4').disabled = true;
                    document.getElementById("Name").value = obj.data[0].BusinessName;
                    document.getElementById("Day").value = obj.data[0].RunTime.split('T'); //不知道為甚麼多一個T, 把T刪除
                    document.getElementById("Text1").value = obj.data[0].OpinionProblem;
                    document.getElementById("Text2").value = obj.data[0].OpinionContent;
                    document.getElementById("Text3").value = obj.data[0].OpinionRemarks;
                    document.getElementById("Text4").value = obj.data[0].Work_Name;
                    document.getElementById("Text5").value = obj.data[0].ScheduleStatus;
                    document.getElementById("Text7").value = obj.data[0].CancelCause;

                    if (obj.data[0].ScheduleStatus == '取消') {
                        document.getElementById('cause').style.display = "";
                    } else if (obj.data[0].ScheduleStatus == '已完成') {
                        document.getElementById('upload_hide').style.display = "";
                        document.getElementById('Text5').disabled = true;
                        ViewPDF();
                    } 
                }
            });
            //$("#Div_Loading").modal('hide');        // 功能??
        }

        function UpdateDaily() {
            var flag = "";
            var PID = document.getElementById('PID_hide').value;
            var BusinessName = document.getElementById('Name').value;
            var OpinionProblem = document.getElementById('Text1').value;
            var OpinionContent = document.getElementById('Text2').value;
            var OpinionRemarks = document.getElementById('Text3').value;
            var ScheduleStatus = document.getElementById('Text5').value;
            var CancelCause = document.getElementById('Text7').value;
            if (ScheduleStatus == '執行中') {
                flag = "1";
            } else if (ScheduleStatus == '維護中') {
                flag = "2";
            } else if (ScheduleStatus == '未完成') {
                flag = "3";
            } else if (ScheduleStatus == '已完成') {
                flag = "4";
            } else if (ScheduleStatus == '取消') {
                flag = "5"
            }
            $.ajax({
                url: '0010010003.aspx/UpdateDaily',
                type: 'POST',
                data: JSON.stringify({
                    PID: PID,
                    BusinessName: BusinessName,
                    OpinionProblem: OpinionProblem,
                    OpinionContent: OpinionContent,
                    OpinionRemarks: OpinionRemarks,
                    ScheduleStatus: ScheduleStatus,
                    CancelCause: CancelCause,
                    flag: flag
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                     alert(json.status);
                    window.location.reload();
                }
            });
        }


        //================【下拉選單】 CSS 修改 ================
        function style(Name, value) {
            var $select_elem = $("#" + Name);
            $select_elem.chosen("destroy")
            document.getElementById(Name).value = value;
            $select_elem.chosen({
                width: "100%",
                height: "23px",
                //search_contains: true
            });
            $('.chosen-single').css({ 'background-color': '#ffffbb' });
        }

        //================================================
    </script>
    <style type="text/css">
        body {
            font-family: "Microsoft JhengHei",Helvetica,Arial,Verdana,sans-serif;
            font-size: 16px;
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

        #Location_Table td:nth-child(6), #Location_Table td:nth-child(5), #Location_Table td:nth-child(4),
        #Location_Table td:nth-child(3), #Location_Table td:nth-child(2), #Location_Table td:nth-child(1),
        #data td:nth-child(10), #data td:nth-child(9), #data td:nth-child(8), #data td:nth-child(7), #data td:nth-child(6), #data td:nth-child(5),
        #data td:nth-child(4), #data td:nth-child(3), #data td:nth-child(2), #data td:nth-child(1), #data th:nth-child(5) {
            text-align: center;
        }
        strong span{
            color:red;
        }

        .auto-style1 {
            height: 47px;
        }
    </style>

     <div style="width:100%; display:flex;">
        <button type="button" id="AddWork" class="btn btn-success btn-lg" data-toggle="modal" data-target="#newWork" style="Font-Size: 18px;" >
        <span class='glyphicon glyphicon-plus'></span>&nbsp;&nbsp;新增工作進度</button>
         <button type="button" id="Search" onclick="GetALLSchedule()" class="btn btn-info btn-lg" data-toggle="modal" data-target="#Div1" style="Font-Size: 18px;margin-left:5px;" >
        <span class='glyphicon glyphicon-search'></span>&nbsp;&nbsp;搜尋所有進度</button>
    </div>

     <%--====================搜尋所有進度==============--%>
        <div class="modal fade" style="width:100%" id="Div1" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>所有進度</label>
                    </strong></h2>
                </div>
                <div class="modal-body">
                    <table id="AllScedule" class="table table-bordered table-striped" style="width: 99%">
                        <tbody>
                            <tr>
                                <td>
                                    <table id="NewList" class="display table table-striped" style="width: 99%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">客戶名稱</th>
                                                <th style="text-align: center;">工作人員</th>
                                                <th style="text-align: center;">預計時程</th>
                                                <th style="text-align: center;">進度狀態</th>
                                                <th style="text-align: center;">編輯</th>
                                            </tr>
                                        </thead>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                    <!-- ========================================== -->
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                </div>
            </div>
            <!-- =========== Modal content =========== -->
        </div>
    </div>

        <div class="modal fade" style="width:auto; margin: auto" id="newWork" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label id="title_modal" ></label>
                    </strong></h2>
                </div>
                <div class="modal-body">
                    <!-- ========================================== 表格 -->
                                <div style="background-color:#666666; color:white">
                                     <span style="font-size: 25px"><strong>新增工作進度</strong></span>
                                </div>

                    <br />

                                    <strong>選擇客戶<span>*</span></strong>
                                <div>
                                    <div data-toggle="tooltip" title="必選" style="width: 100%">
                                         <select id="DropClientCode" name="DropClientCode" class="chosen-select" style="background-color:#ffffbb" >
                                               <option value="">請選擇客戶…</option>
                                         </select>
                            <input id="str_Client_Name" name="str_Client_Name" type="hidden" />
                        </div>
                                </div>
                    <div>
                        <strong>預計時程<span>*</span></strong>
                    </div>
                                <div style="float: left" data-toggle="tooltip" title="">
                                   <input type="text" class="form-control" id="datetimepicker01" name="datetimepicker01" />
                                </div>
                    <br /><br />
                                <div>
                                    <strong>問題處理<span>*</span></strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="必填，應填8位數字">
                                             <textarea id="OpinionProblem" name="OpinionProblem" class="form-control" cols="55" rows="3" placeholder="問題處理描述" maxlength="1000" onkeyup=""
                                                 style="resize: none; background-color: #ffffbb"></textarea>
                                    </div>
                                </div>
                                <div>
                                    <strong>內容<span>*</span></strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="不能超過２００個字">
                                          <textarea id="OpinionContent" name="OpinionContent" class="form-control" cols="55" rows="3" placeholder="內容描述" maxlength="1000" onkeyup=""
                                          style="resize: none; background-color: #ffffbb"></textarea>
                                    </div>
                                </div>
                                <div>
                                    <strong>備註</strong>
                                </div>
                                <div>
                                    <textarea id="OpinionRemarks" name="OpinionRemarks" class="form-control" cols="55" rows="3" placeholder="備註描述" maxlength="1000" onkeyup=""
                                    style="resize: none; background-color: #ffffbb"></textarea>
                                </div>
                                <div>
                                    <strong>工作人員<span>*</span></strong>
                                </div>
                                <div>
                                    <div>
                                        <input type="text" id="Work_Name" class="form-control" placeholder="工作人員"/>
                                    </div>
                                </div>
                                <div>
                                    <strong>進度狀態<span>*</span></strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" style="width:auto; height:auto" >
                                          <select id="Schedule_Status" name="Schedule_Status" class="chosen-single" style="width:100%">
                                          <option value="">請選擇進度狀態…</option>
                                          <option value="執行中">執行中</option>
                                          <option value="維護中">維護中</option>
                                          <option value="未完成">未完成</option>
                                         <option value="已完成">已完成</option>
                                         </select>
                                    </div>
                                </div>
                        </div>
                <div class="modal-footer">
                    <button id="btn_new" type="button" class="btn btn-success btn-lg" onclick="Data_Save()" data-dismiss="modal"><span class="glyphicon glyphicon-ok"></span>&nbsp;&nbsp;新增</button>
                    <button type="button" class="btn btn-danger btn-lg" data-dismiss="modal"><span class="glyphicon glyphicon-remove"></span> &nbsp;取消</button>
                </div>
            </div>
            </div>
      </div>

            <%-----------------------------------------------修改工作進度----------------------------------------------------%>

            <div class="modal fade" style="width:auto; margin: auto" id="EditDaily" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                    </strong></h2>
                </div>
                <div class="modal-body">
                    <!-- ========================================== 表格 -->
                                <div style="background-color:#666666; color:white">
                                     <span style="font-size: 25px"><strong>修改工作進度</strong></span>
                                </div>

                    <br /><input type="hidden" id="PID_hide"/>

                                    <strong>客戶名稱</strong>
                                <div>
                                    <input type="text"  id="Name"  style= "background-color: #ffffbb; font-size: 18px;" />
                            </div>

                    <div>
                        <strong>預計時程<span>*</span></strong>
                    </div>
                                <div>
                                    <input type="text"  id="Day" />
                                </div>
                                <div>
                                    <strong>問題處理</strong>
                                </div>
                                <div>
                                    <textarea id="Text1" name="OpinionProblem" class="form-control" cols="55" rows="3" placeholder="問題處理描述" maxlength="1000" onkeyup=""
                                   style="resize: none; background-color: #ffffbb"></textarea>
                                </div>
                                <div>
                                    <strong>內容</strong>
                                </div>
                                <div>
                                        <textarea id="Text2" name="OpinionContent" class="form-control" cols="55" rows="3" placeholder="內容描述" maxlength="1000" onkeyup=""
                                        style="resize: none; background-color: #ffffbb"></textarea>
                                </div>
                                <div>
                                    <strong>備註</strong>
                                </div>
                                <div>
                                        <textarea id="Text3" name="OpinionRemarks" class="form-control" cols="55" rows="3" placeholder="備註描述" maxlength="1000" onkeyup=""
                                        style="resize: none; background-color: #ffffbb"></textarea>
                                </div>
                                <div>
                                    <strong>工作人員<span>*</span></strong>
                                </div>
                                <div>
                                          <input type="text" id="Text4" /> 
                                </div>
                                <div>
                                    <strong>進度狀態<span>*</span></strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip">
                                            <select id="Text5" name="Schedule_Status" style="background-color: #ffffbb; width: 100%;">
                                                  <option value="">請選擇進度狀態…</option>
                                                  <option value="執行中">執行中</option>
                                                  <option value="維護中">維護中</option>
                                                  <option value="未完成">未完成</option>
                                                 <option value="已完成">已完成</option>
                                                 <option value="取消">取消</option>
                                            </select>
                                    </div>
                                </div>
                    <div id="cause" style="display:none">
                             <div>
                                    <strong>取消原因</strong>
                             </div>
                             <div>
                                    <textarea id="Text7" name="OpinionProblem" class="form-control" cols="55" rows="3" placeholder="取消原因" maxlength="1000" onkeyup=""
                                   style="resize: none; background-color: #ffffbb"></textarea>
                            </div>
                    </div>
                    <br />
                    <div id="upload_hide" style="display:none">
                             <div>
                                    <strong>上傳合約</strong>
                             </div>
                             <div style="width:100%; display:flex">
                                    <input  id="fileBox" type="file" accept=".pdf, .PDF, image/*, .heic" />
                                    <button type="button" id="upload" onclick="fileUpload()" class="btn btn-success btn-sm"  style="Font-Size: 16px; position:relative; right:60px;" >
                                    <span class='glyphicon glyphicon-circle-arrow-up'></span>&nbsp;&nbsp;上傳</button>
                            </div>
                        <table id="ViewPDF" style="width:100%"></table>
                    </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary btn-lg" onclick="UpdateDaily()" data-dismiss="modal"><span class="glyphicon glyphicon-pencil"></span>&nbsp;&nbsp;修改</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                </div>
            </div>
         </div>
        </div>
      </div>

    <%--<div style="width: 1280px; margin: 10px 20px">
        <h2><label id="str_title"></label>
        </h2>        
        <table class="table table-bordered table-striped">
            <thead>
                <tr>
                    <th style="text-align: center" colspan="4">
                        <span style="font-size: 20px"><strong>新增工作進度</strong></span>
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr style="height: 55px;">
                    <th style="text-align: center; width: 15%">
                        <strong>客戶代碼</strong>
                        <br />
                        <br />
                        <label id="PID_Client"></label>
                    </th>
                    <th style="width: 35%">
                        <div data-toggle="tooltip" title="必選" style="width: 100%">
                            <select id="DropClientCode" name="DropClientCode" class="chosen-select"  >
                                <option value="">請選擇客戶…</option>
                            </select>
                            <input id="str_Client_Name" name="str_Client_Name" type="hidden" />
                        </div>
                        <br />
                        <div id="Label1"></div> 
                    </th>
                    <td style="text-align: center">
                        <strong>預計時程</strong>
                    </td>
                    <td style="width: 35%">
                        <div style="float: left" data-toggle="tooltip" title="">
                            <input type="text" class="form-control" id="datetimepicker01" name="datetimepicker01" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center">
                        <strong>問題處理</strong>
                    </td>
                    <td>
                        <textarea id="OpinionProblem" name="OpinionProblem" class="form-control" cols="55" rows="3" placeholder="問題處理描述" maxlength="1000" onkeyup=""
                        style="resize: none; background-color: #ffffbb"></textarea>
                    </td>
                    <td style="text-align: center">
                        <strong>內容</strong>
                    </td>
                    <td>
                        <textarea id="OpinionContent" name="OpinionContent" class="form-control" cols="55" rows="3" placeholder="內容描述" maxlength="1000" onkeyup=""
                        style="resize: none; background-color: #ffffbb"></textarea>
                    </td>
                </tr>
                <tr id="tr1" runat="server">
                    
                    <td style="text-align: center">
                        <strong>備註</strong>
                    </td>
                    <td>
                        <textarea id="OpinionRemarks" name="OpinionRemarks" class="form-control" cols="55" rows="3" placeholder="備註描述" maxlength="1000" onkeyup=""
                        style="resize: none; background-color: #ffffbb"></textarea>
                    </td>
                    <td style="text-align: center; ">
                        <strong>工作人員</strong>
                    </td>
                    <td>
                        <label id="Work_Name"></label> 
                    </td>
                </tr>
                <tr id="tr2" runat="server">
                    <td style="text-align: center">
                        <strong>進度狀態</strong>
                    </td>
                    <td>
                        <select id="Schedule_Status" name="Schedule_Status" class="chosen-select" onchange="">
                            <option value="">請選擇進度狀態…</option>
                            <option value="已完成">已完成</option>
                            <option value="未完成">未完成</option>
                        </select>
                    </td>
                </tr>
            </tbody>
        </table>   
            <div style="text-align: center">
                <button id="Button1" type="button" onclick="Data_Save();" class="btn btn-success btn-lg "><span class="glyphicon glyphicon-ok"></span>新增&nbsp;&nbsp;</button>&nbsp;&nbsp
                <button id="Button3" type="button" onclick="Btn_Back_Click();" class="btn btn-default btn-lg ">&nbsp;&nbsp;取消<span class="glyphicon glyphicon-share-alt"></span></button>
            </div>--%>
        <%-----------------------------------------------修改工作進度----------------------------------------------------%>
           <%--<div class="modal fade" id="EditDaily" role="dialog" data-backdrop="static" data-keyboard="false">
               f
              <div class="modal-dialog" style="width: 1200px;">
            <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                     <button type="button" class="close" data-dismiss="modal">&times;</button>
                       <h2 class="modal-title"><strong>
                            <label id="group_title"></label>
                    </strong></h2>
                    </div>
                <!-- =======新增關鍵字 表格======= -->
                <div class="modal-body">
                    <table class="display table table-striped">
                        <thead>
                            <tr>
                                <th style="text-align: center" colspan="4">
                                    <span style="font-size: 20px"><strong>修改每日進度<label id="title"></label></strong></span>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr style="height: 55px;">
                    <th style="text-align: center; width: 15%">
                        <strong>客戶代碼</strong>
                    </th>
                    <th style="width: 35%">
                        <input type="text"  id="Name"  style= "background-color: #ffffbb; font-size: 18px;" />
                    </th>
                    <td style="text-align: center">
                        <strong>預計時程</strong>
                    </td>
                    <td style="width: 35%">
                        <input type="text"  id="Day" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center">
                        <strong>問題處理</strong>
                    </td>
                    <td>
                        <textarea id="Text1" name="OpinionProblem" class="form-control" cols="55" rows="3" placeholder="問題處理描述" maxlength="1000" onkeyup=""
                        style="resize: none; background-color: #ffffbb"></textarea>
                    </td>
                    <td style="text-align: center">
                        <strong>內容</strong>
                    </td>
                    <td>
                        <textarea id="Text2" name="OpinionContent" class="form-control" cols="55" rows="3" placeholder="內容描述" maxlength="1000" onkeyup=""
                        style="resize: none; background-color: #ffffbb"></textarea>
                    </td>
                </tr>
                <tr id="tr3" runat="server">
                    
                    <td style="text-align: center">
                        <strong>備註</strong>
                    </td>
                    <td>
                        <textarea id="Text3" name="OpinionRemarks" class="form-control" cols="55" rows="3" placeholder="備註描述" maxlength="1000" onkeyup=""
                        style="resize: none; background-color: #ffffbb"></textarea>
                    </td>
                    <td style="text-align: center; ">
                        <strong>工作人員</strong>
                    </td>
                    <td>
                        <input type="text" id="Text4" /> 
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center">
                        <strong>進度狀態</strong>
                    </td>
                    <td>
                        <select id="Text5" name="Schedule_Status" style="background-color: #ffffbb; width: 100%;">
                            <option value="">請選擇進度狀態…</option>
                            <option value="已完成">已完成</option>
                            <option value="未完成">未完成</option>
                            <option value="取消">取消</option>
                        </select>
                    </td>
                </tr>
                        </tbody>
                    </table>
                    </div>
                    <div  class="modal-footer">
                        <button type="button" class="btn btn-success btn-lg" data-toggle="modal"  style="Font-Size: 20px;"  onclick="UpdateDaily()"><span class='glyphicon glyphicon-ok'></span>&nbsp;&nbsp;修改</button>
                        <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                    </div>
                </div>
            </div>
          </div>--%>
        <script>
            $.datetimepicker.setLocale('ch');
            $('#datetimepicker01').datetimepicker({
                allowTimes: [
                    '00:00', '00:30', '01:00', '01:30', '02:00', '02:30', '03:00', '03:30', '04:00', '04:30', '05:00', '05:30',
                    '06:00', '06:30', '07:00', '07:30', '08:00', '08:30', '09:00', '09:30', '10:00', '10:30', '11:00', '11:30',
                    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00', '16:30', '17:00', '17:30',
                    '18:00', '18:30', '19:00', '19:30', '20:00', '20:30', '21:00', '21:30', '22:00', '22:30', '23:00', '23:30'
                ]
            });

            $('#datetimepicker02').datetimepicker({
                allowTimes: [
                    '00:00', '00:30', '01:00', '01:30', '02:00', '02:30', '03:00', '03:30', '04:00', '04:30', '05:00', '05:30',
                    '06:00', '06:30', '07:00', '07:30', '08:00', '08:30', '09:00', '09:30', '10:00', '10:30', '11:00', '11:30',
                    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00', '16:30', '17:00', '17:30',
                    '18:00', '18:30', '19:00', '19:30', '20:00', '20:30', '21:00', '21:30', '22:00', '22:30', '23:00', '23:30'
                ]
            });
            $('#datetimepicker03').datetimepicker({
                allowTimes: [
                    '00:00', '00:30', '01:00', '01:30', '02:00', '02:30', '03:00', '03:30', '04:00', '04:30', '05:00', '05:30',
                    '06:00', '06:30', '07:00', '07:30', '08:00', '08:30', '09:00', '09:30', '10:00', '10:30', '11:00', '11:30',
                    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00', '16:30', '17:00', '17:30',
                    '18:00', '18:30', '19:00', '19:30', '20:00', '20:30', '21:00', '21:30', '22:00', '22:30', '23:00', '23:30'
                ]
            });

            $(function () {
                $('.chosen-select').chosen();
                $('.chosen-select-deselect').chosen({ allow_single_deselect: true });
                $('.chosen-single').css({ 'background-color': '#ffffbb' }); //
            });
        </script>

    <!---------------------行事曆------------------------>
    <h2><strong>&nbsp; &nbsp; 每日進度&nbsp; &nbsp;</strong>
    </h2>

    <div class="table-responsive" style="width: auto; margin: auto">
       <table  id="data" class="display table table-striped" style="width: 99%">
            
            <thead>
                <tr>
<%--                    <th style="text-align: center;" width: 8%;>
                        <strong>儲存日期</strong>
                    </th>--%>
<%--                    <th style="text-align: center;" width: 10%;>資料修改時間</th>--%>
                    <th style="text-align: center;" width: 10%;>客戶名稱</th>                
                    <th style="text-align: center;" width: 10%;>工作人員</th>
                    <th style="text-align: center;" width: 10%;>預計時程</th>
                    <th style="text-align: center;" width: 10%;>進度狀態</th>               
                    <th style="text-align: center;" width: 10%;>編輯</th>
                </tr>
                <%--  =========== 勞工資料 ===========--%>
           
            </thead>
        </table>
        </div>
        <div id="Calendar" style="width:auto"></div>

</asp:Content>

