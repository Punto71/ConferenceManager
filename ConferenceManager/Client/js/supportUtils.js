function sendRequest(url, onComplete) {
    var http = new XMLHttpRequest();
    http.open("GET", url, true);
    http.onreadystatechange = function () {
        if (http.readyState == 4) {
            if (http.status == 200) {
                if (onComplete != null)
                    onComplete(http.responseText);
            } else if (http.status == 302) {
                window.location = http.responseText;
            } else {
                onComplete(http.responseText, http.status);
            }
        }
    };
    http.send();
}

function sendPut(_url, _object, onComplete) {
    $.ajax({
        url: _url,
        type: 'PUT',
        data: {
            data: _object
        },
        success: onComplete,
        error: onComplete
    });
}

function changeDateToAbsolute(elements) {
    for (i in elements)
        if (elements[i] instanceof Date)
            elements[i] = getAbsoluteDate(elements[i]);
    return elements;
}

function getAbsoluteDate(date) {
    return new Date(date.valueOf() - (date.getTimezoneOffset() * 60000));
}

function runPingPong(url, intervalMs, onResponse) {
    sendRequest(url, onResponse);
    setInterval(function () {
        sendRequest(url, onResponse);
    }, intervalMs);
}

function prepareTextPattern(elements) {
    for (var i in elements) {
        var elem = elements[i];
        if (elem.pattern && elem.pattern.allow == "integer")
            elem.pattern.allow = /[0-9]/g;
        else
            delete elem.pattern
    }
}

function toObject(text) {
    return webix.DataDriver.json.toObject(text);
}

function toFixed(value, roundVal) {
    return Number.parseFloat(value).toFixed(roundVal);
}

var isoTemplate = /(\d{4})-(\d{2})-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})/;

function prepareDateTime(element) {
    var regexp = new RegExp(isoTemplate);
    for (var i in element) {
        var elem = element[i];
        if (regexp.test(elem))
            element[i] = parseIsoDatetime(elem);
    }
    return element;
}

function toHHMMSS(value) {
    var sec_num = parseInt(value, 10); // don't forget the second param
    var hours = Math.floor(sec_num / 3600);
    var minutes = Math.floor((sec_num - (hours * 3600)) / 60);
    var seconds = sec_num - (hours * 3600) - (minutes * 60);

    if (hours < 10) { hours = "0" + hours; }
    if (minutes < 10) { minutes = "0" + minutes; }
    if (seconds < 10) { seconds = "0" + seconds; }
    if (hours > 0) {
        return hours + ':' + minutes + ':' + seconds;
    }
    return minutes + ':' + seconds;
}

function parseIsoDatetime(dtstr) {
    var dt = dtstr.split(/[: T-]/).map(parseFloat);
    return new Date(dt[0], dt[1] - 1, dt[2], dt[3] || 0, dt[4] || 0, dt[5] || 0, 0);
}

function prepareAudioName(songName) {
    return songName.replace("amp;", "")
}

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}

function sleep(sleepDuration, action) {
    var now = new Date().getTime();
    while (new Date().getTime() < now + sleepDuration) { action() }
}


function parseNumber(newValue, oldValue) {
    /// <summary>Проверяет число на валидность, и преобразовывает в формат 0.00. Если число не валидно возвращает oldValue</summary>
    if (newValue != "") {
        var strNum = newValue.toString().replace(',', '.');
        var number = parseFloat(strNum);
        if (isNaN(number))
            number = oldValue;
        return number;
    }
}

function loadLocales() {
    webix.Date.startOnMonday = true;
    webix.i18n.locales["ru-RU"] = {
        calendar: {
            monthFull: ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"],
            monthShort: ["Янв", "Фев", "Мар", "Апр", "Май", "Июнь", "Июль", "Авг", "Сен", "Окт", "Нояб", "Дек"],
            dayFull: ["Воскресенье", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота"],
            dayShort: ["Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб"],
            clear: "Очистить",
            today: "Сегодня",
            done: "Ок",
            hours: "Часы",
            minutes: "Минуты",
        },
        dateFormat: "%d.%m.%Y",
        fullDateFormat: "%d.%m.%Y %H:%i:%s",
        groupDelimiter: " ",
        groupSize: 3,
        decimalDelimiter: ",",
        timeFormat: "%H:%i:%s",
        decimalSize: 2,
        am: ["am", "AM"],
        pm: ["pm", "PM"],
    };
    webix.i18n.setLocale("ru-RU");
}

function formatDate(date) {
    /// <summary>Преобразовывает дату (или часть даты) формата dd.MM.yyyy в дату формата yyyy-MM-dd</summary>
    if (date.indexOf('.') > -1) {
        var values = date.split('.');
        if (values.length == 1)
            date = values[0] + '-';
        if (values.length == 2)
            date = values[1] + '-' + values[0];
        if (values.length == 3)
            date = values[2] + '-' + values[1] + '-' + values[0];
    }
    return date;
}

function changeButtonImageStatus(button) {
    var currentImage = button.config.image;
    if (currentImage.indexOf("_disabled") > -1)
        changeButtonImage(button, "_disabled", "_icon");
    else if (currentImage.indexOf("_icon") > -1)
        changeButtonImage(button, "_icon", "_disabled");
}

function changeButtonImage(button, from, to) {
    var currentImage = button.config.image;
    button.config.image = currentImage.replace(from, to);
    button.define("image", button.config.image);
    button.refresh();
}

function changeButtonStatus(button, enabled) {
    if (enabled)
        button.enable();
    else
        button.disable();
    changeButtonImageStatus(button);
    button.refresh();
}

function isTextKeyCode(code) {
    if (code >= 48 && code <= 90)
        return true;
    if (code >= 93 && code <= 105)
        return true;
    return false;
}

function getSelectedCell(grid) {
    var cell = null;
    if (grid != undefined) {
        var cells = grid.getSelectedId(true);
        if (cells.length == 1)
            cell = cells[0];
    }
    return cell;
}

function setCellValue(grid, cell, value) {
    record = grid.getItem(cell.row);
    record[cell.column] = "";
    grid.updateItem(cell.row, record);
}

function copyObject(obj) {
    if (null == obj || "object" != typeof obj) return obj;
    var copy = obj.constructor();
    for (var attr in obj) {
        if (obj.hasOwnProperty(attr)) copy[attr] = obj[attr];
    }
    return copy;
}
