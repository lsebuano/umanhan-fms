
//document.addEventListener("fullscreenchange", function () {
//    let isFullScreen = document.fullscreenElement !== null;
//    console.log({ isFullScreen });
//    farmMap.isFullScreen = isFullScreen;
//});

window.farmMap = {
    map: null,
    mapSnapshot: null,
    drawingManager: null,
    farmBoundary: null,
    zones: {},
    zoneLabels: {},
    searchBox: null,
    dotNetHelper: null,
    markers: {},
    isFullScreen: false,
    isEditMode: false,
    map_id: '5abbee8842287810',
    farmStaticMapUrl: null,
    apiKey: null,

    initialize: (dnHelper, lat, lang, mode) => {
        this.isEditMode = mode;
        this.dotNetHelper = dnHelper;
        farmMap.isEditMode = mode;
        farmMap.dotNetHelper = dnHelper;

        let mapOptions = {
            center: { lat: lat, lng: lang }, // Default center
            zoom: 12,
            mapId: farmMap.map_id
        };
        let mapElement = document.getElementById("map");
        if (!mapElement) {
            console.error("Map element not found. Retrying in 500ms...");
            setTimeout(farmMap.initialize, 500);
            return;
        }
        farmMap.map = new google.maps.Map(mapElement, mapOptions);

        farmMap.drawingManager = new google.maps.drawing.DrawingManager({
            drawingMode: null,  // disable drawing of polygons by default
            drawingControl: false,
            drawingControlOptions: {
                position: google.maps.ControlPosition.TOP_CENTER,
                drawingModes: ['polygon']
            },
            polygonOptions: {
                editable: true,
                draggable: false
            }
        });

        farmMap.drawingManager.setMap(farmMap.map);

        // Capture farm boundary or zones
        google.maps.event.addListener(farmMap.drawingManager, 'overlaycomplete', async (event) => {
            if (!farmMap.isEditMode) {
                event.overlay.setMap(null);
                console.log('Edit mode is disabled. Enable edit mode to draw the zones.');
                return;
            }

            if (event.type !== google.maps.drawing.OverlayType.POLYGON) {
                console.error("Overlay is not a polygon:", event.overlay);
                return;
            }
            if (!farmMap.farmBoundary) {
                let farmPolygon = event.overlay;
                let farmCentroid = farmMap.getPolygonCentroid(farmPolygon);
                let farmSize = farmMap.getPolygonArea(farmPolygon);
                let location = await farmMap.getAddressFromLatLng(farmCentroid.lat, farmCentroid.lng);

                farmMap.farmBoundary = farmPolygon;
                farmMap.attachPolygonEditListeners(farmPolygon, farmCentroid, location, farmSize);

                //const url = farmMap.generateFarmStaticMapUrl();
                farmMap.invokeCSharp('OnFarmBoundaryDrawn', farmMap.getPolygonCoordinates(event.overlay), farmCentroid, location, farmSize, '');
                //dnHelper.invokeMethodAsync('OnFarmBoundaryDrawn', farmMap.getPolygonCoordinates(event.overlay), farmCentroid, location, farmSize, url);
            }
            else {
                // zones go here
                let zonePolygon = event.overlay;

                //farmMap.enforcePolygonClosure(zonePolygon);

                let isClosed = farmMap.isPolygonClosed(zonePolygon);
                if (!isClosed) {
                    // remove
                    zonePolygon.setMap(null);
                    return;
                }

                let zoneId = crypto.randomUUID();
                let zoneName = "Zone " + (Object.keys(farmMap.zones).length + 1);

                // clear old markers first
                //farmMap.clearInvalidMarkers(zoneId);

                //if (!farmMap.isZoneInFarmBoundary(zonePolygon)) {
                //    //alert('Zone must be inside the farm boundary.');
                //    //zonePolygon.setMap(null);
                //    farmMap.highlightInvalidPoints(zonePolygon, zoneId);
                //    zonePolygon.setEditable(true);
                //    return;
                //}
                //else {
                //    console.log('Zone successfully added.');
                //}

                let area = farmMap.getPolygonArea(zonePolygon);

                // Assign color dynamically
                let color = farmMap.getRandomColor();
                farmMap.stylePolygon(zonePolygon, color);

                // remove existing first
                if (farmMap.zones[zoneId]) {
                    farmMap.zones[zoneId].setMap(null);
                }

                farmMap.zones[zoneId] = zonePolygon;
                let centroid = farmMap.getPolygonCentroid(zonePolygon);
                farmMap.addPolygonClickEventListener(zonePolygon, zoneId, zoneName, centroid.lat, centroid.lng, color, area);

                farmMap.attachPolygonEditListenersZone(zonePolygon, centroid, zoneId, area);

                farmMap.invokeCSharp('OnZoneDrawn', zoneId, farmMap.getPolygonCoordinates(zonePolygon), centroid, area, farmMap.farmStaticMapUrl);
                //dnHelper.invokeMethodAsync('OnZoneDrawn', zoneId, farmMap.getPolygonCoordinates(zonePolygon), centroid, area, farmMap.farmStaticMapUrl);

                // Add label to the zone
                farmMap.addZoneLabel(zoneId, centroid.lat, centroid.lng, zoneName, area);
            }
        });

        setTimeout(() => {
            farmMap.loadFarmData();
        }, 500);

        // Initialize map search
        setTimeout(() => {
            farmMap.initSearchBox();
        }, 500);
    },

    invokeCSharp: (methodName, ...args) => {
        if (farmMap.dotNetHelper) {
            return farmMap.dotNetHelper.invokeMethodAsync(methodName, ...args);
        }
        else {
            console.error('dotNetHelper is not initialized yet.');
        }
    },

    loadFarmData: async () => {
        try {
            //const data = await farmMap.invokeCSharp("LoadFarmZoneMapData");
            const data = await farmMap.invokeCSharp("LoadFarmZoneMapData");
            console.log('Farm data: ', data);

            if (data) {
                if (data.farmBoundary)
                    farmMap.drawFarmBoundary(data.farmBoundary);
                if (data.farmZones)
                    farmMap.drawZones(data.farmZones);

                farmMap.zoomInToArea(data.farmBoundary);
            }
        }
        catch (error) {
            console.error("Error loading farm data: ", error);
        }
    },

    addPolygonClickEventListener: (zonePolygon, zoneId, zoneName, lat, lng, color, area) => {
        zonePolygon.addListener("click", () => {
            console.log('zone clicked');
            //if (!farmMap.isFullScreen)
            farmMap.invokeCSharp('OnZoneClicked', zoneId, zoneName, lat, lng, color, area);
            //farmMap.dotNetHelper.invokeMethodAsync('OnZoneClicked', zoneId, zoneName, lat, lng, color, area);
        });
    },

    getPolygonCoordinates: (polygon) => {
        let path = polygon.getPath();
        let coordinates = [];
        for (let i = 0; i < path.getLength(); i++) {
            let point = path.getAt(i);
            coordinates.push({ lat: point.lat(), lng: point.lng() });
        }
        return coordinates;
    },

    // Function to apply color to a polygon
    stylePolygon: (polygon, color) => {
        polygon.setOptions({
            strokeColor: color,
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: color,
            fillOpacity: 0.35
        });
    },

    // Generate random color
    getRandomColor: () => {
        let letters = "0123456789ABCDEF";
        let color = "#";
        for (let i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    },

    // Calculate the centroid of a polygon
    getPolygonCentroid: (polygon) => {
        let path = polygon.getPath();
        let latSum = 0, lngSum = 0, len = path.getLength();

        for (let i = 0; i < len; i++) {
            let point = path.getAt(i);
            latSum += point.lat();
            lngSum += point.lng();
        }

        return { lat: latSum / len, lng: lngSum / len };
    },

    getPolygonCentroidByCoords: (coordinates) => {
        if (!coordinates || coordinates.length === 0) {
            console.error("Invalid coordinates:", coordinates);
            return null;
        }

        let centroid = { lat: 0, lng: 0 };
        let signedArea = 0;
        let x0, y0, x1, y1, a;

        for (let i = 0; i < coordinates.length; i++) {
            let nextIndex = (i + 1) % coordinates.length;
            x0 = coordinates[i].lat();
            y0 = coordinates[i].lng();
            x1 = coordinates[nextIndex].lat();
            y1 = coordinates[nextIndex].lng();

            a = (x0 * y1) - (x1 * y0);
            signedArea += a;
            centroid.lat += (x0 + x1) * a;
            centroid.lng += (y0 + y1) * a;
        }

        signedArea *= 0.5;
        centroid.lat /= (6 * signedArea);
        centroid.lng /= (6 * signedArea);

        console.log(JSON.stringify({ centroid_lat: centroid.lat, centroid_lng: centroid.lng }));
        return new google.maps.LatLng(centroid.lat, centroid.lng);
    },

    // Add a label to the zone
    addZoneLabel: (zoneId, lat, lng, labelText) => {
        if (!farmMap.map) return;

        // remove existing first
        if (farmMap.zoneLabels[zoneId]) {
            farmMap.zoneLabels[zoneId].setMap(null);
        }

        const marker = new google.maps.Marker({
            position: { lat: lat, lng: lng },
            map: farmMap.map,
            label: {
                text: labelText,
                fontSize: "14px",
                fontWeight: "bold",
                color: "#000000"
            }
        });

        // Store marker reference
        farmMap.zoneLabels[zoneId] = marker;
    },

    // Function to get user's location
    showUserLocation: () => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (position) => {
                    const userLocation = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };

                    // Add marker for user location
                    new google.maps.Marker({
                        position: userLocation,
                        map: farmMap.map,
                        title: "Your Location",
                        icon: {
                            url: "https://maps.google.com/mapfiles/ms/icons/blue-dot.png", // Custom blue icon
                            scaledSize: new google.maps.Size(40, 40)
                        }
                    });

                    // Center map on user's location
                    farmMap.map.setCenter(userLocation);
                    farmMap.map.setZoom(15);
                },
                () => {
                    alert("Geolocation permission denied. Unable to fetch location.");
                }
            );
        } else {
            alert("Geolocation is not supported by this browser.");
        }
    },

    initSearchBox: () => {
        let input = document.getElementById("searchBox");
        let searchBox = new google.maps.places.SearchBox(input);

        farmMap.map.addListener("bounds_changed", () => {
            searchBox.setBounds(farmMap.map.getBounds());
        });

        searchBox.addListener("places_changed", () => {
            let places = searchBox.getPlaces();
            if (places.length === 0) return;

            let bounds = new google.maps.LatLngBounds();
            places.forEach((place) => {
                if (!place.geometry) return;
                // set map center
                farmMap.map.setCenter(place.geometry.location);
                farmMap.map.setZoom(15);
                bounds.extend(place.geometry.location);
            });
            //farmMap.map.fitBounds(bounds); // adjust view
        });
    },

    zoomInToArea: (boundary) => {
        if (!boundary || boundary.length == 0) {
            console.error('Invalid coordinates.', boundary);
            return;
        }
        let coordinates = boundary.map(c => new google.maps.LatLng(c.lat, c.lng));
        console.log(JSON.stringify(coordinates));
        let center = farmMap.getPolygonCentroidByCoords(coordinates);
        farmMap.map.setCenter(center);
        farmMap.fitPolygonToMap(coordinates);
    },

    drawPolygon: (polygonData) => {
        let polygon = new google.maps.Polygon({
            paths: polygonData.coordinates.map(c => ({ lat: c.lat, lng: c.lng })),
            strokeColor: polygonData.color || '#FF0000',
            fillColor: polygonData.color || '#FF0000',
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillOpacity: 0.35,
            map: farmMap.map
        });
        farmMap.polygons.push(polygon);
    },

    updateZoneDetails: (zoneId, name, desc, color, lat, lng, area) => {
        let zone = farmMap.zones[zoneId];
        if (!zone) {
            console.error("Zone not found:", zoneId);
            return;
        }

        // Update polygon style
        zone.setOptions({
            strokeColor: color,
            fillColor: color
        });
        farmMap.zones[zoneId] = zone;

        // Update zone label
        if (farmMap.zoneLabels[zoneId]) {
            farmMap.zoneLabels[zoneId].setLabel({
                text: name,
                fontSize: "14px",
                fontWeight: "bold",
                color: "#000000"
            });
        }
        google.maps.event.clearListeners(zone, 'click'); // Remove old listener
        farmMap.addPolygonClickEventListener(zone, zoneId, name, lat, lng, color, area);
    },

    drawPolygons: (polygonsData) => {
        polygonsData.forEach(zone => {
            drawPolygon(zone);
        });
    },

    fitPolygonToMap: (coordinates) => {
        let bounds = new google.maps.LatLngBounds();
        coordinates.forEach(coord => bounds.extend(coord));
        farmMap.map.fitBounds(bounds); // Adjust zoom and ensure the polygon is visible
    },

    attachPolygonEditListeners: (polygon, farmCentroid, location, farmSize) => {
        let path = polygon.getPath();

        // Update coordinates when a vertex is moved
        google.maps.event.addListener(path, "set_at", function () {
            farmMap.updatePolygonCoordinates(polygon, farmCentroid, location, farmSize);
        });

        // Update coordinates when a new vertex is inserted
        google.maps.event.addListener(path, "insert_at", function () {
            farmMap.updatePolygonCoordinates(polygon, farmCentroid, location, farmSize);
        });
    },

    updatePolygonCoordinates: (polygon, farmCentroid, location, farmSize) => {
        const updatedCoordinates = farmMap.getPolygonCoordinates(polygon);
        const updatedCentroid = farmMap.getPolygonCentroid(polygon);
        const updatedArea = farmMap.getPolygonArea(polygon);

        console.log("Updated Coordinates for the farm boundary.", updatedCoordinates);

        //const url = farmMap.generateFarmStaticMapUrl(farmMap.farmBoundary);
        // Send updated coordinates to Blazor
        //farmMap.invokeCSharp('OnFarmBoundaryDrawn', updatedCoordinates, farmCentroid, location, farmSize, '');
        //farmMap.dotNetHelper.invokeMethodAsync('OnFarmBoundaryDrawn', updatedCoordinates, farmCentroid, location, farmSize, url);

        farmMap.getAddressFromLatLng(updatedCentroid.lat, updatedCentroid.lng)
            .then(address => {
                farmMap.invokeCSharp(
                    'OnFarmBoundaryDrawn',
                    updatedCoordinates,
                    updatedCentroid,
                    address,
                    updatedArea,
                    ''
                );
            })
            .catch(error => {
                console.error("Failed to get address for updated centroid: ", error);
                farmMap.invokeCSharp(
                    'OnFarmBoundaryDrawn',
                    updatedCoordinates,
                    updatedCentroid,
                    "",
                    updatedArea,
                    ''
                );
            });

        console.log("Updated farm area (sqm):", updatedArea);
    },

    attachPolygonEditListenersZone: (polygon, zoneCentroid, zoneId, area) => {
        let path = polygon.getPath();
        let validate = () => farmMap.updatePolygonCoordinatesZone(polygon, zoneCentroid, zoneId, area);

        // Update coordinates when a vertex is moved
        google.maps.event.addListener(path, "set_at", validate);

        // Update coordinates when a new vertex is inserted
        google.maps.event.addListener(path, "insert_at", validate);
    },

    updatePolygonCoordinatesZone: (polygon, zoneCentroid, zoneId, area) => {
        //farmMap.validateZone(polygon, zoneId);

        let updatedCoordinates = farmMap.getPolygonCoordinates(polygon);

        console.log("Updated Coordinates for " + zoneId, updatedCoordinates);

        // Send updated coordinates to Blazor
        farmMap.invokeCSharp('OnZoneDrawn', zoneId, updatedCoordinates, zoneCentroid, area, farmMap.farmStaticMapUrl);
        //farmMap.dotNetHelper.invokeMethodAsync('OnZoneDrawn', zoneId, updatedCoordinates, zoneCentroid, area, farmMap.farmStaticMapUrl);
    },

    getPolygonArea: (polygon) => {
        let area = google.maps.geometry.spherical.computeArea(polygon.getPath());
        return area; // Returns the area in square meters
    },

    isZoneInFarmBoundary: (polygon) => {
        if (!farmMap.farmBoundary) {
            console.warn("Farm boundary is not set.");
            return false;
        }

        let allInside = true;
        let zone = polygon.getPath();

        for (let i = 0; i < zone.getLength(); i++) {
            let point = zone.getAt(i);
            if (!google.maps.geometry.poly.containsLocation(point, farmMap.farmBoundary)) {
                allInside = false;
                break;
            }
        }
        return allInside;
    },

    highlightInvalidPoints: (polygon, zoneId) => {
        let zone = polygon.getPath();
        let x = [];
        for (let i = 0; i < zone.getLength(); i++) {
            let point = zone.getAt(i);
            if (!google.maps.geometry.poly.containsLocation(point, farmMap.farmBoundary)) {
                // Add a temporary red marker at the invalid point
                let marker = new google.maps.Marker({
                    position: point,
                    map: farmMap.map,
                    icon: {
                        path: google.maps.SymbolPath.CIRCLE,
                        scale: 10,
                        fillColor: "red",
                        fillOpacity: 1,
                        strokeWeight: 2
                    }
                });
                x.push(marker);
            }
        }
        farmMap.markers[zoneId] = x;
    },

    clearInvalidMarkers: (zoneId) => {
        if (Object.keys(farmMap.markers).length == 0)
            return;

        let x = farmMap.markers[zoneId];
        for (let m of x) {
            m.setMap(null);
        }
        farmMap.markers[zoneId] = [];
    },

    validateZone: (polygon, zoneId) => {
        //let isClosed = farmMap.isPolygonClosed(polygon);
        //if (!isClosed)
        //    // remove
        //    polygon.setMap(null);

        // clear old markers
        farmMap.clearInvalidMarkers();

        if (!farmMap.isZoneInFarmBoundary(polygon)) {
            console.warn('Some points are outside the farm boundary. Highlighting them...');
            farmMap.highlightInvalidPoints(polygon, zoneId);
        }
        else {
            console.log('Zone is valid.');
        }
    },

    validateAllZones: () => {
        for (let key in farmMap.zones) {
            console.log({ key });
            farmMap.validateZone(farmMap.zones[key]);
        }
    },

    isPolygonClosed: (polygon) => {
        let path = polygon.getPath();
        if (path.getLength() < 3) {
            return false; // Not a valid polygon
        }

        //let firstPoint = path.getAt(0);
        //let lastPoint = path.getAt(path.getLength() - 1);

        //console.log({ firstPoint });
        //console.log({ lastPoint });

        return true;
        //    // Compute the distance between first and last point
        //    let distance = google.maps.geometry.spherical.computeDistanceBetween(firstPoint, lastPoint);

        //    return distance < 0.1; // Consider it closed if distance is near zero
    },

    enforcePolygonClosure: (polygon) => {
        let path = polygon.getPath();

        function closePolygon() {
            if (path.getLength() > 2) {
                let firstPoint = path.getAt(0);
                let lastPoint = path.getAt(path.getLength() - 1);

                if (!firstPoint.equals(lastPoint)) {
                    path.push(firstPoint); // Ensure closure
                }
            }
        }

        google.maps.event.addListener(path, "insert_at", closePolygon);
        google.maps.event.addListener(path, "set_at", closePolygon);
    },

    getAddressFromLatLng: (lat, lng) => {
        return new Promise((resolve, reject) => {
            let geocoder = new google.maps.Geocoder();
            let latlng = new google.maps.LatLng(lat, lng);

            geocoder.geocode({ location: latlng }, function (results, status) {
                if (status === "OK") {
                    if (results[0]) {
                        console.log("Exact Address: ", results[0].formatted_address);
                        resolve(results[0].formatted_address);
                    } else {
                        reject("No address found");
                    }
                } else {
                    reject("Geocoder failed due to: " + status);
                }
            });
        });
    },

    enableEditMode: (enabled) => {
        farmMap.isEditMode = enabled;

        if (enabled) {
            farmMap.drawingManager.setOptions({ drawingMode: google.maps.drawing.OverlayType.POLYGON, drawingControl: true });
        }
        else {
            farmMap.drawingManager.setOptions({ drawingMode: null, drawingControl: false });
        }

        if (farmMap.farmBoundary) {
            farmMap.farmBoundary.setOptions({
                editable: enabled
                //draggable: enabled
            });
        }

        Object.values(farmMap.zones).forEach(polygon => {
            polygon.setOptions({
                editable: enabled,  // Show/hide points
                //draggable: enabled
            });
        });

        console.log('Edit Mode: ', enabled);
    },

    clearAllPolygons: () => {
        if (!farmMap.isEditMode) {
            console.log("Can only clear all polygons in edit mode.");
            return;
        }

        Object.values(farmMap.zones).forEach(polygon => polygon.setMap(null));
        Object.values(farmMap.zoneLabels).forEach(polygon => polygon.setMap(null));
        farmMap.zones = {};
        farmMap.zoneLabels = {};
        farmMap.invokeCSharp("OnZonesCleared");

        let msg = 'All zones cleared.';
        farmMap.invokeCSharp("ShowNotification", "success", msg);
        console.log(msg);
    },

    distanceInPx: (pos1, pos2) => {
        var p1 = farmMap.map.getProjection().fromLatLngToPoint(pos1);
        var p2 = farmMap.map.getProjection().fromLatLngToPoint(pos2);
        var pixelSize = Math.pow(2, -farmMap.map.getZoom());
        var d = Math.sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y)) / pixelSize;
        return Math.round(d);
    },
    drawFarmBoundary: (boundaryCoordinates) => {
        if (!boundaryCoordinates || boundaryCoordinates.length === 0) return;

        if (farmMap.farmBoundary) {
            farmMap.farmBoundary.setMap(null); // Remove old boundary
        }

        let bounds = new google.maps.LatLngBounds();
        let path = boundaryCoordinates.map(coord => {
            let latLng = new google.maps.LatLng(coord.lat, coord.lng);
            bounds.extend(latLng); // Expand bounds to include each point
            return latLng;
        });

        farmMap.farmBoundary = new google.maps.Polygon({
            paths: path,
            strokeColor: "#FF0000",
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: "#FF0000",
            fillOpacity: 0.35,
            editable: false,  // Users can't modify the boundary
            map: farmMap.map
        });

        //let farmPolygon = event.overlay;
        //let farmCentroid = farmMap.getPolygonCentroid(farmPolygon);

        ////farmMap.farmBoundary = farmPolygon;
        //farmMap.attachPolygonEditListeners(farmPolygon, farmCentroid, location, farmSize);

        //return farmMap.generateFarmStaticMapUrl();
    },

    drawZones: (zones) => {
        Object.keys(zones).forEach(i => {
            let zone = zones[i];
            let zoneId = zone.zoneId;

            let rawZonePolygon = JSON.parse(zone.zoneBoundaryJson);
            if (!rawZonePolygon || rawZonePolygon.length < 3) return;

            // Convert property names to lowercase for Google Maps API
            let zonePolygon = rawZonePolygon.map(coord => ({
                lat: coord.Lat,
                lng: coord.Lng
            }));

            let bounds = new google.maps.LatLngBounds();
            zonePolygon.forEach(latLng => bounds.extend(latLng));

            let polygon = new google.maps.Polygon({
                paths: zonePolygon,
                strokeColor: zone.zoneColor || "#0000FF",
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: zone.zoneColor || "#0000FF",
                fillOpacity: 0.35,
                editable: false, // Initially not editable
                draggable: false,
                map: farmMap.map
            });

            farmMap.zones[zoneId] = polygon;
            // reattach click listener
            farmMap.addPolygonClickEventListener(polygon, zoneId, zone.zoneName, zone.zoneCentroidLat, zone.zoneCentroidLng, zone.zoneColor, zone.areaInSqm);

            let position = bounds.getCenter();
            let labelText = zone.zoneName || `Zone ${zoneId}`;

            const marker = new google.maps.Marker({
                position: position,
                map: farmMap.map,
                label: {
                    text: labelText,
                    fontSize: "14px",
                    fontWeight: "bold",
                    color: "#000000"
                }
            });
            farmMap.zoneLabels[zoneId] = marker;
        });
    },
    generateFarmStaticMapUrl: (apiKey, boundary) => {
        console.log({ boundary });
        const polygon = new google.maps.Polygon({
            paths: boundary, // Accepts array of LatLng literals
            strokeColor: "#FF0000",
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: "#FF0000",
            fillOpacity: 0.35
        });
        const farmCentroid = farmMap.getPolygonCentroid(polygon);
        //farmMap.map.fitBounds(bounds);

        //const cor1 = bounds.getNorthEast();
        //const cor2 = bounds.getSouthWest();
        //const cor3 = new google.maps.LatLng(cor2.lat(), cor1.lng());
        //const cor4 = new google.maps.LatLng(cor1.lat(), cor2.lng());

        const width = 750;  //farmMap.distanceInPx(cor1, cor4);
        const height = 315; //farmMap.distanceInPx(cor1, cor3);
        const zoom = 15; // farmMap.map.zoom;

        const imgUrl = "https://maps.googleapis.com/maps/api/staticmap?center=" + farmCentroid.lat + "," + farmCentroid.lng + "&zoom=" + zoom +
            "&size=" + width + "x" + height + "&map_id=" + farmMap.map_id + "&key=" + apiKey;

        console.log({ imgUrl });

        farmMap.farmStaticMapUrl = imgUrl;
        return imgUrl;
    },
    setApiKey: (key) => {
        farmMap.apiKey = key;
    }
};