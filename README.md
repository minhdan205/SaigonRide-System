# SaigonRide System

## 1. Project Information

**Project Name:** SaigonRide Smart Vehicle Rental System  
**Course:** Software Engineering  
**Final Project:** SaigonRide - Distributed Vehicle Rental System  
**Technology Tier:** Tier 3 - ASP.NET Core MVC Code First + Entity Framework Core  
**Repository:** https://github.com/minhdan205/SaigonRide-System.git  
**Video Link:** https://drive.google.com/file/d/1BY6xiGvuYxPT3oZ-gZ4_f8voyfO6a6XB/view?usp=sharing

SaigonRide is a smart vehicle rental management system developed for the Software Engineering Final Project. The system is designed to support a distributed network of public bicycles and electric scooters in Ho Chi Minh City. It allows users to rent vehicles from one station and return them to another station, while administrators can manage stations, vehicles, users, discount codes, reports, and support requests.

The system supports both Local Commuters and Foreign Tourists. Local users can use payment methods such as Cash, MoMo, and VNPay, while foreign tourists can use Cash, Apple Pay, and PayPal. The project also implements dynamic pricing based on vehicle category, a 15% discount for returning vehicles to low-inventory stations, discount code application, rental history tracking, simulated MoMo QR payment, support ticket handling, and administrative reports.

---

## 2. Team Members

| No. | Student ID | Full Name | Role / Contribution |
|---|---|---|---|
| 1 | 42300269 | Nguyễn Minh Dân | Business Analyst / Requirements Lead, database design and implementation, Manage Stations module, backend logic, rental workflow, discount logic, reports, unit testing, documentation, GitHub management |
| 2 | 523V0005 | Hồng Bảo Nhi | QA / Technical Lead, Manage Users module, non-functional requirements, testing support, user account validation, revenue and rental report support |
| 3 | 524V0007 | Phan Nguyễn Thảo Như | Project Manager / Team Leader, Manage Vehicles module, project planning, Agile Scrum timeline, vehicle management workflow, UI/UX support |

---

## 3. Technologies Used

### Programming Languages

- C#
- HTML
- Razor
- SQL
- JavaScript

### Frameworks and Platforms

- ASP.NET Core MVC
- .NET 8.0
- Entity Framework Core
- SQL Server LocalDB
- Bootstrap 5
- xUnit Test Framework

### Tools

- Visual Studio
- SQL Server Object Explorer
- NuGet Package Manager
- GitHub
- Git
- Figma
- Microsoft Word / PowerPoint

---

## 4. Solution Structure

The solution contains the following main projects and folders:

```text
SaigonRideSystem
│
├── SaigonRideSystem
│   ├── Controllers
│   │   ├── AccountController.cs
│   │   ├── StationController.cs
│   │   ├── VehicleController.cs
│   │   ├── UserController.cs
│   │   ├── RentalController.cs
│   │   ├── ReportController.cs
│   │   ├── DiscountCodeController.cs
│   │   └── SupportController.cs
│   │
│   ├── Models
│   │   ├── User.cs
│   │   ├── Station.cs
│   │   ├── Vehicle.cs
│   │   ├── Rental.cs
│   │   ├── Payment.cs
│   │   ├── DiscountCode.cs
│   │   ├── SupportTicket.cs
│   │   └── ViewModels / Helper Models
│   │
│   ├── Views
│   │   ├── Account
│   │   ├── Station
│   │   ├── Vehicle
│   │   ├── User
│   │   ├── Rental
│   │   ├── Report
│   │   ├── DiscountCode
│   │   ├── Support
│   │   └── Shared
│   │
│   ├── Data
│   │   └── ApplicationDbContext.cs
│   │
│   ├── Services
│   │   ├── PricingService.cs
│   │   ├── PricingResult.cs
│   │   └── PasswordHelper.cs
│   │
│   ├── wwwroot
│   ├── appsettings.json
│   └── Program.cs
│
└── SaigonRideSystem.Tests
    └── PricingServiceTests.cs
```

---

## 5. Main Features

### 5.1 Authentication and User Management

The system provides user registration, login, logout, forgot password, and role-based navigation. After login, users are redirected to different interfaces depending on their role.

