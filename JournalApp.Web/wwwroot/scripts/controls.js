var Journal;
(function (Journal) {
    class MainMenuBar extends HTMLElement {
        connectedCallback() {
            var self = this;
            this.innerHTML = `<header id="manoirHeaderBar" class="manoir-main-header">
                <div class="header-bar">
                    <div class='breadCrumb'>
                        <ul>
                            <li><button class='nav-button' id="btnSwitchNav"><span class='icon icon-home'></span></button></li>
                        </ul>
                    </div>

                    <button class='journal-button'><span class='icon icon-date'></span></button>
                    <button class='login-button' id="btnLogin"></span></button>
                </div>
                <nav id="navBar" class='public'>
                    <ul id='navPublic'>
                        <li><span class='icon icon-home'></span><span>Home</span></li>
                    </ul>
                    <ul id='navUser'>
                        <li><span class='icon icon-home'></span><span>Home</span></li>
                    </ul>
                    <button id='btnSwitchPublic' class='button-switch-page-type'>Switch to...</button>
                </nav>

                </header>`;
            //$("#manoirMainMenu .manoir-main-menu-close").click(function (e) {
            //    e.preventDefault();
            //    $("body").removeClass("menu-opened");
            //    return false;
            //});
            setTimeout(function () { self.refreshData(); }, 100);
            setTimeout(function () { self.setupMenu(); }, 50);
        }
        refreshNav(isPublic) {
            var btnSwitch = document.getElementById("btnSwitchPublic");
            var nav = document.getElementById("navBar");
            if (isPublic) {
                nav.classList.remove("public");
                nav.classList.add("user");
                btnSwitch.innerText = "Switch to public pages";
            }
            else {
                nav.classList.add("public");
                nav.classList.remove("user");
                btnSwitch.innerText = "Switch to user pages";
            }
        }
        setupMenu() {
            var self = this;
            var btnSwitch = document.getElementById("btnSwitchPublic");
            btnSwitch.addEventListener("click", (evt) => {
                var nav = document.getElementById("navBar");
                if (nav.classList.contains("public")) {
                    self.refreshNav(true);
                }
                else {
                    self.refreshNav(false);
                }
            });
            var btnNav = document.getElementById("btnSwitchNav");
            btnNav.addEventListener("click", (evt) => {
                var nav = document.getElementById("navBar");
                nav.classList.toggle("opened");
                return true;
            });
            var btnLogin = document.getElementById("btnLogin");
            btnLogin.addEventListener("click", (evt) => {
                self.login();
                return true;
            });
        }
        login() {
            fetch('/api/users/login?username=mcarbenay&pincode=123456')
                .then(function (response) { return response.json(); })
                .then(function (data) {
                window.location.reload();
            });
        }
        refreshData() {
            var self = this;
            var navPublic = document.getElementById("navPublic");
            var navUser = document.getElementById("navUser");
            // on commence par binder avec les pages stockées
            // en localstorage si on en a
            if (window.localStorage != null) {
                var tmp = window.localStorage.getItem("pagesPublic");
                if (tmp != null && tmp != "") {
                    var pagesPublic = JSON.parse(tmp);
                    self.BindPages(navPublic, pagesPublic);
                }
            }
            fetch('/api/users/pages')
                .then(function (response) { return response.json(); })
                .then(function (pages) {
                if (pages.publicPages != null)
                    window.localStorage.setItem("pagesPublic", JSON.stringify(pages.publicPages));
                else
                    window.localStorage.removeItem("pagesPublic");
                self.BindPages(navPublic, pages.publicPages);
                if (pages.userPages != null)
                    self.BindPages(navUser, pages.userPages);
            });
            // si on est dans /user/ ce sont des pages utilisateurs
            // on active la partie user.
            var estUserPath = false;
            if (document.location.pathname.startsWith("/user/"))
                estUserPath = true;
            if (estUserPath) {
                self.refreshNav(true);
            }
            else {
                self.refreshNav(false);
            }
        }
        BindPages(navUl, pages) {
            var content = this.GetPageNavContent(pages);
            navUl.innerHTML = content;
        }
        GetPageNavContent(pages) {
            var content = "";
            for (var i = 0; i < pages.length; i++) {
                var pg = pages[i];
                content += "<li class='page icon icon-";
                if (pg.page.pageIcon != null)
                    content += pg.page.pageIcon;
                else
                    content += "document";
                content += "'><a href='";
                content += pg.page.path;
                content += "'>";
                content += pg.page.title;
                content += "</a>";
                if (pg.subPages != null && pg.subPages.length > 0) {
                    content += "<ul class='sub-pages'>";
                    content += this.GetPageNavContent(pg.subPages);
                    content += "</ul>";
                }
            }
            return content;
        }
    }
    Journal.MainMenuBar = MainMenuBar;
})(Journal || (Journal = {}));
customElements.define('journal-menubar', Journal.MainMenuBar);
//# sourceMappingURL=controls.js.map