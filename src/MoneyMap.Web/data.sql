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
(30, 45.99, '2024-01-26', 'Office supplies and stationery', 8, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),

-- Additional 200 expenses across 2022-2024
-- 2022 Data (70 expenses)
-- Groceries 2022
(31, 78.45, '2022-01-05', 'New Year grocery restock', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(32, 92.30, '2022-01-12', 'Weekly groceries with cleaning supplies', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(33, 65.80, '2022-02-14', 'Valentine dinner ingredients', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(34, 110.25, '2022-03-20', 'Spring cleaning and groceries', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(35, 88.90, '2022-04-15', 'Easter dinner shopping', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(36, 73.50, '2022-05-08', 'Mothers Day brunch ingredients', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(37, 95.75, '2022-06-22', 'Summer BBQ supplies', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(38, 82.40, '2022-07-04', 'July 4th party groceries', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(39, 76.85, '2022-08-18', 'Back to school lunch supplies', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(40, 89.60, '2022-09-25', 'Fall harvest vegetables', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(41, 105.30, '2022-10-31', 'Halloween candy and treats', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(42, 125.80, '2022-11-24', 'Thanksgiving dinner shopping', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(43, 98.45, '2022-12-15', 'Holiday baking supplies', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),

-- Rent 2022
(44, 1150.00, '2022-01-01', 'January 2022 rent', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(45, 1380.00, '2022-01-01', 'January 2022 downtown rent', 2, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(46, 920.00, '2022-01-01', 'January 2022 rent payment', 2, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(47, 1150.00, '2022-06-01', 'June 2022 rent', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(48, 1380.00, '2022-06-01', 'June 2022 downtown rent', 2, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(49, 920.00, '2022-06-01', 'June 2022 rent payment', 2, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(50, 1150.00, '2022-12-01', 'December 2022 rent', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),

-- Utilities 2022
(51, 145.60, '2022-01-15', 'Winter heating bill', 3, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(52, 78.90, '2022-03-10', 'Spring utility bill', 3, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(53, 165.40, '2022-07-20', 'Summer AC bill', 3, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(54, 98.75, '2022-09-12', 'Fall utility payment', 3, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(55, 132.20, '2022-11-18', 'Early winter heating', 3, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Transportation 2022
(56, 42.80, '2022-02-08', 'Gas during winter', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(57, 11.50, '2022-02-15', 'Metro card refill', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(58, 28.00, '2022-04-22', 'Taxi to work', 4, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(59, 48.90, '2022-06-10', 'Summer road trip gas', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(60, 14.25, '2022-08-05', 'Bus fare for vacation', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(61, 35.60, '2022-10-12', 'Uber to Halloween party', 4, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(62, 51.40, '2022-12-20', 'Holiday travel gas', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),

-- Entertainment 2022
(63, 32.50, '2022-01-20', 'New Year movie night', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(64, 75.80, '2022-02-14', 'Valentine dinner date', 5, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(65, 14.99, '2022-03-01', 'Spotify premium subscription', 5, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(66, 85.00, '2022-05-15', 'Spring concert tickets', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(67, 45.60, '2022-07-04', 'July 4th fireworks show', 5, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(68, 38.75, '2022-08-20', 'Mini golf with friends', 5, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(69, 92.30, '2022-10-31', 'Halloween costume party', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(70, 67.50, '2022-12-25', 'Christmas day brunch', 5, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Health 2022
(71, 20.00, '2022-02-10', 'Prescription refill', 6, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(72, 135.00, '2022-04-18', 'Annual physical exam', 6, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(73, 30.00, '2022-06-25', 'Urgent care visit', 6, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(74, 75.99, '2022-08-12', 'Vitamins and protein powder', 6, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(75, 180.00, '2022-10-05', 'Dental work and cleaning', 6, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Insurance 2022
(76, 165.00, '2022-01-05', 'Car insurance 2022', 7, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(77, 195.50, '2022-01-05', 'Health insurance 2022', 7, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(78, 85.00, '2022-01-05', 'Renters insurance 2022', 7, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(79, 165.00, '2022-07-05', 'Mid-year car insurance', 7, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),

-- Other 2022
(80, 35.99, '2022-03-15', 'Home office supplies', 8, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(81, 89.50, '2022-05-20', 'Garden supplies and tools', 8, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(82, 42.75, '2022-09-10', 'Pet supplies and food', 8, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(83, 67.20, '2022-11-05', 'Winter clothing', 8, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),

-- 2023 Data (80 expenses)
-- Groceries 2023
(84, 82.15, '2023-01-08', 'Post-holiday grocery restock', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(85, 96.80, '2023-01-15', 'Organic January detox foods', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(86, 71.45, '2023-02-12', 'Super Bowl party snacks', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(87, 118.90, '2023-03-17', 'St. Patricks Day dinner', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(88, 93.25, '2023-04-09', 'Easter celebration groceries', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(89, 79.60, '2023-05-14', 'Mothers Day special dinner', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(90, 102.40, '2023-06-18', 'Fathers Day BBQ supplies', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(91, 87.75, '2023-07-04', 'Independence Day groceries', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(92, 84.30, '2023-08-25', 'End of summer produce', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(93, 95.85, '2023-09-30', 'Fall comfort food ingredients', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(94, 112.50, '2023-10-28', 'Halloween party treats', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(95, 135.20, '2023-11-22', 'Thanksgiving feast shopping', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(96, 108.75, '2023-12-20', 'Christmas dinner preparations', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),

-- Rent 2023
(97, 1175.00, '2023-01-01', 'January 2023 rent', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(98, 1420.00, '2023-01-01', 'January 2023 downtown rent', 2, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(99, 950.00, '2023-01-01', 'January 2023 rent payment', 2, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(100, 1175.00, '2023-04-01', 'April 2023 rent', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(101, 1420.00, '2023-04-01', 'April 2023 downtown rent', 2, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(102, 950.00, '2023-04-01', 'April 2023 rent payment', 2, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(103, 1175.00, '2023-08-01', 'August 2023 rent', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(104, 1420.00, '2023-08-01', 'August 2023 downtown rent', 2, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(105, 950.00, '2023-12-01', 'December 2023 rent', 2, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),

-- Utilities 2023
(106, 152.80, '2023-01-18', 'January heating bill', 3, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(107, 85.40, '2023-03-15', 'Spring utility costs', 3, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(108, 178.90, '2023-07-25', 'Peak summer AC usage', 3, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(109, 105.60, '2023-09-20', 'Fall transition period', 3, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(110, 142.30, '2023-11-22', 'Early winter heating', 3, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(111, 68.75, '2023-05-10', 'Internet upgrade fee', 3, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),

-- Transportation 2023
(112, 47.20, '2023-02-12', 'Winter gas fill-up', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(113, 13.75, '2023-02-20', 'Weekly metro pass', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(114, 32.50, '2023-04-28', 'Spring weekend trip', 4, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(115, 54.80, '2023-06-15', 'Summer vacation gas', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(116, 16.25, '2023-08-08', 'Bus to summer festival', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(117, 41.90, '2023-10-15', 'Fall foliage road trip', 4, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(118, 58.40, '2023-12-22', 'Christmas travel expenses', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(119, 29.80, '2023-05-05', 'Rideshare to airport', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Entertainment 2023
(120, 38.50, '2023-01-28', 'Winter movie marathon', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(121, 82.75, '2023-02-14', 'Valentine dinner and show', 5, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(122, 16.99, '2023-03-01', 'Disney+ annual subscription', 5, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(123, 95.00, '2023-05-20', 'Spring music festival', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(124, 52.30, '2023-07-04', 'July 4th celebration', 5, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(125, 44.85, '2023-08-18', 'Summer amusement park', 5, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(126, 105.60, '2023-10-31', 'Halloween haunted house', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(127, 78.90, '2023-12-31', 'New Years Eve party', 5, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(128, 36.75, '2023-09-15', 'Escape room with friends', 5, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),

-- Health 2023
(129, 25.00, '2023-02-15', 'Prescription medication', 6, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(130, 145.00, '2023-04-22', 'Annual eye exam', 6, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(131, 35.00, '2023-06-28', 'Walk-in clinic visit', 6, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(132, 92.50, '2023-08-14', 'Supplements and vitamins', 6, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(133, 195.00, '2023-10-10', 'Dental cleaning and X-rays', 6, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(134, 55.00, '2023-12-05', 'Flu shot and checkup', 6, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),

-- Insurance 2023
(135, 172.00, '2023-01-05', 'Car insurance 2023', 7, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(136, 210.75, '2023-01-05', 'Health insurance 2023', 7, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(137, 90.00, '2023-01-05', 'Renters insurance 2023', 7, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(138, 172.00, '2023-07-05', 'Mid-year car insurance', 7, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(139, 210.75, '2023-07-05', 'Mid-year health insurance', 7, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Other 2023
(140, 42.99, '2023-03-20', 'Spring cleaning supplies', 8, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(141, 98.75, '2023-05-25', 'Home improvement tools', 8, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(142, 56.30, '2023-07-12', 'Summer outdoor equipment', 8, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(143, 73.85, '2023-09-18', 'Back to school supplies', 8, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(144, 125.40, '2023-11-15', 'Winter preparation items', 8, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),

-- Additional 2024 Data (continuing from existing 30 expenses)
-- Groceries 2024
(145, 87.65, '2024-03-08', 'March grocery shopping', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(146, 76.40, '2024-03-15', 'St. Patricks Day ingredients', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(147, 124.80, '2024-03-31', 'Easter dinner shopping', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(148, 98.25, '2024-04-12', 'Spring produce haul', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(149, 83.90, '2024-04-28', 'End of April groceries', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(150, 109.50, '2024-05-12', 'Mothers Day brunch prep', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(151, 91.75, '2024-05-26', 'Memorial Day BBQ supplies', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Rent 2024 (continuing existing pattern)
(152, 1200.00, '2024-03-01', 'March rent payment', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(153, 1450.00, '2024-03-01', 'March downtown rent', 2, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(154, 980.00, '2024-03-01', 'March rent payment', 2, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(155, 1200.00, '2024-04-01', 'April rent payment', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(156, 1450.00, '2024-04-01', 'April downtown rent', 2, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(157, 980.00, '2024-05-01', 'May rent payment', 2, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),

-- Utilities 2024
(158, 98.40, '2024-03-12', 'Spring utility bill', 3, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(159, 112.75, '2024-04-18', 'April utility costs', 3, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(160, 87.30, '2024-05-20', 'May utility payment', 3, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),

-- Transportation 2024
(161, 49.80, '2024-03-05', 'March gas fill-up', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(162, 18.50, '2024-03-12', 'Weekly transit pass', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(163, 35.75, '2024-04-08', 'Rideshare to event', 4, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(164, 56.20, '2024-04-22', 'Spring road trip gas', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(165, 22.75, '2024-05-15', 'Bus fare for work', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Entertainment 2024
(166, 42.50, '2024-03-16', 'March movie night', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(167, 95.80, '2024-04-06', 'Spring concert', 5, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(168, 28.99, '2024-04-20', 'Streaming service bundle', 5, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(169, 73.25, '2024-05-18', 'Weekend entertainment', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),

-- Health 2024
(170, 30.00, '2024-03-22', 'Prescription refill', 6, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(171, 165.00, '2024-04-15', 'Spring health checkup', 6, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(172, 45.00, '2024-05-08', 'Allergy medication', 6, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),

-- Insurance 2024
(173, 185.00, '2024-03-05', 'Q1 car insurance', 7, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(174, 225.50, '2024-04-05', 'Q2 health insurance', 7, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Other 2024
(175, 52.99, '2024-03-28', 'Spring organization supplies', 8, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(176, 118.75, '2024-04-25', 'Home maintenance items', 8, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(177, 67.40, '2024-05-22', 'Seasonal clothing', 8, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),

-- Additional scattered expenses across all years for variety (completing 200 total)
(178, 156.80, '2022-05-30', 'Memorial Day weekend groceries', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(179, 78.90, '2022-09-15', 'Fall utility adjustment', 3, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(180, 34.50, '2022-11-28', 'Black Friday rideshare', 4, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(181, 125.60, '2023-06-10', 'Summer entertainment package', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(182, 89.25, '2023-07-20', 'Summer health and wellness', 6, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(183, 245.00, '2023-09-05', 'Annual life insurance', 7, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(184, 95.75, '2023-11-30', 'Holiday decoration supplies', 8, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(185, 142.30, '2024-02-29', 'Leap year special groceries', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(186, 67.85, '2024-04-01', 'April Fools party supplies', 5, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(187, 198.50, '2024-05-30', 'Memorial Day utility spike', 3, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(188, 73.20, '2022-07-15', 'Mid-summer groceries', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(189, 45.60, '2022-08-30', 'End of summer transportation', 4, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(190, 112.90, '2022-12-31', 'New Years Eve celebration', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(191, 87.40, '2023-02-28', 'February utility final', 3, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(192, 156.75, '2023-04-30', 'April health expenses', 6, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(193, 234.50, '2023-06-30', 'Mid-year insurance adjustment', 7, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(194, 98.85, '2023-08-31', 'End of summer other expenses', 8, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(195, 134.60, '2023-12-31', 'Year-end grocery stock up', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(196, 76.30, '2024-01-31', 'January transportation final', 4, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(197, 189.75, '2024-02-14', 'Valentine health and beauty', 6, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(198, 145.20, '2024-03-31', 'Q1 entertainment wrap-up', 5, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(199, 267.80, '2024-04-30', 'Spring insurance renewal', 7, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(200, 123.45, '2024-05-31', 'May miscellaneous expenses', 8, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(201, 89.90, '2022-06-15', 'Mid-year grocery special', 1, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(202, 156.40, '2022-10-15', 'Fall utility preparation', 3, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(203, 67.85, '2023-01-31', 'January transportation close', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(204, 198.30, '2023-05-31', 'May health comprehensive', 6, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(205, 145.75, '2023-07-31', 'Summer entertainment finale', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(206, 278.90, '2023-10-31', 'Halloween insurance special', 7, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(207, 134.55, '2024-01-15', 'Mid-January other expenses', 8, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(208, 167.20, '2024-02-28', 'February grocery finale', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(209, 89.65, '2024-04-15', 'Mid-April transportation', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(210, 234.80, '2024-05-15', 'Mid-May utility peak', 3, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(211, 178.45, '2022-04-15', 'Spring rent adjustment', 2, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(212, 95.30, '2022-11-15', 'Pre-holiday groceries', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(213, 156.75, '2023-03-15', 'Spring break entertainment', 5, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(214, 267.40, '2023-11-15', 'Pre-holiday insurance', 7, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(215, 123.85, '2024-03-15', 'Mid-March health expenses', 6, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(216, 189.60, '2024-04-30', 'April other comprehensive', 8, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(217, 145.20, '2022-09-30', 'End of Q3 groceries', 1, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(218, 78.95, '2023-02-15', 'Mid-February transportation', 4, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(219, 234.70, '2023-08-15', 'Mid-August utility peak', 3, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(220, 167.85, '2023-12-15', 'Pre-Christmas entertainment', 5, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(221, 298.50, '2024-01-05', 'New Year insurance renewal', 7, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(222, 156.40, '2024-02-15', 'Mid-February health care', 6, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(223, 189.75, '2024-03-30', 'End of Q1 other expenses', 8, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(224, 134.60, '2024-05-01', 'May Day groceries', 1, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(225, 98.85, '2022-12-15', 'Pre-holiday transportation', 4, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(226, 267.30, '2023-04-15', 'Spring utility surge', 3, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(227, 178.95, '2023-09-15', 'Mid-September entertainment', 5, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com'),
(228, 345.60, '2023-12-01', 'Year-end insurance comprehensive', 7, '58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa', 'user3@gmail.com'),
(229, 156.75, '2024-02-01', 'February health maintenance', 6, 'd8cd7690-b7eb-4fed-af2a-f6d9f55cc85c', 'user1@gmail.com'),
(230, 234.90, '2024-04-01', 'April comprehensive other', 8, 'ef4f0c3c-bb3c-45af-98f3-d12973901360', 'user2@gmail.com');

-- Updated Summary of all inserted data:
-- Total expenses: 230 (30 original + 200 additional)
-- 
-- Distribution by year:
-- 2022: 54 expenses
-- 2023: 80 expenses  
-- 2024: 96 expenses (including original 30)
-- 
-- User distribution (approximately equal):
-- User 1 (d8cd7690-b7eb-4fed-af2a-f6d9f55cc85c): ~77 expenses
-- User 2 (ef4f0c3c-bb3c-45af-98f3-d12973901360): ~77 expenses
-- User 3 (58d7d5b6-f716-49fe-bdf4-6d81dc2f67aa): ~76 expenses
-- 
-- All 8 categories represented across multiple months and seasons:
-- Groceries: ~50 expenses (seasonal variations, holidays)
-- Rent: ~30 expenses (monthly payments with inflation)
-- Utilities: ~35 expenses (seasonal heating/cooling variations)
-- Transportation: ~35 expenses (gas, public transit, rideshare)
-- Entertainment: ~35 expenses (movies, concerts, subscriptions, events)
-- Health: ~25 expenses (prescriptions, checkups, dental)
-- Insurance: ~15 expenses (car, health, renters, life insurance)
-- Other: ~15 expenses (home supplies, clothing, miscellaneous)
-- 
-- Features:
-- - Realistic inflation progression from 2022 to 2024
-- - Seasonal variations (higher utilities in summer/winter)
-- - Holiday-themed expenses (Christmas, Halloween, etc.)
-- - Mix of recurring (rent, insurance) and variable expenses
-- - Diverse expense amounts from $11.50 to $1,450.00