Admin users can access administrative functions, while Local Commuters and Foreign Tourists can access rental, payment, receipt, support, and rental history functions.

The system separates user accounts into Admin Accounts and User Accounts. User Accounts are further divided into Local Users and Tourist Users for easier account management.

### 5.2 Station Management

Admins can create, view, update, and delete station information. Each station stores station name, location, capacity, current inventory, and status.

Station inventory is used by the rental return process to determine whether a 15% low-inventory discount should be applied.

### 5.3 Vehicle Management

Admins can manage vehicles by creating, viewing, updating, and deleting vehicle records. Each vehicle has a vehicle ID, category, status, and assigned station.

Vehicle statuses include:

```text
Available
InTransit
Maintenance
```

When a vehicle is currently rented and marked as InTransit, the Vehicle Details page displays active rental information, including rental code, renter information, start station, start time, and rental duration.

### 5.4 Rental Workflow

Users can view available vehicles and rent a selected vehicle. When a rental starts, the system records the rental start time, creates a rental transaction, assigns a rental code, changes the vehicle status to InTransit, and updates station inventory.

Each rental transaction has a unique rental code using the following format:

```text
Rent.No001
Rent.No002
Rent.No003
```

Users can return vehicles to a selected station. The system then calculates the fare, applies station discount if eligible, applies discount code if entered, updates the vehicle status, updates the station inventory, and redirects the user to the payment page.

### 5.5 Pricing and Discount Logic

The system calculates rental fares based on rental duration and vehicle category.

```text
Standard Bike: 500 VND per minute
E-Scooter: 1,500 VND per minute
```

If a user returns a vehicle to a station with current inventory below 20% of capacity, the system applies a 15% discount to the fare.

The system also supports admin-created discount codes. Discount codes can have one of the following discount levels:

```text
30%
50%
70%
100%
```

Users can enter a discount code before confirming the return vehicle process.

### 5.6 Simulated Payment

The system supports simulated payment methods based on user type.

Local Commuters can pay using:

```text
Cash
MoMo
VNPay
```

Foreign Tourists can pay using:

```text
Cash
Apple Pay
PayPal
```

For MoMo payment, the system provides a simulated MoMo QR payment page. The QR code contains rental information, rental code, vehicle ID, and payment amount. After the simulated payment is completed, the user clicks the Done Payment button and the system records the payment as Paid.

This payment feature is implemented for academic demonstration purposes only and does not process real financial transactions.

### 5.7 Rental Receipt

After payment, the system generates a rental receipt. The receipt includes invoice information, customer information, rental details, fare summary, discount information, and payment information.

The receipt page also supports printing through the browser print function.

### 5.8 Rental History

Users can view their own rental history, including rental code, vehicle ID, start station, return station, start time, end time, payment status, and total fare.

Admins can view all rental history records in the system through the Rental History Report.

### 5.9 Support Ticket System

Users can submit support requests for issues such as traffic accidents, technical problems, full stations, or other problems. If the issue is reported during an active rental, the support request includes rental code, vehicle ID, and current location.

Admins can view all support reports, review issue details, and respond to users.

### 5.10 Reports

The system provides administrative reports, including:

```text
Revenue Report
Station Inventory Report
Rental History Report
```

The Revenue Report shows revenue grouped by vehicle category. The Station Inventory Report displays station capacity, current inventory, utilization rate, and low-inventory status. The Rental History Report provides transaction-level traceability for all rental records.

---

## 6. Database Design

The system uses Entity Framework Core Code First to generate and manage the database schema.

Main entities include:

```text
User
Station
Vehicle
Rental
Payment
DiscountCode
SupportTicket
```

Important relationships include:

```text
User 1 - Many Rentals
Vehicle 1 - Many Rentals
Station 1 - Many Vehicles
Station 1 - Many Start Rentals
Station 1 - Many Return Rentals
Rental 1 - 1 Payment
Rental Many - 1 DiscountCode
User 1 - Many SupportTickets
Rental 1 - Many SupportTickets
```

The database is created and updated using Entity Framework Core migrations.

---

## 7. Setup Instructions

### Step 1: Clone the Repository

```bash
git clone https://github.com/minhdan205/SaigonRide-System.git
```

