var editForm = {
    view: "form",
    id: "editForm",
    borderless: true,
    elements: [],
    rules: {},
    elementsConfig: {
        labelPosition: "top",
    }
};

var formButtons = {
    cols: [{ view: "button", type: "form", value: "Сохранить", id: "saveButton", height: 35 }, { view: "button", value: "Отмена", id: "cancelButton", height: 35 }]
}

var modalEditor = {
    id: "modalEditor",
    view: "window",
    width: 300,
    headHeight: 52,
    position: "center",
    modal: true,
    body: webix.copy(editForm)
};

var searchCtrl = { view: "search", id: 'search', labelWidth: 100 };
var addConferenceBtn = { view: "button", id: 'addConference', labelWidth: 110, value: "Добавить конференцию", tooltup: "Выберите секцию", width: 180 };
var addSectionBtn = { view: "button", id: 'addSection', gravity: 0.9, minWidth:135, value: "Добавить секцию" };
var cityCtrl = { view: "combo", id: 'cityFilter', labelWidth: 100, multiselect: false, gravity:0.4 };
var sectionList = { id: "sectionList", view: "list", select: true, scroll: "y", template: "#value#", multiselect: false };
var conferenceCtrl = {  id: "conferenceList", view: "list", select: false, scroll: "y", type: {height: 93},
    template: function (obj) {
        var dateFormat = webix.Date.dateToStr("%d.%m.%Y %H:%i")
        var date = dateFormat(parseIsoDatetime(obj.date));
        return "<div class='header_style'>Секция " + obj.section + ": " + obj.name +
            "</div>\n<div>Где: <span class='location_style'>" + obj.city + ", " + obj.location +
            "</span></div>\nКогда: <span class='date_style'>" + date + "</span>";
    }};

var desktopLeft = {
    id: "first", gravity: 1.6,
    rows: [ { cols: [cityCtrl, searchCtrl, addConferenceBtn ] }, conferenceCtrl ]
};

var desktopRight = {
    id: "second", gravity: 0.5, rows: [
        { cols: [{}, addSectionBtn] }, sectionList,
    ]
};

var mobileLeft = {
    id: "first",
    rows: [cityCtrl, searchCtrl, conferenceCtrl]
};

var mobileRight = {
    id: "second", rows: [
        { cols: [addConferenceBtn, addSectionBtn] }, sectionList,
    ]
};

var mobileUI = {
    rows: [{ view: "multiview", id: "mainView", cells: [mobileLeft, mobileRight] }, {
        view: "tabbar", id: "tabs", type: "bottom", multiview: true, tabMinWidth: 80,
        options: [{ id: "first", value: "Конференции" }, { id: "second", value: "Секции" }]
    }]
};

var desktopUI = { cols: [desktopLeft, { view: "resizer", css: "resizerStyle", width: 6 }, desktopRight, ] };

function getSelectedSection() {
    return $$("sectionList").getSelectedItem();
}

var context = {
    view: "contextmenu",
    id: "cmenu",
    width: 215,
    data: [{ id: "edit", icon: "edit", value: "Редактировать" }, { id: "register", icon: "sign-in", value: "Регистрация" }]
};

function attachEvents() {
    $$("addConference").attachEvent("onItemClick", function (id, e, node) {
        showEditor("conference", "Добавить новую конференцию");
    });
    $$("addSection").attachEvent("onItemClick", function (id, e, node) {
        showEditor("section", "Добавить новую секцию");
    });
    $$("sectionList").attachEvent("onSelectChange", function (id, e, node) {
        var button = $$("addConference");
        if (id < 1)
            button.disable();
        else
            button.enable();
        loadConferenceList();
    });
    $$("conferenceList").attachEvent("onItemClick", function (id, e, node) {
        var _item = this.getItem(id);
        var menu = $$("cmenu");
        menu.show(e.target);
        menu.setContext({ obj: this, id: id, item: _item });
    });
    $$("cmenu").attachEvent("onItemClick", function (id, e, node) {
        var c = this.getContext();
        switch (id) {
            case "edit":
                showEditor("conference", "Добавить новую конференцию", c.item.id);
                break;
            case "register":
                selectedConference = c.item.id;
                showEditor("member", "Регистрация участника в концеренции\n'" + c.item.name + "'", window.localStorage.getItem("user_id"));
                break;
        }
    });
    $$("search").attachEvent("onTimedKeyPress", function () {
        $$("conferenceList").filter(function (obj) {
            var dateFormat = webix.Date.dateToStr("%d.%m.%Y %H:%i")
            var date = dateFormat(parseIsoDatetime(obj.date));
            return (obj.name + obj.city + obj.location + date).toLowerCase().indexOf($$("search").getValue().toLowerCase()) != -1;
        });
    });
}

function attachCityFilterEvent() {
    $$("cityFilter").attachEvent("onChange", function (id, e, node) {
        loadConferenceList();
    });
}