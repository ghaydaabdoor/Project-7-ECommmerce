async function addToCart() {

    let n = localStorage.getItem("productId");
    let cartID = localStorage.getItem("cartID") || Date.now();  // generate cartID if not present
    let cartItems = JSON.parse(localStorage.getItem("cartItems")) || [];
    console.log("befor cartItems", cartItems);
   
    let quantity = document.getElementById("quantity").value;
    if(quantity==null ||quantity==""){
        alert("Please INSERT THE QUANNTITY ");
        return;
    }
    let cartItemId = localStorage.getItem("CartItemId") ? parseInt(localStorage.getItem("CartItemId")) : 1;
    debugger;
    let response = await fetch(`https://localhost:44325/api/Products/${n}`);
    let result = await response.json();
    // console.log("result", result.price)
   

    let existingProductIndex = cartItems.findIndex(item => item.productID == n);
    if (existingProductIndex !== -1) {
           // Update quantity if the product is already in the cart
         cartItems[existingProductIndex].quantity += parseInt(quantity);
        }
        else {

    // Add new item to the cart with a unique CartItemId
    let product = {
        cartItemId: cartItemId,  // Set the auto-incremented CartItemId
        cartID: cartID,
        productID: n,
        quantity: parseInt(quantity),
        price: result.price,  // Example price, this should come from the actual product data
        imageUrl: result.productImage,  // Example image URL, replace with actual data
        name: result.productName  // Example product name, replace with actual data
    };

   

    cartItems.push(product);

    console.log("cartItems", cartItems);
    // Increment the CartItemId for the next item
    localStorage.setItem("CartItemId", cartItemId + 1);
}
    localStorage.setItem("cartItems", JSON.stringify(cartItems));


    window.alert("The item was added to the mini cart successfully.");
    updateMiniCart();  // Update the mini cart display
    event.preventDefault();
    
}



function updateMiniCart() {
    let cartItems = JSON.parse(localStorage.getItem("cartItems")) || [];
    let cartContainer = document.querySelector('.cart-item');
    cartContainer.innerHTML = '';  // Clear existing items


  
    let subtotalValue = 0;

    let count=document.getElementById("count");
    let itemCount = cartItems.length;
count.innerHTML=` <span class="cart-count-desc">There are</span>
                    <span class="cart-count">${itemCount}</span>
                    <span class="cart-count-desc">products</span>`;
    if (cartItems.length === 0) {
        document.querySelector('.empty-cart').classList.remove('d-none');
        document.querySelector('.cart-text').classList.add('d-none');
    } else {
        document.querySelector('.empty-cart').classList.add('d-none');
        document.querySelector('.cart-text').classList.remove('d-none');
        cartItems.forEach(item => {
            cartContainer.innerHTML += `
                <li class="cart-product">
                    <div class="cart-img">
                        <a href="product-template.html" class="img-area">
                            <img src="${item.imageUrl}" class="img-fluid" alt="${item.name}">
                        </a>
                    </div>
                    <div class="cart-content">
                        <h6><a href="product-template.html">${item.name}</a></h6>
                        <div class="product-info">
                            <div class="info-item">
                                <span class="product-qty">${item.quantity}</span>
                                <span>×</span>
                                <span class="product-price">$${item.price}  </span>
                            </div>
                        </div>
                        <div class="product-quantity-action">
                            <button class="delete-icon" onclick="removeCartItem(${item.cartItemId})">
                                <i class="feather-trash-2"></i>
                            </button>
                        </div>
                    </div>
                </li>
            `;
            
            subtotalValue += item.price * item.quantity;
        });
        let subtotal = document.getElementById("subtotal");
    subtotal.innerHTML = `<span class="subtotal-price">€${subtotalValue.toFixed(2)}</span>`;
    }
}

// Call this function to load the cart on page load
updateMiniCart();

function removeCartItem(cartItemId) {
    let cartItems = JSON.parse(localStorage.getItem("cartItems")) || [];
    let updatedCartItems = cartItems.filter(item => item.cartItemId != cartItemId);

    localStorage.setItem("cartItems", JSON.stringify(updatedCartItems));
    updateMiniCart();  // Update the display after deletion

    window.alert("Item removed from cart.");
}


async function checkoutCart() {
    debugger;
    let cartItems = JSON.parse(localStorage.getItem("cartItems")) || [];

    if (cartItems.length === 0) {
        window.alert("Your cart is empty!");
        return;
    }

    const url = "https://localhost:44325/api/CartItem/addtocart";  // Your backend endpoint for saving cart items

    for (let item of cartItems) {
        let response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify(item),
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            window.alert("Failed to save some items. Please try again.");
            return;
        }
    }

    window.alert("Checkout completed successfully!");
    localStorage.removeItem("cartItems");  // Clear the cart after checkout
    location.reload();  // Reload the page or redirect to a success page
}