Open the project folder:

```bash
cd SaigonRide-System
```

---

### Step 2: Open the Solution

Open the solution file in Visual Studio:

```text
SaigonRideSystem.sln
```

---

### Step 3: Restore NuGet Packages

Visual Studio usually restores packages automatically. If needed, right-click the solution and select:

```text
Restore NuGet Packages
```

---

### Step 4: Check the Connection String

Open:

```text
appsettings.json
```

Default LocalDB connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SaigonRideDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

If your SQL Server configuration is different, update the connection string before running migrations.

---

### Step 5: Apply Database Migration

Open Package Manager Console in Visual Studio:

```text
Tools > NuGet Package Manager > Package Manager Console
```

Run:

```powershell
Update-Database
```

This command creates the SaigonRideDB database and required tables.

---

### Step 6: Run the Application

Run the project using:

```text
Ctrl + F5
```

or click:

```text
Start Without Debugging
```

The application will open in the browser.

---

## 8. Demo Accounts

### Admin Account

```text
Email: admin@saigonride.com
Password: admin123
```

The admin account is seeded automatically when the application starts.

### User Account

Users can create a new account through the Sign Up page.

Example local user:

```text
Full Name: Nguyen Van A
Email: user@gmail.com
Password: 123456
Phone Number: 0909123456
Country: Vietnam
```

Example tourist user:

```text
Full Name: John Smith
Email: john@gmail.com
Password: 123456
Phone Number: 123456789
Country: USA
Passport Number: P123456
```

---

## 9. How to Use the System

### Admin Flow

```text
1. Open the system.
2. Log in using the admin account.
3. Go to the Admin Home page.
4. Manage Stations, Vehicles, Users, Discount Codes, Support Reports, and Reports.
5. View Revenue Report, Station Inventory Report, and Rental History Report.
```

### User Flow

```text
1. Register a new user account or log in with an existing account.
2. View available vehicles.
3. Rent a vehicle.
4. View active rental.
5. Submit support request if needed.
6. Return the vehicle.
7. Apply discount code if available.
8. Select payment method.
9. Complete simulated payment.
10. View and print rental receipt.
11. Review rental history.
```

---

## 10. Testing

The project includes a separate xUnit test project:

```text
SaigonRideSystem.Tests
```

The automated tests focus on the core pricing and discount logic implemented in PricingService.

Tested scenarios include:

```text
Standard Bike fare calculation at 500 VND/min
E-Scooter fare calculation at 1,500 VND/min
15% discount when station inventory is below 20%
No discount when station inventory equals 20%
No discount when station inventory is above 20%
Invalid duration handling
Invalid station capacity handling
Invalid inventory handling
```

To run tests:

```text
Test > Test Explorer > Run All Tests
```

or use the command line:

```bash
dotnet test
```

---

## 11. Simulated MoMo QR Payment

The system includes a simulated MoMo QR payment feature. When the user selects MoMo as the payment method, the system displays a QR code containing rental code, vehicle ID, and payment amount.

This feature is used for demonstration purposes and does not integrate with the real MoMo payment gateway. After scanning or reviewing the QR information, the user clicks Done Payment to simulate successful payment.

---

## 12. Deployment

If deployed, add the live cloud URL here:

```text
Live Cloud URL: Add your deployed link here
```

Recommended deployment option:

```text
Azure App Service + Azure SQL Database
```

For local submission, the project can be executed through Visual Studio using SQL Server LocalDB.

---

## 13. GitHub Repository

```text
https://github.com/minhdan205/SaigonRide-System.git
```

The repository contains the full source code, commit history, and test project. The project follows meaningful GitHub commit practices to document development progress.

---

## 14. Notes

This project was developed for academic purposes as part of the Software Engineering Final Project. Some external service-related features, such as MoMo QR payment, are simulated to demonstrate the intended workflow without processing real financial transactions.

---

## 15. AI Usage Declaration

Generative AI tools were used to support debugging, code explanation, documentation refinement, and report writing assistance. All AI-generated suggestions were reviewed, modified, and verified by the team before being included in the final project.

The project team remains responsible for the final source code, documentation, testing, and system behavior.

---

## 16. License

This project is developed for educational purposes only.
