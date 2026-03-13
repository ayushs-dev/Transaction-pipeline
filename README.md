# Transactions Ingest Pipeline

## Overview

This project implements a transaction ingestion pipeline using a .NET 10 console application.  
It processes transaction snapshots, detects updates, handles revocations, and maintains an audit trail using SQLite and Entity Framework Core.

The system is designed to ensure idempotent processing and maintain transactional consistency.

---

## Architecture

The application processes transaction snapshots through the following pipeline:

JSON Snapshot  
↓  
SnapshotReader  
↓  
TransactionProcessor  
↓  
SQLite Database (EF Core)

Tables:

- Transactions
- TransactionAudits

---

## Features

- Ingest hourly transaction snapshots
- Insert new transactions
- Detect and update changed transactions
- Audit logging for field changes
- Revocation detection for missing transactions
- Finalization of transactions older than 24 hours
- Idempotent processing
- Automated tests using xUnit

---

## Project Structure


TransactionsIngest/
│
├── Data/
├── Infrastructure/
├── Models/
├── Services/
├── MockData/
├── Program.cs
├── appsettings.json
└── TransactionsIngest.csproj

TransactionsIngest.Tests/
└── BasicTests.cs





---

## Configuration

Application configuration is stored in:


appsettings.json



Example:

```json
{
  "Database": {
    "ConnectionString": "Data Source=transactions.db"
  },
  "Api": {
    "SnapshotPath": "MockData/transactions.json"
  }
}




How to Build

Install the .NET SDK (version 10 or later).

Then run:

dotnet restore
dotnet build
Run the Application
dotnet run --project TransactionsIngest
Run Tests
dotnet test
Assumptions

Transactions are uniquely identified by TransactionId.

Snapshots represent transactions from the last 24 hours.

Missing transactions within this window are marked as Revoked.

Transactions older than 24 hours become Finalized and are no longer updated.






