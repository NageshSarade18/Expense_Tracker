using Expense_Tracker.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Expense_Tracker.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly ApplicationDBContext _context;
        public DashBoardController(ApplicationDBContext context)
        {
            _context = context;


        }
        public async Task<ActionResult> Index()
        {
            //Last 7 days transaction
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;
            List<Expense_Tracker.Models.Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.category)  // Ensure lowercase 'category'
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();  // Use ToListAsync() for better flexibility
                                 // Changed `ToArrayAsync()` to `ToListAsync()` for better flexibility


            //Total Income
            int TotalIncome = SelectedTransactions
                .Where(i => i.category.Type == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");





            //Total Expense
            int TotalExpense = SelectedTransactions
                .Where(i => i.category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");




            //Balance
            int Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("C0");


            //DoNut Chart - Expense By Category
            ViewBag.DoNutChartData = SelectedTransactions
                .Where(i => i.category.Type == "Expense")
                .GroupBy(j => j.category.categoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon=k.First().category.Icon+" "+ k.First().category.Title,
                    amount =k.Sum(j=>j.Amount),
                    formattedAmount=k.Sum(j=>j.Amount).ToString("C0"),

                }).OrderByDescending(l=>l.amount).ToList();



            //spline Chart - Income VS Expense

            //income
            List<SplineChartData> IncomeSummary = new List<SplineChartData>();

            IncomeSummary = SelectedTransactions
         .Where(i => i.category != null && i.category.Type == "Income") // Prevents NullReferenceException
         .GroupBy(j => j.Date.Date) // Groups by date only, ignoring time
         .Select(k => new SplineChartData()
         {
             day = k.Min(d => d.Date).ToString("dd-MMM"), // Safely gets the earliest date
             income = k.Sum(l => l.Amount) // Sum of all amounts for the day
         })
         .ToList();


            //Expense
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.category.Type == "Expense")
                .GroupBy(j => j.Date.Date)  // Ensure date-only grouping
                .Select(k => new SplineChartData()
                {
                    day = k.Key.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount),
                    income = 0  // Ensure income field exists
                })
                .ToList();


            //Combine Income and Expense
            DateTime startDate = DateTime.Today.AddDays(-6); // Start from 6 days ago, including today
            string[] LastyDays = Enumerable.Range(0, 7)
                .Select(i => startDate.AddDays(i).ToString("dd-MMM")) // Match format
                .ToArray();

            ViewBag.SplineChartData = (from day in LastyDays
                                       join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                       from income in dayIncomeJoined.DefaultIfEmpty(new SplineChartData { income = 0 }) // Avoid null
                                       join expense in ExpenseSummary on day equals expense.day into DayExpenseJoined
                                       from expense in DayExpenseJoined.DefaultIfEmpty(new SplineChartData { expense = 0 }) // Avoid null
                                       select new SplineChartData
                                       {
                                           day = day,
                                           income = income.income, // No need for null check now
                                           expense = expense.expense
                                       }).ToList();


            //Recent Transactions
            ViewBag.Recenttransactions = await _context.Transactions
                .Include(i => i.category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
    public class SplineChartData
    {
        public string day { get; set; }
        public int income { get; set; }
        public int expense { get; set; }
    }
}
