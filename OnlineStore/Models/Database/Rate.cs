﻿namespace OnlineStore.Models.Database;

public class Rate
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public User User { get; set; }
    public Product Product { get; set; }
    public int Score { get; set; }
}