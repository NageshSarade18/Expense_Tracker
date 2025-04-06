-- Step 1: Create the Database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TransactionDB')
BEGIN
    CREATE DATABASE TransactionDB;
END
GO

-- Step 2: Switch to the TransactionDB database
USE TransactionDB;
GO

-- Step 3: Drop existing indexes
IF EXISTS (SELECT name FROM sys.indexes WHERE name = 'idx_Transactions_CategoryId')
    DROP INDEX idx_Transactions_CategoryId ON Transactions;
IF EXISTS (SELECT name FROM sys.indexes WHERE name = 'idx_Transactions_Date')
    DROP INDEX idx_Transactions_Date ON Transactions;
GO

-- Step 4: Drop existing tables
IF OBJECT_ID('Transactions', 'U') IS NOT NULL DROP TABLE Transactions;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
GO

-- Step 5: Create the Categories table
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    Icon NVARCHAR(255) NOT NULL,
    Type NVARCHAR(20) CHECK (Type IN ('Income', 'Expense')) NOT NULL
);
GO

-- Step 6: Create the Transactions table
CREATE TABLE Transactions (
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL,
    Amount INT CHECK (Amount > 0) NOT NULL,
    Note NVARCHAR(255),
    Date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId) ON DELETE CASCADE
);
GO

-- Step 7: Create indexes again
CREATE INDEX idx_Transactions_CategoryId ON Transactions(CategoryId);
CREATE INDEX idx_Transactions_Date ON Transactions(Date);
GO

