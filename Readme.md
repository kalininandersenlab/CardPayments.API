# CardPaymentAPI

**CardPaymentAPI** is a service for processing card payments. This project implements transaction handling, fee tracking, and fee updates based on certain logic. Security improvements are also implemented to protect sensitive data.

---

## Table of Contents
1. [Setup Instructions](#setup-instructions)
2. [API Documentation](#api-documentation)
   - [CardController](#cardcontroller---card-management-api)
   - [AuthController](#authcontroller---authentication-api)
   - [TransactionController](#transactioncontroller---transaction-management-api)
3. [Architectural Choices](#architectural-choices)
4. [Security Improvements](#security-improvements)

---

## Setup Instructions

### Prerequisites

Before running the project, ensure you have the following installed:

1. **.NET SDK** (version 8.0 or higher):
   - Check your version: `dotnet --version`
2. **SQL Server**: The application uses **SQL Server** as the database.

### Installation Steps

1. **Clone the repository:**
   ```sh
   git clone <repository_url>
   ```
2. **Navigate to the project folder:**
   ```sh
   cd CardPaymentAPI
   ```
3. **Restore dependencies:**
   ```sh
   dotnet restore
   ```
4. **Apply database migrations:**
   ```sh
   dotnet ef database update
   ```
5. **Run the application:**
   ```sh
   dotnet run
   ```
6. **Run tests:**
   ```sh
   dotnet test
   ```

---

## API Documentation

### 1. Card Management

#### Create a New Card
- **Endpoint:** `POST /api/cards`
- **Description:** Creates a new card and returns the card details.
- **Responses:**
  - `201 Created` – Card successfully created.
  - `400 Bad Request` – If creation fails.

#### Get Card Balance
- **Endpoint:** `GET /api/cards/{cardId}/balance`
- **Description:** Retrieves the balance of a specific card.
- **Responses:**
  - `200 OK` – Returns the card balance.
  - `404 Not Found` – If the card does not exist.

#### Update Card Details
- **Endpoint:** `PUT /api/cards/{cardId}`
- **Description:** Updates the details of an existing card.
- **Responses:**
  - `200 OK` – Card updated successfully.
  - `404 Not Found` – If the card does not exist.

---

### 2. Authentication

#### Register a User
- **Endpoint:** `POST /api/auth/register`
- **Description:** Registers a new user.
- **Responses:**
  - `200 OK` – Returns a token and refresh token.
  - `400 Bad Request` – If registration fails.

#### User Login
- **Endpoint:** `POST /api/auth/login`
- **Description:** Logs in an existing user.
- **Responses:**
  - `200 OK` – Returns a token and refresh token.
  - `401 Unauthorized` – If login fails.

#### Refresh JWT Token
- **Endpoint:** `POST /api/auth/refresh-token`
- **Description:** Refreshes the JWT token using a valid refresh token.
- **Responses:**
  - `200 OK` – Returns a new token and refresh token.
  - `401 Unauthorized` – If the refresh token is invalid.

---

### 3. Transactions

#### Make a Payment
- **Endpoint:** `POST /api/transactions/pay`
- **Description:** Makes a payment using a card.
- **Responses:**
  - `200 OK` – Payment successful.
  - `400 Bad Request` – If payment fails.

#### Get Card Transactions
- **Endpoint:** `GET /api/transactions/{cardId}`
- **Description:** Retrieves a list of transactions for a specific card.
- **Responses:**
  - `200 OK` – Returns the list of transactions.
  - `404 Not Found` – If no transactions exist for the card.

---

## Architectural Choices

### Why EF Core?
EF Core was chosen as the ORM for this project because:
- It provides **seamless integration** with .NET applications.
- Supports **migrations** for database versioning and schema updates.
- Making it easy to switch between database providers.

### Why CQRS?
CQRS (Command Query Responsibility Segregation) was implemented because:
- It **separates read and write operations**, improving scalability and maintainability. (There is good option to use 'dapper' for write operations)
- Enables **optimized queries** for read operations without affecting the write model.

### Why DDD (Domain-Driven Design)?
Domain-Driven Design was chosen for:
- **Encapsulation of business logic** within entities and aggregates.
- **Clear separation** between domain, application, and infrastructure layers.
- **Improved maintainability** and flexibility when extending the application.
- Helps ensure the **code reflects real-world business processes**.

---

## Authorization

Most endpoints require authorization. A valid **JWT token** must be provided in the `Authorization` header:
```sh
Authorization: Bearer <your_token_here>
```

---
