using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WhereClauseDynamicLinq;

List<Person> persons = new List<Person>
{
    new Person
    {
        Name = "Flamur", Surname = "Dauti", Age = 39,
        City = "Prishtine", IsHomeOwner = true, Salary = 12000.0
    },

    new Person
    {
        Name = "Blerta", Surname = "Frasheri", Age = 25,
        City = "Mitrovice", IsHomeOwner = false, Salary = 9000.0
    },
    
    new Person
    {
        Name = "SOngoku", Surname = "Frasheri", Age = 25,
        City = "Mitrovice", IsHomeOwner = false, Salary = 9000.0
    },

    new Person
    {
        Name = "Berat", Surname = "Dajti", Age = 45,
        City = "Peje", IsHomeOwner = true, Salary = 10000.0
    },

    new Person
    {
        Name = "Laura", Surname = "Morina", Age = 23,
        City = "Mitrovice", IsHomeOwner = true, Salary = 25000.0
    },

    new Person
    {
        Name = "Olti", Surname = "Kodra", Age = 19,
        City = "Prishtine", IsHomeOwner = false, Salary = 8000.0
    },

    new Person
    {
        Name = "Xhenis", Surname = "Berisha", Age = 26,
        City = "Gjakove", IsHomeOwner = false, Salary = 7000.0
    },

    new Person
    {
        Name = "Fatos", Surname = "Gashi", Age = 32,
        City = "Peje", IsHomeOwner = true, Salary = 6000.0
    },
};


List<Filter> filter = new List<Filter>
{
    new Filter {Property = "City", Value = "Mitrovice"},
    new Filter {Property = "IsHomeOwner", Value = false},
    new Filter {Property = "Salary", Value = 9000.0}
};

var deleg = PersonExpressionBuilder.Build(filter);
var filteredCollection = persons.Where(deleg).ToList();

Console.Write("");