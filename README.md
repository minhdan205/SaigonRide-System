# SaigonRide-System
Final Project for Software Engineering: Saigon Ride

## Project Overview

SaigonRide is a smart urban vehicle rental management system developed for Software Engineering Final Project.

The system allows users to:
- Register and log into the system
- View available vehicles
- Rent and return vehicles
- Make simulated digital payments
- Submit support requests
- View rental history

The system also provides an administrator dashboard for:
- Managing stations
- Managing vehicles
- Managing user accounts
- Managing support reports
- Viewing rental reports
- Creating discount codes

---

# Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap 5
- Session-based Authentication
- GitHub Version Control

---

# Main Features

## User Features

- User Registration & Login
- Password Recovery
- View Available Vehicles
- Vehicle Rental & Return
- Rental History
- Support Ticket System
- Discount Code Application
- Simulated MoMo QR Payment
- Printable Rental Receipt

## Admin Features

- Manage Stations
- Manage Vehicles
- Manage User Accounts
- Manage Discount Codes
- Manage Support Reports
- View Revenue Reports
- View Station Inventory Reports
- View Rental History Reports

---

# Simulated MoMo QR Payment

The system includes a simulated MoMo QR payment feature.

Users can:
- Select MoMo as the payment method
- View a generated QR code containing payment information
- Simulate payment completion
- Receive a digital receipt after payment

This feature is implemented for demonstration and educational purposes only.

---

# Database Setup

## Step 1: Open the Project

Open the solution file in Visual Studio.

```bash
SaigonRideSystem.sln
```

---

## Step 2: Configure SQL Server Connection

Open:

```txt
appsettings.json
```

Update the connection string if necessary.

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=SaigonRideDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## Step 3: Run Database Migration

Open Package Manager Console and run:

```powershell
Update-Database
```

---

## Step 4: Run the Project

Press:

```txt
Ctrl + F5
```

or click:

```txt
Start Without Debugging
```

---

# Demo Accounts

## Admin Account

```txt
Email: admin@saigonride.com
Password: admin123
```

## User Account

```txt
Email: user@saigonride.com
Password: user123
```

You may register a new user account directly from the Sign Up page.

---

# Project Structure

```txt
/Controllers
/Models
/Views
/Data
/Services
/wwwroot
```

---

# Important Notes

- The payment feature is a simulation and does not process real financial transactions.
- The project is developed for academic purposes only.
- Internet connection may be required for QR image generation.

---

# GitHub Repository

https://github.com/minhdan205/SaigonRide-System

---

# Team Members

- Nguyen Minh Dan - 42300269
- Hong Bao Nhi - 523V0005
- Phan Nguyen Thao Nhu - 524V0007

---

# AI Usage Declaration

AI tools were used to assist with:
- Code explanation
- Debugging support
- UI improvement suggestions
- Documentation assistance

All generated code and documentation were reviewed and modified by the project team before submission.

---

# License

This project is for educational purposes only.
