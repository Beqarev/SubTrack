// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    var storageKey = "subtrack-theme";
    var allowedThemes = ["midnight", "paper"];

    function isAllowedTheme(theme) {
        return allowedThemes.indexOf(theme) !== -1;
    }

    function setTheme(theme, persist) {
        if (!isAllowedTheme(theme)) {
            theme = "midnight";
        }

        document.documentElement.dataset.theme = theme;

        if (persist) {
            try {
                localStorage.setItem(storageKey, theme);
            } catch (_) {
                // Ignore storage failures; the live theme change still applies.
            }
        }

        document.querySelectorAll("[data-theme-option]").forEach(function (button) {
            var isActive = button.dataset.themeOption === theme;
            button.classList.toggle("active", isActive);
            button.setAttribute("aria-pressed", isActive ? "true" : "false");

            var status = button.querySelector("[data-theme-status]");
            if (status) {
                status.textContent = isActive ? "Active" : "Select";
            }
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        var selectedTheme = document.documentElement.dataset.theme || "midnight";
        setTheme(selectedTheme, false);

        document.querySelectorAll("[data-theme-option]").forEach(function (button) {
            button.addEventListener("click", function () {
                setTheme(button.dataset.themeOption, true);
            });
        });
    });
})();
