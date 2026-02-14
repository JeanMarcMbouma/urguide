#!/usr/bin/env dotnet-script

#r "nuget: Microsoft.AspNetCore.Identity.EntityFrameworkCore, 8.0.0"
#r "nuget: Microsoft.EntityFrameworkCore.SqlServer, 8.0.0"

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

// Simple admin user provisioning script
// Usage: dotnet script provision-admin.csx [email] [password]

var email = Args.Count > 0 ? Args[0] : "admin@urguide.local";
var password = Args.Count > 1 ? Args[1] : "Admin123!";
var firstName = Args.Count > 2 ? Args[2] : "Admin";
var lastName = Args.Count > 3 ? Args[3] : "User";

Console.WriteLine("=================================================");
Console.WriteLine("  UrGuide Admin User Provisioning");
Console.WriteLine("=================================================");
Console.WriteLine();
Console.WriteLine($"Email: {email}");
Console.WriteLine($"Password: {password}");
Console.WriteLine($"Name: {firstName} {lastName}");
Console.WriteLine();

// Connection string - update this for your environment
var connectionString = "Server=(localdb)\\mssqllocaldb;Database=urguide_id4;Trusted_Connection=True;";

Console.WriteLine("Note: This script requires manual implementation.");
Console.WriteLine("For a working solution, use the C# console tool or PowerShell script.");
Console.WriteLine();
Console.WriteLine("To create an admin user:");
Console.WriteLine("1. Run: dotnet run --project UrGuide.AdminTool create-admin");
Console.WriteLine("   OR");
Console.WriteLine("2. Run: pwsh scripts/provision-admin-user.ps1");
Console.WriteLine();
