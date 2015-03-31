<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Meteoroooos</title>

    <style>
        html, body {
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
        }

        h1, h2 {
            margin: 0;
        }

        #map-canvas {
            width: 100%;
            height: 100%;
        }
    </style>

    <script src="/Scripts/jquery-2.1.3.js"></script>

    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyB9Sl9qMkr9ZUWM7EYB-YaRRgiFYXgDkFc"></script>

    <script type="text/javascript">
        var map;

        function initialize() {
            var mapOptions = {
                center: { lat: 0, lng: 0 },
                zoom: 3
            };

            map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
        }

        google.maps.event.addDomListener(window, 'load', initialize);
    </script>

    <script type="text/javascript">
        $(function () {
            var craters = [];
            var meteorites = [];

            $.get("/api/MeteoriteMarket", null, function (data) {
                for (var i = 0; i < data.length; i++) {
                    meteorites.push({
                        Title: data[i].Name,
                        Location: new google.maps.LatLng(parseDms(data[i].Latitude), parseDms(data[i].Longitude)),
                        Url: data[i].Url
                    });
                }

                loadCraters();
            });

            function loadCraters() {
                $.get("/api/EarthImpact", null, function (data) {
                    for (var i = 0; i < data.length; i++) {
                        craters.push({
                            Title: data[i].Name,
                            Location: new google.maps.LatLng(parseDms(data[i].Latitude), parseDms(data[i].Longitude)),
                            Url: null
                        });
                    }

                    initializeMarkers();
                });
            }

            function initializeMarkers() {
                craters.map(function (crater) {
                    addMarker(map, crater.Location, crater.Title, crater.Url, "FF0000")
                });

                meteorites.map(function (meteorite) {
                    addMarker(map, meteorite.Location, meteorite.Title, meteorite.Url, "00FF00")
                });
            }

            var infowindow = new google.maps.InfoWindow();

            function addMarker(map, latlong, title, url, color) {
                var marker = new google.maps.Marker({
                    position: latlong,
                    map: map,
                    title: title,
                    icon: new google.maps.MarkerImage("http://www.googlemapsmarkers.com/v1/" + color + "/")
                });

                var infoContent = '<h2>' + title + '</h2>' +
                                  '<a href="' + url + '" target="_blank">Link</a>';

                google.maps.event.addListener(marker, 'click', (function (marker, content, infowindow) {
                    return function () {
                        infowindow.setContent(content);
                        infowindow.open(map, marker);
                    };
                })(marker, infoContent, infowindow));
            }
        });

        var dmsRe = /^([NSEW]) (\d+)° (\d+)\'/i;

        function parseDms(dmsStr) {
            var output = NaN, dmsMatch, degrees, minutes, seconds, hemisphere;
            dmsMatch = dmsRe.exec(dmsStr);

            if (dmsMatch) {
                degrees = Number(dmsMatch[2]);

                minutes = typeof (dmsMatch[3]) !== "undefined" ? Number(dmsMatch[2]) / 60 : 0;
                seconds = typeof (dmsMatch[4]) !== "undefined" ? Number(dmsMatch[3]) / 3600 : 0;
                console.log(minutes)
                hemisphere = dmsMatch[0] || null;

                if (hemisphere !== null && /[SW]/i.test(hemisphere)) {
                    degrees = Math.abs(degrees) * -1;
                }

                if (degrees < 0) {
                    output = degrees - minutes - seconds;
                } else {
                    output = degrees + minutes + seconds;
                }
            }

            return output;
        }
    </script>

</head>


<body>
    <div id="map-canvas"></div>
</body>
</html>
