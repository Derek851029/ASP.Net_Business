<%@ Page Language="C#" EnableEventValidation="true" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="0010010001.aspx.cs" Inherits="_0010010001" %>

<asp:Content ID="Content" ContentPlaceHolderID="head2" runat="Server">
    

    <link href="../css/jquery.datetimepicker.min.css" rel="stylesheet" />
    <script src="../DataTables/jquery.dataTables.min.js"></script>
    <script src="../js/jquery.datetimepicker.full.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.13.5/xlsx.full.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.13.5/jszip.js"></script>
    <script type="text/javascript">
        $(function () {
            $('[data-toggle="tooltip"]').tooltip();
            bindTable();
            GroupList()
            ShowTime();
            document.getElementById('add_line').addEventListener("click", function () {
                if ($('#oj_keyword2').css('display') == 'none') {
                    document.getElementById('oj_keyword2').style.display = "";
                    
                } else if ($('#oj_keyword3').css('display') == 'none') {
                    document.getElementById('oj_keyword3').style.display = "";
                }
            }, false);
        });

        //取得建立者&權限
        var menu = document.getElementById('menu_number').innerHTML;
        var menu_permission = document.getElementById('menu_permission').innerHTML;
        var Owner = menu.slice(11); //擷取從前面數第11個字之後的字串
        var Permission = menu_permission.slice(11, -12);

        //取得公司和部門
        var compony = document.getElementById('menu_compony').innerHTML;
        //var department = document.getElementById('menu_department').innerHTML;

        //判斷是否為Excel檔案
        $("body").on("click", "#upload", function () {
            var fileUpload = $("#fileUpload")[0];
            var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.xls|.xlsx)$/;
            if (regex.test(fileUpload.value.toLowerCase())) {
                if (typeof (FileReader) != "undefined") {
                    var reader = new FileReader();
                    //For Browsers other than IE.
                    if (reader.readAsBinaryString) {
                        reader.onload = function (e) {
                            ProcessExcel(e.target.result);
                        };
                        reader.readAsBinaryString(fileUpload.files[0]);
                    } else {
                        //For IE Browser.
                        reader.onload = function (e) {
                            var data = "";
                            var bytes = new Uint8Array(e.target.result);
                            for (var i = 0; i < bytes.byteLength; i++) {
                                data += String.fromCharCode(bytes[i]);
                            }
                            ProcessExcel(data);
                        };
                        reader.readAsArrayBuffer(fileUpload.files[0]);
                    }
                } else {
                    alert("此瀏覽器不支援HTML５");
                }
            } else {
                alert("請上傳Excel檔案！");
            }
        });
        function ProcessExcel(data) {
            var workbook = XLSX.read(data, {
                type: 'binary'
            });
            var firstSheet = workbook.SheetNames[0];
            var excelRows = XLSX.utils.sheet_to_row_object_array(workbook.Sheets[firstSheet]);
            for (var i = 0; i<excelRows.length; i++) {
                $.ajax({
                    url: '0010010001.aspx/ProcessExcel',
                    type: 'POST',
                    data: JSON.stringify({
                        Owner: Owner,
                        CONTACT: excelRows[i].全名,
                        BUSINESSNAME: excelRows[i].公司,
                        Department: excelRows[i].部門,
                        BUSINESSNAME_EN: excelRows[i].職稱,
                        COMPANY_PHONE: excelRows[i].商務電話,
                        EMAIL: excelRows[i].商務電子郵件,
                        FAX_PHONE: excelRows[i].商務傳真,
                        CONTACT_PHONE: excelRows[i].手機電話,
                        CONTACT_ADDR: excelRows[i].商務地址,
                        COMPANY_REMARK: excelRows[i].商務地址2,
                        ID: excelRows[i].統一編號
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                        var json = JSON.parse(doc.d.toString());
                    }
                });
            }
            alert('【新增客戶資料成功】');
            window.location.reload();
        }

        function bindTable() {
            $.ajax({
                url: '0010010001.aspx/GetPartnerList',
                type: 'POST',
                data: JSON.stringify({
                    Owner: Owner,
                    Permission: Permission
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#data').DataTable({
                        destroy: true,
                        data: eval(doc.d),
                        "oLanguage": {
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
                        columns: [                                      // 顯示資料列
                            { data: "BUSINESSNAME" },
                            //{ data: "ID" },
                            //{ data: "CONTACT_ADDR" },
                            //{ data: "CONTACT" },
                            //{ data: "CONTACT_PHONE" },
                            //{ data: "COMPANY_PHONE" },
 
                            {
                                data: "PID", render: function (data, type, row, meta) {  
                                    return "<button type='button' id='edit' class='btn btn-info btn-lg' " +
                                        "data-toggle='modal' data-target='#newModal' >" +
                                        "<span class='glyphicon glyphicon-search'>" +
                                        "</span></button>";
                                }
                            },
                            {
                                data: "PID", render: function (data, type, row, meta) {
                                    return "<button type='button' class='btn btn-danger btn-lg' id='delete'>" +
                                        "<span class='glyphicon glyphicon-remove'>" +
                                        "</span></button>";

                                }
                            }

                        ]
                    });
                    $('#data tbody').unbind('click').
                            on('click', '#edit', function () {
                            var table = $('#data').DataTable();
                             var PID = table.row($(this).parents('tr')).data().PID;
                             Load_Modal(PID); //抓取上面data獲取的ID帶進去
                        }).on('click', '#delete', function () {
                            var table = $('#data').DataTable();
                            var PID = table.row($(this).parents('tr')).data().PID;
                            Delete(PID);
                        });
                }
            });
        }

        function Delete(PID) {
            if (confirm("確定要刪除嗎？")) {
                $.ajax({
                    url: '0010010001.aspx/Delete',
                    ache: false,
                    type: 'POST',
                    //async: false,
                    data: JSON.stringify({ ID: ID }),
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

        function URL(PID) {
            $.ajax({
                url: '0010010001.aspx/URL',
                type: 'POST',
                data: JSON.stringify({ PID: PID }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    if (json.type == "ok") {
                        window.location = json.status;
                    } else {
                        alert(json.status);
                    }
                }
            });
        }

        function URL2() {
            window.location.href = "/0010010001.aspx";
        }

        function ShowTime() {                               //自動抓現在時間(實行指令時)
            var NowDate = new Date();
            var h = NowDate.getHours();
            var m = NowDate.getMinutes();
            var s = NowDate.getSeconds();
            var y = NowDate.getFullYear();
            var mon = NowDate.getMonth() + 1;
            var d = NowDate.getDate();
            <%--if (mon < 10) {
                if (d < 10) {
                    if (h < 10) {
                        document.getElementById('LoginTime').value = y + "/0" + mon + "/0" + d + " " + h + ":" + m;
                    }
                } else { document.getElementById('LoginTime').value = y + "/0" + mon + "/" + d + " " + h + ":" + m; }
            } else {
                if (d < 10) {
                    document.getElementById('LoginTime').value = y + "/" + mon + "/0" + d + " " + h + ":" + m;
                } else { document.getElementById('LoginTime').value = y + "/" + mon + "/" + d + " " + h + ":" + m; }
                   }--%>
            document.getElementById('LoginTime').value = y + "/" + mon + "/" + d + " " + h + ":" + m;
            //document.getElementById('EstimatedFinishTime').value = y + "/" + mon + "/" + d + " " + h + ":" + m;
        }

        function Xin_De() {     //按新增客戶資料後
            Load_Modal('0');
        }

        function Load_Modal(PID) {// 讀資料
            if (PID == 0) {
                document.getElementById("id").disabled = false;
                document.getElementById("btn_new").style.display = "";
                document.getElementById("btn_update").style.display = "none";
                document.getElementById("title_modal").innerHTML = "客戶資料（新增）";
                document.getElementById('ID').innerHTML = "";
                document.getElementById("business_name").value = "";
                document.getElementById("business_en").value = "";
                document.getElementById("id").value = "";
                document.getElementById("Text1").value = "";
                document.getElementById("Text3").value = "";
                document.getElementById("Text4").value = "";
                document.getElementById("Text10").value = "";
                document.getElementById("Text11").value = "";
                document.getElementById("Text12").value = "";
                document.getElementById("department").value = "";
                document.getElementById("Text39").value = "";
                document.getElementById("Text40").value = "";
                document.getElementById("time_06").innerHTML = "";  //*/
                document.getElementById("Check_Saturday").checked = "checked";
                document.getElementById("Check_Sunday").checked = "checked";
                document.getElementById("oj_keyword2").style.display = "none"; 
                document.getElementById("oj_keyword3").style.display = "none"; 
            } else {
                document.getElementById('id').disabled = true; //鎖定輸入框
                document.getElementById('LoginTime').disabled = true;
                document.getElementById("btn_update").style.display = "";                               //顯示修改鈕
                document.getElementById("btn_new").style.display = "none";                          //隱藏新增鈕
                document.getElementById("title_modal").innerHTML = '客戶資料（修改）';
                Load_Data(PID);
            }   //else 結束
        }

        // 預定修改執行部分
        function Load_Data(PID) {
            document.getElementById("ID").innerHTML = PID;
            $.ajax({
                url: '0010010001.aspx/Load_Data',
                type: 'POST',
                data: JSON.stringify({ PID: PID }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var text = '{"data":' + doc.d + '}';
                    var obj = JSON.parse(text);
                    document.getElementById("business_name").value = obj.data[0].BUSINESSNAME;
                    document.getElementById("business_en").value = obj.data[0].BUSINESSNAME_EN;
                    document.getElementById("id").value = obj.data[0].ID;
                    document.getElementById("Text1").value = obj.data[0].CONTACT;
                    document.getElementById("Text3").value = obj.data[0].CONTACT_PHONE;
                    document.getElementById("Text4").value = obj.data[0].EMAIL;
                    document.getElementById("Text10").value = obj.data[0].CONTACT_ADDR;
                    document.getElementById("Text11").value = obj.data[0].COMPANY_PHONE;
                    document.getElementById("Text12").value = obj.data[0].FAX_PHONE;
                    document.getElementById("department").value = obj.data[0].Department;
                    document.getElementById("Text39").value = obj.data[0].CUSTOMER_REMARK;
                    document.getElementById("Text40").value = obj.data[0].COMPANY_REMARK;
                    document.getElementById("time_06").innerHTML = obj.data[0].SetupDate;
                    document.getElementById("Check_Saturday").checked = obj.data[0].Check_Saturday;
                    document.getElementById("Check_Sunday").checked = obj.data[0].Check_Sunday;
                    if (obj.data[0].ojkeyword2 != "") {
                        document.getElementById('oj_keyword2').style.display = "";
                    }
                    if (obj.data[0].ojkeyword3 != "") {
                        document.getElementById('oj_keyword3').style.display = "";
                    }
                    document.getElementById("oj_keyword").value = obj.data[0].ojkeyword;
                    document.getElementById("oj_keyword2").value = obj.data[0].ojkeyword2;
                    document.getElementById("oj_keyword3").value = obj.data[0].ojkeyword3;
                }
            });
            $("#Div_Loading").modal('hide');        // 功能??
        }

        //================ 存新客戶資料用===============
        function Safe(Flag) {
            document.getElementById("btn_update").disabled = true;
            document.getElementById("btn_new").disabled = true;
           
            var Flag = Flag;
            if (Flag == 0) {
                let BUSINESSNAME = document.getElementById("business_name").value;
                let BUSINESSNAME_EN = document.getElementById("business_en").value;
                let ID = document.getElementById("id").value;
                let CONTACT = document.getElementById("Text1").value;
                let CONTACT_PHONE = document.getElementById("Text3").value;
                let EMAIL = document.getElementById("Text4").value;
                let CONTACT_ADDR = document.getElementById("Text10").value;
                let COMPANY_PHONE = document.getElementById("Text11").value;
                let FAX_PHONE = document.getElementById("Text12").value;
                let CUSTOMER_REMARK = document.getElementById("Text39").value;
                let COMPANY_REMARK = document.getElementById("Text40").value;
                let UpDateDate = document.getElementById("LoginTime").value;
                let SetupDate = document.getElementById("LoginTime").value;
                let Department = document.getElementById("department").value;
                let C_Saturday = document.getElementById("Check_Saturday").checked;
                let C_Sunday = document.getElementById("Check_Sunday").checked;
                let ojkeyword = document.getElementById("oj_keyword").value;
                let ojkeyword2 = document.getElementById("oj_keyword2").value;
                let ojkeyword3 = document.getElementById("oj_keyword3").value;

                $.ajax({
                    url: '0010010001.aspx/Safe',
                    type: 'POST',
                    data: JSON.stringify({
                        Owner: Owner, Flag: Flag, BUSINESSNAME: BUSINESSNAME, BUSINESSNAME_EN: BUSINESSNAME_EN, ID: ID, CONTACT: CONTACT, CONTACT_PHONE: CONTACT_PHONE,
                        EMAIL: EMAIL, CONTACT_ADDR: CONTACT_ADDR, COMPANY_PHONE: COMPANY_PHONE, FAX_PHONE: FAX_PHONE, CUSTOMER_REMARK: CUSTOMER_REMARK,
                        COMPANY_REMARK: COMPANY_REMARK, UpDateDate: UpDateDate, SetupDate: SetupDate, Department: Department, C_Saturday: C_Saturday, C_Sunday: C_Sunday,
                        ojkeyword: ojkeyword, ojkeyword2: ojkeyword2, ojkeyword3: ojkeyword3
                        // 共讀取 16 個 含Flag
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                        var json = JSON.parse(doc.d.toString());
                        alert(json.status);
                        if (json.status == '【新增完成！】') {
                            window.location.reload();
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
        }

        //================ 新增【使用者權限】===============
        function New(Flag) {
            document.getElementById("btn_update").disabled = true;
            document.getElementById("btn_new").disabled = true;
            document.getElementById("edit").disabled = true;
            let PID = document.getElementById("ID").innerHTML;
            var Flag = Flag;
            if (Flag == 1) {
                let BUSINESSNAME = document.getElementById("business_name").value;
                let BUSINESSNAME_EN = document.getElementById("business_en").value;
                let ID = document.getElementById("id").value;
                let CONTACT = document.getElementById("Text1").value;
                let CONTACT_PHONE = document.getElementById("Text3").value;
                let EMAIL = document.getElementById("Text4").value;
                let CONTACT_ADDR = document.getElementById("Text10").value;
                let COMPANY_PHONE = document.getElementById("Text11").value;
                let FAX_PHONE = document.getElementById("Text12").value;
                let CUSTOMER_REMARK = document.getElementById("Text39").value;
                let COMPANY_REMARK = document.getElementById("Text40").value;
                let UpDateDate = document.getElementById("LoginTime").value;
                let SetupDate = document.getElementById("LoginTime").value;
                let C_Saturday = document.getElementById("Check_Saturday").checked;
                let C_Sunday = document.getElementById("Check_Sunday").checked;
                let ojkeyword = document.getElementById("oj_keyword").value;
                let ojkeyword2 = document.getElementById("oj_keyword2").value;
                let ojkeyword3 = document.getElementById("oj_keyword3").value;

                $.ajax({
                    //alert: ("新增完成！"),
                    url: '0010010001.aspx/New',
                    type: 'POST',
                    data: JSON.stringify({
                        Flag: Flag, BUSINESSNAME: BUSINESSNAME, BUSINESSNAME_EN: BUSINESSNAME_EN, ID: ID, CONTACT: CONTACT, CONTACT_PHONE: CONTACT_PHONE,
                        EMAIL: EMAIL, CONTACT_ADDR: CONTACT_ADDR, COMPANY_PHONE: COMPANY_PHONE, FAX_PHONE: FAX_PHONE, CUSTOMER_REMARK: CUSTOMER_REMARK,
                        COMPANY_REMARK: COMPANY_REMARK, UpDateDate: UpDateDate, SetupDate: SetupDate, C_Saturday: C_Saturday, C_Sunday: C_Sunday,
                        ojkeyword: ojkeyword, ojkeyword2: ojkeyword2, ojkeyword3: ojkeyword3,PID,PID
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                        var json = JSON.parse(doc.d.toString());
                        alert(json.status);
                        if (json.status == '【修改完成！】') {
                            window.location.reload();
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
        }

        //==================【新增群組】===============
        function SafeGroup() {
            let GroupName = document.getElementById("GroupName").value;
            $.ajax({
                url: '0010010001.aspx/SafeGroup',
                type: 'POST',
                data: JSON.stringify({ Owner: Owner, GroupName: GroupName }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    alert(json.status);
                    if (json.status == '【新增成功】') {
                        window.location.reload();
                    }
                }
            });
        }

        function GroupList() {
            $.ajax({
                url: '0010010001.aspx/GroupList',
                type: 'POST',
                data: JSON.stringify({ Owner: Owner }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#Group').DataTable({
                        destroy: true,
                        data: eval(doc.d),
                        "oLanguage": {
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
                        columns:[ // 顯示資料列
                            { data: "GroupName" },
                            {
                                data: "SEARCH", render: function (data, type, row, meta) {
                                    return "<button type='button' id='search' class='btn btn-info btn-lg' " +
                                        "data-toggle='modal' data-target='#Div1' >" +
                                        "<span class='glyphicon glyphicon-search'>" +
                                        "</span></button>";
                                }
                            },
                            {
                                data: "EDIT", render: function (data, type, row, meta) {
                                    return "<button type='button' id='edit' class='btn btn-primary btn-lg' " +
                                        "data-toggle='modal' data-target='#Div2'>" +
                                        "<span class='glyphicon glyphicon-plus'>" +
                                        "</span></button>";

                                }
                            },
                            {
                                data: "DELETE", render: function (data, type, row, meta) {
                                    return "<button type='delete' class='btn btn-danger btn-lg' id='delete'>" +
                                        "<span class='glyphicon glyphicon-remove'>" +
                                        "</span></button>";

                                }
                            }
                        ]
                    });
                    $('#Group tbody').unbind('click')
                       . on('click', '#search', function () {
                           var GROUPID = table.row($(this).parents('tr')).data().GroupName;
                           SearchGroup(GROUPID)
                        })
                        .on('click', '#edit', function () {
                            var GROUPID = table.row($(this).parents('tr')).data().GroupName;
                            document.getElementById('group').value = GROUPID;
                            CustomerList(GROUPID)
                        }).on('click', '#delete', function () {
                            var GROUPID = table.row($(this).parents('tr')).data().GroupName;
                            DeleteGroup(GROUPID);
                        });
                }
            });
        }

        function SearchGroup(GROUPID) {
            $.ajax({
                url: '0010010001.aspx/SearchGroup',
                type: 'POST',
                data: JSON.stringify({ GROUPID: GROUPID, Owner: Owner }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#GroupOwner').DataTable({
                        destroy: true,
                        data: eval(doc.d),
                        "oLanguage": {
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
                        columns: [              // 顯示資料列
                            { data: "BUSINESSNAME" },
                            //{ data: "EMAIL" },
                            { data: "CONTACT" },
                            { data: "CONTACT_PHONE" },
                            //{ data: "COMPANY_PHONE" },
                            {
                                data: "BUSINESSNAME",
                                render: function (data, type, row, meta) {
                                    return "<div class='checkbox' ><label>" +
                                        "<input type='checkbox' style='width: 30px; height: 30px;' id='chack2' name='chack2' value='"+data+"'/>" +
                                        "</label></div>";
                                }
                            }
                        ]
                    });
                    var name = [];
                    var btn = document.getElementById("Remove_gp");
                    btn.addEventListener('click', function () {
                        var rowcollection = table.$('input[name="chack2"]:checked', { "page": "all" }); //獲取table中name=check2打勾的地方
                        rowcollection.each(function (index, elem) { // 抓到打勾的value($(elem).val())並push到陣列中
                            name.push($(elem).val());
                            RemoveGroup(name)
                        });
                    }, false)
                }
            });
        }

        function CustomerList(GROUPID) {
            $.ajax({
                url: '0010010001.aspx/CustomerList',
                type: 'POST',
                data: JSON.stringify({ Owner: Owner, GROUPID: GROUPID}),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#data_company').DataTable({
                        destroy: true,
                        data: eval(doc.d),
                        "oLanguage": {
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
                        columns: [ // 顯示資料列
                            { data: "BUSINESSNAME" },
                            { data: "CONTACT" },
                            {
                                data: "BUSINESSNAME",
                                render: function (data, type, row, meta) {
                                    return "<div class='checkbox' style='text-align:center'><label>" +
                                        "<input type='checkbox' style='width: 30px; height: 30px;' name='chack2' id='chack2' value='"+data+"' />" +
                                        "</label></div>";
                                }
                            }
                        ]
                    });
                    var name = [];
                    var message = '請選擇公司'
                    var btn = document.getElementById("AddGroup");
                    btn.addEventListener('click', function () { //當新增按鈕被點時
                        var rowcollection = table.$('input[name="chack2"]:checked', { "page": "all" }); //獲取table中name=check2打勾的地方
                        rowcollection.each(function (index, elem) { // 抓到打勾的value($(elem).val())並push到陣列中
                            name.push($(elem).val());
                            AddToCompany(name)
                        });
                    },false)
                }
            });
        }

        function AddToCompany(name) {
            var GROUPID = document.getElementById('group').value;
            for (var i = 0; i < name.length; i++) {
                $.ajax({
                    url: '0010010001.aspx/AddToCompany',
                    type: 'POST',
                    data: JSON.stringify({
                        BUSINESSNAME: name[i],
                        GROUPID: GROUPID
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                    },
                });
            }
            alert('【新增完成】')
            window.location.reload();
        }

        function RemoveGroup(name) {
            for (var i = 0; i < name.length; i++) {
                $.ajax({
                    url: '0010010001.aspx/RemoveGroup',
                    type: 'POST',
                    data: JSON.stringify({
                        BUSINESSNAME: name[i],
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                    },
                });
            }
            alert('【移除成功】')
            window.location.reload();
        }

        function DeleteGroup(GROUPID) {
            if (confirm("確定要刪除嗎？")) {
                $.ajax({
                    url: '0010010001.aspx/DeleteGroup',
                    type: 'POST',
                    data: JSON.stringify({
                        GROUPID: GROUPID
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",       //如果要回傳值，請設成 json
                    success: function (data) {
                        var json = JSON.parse(data.d.toString());
                        if (json.status == "success") {
                            alert('【刪除群組成功】');
                            GroupList();
                        }
                    }
                });
            }
        }
        
    </script>
    <style>
/*        #navbar-example{
            float: right;
            left:-50%;
            position:relative
        }
        #ul{
            float:left;
            left:50%;
            position:relative;
        }*/
        body
        {
            font-family: "Microsoft JhengHei",Helvetica,Arial,Verdana,sans-serif;
            font-size: 18px;
            padding-left:8px;
            padding-right:8px;
        }

        thead th
        {
            background-color: #666666;
            color: white;
        }

        tr td:first-child,
        tr th:first-child
        {
            border-top-left-radius: 8px;
            border-bottom-left-radius: 8px;
        }

        tr td:last-child,
        tr th:last-child
        {
            border-top-right-radius: 8px;
            border-bottom-right-radius: 8px;
        }

        #data2 td:nth-child(6), #data2 td:nth-child(5), #data2 td:nth-child(4),
        #data2 td:nth-child(3), #data2 td:nth-child(2), #data2 td:nth-child(1),
        #data td:nth-child(6), #data td:nth-child(5), #data td:nth-child(4),
        #data td:nth-child(3), #data td:nth-child(2), #data td:nth-child(1), #data th:nth-child(5)
        {
            text-align: center;
        }
        strong span{
            color:red;
        }
        
    </style>

    <!-- ====== 母資料新增修改表 ====== -->
    <div class="modal fade" style="width:auto; margin: auto" id="newModal" role="dialog" data-backdrop="static" data-keyboard="false">
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
                                     <span style="font-size: 25px"><strong>客戶資料<label id="ID" style="display:none"></label></strong></span>
                                </div>

                    <br />

                                    <strong>公司名稱<span>*</span></strong>
                                <div>
                                    <div data-toggle="tooltip" title="必填，不能超過５０個字">
                                        <input  type="text" id="business_name" name="business_name" autocomplete="off" class="form-control"  placeholder="中文名稱" 
                                            maxlength="50" style="resize: none; background-color: #ffffbb "/>
                                    </div>
                                </div>
                                    <strong>職稱<span>*</span></strong>
                                <div>
                                    <div data-toggle="tooltip" title="不能超過５０個字">
                                        <input type="text" id="business_en" name="business_id" autocomplete="off" class="form-control"  placeholder="職稱" 
                                            maxlength="50" style="resize: none; background-color: #ffffbb"/>
                                    </div>
                                </div>
                    <div>
                                    <strong>聯絡人<span>*</span></strong>
                                </div>
                                <div>
                                    <input id="Text1" name="Text1" class="form-control" autocomplete="off" placeholder="聯絡人" 
                                            maxlength="50" style="resize: none; background-color: #ffffbb;" />
                                </div>
                                <div>
                                    <strong>行動電話<span>*</span></strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="不能超過５０個字">
                                        <input type="text" id="Text3" name="txt_Agent_Name" autocomplete="off" class="form-control" placeholder="行動電話"
                                            maxlength="50" onkeyup="value=value.replace(/[^\d]/g,'')" style="background-color: #ffffbb;" />
                                    </div>
                                </div>
                            <!-- ========================================== -->
                                <div>
                                    <strong>統一編號<span>*</span></strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="必填，應填8位數字">
                                        <input type="text" id="id" name="id" autocomplete="off" class="form-control" placeholder="統一編號"
                                            maxlength="8" onkeyup="value=value.replace(/[^\d]/g,'')" style="background-color: #ffffbb" />
                                    </div>
                                </div>
                                <div>
                                    <strong>地址</strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="不能超過２００個字">
                                        <textarea id="Text10" name="Text10" class="form-control" placeholder="通訊地址" 
                                            maxlength="200" style="resize: none;"></textarea>
                                    </div>
                                </div>
                                
                                <div>
                                    <strong>E-mail</strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip">
                                        <input type="text" id="Text4" name="Text4" autocomplete="off" class="form-control" placeholder="E-mail" 
                                            maxlength="100" style="resize: none;"/>
                                    </div>
                                </div>
                                  <div>
                                    <strong>部門</strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="不能超過１００個字">
                                        <input type="text" id="department" name="Text4" autocomplete="off" class="form-control" placeholder="部門" 
                                            maxlength="100" style="resize: none;"/>
                                    </div>
                                </div>
                                <div>
                                    <strong>公司電話</strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="不能超過５０個字">
                                        <input type="text" id="Text11" name="txt_Agent_Name" autocomplete="off" class="form-control" placeholder="公司電話"
                                            maxlength="50" onkeyup="value=value.replace(/[^\d]/g,'')"/>
                                    </div>
                                </div>
                                <div>
                                    <strong>傳真電話</strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="不能超過５０個字">
                                        <input type="text" id="Text12" name="txt_Agent_Name" autocomplete="off" class="form-control" placeholder="傳真電話"
                                            maxlength="50" onkeyup="value=value.replace(/[^\d]/g,'')"/>
                                    </div>
                                </div>
                                <div>
                                    <strong>顧客備註</strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="不能超過２０００個字">
                                        <textarea id="Text39" name="Text39" class="form-control" placeholder="顧客資訊備註" 
                                            maxlength="2000" onkeyup="" style="resize: none;"></textarea>
                                    </div>
                                </div>
                                <div>
                                    <strong>公司資訊備註</strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="不能超過２０００個字">
                                        <textarea id="Text40" name="Text40" class="form-control" placeholder="公司資訊備註" 
                                            maxlength="2000" onkeyup="" style="resize: none;"></textarea>
                                    </div>
                                </div>
                            <div>
                                <div>
                                    <strong>相關產品</strong>
                                </div>
                                <div style="display:inline-flex">
                                        <input type="text" id="oj_keyword" name="oj_keyword" autocomplete="off" class="form-control" placeholder="相關產品關鍵字" 
                                            maxlength="100" style="resize: none;"/><button id="add_line" type="button" class="btn btn-success btn-sm">新增</button>
                                </div>
                                <div>
                                           <input type="text" id="oj_keyword2" name="oj_keyword2" autocomplete="off" class="form-control" placeholder="相關產品關鍵字" 
                                            maxlength="100" style="resize: none; display:none"/>
                                </div>
                                <div>
                                          <input type="text" id="oj_keyword3" name="oj_keyword3" autocomplete="off" class="form-control" placeholder="相關產品關鍵字" 
                                            maxlength="100" style="resize: none; display:none"/>
                                </div>
                                <div>
                                    <strong>※輸入相關產品後，可自動抓取與產品有關的新聞</strong>
                                </div>
                            </div>
                            <div>
                                <div>
                                    <strong>工作日</strong>
                                </div>
                                <div>
                                    <strong>星期六</strong>
                                    <input id='Check_Saturday' type='checkbox' style='width: 30px; height: 30px;' />&nbsp;
                                    <strong>星期日</strong>
                                    <input id='Check_Sunday' type='checkbox' style='width: 30px; height: 30px;' />&nbsp;
                                </div>
                                <div>
                                    <strong>※打勾代表六日照常計算服務單的工作日</strong>
                                </div>
                            </div>

                    <br />

                                <div>
                                    <strong>最後修檔日期</strong>
                                </div>
                                    <div style="float: left" data-toggle="tooltip" title="自動抓時間">
                                        <input type="text" class="form-control" id="LoginTime" name="LoginTime" style="" value="" />
                                    </div>
                    <br /><br />
                                <div>
                                    <strong>&nbsp;建檔日期</strong>
                                </div>
                                <div>
                                    <div style="float: left" data-toggle="tooltip" title="">
                                        <label id="time_06"></label>
                                    </div>
                                </div>
                        </div>
                <div class="modal-footer">
                    <button id="btn_new" type="button" class="btn btn-success btn-lg" onclick="Safe(0)" data-dismiss="modal"><span class="glyphicon glyphicon-ok"></span>&nbsp;&nbsp;新增</button>
                    <button id="btn_update" type="button" class="btn btn-primary btn-lg" onclick="New(1)" data-dismiss="modal"><span class="glyphicon glyphicon-pencil"></span>&nbsp;&nbsp;修改</button>
                    <button type="button" class="btn btn-danger btn-lg" data-dismiss="modal"><span class="glyphicon glyphicon-remove"></span> &nbsp;取消</button>
                </div>
            </div>
            </div>
      </div>
            <!-- =========== Modal content =========== -->
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
            $('#datetimepicker04').datetimepicker({
                allowTimes: [
                    '00:00', '00:30', '01:00', '01:30', '02:00', '02:30', '03:00', '03:30', '04:00', '04:30', '05:00', '05:30',
                    '06:00', '06:30', '07:00', '07:30', '08:00', '08:30', '09:00', '09:30', '10:00', '10:30', '11:00', '11:30',
                    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00', '16:30', '17:00', '17:30',
                    '18:00', '18:30', '19:00', '19:30', '20:00', '20:30', '21:00', '21:30', '22:00', '22:30', '23:00', '23:30'
                ]
            });
            $('#datetimepicker05').datetimepicker({
                allowTimes: [
                    '00:00', '00:30', '01:00', '01:30', '02:00', '02:30', '03:00', '03:30', '04:00', '04:30', '05:00', '05:30',
                    '06:00', '06:30', '07:00', '07:30', '08:00', '08:30', '09:00', '09:30', '10:00', '10:30', '11:00', '11:30',
                    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00', '16:30', '17:00', '17:30',
                    '18:00', '18:30', '19:00', '19:30', '20:00', '20:30', '21:00', '21:30', '22:00', '22:30', '23:00', '23:30'
                ]
            });

            $('#New_StartTime,#txt_CALL_Time').datetimepicker({
                datepicker: false,
                useSeconds: false,
                format: 'H:i',
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
                $('.chosen-single').css({ 'background-color': '#ffffbb' });
            });
        </script>
    <!--===================================================-->
    <input type="hidden" id="group" name="name" value="" />
    <!--====================客戶資料維護&上傳下載Excel========================-->
    <div style="margin-left:10px;height:245px">
            <div class="row">
                    <div class="col-md-12" style="margin-bottom:15px">
                        <h2>
                            <strong>客戶資料維護&nbsp; &nbsp;</strong>
                        </h2>
                        <button type="button" class="btn btn-success btn-lg" data-toggle="modal" data-target="#newModal" style="Font-Size: 20px;" onclick="Xin_De()">
                        <span class='glyphicon glyphicon-plus'></span>&nbsp;&nbsp;新增客戶資料</button>
                    </div>
                </div>
                        <div style="height:120px;">
                             <div style="height:40px"><span style="color:red">※</span>上傳Excel客戶資料 <a href="DownloadExcel.html" >範例下載</a></div>
                             <input  type="file" id="fileUpload" />
                            <button type="button" id="upload"  class="btn btn-success btn-lg" style="position:absolute; left: 240px;"><span class="glyphicon glyphicon-upload"></span>&nbsp;&nbsp;上傳</button>
                        </div>
            </div>
                    <div>
                        <table id="data" class="display table table-striped " style="width: 100%">
                            <thead>
                                <tr>
                                    <th style="text-align: center; width: 30%;"">名稱</th>
<%--                                    <th style="text-align: center; width: 10%;"">統編</th>--%>
<%--                                    <th style="text-align: center; width: 10%;">地址</th>
                                    <th style="text-align: center; width: 10%">聯絡人</th>
                                    <th style="text-align: center; width: 10%">行動電話</th>
                                    <th style="text-align: center; width: 10%">公司電話</th>--%>
                                    <th style="text-align: center; width: 30%">查看</th>
                                    <!--<th style="text-align: center; width: 10%">子公司</th>-->
                                    <th style="text-align: center; width: 30%">刪除</th>
                                </tr>
                            </thead>
                        </table>
                        </div>
        <div style="margin-left:10px;height:150px">
            <div class="row">
                <div class="col-md-12">
                                    <h2><strong>客戶群組列表&nbsp; &nbsp;</strong></h2>
                                    <button type="button" class="btn btn-success btn-lg" data-toggle="modal"  data-target="#newGroup"style="Font-Size: 20px;">
                                    <span class='glyphicon glyphicon-plus'></span>&nbsp;&nbsp;新增群組</button>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <table id="Group" class="display table table-striped" style="width:99%; text-align:center">
                    <thead>
                        <tr>
                            <th style="text-align: center; width: auto;">名稱</th>
                            <th style="text-align: center; width: auto;">查看</th>
                            <th style="text-align: center; width: auto;">新增</th>
                            <th style="text-align: center; width: auto;">刪除</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    
        <!-- ====== 新增群組 ====== -->
    <div class="modal fade" style="width:80%; margin:auto" id="newGroup" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label id="group_title"></label>
                    </strong></h2>
                </div>
                <div class="modal-body">
                    <!-- =======新增群組 表格======= -->
                    <div class="display table table-striped" style="width: 99%">
                        <div>
                            <div>
                                <div style="text-align: center; background-color: #666666">
                                    <span style="font-size: 20px"><strong>新增群組<label id="title"></label></strong></span>
                                </div>

                                <br />

                            </div>
                        </div>
                        <div>
                            <div>
                                <div>
                                    <strong>群組名稱</strong>
                                </div>
                                <div>
                                    <div data-toggle="tooltip" title="必填，不能超過２０個字">
                                        <input type="text" id="GroupName" name="business_name" autocomplete="off" class="form-control" placeholder="群組名稱"
                                            maxlength="20" style="Font-Size: 18px; background-color: #ffffbb " title="" />
                                    </div>
                                </div>
                            </div>                            
                            
                            <div class="modal-footer">
                                    <button id="GroupNew" type="button" class="btn btn-success btn-lg" onclick="SafeGroup()" data-dismiss="modal"><span class="glyphicon glyphicon-ok"></span>&nbsp;&nbsp;新增</button>
                                    <button type="button" class="btn btn-danger btn-lg" data-dismiss="modal"><span class="glyphicon glyphicon-remove"></span>&nbsp;取消</button>
                                </div>
                            </div>
                    </div>
                </div>
            </div>
        </div>
        </div>
        <!-- ====== 加入群組====== -->
    <div class="modal fade" style="width:100%; margin:auto" " id="Div2" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>選擇客戶</label>
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
                                                <th style="text-align: center;">聯絡人</th>
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
                    <button type="button" class="btn btn-success btn-lg" data-toggle="modal"  style="Font-Size: 20px; float: left;" id="AddGroup"><span class='glyphicon glyphicon-plus'></span>&nbsp;&nbsp;新增至群組</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>
                <!-- =========== 查看(群組列表)=========== -->
    <div class="modal fade" style="width:100%; margin:auto" id="Div1" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>群組列表</label>
                    </strong></h2>
                </div>
                <div class="modal-body">
                    <table class="table table-bordered table-striped" style="width: 99%; text-align:center">
                        <tbody>
                            <tr>
                                <td>
                                    <table id="GroupOwner" class="display table table-striped" style="width:100%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">名稱</th>
<%--                                                <th style="text-align: center;">信箱</th>--%>
                                                <th style="text-align: center;">聯絡人</th>
                                                <th style="text-align: center;">電話</th>
<%--                                                <th style="text-align: center;">公司電話</th>--%>
                                                <th style="text-align: center;">選擇</th>
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
                    <button type="button" id="Remove_gp" class="btn btn-danger btn-lg" data-dismiss="modal" onclick="RemoveGroup()"><span class="glyphicon glyphicon-remove"></span> &nbsp;移除</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                </div>
            </div>
            <!-- =========== Modal content =========== -->
        </div>
    </div>
</asp:Content>
