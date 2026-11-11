
document.addEventListener("DOMContentLoaded", function () {

    // Sidebar Toggle

    const sidebar = document.getElementById("sidebar");
    const sidebarToggle = document.getElementById("sidebarToggle");

    sidebarToggle?.addEventListener("click", function () {
        sidebar.classList.toggle("show");
    });

    // Sales Chart

    const salesCanvas = document.getElementById("salesChart");

    if (salesCanvas) {
        new Chart(salesCanvas, {
            type: "line",
            data: {
                labels: ["Nov 04", "Nov 05", "Nov 06", "Nov 07", "Nov 08", "Nov 09", "Nov 10"],
                datasets: [{
                    label: "Revenue",
                    data: [12000, 18000, 14500, 24000, 19500, 28000, 32000],
                    borderColor: "#344e75",
                    backgroundColor: "rgba(52, 78, 117, 0.08)",
                    fill: true,
                    tension: 0.35,
                    borderWidth: 2,
                    pointRadius: 3,
                    pointBackgroundColor: "#344e75"
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: "#f0f2f5" },
                        ticks: {
                            font: { size: 10 },
                            callback: value => "EGP " + value / 1000 + "k"
                        }
                    },
                    x: {
                        grid: { display: false },
                        ticks: { font: { size: 10 } }
                    }
                }
            }
        });
    }

    // Orders Chart

    const ordersCanvas = document.getElementById("ordersChart");

    if (ordersCanvas) {
        new Chart(ordersCanvas, {
            type: "doughnut",
            data: {
                labels: ["Delivered", "Processing", "Cancelled"],
                datasets: [{
                    data: [68, 22, 10],
                    backgroundColor: ["#397d6d", "#e0a34d", "#d66d69"],
                    borderWidth: 0,
                    hoverOffset: 5
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: "76%",
                plugins: {
                    legend: { display: false }
                }
            }
        });
    }

});
