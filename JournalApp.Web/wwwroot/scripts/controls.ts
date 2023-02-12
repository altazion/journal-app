namespace Journal {

    export class MainMenuBar extends HTMLElement {
        connectedCallback() {
            var self = this;
            this.innerHTML = '<section id="manoirMainMenu" class="manoir-main-menu-content"><button class="manoir-main-menu-close"><img src="/assets/imgs/streamlinehq-close-interface-essential.svg" /></button><ul></ul></section>';
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
