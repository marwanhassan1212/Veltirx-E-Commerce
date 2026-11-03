
/* =========================================================
   VELTRIX — GLOBAL INTERACTIONS
========================================================= */

document.addEventListener("DOMContentLoaded", () => {

    const sidebar = document.getElementById("appSidebar");
    const sidebarToggle = document.getElementById("sidebarToggle");
    const sidebarClose = document.getElementById("sidebarClose");
    const sidebarOverlay = document.getElementById("sidebarOverlay");

    if (!sidebar || !sidebarToggle || !sidebarOverlay) {
        return;
    }

    // Open Sidebar
    function openSidebar() {
        sidebar.classList.add("show");
        sidebarOverlay.classList.add("show");

        document.body.style.overflow = "hidden";
        sidebarToggle.setAttribute("aria-expanded", "true");
    }

    // Close Sidebar
    function closeSidebar() {
        sidebar.classList.remove("show");
        sidebarOverlay.classList.remove("show");

        document.body.style.overflow = "";
        sidebarToggle.setAttribute("aria-expanded", "false");
    }

    // Toggle Sidebar
    sidebarToggle.addEventListener("click", () => {
        const isOpen = sidebar.classList.contains("show");

        if (isOpen) {
            closeSidebar();
        } else {
            openSidebar();
        }
    });

    // Close Button
    if (sidebarClose) {
        sidebarClose.addEventListener("click", closeSidebar);
    }

    // Overlay Click
    sidebarOverlay.addEventListener("click", closeSidebar);

    // Escape Key
    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            closeSidebar();
        }
    });

    // Close Sidebar After Navigation on Mobile
    sidebar.querySelectorAll(".sidebar-link").forEach(link => {
        link.addEventListener("click", () => {
            if (window.innerWidth < 992) {
                closeSidebar();
            }
        });
    });

    // Reset Sidebar State When Returning to Desktop
    window.addEventListener("resize", () => {
        if (window.innerWidth >= 992) {
            closeSidebar();
        }
    });

});
