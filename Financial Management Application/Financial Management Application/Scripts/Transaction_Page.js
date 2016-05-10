function addItem(id) {
    
    var table = document.getElementById("myCart");
    var item = document.getElementsByName("ProductNumber")[id];
    var quantity = document.getElementById("quantity"+id);
   
    
    if (quantity != "") {
        var row = table.insertRow(1);
        var newItem = row.insertCell(0);
        var newQuantity = row.insertCell(1);
        newItem.innerHTML = item.innerHTML;
        newQuantity.innerHTML = quantity.value;
        document.getElementById("quantity"+id).value = ""; 
    }

    else {
        alert("invalid entry");
    }
}

function doSearch() {
   
    var searchText = document.getElementById('searchTerm').value;
    var targetTable = document.getElementById('datatable');
    var targetTableColCount; 
    //Loop through table rows
    for (var rowIndex = 0; rowIndex < targetTable.rows.length; rowIndex++) {
        var rowData = '';

        //Get column count from header row
        if (rowIndex == 0) {
            targetTableColCount = targetTable.rows.item(rowIndex).cells.length;
            continue; //do not execute further code for header row.
        }

        //Process data rows. (rowIndex >= 1)
        for (var colIndex = 0; colIndex < targetTableColCount; colIndex++) {
            rowData += targetTable.rows.item(rowIndex).cells.item(colIndex).textContent;
        }

        //If search term is not found in row data
        //then hide the row, else show
        if (rowData.toLocaleLowerCase().indexOf(searchText.toLocaleLowerCase()) == -1)
            targetTable.rows.item(rowIndex).style.display = 'none';
        else
            targetTable.rows.item(rowIndex).style.display = 'table-row';
    }
}

