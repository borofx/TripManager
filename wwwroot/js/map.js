document.addEventListener("DOMContentLoaded", function () {
    var map = L.map('map').setView([42.6977, 23.3242], 6); // Default center: Sofia, Bulgaria

    // Load OpenStreetMap tiles
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    function loadLandmarks() {
        fetch('/api/landmarks')
            .then(response => response.json())
            .then(landmarks => {
                landmarks.forEach(landmark => {
                    L.marker([landmark.latitude, landmark.longitude])
                        .addTo(map)
                        .bindPopup(`<b>${landmark.name}</b>`);
                });
            })
            .catch(error => console.error("Error loading landmarks:", error));
    }

    loadLandmarks(); // Load landmarks when the page loads

    // Refresh map every 10 seconds to check for new landmarks
    setInterval(loadLandmarks, 10000);
});
