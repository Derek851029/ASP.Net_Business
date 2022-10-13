<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Position.aspx.cs" Inherits="Position" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script src="js/jquery.js"></script>
    <script src="js/jquery.min.js"></script>
    <link rel="stylesheet" href="css/style.css">
	<style>
	
	</style>
    <title></title>
</head>
<body>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(function () {
            test();
        });

        function test() {
            navigator.permissions.query({ name: 'geolocation' })
                .then(function (permissionStatus) {
                    console.log('geolocation permission state is ', permissionStatus.state);

                    permissionStatus.onchange = function () {
                        console.log('geolocation permission state has changed to ', this.state);
                    };
                });
        }

        function makeRequest2() {
            xhr = new XMLHttpRequest();
            xhr.open('POST', 'https://www.googleapis.com/geolocation/v1/geolocate?key=AIzaSyAMPWTpAHQni_s7ivZB-OGuH2nRbA96GlU');
            xhr.onload = function () {
                // do something
                var response = JSON.parse(this.responseText);
                console.log(response)
            }
            xhr.send();
        }

        function makeRequest() {
            let lat = '';
            let lng = ''

            navigator.geolocation.watchPosition((position) => {
                console.log(position.coords);
                lat = position.coords.latitude;
                lng = position.coords.longitude;
                console.log(lat)
                console.log(lng)
            });
        }

    </script>
        <div>
            <p><button onclick="makeRequest()">Show my location</button></p>
            <p><button onclick="makeRequest2()">Show my location2</button></p>
        </div>
</body>
</html>
