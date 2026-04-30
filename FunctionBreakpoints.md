# 🛠 Master Project Function Breakpoint Locations

This is the **complete** developer's guide for debugging the ShoppingApp. It includes precise line numbers for every major action and logic point.

---

## 1. 🔐 User Authentication & Identity
| Feature | Method | File Path | Line # |
| :--- | :--- | :--- | :--- |
| **User Signup (Submit)** | `Signup` | `Controllers/UsersController.Auth.cs` | **22** |
| **User Login (Submit)** | `Login` | `Controllers/UsersController.Auth.cs` | **53** |
| **JWT Token Creation** | `CreateToken` | `Services/TokenService.cs` | **25** |
| **Logout Logic** | `Logout` | `Controllers/UsersController.Auth.cs` | **96** |
| **Credential Logic** | `LoginAsync` | `Services/UserService.cs` | **52** |

---

## 2. 🛍 Product Catalog & UI
| Feature | Method | File Path | Line # |
| :--- | :--- | :--- | :--- |
| **Main Catalog Page** | `Index` | `Controllers/ProductsController.cs` | **20** |
| **Product Details View** | `Details` | `Controllers/ProductsController.cs` | **45** |
| **Live Search Suggester** | `GetSuggestions` | `Controllers/ProductsController.cs` | **69** |
| **Duplicate Name Check** | `CheckDuplicate` | `Controllers/ProductsController.cs` | **91** |
| **Category Filter Query** | `GetAllProductsAsync`| `Services/ProductService.cs` | **53** |

---

## 3. 🛒 Shopping Cart & Checkout
| Feature | Method | File Path | Line # |
| :--- | :--- | :--- | :--- |
| **Add Item to Cart** | `AddToCart` | `Controllers/CartsController.Actions.cs` | **62** |
| **Change Cart Quantity**| `UpdateQuantity` | `Controllers/CartsController.Actions.cs` | **19** |
| **Remove from Cart** | `Delete` | `Controllers/CartsController.Actions.cs` | **12** |
| **Initiate Checkout** | `Checkout` | `Controllers/CartsController.cs` | **40** |
| **Finalize Payment** | `Process` | `Controllers/PaymentsController.Process.cs` | **11** |
| **Payment Logic (COD)** | `ProcessPaymentAsync`| `Services/PaymentService.cs` | **45** |

---

## 4. 📦 Orders & Reviews
| Feature | Method | File Path | Line # |
| :--- | :--- | :--- | :--- |
| **My Order History** | `Index` | `Controllers/OrdersController.cs` | **20** |
| **Specific Order Info** | `Details` | `Controllers/OrdersController.cs` | **41** |
| **Submit New Review** | `SubmitReview` | `Controllers/ReviewsController.Actions.cs` | **12** |
| **Review Logic Save** | `AddOrUpdateReview` | `Services/ReviewService.cs` | **45** |

---

## 5. 👑 Admin Dashboard (Web Views)
| Feature | Method | File Path | Line # |
| :--- | :--- | :--- | :--- |
| **Stats Overview** | `Dashboard` | `Controllers/AdminController.cs` | **25** |
| **Admin Product List** | `Products` | `Controllers/AdminController.Management.cs` | **12** |
| **Admin User List** | `Users` | `Controllers/AdminController.Management.cs` | **22** |
| **Update Order Status** | `UpdateOrderStatus` | `Controllers/AdminController.Management.cs` | **46** |
| **Bulk Import (CSV)** | `ImportProducts` | `Controllers/FilesController.Actions.cs` | **50** |
| **Role Promotion** | `ToggleRole` | `Controllers/UsersController.cs` | **126** |

---

## 6. 🔌 Admin API (Backend Data)
| Feature | Method | File Path | Line # |
| :--- | :--- | :--- | :--- |
| **Get Dashboard Data** | `GetStats` | `Controllers/AdminApiController.cs` | **30** |
| **Create New Product** | `CreateProduct` | `Controllers/AdminApiController.Actions.cs` | **18** |
| **Update User API** | `UpdateUser` | `Controllers/AdminApiController.Actions.cs` | **85** |
| **Delete Category API** | `DeleteCategory` | `Controllers/AdminApiController.Actions.cs` | **69** |
| **Export to Excel** | `ExportProducts` | `Controllers/AdminApiController.Actions.cs` | **125** |

---

## 7. 🧠 Core Business Logic (Services Layer)
*These are the best places to set breakpoints to watch database interaction.*

| Logical Action | Method | File Path | Line # |
| :--- | :--- | :--- | :--- |
| **Database Save** | `CompleteAsync` | `Repositories/UnitOfWork.cs` | **25** |
| **Product Save** | `CreateProductAsync` | `Services/ProductService.cs` | **116** |
| **User Save** | `RegisterUserAsync` | `Services/UserService.cs` | **72** |
| **Order Status Update**| `UpdateOrderStatusAsync`| `Services/OrderService.cs` | **98** |
| **Category Creation** | `GetOrCreateCategory` | `Services/ProductService.cs` | **254** |

---

## 8. 🚦 Debugging Flow Guide

### Scenario: "How does an order get saved?"
1.  **Breakpoint 1**: `PaymentsController.Process.cs` @ Line **11** (Request received).
2.  **Breakpoint 2**: `Services/CartService.cs` @ Line **120** (Cart items cleared).
3.  **Breakpoint 3**: `Services/PaymentService.cs` @ Line **45** (Payment status set).
4.  **Breakpoint 4**: `Repositories/UnitOfWork.cs` @ Line **25** (Data actually written to SQL).
