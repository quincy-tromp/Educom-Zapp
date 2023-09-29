﻿
function addTableRow(table, rowIndex) {
    var newRowIndex = rowIndex + 1;

    var newRow = table.insertRow(rowIndex);
    var cell1 = newRow.insertCell(0);
    var cell2 = newRow.insertCell(1);
    var cell3 = newRow.insertCell(2);
    var cell4 = newRow.insertCell(3);
    var cell5 = newRow.insertCell(4);

    cell1.innerHTML =
    '<span>' + newRowIndex + '</span>';

    cell2.innerHTML =
    '<div class="form-group">' +
        '<input id="task-names" list="task-list" name="AppointmentTasks[' + rowIndex + '].Task.Name" asp-for="AppointmentTasks[i].Task.Name" class="form-control" value="" placeholder="Zoek taak"/>' +
    '</div>';

    cell3.innerHTML =
    '<div class="form-group">' +
        '<input id="task-info" name="AppointmentTasks[' + rowIndex + '].AdditionalInfo" asp-for="AppointmentTasks[i].AdditionalInfo" type="text" class="form-control" />' +
    '</div>';

    cell4.innerHTML = '';

    cell5.innerHTML =
    '<button id="addRow" onclick="addNewTask()">Add</button>';
}

function removeButton(table, rowIndex) {
    if (rowIndex < 0) return;
    var row = table.rows[rowIndex];
    var cell = row.cells[4].innerHTML = '';
}

function addNewTask() {
    event.preventDefault();
    var table = document.getElementById("appointment-table").getElementsByTagName("tbody")[0];
    var rowIndex = table.rows.length;
    addTableRow(table, rowIndex);
    removeButton(table, rowIndex - 1);
}