<%@ Page Title="新聞監測" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="0010010002.aspx.cs" Inherits="KeywordSearch" %>


<asp:Content ID="Content" ContentPlaceHolderID="head2" runat="Server">
    <link href="../css/jquery.datetimepicker.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css" />
    <link href="../js/jquery-ui.css" rel="stylesheet" />
    <link href="../DataTables/jquery.dataTables.min.css" rel="stylesheet" />
    <script src="../js/jquery-ui.js"></script>
    <script src="../DataTables/jquery.dataTables.min.js"></script>
    <script src="../DataTables/jquery.datepicker-zh-TW.js"></script>
    <script src="../js/jquery.datetimepicker.full.min.js"></script>
    <%--<script src="http://code.jquery.com/jquery-1.10.2.js"></script>--%>
    <script src="http://code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
    <script src="https://unpkg.com/current-device/umd/current-device.min.js"></script> <%--判斷手機還是PC, 若網址失效google current device--%>
    <script src="js/notiflix.js"></script> <%--// loading圖案--%>
    <link href="js/notiflix.css" rel="stylesheet" />

    <script type="text/javascript">

        var menu = document.getElementById('menu_number').innerHTML;
        var Owner = menu.slice(11); //擷取從前面數第11個字之後的字串

        var isMobile = device.mobile(); //判斷手機還是PC
        isTable = device.tablet();

        $(function () {
            //Auto_click();

            //手機隱藏電腦datatable, 反之
            if (isMobile) { //如果是手機todo, 不是todo
                document.getElementById("PCtable").style.display = "none";
            } else {
                document.getElementById("Phonetable").style.display = "none";
            }

            $("[id$='sdate'],[id$='edate']").datepicker({ dateFormat: "yy-mm-dd" });

            var sdate = $("[id$='sdate']").val();
            var edate = $("[id$='edate']").val();
            var cookiekey = document.cookie; //取得所有cookie
            var keynumber = cookiekey.indexOf("SearchNews"); //尋找cookie中的關鍵字位置, (因為asp.ent有其他預設cookie)
            cookiekey = cookiekey.substr(keynumber); //印出位置之後的字串
            cookiekey = cookiekey.slice(11, 10000); //擷取第11個位置之後的所有字串
            cookiekey = cookiekey.replaceAll(",", "','"); //將,取代成',' 字串會變成 'xxx','xxx'
            console.log(cookiekey)
            if (isMobile) {
                if (cookiekey == "") {
                } else {
                    bindtable2(cookiekey,sdate, edate);
                }
            } else {
                if (cookiekey == "") {
                } else {
                    bindtable(cookiekey, sdate, edate);
                }
            }

            $('.keyword-select input').click(function () {
                sdate = $("[id$='sdate']").val();
                edate = $("[id$='edate']").val();

                if (sdate > edate) {
                    alert('起始時間不能大於結束時間')
                    return
                }

                var nullstring = "";

                if ($(this).val() == "全部") {
                    if (isMobile) {
                        bindtable2(nullstring,sdate, edate);
                    } else {
                        bindtable(nullstring,sdate, edate);
                    }
                }
                else {
                    if (isMobile) {
                        bindtable2(nullstring,sdate, edate);
                    } else {
                        bindtable(nullstring,sdate, edate);
                    }
                }
            });
            document.cookie = "SearchNews=; expires=Thu, 01 Jan 1970 00:00:00 GMT"; //執行完成後把cookie清除
        });

        //function Auto_click() {
        //    $("#auto_button").trigger("click");
        //    $("#auto_button2").trigger("click");
        //}

        //電腦
        function bindtable(key, sdate, edate) {
                $.ajax({
                    url: '0010010002.aspx/searchresult',
                    type: 'POST',
                    data: JSON.stringify({Owner: Owner, sdate: sdate, edate: edate,key:key }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                        var table = $('#tb_Record').DataTable({
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
                            "bProcessing": true,
                            "columnDefs": [{
                                "targets": -1,
                                "data": null,
                                "searchable": false,
                                "paging": false,
                                "ordering": false,
                                "info": false
                            }],
                            columns: [
                                { data: "date" },
                                { data: "keyword" },
                                { data: "title" },
                                { data: "source" },
                                { data: "browser" },
                                {
                                    data: "href", render: function (data, type, row, meta) {
                                        var restr = "<a id='links' href='" + data + "' target='_blank' data-sysid='0'>瀏覽新聞</a>";
                                        return restr;
                                    }
                                },
                                 {
                                     data: "select",
                                    render: function (data, type, row, meta) {
                                        return "<div><label>" +
                                            "<buttin class='btn btn-success btn lg' id='savenews' style='width:40px'><span class='glyphicon glyphicon-plus'></span>&nbsp;</button>" +
                                            "</label></div>";
                                    }
                                 }

                            ],
                            "order": [[0, "desc"]],
                            "rowCallback": function (row, data, index) {
                                //console.log(data.sysid);
                                $(row).find('a').data('sysid', data.sysid);
                            }
                        });
                    <%--=====================--%>
                        $('#tb_Record tbody').unbind('click')
                            .on('click', '#links', function () {
                                var table = $('#tb_Record').DataTable();
                                var data = table.row($(this).parents('tr')).data();
                                data.browser += 1;
                                table.row($(this).parents('tr')).data(data).draw();
                                var id = $(this).data('sysid');
                                browser(id);
                            })
                            .on('click', '#savenews', function () {
                                var date = table.row($(this).parents('tr')).data().date;
                                var keyword = table.row($(this).parents('tr')).data().keyword;
                                var href = table.row($(this).parents('tr')).data().href;
                                SaveNews(date, keyword, href);
                            });
                    }
                });
        }

        //手機

        function bindtable2(key,sdate, edate) {
            $.ajax({
                url: '0010010002.aspx/searchresult',
                type: 'POST',
                data: JSON.stringify({ Owner: Owner, sdate: sdate, edate: edate,key: key }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var table = $('#tb_Record2').DataTable({
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
                        "bProcessing": true,
                        "columnDefs": [{
                            "targets": -1,
                            "data": null,
                            "searchable": false,
                            "paging": false,
                            "ordering": false,
                            "info": false
                        }],
                        columns: [
                            { data: "date" },
                            { data: "keyword" },
                            {
                                data: "href", render: function (data, type, row, meta) {
                                    var title = data.split(';'); //切割後端傳上來的字串, 利用;切割左右邊, 左邊0, 右邊1
                                    title = title[1];
                                    var restr = "<a id='links' href='" + data + "' target='_blank' data-sysid='0'>'" + title + "'</a>";
                                    return restr;
                                }
                            },
                            {
                                data: "select",
                                render: function (data, type, row, meta) {
                                    return "<div><label>" +
                                        "<buttin class='btn btn-success btn lg' id='savenews'  style='width:40px'><span class='glyphicon glyphicon-plus'></span>&nbsp;</button>" +
                                        "</label></div>";
                                }
                            }

                        ],
                        "order": [[0, "desc"]],
                        "rowCallback": function (row, data, index) {
                            //console.log(data.sysid);
                            $(row).find('a').data('sysid', data.sysid);
                        }
                    });
                    <%--=====================--%>
                    //var BusinessName = [];
                    //var title = [];
                    //var News = [];
                    //var btn = document.getElementById('savenew');
                    //btn.addEventListener('click', function () {
                    //    SaveNews(BusinessName, title, News)
                    //}, false)
                    $('#tb_Record2 tbody').unbind('click')
                        .on('click', '#savenews', function () {
                            var date = table.row($(this).parents('tr')).data().date;
                            var keyword = table.row($(this).parents('tr')).data().keyword;
                            var href = table.row($(this).parents('tr')).data().href;
                            SaveNews(date, keyword, href);
                        });
                }
            });
        }

        function browser(id) {
            $.ajax({
                url: 'KeywordSearch.aspx/browser',
                type: 'POST',
                data: JSON.stringify({ id: id }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {

                }
            });
        }

        function List_company() {
            $.ajax({
                url: '0010010002.aspx/List_company',
                type: 'POST',
                data: JSON.stringify({Owner: Owner}),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    let json = JSON.parse(doc.d.toString());
                    var sdate = document.getElementById('ctl00_head2_sdate').value;
                    var edate = document.getElementById('ctl00_head2_edate').value;
                    for (var key in json) {
                        let keyword = "'" + json[key].BUSINESSNAME + "'";
                        sdate = "'" + sdate + "'";
                        edate = "'" + edate + "'";
                        let company = '<input type="button" class="btn btn-primary btn-lg col-md-2.5" style="white-space: break-spaces; margin-right:10px;" value=' + json[key].BUSINESSNAME + ' onclick="bindtable(' + keyword + ',' + sdate +','+ edate+')"></input>';
                        $("#Businessname").append(company);
                    }
                }
            });
        }

        function SafeKey() {
            var Key = document.getElementById('Key').value;
            $.ajax({
                url: '0010010002.aspx/SafeKey',
                type: 'POST',
                data: JSON.stringify({ Owner: Owner, Key: Key }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    if (json.status == "新增關鍵字成功！") {
                        window.location.reload();
                    }
                }
            });
        }

        function KeyListTable() {
            $.ajax({
                url: '0010010002.aspx/KeyListTable',
                type: 'POST',
                data: JSON.stringify({Owner: Owner}),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#KeyWord').DataTable({
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
                            { data: "KeyWord" },
                            {
                                data: "KeyWord",
                                render: function (data, type, row, meta) {
                                    return "<div class='checkbox'><label>" +
                                        "<input type='checkbox' style='width: 30px; height: 30px; right: 40px;' id='chack' name='chack' value='"+data+"'/>" +
                                        "</label></div>";
                                }
                            }
                        ]
                    });
                    var key = [];
                    var deletebtn = document.getElementById('DeleteKeyWord');
                    deletebtn.addEventListener('click', function () {
                        var rowcollection = table.$('input[name="chack"]:checked', { "page": "all" }); //獲取table中name=check2打勾的地方
                        rowcollection.each(function (index, elem) { // 抓到打勾的value($(elem).val())並push到陣列中
                            key.push($(elem).val())
                            DeleteKeyWord(key)
                        });
                    }, false)
                    //$('#KeyWord tbody').unbind('click').
                    //    on('click', '#chack', function () {
                    //        var KeyWord = table.row($(this).parents('tr')).data().KeyWord;
                    //        if ($(this).prop("checked")) {
                    //            key.push(KeyWord)
                    //        }
                    //    })
                }
            });
        }

        function DeleteKeyWord(key) {
            for (var i = 0; i < key.length; i++) {
                $.ajax({
                    url: '0010010002.aspx/DeleteKeyWord',
                    type: 'POST',
                    data: JSON.stringify({
                        KeyWord: key[i],
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                    },
                });
            }
            alert("刪除關鍵字成功！");
            window.location.reload();
        }

        //function ListKeyWord() {
        //    $.ajax({
        //        url: '0010010002.aspx/ListKeyWord',
        //        data: JSON.stringify({Owner: Owner}),
        //        type: 'POST',
        //        contentType: 'application/json; charset=UTF-8',
        //        dataType: "json",       //如果要回傳值，請設成 json
        //        success: function (doc) {
        //            let json = JSON.parse(doc.d.toString());
        //            var sdate = document.getElementById('ctl00_head2_sdate').value; //ID從f12獲取
        //            var edate = document.getElementById('ctl00_head2_edate').value;
        //            for (var key in json) {
        //                let keyword = "'" + json[key].KeyWord + "'";
        //                sdate = "'" + sdate + "'";
        //                edate = "'" + edate + "'";
        //                let Word = '<input type="button" class="btn btn-primary btn-lg col-md-2.5" style="white-space: break-spaces; margin-right:10px;" value= ' + json[key].KeyWord + ' onclick="bindtable(' + keyword + ',' + sdate + ',' + edate +')"></input>';
        //                $("#ListKey").append(Word);
        //            }
        //        }
        //    });
        //}

        function SaveNews(date, keyword, href) {
                $.ajax({
                    url: '0010010002.aspx/SaveNews',
                    type: 'POST',
                    data: JSON.stringify({ Owner: Owner, date: date, keyword: keyword,href: href }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                        var json = JSON.parse(doc.d.toString());
                        alert(json.status);
                    }
                });
        }

        function SaveNewsList() {
            $.ajax({
                url: '0010010002.aspx/SaveNewsList',
                type: 'POST',
                data: JSON.stringify({Owner: Owner}),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#NewList').DataTable({
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
                            { data: "date" },
                            { data: "keyword" },
                            {
                                data: "href", render: function (data, type, row, meta) {
                                    var title = data.split(';'); //切割後端傳上來的字串, 利用;切割左右邊, 左邊0, 右邊1
                                    title = title[1];
                                    var restr = "<a id='links' href='" + data + "' target='_blank' data-sysid='0'>'"+title+"'</a>";
                                    return restr;
                                }
                            },
                            {
                                data: "Remove", render: function () {
                                    return "<div><label>" +
                                        "<buttin class='btn btn-danger btn lg' id='removenew'><span class='glyphicon glyphicon-remove'></span>&nbsp;</button>" +
                                        "</label></div>";
                                }
                            },
                        ]
                    });
                    $('#NewList tbody').unbind('click')
                        .on('click', '#removenew', function () {
                            var href = table.row($(this).parents('tr')).data().href;
                            RemoveNew(href);
                        });
                }
            });
        }

        function RemoveNew(href) {
            href = href.substring(0, href.length - 1); //因為最後出現';'. 刪除最後一個分號
            $.ajax({
                url: '0010010002.aspx/RemoveNew',
                type: 'POST',
                data: JSON.stringify({ href: href}),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    alert(json.status);
                    SaveNewsList();
                }
            });
        }

        function SelectKey() {
            $.ajax({
                url: '0010010002.aspx/SelectKey',
                type: 'POST',
                data: JSON.stringify({ Owner: Owner }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#SearchNew').DataTable({
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
                            { data: "KeyWord" },
                            {
                                data: "KeyWord",
                                render: function (data, type, row, meta) {
                                    return "<div class='checkbox' style=' text-align:center;'><label>" +
                                        "<input type='checkbox' style='width: 30px; height: 30px;' id='chack' name='chack' value='" + data + "'/>" +
                                        "</label></div>";
                                }
                            }
                        ]
                    });
                    var key = [];
                    var searchbtn = document.getElementById('search');
                    searchbtn.addEventListener('click', function () {
                        var rowcollection = table.$('input[name="chack"]:checked', { "page": "all" }); //獲取table中name=check打勾的地方
                        rowcollection.each(function (index, elem) { // 抓到打勾的value($(elem).val())並push到陣列中
                            key.push($(elem).val());
                        });
                        document.getElementById('search').setAttribute("disabled", true);
                        Notiflix.Loading.Standard('Loading...');
                        document.cookie = "SearchNews=; expires=Thu, 01 Jan 1970 00:00:00 GMT";
                        SetFlag(key);
                    }, false)
                }
            });
        }

        function SetFlag(key) {
            for (let i = 0; i < key.length; i++) {
                $.ajax({
                    url: '0010010002.aspx/SetFlag',
                    type: 'POST',
                    async: false ,
                    data: JSON.stringify({ Owner: Owner, key: key[i] }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                    }
                });
            }
            SearchNews(key);
        }

        function SearchNews(key) {
                $.ajax({
                    url: 'http://192.168.2.40:5000/Customer_news',
                    type: 'POST',
                    data: JSON.stringify({ Owner: Owner, key: key }),
                    dataType: "json",
                    success: function (doc) {
                        document.cookie = "SearchNews = " + key + "";
                        Notiflix.Loading.Remove(600);
                            alert('搜尋完成');
                        window.location.reload();
                    },
                    error: function () {
                        document.cookie = "SearchNews = "+key+"";
                        Notiflix.Loading.Remove(600);

                        //搜尋失敗後把flag改回0
                        $.ajax({
                            url: '0010010002.aspx/reductionFlag',
                            type: 'POST',
                            data: JSON.stringify({ Owner: key }),
                            contentType: 'application/json; charset=UTF-8',
                            dataType: "json",
                            success: function (doc) {
                            }
                        });
                        alert("搜尋失敗，請詢問管理員")
                        window.location.reload();
                    },
                });
        }

        function SetAutoSend() {
            $.ajax({
                url: '0010010002.aspx/SetAutoSend',
                type: 'POST',
                data: JSON.stringify({ Owner: Owner }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#SetSchedule').DataTable({
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
                            {
                                data: "BUSINESSNAME",
                                render: function (data, type, row, meta) {
                                    return data+"<input name='BUSINESSNAME' value='"+data+"' style='display: none'</input>"
                                }
                            },
                            {
                                data: "PID",
                                render: function (data, type, row, meta) {
                                    return "<button type='button'  id='editnew' class='btn btn-primary btn-lg' data-toggle='modal' data-target='#EditNew'>" +
                                               "<span class='glyphicon glyphicon-pencil'></span></button>"
                                }
                            },
                            {
                                data: "cycle",
                                render: function (data, type, row, meta) {
                                    //let today = new Date()
                                    //let todayyear = today.getFullYear();
                                    //let todaymonth = today.getMonth();
                                    //todaymonth = todaymonth + 1;
                                    //var disabledDays = ["" + todayyear + "-" + todaymonth + "-30", "" + todayyear + "-" + todaymonth + "-31"];
                                    //$('#scheduledate').datetimepicker({
                                    //    dateFormat: "mm-dd",
                                    //    constrainInput: false,
                                    //    beforeShowDay: function (date) {

                                    //        var string = jQuery.datepicker.formatDate('yy-mm-dd', date);
                                    //        if ($.inArray(string, disabledDays) != -1) {
                                    //            return [false];
                                    //        }
                                    //    },
                                    //    allowTimes: [
                                    //        '00:00', '00:30', '01:00', '01:30', '02:00', '02:30', '03:00', '03:30', '04:00', '04:30', '05:00', '05:30',
                                    //        '06:00', '06:30', '07:00', '07:30', '08:00', '08:30', '09:00', '09:30', '10:00', '10:30', '11:00', '11:30',
                                    //        '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00', '16:30', '17:00', '17:30',
                                    //        '18:00', '18:30', '19:00', '19:30', '20:00', '20:30', '21:00', '21:30', '22:00', '22:30', '23:00', '23:30'
                                    //    ]
                                    //});
                                    return "<select id='Schedule_Status' name='Schedule_Status' class='chosen - single' style='width: 100%; text-align: center'>" +
                                          "<option value='"+data+"'>"+data+"</option>" +
                                            "<option value='無'>無</option>"+
                                            "<option value='每周'>每周</option>"+
                                            "<option value='兩周'>兩周</option>"+
                                            "<option value='一個月'>一個月</option></select >" 
                                }
                            },
                            {
                                data: "day",
                                render: function (data, type, row, meta) {
                                    var r = /^\+?[1-9][0-9]*$/;
                                    let displaystring_week = "";
                                    let displaystring_month = "";
                                    if (data == "星期一" || data == "星期二" || data == "星期三" || data == "星期四" || data == "星期五") {
                                        displaystring_week = "";
                                        displaystring_month = "display:none";
                                    } else if (isNaN(data) == false) {
                                        displaystring_month = "";
                                        displaystring_week = "display:none";
                                    }
                                    return "<select id='week' name='week' class='chosen - single' style='width: 100%; text-align: center; " + displaystring_week + "'>" +
                                        "<option value='"+data+"'>"+data+"</option>" +
                                        "<option value='選擇日期'>選擇星期</option>" +
                                        "<option value='星期一'>星期一</option>" +
                                        "<option value='星期二'>星期二</option>" +
                                        "<option value='星期三'>星期三</option>" +
                                        "<option value='星期四'>星期四</option>" +
                                        "<option value='星期五'>星期五</option></select>" +

                                        "<select id='month' name='month' class='chosen - single' style='width: 100%; text-align: center; " + displaystring_month + "'>" +
                                        "<option value='" + data + "'>" + data + "</option>" +
                                        "<option value='選擇日期'>選擇日期</option>" +
                                        "<option value='1'>1</option>" +
                                        "<option value='2'>2</option>" +
                                        "<option value='3'>3</option>" +
                                        "<option value='4'>4</option>" +
                                        "<option value='5'>5</option>" +
                                        "<option value='6'>6</option>" +
                                        "<option value='7'>7</option>" +
                                        "<option value='8'>8</option>" +
                                        "<option value='9'>9</option>" +
                                        "<option value='10'>10</option>" +
                                        "<option value='11'>11</option>" +
                                        "<option value='12'>12</option>" +
                                        "<option value='13'>13</option>" +
                                        "<option value='14'>14</option>" +
                                        "<option value='15'>15</option>" +
                                        "<option value='16'>16</option>" +
                                        "<option value='17'>17</option>" +
                                        "<option value='18'>18</option>" +
                                        "<option value='19'>19</option>" +
                                        "<option value='20'>20</option>" +
                                        "<option value='21'>21</option>" +
                                        "<option value='22'>22</option>" +
                                        "<option value='23'>23</option>" +
                                        "<option value='24'>24</option>" +
                                        "<option value='25'>25</option>" +
                                        "<option value='26'>26</option>" +
                                        "<option value='27'>27</option>" +
                                        "<option value='28'>28</option></select>"
                                }
                            }
                        ]
                    });
                    $('#SetSchedule tbody').unbind('change')
                        .on('change', '#Schedule_Status', function () {
                            if  ($(this).val() == '每周') {
                                $(this).parents('tr').find('#week').show();
                                $(this).parents('tr').find('#month').hide();
                                $(this).parents('tr').find('#week')[0].selectedIndex = 1; //將select恢復預設值(第一個值)
                                $(this).parents('tr').find('#month')[0].selectedIndex = 1;
                            }
                            else if ($(this).val() == '兩周') {
                                $(this).parents('tr').find('#week').show();
                                $(this).parents('tr').find('#month').hide();
                                $(this).parents('tr').find('#week')[0].selectedIndex = 1;
                                $(this).parents('tr').find('#month')[0].selectedIndex = 1;
                            }
                            else if ($(this).val() == '一個月') {
                                $(this).parents('tr').find('#week').hide();
                                $(this).parents('tr').find('#month').show();
                                $(this).parents('tr').find('#week')[0].selectedIndex = 1;
                                $(this).parents('tr').find('#month')[0].selectedIndex = 1;
                            }
                            else {
                                $(this).parents('tr').find('#week').hide();
                                $(this).parents('tr').find('#month').hide();
                                $(this).parents('tr').find('#week')[0].selectedIndex = 1;
                                $(this).parents('tr').find('#month')[0].selectedIndex = 1;
                            }
                        })
                        .on('click', '#editnew', function () {
                            let BUSINESSNAME = table.row($(this).parents('td')).data().BUSINESSNAME;
                            LoadNews(BUSINESSNAME);
                        });

                    let con_btn = document.getElementById('Schedule_confirm');
                    con_btn.addEventListener('click', function () {
                        var cyclecollection = table.$('select[name="Schedule_Status"]', { "page": "all" });
                        let table_status = true;
                        cyclecollection.each(function (index, elem) {
                            if ($(this).val() != '無') {
                                if ($(this).val() == '每周' || $(this).val() == '兩周') {
                                    if ($(this).parents('tr').find('#week').val() == '選擇日期') {
                                        table_status = false;
                                        return;
                                    }
                                }
                                else if ($(this).val() == '一個月') {
                                    if ($(this).parents('tr').find('#month').val() == '選擇日期') {
                                        table_status = false;
                                        return;
                                    }
                                }
                            }
                        })
                        if (table_status == false) {
                            alert('有日期未選擇，如不需要請在週期選擇"無"');
                            return;
                        }
                        var namecollection = table.$('input[name="BUSINESSNAME"]', { "page": "all" });
                        namecollection.each(function (index, elem) {
                            let thisID = "";
                            switch ($(this).parents('tr').find('#Schedule_Status').val()) {
                                case "每周":
                                    thisID = $(this).parents('tr').find('#week').val()
                                    break;
                                case "兩周":
                                    thisID = $(this).parents('tr').find('#week').val()
                                    break;
                                case "一個月":
                                    thisID = $(this).parents('tr').find('#month').val()
                                    break;
                            }
                            let data = '{';
                            data += '"businessname"' + ':' + '"' + $(this).val() + '"' + ',"cycle"' + ':' + '"' + $(this).parents('tr').find('#Schedule_Status').val() + '"' + ',"day"' + ':' + '"' + thisID + '"' + '}';
                            console.log(data);
                            data = JSON.parse(data);
                            $.ajax({
                                url: '0010010002.aspx/AutoSend_insertSQL',
                                type: 'POST',
                                data: JSON.stringify({
                                    businessname: data.businessname,
                                    cycle: data.cycle,
                                    day: data.day,
                                    Owner: Owner,
                                }),
                                contentType: 'application/json; charset=UTF-8',
                                dataType: "json",
                                success: function (doc) {
                                }
                            });
                        })
                        alert("設定成功！");
                        window.location.reload();
                    }, false)
                }
            });
        }

        var LoadNews_BUSINESSNAME = ""
        function LoadNews(BUSINESSNAME) {
            LoadNews_BUSINESSNAME = BUSINESSNAME;
            $.ajax({
                url: '0010010002.aspx/LoadNews',
                type: 'POST',
                data: JSON.stringify({
                    Owner: Owner,
                    BUSINESSNAME: BUSINESSNAME
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#SendNewList').DataTable({
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
                        "pageLength": 5, //限制顯示長度
                        "lengthMenu": [5],
                        "columnDefs": [{
                            "targets": -1,
                            "data": null,
                            "searchable": false,
                            "paging": false,
                            "ordering": false,
                            "info": false
                        }],
                        columns: [ // 顯示資料列
                            {
                                data: "href", render: function (data, type, row, meta) {
                                    var title = data.split(';'); //切割後端傳上來的字串, 利用;切割左右邊, 左邊0, 右邊1
                                    title = title[1];
                                    var restr = "<a id='links' href='" + data + "' target='_blank' data-sysid='0'>'" + title + "'</a>";
                                    return restr;
                                }
                            },
                            {
                                data: "SYSID", render: function () {
                                    return "<div><label>" +
                                        "<buttin class='btn btn-success btn-lg' id='save_news'><span class='glyphicon glyphicon-plus'></span></button>" +
                                        "</label></div>";
                                }
                            },
                        ]
                    });
                    $('#SendNewList tbody').unbind('click')
                        .on('click', '#save_news', function () {
                            var SYSID = table.row($(this).parents('tr')).data().SYSID;
                            SetAutoType(SYSID);
                        })
                        .on('click', '#Save_news_list', function () {
                            var SYSID = table.row($(this).parents('tr')).data().SYSID;
                            Save_News_List();
                        });
                }
            });
        }

        function SetAutoType(SYSID) {
            $.ajax({
                url: '0010010002.aspx/SetAutoType',
                type: 'POST',
                data: JSON.stringify({
                    Owner: Owner,
                    SYSID: SYSID
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    LoadNews(LoadNews_BUSINESSNAME); //此參數為全域變數
                }
            });
        }

        function Save_News_List() {
            $.ajax({
                url: '0010010002.aspx/Save_News_List',
                type: 'POST',
                data: JSON.stringify({
                    Owner: Owner,
                    LoadNews_BUSINESSNAME: LoadNews_BUSINESSNAME
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",       //如果要回傳值，請設成 json
                success: function (doc) {
                    var table = $('#SaveNewList').DataTable({
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
                        "pageLength": 5, //限制顯示長度
                        "lengthMenu": [5],
                        "columnDefs": [{
                            "targets": -1,
                            "data": null,
                            "searchable": false,
                            "paging": false,
                            "ordering": false,
                            "info": false
                        }],
                        columns: [ // 顯示資料列
                            {
                                data: "href", render: function (data, type, row, meta) {
                                    var title = data.split(';'); //切割後端傳上來的字串, 利用;切割左右邊, 左邊0, 右邊1
                                    title = title[1];
                                    var restr = "<a id='links' href='" + data + "' target='_blank' data-sysid='0'>'" + title + "'</a>";
                                    return restr;
                                }
                            },
                            {
                                data: "SYSID", render: function () {
                                    return "<button type='button'  id='removenew' class='btn btn-danger btn-lg' data-toggle='modal' data-target='#SaveNew'>" +
                                                "<span class='glyphicon glyphicon-remove'></span></button>"
                                }
                            },
                        ]
                    });
                    $('#SaveNewList tbody').unbind('click')
                        .on('click', '#removenew', function () {
                            var SYSID = table.row($(this).parents('tr')).data().SYSID;
                            Remove_Save_New(SYSID);
                        })
                }
            });
        }

        function Remove_Save_New(SYSID) {
            $.ajax({
                url: '0010010002.aspx/Remove_Save_New',
                type: 'POST',
                data: JSON.stringify({
                    Owner: Owner,
                    SYSID: SYSID
                }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    Save_News_List(LoadNews_BUSINESSNAME); //此參數為全域變數
                    LoadNews(LoadNews_BUSINESSNAME);
                }
            });
        }

    </script>
        <style>
        .xdsoft_monthpicker { /*隱藏週期設定日曆上方年月*/
            display: none;
        }

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
        
        .search-result{
            margin: 2px 2px 2px;
        }

        .table th input{
            width:100%;
        }

        button{
            margin-bottom:10px;
        }

        @-webkit-keyframes spin {
              0% { -webkit-transform: rotate(0deg); }
              100% { -webkit-transform: rotate(360deg); }
         }

        @keyframes spin {
              0% { transform: rotate(0deg); }
              100% { transform: rotate(360deg); }
         }

    </style>
<%--    <button type="button"id="auto_button"onclick='List_company()' style="display:none">更新關鍵字公司資料</button>
    <button type="button"id="auto_button2"onclick='ListKeyWord()' style="display:none">更新關鍵字</button>--%>

    <div>
        <h1 style="text-align:center">新聞監測</h1>
        <button type="button" onclick="KeyListTable()" class="btn btn-success btn-lg" data-toggle="modal" data-target="#keyword" style="Font-Size: 20px; margin-bottom:10px" >
        <span class='glyphicon glyphicon-plus'></span>&nbsp;&nbsp;新增/刪除&nbsp關鍵字</button>
        <button type="button" onclick="SelectKey()" class="btn btn-info btn-lg" data-toggle="modal" data-target="#Div3" style="Font-Size: 20px; margin-bottom:10px" >
        <span class='glyphicon glyphicon-search'></span>&nbsp;&nbsp;選擇搜尋的新聞</button>
        <button type="button" onclick="SaveNewsList()" class="btn btn-info btn-lg" data-toggle="modal" data-target="#Div1" style="Font-Size: 20px; margin-bottom:10px" >
        <span class='glyphicon glyphicon-ok'></span>&nbsp;&nbsp;已儲存清單</button>
        <button type="button" id="test" onclick="SetAutoSend()" class="btn btn-warning btn-lg" data-toggle="modal" data-target="#Div4" style="Font-Size: 20px; margin-bottom:10px" >
        <span class='glyphicon glyphicon-cog'></span>&nbsp;&nbsp;週期設定</button>
    </div>
    <br />
    <div style="margin-top:30px">
        <table class="table">
            <thead><tr><th colspan="3">資料的時間範圍</th></tr></thead>
            <tr>
                <th style="width:15%">輸入時間範圍</th>
            </tr>
            <tr>
                <td colspan="2" style="width:10%">
                    <input type="text" id="sdate" runat="server"/>
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/calendar-512.png" Width="20px"  />
                    &nbsp; ~ &nbsp; 
                    <input type="text" id="edate" runat="server"/>
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/images/calendar-512.png" Width="20px"  />
                    <table class="table keyword-select" style="width: 10px">
                        <tr><th><input class="btn btn-success btn-lg" type="button" value="搜尋" /></th></tr>
                    </table>
                </td>
                
            </tr>
        </table>
        </div>

            <!-- =========== 選擇關鍵字並搜尋新聞 =========== -->
<div class="modal fade" style="width:95%" id="Div3" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>選擇搜尋的新聞</label>
                    </strong></h2>
                </div>
                <div class="modal-body">
                    <table id="SearchNew" class="table table-bordered table-striped" style="width: 99%">
                        <tbody>
                            <tr>
                                <td>
                                    <table class="display table table-striped" style="width: 99%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">關鍵字</th>
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
                    <button type="button" id="search" onclick="()" class="btn btn-success btn-lg"style="Font-Size: 20px;" >
                    <span class='glyphicon glyphicon-search'></span>&nbsp;&nbsp;搜尋</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>
    <br /><br />
    <%-----------------------------------For PC-------------------------------%>
    <div class="search-result" id="PCtable" style="width:100%">
        <div id="table_data" class="table-responsive" runat="server">
            <table id="tb_Record" class="display table table-striped" >
            <thead>
                <tr>
                    <th style="text-align: center;width:15%;">日期</th>
                    <th style="text-align: center;width:15%;">關鍵字</th>
                    <th style="text-align: center;width:40%;">標題</th>
                    <th style="text-align: center;width:10%;">來源</th>
                    <th style="text-align: center;width:10%;">瀏覽次數</th>
                    <th style="text-align: center;width:10%;">網址</th>
                    <th style="text-align: center;width:10%;">儲存</th>
                </tr>
            </thead>
        </table>
        </div>
    </div>
        <%-----------------------------------For Phone-------------------------------%>
    <div class="search-result" id="Phonetable" style="width:100%">
        <div id="Div2" class="table-responsive" runat="server">
            <table id="tb_Record2" class="display table table-striped" >
            <thead>
                <tr>
                    <th style="text-align: center;width:15%;">日期</th>
                    <th style="text-align: center;width:15%;">關鍵字</th>
                    <th style="text-align: center;width:40%;">標題</th>
                    <th style="text-align: center;width:5%;">儲存</th>
                </tr>
            </thead>
        </table>
        </div>
    </div>

    <br /><br /><br /><br /><br />

     <!-- ====== 新增關鍵字====== -->
    <div class="modal fade" style="width:95%" id="keyword" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
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
                    <table class="display table table-striped" style="width: 100%">
                        <thead>
                            <tr>
                                <th style="text-align: center" colspan="4">
                                    <span style="font-size: 20px"><strong>新增關鍵字<label id="title"></label></strong></span>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <th style="text-align: center; width: 15%; height: 55px;">
                                    <strong>關鍵字</strong>
                                </th>
                                <th style="text-align: center; width: 35%">
                                    <div data-toggle="tooltip" title="必填，不能超過２０個字">
                                        <input type="text" id="Key"  class="form-control" placeholder="關鍵字"
                                            maxlength="20" style="Font-Size: 18px; background-color: #ffffbb " title="" />
                                    </div>
                                </th>
                            </tr>                            
                            
                            <tr>
                                <th style="text-align: center; width: 15%; height: 55px;"></th>
                                <th style="text-align: right; width: 35%; height: 65px;">
                                    <button type="button" class="btn btn-success btn-lg" onclick="SafeKey()" data-dismiss="modal"><span class="glyphicon glyphicon-ok"></span>&nbsp;&nbsp;新增</button>
                                </th>
                            </tr>
                        </tbody>
                    </table>
                    <br /><br /><br />
          <%-----------------------------------------------------------------------------------%>
                    <table class="table table-bordered table-striped" style="width: 100%">
                         <thead>
                            <tr>
                                <th style="text-align: center" colspan="4">
                                    <span style="font-size: 20px"><strong>刪除關鍵字</strong></span>
                                </th>
                            </tr>
                         </thead>
                        <tbody>
                            <tr>
                                <td>
                                    <table id="KeyWord" class="display table table-striped" style="width: 99%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">關鍵字</th>
                                                <th style="text-align: center;">選擇</th>
                                            </tr>
                                        </thead>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div class="modal-footer" style="width: 80%">
                    <button id="DeleteKeyWord" type="button" class="btn btn-danger btn-lg" onclick="DeleteKey()" data-dismiss="modal"><span class="glyphicon glyphicon-delete"></span>&nbsp;&nbsp;刪除</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
        </div>
    <!-- =========== 儲存新聞列表=========== -->
    <div class="modal fade" style="width:95%" id="Div1" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>新聞列表</label>
                    </strong></h2>
                </div>
                <div class="modal-body">
                    <table class="table table-bordered table-striped" style="width: 99%">
                        <tbody>
                            <tr>
                                <td>
                                    <table id="SetScheldule" class="display table table-striped" style="width: 99%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">公司</th>
                                                <th style="text-align: center;">關鍵字</th>
                                                <th style="text-align: center;">連結</th>
                                                <th style="text-align: center;">移除</th>
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
        </div>
    </div>

<div class="modal fade" style="width:95%" id="Div4" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>週期設定</label>
                    </strong></h2>
                </div>
                <div class="modal-body" style="text-align:center">
                    <table id="SetSchedule" class="table table-bordered table-striped" style="width: 99%">
                        <tbody>
                            <tr>
                                <td>
                                    <table class="display table table-striped" style="width: 99%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">公司</th>
                                                <th style="text-align: center;">新聞篩選</th>
                                                <th style="text-align: center;">週期</th>
                                                <th style="text-align: center;">選擇日期</th>
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
                    <button type="button"  id="Schedule_confirm" class="btn btn-success btn-lg" style="Font-Size: 20px;">
                    <span class='glyphicon glyphicon-ok'></span>&nbsp;&nbsp;確定</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" style="width:95%" id="EditNew" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>篩選新聞</label>
                    </strong></h2>
                    <div style="text-align:right">
                        <button type="button" id="Save_news_list" onclick="Save_News_List()" class="btn btn-info btn-lg" data-toggle='modal' data-target='#List_Save' style="Font-Size: 20px;">
                        <span class='glyphicon glyphicon-search'></span>&nbsp;&nbsp;已加入清單</button>
                    </div>
                </div>
                <div class="modal-body">
                    <table class="table table-bordered table-striped" style="width: 99%">
                        <tbody>
                            <tr>
                                <td>
                                    <table id="SendNewList" class="display table table-striped" style="width: 99%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">連結</th>
                                                <th style="text-align: center;">加入</th>
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
        </div>
    </div>

    <div class="modal fade" style="width:95%" id="List_Save" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" style="margin:auto">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h2 class="modal-title"><strong>
                        <label>加入清單</label>
                    </strong></h2>
                </div>
                <div class="modal-body">
                    <table class="table table-bordered table-striped" style="width: 99%">
                        <tbody>
                            <tr>
                                <td>
                                    <table id="SaveNewList" class="display table table-striped" style="width: 99%">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center;">連結</th>
                                                <th style="text-align: center;">移除</th>
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
        </div>
    </div>
</asp:Content>
