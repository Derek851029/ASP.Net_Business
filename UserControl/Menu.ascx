<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Menu.ascx.vb" Inherits="UserControl_Menu" %>
<script src="../js/jquery.marquee.js"></script>
<script src="../js/jquery.marquee.min.js"></script>
<script type="text/javascript">
        var co = 0;
        //window.setInterval("marqueeString()", "1000");
        $(function () {
            marqueeString();
        })
        function marqueeString() {
            $.ajax({
                type: 'post',
                async: false, //啟用同步請求
                cache: false, //避免一直抓到舊的資料
                url: 'WebServiceDoc.asmx/marquees',
                contentType: 'application/json;utf-8',
                data: JSON.stringify({}),
                dataType: 'json',
                success: function (data) {
                    $('#marquee li').remove()
                    console.log('清除跑馬燈');
                    $(JSON.parse(data.d)).each(function (index, element) {
                        console.log(element.Context);
                        $('#marquee').append('<li style="color:black;font-size:40px">' + element.Context+'</li>')
                    })
                    marqueeSET();
                }
            });
        }
        function marqueeSET() {
            $("#marquee").marquee({
                 yScroll: "top"              // the position of the marquee initially scroll (can
                                          // be either "top" or "bottom")
                , showSpeed: 1000             // the speed of to animate the initial dropdown of
                                              // the messages
                , scrollSpeed: 12           // the speed of the scrolling (keep number low)
                , pauseSpeed: 1000            // the time to wait before showing the next message
                                              // or scrolling current message
                , pauseOnHover: true          // determine if we should pause on mouse hover
                , loop: -1                    // determine how many times to loop through the
                                              // marquees (#'s < 0 = infinite)
                , fxEasingShow: "swing"       // the animition easing to use when showing a new
                                              // marquee
                , fxEasingScroll: "linear"    // the animition easing to use when showing a new
                                              // marquee
                // define the class statements
                , cssShowing: "marquee-showing"
                // event handlers
                //, init: null                // callback that occurs when a marquee is initialized
                //, beforeshow: null          // callback that occurs before message starts
                //                            // scrolling on screen
                //, show: null                // callback that occurs when a new marquee message is
                //                            // displayed
                //, aftershow: null           // callback that occurs after the message has scrolled
            });
        }
        //window.setInterval("marqueeString()", "3000");
</script>
<style type="text/css">

        #marquee {
            /* required styles */
            display: block;
            padding: 0;
            margin: 0;
            list-style: none;
            line-height: 1;
            position: relative;
            overflow: hidden;

            /* optional styles for appearance */
            width: 100%;
            height: 22px; /* height should be included to reserve visual space for the marquee */
        /*
            background-color: #f2f2ff;
            border: 1px solid #08084d;
        */    
        }

        #marquee li {
            /* required styles */
            position: absolute;
            top: -999em;
            left: 0;
            display: block;
            white-space: nowrap; /* keep all text on a single line */

            /* optional styles for appearance */
            font-family: "微軟正黑體", Arial;
            font-size: 120%;
            font-weight: 600;
        }

    </style>
<div>
    <div style="width: 100%;">
         <table id="table_image" class="navbar-light bg-light" style="height:60px;width:100%">
            <tr style="margin-left:10px">
                <td>
                    <a href="../HomePage.aspx">
                    <img src="styles/images/index/apple-touch-icon.png" /></a>
                </td>
                <td style="text-align:left;width:20%;height:60%">
                </td>
                <td style="width:80%;height:60px;" >
                    <ul id="marquee" style="width:100%;height:60px;">
                        <li style="color:black;font-size:40px">測試跑馬燈001</li>
                        <li style="color:black;font-size:40px">測試跑馬燈002</li>
                        <li style="color:black;font-size:40px">測試跑馬燈003</li>
                        <li style="color:black;font-size:40px">測試跑馬燈004</li>
                    </ul>
                </td>
            </tr>
        </table>
            <div style="background-color: #011023; width: auto;"></div>
    </div>
    <nav id="navbar-example" class="navbar navbar-inverse navbar-static-top" role="navigation">
        <div class="container-fluid">
            <div class="collapse navbar-collapse bs-example-js-navbar-collapse" style="display: inline-block; position:relative; left:13px">
                <div id="the_Menu" runat="server"></div>
                <ul class="nav navbar-nav navbar-right">
                    <li id="fat-menu" class="dropdown">
                        <a id="agent_info" class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" role="button" aria-expanded="false" runat="server"></a>
                        <ul class="dropdown-menu" role="menu" aria-labelledby="agent_info">
                            <li id="user_li" runat="server"></li>
                            <li role="presentation" class="divider"></li>
                            <li>&nbsp;&nbsp;
                                <button id="Btn_Default" type="button" class="btn btn-success" onserverclick="Logout" runat="server">
                                    <span class="glyphicon glyphicon-home"></span>&nbsp;登出</button>
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
            <!-- /.nav-collapse -->
        </div>
        <!-- /.container-fluid -->
    </nav>
    <style type="text/css">
        body {
            font-family: "Microsoft JhengHei",Helvetica,Arial,Verdana,sans-serif;
            font-size: 16px;
        }

        .dropdown:hover .dropdown-menu {
            display: block;
        }
    </style>
</div>
