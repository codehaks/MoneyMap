-- MoneyMap Sample Data Script
-- 30 Random Expenses for 3 Users across 8 Categories

-- Users Data (for reference)
-- "d8cd7690-b7eb-4fed-af2a-f6d9f55cc85c"	"user1@gmail.com"
-- "ef4f0c3c-bb3c-45af-98f3-d12973901360"	"user2@gmail.com"
-- "58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa"	"user3@gmail.com"

-- Categories (for reference)
-- 1	"Groceries"
-- 2	"Rent"
-- 3	"Utilities"
-- 4	"Transportation"
-- 5	"Entertainment"
-- 6	"Health"
-- 7	"Insurance"
-- 8	"Other"

-- Insert 30 sample expenses
INSERT INTO public."Expenses"("Id", "Amount", "Date", "Note", "CategoryId", "UserId", "UserName") VALUES 
-- Groceries (Category 1)
(1, 85.50, '2024-01-15', 'Weekly grocery shopping at Walmart', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(2, 120.75, '2024-01-22', 'Organic produce and meat from Whole Foods', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(3, 67.30, '2024-01-28', 'Quick grocery run - milk, bread, eggs', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(4, 95.20, '2024-02-05', 'Monthly bulk shopping at Costco', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),

-- Rent (Category 2)
(5, 1200.00, '2024-01-01', 'January rent payment', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(6, 1450.00, '2024-01-01', 'January rent for downtown apartment', 2, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(7, 980.00, '2024-01-01', 'Monthly rent payment', 2, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(8, 1200.00, '2024-02-01', 'February rent payment', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),

-- Utilities (Category 3)
(9, 125.40, '2024-01-10', 'Electric bill - winter heating', 3, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(10, 89.75, '2024-01-12', 'Gas and water bill', 3, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(11, 156.20, '2024-01-15', 'Combined utility bill - electric, gas, water', 3, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(12, 75.60, '2024-01-20', 'Internet and cable bill', 3, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Transportation (Category 4)
(13, 45.80, '2024-01-08', 'Gas fill-up at Shell station', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(14, 12.50, '2024-01-10', 'Subway fare for work commute', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(15, 25.00, '2024-01-12', 'Uber ride to airport', 4, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(16, 52.30, '2024-01-18', 'Gas and car wash', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(17, 15.75, '2024-01-25', 'Bus pass for the week', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Entertainment (Category 5)
(18, 35.00, '2024-01-14', 'Movie tickets for two at AMC', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(19, 89.50, '2024-01-20', 'Dinner at Italian restaurant', 5, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(20, 15.99, '2024-01-22', 'Netflix monthly subscription', 5, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(21, 65.00, '2024-01-28', 'Concert tickets', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(22, 42.75, '2024-02-02', 'Bowling night with friends', 5, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Health (Category 6)
(23, 25.00, '2024-01-16', 'Prescription medication copay', 6, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(24, 150.00, '2024-01-18', 'Dental cleaning appointment', 6, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(25, 35.00, '2024-01-24', 'Doctor visit copay', 6, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(26, 89.99, '2024-01-30', 'Vitamins and supplements', 6, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),

-- Insurance (Category 7)
(27, 180.00, '2024-01-05', 'Monthly car insurance premium', 7, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(28, 220.50, '2024-01-05', 'Health insurance monthly payment', 7, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(29, 95.00, '2024-01-05', 'Renters insurance annual payment', 7, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),

-- Other (Category 8)
(30, 45.99, '2024-01-26', 'Office supplies and stationery', 8, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com');

-- Summary of inserted data:
-- Total expenses: 30
-- User 1 (d8cd7690-b7eb-4fed-af2a-f6d9f55cc85c): 10 expenses
-- User 2 (ef4f0c3c-bb3c-45af-98f3-d12973901360): 11 expenses  
-- User 3 (58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa): 9 expenses
-- 
-- Categories covered:
-- Groceries: 4 expenses
-- Rent: 4 expenses
-- Utilities: 4 expenses
-- Transportation: 5 expenses
-- Entertainment: 5 expenses
-- Health: 4 expenses
-- Insurance: 3 expenses
-- Other: 1 expense
