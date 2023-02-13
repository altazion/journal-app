namespace Journal {

    export class MainMenuBar extends HTMLElement {
        connectedCallback() {
            var self = this;
            this.innerHTML = `<header id="manoirHeaderBar" class="manoir-main-header">
                <div class="header-bar">
                    <div class='breadCrumb'>
                        <ul>
                            <li><span class='icon icon-home'></span></li>
                        </ul>
                    </div>

                    <button class='journal-button'><span class='icon icon-date'></span></button>
                    <button class='login-button'></span></button>
                </div>
                <nav>
                    <ul>
                        <li><span class='icon icon-home'></span><span>Home</span></li>
                    </ul>
                    <button>Switch to...</button>
                </nav>

                </header>`;
            //$("#manoirMainMenu .manoir-main-menu-close").click(function (e) {
            //    e.preventDefault();
            //    $("body").removeClass("menu-opened");
            //    return false;
            //});
            //setTimeout(function () { self.refreshData(); }, 100);
        }
    }
}

customElements.define('journal-menubar', Journal.MainMenuBar);
