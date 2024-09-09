
debugger

async function Product() {

    var id = localStorage.getItem("productId");
    let url = `https://localhost:44325/api/productDetails/GetOneProductByID/${id}`;
    let response = await fetch(url);
    let data = await response.json();
    let priceItem = document.getElementById("Priice");
    priceItem.innerHTML += `
      <div class="pro-prlb pro-sale">
          <div class="price-box">
              <span class="new-price">${data.price}</span>
          </div>
      </div>
      `;
    console.log(data);
  }
  Product();
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  async function TitleForProduct() {

    var n = localStorage.getItem("productId");
    let url = `https://localhost:44325/api/productDetails/GetOneProductByID/${n}`;
    let response = await fetch(url);
    let data = await response.json();
    let TitleItem = document.getElementById("TitleJS");
    TitleItem.innerHTML = `
      <h2>${data.productName}</h2>
      `;
    console.log(data);
  }
  TitleForProduct();
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  async function updateStockStatus() {

    var s = localStorage.getItem("productId");
    let url = `https://localhost:44325/api/productDetails/GetOneProductByID/${s}`;
    let response = await fetch(url);
    let data = await response.json();
    document.getElementById("stock-status").style.display = "none";
    document.getElementById("near-penetration").style.display = "none";
    document.getElementById("out-of-stock").style.display = "none";
    if (data.stock === 0) {
      document.getElementById("out-of-stock").style.display = "inline";
    } else if (data.stock <= 40) {
      document.getElementById("near-penetration").style.display = "inline";
    } else if (data.stock > 40) {
      document.getElementById("stock-status").style.display = "inline";
    }
    console.log(data);
  }
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  async function ProductRating() {
    var s = localStorage.getItem("productId");
    const url = `https://localhost:44325/api/productDetails/GetOneProductByID/${s}`;
    const response = await fetch(url);
    const { averageRate } = await response.json(); // افترض أن الاستجابة تحتوي على متوسط التقييم
    if (averageRate != null) {
      const starRatingElement = document.querySelector(
        "#ProductRating .star-rating"
      );
      const fullStars = Math.floor(averageRate);
      const halfStar = averageRate % 1 >= 0.5;
      const totalStars = 5;
      starRatingElement.innerHTML = `${'<i class="fas fa-star"></i>'.repeat(
        fullStars
      )}${
        halfStar ? '<i class="fas fa-star-half-alt"></i>' : ""
      }${'<i class="far fa-star"></i>'.repeat(
        totalStars - fullStars - (halfStar ? 1 : 0)
      )}`;
      const captionElement = document.querySelector(
        "#ProductRating .spr-badge-caption"
      );
      captionElement.textContent = `${averageRate} out of 5 stars`;
    } else {
      console.error("Rating data is missing or invalid");
    }
  }
  ProductRating();
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  async function ProductImage() {
    
    var id = localStorage.getItem("productId");
    let url = `https://localhost:44325/api/productDetails/GetOneProductByID/${id}`;
    let response = await fetch(url);
    let data = await response.json();
    console.log(data);
    let ImagePro = document.getElementById("ProductsImage");
    ImagePro.innerHTML = `
      <div class="product_detail_img product_detail_img_left">
              <div class="product_img_top">
                  <button class="full-view"><i class="bi bi-arrows-fullscreen"></i></button>
                  <!-- blog slick slider start -->
                  <div class="style4-slider-big slick-slider">
                      <!-- <div class="slick-slide"> -->
                          <a href="${data.productImage}" class="product-single">
                              <figure class="zoom" onmousemove="zoom(event)" style="background-image: url('${data.productImage}');">
                                  <img src="${data.productImage}" class="img-fluid" alt="product image">
                              </figure>
                          </a>
                      <!-- </div> -->
                  </div>
              </div>
              <div class="pro-slider">
                  <div class="style4-slider-small pro-detail-slider">
                      <!-- <div class="slick-slide"> -->
                          <a href="javascript:void(0)" class="product-single__thumbnail">
                              <img src="${data.productImage}" class="img-fluid" alt="product thumbnail">
                          </a>
                      <!-- </div> -->
                  </div>
              </div>
          </div>
      `;
    console.log(data);
  }
  ProductImage();
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  // async function AddToCart() {
  //   localStorage.setItem("productId", 1);
  //   var id = localStorage.getItem("productId");
  //   let url = `https://localhost:44325/api/productDetails/GetOneProductByID/${id}`;
  
  //   let response = await fetch(url);
  //   let data = await response.json();
  
  //   let priceItem = document.getElementById("AddedToCart");
  
  //   if (priceItem) {
  //     // Check if the element exists
  //     priceItem.innerHTML += `
  //         <button type="button" class="btn add-to-cart ajax-spin-cart" onclick="Added(${data.productId})">
  //             <span class="cart-title">Add to cart</span>
  //         </button>
  //         `;
  //   }
  
  //   console.log(data);
  // }
  
  // AddToCart();
  
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  async function DescriptionToPro() {
  
    var id = localStorage.getItem("productId");
    let url = `https://localhost:44325/api/productDetails/GetOneProductByID/${id}`;
  
    let response = await fetch(url);
    let data = await response.json();
  
    let Description = document.getElementById("collapse-description");
  
    if (Description) {
      Description.innerHTML += `
              <p> ${data.productDescription} </p>
          `;
    }
  
    console.log(data);
  }
  
  DescriptionToPro();
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  document.addEventListener("DOMContentLoaded", function () {
  
  
    const stars = document.querySelectorAll("#rating-stars span");
    const ratingValue = document.getElementById("ratingValue");
  
    stars.forEach((star) => {
      star.addEventListener("click", function () {
        const value = this.getAttribute("data-value");
        ratingValue.value = value;
  
        // Update selected stars
        stars.forEach((star) => {
          star.classList.remove("selected");
          if (star.getAttribute("data-value") <= value) {
            star.classList.add("selected");
          }
        });
      });
    });
  
  
  
  
  
  
  
  
  
  
  
  
  
  
    document
      .getElementById("ratingForm")
      .addEventListener("submit", async function (event) {
        event.preventDefault(); // Prevent the default form submission
  
        // Retrieve form data
        const productId = localStorage.getItem("productId");
        const userId = localStorage.getItem("userID");
        const ratingValue = document.getElementById("ratingValue").value;
  
        if (!ratingValue) {
          window.alert("Please select a rating before submitting.");
          return;
        }
  
        // Create the rating object
        const rating = {
          productId: parseInt(productId),
          userId: parseInt(userId),
          ratingValue: parseInt(ratingValue),
        };
        try {
          const response = await fetch(
            "https://localhost:44325/api/productDetails/addRate",
            {
              method: "POST",
              headers: {
                "Content-Type": "application/json",
              },
              body: JSON.stringify(rating),
            }
          );
  
          if (!response.ok) {
            const errorMessage = await response.text();
            window.alert("Error: " + errorMessage);
            console.error("Server Error:", errorMessage); // Log server error
            return;
          }
  
          const data = await response.json();
          window.alert(data.message);
  
          // Update the displayed rating after successfully submitting
          ProductRating();
      } catch (error) {
          window.alert(
          "An error occurred while adding the rating. Please try again."
          );
          console.error("Error:", error); // Log the error
      }
      });
  });
  
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  async function CheckRate() {
  try {
      let url = `https://localhost:44325/api/productDetails/checkrating`;
      let response = await fetch(url);
      let data = await response.json();
      
      // افترض أن البيانات المسترجعة هي قيمة التقييم
      const averageRate = data; // تأكد من أن هذا هو نوع البيانات الصحيح
  
      const starContainer = document.querySelector("#ProductRating .star-rating");
      const ratingText = document.querySelector("#noReview");
  
      starContainer.innerHTML = ''; // إعادة تعيين محتوى النجوم
  
      const fullStars = Math.floor(averageRate);
      const halfStar = averageRate % 1 >= 0.5;
      const totalStars = 5;
  
      // إنشاء HTML للنجوم
      starContainer.innerHTML += `${'<i class="fas fa-star"></i>'.repeat(fullStars)}${
      halfStar ? '<i class="fas fa-star-half-alt"></i>' : ""
      }${'<i class="far fa-star"></i>'.repeat(totalStars - fullStars - (halfStar ? 1 : 0))}`;
  
      // إضافة نص التقييم
      ratingText.textContent = `${averageRate.toFixed(1)} out of 5 stars`;
  } catch (error) {
      console.error("Error fetching rating data:", error);
  }
  }
  CheckRate();
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  async function DetailsCommentToPro() {
  
    const id = localStorage.getItem("productId");
    const url = `https://localhost:44325/api/productDetails/GetAllComment/${id}`;
    
    try {
      const response = await fetch(url);
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }
      
      const comments = await response.json(); // This will be an array of comments
      const AddComment = document.getElementById("AddNewComment");
      
      // Clear existing content
      AddComment.innerHTML = '';
      
      // Check if there are comments
      if (comments.length === 0) {
        AddComment.innerHTML = '<p>No comments found.</p>';
        return;
      }
      
      // Iterate over the comments array and create HTML for each comment
      comments.forEach(comment => {
        AddComment.innerHTML += `
          <div class="comment-avtar">
            <div class="review-name">
              <span class="avtar-cmt">
                <span class="cmt-auth">Ol</span>
              </span>
            </div>
            <div class="review-info">
              <span class="cmt-authr"> ${comment.commentId} </span>
            </div>
          </div>
          <div class="comment-content">
            <div class="comment-desc">
              <p>${comment.commentText}</p>
            </div>
          </div>
        `;
      });
    } catch (error) {
      console.error('Fetch error:', error);
      const AddComment = document.getElementById("AddNewComment");
      AddComment.innerHTML = '<p>There was an error loading comments.</p>';
    }
  }
  
  DetailsCommentToPro();
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  async function CountToProduct() {
    try {
      // Remove event.preventDefault(); as it's not applicable in this context
      // localStorage.setItem("productId", 1);
      var id = localStorage.getItem("productId") || 1; // Default to 1 if not set
  
      let url = `https://localhost:44325/api/productDetails/GetCountComments/${id}`;
      let response = await fetch(url);
  
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
  
      let data = await response.json();
      let ComPro = document.getElementById("CmtTitCount");
  
      // Extract the comment count from the API response
      let commentCount = data.commentCount || 0; // Default to 0 if commentCount is not present
  
      ComPro.innerHTML = `
        <h6 class="comment-title">
          <span class="cmt-title">Comments</span>
          <span class="cmt-count">(${commentCount})</span>
        </h6>
      `;
  
      console.log(data);
    } catch (error) {
      console.error('Error fetching comments:', error);
      let ComPro = document.getElementById("CmtTitCount");
      ComPro.innerHTML = `
        <h6 class="comment-title">
          <span class="cmt-title">Comments</span>
          <span class="cmt-count">(0)</span>
        </h6>
      `;
    }
  }
  CountToProduct();
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  // debugger;
  async function CommentToPro() {
  
    var id = localStorage.getItem("productId");
    let url = `https://localhost:44325/api/productDetails/AddCommentToProductById/${id}`;
    let response = await fetch(url);
    let data = await response.json();
    let AddNewComment = document.getElementById("NewCommentByUser");
    AddNewComment.innerHTML += `
  <form method="post" id="commentForm" class="comment-form">
            <div class="comments-reply-area">
                <h6 class="comment-title">Leave a comment</h6>
                <div class="form-wrap">
                    <div class="form-filed">
                        <label>Name<span class="required">*</span></label>
                        <input type="text" id="authorName" name="comment[author]" placeholder="Name" value="${data.userrRequest.firstName} ${data.userrRequest.lastName}">
                    </div>
    
                    <div class="form-filed">
                        <label>Email address<span class="required">*</span></label>
                        <input type="text" id="authorEmail" name="comment[email]" placeholder="Email address" value="${data.userrRequest.email}">
                    </div>
    
                    <div class="form-filed">
                        <label>Message<span class="required">*</span></label>
                        <textarea rows="5" id="commentText" class="comment-notes" placeholder="Message">${data.commentText}</textarea>
                    </div>
                    
                </div>
                <div class="comment-form-submit">
                    <button type="button" class="btn btn-style2" id="postCommentButton">
                        <span class="pre-msg">Post comment</span>
                    </button>
                </div>
            </div>
        </form>
        `;
        console.log(data);
  }
    CommentToPro();
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  function Added(id) {
  // debugger;

    // window.location.href = "cart-page.html";
  window.location.href = "#img-1";
  }
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  
  ///////////////////////////////////////////////////////////////////////////////Break///////////////////////////////////////////////////////////////////////////////
  
  
  updateStockStatus();
  ProductRating();
 