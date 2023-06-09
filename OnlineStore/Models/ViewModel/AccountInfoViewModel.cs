﻿using System;

namespace OnlineStore.Models.ViewModel;

public class AccountInfoViewModel
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsModerator { get; set; }
    public bool IsAdmin { get; set; }
}