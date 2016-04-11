function addItem() {
    var table = document.getElementById("myCart");
    var item = document.getElementById("ProductNumber");
    var quantity = document.getElementById("Quantity");
    var row = table.insertRow(1); 
    var newItem = row.insertCell(0);
    var newQuantity = row.insertCell(1); 
    newItem.innerHTML = item.value;
    newQuantity.innerHTML = quantity.value;
    item.value = "";
    quantity.value = ""; 

    //return false;
    
     
}

