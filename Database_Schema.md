# Database Schema: CommerceDB

This document provides a summary of all tables and columns currently present in the `CommerceDB` database.

## Tables Overview

1. [Users](#table-users)
2. [Products](#table-products)
3. [Categories](#table-categories)
4. [Orders](#table-orders)
5. [OrderItems](#table-orderitems)
6. [Payments](#table-payments)
7. [Cart](#table-cart)
8. [__EFMigrationsHistory](#table-__efmigrationshistory)

---

<a name="table-users"></a>
## Table: Users
| Column | Data Type | Nullable |
| :--- | :--- | :--- |
| Id | int | NO |
| Username | nvarchar | NO |
| PasswordHash | nvarchar | NO |
| Role | nvarchar | NO |
| Address | nvarchar | NO |
| Email | nvarchar | NO |
| PhoneNumber | nvarchar | NO |

<a name="table-products"></a>
## Table: Products
| Column | Data Type | Nullable |
| :--- | :--- | :--- |
| Id | int | NO |
| Name | nvarchar | NO |
| Description | nvarchar | YES |
| Price | decimal | NO |
| Stock | int | NO |
| ImageUrl | nvarchar | YES |
| CategoryId | int | YES |

<a name="table-categories"></a>
## Table: Categories
| Column | Data Type | Nullable |
| :--- | :--- | :--- |
| Id | int | NO |
| Name | nvarchar | NO |
| Description | nvarchar | YES |

<a name="table-orders"></a>
## Table: Orders
| Column | Data Type | Nullable |
| :--- | :--- | :--- |
| Id | int | NO |
| UserId | int | NO |
| OrderDate | datetime2 | NO |
| TotalAmount | decimal | NO |

<a name="table-orderitems"></a>
## Table: OrderItems
| Column | Data Type | Nullable |
| :--- | :--- | :--- |
| Id | int | NO |
| OrderId | int | NO |
| ProductId | int | NO |
| Quantity | int | NO |
| UnitPrice | decimal | NO |

<a name="table-payments"></a>
## Table: Payments
| Column | Data Type | Nullable |
| :--- | :--- | :--- |
| Id | int | NO |
| OrderId | int | NO |
| Amount | decimal | NO |
| Method | nvarchar | NO |
| Status | nvarchar | NO |
| PaidAt | datetime2 | YES |

<a name="table-cart"></a>
## Table: Cart
| Column | Data Type | Nullable |
| :--- | :--- | :--- |
| CartID | int | NO |
| UserID | int | YES |
| ProductID | int | YES |
| Quantity | int | YES |

<a name="table-__efmigrationshistory"></a>
## Table: __EFMigrationsHistory
| Column | Data Type | Nullable |
| :--- | :--- | :--- |
| MigrationId | nvarchar | NO |
| ProductVersion | nvarchar | NO |
