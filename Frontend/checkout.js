// Retrieve the stored data
let userId = localStorage.getItem('UserID');
// let TotalAmount = localStorage.getItem('totalAmount');

// Function to fetch user address data by UserId
async function findUserAddressByUserId() {
    // Get form fields
    debugger
    let fname = document.getElementById("f-name");
    let lname = document.getElementById("l-name");
    let email = document.getElementById("mail");
    let phone = document.getElementById("phone");
    let city = document.getElementById("city");
    let hnumber = document.getElementById("h-number");

    let spanForTotal = document.getElementById("spanForTotal");
    spanForTotal.innerHTML = localStorage.getItem('TotalAmount');

    // Fetch data from the API
    const url = `https://localhost:44325/api/UserAdress/name/${userId}`;
    try {
        let response = await fetch(url);
        if (!response.ok) {
            throw new Error("User not found");
        }

        let result = await response.json();

        if (result.length == 0) {
            alert("User not found");
            return;
        }

        // Set the fetched data into the form fields for read-only display
        fname.value = result[0].firstName;
        lname.value = result[0].lastName;
        email.value = result[0].email;
        phone.value = result[0].phone;
        

        // Make the fields read-only
        fname.setAttribute("readonly", true);
        lname.setAttribute("readonly", true);
        email.setAttribute("readonly", true);
        phone.setAttribute("readonly", true);
        

    } catch (error) {
        console.error("Error fetching user data: ", error);
        alert("Unable to fetch user data");
    }
}

// city.value = result[0].city;
//         hnumber.value = result[0].apartmentNumber;
// city.setAttribute("readonly", true);
//         hnumber.setAttribute("readonly", true);


// Fetch the user address on page load
findUserAddressByUserId();



let addresses = []; // Variable to store the fetched address data

// Function to fetch user addresses by userId and populate the dropdown
async function findUserAddressesByUserId() {
    let streetDropdown = document.getElementById("street");

    const url = `https://localhost:44325/api/UserAdress/userAddresses/${userId}`;
    try {
        let response = await fetch(url);
        if (!response.ok) {
            throw new Error("Failed to fetch addresses");
        }

        let result = await response.json();

        // Check if any addresses are returned
        if (result.length === 0) {
            alert("No addresses found for this user");
            return;
        }

        // Store the fetched addresses for later use
        addresses = result;

        // Clear the existing options in the dropdown
        streetDropdown.innerHTML = "";

        // Loop through the result and create options for each street
        result.forEach(address => {
            let option = document.createElement("option");
            option.value = address.street; // Set the value as the street name
            option.text = address.street;  // Display the street name
            streetDropdown.appendChild(option); // Add the option to the dropdown
        });

        // Trigger the event to populate other fields based on the first option
        streetDropdown.dispatchEvent(new Event('change'));

    } catch (error) {
        console.error("Error fetching addresses:", error);
        alert("Unable to fetch addresses");
    }
}

// Function to update Apartment Number and City based on selected street
function updateAddressDetails() {
    let streetDropdown = document.getElementById("street");
    let selectedStreet = streetDropdown.value;

    // Find the selected address based on the street
    let selectedAddress = addresses.find(address => address.street === selectedStreet);

    if (selectedAddress) {
        // Update the Apartment Number and City fields
        document.getElementById("h-number").value = selectedAddress.homeNumberCode;
        document.getElementById("city").value = selectedAddress.city;
    } else {
        // Clear fields if no matching address is found
        document.getElementById("h-number").value = "";
        document.getElementById("city").value = "";
    }
}

// Add event listener to the street dropdown to update other fields when the user selects a street
document.getElementById("street").addEventListener("change", updateAddressDetails);

// Fetch and display user addresses in the dropdown on page load
findUserAddressesByUserId();
 

document.getElementById('placeOrderBtn').addEventListener('click', function (event) {
    event.preventDefault(); // Prevent the default link behavior

    let comments = document.getElementById("comments").value;
    localStorage.setItem("OrderStatus","Completed");
    localStorage.setItem("Comment",comments);

    let isPaypalChecked = document.getElementById('checkPaypal').checked;

    if (isPaypalChecked) {

        localStorage.setItem("PaymentMethod","Paypal");
        // Redirect to the PayPal payment page if PayPal is selected
        window.location.href = "Paypal.html";
    } else {

        localStorage.setItem("PaymentMethod","Cash"); 
        // If PayPal is not selected, continue to the default order complete page
        window.location.href = "order-complete.html";
    }
});
