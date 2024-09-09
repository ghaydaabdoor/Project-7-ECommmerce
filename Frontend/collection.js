async function getCategory() {
    const url = "https://localhost:44325/api/Categories/GetAllCategories";
    let response = await fetch(url);
    if (!response.ok) {
        console.error("Failed to fetch categories");
        return;
    }
    var result = await response.json();
    const dropdown = document.getElementById('dynamicDropdown');

    // Populate the dropdown with options
    result.forEach(category => {
        const option = document.createElement('option');
        option.value = category.categoryId;
        option.innerHTML = category.categoryName;
        dropdown.appendChild(option);
    });

    // Add event listener to store selected categoryId in localStorage
    dropdown.addEventListener('change', function () {

        const selectedCategoryId = dropdown.value;
        localStorage.setItem('selectedCategoryId', selectedCategoryId);
        console.log(`Category ID ${selectedCategoryId} saved to localStorage`);

        // Instead of reloading, call the function directly
        if (localStorage.getItem('selectedCategoryId') === 	"All Category") {
            clearStorage()
        }else {
            getProductsByCategoryId();
            }
    });
}

// Function to populate the product grid
function renderProduct(element, container) {
    container.innerHTML += `
        <li class="st-col-item">
            <div class="single-product-wrap">
                <!-- product-img start -->
                <div class="product-image">
                    <div  class="pro-img">
                        <img src="${element.productImage}" class="img-fluid img1" alt="p-1">
                    </div>
                    <div class="product-action" >
                        <a href="product-details.html" class="quick-view" onclick="saveProductId(${element.productId})" >
                            <span class="tooltip-text">Product Details</span>
                            <span class="quickview-icon"><i class="feather-eye"></i></span>                            
                        </a>
                    </div>
                </div>
                <div class="product-content">
                    <h6>${element.productName}</h6>
                    <div class="price-box">
                        <span class="new-price">${element.price} JOD</span>
                    </div>
                </div>
            </div>
        </li>
    `;
}

// Fetch and display products by category
async function getProductsByCategoryId() {
    const selectedCategoryId = localStorage.getItem("selectedCategoryId");
    if (!selectedCategoryId) return;
    
    const url = `https://localhost:44325/api/Products/category/${selectedCategoryId}`;
    let response = await fetch(url);
    if (!response.ok) {
        console.error("Failed to fetch products for category");
        return;
    }
    const result = await response.json();
    const container = document.getElementById("product-grid");
    container.innerHTML = ''; // Clear existing products

    result.forEach(element => renderProduct(element, container));
}

// Fetch and display all products
async function getAllProduct() {
    const url = "https://localhost:44325/api/Products";
    let response = await fetch(url);
    if (!response.ok) {
        console.error("Failed to fetch all products");
        return;
    }
    const result = await response.json();
    const container = document.getElementById("product-grid");
    container.innerHTML = ''; // Clear existing products

    result.forEach(element => renderProduct(element, container));
}

// Call the appropriate function based on selected category
if (!localStorage.getItem("selectedCategoryId")) {
    getAllProduct();
} else {
    getProductsByCategoryId();
}

// Clear localStorage function
function clearStorage() {
    debugger
    localStorage.removeItem("selectedCategoryId");
    getAllProduct();
}

// Call the function to populate the dropdown on page load
getCategory();





function updatePriceRange() {

    const minPrice = document.getElementById('range1').value;
    const maxPrice = document.getElementById('range2').value;

    // Debugging log
    console.log("Min Price:", minPrice);
    console.log("Max Price:", maxPrice);

    // document.getElementById('demo1').innerHTML = minPrice;
    // document.getElementById('demo2').innerHTML = maxPrice;

    fetchProductsByPriceRange(minPrice, maxPrice);
}


async function fetchProductsByPriceRange(minPrice, maxPrice) {
    const url = `https://localhost:44325/api/Products/priceRange?minPrice=${minPrice}&maxPrice=${maxPrice}`;

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error('No products found within the specified price range.');
        }
        
        const products = await response.json();
        displayFilteredProducts(products);
    } catch (error) {
        console.error('Error fetching products:', error);
    }
}

function displayFilteredProducts(products) {
    const container = document.getElementById("product-grid");
    container.innerHTML = "";

    products.forEach(element => {
        container.innerHTML += `
        <li class="st-col-item">
            <div class="single-product-wrap">
                <div class="product-image">
                    <div class="pro-img">
                        <img src="${element.productImage}" class="img-fluid img1" alt="p-1">
                    </div>
                    <div class="product-action" >
                        <a href="productDetails.html" class="quick-view" onclick="saveProductId(${element.productId})" >
                            <span class="tooltip-text">Product Details</span>
                            <span class="quickview-icon"><i class="feather-eye"></i></span>                            
                        </a>
                    </div>
                </div>
                <div class="product-content">
                    <h6>${element.productName}</h6>
                    <div class="price-box">
                        <span class="new-price">${element.price} JOD</span>
                    </div>
                </div>
            </div>
        </li>
        `;
    });
}

function saveProductId(productId) {
    localStorage.setItem('productId', productId);
}

////////////////
///Searching for products
//////////////////////

function searchProducts() {
    debugger
    const searchQuery = document.getElementById('productSearchInput').value;

    // Fetch products filtered by search query
    fetchProductsByName(searchQuery);
}

async function fetchProductsByName(query) {
    const url = `https://localhost:44325/api/Products/search?query=${encodeURIComponent(query)}`;

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error('No products found with the specified search term.');
        }
        const products = await response.json();
        console.log('API Products:', products); // Log API products for debugging
        displayFilteredProducts(products);
    } catch (error) {
        console.error('Error fetching products:', error);
    }
}

