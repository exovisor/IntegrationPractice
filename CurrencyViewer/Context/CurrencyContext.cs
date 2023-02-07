using CurrencyViewer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace CurrencyViewer.Context
{
    internal class CurrencyContext : DbContext
    {
        public CurrencyContext()
            : base("DBConnection") //TODO: Add connection
        { }

        public DbSet<FavoriteCurrency> FavoriteCurrencies { get; set; }
    }
}
